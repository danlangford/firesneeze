using System;
using UnityEngine;

public class GuiLayoutExplore : GuiLayout
{
    [Tooltip("push the position of blocker cards 1.2 * Vector.Right")]
    public const float BlockerOffset = 1.2f;
    [Tooltip("this visual aid indicates that a confirm is possible now")]
    public Animator VisualAidConfirm;
    [Tooltip("this visual aid indicates that an explore is possible now")]
    public Animator VisualAidExplore;

    public override void Display()
    {
        for (int i = 0; i < Location.Current.Deck.Count; i++)
        {
            Card card = Location.Current.Deck[i];
            if ((card.Blocker == BlockerType.Movement) && (Turn.CountExplores > i))
            {
                card.transform.localScale = this.Scale;
                card.transform.position = base.transform.position + ((Vector3) ((Vector3.right * (i + 1)) * 1.2f));
                card.SortingOrder = (1 + Location.Current.Deck.Count) - i;
                card.Side = !card.IsBlocker() ? CardSideType.Back : CardSideType.Front;
                card.Show(true);
                Geometry.SetLayerRecursively(card.gameObject, Constants.LAYER_CARD);
            }
            else
            {
                card.transform.localScale = this.Scale;
                card.transform.position = base.transform.position;
                card.SortingOrder = 1;
                card.Side = !card.IsBlocker() ? CardSideType.Back : CardSideType.Front;
                card.Show(true);
                Geometry.SetLayerRecursively(card.gameObject, Constants.LAYER_CARD);
                while (((i + 1) < Location.Current.Deck.Count) && Location.Current.Deck[i + 1].Visible)
                {
                    i++;
                    Location.Current.Deck[i].Show(false);
                }
                break;
            }
        }
    }

    public void Help(bool isVisible)
    {
        if (Turn.State == GameStateType.Confirm)
        {
            this.VisualAidConfirm.SetBool("Glow", isVisible);
        }
        else
        {
            this.VisualAidExplore.SetBool("Glow", isVisible);
        }
    }

    public override bool OnGuiDrag(Card card)
    {
        if (card.Locked)
        {
            return false;
        }
        if (!Rules.IsExplorePossible())
        {
            return false;
        }
        if (CardPropertyBlocker.IsExploreBlocked(card) && (Turn.CountExplores > card.Deck.IndexOf(card)))
        {
            return false;
        }
        return Turn.Explore;
    }

    public override bool OnGuiDrop(Card card) => 
        false;

    public override void Refresh()
    {
        if (Location.Current.Deck.Count > 0)
        {
            Location.Current.Deck.Prepare();
            Card card = Location.Current.Deck[0];
            if (card.IsBlocker())
            {
                card.SortingOrder = 1;
                card.Side = CardSideType.Front;
                card.Show(true);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        if (base.Counter != null)
        {
            base.Counter.Deck = Location.Current.Deck;
        }
    }

    public override bool AutoRefresh =>
        false;

    public override Deck Deck
    {
        get => 
            base.myDeck;
        set
        {
            if (base.myDeck != value)
            {
                base.myDeck = value;
                for (int i = 0; i < base.myDeck.Count; i++)
                {
                    Card card = base.myDeck[i];
                    card.transform.localScale = this.Scale;
                    card.transform.position = base.transform.position;
                }
            }
        }
    }
}

