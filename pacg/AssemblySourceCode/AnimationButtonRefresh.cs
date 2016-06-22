using System;
using UnityEngine;

public class AnimationButtonRefresh : MonoBehaviour
{
    [Tooltip("The region buttons that were moved during this animation")]
    public GuiButtonRegion[] ButtonRegions;
    [Tooltip("The buttons that were moved during this animation")]
    public GuiButton[] Buttons;

    public void Done()
    {
        for (int i = 0; i < this.Buttons.Length; i++)
        {
            this.Buttons[i].Refresh();
        }
        for (int j = 0; j < this.ButtonRegions.Length; j++)
        {
            this.ButtonRegions[j].Refresh();
        }
    }
}

