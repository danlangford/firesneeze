using System;
using UnityEngine;

public class GuiLayoutStack : GuiLayout
{
    [Tooltip("world space offset where the stack is drawn")]
    public Vector2 Offset = Vector2.zero;
    [Tooltip("amount of world space left between cards")]
    public Vector2 Spacing = Vector2.zero;

    protected override void Awake()
    {
        base.Awake();
        this.Deck = base.FindDeckChild("Deck");
        if (this.Deck != null)
        {
            this.Deck.Layout = this;
        }
    }

    private Vector3 GetCardPosition(int i)
    {
        Vector3 vector = base.transform.position + new Vector3(this.Offset.x, this.Offset.y, 0f);
        return (vector + ((Vector3) (i * new Vector3(this.Spacing.x, this.Spacing.y, 0f))));
    }

    private int GetCardSortingOrder(int index) => 
        -(this.Deck.Count - index);

    private bool IsActionAllowed(ActionType action, Card card) => 
        (Turn.IsActionAllowed(action, card) || ((base.CardAction == ActionType.Recharge) && Turn.IsActionAllowed(ActionType.Top, card)));

    public override bool IsDropPossible(Card card)
    {
        if (!base.IsDropPossible(card))
        {
            return false;
        }
        if (card.Deck == Location.Current.Deck)
        {
            return false;
        }
        return true;
    }

    public override bool OnGuiDrag(Card card)
    {
        if (card.Locked)
        {
            return false;
        }
        if (Turn.State == GameStateType.Examine)
        {
            return false;
        }
        if (string.IsNullOrEmpty(card.PlayedOwner))
        {
            return true;
        }
        if (((Turn.State == GameStateType.Power) || (Turn.State == GameStateType.Sacrifice)) && (card.Side == CardSideType.Front))
        {
            return false;
        }
        return (card.PlayedOwner == Turn.Character.ID);
    }

    public override bool OnGuiDrop(Card card)
    {
        if (!this.IsDropPossible(card))
        {
            return false;
        }
        if (!this.IsActionAllowed(base.CardAction, card))
        {
            return false;
        }
        if (card.Deck != this.Deck)
        {
            card.Glow(false);
            card.Revealed = false;
            card.Displayed = false;
            this.Deck.Add(card);
            card.Show(true);
            this.Refresh();
            Tutorial.Notify(this.GetTutorialEvent());
            base.PlayOnCardDroppedSfx();
        }
        return true;
    }

    public override void Refresh()
    {
        if (this.Deck != null)
        {
            int index = 0;
            for (int i = 0; i < this.Deck.Count; i++)
            {
                Card card = this.Deck[i];
                Geometry.SetLayerRecursively(card.gameObject, Constants.LAYER_CARD);
                card.SortingOrder = this.GetCardSortingOrder(index);
                LeanTween.cancel(card.gameObject);
                if (card.Visible)
                {
                    LeanTween.scale(card.gameObject, this.Scale, 0.15f).setEase(LeanTweenType.easeInOutQuad);
                    card.MoveCard(this.GetCardPosition(index), 0.15f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                }
                else
                {
                    card.transform.localScale = this.Scale;
                    card.transform.position = this.GetCardPosition(index);
                }
                if (string.IsNullOrEmpty(card.PlayedOwner) || (card.PlayedOwner == Turn.Character.ID))
                {
                    card.Show(true);
                    index++;
                }
                else
                {
                    card.Show(false);
                }
            }
        }
    }
}

