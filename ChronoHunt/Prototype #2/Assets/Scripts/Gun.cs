﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{   
    //Shooting
    [SerializeField] enum FireMode {_Burst, _Single, _Auto}
    [SerializeField] FireMode _fireMode;
    int _burstCount = 3;
    int _shotsRemaingingInBurst;
    [SerializeField] Transform[] _muzzles;
    [SerializeField] Projectile _projectile;
    float _muzzleVelocity = 50f;
    public float nextShotTime = 2;
    [SerializeField]float _smoothTime = 5;
    public float shotTime = 2;
    Vector3 _recoilSmotherDampVel;
    Vector3 _vel;
    bool _triggerReleasedSinceLastShot;

    //recoil
    float _recoilStrength = 9;
    float _maxRecoil = 9;
    Player _player;

    //Reloading
    [SerializeField]int _projectilesPerMag;
    int _projectilesRemainingInMag;
    bool _isReloading;
    [SerializeField]float _reloadTime;
    [SerializeField] Transform _bolt;

    //shellEjection
    [SerializeField] Transform _shell;
    [SerializeField] Transform _shellEjection;

    //muzzleFlash
    Muzzleflash _muzzleFlash;


    void Start()
    {
        _shotsRemaingingInBurst = _burstCount;
        _muzzleFlash = GetComponent<Muzzleflash>();
        _player = FindObjectOfType<Player>();
        _projectilesRemainingInMag = _projectilesPerMag;
    }
    private void LateUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref _recoilSmotherDampVel, .3f);
        _bolt.localPosition = Vector3.SmoothDamp(_bolt.localPosition, new Vector3(.387f, -.008f, -.444f), ref _vel, .1f);
    }
    private void Update()
    {
        if (!_isReloading && _projectilesRemainingInMag == 0)
        {
            Reload();
        }
        if (nextShotTime >= 0)
        {
            nextShotTime -= Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _recoilStrength = 4;
        }
        else
        {
            _recoilStrength = _maxRecoil;
        }
    }
    public void Aim(Vector3 point)
    {
        Vector3 direction = point - _player.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _smoothTime * Time.deltaTime);
    }
    void Shoot()
    {
        if (!_isReloading && nextShotTime <= 0 && _projectilesRemainingInMag > 0)
        {
            if(_fireMode == FireMode._Burst)
            {
                if(_shotsRemaingingInBurst == 0)
                {
                    return;
                }
                _shotsRemaingingInBurst--;
            }
            else if (_fireMode == FireMode._Single)
            {
                if(!_triggerReleasedSinceLastShot)
                {
                    return;
                }
            }
            for(int i = 0; i < _muzzles.Length; i++)
            {
                nextShotTime = shotTime;
                Projectile newProjectile = Instantiate(_projectile, _muzzles[i].position, _muzzles[i].rotation) as Projectile;
                newProjectile.SetSpeed(_muzzleVelocity);
            }
            _projectilesRemainingInMag--;
            Instantiate(_shell, _shellEjection.position, _shellEjection.rotation);
            _muzzleFlash.Activate();
            transform.localPosition -= new Vector3(.5f,0,.5f) * Random.Range(.7f, 1);
            _player.rb.velocity = Vector3.Lerp(_player.rb.velocity, _player.rb.velocity - (_muzzles[0].transform.position - transform.position) * _recoilStrength, 1f);
        }
    }

    public void Reload()
    {
        StartCoroutine(AnimateReload());
    }
    IEnumerator AnimateReload()
    {
        _isReloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1 / _reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 10;
        float maxDrawBack = .07f;

        while(percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            float drawBack = Mathf.Lerp(0, maxDrawBack, interpolation);
            _bolt.localPosition = _bolt.localPosition - Vector3.right * drawBack;
            transform.localEulerAngles = initialRot + Vector3.forward * reloadAngle;

            yield return null;
        }
        _isReloading = false;
        _projectilesRemainingInMag = _projectilesPerMag;
    }

    public void OnTriggerHold()
    {
        Shoot();
        _triggerReleasedSinceLastShot = false;
    }
    public void OnTriggerRelease()
    {
        _triggerReleasedSinceLastShot = true;
        _shotsRemaingingInBurst = _burstCount;
    }
}
