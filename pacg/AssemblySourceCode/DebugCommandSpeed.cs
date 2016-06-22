using System;
using UnityEngine;

public class DebugCommandSpeed : DebugCommand
{
    [Tooltip("the max speed allowed. Unity only accepts value between 0-100")]
    public const float MAX_TIME_SPEED = 100f;
    [Tooltip("the min speed allowed. Unity only accepts values between 0-100 and if timescale is 0 you might not be able to reset the timescale again")]
    public const float MIN_TIME_SPEED = 0.01f;

    public override string Run(string[] args)
    {
        float timeScale = Time.timeScale;
        if (args.Length <= 1)
        {
            return base.Success("Current speed is : " + Time.timeScale);
        }
        if (!float.TryParse(args[1], out timeScale))
        {
            return base.Error("Could not parse : " + args[1]);
        }
        timeScale = Mathf.Clamp(timeScale, 0.001f, 100f);
        Time.timeScale = timeScale;
        return base.Success("Speed set to : " + timeScale);
    }

    public override string Command =>
        "speed";

    public override string HelpText =>
        "Syntax: speed optional:number";
}

