using System;

public class GameStateClose : GameState
{
    private void CloseLocation(bool isClosed)
    {
        this.GlowText(TextHilightType.None);
        if (Turn.CloseType == CloseType.Temporary)
        {
            Location.Current.Closed = isClosed;
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "Iterator_Next"));
            Turn.State = GameStateType.Recharge;
        }
        else if (isClosed)
        {
            if (Turn.State == GameStateType.Close)
            {
                Turn.PushStateDestination(GameStateType.Finish);
            }
            else
            {
                Turn.PushStateDestination(Turn.State);
            }
            Turn.PushStateDestination(GameStateType.Recharge);
            Turn.State = GameStateType.Closing;
        }
        else
        {
            if (Turn.Close)
            {
                Turn.CloseType = CloseType.None;
                Turn.Close = false;
            }
            Location.Current.OnLocationCloseAttempted();
            if (Turn.State == GameStateType.Close)
            {
                Turn.PushStateDestination(GameStateType.Done);
                Turn.State = GameStateType.Recharge;
            }
        }
    }

    public void CloseViaBanish()
    {
        if (!Location.Current.IsClosePossible(Turn.Owner))
        {
            this.CloseLocation(false);
        }
        else
        {
            Turn.BlackBoard.Set<int>("CloseLocationMenuChoice", 1);
            Turn.SetStateData(new TurnStateData(ActionType.Banish, Location.Current.Banish.ToFilter(), 1));
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "EventLocationCloseBanish_Finish"));
            if (Turn.CloseType == CloseType.Temporary)
            {
                Turn.PushCancelDestination(new TurnStateCallback(TurnStateCallbackType.Global, "Temp_CancelClose"));
            }
            else
            {
                Turn.PushCancelDestination(GameStateType.ClosePrompt);
            }
            Turn.State = GameStateType.Penalty;
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ShowCancelButton(true);
            }
            this.GlowText(TextHilightType.WhenClosing);
        }
    }

    public void CloseViaBlock()
    {
        if (Location.Current.BlockClose != null)
        {
            Location.Current.BlockClose.Invoke();
        }
        this.GlowText(TextHilightType.WhenClosing);
    }

    public void CloseViaChecks()
    {
        Turn.BlackBoard.Set<int>("CloseLocationMenuChoice", 2);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(false);
            window.dicePanel.Show(true);
            this.SetupDicePanel(window.dicePanel);
        }
        this.GlowText(TextHilightType.WhenClosing);
    }

    public void CloseViaEncounter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (Location.Current.Deck.Count > 0)
            {
                window.layoutLocation.Show(true);
                Turn.EncounterType = EncounterType.Close;
                Turn.State = GameStateType.Encounter;
                this.GlowText(TextHilightType.WhenClosing);
                window.layoutLocation.Refresh();
            }
            else
            {
                this.CloseLocation(true);
            }
        }
    }

    public void CloseViaMenu()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Popup.Clear();
            if (this.IsClosedViaBanish())
            {
                TurnStateCallback callback = new TurnStateCallback(TurnStateCallbackType.Global, "EventLocationCloseAsk_Banish");
                string helperText = StringTableManager.GetHelperText(0x40);
                window.Popup.Add(helperText, callback);
            }
            if (this.IsClosedViaChecks())
            {
                TurnStateCallback callback2 = new TurnStateCallback(TurnStateCallbackType.Global, "EventLocationCloseAsk_Checks");
                string text = StringTableManager.GetHelperText(0x41);
                window.Popup.Add(text, callback2);
            }
            if (this.IsClosedViaSummons())
            {
                TurnStateCallback callback3 = new TurnStateCallback(TurnStateCallbackType.Global, "EventLocationCloseAsk_Summons");
                string str3 = StringTableManager.GetHelperText(0x42);
                window.Popup.Add(str3, callback3);
            }
            window.Popup.SetDeckPosition(DeckType.Location);
            Turn.State = GameStateType.Popup;
            this.GlowText(TextHilightType.WhenClosing);
        }
    }

    public void CloseViaSummons()
    {
        Turn.BlackBoard.Set<int>("CloseLocationMenuChoice", 3);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card card = Location.Current.Summons.Summon();
            if (card != null)
            {
                Turn.SummonsType = SummonsType.Close;
                Location.Current.Deck.Add(card, DeckPositionType.Top);
                window.layoutLocation.Show(true);
                window.layoutLocation.Display();
                Turn.BlackBoard.Set<bool>("CharacterPowerExamine_Encounter", true);
                Turn.State = GameStateType.Encounter;
                this.GlowText(TextHilightType.WhenClosing);
            }
            else
            {
                this.CloseLocation(true);
            }
        }
    }

    public override void Enter()
    {
        base.Enter();
        switch (Turn.BlackBoard.Get<int>("CloseLocationMenuChoice"))
        {
            case 1:
                this.CloseViaBanish();
                return;

            case 2:
                this.CloseViaChecks();
                return;

            case 3:
                this.CloseViaSummons();
                return;
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (this.IsClosedViaMenu())
            {
                this.CloseViaMenu();
            }
            else if (this.IsClosedViaChecks())
            {
                this.CloseViaChecks();
            }
            else if (this.IsClosedViaSummons())
            {
                this.CloseViaSummons();
            }
            else if (this.IsClosedViaBanish())
            {
                this.CloseViaBanish();
            }
            else if (this.IsClosedViaBlock())
            {
                this.CloseViaBlock();
            }
            else if (this.IsClosedViaEncounter())
            {
                this.CloseViaEncounter();
            }
            else
            {
                this.CloseLocation(true);
            }
        }
    }

    public override void Exit(GameStateType nextState)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (((nextState != GameStateType.Roll) && (nextState != GameStateType.Penalty)) && ((nextState != GameStateType.Power) && (nextState != GameStateType.Sacrifice)))
            {
                window.dicePanel.Clear();
                Turn.ClearCheckData();
            }
            window.closeLocationPanel.Show(false);
        }
        if (nextState != GameStateType.Power)
        {
            Turn.BlackBoard.Set<int>("CloseLocationMenuChoice", 0);
        }
    }

    private void GlowText(TextHilightType hilightType)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.locationPanel.GlowText(hilightType);
        }
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        card.IsActionAllowed(action);

    private bool IsClosedViaBanish() => 
        (Location.Current.Banish != null);

    private bool IsClosedViaBlock() => 
        (Location.Current.BlockClose != null);

    private bool IsClosedViaChecks() => 
        (Location.Current.Checks.Length > 0);

    private bool IsClosedViaEncounter() => 
        Location.Current.Encounter;

    private bool IsClosedViaMenu()
    {
        int num = 0;
        if (this.IsClosedViaSummons())
        {
            num++;
        }
        if (this.IsClosedViaChecks())
        {
            num++;
        }
        if (this.IsClosedViaBanish() && (Location.Current.Banish.Filter(Turn.Character.Hand) > 0))
        {
            num++;
        }
        return (num > 1);
    }

    private bool IsClosedViaSummons() => 
        (Location.Current.Summons != null);

    public override bool IsResolveSuccess() => 
        (Turn.DiceTotal >= Turn.DiceTarget);

    public override void Proceed()
    {
        if (Turn.Defeat)
        {
            Party.OnCheckCompleted();
            this.CloseLocation(true);
        }
    }

    public override void Resolve()
    {
        base.Resolve();
        this.CloseLocation(this.IsResolveSuccess());
    }

    private void SetupDicePanel(GuiPanelDice dicePanel)
    {
        SkillCheckValueType bestSkillCheck = Turn.Character.GetBestSkillCheck(Location.Current.Checks);
        dicePanel.SetCheck(Location.Current.Card, Location.Current.Checks, bestSkillCheck.skill);
        base.ShowAidButton();
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Close;
}

