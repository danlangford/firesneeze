using System;

public class GameStateTarget : GameState
{
    public static string DisplayText = string.Empty;

    public override void Cancel()
    {
        Turn.TargetType = TargetType.None;
        Turn.GotoCancelDestination();
        this.RefreshPanels();
    }

    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (!Party.Characters[i].Alive)
                {
                    Party.Characters[i].Active = ActiveType.Inactive;
                }
            }
            if ((Turn.TargetType == TargetType.AllAtLocation) || (Turn.TargetType == TargetType.MultipleAtLocation))
            {
                for (int k = 0; k < Party.Characters.Count; k++)
                {
                    if (Party.Characters[k].Location != Turn.Character.Location)
                    {
                        Party.Characters[k].Active = ActiveType.Inactive;
                    }
                }
            }
            else if (Turn.TargetType == TargetType.Another)
            {
                Turn.Character.Active = ActiveType.Inactive;
            }
            else if ((Turn.TargetType == TargetType.AnotherAtLocation) || (Turn.TargetType == TargetType.MultipleAnotherAtLocation))
            {
                for (int m = 0; m < Party.Characters.Count; m++)
                {
                    if (Party.Characters[m].Location != Turn.Character.Location)
                    {
                        Party.Characters[m].Active = ActiveType.Inactive;
                    }
                    Turn.Character.Active = ActiveType.Inactive;
                }
            }
            for (int j = 0; j < Party.Characters.Count; j++)
            {
                Party.Characters[j].Selected = false;
            }
            window.dicePanel.Show(false);
            window.ShowCancelButton(true);
            window.ShowProceedButton(false);
            window.targetPanel.Title = DisplayText;
            this.RefreshPanels();
            if (Turn.GetStateData() != null)
            {
                base.Message(Turn.GetStateData().Message);
            }
            else
            {
                base.Message(0x55);
            }
        }
    }

    public override void Exit(GameStateType nextState)
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Party.Characters[i].Active = ActiveType.Active;
        }
        base.Message((string) null);
        DisplayText = string.Empty;
        Turn.TargetType = TargetType.None;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Show(true);
            window.shadePanel.Show(window.shadePanel.TargetShade, false, 0f, 0.25f);
            window.partyPanel.ShowHighlights(false);
            window.targetPanel.Show(false);
        }
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        false;

    public override void Proceed()
    {
        Turn.TargetType = TargetType.None;
        Turn.EmptyLayoutDecks = false;
        Turn.GotoStateDestination();
        this.RefreshPanels();
    }

    public override void Refresh()
    {
        if ((Turn.TargetType == TargetType.MultipleAnotherAtLocation) || (Turn.TargetType == TargetType.MultipleAtLocation))
        {
            bool isVisible = false;
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (Party.Characters[i].Selected)
                {
                    isVisible = true;
                    break;
                }
            }
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ShowProceedButton(isVisible);
            }
        }
    }

    protected void RefreshPanels()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.shadePanel.Refresh();
            window.partyPanel.Refresh();
            window.targetPanel.Refresh();
        }
    }

    public override GameStateType Type =>
        GameStateType.Target;
}

