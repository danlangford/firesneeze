using System;
using UnityEngine;

public class CardPowerExchange : CardPower
{
    [Tooltip("the selector that holds the types of cards that can be exchanged")]
    public CardSelector CardSelector;
    private CardType discardType;
    [Tooltip("the message to display during the pick state")]
    public StrRefType Message;
    [Tooltip("spell range")]
    public TargetType Range = TargetType.AllAtLocation;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            Turn.Character.MarkCardType(card.Type, true);
            Turn.BlackBoard.Set<string>("CardPowerExchangePlayedOwner", Turn.Character.ID);
            Turn.PushReturnState();
            if (this.CharactersWithDiscards(Turn.Character.Location) > 1)
            {
                this.SpellExchange_Target();
            }
            else
            {
                Turn.Target = this.CharacterWithDiscard(Turn.Character.Location);
                if (Turn.Target < 0)
                {
                    Turn.Target = Turn.Number;
                }
                Turn.PushCancelDestination(new TurnStateCallback(base.Card, "SpellExchange_Cancel"));
                Turn.PushStateDestination(new TurnStateCallback(base.Card, "SpellExchange_Discard"));
                Turn.EmptyLayoutDecks = false;
                Turn.State = GameStateType.ConfirmProceed;
            }
        }
    }

    private bool CharacterHasCard(Character character)
    {
        for (int i = 0; i < this.CardSelector.CardTypes.Length; i++)
        {
            if ((character.Discard.Filter(this.CardSelector.CardTypes[i]) > 0) && (character.Hand.Filter(this.CardSelector.CardTypes[i]) > 0))
            {
                return true;
            }
        }
        return false;
    }

    private int CharactersWithDiscards(string loc)
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((Party.Characters[i].Location == loc) && this.CharacterHasCard(Party.Characters[i]))
            {
                num++;
            }
        }
        return num;
    }

    private int CharacterWithDiscard(string loc)
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((Party.Characters[i].Location == loc) && this.CharacterHasCard(Party.Characters[i]))
            {
                return i;
            }
        }
        return -1;
    }

    public override void Deactivate(Card card)
    {
        base.Deactivate(card);
        this.SpellExchange_Cancel();
    }

    private CardType GetDiscardCardType()
    {
        CardType none = CardType.None;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((window != null) && (window.layoutDiscard.Deck.Count > 0))
        {
            none = window.layoutDiscard.Deck[0].Type;
        }
        return none;
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
        if (Turn.Character.IsCardTypeMarked(card.Type))
        {
            return false;
        }
        if ((Turn.State != GameStateType.Finish) && (Turn.State != GameStateType.Setup))
        {
            return false;
        }
        return (this.CharacterWithDiscard(Location.Current.ID) >= 0);
    }

    private void ProcessLayoutDecks()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessLayoutDecks();
        }
    }

    private void SpellExchange_Cancel()
    {
        Turn.PopStateDestination();
        Turn.PopCancelDestination();
        Turn.ReturnToReturnState();
        Turn.EmptyLayoutDecks = true;
    }

    private void SpellExchange_Discard()
    {
        Turn.SwitchCharacter(Turn.Target);
        Turn.Current = Turn.Number;
        CardFilter filter = new CardFilter {
            CardTypes = new CardType[this.CardSelector.Filter(Turn.Character.Discard)]
        };
        int index = -1;
        for (int i = 0; i < Turn.Character.Discard.Count; i++)
        {
            if (this.CardSelector.Match(Turn.Character.Discard[i]))
            {
                index++;
                filter.CardTypes[index] = Turn.Character.Discard[i].Type;
            }
        }
        TurnStateData data = new TurnStateData(ActionType.Discard, filter, 1);
        Turn.SetStateData(data);
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "SpellExchange_Pick"));
        Turn.EmptyLayoutDecks = true;
        this.ProcessLayoutDecks();
        Turn.EmptyLayoutDecks = false;
        Turn.State = GameStateType.Penalty;
    }

    private void SpellExchange_Finish()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.EmptyLayoutDecks = true;
            window.ProcessLayoutDecks();
        }
        Character character = Party.Characters[Turn.BlackBoard.Get<string>("CardPowerExchangePlayedOwner")];
        Card card = character.Discard.Draw(base.Card.ID);
        card.PlayedOwner = Turn.BlackBoard.Get<string>("CardPowerExchangePlayedOwner");
        if (window != null)
        {
            window.layoutDiscard.Deck.Add(card);
        }
        card.SetPowerInfo(0, base.Card);
        Turn.SwitchCharacter(Turn.InitialCharacter);
        Turn.Current = Turn.Number;
        if (window != null)
        {
            window.Refresh();
        }
        Turn.PushStateDestination(Turn.PopReturnState());
        Turn.State = GameStateType.Recharge;
    }

    private void SpellExchange_Pick()
    {
        CardFilter filter = new CardFilter {
            CardTypes = new CardType[] { this.GetDiscardCardType() }
        };
        TurnStateData data = new TurnStateData(ActionType.Discard, filter, ActionType.Draw, 1) {
            Message = this.Message.ToString()
        };
        Turn.SetStateData(data);
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "SpellExchange_Finish"));
        Turn.EmptyLayoutDecks = true;
        Turn.State = GameStateType.Pick;
    }

    private void SpellExchange_Target()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((Party.Characters[i].Active == ActiveType.Active) && !this.CharacterHasCard(Party.Characters[i]))
            {
                Party.Characters[i].Active = ActiveType.Inactive;
            }
        }
        Turn.PushCancelDestination(new TurnStateCallback(base.Card, "SpellExchange_Cancel"));
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "SpellExchange_Discard"));
        GameStateTarget.DisplayText = base.Card.DisplayName;
        Turn.TargetType = this.Range;
        Turn.EmptyLayoutDecks = false;
        Turn.State = GameStateType.Target;
    }
}

