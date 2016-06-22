using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiLayoutCard : GuiElement
{
    [Tooltip("scale of a card displayed in this layout")]
    public float CardSize = 0.25f;
    [Tooltip("unique id used to save or load")]
    public string GUID;
    private Card myCard;

    public void Animate(ActionType animation)
    {
        if (this.myCard != null)
        {
            if (animation == ActionType.Banish)
            {
                LeanTween.scale(this.myCard.gameObject, Vector3.zero, 0.4f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(new Action(this.Clear));
            }
            if (animation == ActionType.Display)
            {
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (window != null)
                {
                    this.myCard.transform.position = window.layoutLocation.transform.position;
                    this.myCard.transform.localScale = window.layoutLocation.Scale;
                    this.myCard.MoveCard(base.transform.position, 0.4f).setEase(LeanTweenType.easeInOutQuad);
                    LeanTween.scale(this.myCard.gameObject, this.Scale, 0.4f).setEase(LeanTweenType.easeInOutQuad);
                }
            }
        }
    }

    public void Clear()
    {
        if (this.myCard != null)
        {
            UnityEngine.Object.Destroy(this.myCard.gameObject);
            this.myCard = null;
        }
    }

    public void OnLoadData()
    {
        byte[] buffer;
        if (!string.IsNullOrEmpty(this.GUID) && Game.GetObjectData(this.GUID, out buffer))
        {
            ByteStream stream = new ByteStream(buffer);
            if (stream != null)
            {
                stream.ReadInt();
                string str = stream.ReadString();
                if (!string.IsNullOrEmpty(str))
                {
                    this.Show(str);
                }
            }
        }
    }

    public void OnSaveData()
    {
        if (!string.IsNullOrEmpty(this.GUID))
        {
            ByteStream stream = new ByteStream();
            if (stream != null)
            {
                stream.WriteInt(1);
                if (this.myCard != null)
                {
                    stream.WriteString(this.myCard.ID);
                }
                else
                {
                    stream.WriteString(string.Empty);
                }
                Game.SetObjectData(this.GUID, stream.ToArray());
            }
        }
    }

    private void RefreshLocationLayout()
    {
        base.StartCoroutine(this.RefreshLocationLayoutCoroutine());
    }

    [DebuggerHidden]
    private IEnumerator RefreshLocationLayoutCoroutine() => 
        new <RefreshLocationLayoutCoroutine>c__Iterator46();

    public Card Show(string ID)
    {
        if (this.myCard != null)
        {
            this.Clear();
        }
        this.myCard = CardTable.Create(ID);
        if (this.myCard != null)
        {
            this.myCard.transform.parent = base.transform;
            this.myCard.transform.localScale = this.Scale;
            this.myCard.transform.position = base.transform.position;
            this.myCard.Show(true);
            Geometry.SetLayerRecursively(this.myCard.gameObject, Constants.LAYER_CARD);
        }
        return this.myCard;
    }

    public Card Summon(string cardID)
    {
        Card card = SummonsSelector.Summon(cardID);
        if (card != null)
        {
            Location.Current.Deck.Add(card, DeckPositionType.Top);
            card.transform.position = base.transform.position + Vector3.right;
            card.transform.localScale = this.Scale;
            card.Show(true);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                Vector3[] destinations = Geometry.GetCurve(card.transform.position, window.layoutLocation.transform.position, 1f);
                card.MoveCard(destinations, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                LeanTween.scale(card.gameObject, window.layoutLocation.Scale, 0.3f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(new Action(this.RefreshLocationLayout));
            }
        }
        return card;
    }

    public Card Card =>
        this.myCard;

    public virtual Vector3 Scale =>
        new Vector3(this.CardSize, this.CardSize, 1f);

    [CompilerGenerated]
    private sealed class <RefreshLocationLayoutCoroutine>c__Iterator46 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowLocation <window>__0;

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
                    this.$current = null;
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    if (this.<window>__0 != null)
                    {
                        this.<window>__0.layoutLocation.Show(true);
                        this.<window>__0.layoutLocation.Refresh();
                    }
                    this.$PC = -1;
                    break;
            }
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

