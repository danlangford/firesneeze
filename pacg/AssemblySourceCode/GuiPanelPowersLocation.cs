using System;
using UnityEngine;

public class GuiPanelPowersLocation : GuiPanel
{
    [Tooltip("reference to the powers panel in this scene")]
    public GuiPanelPowers powersPanel;

    private void OnLocationPowerButton0Pushed()
    {
        this.powersPanel.OnLocationPowerButtonPushed(0);
    }

    private void OnLocationPowerButton1Pushed()
    {
        this.powersPanel.OnLocationPowerButtonPushed(1);
    }

    private void OnLocationPowerButton2Pushed()
    {
        this.powersPanel.OnLocationPowerButtonPushed(2);
    }

    private void OnScenarioPowerButton0Pushed()
    {
        this.powersPanel.OnScenarioPowerButtonPushed(0);
    }

    private void OnScenarioPowerButton1Pushed()
    {
        this.powersPanel.OnScenarioPowerButtonPushed(1);
    }

    private void OnScenarioPowerButton2Pushed()
    {
        this.powersPanel.OnScenarioPowerButtonPushed(2);
    }
}

