using System;
using UnityEngine;

public class CardPowerRecover : CardPower
{
    [Tooltip("the position of the deck to place the card in")]
    public DeckPositionType DeckPosition;
    [Tooltip("the deck to pull the card from")]
    public DeckType From = DeckType.Discard;
    [Tooltip("the message to show when in the pick state")]
    public StrRefType Message;
    [Tooltip("who can be targeted by this power")]
    public TargetType Range;
    [Tooltip("if not null, may only recover types that match the selector")]
    public CardSelector RecoverSelector;
    [Tooltip("the deck to send the card to")]
    public DeckType To = DeckType.Hand;

    public override void Activate(Card card)
    {
        Turn.PushReturnState();
        Turn.EmptyLayoutDecks = false;
        if ((this.GetValidTargetCount() > 1) && Rules.IsTargetRequired(this.Range))
        {
            Turn.TargetType = this.Range;
            Turn.PushStateDestination(new TurnStateCallback(card, "Recover_Activate"));
            Turn.PushCancelDestination(new TurnStateCallback(card, "Recover_Cancel"));
            GameStateTarget.DisplayText = card.DisplayName;
            CardFilter filter = this.RecoverSelector.ToFilter();
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (Party.Characters[i].GetDeck(this.From).Filter(filter) <= 0)
                {
                    Party.Characters[i].Active = ActiveType.Inactive;
                }
            }
            Turn.State = GameStateType.Target;
        }
        else
        {
            Turn.Target = this.GetDefaultTarget();
            this.Recover_Activate();
        }
    }

    private int GetDefaultTarget()
    {
        CardFilter filter = this.RecoverSelector.ToFilter();
        switch (this.Range)
        {
            case TargetType.None:
                return Turn.Number;

            case TargetType.AllAtLocation:
                for (int i = 0; i < Party.Characters.Count; i++)
                {
                    if ((Party.Characters[i].Location == Turn.Character.Location) && (Party.Characters[i].GetDeck(this.From).Filter(filter) > 0))
                    {
                        return i;
                    }
                }
                break;

            case TargetType.Another:
                for (int j = 0; j < Party.Characters.Count; j++)
                {
                    if ((j != Turn.Number) && (Party.Characters[j].GetDeck(this.From).Filter(filter) > 0))
                    {
                        return j;
                    }
                }
                break;

            case TargetType.AnotherAtLocation:
                for (int k = 0; k < Party.Characters.Count; k++)
                {
                    if (((k != Turn.Number) && (Party.Characters[k].Location == Turn.Character.Location)) && (Party.Characters[k].GetDeck(this.From).Filter(filter) > 0))
                    {
                        return k;
                    }
                }
                break;
        }
        return Turn.Number;
    }

    private int GetValidTargetCount()
    {
        int num = 0;
        switch (this.Range)
        {
            case TargetType.None:
                if ((Turn.Character.GetDeck(this.From).Count >= 1) && (Turn.Character.GetDeck(this.From).Filter(this.RecoverSelector.ToFilter()) >= 1))
                {
                    num++;
                }
                return num;

            case TargetType.All:
                return num;

            case TargetType.AllAtLocation:
            {
                CardFilter filter3 = this.RecoverSelector.ToFilter();
                for (int i = 0; i < Party.Characters.Count; i++)
                {
                    if ((Party.Characters[i].Location == Turn.Character.Location) && (Party.Characters[i].GetDeck(this.From).Filter(filter3) >= 1))
                    {
                        num++;
                    }
                }
                return num;
            }
            case TargetType.Another:
            {
                CardFilter filter2 = this.RecoverSelector.ToFilter();
                for (int j = 0; j < Party.Characters.Count; j++)
                {
                    if ((j != Turn.Number) && (Party.Characters[j].GetDeck(this.From).Filter(filter2) >= 1))
                    {
                        num++;
                    }
                }
                return num;
            }
            case TargetType.AnotherAtLocation:
            {
                CardFilter filter = this.RecoverSelector.ToFilter();
                for (int k = 0; k < Party.Characters.Count; k++)
                {
                    if (((k != Turn.Number) && (Party.Characters[k].Location == Turn.Character.Location)) && (Party.Characters[k].GetDeck(this.From).Filter(filter) >= 1))
                    {
                        num++;
                    }
                }
                return num;
            }
        }
        return num;
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (this.GetValidTargetCount() <= 0)
        {
            return false;
        }
        if ((Turn.State != GameStateType.Finish) && (Turn.State != GameStateType.Setup))
        {
            return false;
        }
        return true;
    }

    public override bool IsValidationRequired() => 
        (this.GetValidTargetCount() <= 0);

    private void Recover_Activate()
    {
        Turn.SwitchCharacter(Turn.Target);
        CardFilter filter = this.RecoverSelector.ToFilter();
        TurnStateData data = new TurnStateData(this.From.ToActionType(), filter, this.To.ToActionType(), 1);
        if (!this.Message.IsNullOrEmpty())
        {
            data.Message = this.Message.ToString();
        }
        Turn.SetStateData(data);
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "Recover_Finish"));
        Turn.PushCancelDestination(new TurnStateCallback(base.Card, "Recover_Cancel"));
        Turn.State = GameStateType.Pick;
    }

    private void Recover_Cancel()
    {
        Turn.EmptyLayoutDecks = true;
        Turn.ReturnToReturnState();
    }

    private void Recover_Finish()
    {
        Turn.Current = Turn.InitialCharacter;
        Turn.SwitchCharacter(Turn.Current);
        Turn.EmptyLayoutDecks = true;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(false);
        }
        Turn.PushStateDestination(Turn.PopReturnState());
        Turn.State = GameStateType.Recharge;
    }
}

