using System;
using UnityEngine;

public class GuiLayoutHand : GuiLayout
{
    private readonly float basey = -0.4f;
    private Vector2 cardSize = Vector2.zero;
    [Tooltip("reference to reveal layout manager in window hierarchy")]
    public GuiLayoutReveal LayoutReveal;
    private readonly float padx = 0.1f;
    private readonly float pady = 0.1f;

    public override void Display()
    {
        if (base.Visible && (this.Deck != null))
        {
            int count = 0;
            for (int i = 0; i < this.Deck.Count; i++)
            {
                Card card = this.Deck[i];
                if (card.Revealed || card.Displayed)
                {
                    count++;
                }
            }
            int num3 = this.Deck.Count - count;
            int index = 0;
            for (int j = 0; j < this.Deck.Count; j++)
            {
                Card card2 = this.Deck[j];
                if (card2.Revealed || card2.Displayed)
                {
                    Geometry.SetLayerRecursively(card2.gameObject, Constants.LAYER_CARD);
                    this.ShowCard(card2, true);
                    card2.SortingOrder = this.GetCardSortingOrder(card2, index);
                    card2.transform.localScale = this.GetCardScale(card2);
                    card2.transform.position = this.GetCardPosition(card2, index++, count);
                }
            }
            int num6 = 0;
            for (int k = 0; k < this.Deck.Count; k++)
            {
                Card card3 = this.Deck[k];
                if (!card3.Revealed && !card3.Displayed)
                {
                    Geometry.SetLayerRecursively(card3.gameObject, Constants.LAYER_CARD);
                    this.ShowCard(card3, true);
                    card3.SortingOrder = this.GetCardSortingOrder(card3, num6);
                    card3.transform.localScale = this.GetCardScale(card3);
                    card3.transform.position = this.GetCardPosition(card3, num6++, num3);
                }
            }
        }
    }

    public override ActionType GetActionType(Card card)
    {
        if (card.Revealed)
        {
            return ActionType.Reveal;
        }
        return base.CardAction;
    }

    public Vector3 GetCardPosition(Card card)
    {
        int count = 0;
        for (int i = 0; i < this.Deck.Count; i++)
        {
            if (this.Deck[i].Revealed == card.Revealed)
            {
                count++;
            }
            if (this.Deck[i].Displayed == card.Displayed)
            {
                count++;
            }
        }
        int num3 = 0;
        for (int j = 0; j < this.Deck.Count; j++)
        {
            if (this.Deck[j] == card)
            {
                return this.GetCardPosition(card, num3, count);
            }
            if (this.Deck[j].Revealed == card.Revealed)
            {
                num3++;
            }
            if (this.Deck[j].Displayed == card.Displayed)
            {
                num3++;
            }
        }
        return base.transform.position;
    }

    public Vector3 GetCardPosition(Card card, int i, int count)
    {
        if (card.Revealed || card.Displayed)
        {
            return this.LayoutReveal.GetCardPosition(card, i, count);
        }
        Vector3 position = base.transform.position;
        if (count <= 1)
        {
            position = base.transform.position;
        }
        if (count == 2)
        {
            position = base.transform.position - new Vector3((0.5f * this.cardSize.x) + this.padx, 0f, 0f);
        }
        if (count == 3)
        {
            position = base.transform.position - new Vector3(this.cardSize.x + this.padx, 0f, 0f);
        }
        if (count >= 4)
        {
            position = base.transform.position - new Vector3((1.5f * this.cardSize.x) + this.padx, 0f, 0f);
        }
        float x = (count >= 5) ? ((this.cardSize.x * 4f) / ((float) count)) : (this.cardSize.x + this.padx);
        float cardVerticalOffset = this.GetCardVerticalOffset(i, count);
        return ((position + ((Vector3) (i * new Vector3(x, 0f, 0f)))) + new Vector3(0f, cardVerticalOffset, 0f));
    }

    private Vector3 GetCardScale(Card card)
    {
        if (!card.Revealed && !card.Displayed)
        {
            return this.Scale;
        }
        return this.LayoutReveal.Scale;
    }

    private int GetCardSortingOrder(Card card, int index)
    {
        if (!card.Revealed && !card.Displayed)
        {
            return -(this.Deck.Count - index);
        }
        return 1;
    }

    private float GetCardVerticalOffset(int i, int count)
    {
        if (count <= 4)
        {
            return this.basey;
        }
        int num = count;
        if (i < (num / 2))
        {
            return (this.basey + (i * this.pady));
        }
        return (this.basey + (Mathf.Abs((int) ((count - i) - 1)) * this.pady));
    }

    public override Vector3 GetPosition(int i) => 
        this.GetCardPosition(this.Deck[i]);

    private void InitializeCards()
    {
        if (this.Deck != null)
        {
            int count = 0;
            for (int i = 0; i < this.Deck.Count; i++)
            {
                Card card = this.Deck[i];
                if (card.Revealed || card.Displayed)
                {
                    count++;
                }
            }
            int num3 = this.Deck.Count - count;
            int num4 = 0;
            int num5 = 0;
            for (int j = 0; j < this.Deck.Count; j++)
            {
                Card card2 = this.Deck[j];
                if (card2 != null)
                {
                    card2.SortingOrder = 1;
                    card2.transform.localScale = this.Scale;
                    if (this.cardSize == Vector2.zero)
                    {
                        this.cardSize = card2.Size;
                    }
                    if (card2.Revealed || card2.Displayed)
                    {
                        card2.transform.position = this.GetCardPosition(card2, num5++, count);
                    }
                    else
                    {
                        card2.transform.position = this.GetCardPosition(card2, num4++, num3);
                    }
                }
            }
        }
    }

    private bool IsCardDraggable(Card card)
    {
        if (card.Locked)
        {
            return false;
        }
        return true;
    }

    public override bool IsDropPossible(Card card)
    {
        if (card.Deck != this.Deck)
        {
            if (!base.IsDropPossible(card))
            {
                return false;
            }
            if (card.Deck == Location.Current.Deck)
            {
                return false;
            }
            if (card.Deck == Turn.Character.Deck)
            {
                return false;
            }
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (card.Deck == Party.Characters[i].Deck)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public override bool OnGuiDrag(Card card)
    {
        if (Turn.State == GameStateType.Examine)
        {
            return false;
        }
        if (card.Displayed)
        {
            return this.IsCardDraggable(card);
        }
        if (card.Side == CardSideType.Back)
        {
            return false;
        }
        return base.OnGuiDrag(card);
    }

    public override bool OnGuiDrop(Card card)
    {
        if (!this.IsDropPossible(card))
        {
            return false;
        }
        if (Turn.State == GameStateType.Share)
        {
            GameStateShare.Share(card, false);
        }
        if (card.Deck == this.Deck)
        {
            card.Revealed = false;
            card.Displayed = false;
        }
        base.InsertAtDropPosition(card);
        this.Refresh();
        return true;
    }

    public override void Refresh()
    {
        if (base.Visible && (this.Deck != null))
        {
            int count = 0;
            for (int i = 0; i < this.Deck.Count; i++)
            {
                Card card = this.Deck[i];
                if (card.Revealed || card.Displayed)
                {
                    count++;
                }
            }
            int num3 = this.Deck.Count - count;
            int index = 0;
            for (int j = 0; j < this.Deck.Count; j++)
            {
                Card card2 = this.Deck[j];
                if (card2.Revealed || card2.Displayed)
                {
                    Geometry.SetLayerRecursively(card2.gameObject, Constants.LAYER_CARD);
                    this.ShowCard(card2, true);
                    card2.SortingOrder = this.GetCardSortingOrder(card2, index);
                    LeanTween.cancel(card2.gameObject);
                    LeanTween.scale(card2.gameObject, this.GetCardScale(card2), 0.15f).setEase(LeanTweenType.easeInOutQuad);
                    card2.MoveCard(this.GetCardPosition(card2, index++, count), 0.15f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                }
            }
            int num6 = 0;
            for (int k = 0; k < this.Deck.Count; k++)
            {
                Card card3 = this.Deck[k];
                if (!card3.Revealed && !card3.Displayed)
                {
                    Geometry.SetLayerRecursively(card3.gameObject, Constants.LAYER_CARD);
                    this.ShowCard(card3, true);
                    card3.SortingOrder = this.GetCardSortingOrder(card3, num6);
                    LeanTween.cancel(card3.gameObject);
                    LeanTween.scale(card3.gameObject, this.GetCardScale(card3), 0.15f).setEase(LeanTweenType.easeInOutQuad);
                    card3.SetLastPosition(this.GetCardPosition(card3, num6, num3));
                    card3.MoveCard(this.GetCardPosition(card3, num6++, num3), 0.15f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                }
            }
        }
    }

    public void ReturnCard(Guid guid)
    {
        for (int i = 0; i < this.Deck.Count; i++)
        {
            if (this.Deck[i].GUID == guid)
            {
                this.Deck[i].Displayed = false;
                this.Deck[i].Revealed = false;
                break;
            }
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        for (int i = 0; i < this.Deck.Count; i++)
        {
            this.ShowCard(this.Deck[i], isVisible);
        }
    }

    private void ShowCard(Card card, bool isVisible)
    {
        if (isVisible)
        {
            if ((Game.GameType == GameType.LocalMultiPlayer) && (Turn.Number != Turn.Switch))
            {
                if ((Turn.State == GameStateType.Power) || (Turn.State == GameStateType.Penalty))
                {
                    if (((Turn.State == GameStateType.Power) && GameStatePower.Fulfilled) || ((Turn.State == GameStateType.Penalty) && GameStatePenalty.Fulfilled))
                    {
                        card.Show(CardSideType.Back);
                        return;
                    }
                    if (!GameStateShare.IsCardSharedAsFuel(card))
                    {
                        card.Show(CardSideType.Back);
                        return;
                    }
                    Card topPriorityCard = GameStateShare.GetTopPriorityCard(card.Deck);
                    if ((topPriorityCard == null) || (topPriorityCard.GUID != card.GUID))
                    {
                        card.Show(CardSideType.Back);
                        return;
                    }
                }
                else if (!GameStateShare.IsCardSharedAsCard(card))
                {
                    card.Show(CardSideType.Back);
                    return;
                }
            }
            card.Show(CardSideType.Front);
        }
        else
        {
            card.Show(false);
        }
    }

    private void Update()
    {
        if (this.Deck != null)
        {
            for (int i = 0; i < this.Deck.Count; i++)
            {
                Card card = this.Deck[i];
                if (card != null)
                {
                    if (((Turn.IsActionAllowed(ActionType.Reveal, card) || Turn.IsActionAllowed(ActionType.Display, card)) || (Turn.IsActionAllowed(ActionType.Recharge, card) || Turn.IsActionAllowed(ActionType.Discard, card))) || ((Turn.IsActionAllowed(ActionType.Bury, card) || Turn.IsActionAllowed(ActionType.Banish, card)) || ((Turn.IsActionAllowed(ActionType.Give, card) || Turn.IsActionAllowed(ActionType.Share, card)) || Turn.IsActionAllowed(ActionType.Top, card))))
                    {
                        card.Glow(true);
                    }
                    else
                    {
                        card.Glow(false);
                    }
                }
            }
        }
    }

    public override Deck Deck
    {
        get => 
            base.myDeck;
        set
        {
            base.myDeck = value;
            if (base.myDeck != null)
            {
                base.myDeck.Layout = this;
            }
            this.InitializeCards();
            this.Refresh();
        }
    }
}

