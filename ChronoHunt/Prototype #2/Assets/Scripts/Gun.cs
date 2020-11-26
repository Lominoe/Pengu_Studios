﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;
    public Projectile projectile;
    float muzzleVelocity = 50f;
    public float nextShotTime = 2;
    Player player;
    [SerializeField]float smoothTime = 5;
    float recoilStrength = 11;
    float maxRecoil = 11;
    float displacement = .1f;
    float maxDisplacement;
    public float shotTime = 2;

    void Start()
    {
        player = FindObjectOfType<Player>();
        maxDisplacement = displacement;
    }
    void FixedUpdate()
    {
        transform.localPosition = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            recoilStrength = 4;
            displacement = 0;
        }
        else
        {
            recoilStrength = maxRecoil;
            displacement = maxDisplacement;
        }
        if (nextShotTime >= 0)
        {
            nextShotTime -= Time.deltaTime;
        }
    }
    public void Aim(Vector3 point)
    {
        Vector3 direction = point - player.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothTime * Time.deltaTime);
    }
    public void Shoot()
    {
        if (nextShotTime <= 0)
        {
            nextShotTime = shotTime;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);
            player.transform.position = player.transform.position + new Vector3(0,displacement, 0);
            player.rb.velocity = Vector3.Lerp(player.rb.velocity, player.rb.velocity - (muzzle.transform.position - transform.position) * recoilStrength, 1f);
        }
    }
}
