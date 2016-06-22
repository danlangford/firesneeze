using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Timer
{
    private bool isRunning;

    public Timer()
    {
        this.TimeInSeconds = 0f;
        this.isRunning = false;
    }

    public static int ConvertSecondsToFrames(float seconds)
    {
        int targetFrameRate = Application.targetFrameRate;
        if (targetFrameRate <= 0)
        {
            targetFrameRate = 60;
        }
        return Mathf.CeilToInt(seconds * targetFrameRate);
    }

    public void Pause(bool pause)
    {
        this.isRunning = !pause;
    }

    public void Start(int minutes)
    {
        this.TimeInSeconds = minutes * Application.targetFrameRate;
        this.isRunning = true;
    }

    public int Stop()
    {
        this.isRunning = false;
        return this.TimeInMinutes;
    }

    public void Tick(float deltaTime)
    {
        if (this.isRunning)
        {
            this.TimeInSeconds += deltaTime;
        }
    }

    public int TimeInMinutes =>
        Mathf.CeilToInt(this.TimeInSeconds / 60f);

    public float TimeInSeconds { get; private set; }
}

