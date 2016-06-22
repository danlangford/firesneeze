using System;
using UnityEngine;

public class TutorialConditionExperienced : TutorialCondition
{
    [Tooltip("minimum amount of xp gained")]
    public int Gain = 1;

    public override bool Evaluate()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].XPX >= this.Gain)
            {
                return true;
            }
        }
        return false;
    }
}

