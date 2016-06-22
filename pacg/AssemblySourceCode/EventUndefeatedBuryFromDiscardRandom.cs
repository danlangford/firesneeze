using System;
using UnityEngine;

public class EventUndefeatedBuryFromDiscardRandom : Event
{
    [Tooltip("the type of card to bury")]
    public CardType[] Cards = new CardType[1];
    [Tooltip("dice to roll to determine the amount")]
    public DiceType[] Dice;
    [Tooltip("bonus amount of cards to bury")]
    public int Number = 1;

    private void BuryCards(int amountToBury)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        int num = 0;
        if (window != null)
        {
            for (int i = 0; (i < Turn.Character.Discard.Count) && (num < amountToBury); i++)
            {
                Card card = Turn.Character.Discard[i];
                if (this.IsCardValid(card))
                {
                    window.Bury(card);
                    num++;
                }
            }
        }
        Event.Done();
    }

    private void EventUndefeatedBuryFromDiscardRandom_DiceFinish()
    {
        this.BuryCards(Turn.DiceTotal);
    }

    private bool IsCardValid(Card card)
    {
        if (this.Cards.Length == 0)
        {
            return true;
        }
        for (int i = 0; i < this.Cards.Length; i++)
        {
            if (card.Type == this.Cards[i])
            {
                return true;
            }
        }
        return false;
    }

    public override bool IsEventValid(Card card) => 
        base.IsConditionValid(card);

    public override void OnCardUndefeated(Card card)
    {
        if (this.IsEventValid(card))
        {
            VisualEffect.Shuffle(DeckType.Discard);
            Turn.Character.Discard.Shuffle();
            if (this.Dice.Length > 0)
            {
                Turn.RollReason = RollType.EnemyDamage;
                Turn.Dice.Clear();
                Turn.Dice.AddRange(this.Dice);
                Turn.DiceBonus = this.Number;
                if (this.IsCardEvent())
                {
                    Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventUndefeatedBuryFromDiscardRandom_DiceFinish"));
                }
                else
                {
                    Turn.PushStateDestination(new TurnStateCallback(this.CallbackType, "EventUndefeatedBuryFromDiscardRandom_DiceFinish"));
                }
                Turn.State = GameStateType.Roll;
            }
            else
            {
                this.BuryCards(this.Number);
            }
        }
    }

    public override bool Stateless =>
        (this.Dice.Length == 0);

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

