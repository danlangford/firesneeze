using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CardPowerExamine : CardPower
{
    [Tooltip("acquire cards matching this optional selector")]
    public CardSelector Acquire;
    [Tooltip("examine all cards in the deck")]
    public bool All;
    [Tooltip("block executed when the panel is closed")]
    public Block BlockClose;
    [Tooltip("block executed after the encounter button is pushed")]
    public Block BlockEncounter;
    [Tooltip("encounter cards matching this optional selector")]
    public CardSelector Encounter;
    [Tooltip("evade (shuffle) cards matching this optional selector")]
    public CardSelector Evade;
    [Tooltip("the type of encounter this card performs")]
    public EncounterType ExamineEncounterType;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("true means that cards can be dragged from/to the bottom of the deck")]
    public bool ModifyBottom;
    [Tooltip("true means that cards can be dragged from/to the top of the deck")]
    public bool ModifyTop;
    [Tooltip("number of cards to examine")]
    public int Number = 1;
    [Tooltip("examine from the top or bottom of the deck")]
    public DeckPositionType Position = DeckPositionType.Top;
    [Tooltip("reveal cards until you find one matching this optional selector")]
    public CardSelector Seek;
    [Tooltip("true means that a shuffle animation will play when the closed")]
    public bool Shuffle;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            Turn.PushReturnState();
            Turn.SetStateData(new TurnStateData(this.Message));
            Turn.PushStateDestination(new TurnStateCallback(card, "CardPowerExamine_Examine"));
            Turn.EmptyLayoutDecks = false;
            Turn.State = GameStateType.Confirm;
        }
    }

    private void CardPowerExamine_Action()
    {
        Turn.EmptyLayoutDecks = true;
        Turn.BlackBoard.Set<bool>("CharacterPowerExamine_Encounter", true);
        if (this.BlockEncounter != null)
        {
            this.BlockEncounter.Invoke();
        }
    }

    private void CardPowerExamine_Close()
    {
        Turn.EmptyLayoutDecks = true;
        if (this.BlockClose != null)
        {
            this.BlockClose.Invoke();
        }
        Game.Instance.StartCoroutine(this.CardPowerExamine_Return());
    }

    private void CardPowerExamine_Examine()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        string locationToExamine = this.GetLocationToExamine();
        if (locationToExamine != Turn.Owner.Location)
        {
            Location.Load(locationToExamine);
            window.Refresh();
        }
        if (window != null)
        {
            window.layoutExamine.Source = DeckType.Location;
            window.layoutExamine.Number = this.GetNumberOfRevealedCards(this.Number);
            window.layoutExamine.RevealPosition = this.Position;
            window.layoutExamine.ModifyTop = this.ModifyTop;
            window.layoutExamine.ModifyBottom = this.ModifyBottom;
            window.layoutExamine.Shuffle = this.Shuffle;
            if (this.Seek != null)
            {
                window.layoutExamine.Sort = this.Seek.ToFilter();
            }
            window.layoutExamine.ActionCallback = new TurnStateCallback(base.Card, "CardPowerExamine_Action");
            window.layoutExamine.CloseCallback = new TurnStateCallback(base.Card, "CardPowerExamine_Close");
            if (this.ExamineEncounterType != EncounterType.None)
            {
                Turn.EncounterType = this.ExamineEncounterType;
            }
            if (this.Deck.Count > 0)
            {
                if (this.Position == DeckPositionType.Top)
                {
                    window.layoutExamine.Action = this.GetActionType(this.Deck[0]);
                }
                if (this.Position == DeckPositionType.Bottom)
                {
                    window.layoutExamine.Action = this.GetActionType(this.Deck[this.Deck.Count - 1]);
                }
            }
            if (this.All)
            {
                window.layoutExamine.Scroll = true;
                window.layoutExamine.Mode = ExamineModeType.All;
            }
            else
            {
                window.layoutExamine.Mode = ExamineModeType.Reveal;
            }
            Turn.EmptyLayoutDecks = false;
            Turn.State = GameStateType.Examine;
        }
    }

    [DebuggerHidden]
    private IEnumerator CardPowerExamine_Return() => 
        new <CardPowerExamine_Return>c__Iterator14 { <>f__this = this };

    public override void Deactivate(Card card)
    {
        card.ReturnCard(card);
        Turn.PopStateDestination();
        Turn.ReturnToReturnState();
    }

    private ExamineActionType GetActionType(Card card)
    {
        if ((this.Encounter != null) && this.Encounter.Match(card))
        {
            return ExamineActionType.Encounter;
        }
        if ((this.Acquire != null) && this.Acquire.Match(card))
        {
            return ExamineActionType.Acquire;
        }
        if ((this.Evade != null) && this.Evade.Match(card))
        {
            return ExamineActionType.Evade;
        }
        return ExamineActionType.None;
    }

    private string GetLocationToExamine()
    {
        Character character = Party.Find(base.Card.PlayedOwner);
        if (character != null)
        {
            return character.Location;
        }
        return Turn.Character.Location;
    }

    private int GetNumberOfRevealedCards(int max)
    {
        if (this.Seek == null)
        {
            return max;
        }
        int num = 0;
        if (this.Position == DeckPositionType.Top)
        {
            for (int i = 0; i < this.Deck.Count; i++)
            {
                num++;
                if (this.Match(this.Deck[i]))
                {
                    return num;
                }
            }
        }
        if (this.Position == DeckPositionType.Bottom)
        {
            for (int j = this.Deck.Count - 1; j >= 0; j--)
            {
                num++;
                if (this.Match(this.Deck[j]))
                {
                    return num;
                }
            }
        }
        return this.Deck.Count;
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (this.Number <= 0)
        {
            return false;
        }
        if (Rules.IsTurnOwner() && (this.Deck.Count <= 0))
        {
            return false;
        }
        if (!Rules.IsTurnOwner() && (Scenario.Current.GetCardCount(Turn.Character.Location) <= 0))
        {
            return false;
        }
        if (!Rules.IsTurnOwner() && (base.Aid == AidType.None))
        {
            return false;
        }
        return ((Turn.State == GameStateType.Finish) || (Turn.State == GameStateType.Setup));
    }

    public override bool IsPowerDeactivationAllowed(Card card) => 
        (((Turn.State == GameStateType.Finish) || (Turn.State == GameStateType.Setup)) || (Turn.State == GameStateType.Confirm));

    private bool Match(Card card) => 
        ((this.Seek == null) || this.Seek.Match(card));

    private Deck Deck =>
        Location.Current.Deck;

    [CompilerGenerated]
    private sealed class <CardPowerExamine_Return>c__Iterator14 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal CardPowerExamine <>f__this;
        internal string <locationID>__1;
        internal string <originalLocationID>__0;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<originalLocationID>__0 = Turn.Owner.Location;
                    this.<locationID>__1 = this.<>f__this.GetLocationToExamine();
                    if ((this.<locationID>__1 == this.<originalLocationID>__0) || (this.<>f__this.Position != DeckPositionType.Shuffle))
                    {
                        break;
                    }
                    this.$current = new WaitForSeconds(1.7f);
                    this.$PC = 1;
                    return true;

                case 1:
                    break;

                default:
                    goto Label_0097;
            }
            this.<>f__this.ReturnControlToInitialCharacter();
            this.$PC = -1;
        Label_0097:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }
}

