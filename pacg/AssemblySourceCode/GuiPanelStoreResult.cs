using System;
using UnityEngine;

public class GuiPanelStoreResult : GuiPanelBackStack
{
    [Tooltip("reference to the store manager window in the scene")]
    public GuiWindowStore StoreManager;
    private TKTapRecognizer tapRecognizer;

    public override void Initialize()
    {
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.zIndex = 3;
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
        this.Show(false);
    }

    private void OnCloseButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            this.Show(false);
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.tapRecognizer.enabled = isVisible;
        if (isVisible)
        {
            this.Refresh();
        }
        else
        {
            UI.Busy = false;
        }
    }
}

