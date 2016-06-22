using System;
using UnityEngine;

public class AnimatonEventHeal : MonoBehaviour
{
    public void AnimationTriggerHeal()
    {
        GuiPanelHeal componentInParent = base.GetComponentInParent<GuiPanelHeal>();
        if (componentInParent != null)
        {
            componentInParent.OnAnimationTriggerHeal();
        }
    }
}

