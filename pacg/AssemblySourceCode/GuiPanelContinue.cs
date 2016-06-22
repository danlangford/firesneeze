using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelContinue : GuiPanel
{
    [Tooltip("reference to the continue button on this panel")]
    public GuiButton ContinueButton;

    private void OnContinueButtonPushed()
    {
        this.Show(false);
        if (this.Callback != null)
        {
            this.Callback.Invoke();
        }
        this.Callback = null;
    }

    public override void Rebind()
    {
        this.ContinueButton.Rebind();
    }

    public TurnStateCallback Callback { get; set; }

    public override bool Fullscreen =>
        true;
}

