using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterPowerExamine : CharacterPower
{
    [Tooltip("filter what card types must be played in order to use this power")]
    public CardSelector CardSelector;
    [Tooltip("optional: block to execute when the close button is pushed")]
    public Block CloseBlock;
    [Tooltip("which deck to examine")]
    public DeckType DeckType = DeckType.Location;
    [Tooltip("the type of card we can draw after examining")]
    public CardType DrawCardType;
    [Tooltip("number of cards to examine")]
    public int Number = 1;
    [Tooltip("examine from the top or bottom of the deck")]
    public DeckPositionType Position = DeckPositionType.Top;
    [Tooltip("the type of card we can recharge after examining")]
    public CardType RechargeCardType;
    [Tooltip("when true, character must own the turn to use this power")]
    public bool RequiresTurnOwnership = true;
    [Tooltip("optional: block to execute after the cards are revealed")]
    public Block RevealBlock;

    public override void Activate()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.MarkPowerActive(this, true);
            if (this.Deck.Count >= this.Number)
            {
                base.PowerBegin();
                if (Turn.State == GameStateType.Recharge)
                {
                    Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_Yes", true);
                }
                window.layoutExamine.Mode = ExamineModeType.Reveal;
                window.layoutExamine.Source = this.DeckType;
                window.layoutExamine.Number = this.Number;
                window.layoutExamine.RevealPosition = this.Position;
                window.layoutExamine.RevealCallback = new TurnStateCallback(this, "CharacterPowerExamine_Reveal");
                window.layoutExamine.CloseCallback = new TurnStateCallback(this, "CharacterPowerExamine_Close");
                if (this.Modifier == null)
                {
                    this.SetModifier(base.Character);
                }
                if ((this.Modifier != null) && this.Modifier.IsValid())
                {
                    this.Modifier.Activate();
                }
                if (this.Position == DeckPositionType.Top)
                {
                    for (int j = 0; j < this.Number; j++)
                    {
                        if ((this.CardSelector != null) && (this.Deck[j].Type == this.DrawCardType))
                        {
                            window.layoutExamine.Action = ExamineActionType.Draw;
                            if (this.Deck[j].Type == this.RechargeCardType)
                            {
                                window.layoutExamine.AlternateAction = ExamineActionType.Recharge;
                            }
                            break;
                        }
                    }
                }
                this.ClearFilteredCardPlayed(Turn.Number);
                for (int i = 0; i < window.layoutHand.Deck.Count; i++)
                {
                    if (window.layoutHand.Deck[i].Displayed)
                    {
                        window.layoutHand.Deck[i].Locked = true;
                    }
                }
                window.CancelAllPowers(true, true);
                if ((Turn.State == GameStateType.Damage) || (Turn.State == GameStateType.Ambush))
                {
                    Turn.EnqueueDamageData();
                }
                if (Turn.State != GameStateType.Popup)
                {
                    Turn.PushReturnState();
                }
                else
                {
                    if (Turn.ReturnState == GameStateType.Dispose)
                    {
                        Turn.CombatCheckSequence--;
                    }
                    Turn.PushReturnState(Turn.PopReturnState());
                }
                if (Turn.State == GameStateType.Damage)
                {
                    Turn.EmptyLayoutDecks = false;
                    window.layoutExamine.Finish = true;
                }
                Turn.State = GameStateType.Examine;
            }
        }
    }

    private void CharacterPowerExamine_Activate()
    {
        this.Activate();
    }

    private void CharacterPowerExamine_Close()
    {
        if (this.CloseBlock != null)
        {
            this.CloseBlock.Invoke();
        }
    }

    private void CharacterPowerExamine_Reveal()
    {
        if (this.RevealBlock != null)
        {
            this.RevealBlock.Invoke();
        }
    }

    private void CharacterPowerExamine_SkipActivation()
    {
        this.ClearFilteredCardPlayed(Turn.Number);
        if (Turn.ReturnState == GameStateType.Dispose)
        {
            Turn.CombatCheckSequence--;
        }
        Turn.ReturnToReturnState();
    }

    private void ClearFilteredCardPlayed(int n)
    {
        Turn.BlackBoard.ClearBitFlag("Character_PlayedFilteredTrait", n);
    }

    public override void Deactivate()
    {
    }

    private bool GetFilteredCardPlayed(int n) => 
        Turn.BlackBoard.GetBitFlag("Character_PlayedFilteredTrait", n);

    public override void Initialize(Character self)
    {
        this.SetModifier(self);
    }

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (this.Number <= 0)
        {
            return false;
        }
        if (this.Deck.Count < this.Number)
        {
            return false;
        }
        if ((this.CardSelector != null) && !this.GetFilteredCardPlayed(Turn.Number))
        {
            return false;
        }
        if ((this.CardSelector == null) && Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (this.RequiresTurnOwnership && !Rules.IsTurnOwner())
        {
            return false;
        }
        if ((!this.RequiresTurnOwnership && (Game.GameType == GameType.LocalMultiPlayer)) && (Turn.Number != Turn.Switch))
        {
            return false;
        }
        if (Turn.End && (Turn.EndReason == GameReasonType.MonsterForced))
        {
            return false;
        }
        if ((((Turn.State != GameStateType.Setup) && (Turn.State != GameStateType.Finish)) && ((Turn.State != GameStateType.EndTurn) && (Turn.State != GameStateType.StartTurn))) && ((Turn.State != GameStateType.Damage) && (Turn.State != GameStateType.ConfirmPowerUse)))
        {
            return false;
        }
        return true;
    }

    public override void OnCardDeactivated(Card card)
    {
        if (((this.CardSelector != null) && ((card.PlayedOwner == base.Character.ID) || (card.Displayed && string.IsNullOrEmpty(card.PlayedOwner)))) && this.CardSelector.ToFilter().Match(card))
        {
            this.ClearFilteredCardPlayed(Turn.Number);
        }
    }

    public override void OnCardEncountered(Card card)
    {
        if (((Turn.Owner.ID == base.Character.ID) && this.GetFilteredCardPlayed(Turn.Number)) && Turn.BlackBoard.Get<bool>("CharacterPowerExamine_Encounter"))
        {
            Turn.BlackBoard.Set<bool>("CharacterPowerExamine_Encounter", false);
            this.ShowPopUp();
        }
    }

    public override void OnCardPlayed(Card card)
    {
        if (((this.CardSelector != null) && ((card.PlayedOwner == base.Character.ID) || (card.Displayed && string.IsNullOrEmpty(card.PlayedOwner)))) && this.CardSelector.ToFilter().Match(card))
        {
            if (Party.IndexOf(card.PlayedOwner) < 0)
            {
                int number = Turn.Number;
            }
            this.SetFilteredCardPlayed(Party.IndexOf(card.PlayedOwner));
            Turn.BlackBoard.Set<string>("ActivatableAbility", base.ID);
        }
    }

    public override void OnExamineComplete()
    {
        this.PowerEnd();
    }

    public override void OnSecondCombat(Card card)
    {
        if ((Turn.Owner.ID == base.Character.ID) && this.GetFilteredCardPlayed(Turn.Number))
        {
            this.ShowPopUp();
        }
    }

    private void SetFilteredCardPlayed(int n)
    {
        Turn.BlackBoard.SetBitFlag("Character_PlayedFilteredTrait", n);
    }

    private void SetModifier(Character character)
    {
        for (int i = 0; i < character.Powers.Count; i++)
        {
            BaseCharacterPowerMod mod = character.Powers[i] as BaseCharacterPowerMod;
            if ((mod != null) && mod.Modifies(base.ID))
            {
                this.Modifier = mod;
            }
        }
    }

    private void ShowPopUp()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            string name = PowerTable.Get(Turn.BlackBoard.Get<string>("ActivatableAbility")).Name;
            window.Popup.Clear();
            base.Message("You may activate player powers before proceeding.");
            window.Popup.Add("Skip", new TurnStateCallback(this, "CharacterPowerExamine_SkipActivation"));
            window.Popup.Add(name, new TurnStateCallback(this, "CharacterPowerExamine_Activate"));
            window.Popup.SetDeckPosition(DeckType.Character);
            if ((Turn.State == GameStateType.Ambush) || (Turn.State == GameStateType.Damage))
            {
                Turn.EnqueueDamageData();
            }
            Turn.PushReturnState();
            Turn.State = GameStateType.Popup;
        }
    }

    private Deck Deck
    {
        get
        {
            if (this.DeckType == DeckType.Character)
            {
                return Turn.Character.Deck;
            }
            return Location.Current.Deck;
        }
    }

    private BaseCharacterPowerMod Modifier { get; set; }
}

