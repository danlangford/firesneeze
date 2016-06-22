using System;
using UnityEngine;

public class CardPowerMovePlayer : CardPower
{
    [Tooltip("should this move power auto-succeed the location's move restrictions?")]
    public bool BypassMoveRestrictions;
    [Tooltip("can this power be used only during the end of the turn?")]
    public bool EndOfTurn;
    [Tooltip("can this power cause an evade then move shuffling the enemy?")]
    public bool Evade;
    [Tooltip("should this move power ignore the location's move restrictions?")]
    public bool IgnoreMoveRestrictions;
    [Tooltip("message displayed when moving")]
    public StrRefType Message;
    [Tooltip("short = characters at location, long = any active character")]
    public TargetType Range;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            TurnStateCallback.SetActiveCallback(this);
            if (this.Evade)
            {
                Turn.Validate = true;
            }
            card.SetPowerInfo(card.GetIndexOf(this), card);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                if (this.Evade)
                {
                    window.mapPanel.Mode = MapModeType.EvadeThenChoose;
                }
                else if (this.IgnoreMoveRestrictions && ((this.Range == TargetType.MultipleAnotherAtLocation) || (this.Range == TargetType.MultipleAtLocation)))
                {
                    window.mapPanel.Mode = MapModeType.MoveMultipleIgnoreRestrictions;
                }
                else
                {
                    window.mapPanel.Mode = MapModeType.Choose;
                }
            }
            Turn.Owner.MarkCardType(card.Type, true);
            Turn.PushCancelDestination(new TurnStateCallback(this, "MovePlayer_Cancel"));
            Turn.PushReturnState();
            if (this.NumberOfValidTargets(this.Range) > 1)
            {
                this.MovePlayer_Target(card);
            }
            else
            {
                Turn.Target = this.SelectDefaultTarget();
                Party.Characters[Turn.Target].Selected = true;
                this.MovePlayer_Move();
            }
        }
    }

    public override void Deactivate(Card card)
    {
        if (TurnStateCallback.IsCallbackRunning(this))
        {
            TurnStateCallback.SetActiveCallback(null);
            Turn.PopStateDestination();
            Turn.PopCancelDestination();
            if ((Turn.PeekStateDestination() != null) && (Turn.PeekCancelDestination() == null))
            {
                Turn.PopStateDestination();
            }
            Turn.Owner.MarkCardType(card.Type, false);
            card.RemovePowerInfo(card.GetIndexOf(this), card);
            card.ReturnCard(card);
            Turn.Evade = false;
            Turn.Defeat = false;
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ShowMap(false);
                window.mapPanel.Mode = MapModeType.Move;
                window.mapPanel.LockMapIcon(Party.Characters[Turn.Target].Location, false);
                window.messagePanel.Clear();
                Location.Load(Turn.Owner.Location);
                if (Turn.ReturnState == GameStateType.Setup)
                {
                    Turn.State = Turn.PopReturnState();
                    Turn.Explore = true;
                    window.ShowExploreButton(true);
                    window.ShowProceedButton(false);
                }
                else if (Turn.ReturnState == GameStateType.Combat)
                {
                    Turn.PushStateDestination(Turn.PopReturnState());
                    Turn.State = GameStateType.Recharge;
                    Turn.Explore = false;
                    Turn.Card.Show(true);
                    window.dicePanel.Refresh();
                    window.ShowProceedButton(false);
                }
                else if ((Turn.ReturnState == GameStateType.Horde) || (Turn.ReturnState == GameStateType.EvadeOption))
                {
                    Turn.PushStateDestination(Turn.PopReturnState());
                    Turn.State = GameStateType.Recharge;
                    Turn.Explore = false;
                    Turn.Card.Show(true);
                    if (Turn.ReturnState == GameStateType.Horde)
                    {
                        window.ShowProceedButton(true);
                    }
                    else
                    {
                        window.ShowEncounterButton(true);
                    }
                }
                else
                {
                    Turn.PushStateDestination(Turn.PopReturnState());
                    Turn.State = GameStateType.Recharge;
                    Turn.Explore = false;
                    if (Turn.Owner.IsOverHandSize())
                    {
                        window.ShowProceedDiscardButton(true);
                    }
                    else
                    {
                        window.ShowProceedEndButton(true);
                    }
                }
                window.ShowCancelButton(false);
            }
        }
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (!base.IsAidPossible(card))
        {
            return false;
        }
        if (Turn.Owner.IsCardTypeMarked(card.Type))
        {
            return false;
        }
        if (this.NumberOfValidTargets(this.Range) <= 0)
        {
            return false;
        }
        if (this.Evade && !Rules.IsEvadePossible(card))
        {
            return false;
        }
        if (this.Evade && (Turn.State == GameStateType.Horde))
        {
            return true;
        }
        if (this.EndOfTurn)
        {
            return (Turn.State == GameStateType.EndTurn);
        }
        return ((((Turn.State == GameStateType.Finish) || (Turn.State == GameStateType.Setup)) || ((Turn.State == GameStateType.Combat) || (Turn.State == GameStateType.EvadeOption))) || (Turn.State == GameStateType.Horde));
    }

    public override bool IsValidationRequired() => 
        false;

    private void MovePlayer_Activate()
    {
        if (TurnStateCallback.IsCallbackRunning(this))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                if (Turn.State == GameStateType.Combat)
                {
                    int target = Turn.Target;
                    Turn.ClearCheckData();
                    Turn.ClearCombatData();
                    window.dicePanel.Clear();
                    Turn.Card.Show(false);
                    Turn.Evade = true;
                    Turn.Target = target;
                }
                else if (Turn.State == GameStateType.EvadeOption)
                {
                    Turn.Card.Show(false);
                    Turn.Evade = true;
                }
                else if (Turn.State == GameStateType.Horde)
                {
                    Turn.Card.Show(false);
                    Turn.Evade = true;
                }
                else if (Turn.State == GameStateType.Setup)
                {
                    Turn.Explore = true;
                }
                else if (this.EndOfTurn)
                {
                    Turn.Explore = false;
                }
                Turn.Validate = true;
                Turn.EmptyLayoutDecks = false;
                Location.Load(Party.Characters[Turn.Target].Location);
                Turn.PushStateDestination(new TurnStateCallback(this, "MovePlayer_Proceed"));
                Location.Current.Move();
                window.ShowCancelButton(true);
                window.messagePanel.Show(this.Message.ToString());
                window.mapPanel.LockMapIcon(Party.Characters[Turn.Target].Location, true);
                Turn.State = GameStateType.Null;
            }
        }
    }

    private void MovePlayer_Cancel()
    {
        if (TurnStateCallback.IsCallbackRunning(this))
        {
            this.Deactivate(base.Card);
            TurnStateCallback.SetActiveCallback(null);
        }
    }

    private void MovePlayer_Move()
    {
        if (TurnStateCallback.IsCallbackRunning(this))
        {
            if (this.BypassMoveRestrictions)
            {
                Turn.Defeat = true;
            }
            this.MovePlayer_Activate();
        }
    }

    private void MovePlayer_Proceed()
    {
        if (TurnStateCallback.IsCallbackRunning(this))
        {
            Turn.PopCancelDestination();
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ProcessRechargableCards();
                window.ProcessLayoutDecks();
                window.messagePanel.Clear();
                if (Turn.Current != Turn.InitialCharacter)
                {
                    Turn.Current = Turn.InitialCharacter;
                }
                Turn.Number = Turn.Current;
                window.Refresh();
                window.ShowMap(false);
                window.messagePanel.Clear();
                window.mapPanel.Mode = MapModeType.Move;
            }
            Turn.Card.Show(false);
            if (Turn.Evade)
            {
                Turn.NumCombatEvades++;
                Turn.Evade = false;
                if (Turn.State == GameStateType.Horde)
                {
                    Turn.Iterators.Remove(TurnStateIteratorType.Horde);
                }
            }
            if (this.EndOfTurn)
            {
                Turn.PushStateDestination(GameStateType.EndTurn);
                Turn.PopReturnState();
            }
            else if (Turn.ReturnState != GameStateType.Setup)
            {
                Turn.LastCombatResult = CombatResultType.None;
                Turn.Explore = false;
                Turn.PushStateDestination(GameStateType.Done);
                Turn.PopReturnState();
            }
            else
            {
                Turn.PushStateDestination(Turn.PopReturnState());
            }
            Turn.Defeat = false;
            Location.Load(Turn.Owner.Location);
            Turn.State = GameStateType.Recharge;
            UI.Window.Refresh();
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                Party.Characters[i].Selected = false;
            }
            TurnStateCallback.SetActiveCallback(null);
        }
    }

    private void MovePlayer_Target(Card card)
    {
        Turn.TargetType = this.Range;
        Turn.EmptyLayoutDecks = false;
        Turn.PushStateDestination(new TurnStateCallback(this, "MovePlayer_Move"));
        GameStateTarget.DisplayText = base.Card.DisplayName;
        if (!this.IgnoreMoveRestrictions)
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (!Party.Characters[i].CanMove)
                {
                    Party.Characters[i].Active = ActiveType.Inactive;
                }
            }
        }
        Turn.State = GameStateType.Target;
    }

    private int NumberOfValidTargets(TargetType range)
    {
        int num = 0;
        switch (range)
        {
            case TargetType.None:
                return 1;

            case TargetType.All:
                for (int i = 0; i < Party.Characters.Count; i++)
                {
                    if (Party.Characters[i].Alive && Party.Characters[i].CanMove)
                    {
                        num++;
                    }
                }
                return num;

            case TargetType.AllAtLocation:
            case TargetType.MultipleAtLocation:
                for (int j = 0; j < Party.Characters.Count; j++)
                {
                    if (((Party.Characters[j].Location == Turn.Character.Location) && Party.Characters[j].Alive) && Party.Characters[j].CanMove)
                    {
                        num++;
                    }
                }
                return num;

            case TargetType.Another:
                for (int k = 0; k < Party.Characters.Count; k++)
                {
                    if ((Party.Characters[k].Alive && (Party.Characters[k] != Turn.Character)) && Party.Characters[k].CanMove)
                    {
                        num++;
                    }
                }
                return num;

            case TargetType.AnotherAtLocation:
            case TargetType.MultipleAnotherAtLocation:
                for (int m = 0; m < Party.Characters.Count; m++)
                {
                    if (((Party.Characters[m].Location == Turn.Character.Location) && Party.Characters[m].Alive) && ((Party.Characters[m] != Turn.Character) && Party.Characters[m].CanMove))
                    {
                        num++;
                    }
                }
                return num;
        }
        return num;
    }

    private int SelectDefaultTarget()
    {
        switch (this.Range)
        {
            case TargetType.AllAtLocation:
            case TargetType.MultipleAtLocation:
                for (int i = 0; i < Party.Characters.Count; i++)
                {
                    if (Party.Characters[i].Location == Turn.Owner.Location)
                    {
                        return i;
                    }
                }
                break;

            case TargetType.Another:
                for (int j = 0; j < Party.Characters.Count; j++)
                {
                    if (Party.Characters[j].ID != Turn.Owner.ID)
                    {
                        return j;
                    }
                }
                break;

            case TargetType.AnotherAtLocation:
            case TargetType.MultipleAnotherAtLocation:
                for (int k = 0; k < Party.Characters.Count; k++)
                {
                    if ((Party.Characters[k].ID != Turn.Owner.ID) && (Party.Characters[k].Location == Turn.Owner.Location))
                    {
                        return k;
                    }
                }
                break;
        }
        return Turn.Current;
    }

    public override PowerType Type
    {
        get
        {
            if (this.Evade)
            {
                return PowerType.Evade;
            }
            return base.Type;
        }
    }
}

