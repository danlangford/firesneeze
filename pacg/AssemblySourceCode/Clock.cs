using System;
using UnityEngine;

public class Clock
{
    public static float ConvertFramesToSeconds(int frames)
    {
        float num = 1f / Time.deltaTime;
        return ((1f / num) * frames);
    }

    public static int ConvertSecondsToFrames(float seconds)
    {
        float num = 1f / Time.deltaTime;
        return Mathf.CeilToInt(num * seconds);
    }
}

