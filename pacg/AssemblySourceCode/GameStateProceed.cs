using System;
using System.Runtime.CompilerServices;

public class GameStateProceed : GameState
{
    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (Scenario)
            {
                window.powersPanel.ShowScenarioPowerProceedButton(true);
            }
            else
            {
                window.powersPanel.ShowLocationPowerProceedButton(true);
            }
            window.shadePanel.Show(window.shadePanel.LocationShade, true);
        }
    }

    public override void Exit(GameStateType nextState)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (Scenario)
            {
                window.powersPanel.ShowScenarioPowerProceedButton(false);
            }
            else
            {
                window.powersPanel.ShowLocationPowerProceedButton(false);
            }
            window.shadePanel.Show(window.shadePanel.LocationShade, false);
        }
    }

    public override void Proceed()
    {
        Turn.GotoStateDestination();
    }

    public static bool Scenario
    {
        [CompilerGenerated]
        get => 
            <Scenario>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Scenario>k__BackingField = value;
        }
    }

    public override GameStateType Type =>
        GameStateType.Proceed;
}

