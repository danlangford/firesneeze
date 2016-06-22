using System;
using UnityEngine;

public class GuiPanelTutorial : GuiPanel
{
    [Tooltip("reference to the tutorial popup in our hierarchy")]
    public GuiPanelTutorialPopup Popup;

    public override void Show(bool isVisible)
    {
        if (!isVisible && (this.Popup != null))
        {
            this.Popup.Show(isVisible);
        }
        base.Show(isVisible);
    }

    public void Show(int id)
    {
        this.Show(true);
        if (this.Popup != null)
        {
            this.Popup.Display(id);
        }
    }

    public void Show(string msg)
    {
        this.Show(true);
        this.Popup.Display(msg);
    }
}

