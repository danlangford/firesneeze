using System;
using System.IO;
using UnityEngine;

public class Device
{
    public static Vector3 GetCardZoomScale()
    {
        if (GetScreenProfile() == DeviceScreenType.TabletLow)
        {
            return new Vector3(0.9f, 0.9f, 1f);
        }
        return new Vector3(0.75f, 0.75f, 1f);
    }

    public static int GetCpuProfile()
    {
        if (Application.isEditor)
        {
            return 2;
        }
        return SystemInfo.processorCount;
    }

    public static string GetDeviceName() => 
        SystemInfo.deviceModel;

    public static string GetDocumentFolderPath()
    {
        if (Application.isEditor)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PACG1_SAVE_GAMES");
        }
        return Path.Combine(Application.persistentDataPath, "PACG1_SAVE_GAMES");
    }

    public static bool GetIsAndroid()
    {
        if (Application.isEditor)
        {
            return false;
        }
        return true;
    }

    public static bool GetIsApplicationGenuine() => 
        true;

    public static bool GetIsBackButtonPushed() => 
        (GetIsAndroid() && Input.GetKeyDown(KeyCode.Escape));

    public static bool GetIsIphone() => 
        (Application.isEditor && false);

    public static bool GetIsWindowsPhone() => 
        (Application.isEditor && false);

    public static float GetMarginLeft() => 
        0f;

    public static float GetMarginRight() => 
        1f;

    public static float GetMarginSize()
    {
        float num = ((float) Screen.width) / ((float) Screen.height);
        if (num >= 1.7f)
        {
            return 0.125f;
        }
        if (num >= 1.6f)
        {
            return 0.09f;
        }
        return 0f;
    }

    public static int GetMemoryProfile() => 
        2;

    public static float GetMinimumDragDistance()
    {
        if (!GetIsIphone() && !GetIsAndroid())
        {
            return 0f;
        }
        return 0.15f;
    }

    public static DeviceScreenType GetScreenProfile()
    {
        if ((Screen.height > 800) && (Screen.width > 0x4b0))
        {
            return DeviceScreenType.TabletHigh;
        }
        return DeviceScreenType.TabletLow;
    }
}

