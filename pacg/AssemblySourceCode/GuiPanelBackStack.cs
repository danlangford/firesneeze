using System;
using UnityEngine;

public abstract class GuiPanelBackStack : GuiPanel
{
    protected GuiPanelBackStack()
    {
    }

    protected virtual void Update()
    {
        if (Device.GetIsBackButtonPushed())
        {
            base.SendMessage("OnCloseButtonPushed", SendMessageOptions.DontRequireReceiver);
        }
    }
}

