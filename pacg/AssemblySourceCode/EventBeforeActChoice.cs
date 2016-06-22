using System;
using UnityEngine;

public class EventBeforeActChoice : Event
{
    [Tooltip("execute when the examine window's action button is used")]
    public Block ActionBlock;
    [Tooltip("text displayed on the examine action button")]
    public StrRefType ActionButtonText;
    [Tooltip("execute when the examine window's close button is used")]
    public Block CloseBlock;
    [Tooltip("the number of cards to draw from the box")]
    public int Number = 3;

    public override void OnBeforeAct()
    {
        int num = 0;
        for (int i = 0; i < this.Number; i++)
        {
            Card card = Campaign.Box.Draw(CardType.Item);
            if (card != null)
            {
                this.Deck.Add(card, DeckPositionType.Top);
                this.SetCardStackPosition(card, 1 + num);
                num++;
            }
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((window != null) && (this.Deck.Count >= 0))
        {
            window.layoutExamine.Mode = ExamineModeType.Reveal;
            window.layoutExamine.Source = DeckType.Location;
            window.layoutExamine.Number = num;
            window.layoutExamine.RevealPosition = DeckPositionType.Top;
            window.layoutExamine.Choose = CardFilter.Boon;
            window.layoutExamine.Finish = true;
            window.layoutExamine.ActionCallback = new TurnStateCallback(base.Card, "OnCardEncountered_Action");
            window.layoutExamine.CloseCallback = new TurnStateCallback(base.Card, "OnCardEncountered_Close");
            window.layoutExamine.ActionButtonText = this.ActionButtonText.ToString();
            if (base.Card.Checks.Length <= 0)
            {
                Turn.LastCombatResult = CombatResultType.None;
                Turn.CombatStage = TurnCombatStageType.Encounter;
                Turn.PushReturnState(GameStateType.Post);
            }
            Turn.State = GameStateType.Examine;
            Event.Done();
        }
    }

    private void OnCardEncountered_Action()
    {
        if (this.ActionBlock != null)
        {
            this.ActionBlock.Invoke();
        }
    }

    private void OnCardEncountered_Close()
    {
        if (this.CloseBlock != null)
        {
            this.CloseBlock.Invoke();
        }
    }

    private void SetCardStackPosition(Card card, int order)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            card.SortingOrder = order;
            card.transform.position = window.layoutLocation.transform.position;
            card.transform.localScale = window.layoutLocation.Scale;
        }
    }

    private Deck Deck =>
        Location.Current.Deck;

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

