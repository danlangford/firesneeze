using System;
using UnityEngine;

public class EventDamageTakenPenalty : Event
{
    [Tooltip("number of penalty cards")]
    public int Amount = 1;
    [Tooltip("the minimum number of cards discarded as damage for this ability to trigger")]
    public int DiscardDamageMin = 2;
    [Tooltip("should this event count as a \"forced action\" for triggers?")]
    public bool Forced = true;
    [Tooltip("the type of deck to pick from")]
    public ActionType From;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("used for card filters during penalty state")]
    public CardSelector PickFilter;
    [Tooltip("position to add the cards in the deck")]
    public DeckPositionType Position;
    [Tooltip("the type of deck to send cards to")]
    public ActionType To;

    private void EventPlayerDamagedPick_Resolve()
    {
        TurnStateData data;
        if (this.To == ActionType.Recharge)
        {
            Turn.RechargePositionType = this.Position;
            if (this.Forced)
            {
                Turn.RechargeReason = GameReasonType.MonsterForced;
            }
        }
        if (this.PickFilter != null)
        {
            data = new TurnStateData(this.To, this.PickFilter.ToFilter(), this.Amount);
        }
        else
        {
            data = new TurnStateData(this.To, this.Amount);
        }
        data.Deck = this.From;
        data.Message = this.Message.ToString();
        Turn.SetStateData(data);
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "EventPlayerDamagePick_Finish"));
        Turn.State = GameStateType.Pick;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(false);
            window.ShowProceedButton(false);
        }
    }

    private void EventPlayerDamagePick_Finish()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.EmptyLayoutDecks = true;
            window.ProcessRechargableCards();
            window.ProcessLayoutDecks();
            window.ShowCancelButton(false);
            Turn.ClearCombatData();
            if (Turn.LastCombatResult == CombatResultType.None)
            {
                Event.DonePost(GameStateType.Combat);
            }
            else
            {
                Event.DonePost(GameStateType.Post);
            }
        }
    }

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (Turn.DamageDiscarded < this.DiscardDamageMin)
        {
            return false;
        }
        if (!Rules.IsCombatDamage(Turn.DamageTraits))
        {
            return false;
        }
        if (this.PickFilter != null)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if ((window != null) && (this.PickFilter.Filter(window.layoutDiscard.Deck) <= 1))
            {
                return false;
            }
        }
        return true;
    }

    public override void OnDamageTaken(Card card)
    {
        if (!this.IsEventValid(card))
        {
            Event.Done();
        }
        else
        {
            Turn.EmptyLayoutDecks = false;
            this.EventPlayerDamagedPick_Resolve();
        }
    }

    public override bool Stateless =>
        !this.IsEventValid(Turn.Card);

    public override EventType Type =>
        EventType.OnDamageTaken;
}

