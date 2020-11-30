﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardTracker : DeviceTracker
{
    public AxisKeys[] axisKeys;
    public KeyCode[] buttonKeys;

    void Reset()
    {
        im = GetComponent<InputManager>();
        axisKeys = new AxisKeys[im.axisCount];
        buttonKeys = new KeyCode[im.buttonCount];
    }

    public override void Refresh()
    {
        im = GetComponent<InputManager>();
        KeyCode[] _newButtons = new KeyCode[im.buttonCount];
        AxisKeys[] _newAxes = new AxisKeys[im.axisCount];
        if (buttonKeys != null)
        {
            for (int i = 0; i < Mathf.Min(_newButtons.Length, buttonKeys.Length); i++)
            {
                _newButtons[i] = buttonKeys[i];
            }
            buttonKeys = _newButtons;
        }
        if (axisKeys != null)
        {
            for (int i = 0; i < Mathf.Min(_newAxes.Length, axisKeys.Length); i++)
            {
                _newAxes[i] = axisKeys[i];
            }
            axisKeys = _newAxes;
        }
    }

    void Update()
    {
        //Check for inputs, if inputs detected, set newData to true
        //populate InputData to pass to the InputManager
        for (int i = 0; i < axisKeys.Length; i++)
        {
            float val = 0f;
            if (Input.GetKey(axisKeys[i].positive))
            {
                val += 1f;
                newData = true;
            }
            if (Input.GetKey(axisKeys[i].negative))
            {
                val -= 1f;
                newData = true;
            }
            data.axes[i] = val;
        }

        for (int i = 0; i < buttonKeys.Length; i++)
        {
            if (Input.GetKeyDown(buttonKeys[i]))
            {
                data.buttons[i] = true;
                newData = true;
            }

        }

        if (newData)
        {
            im.PassInput(data);
            newData = false;
            data.Reset();
        }
    }
}

[System.Serializable]
public struct AxisKeys
{
    public KeyCode positive;
    public KeyCode negative;
}
