using System;
using UnityEngine;

public class AnimationEventHide : MonoBehaviour
{
    [Tooltip("The buttons that will be hidden")]
    public GuiButton[] Buttons;

    public void AnimationTriggerHide()
    {
        for (int i = 0; i < this.Buttons.Length; i++)
        {
            this.Buttons[i].Show(false);
        }
    }
}

