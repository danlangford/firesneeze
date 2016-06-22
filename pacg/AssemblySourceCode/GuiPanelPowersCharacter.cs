using System;
using UnityEngine;

public class GuiPanelPowersCharacter : GuiPanel
{
    [Tooltip("reference to the powers panel in this scene")]
    public GuiPanelPowers powersPanel;

    private void OnCharacterPowerButton0Pushed()
    {
        this.powersPanel.OnCharacterPowerButtonPushed(0);
    }

    private void OnCharacterPowerButton1Pushed()
    {
        this.powersPanel.OnCharacterPowerButtonPushed(1);
    }

    private void OnCharacterPowerButton2Pushed()
    {
        this.powersPanel.OnCharacterPowerButtonPushed(2);
    }

    private void OnToggleButton0Pushed()
    {
        this.powersPanel.OnCharacterToggleButtonPushed(0);
    }

    private void OnToggleButton1Pushed()
    {
        this.powersPanel.OnCharacterToggleButtonPushed(1);
    }

    public override uint zIndex =>
        (base.zIndex + 20);
}

