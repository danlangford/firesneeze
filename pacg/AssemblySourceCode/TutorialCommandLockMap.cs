using System;
using UnityEngine;

public class TutorialCommandLockMap : TutorialCommand
{
    [Tooltip("if true, ignore the list and lock everything")]
    public bool LockAll;
    [Tooltip("list of icons to be locked (everything else will be unlocked)")]
    public string[] LockedIcons;

    public override void Invoke()
    {
        ScenarioMapIcon[] iconArray = UnityEngine.Object.FindObjectsOfType<ScenarioMapIcon>();
        for (int i = 0; i < iconArray.Length; i++)
        {
            iconArray[i].Locked = false;
        }
        for (int j = 0; j < iconArray.Length; j++)
        {
            for (int k = 0; k < this.LockedIcons.Length; k++)
            {
                if ((iconArray[j].ID == this.LockedIcons[k]) || this.LockAll)
                {
                    iconArray[j].Locked = true;
                    break;
                }
            }
        }
    }
}

