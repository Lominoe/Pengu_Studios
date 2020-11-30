﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Range(0,10)]
    public int axisCount;
    [Range (0,20)]
    public int buttonCount;

    public Controller playerController;
    public void PassInput(InputData data)
    {
        playerController.ReadInput(data);
    }

    public void RefreshTracker()
    {
        DeviceTracker dt = GetComponent<DeviceTracker>();
        if (dt != null)
        {
            dt.Refresh();
        }
    }
}

public struct InputData
{
    public float[] axes;
    public bool[] buttons;

    public InputData(int axisCount, int buttonCount)
    {
        axes = new float[axisCount];
        buttons = new bool[buttonCount];
    }

    public void Reset()
    {
        for(int i = 0; i < axes.Length; i++)
        {
            axes[i] = 0f;
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = false;
        }
    }
}
