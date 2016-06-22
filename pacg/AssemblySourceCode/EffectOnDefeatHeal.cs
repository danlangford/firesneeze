using System;
using UnityEngine;

public class EffectOnDefeatHeal : Effect
{
    public EffectOnDefeatHeal(string source, int duration, CardFilter filter, int target, int healAmount, DeckType healFrom, DeckPositionType position) : base(source, duration, filter)
    {
        base.genericParameters = new int[4];
        this.Target = target;
        this.HealAmount = healAmount;
        this.HealFrom = healFrom;
        this.Position = position;
    }

    public override string GetDisplayText() => 
        StringTableManager.GetUIText(0x1eb);

    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Deck deck = Turn.Owner.GetDeck(this.HealFrom);
            deck.Shuffle();
            int num = Mathf.Min(this.HealAmount, deck.Count);
            Card[] cards = new Card[num];
            for (int i = 0; i < num; i++)
            {
                cards[i] = deck[i];
            }
            window.Heal(Party.Characters[this.Target], cards, this.Position);
        }
    }

    public int HealAmount
    {
        get => 
            base.genericParameters[1];
        set
        {
            base.genericParameters[1] = value;
        }
    }

    public DeckType HealFrom
    {
        get => 
            ((DeckType) base.genericParameters[2]);
        set
        {
            base.genericParameters[2] = (int) value;
        }
    }

    public DeckPositionType Position
    {
        get => 
            ((DeckPositionType) base.genericParameters[3]);
        set
        {
            base.genericParameters[3] = (int) value;
        }
    }

    public int Target
    {
        get => 
            base.genericParameters[0];
        set
        {
            base.genericParameters[0] = value;
        }
    }

    public override EffectType Type =>
        EffectType.OnDefeatHeal;
}

