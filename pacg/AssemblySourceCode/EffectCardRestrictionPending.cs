using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EffectCardRestrictionPending : Effect
{
    private bool invoked;

    public EffectCardRestrictionPending(string source, int duration, SkillCheckValueType[] checks, int msg, CardFilter filter) : base(source, duration, checks, filter)
    {
        base.genericParameters = new int[1];
        this.Message = msg;
    }

    private void CancelAllPowers()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            List<Card> list = new List<Card>();
            for (int i = 0; i < window.layoutDiscard.Deck.Count; i++)
            {
                list.Add(window.layoutDiscard.Deck[i]);
            }
            for (int j = 0; j < window.layoutBury.Deck.Count; j++)
            {
                list.Add(window.layoutBury.Deck[j]);
            }
            for (int k = 0; k < window.layoutRecharge.Deck.Count; k++)
            {
                list.Add(window.layoutRecharge.Deck[k]);
            }
            window.CancelAllPowers(true, false);
            Turn.Dice.Clear();
            for (int m = 0; m < list.Count; m++)
            {
                Card card = list[m];
                for (int n = 0; n < Party.Characters.Count; n++)
                {
                    if (Party.Characters[n].Hand.Contains(card))
                    {
                        Party.Characters[n].Hand.Remove(card);
                        break;
                    }
                }
                UnityEngine.Object.Destroy(card);
            }
            list.Clear();
        }
    }

    public override string GetCardDecoration(Card card)
    {
        if ((base.filter != null) && base.filter.Match(card))
        {
            return "Blueprints/Gui/Vfx_Card_Restricted";
        }
        return null;
    }

    public override string GetDisplayText() => 
        "Card Restriction";

    public override int GetHashCode() => 
        (base.GetHashCode() ^ base.filter.GetHashCode());

    public static Card GetRestrictedCard()
    {
        string str = Turn.BlackBoard.Get<string>("EffectCardRestriction_Card");
        Character restrictedCharacter = GetRestrictedCharacter();
        if (restrictedCharacter != null)
        {
            return restrictedCharacter.Hand[str];
        }
        return null;
    }

    public static Character GetRestrictedCharacter()
    {
        int num = Turn.BlackBoard.Get<int>("EffectCardRestriction_Number");
        return Party.Characters[num];
    }

    public void Invoke(CharacterPower power)
    {
        if (!this.invoked)
        {
            this.invoked = true;
            Turn.BlackBoard.Set<int>("EffectCardRestriction_Number", Turn.Number);
            Turn.BlackBoard.Set<string>("EffectCardRestriction_Card", string.Empty);
            Turn.BlackBoard.Set<int>("EffectCardRestriction_Action", 0);
            Turn.BlackBoard.Set<string>("EffectCardRestriction_Power", power.ID);
            this.InvokeThisEffect();
        }
    }

    public void Invoke(Card card, ActionType action)
    {
        if (!this.invoked)
        {
            this.invoked = true;
            Turn.BlackBoard.Set<int>("EffectCardRestriction_Number", Turn.Number);
            Turn.BlackBoard.Set<string>("EffectCardRestriction_Card", card.ID);
            Turn.BlackBoard.Set<int>("EffectCardRestriction_Action", (int) action);
            Turn.BlackBoard.Set<string>("EffectCardRestriction_Power", string.Empty);
            this.InvokeThisEffect();
        }
    }

    private void InvokeThisEffect()
    {
        Turn.Park(true);
        this.CancelAllPowers();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.SwitchCharacter(Turn.Number);
            Turn.Current = Turn.Number;
            SkillCheckValueType bestSkillCheck = Turn.Character.GetBestSkillCheck(base.checks);
            window.dicePanel.SetCheck(Turn.Card, base.checks, bestSkillCheck.skill);
        }
        Turn.PushReturnState(Turn.State);
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "CardRestrictionPending_Resolve"));
        Turn.SetStateData(new TurnStateData(UI.Text(this.Message)));
        Turn.State = GameStateType.Roll;
    }

    protected override bool IsEffectValid()
    {
        CardType cardType = CardTable.LookupCardType(base.source);
        return ((((cardType != CardType.Monster) && (cardType != CardType.Villain)) && (cardType != CardType.Henchman)) || ((Turn.Card.ID == base.source) && (Turn.State == GameStateType.Combat)));
    }

    public bool Match(Card card) => 
        ((this.IsEffectValid() && (base.filter != null)) && base.filter.Match(card));

    public bool Match(CardType cardType)
    {
        if ((this.IsEffectValid() && (base.filter != null)) && (base.filter.CardTypes != null))
        {
            for (int i = 0; i < base.filter.CardTypes.Length; i++)
            {
                if (base.filter.CardTypes[i] == cardType)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override void OnEffectFinished()
    {
        for (int i = 0; i < Turn.Owner.Hand.Count; i++)
        {
            <OnEffectFinished>c__AnonStorey120 storey = new <OnEffectFinished>c__AnonStorey120 {
                vfx = VisualEffect.Apply(Turn.Owner.Hand[i], VisualEffectType.CardRestrictStop)
            };
            LeanTween.delayedCall(0.8f, new Action(storey.<>m__F2));
        }
    }

    public override void OnEffectStarted(Character character)
    {
        for (int i = 0; i < character.Hand.Count; i++)
        {
            <OnEffectStarted>c__AnonStorey11F storeyf = new <OnEffectStarted>c__AnonStorey11F {
                vfx = VisualEffect.Apply(character.Hand[i], VisualEffectType.CardRestrictStart)
            };
            LeanTween.delayedCall(0.9f, new Action(storeyf.<>m__F1));
        }
    }

    public void Play()
    {
        Character restrictedCharacter = GetRestrictedCharacter();
        string id = Turn.BlackBoard.Get<string>("EffectCardRestriction_Power");
        Power power = restrictedCharacter.GetPower(id);
        if (power != null)
        {
            power.Activate();
        }
        else
        {
            ActionType action = Turn.BlackBoard.Get<int>("EffectCardRestriction_Action");
            if (action == ActionType.None)
            {
                action = ActionType.Reveal;
            }
            string str2 = Turn.BlackBoard.Get<string>("EffectCardRestriction_Card");
            Card card = restrictedCharacter.Hand[str2];
            if (card != null)
            {
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (window != null)
                {
                    GuiLayout layoutDeck = window.GetLayoutDeck(action);
                    if (layoutDeck != null)
                    {
                        window.DropCardOnLayout(card, layoutDeck);
                    }
                }
            }
        }
    }

    public override bool Resolve()
    {
        if (Turn.DiceTotal < Turn.DiceTarget)
        {
            Character restrictedCharacter = GetRestrictedCharacter();
            EffectCardRestriction e = new EffectCardRestriction(base.source, base.duration, base.filter);
            restrictedCharacter.ApplyEffect(e);
            return false;
        }
        return true;
    }

    public int Message
    {
        get => 
            base.genericParameters[0];
        set
        {
            base.genericParameters[0] = value;
        }
    }

    public override bool Single =>
        true;

    public override EffectType Type =>
        EffectType.CardRestrictionPending;

    [CompilerGenerated]
    private sealed class <OnEffectFinished>c__AnonStorey120
    {
        internal GameObject vfx;

        internal void <>m__F2()
        {
            UnityEngine.Object.Destroy(this.vfx);
        }
    }

    [CompilerGenerated]
    private sealed class <OnEffectStarted>c__AnonStorey11F
    {
        internal GameObject vfx;

        internal void <>m__F1()
        {
            UnityEngine.Object.Destroy(this.vfx);
        }
    }
}

