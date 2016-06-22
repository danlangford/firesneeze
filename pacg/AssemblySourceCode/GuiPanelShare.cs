using System;
using UnityEngine;

public class GuiPanelShare : GuiPanel
{
    [Tooltip("reference to the share layout in this hierarchy")]
    public GuiLayoutShare Layout;

    private void OnCloseButtonPushed()
    {
        this.Show(false);
    }

    public override void Show(bool isVisible)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (isVisible)
        {
            this.Layout.Hand = Turn.Character.Hand;
            window.layoutLocation.gameObject.SetActive(false);
            this.Layout.Show(true);
        }
        else
        {
            window.layoutLocation.gameObject.SetActive(true);
            this.Layout.Show(false);
        }
    }
}

