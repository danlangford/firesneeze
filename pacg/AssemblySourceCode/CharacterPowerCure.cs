using System;
using UnityEngine;

public class CharacterPowerCure : CharacterPower
{
    [Tooltip("if true, this power is an \"instead of your first explore\" activation consuming the exploration")]
    public bool ConsumeExplore = true;
    [Tooltip("dice used to compute the amount")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the amount (total not per dice)")]
    public int DiceBonus;
    [Tooltip("final destination of cards that were used to activate this power")]
    public ActionType FinalDestinationPenalty = ActionType.Discard;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    private BaseCharacterPowerMod Modifier;
    [Tooltip("where should the new cards go in the deck?")]
    public DeckPositionType Position = DeckPositionType.Shuffle;
    [Tooltip("spell range")]
    public TargetType Range = TargetType.AllAtLocation;
    [Tooltip("if true, this power requires the card to be revealed before going to the final destination")]
    public bool RevealFirst = true;

    public override void Activate()
    {
        base.PowerBegin();
        TurnStateData data = new TurnStateData(ActionType.Reveal, 1);
        if (!this.RevealFirst)
        {
            data.Actions[0] = this.FinalDestinationPenalty;
        }
        data.Filter = new CardFilter();
        data.Filter.CardTraits = new TraitType[] { TraitType.Divine };
        Turn.SetStateData(data);
        Turn.EmptyLayoutDecks = false;
        Turn.PushReturnState();
        if (Rules.IsTargetRequired(this.Range))
        {
            Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerCure_Cancel"));
            Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerCure_Target"));
            Turn.State = GameStateType.Penalty;
        }
        else
        {
            Turn.Target = Turn.Number;
            Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerCure_Cancel"));
            Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerCure_Roll"));
            Turn.State = GameStateType.Penalty;
        }
        base.Activate();
    }

    private void CharacterPowerCure_Cancel()
    {
        Turn.MarkPowerActive(this, false);
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Remove(this.Dice[i]);
        }
        Turn.DiceBonus -= this.DiceBonus;
        this.RefreshDicePanel();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        Card revealedCard = this.GetRevealedCard();
        if (revealedCard != null)
        {
            revealedCard.Revealed = false;
            revealedCard.Locked = false;
            if (window != null)
            {
                window.layoutHand.Refresh();
            }
        }
        Turn.ReturnToReturnState();
        this.PowerEnd();
    }

    private void CharacterPowerCure_Finish()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Character character = Party.Characters[Turn.Target];
            character.Discard.Shuffle();
            int num = Mathf.Clamp(Turn.DiceTotal, 0, character.Discard.Count);
            if (Turn.Number == Turn.Target)
            {
                Card[] cards = new Card[num];
                for (int i = num - 1; i >= 0; i--)
                {
                    cards[i] = character.Discard[i];
                }
                window.Heal(character, cards, this.Position);
            }
            else
            {
                for (int j = num - 1; j >= 0; j--)
                {
                    Card card = character.Discard[j];
                    character.Deck.Add(card, this.Position);
                    card.PreviousDeck = null;
                }
            }
            if (this.RevealFirst)
            {
                Card revealedCard = this.GetRevealedCard();
                switch (this.FinalDestinationPenalty)
                {
                    case ActionType.Discard:
                        window.Discard(revealedCard);
                        break;

                    case ActionType.Bury:
                        window.Bury(revealedCard);
                        break;
                }
            }
            if (this.ConsumeExplore)
            {
                Turn.Explore = false;
                window.ShowExploreButton(false);
            }
        }
        Turn.EmptyLayoutDecks = true;
        if (this.ConsumeExplore)
        {
            Turn.PopReturnState();
            Turn.State = GameStateType.Done;
        }
        else
        {
            Turn.ReturnToReturnState();
        }
        this.SetModifier(base.Character);
        if (this.Modifier != null)
        {
            this.Modifier.Activate();
        }
        this.PowerEnd();
    }

    private void CharacterPowerCure_Roll()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus += this.DiceBonus;
        this.RefreshDicePanel();
        Turn.Checks = null;
        Card revealedCard = this.GetRevealedCard();
        if (revealedCard != null)
        {
            revealedCard.SetPowerInfo(base.Character.Powers.IndexOf(this), base.Character.ID);
        }
        Turn.EmptyLayoutDecks = false;
        Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerCure_Cancel"));
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerCure_Finish"));
        Turn.State = GameStateType.Roll;
    }

    private void CharacterPowerCure_Target()
    {
        Turn.TargetType = this.Range;
        Turn.EmptyLayoutDecks = false;
        Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerCure_Cancel"));
        Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerCure_Roll"));
        GameStateTarget.DisplayText = this.Message.ToString();
        Card revealedCard = this.GetRevealedCard();
        if (revealedCard != null)
        {
            revealedCard.SetPowerInfo(base.Character.Powers.IndexOf(this), base.Character.ID);
        }
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].Discard.Count == 0)
            {
                Party.Characters[i].Active = ActiveType.Inactive;
            }
        }
        Turn.State = GameStateType.Target;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        Turn.PopStateDestination();
        Turn.PopCancelDestination();
        Card revealedCard = this.GetRevealedCard();
        if (revealedCard != null)
        {
            revealedCard.ReturnCard(revealedCard);
        }
        Turn.Dice.Clear();
        Turn.DiceBonus = 0;
        this.RefreshDicePanel();
        Turn.ReturnToReturnState();
    }

    private Card GetRevealedCard()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Character character = Turn.Character;
            if (this.RevealFirst)
            {
                for (int i = 0; i < character.Hand.Count; i++)
                {
                    if (character.Hand[i].Revealed)
                    {
                        return character.Hand[i];
                    }
                }
            }
            else
            {
                Deck deck = window.GetLayoutDeck(this.FinalDestinationPenalty).Deck;
                if (deck.Count > 0)
                {
                    return deck[0];
                }
            }
        }
        return null;
    }

    public override void Initialize(Character self)
    {
        this.SetModifier(self);
    }

    public override bool IsValid()
    {
        if (Turn.Character.Hand.Filter(TraitType.Divine) < 1)
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Rules.GetCharactersWithDiscardCount(Turn.Owner.Location) <= 0)
        {
            return false;
        }
        if (this.ConsumeExplore && ((Turn.State != GameStateType.Setup) || !Turn.Explore))
        {
            return false;
        }
        return ((Turn.State == GameStateType.Setup) || (Turn.State == GameStateType.Finish));
    }

    private void RefreshDicePanel()
    {
        Turn.DiceTarget = 0;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Refresh();
        }
    }

    private void SetModifier(Character self)
    {
        if (this.Modifier == null)
        {
            for (int i = 0; i < self.Powers.Count; i++)
            {
                if (self.Powers[i].Modifies(base.ID))
                {
                    this.Modifier = self.Powers[i] as BaseCharacterPowerMod;
                    if (this.Modifier != null)
                    {
                        break;
                    }
                }
            }
        }
    }
}

