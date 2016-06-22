using System;
using UnityEngine;

public class CardPowerAugury : CardPower
{
    [Tooltip("if true you can Augury any location, you must have EventExamineAnyLocationAugury.cs attached too. Else you can only augury this location.")]
    public bool AnyLocation;
    [Tooltip("close block needs to force a recharge.")]
    public Block BlockClose;
    [Tooltip("string reference for helper text.")]
    public StrRefType Message;
    [Tooltip("if true you can rearrange the cards in the bottom part and to the bottom part.")]
    public bool ModifyBot = true;
    [Tooltip("if true you can rearrange the cards in the top part and to the top part.")]
    public bool ModifyTop = true;
    [Tooltip("number of cards revealed.")]
    public int Number = 3;
    [Tooltip("where should the reveal start from.")]
    public DeckPositionType Position = DeckPositionType.Top;
    [Tooltip("if true, Augury will bring up a prompt to select the card type to filter for.")]
    public bool SelectCardType = true;
    [Tooltip("selects the chosen card type.")]
    public CardSelector Selector;
    [Tooltip("do we shuffle the deck after laying out what we examined?")]
    public bool Shuffle = true;

    public override void Activate(Card card)
    {
        this.SpellAugury_Choose(card);
    }

    public override void Deactivate(Card card)
    {
        Turn.PopStateDestination();
        Turn.PopReturnState();
        Turn.EmptyLayoutDecks = false;
        Turn.GotoCancelDestination();
        Turn.EmptyLayoutDecks = true;
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

    private int GetNumMatchingCards(CardType type)
    {
        int num = 0;
        int num2 = Mathf.Min(this.Number, this.Deck.Count);
        for (int i = 0; i < num2; i++)
        {
            if (this.Deck[i].Type == type)
            {
                num++;
            }
        }
        return num;
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
        if ((Rules.IsTurnOwner() && (this.Deck.Count <= 0)) && !this.AnyLocation)
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
        false;

    private void SpellAugury_CancelChoose()
    {
        Turn.Current = Turn.InitialCharacter;
        Turn.ReturnToReturnState();
    }

    private void SpellAugury_Choose(Card card)
    {
        Turn.PushReturnState();
        Turn.EmptyLayoutDecks = false;
        Turn.PushCancelDestination(new TurnStateCallback(card, "SpellAugury_CancelChoose"));
        Turn.Current = Turn.Number;
        if (this.SelectCardType)
        {
            if (this.AnyLocation)
            {
                Turn.PushStateDestination(new TurnStateCallback(card, "SpellAugury_Location"));
            }
            else
            {
                Turn.PushStateDestination(new TurnStateCallback(card, "SpellAugury_Examine"));
            }
            Turn.State = GameStateType.SelectType;
        }
        else if (this.AnyLocation)
        {
            Turn.PushStateDestination(new TurnStateCallback(card, "SpellAugury_Location"));
            Turn.State = GameStateType.ConfirmProceed;
        }
        else
        {
            Turn.PushStateDestination(new TurnStateCallback(card, "SpellAugury_Examine"));
            Turn.State = GameStateType.Confirm;
        }
    }

    private void SpellAugury_Close()
    {
        if (this.BlockClose != null)
        {
            this.BlockClose.Invoke();
        }
        base.ReturnControlToInitialCharacter();
    }

    private void SpellAugury_Examine()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        Turn.EmptyLayoutDecks = true;
        string locationToExamine = this.GetLocationToExamine();
        if (locationToExamine != Turn.Owner.Location)
        {
            Location.Load(locationToExamine);
            window.Refresh();
        }
        if (window != null)
        {
            CardType selectedCardType = window.chooseTypePanel.SelectedCardType;
            if (this.SelectCardType)
            {
                this.Selector.CardTypes[0] = selectedCardType;
                this.Selector.Strict = false;
            }
            window.layoutExamine.Mode = ExamineModeType.Reveal;
            window.layoutExamine.Source = DeckType.Location;
            window.layoutExamine.RevealPosition = this.Position;
            window.layoutExamine.Number = Mathf.Min(this.Number, this.Deck.Count);
            window.layoutExamine.ModifyTop = this.ModifyTop;
            window.layoutExamine.ModifyBottom = this.ModifyBot;
            window.layoutExamine.Group = true;
            window.layoutExamine.Shuffle = this.Shuffle;
            if (this.Selector != null)
            {
                window.layoutExamine.Sort = this.Selector.ToFilter();
            }
            window.layoutExamine.CloseCallback = new TurnStateCallback(base.Card, "SpellAugury_Close");
            if ((this.GetNumMatchingCards(selectedCardType) > 0) && this.SelectCardType)
            {
                Turn.SetStateData(new TurnStateData(this.Message));
            }
            Turn.State = GameStateType.Examine;
        }
    }

    private void SpellAugury_Location()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutSummoner.Show(base.Card.ID);
        }
        if (base.Card.PlayedOwner != string.Empty)
        {
            Turn.BlackBoard.Set<string>("LastLocation", Party.Characters[base.Card.PlayedOwner].Location);
        }
        if (window != null)
        {
            window.mapPanel.Mode = MapModeType.Examine;
            window.ShowMap(true);
            window.messagePanel.Show(StringTableManager.GetHelperText(0x1c));
        }
    }

    private Deck Deck =>
        Location.Current.Deck;
}

