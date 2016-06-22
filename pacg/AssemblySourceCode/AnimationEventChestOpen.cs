using System;
using UnityEngine;

public class AnimationEventChestOpen : MonoBehaviour
{
    public void AnimationTriggerChestOpen()
    {
        base.StartCoroutine(GuiPanelStoreTreasureReveal.ChestOpened());
    }
}

