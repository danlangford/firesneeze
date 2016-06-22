using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Deck : MonoBehaviour
{
    private List<Card> Cards = new List<Card>();

    public event EventHandlerDeckChanged Changed;

    public void Add(Card card)
    {
        if (card != null)
        {
            if (card.Deck != null)
            {
                if ((this.Layout == null) || (((this.Layout.CardAction != ActionType.Draw) && (this.Layout.CardAction != ActionType.Reveal)) && (this.Layout.CardAction != ActionType.Display)))
                {
                    card.PreviousDeck = card.Deck;
                }
                card.Deck.Remove(card);
            }
            this.Cards.Add(card);
            card.Deck = this;
            card.transform.parent = base.transform;
            card.Show(false);
            this.RaiseChangedEvent(card.Type, 1);
        }
    }

    public void Add(Card card, DeckPositionType position)
    {
        if (card != null)
        {
            this.Add(card);
            if (position == DeckPositionType.Shuffle)
            {
                this.Shuffle();
            }
            if (position == DeckPositionType.Top)
            {
                this.Move(this.Count - 1, 0);
            }
            if ((position == DeckPositionType.UnderTop) && (this.Cards.Count > 1))
            {
                this.Move(this.Count - 1, 1);
            }
        }
    }

    public void Clear()
    {
        int count = this.Cards.Count;
        for (int i = this.Cards.Count - 1; i >= 0; i--)
        {
            Campaign.Box.Add(this.Cards[i], false);
        }
        this.Cards.Clear();
        this.RaiseChangedEvent(CardType.None, -count);
    }

    public void Combine(Deck deck)
    {
        for (int i = deck.Count - 1; i >= 0; i--)
        {
            this.Add(deck[i]);
        }
    }

    public bool Contains(Card card)
    {
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].ID == card.ID)
            {
                return true;
            }
        }
        return false;
    }

    public bool Contains(string id)
    {
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].ID == id)
            {
                return true;
            }
        }
        return false;
    }

    public void Destroy(string ID)
    {
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].ID == ID)
            {
                Card card = this.Cards[i];
                this.Cards.RemoveAt(i);
                UnityEngine.Object.Destroy(card.gameObject);
                break;
            }
        }
    }

    public Card Draw()
    {
        Card card = null;
        if (this.Cards.Count >= 1)
        {
            card = this.Cards[0];
            this.Cards.RemoveAt(0);
            card.transform.parent = null;
            card.Deck = null;
            this.RaiseChangedEvent(card.Type, -1);
        }
        return card;
    }

    public Card Draw(CardType cardType)
    {
        Card card = null;
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].Type == cardType)
            {
                card = this.Cards[i];
                this.Cards.RemoveAt(i);
                card.Deck = null;
                this.RaiseChangedEvent(card.Type, -1);
                return card;
            }
        }
        return card;
    }

    public Card Draw(string ID)
    {
        Card card = null;
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].ID == ID)
            {
                card = this.Cards[i];
                this.Cards.RemoveAt(i);
                card.Deck = null;
                this.RaiseChangedEvent(card.Type, -1);
                return card;
            }
        }
        return card;
    }

    public int Filter(CardFilter filter)
    {
        if (filter == null)
        {
            return this.Cards.Count;
        }
        int num = 0;
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (filter.Match(this.Cards[i]))
            {
                num++;
            }
        }
        return num;
    }

    public int Filter(CardType filter)
    {
        int num = 0;
        if (filter == CardType.None)
        {
            return this.Cards.Count;
        }
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].Type == filter)
            {
                num++;
            }
        }
        return num;
    }

    public int Filter(TraitType filter)
    {
        int num = 0;
        if (filter == TraitType.None)
        {
            return this.Cards.Count;
        }
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].HasTrait(filter))
            {
                num++;
            }
        }
        return num;
    }

    public void FromStream(ByteStream bs)
    {
        this.Cards.Clear();
        bs.ReadInt();
        int num = bs.ReadInt();
        for (int i = 0; i < num; i++)
        {
            this.Add(CardData.CardFromStream(bs));
        }
    }

    public string[] GetCardList()
    {
        string[] strArray = new string[this.Cards.Count];
        for (int i = 0; i < this.Cards.Count; i++)
        {
            strArray[i] = this.Cards[i].ID;
        }
        return strArray;
    }

    public int IndexOf(Card card)
    {
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i] == card)
            {
                return i;
            }
        }
        return -1;
    }

    public void Move(int oldIndex, int newIndex)
    {
        if (oldIndex != newIndex)
        {
            Card item = this.Cards[oldIndex];
            this.Cards.RemoveAt(oldIndex);
            this.Cards.Insert(newIndex, item);
        }
    }

    public void Prepare()
    {
        int newIndex = 0;
        if ((this.Count > 0) && (this.Cards[0].GUID.Equals(Turn.EncounteredGuid) || (this.Cards[0].Blocker == BlockerType.Movement)))
        {
            newIndex = 1;
        }
        for (int i = 1; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].Blocker == BlockerType.Movement)
            {
                this.Move(i, newIndex);
                newIndex = i + 1;
            }
        }
    }

    private void RaiseChangedEvent(CardType card, int changeInSize)
    {
        EventHandlerDeckChanged changed = this.Changed;
        if (changed != null)
        {
            changed(this, new EventArgsCard(card, changeInSize));
        }
    }

    public void Remove(Card card)
    {
        if (card != null)
        {
            this.Cards.Remove(card);
            card.Deck = null;
            card.transform.parent = null;
            card.Show(false);
            this.RaiseChangedEvent(card.Type, -1);
        }
    }

    public void Seek(string ID)
    {
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].ID == ID)
            {
                this.Move(i, 0);
                break;
            }
        }
    }

    public void SetCardList(string[] cardList)
    {
        this.Cards.Clear();
        for (int i = 0; i < cardList.Length; i++)
        {
            Card card = CardTable.Create(cardList[i]);
            this.Add(card);
        }
    }

    public void Shuffle()
    {
        for (int i = 0; i < this.Cards.Count; i++)
        {
            this.Cards[i].Known = false;
        }
        this.Cards.Shuffle<Card>();
    }

    public void ShuffleUnderTop()
    {
        if (this.Cards.Count > 1)
        {
            Card card = this.Cards[0];
            Card card2 = null;
            if (Rules.IsCardSummons(card) && (this.Cards.Count > 2))
            {
                card2 = this.Cards[1];
                this.Remove(card2);
            }
            this.Remove(card);
            this.Shuffle();
            if (card2 != null)
            {
                this.Add(card2, DeckPositionType.Top);
            }
            this.Add(card, DeckPositionType.Top);
        }
    }

    public void Sort()
    {
        IComparer<Card> comparer = new TypeSortByName();
        this.Cards.Sort(comparer);
    }

    public void Sort(BoosterFilterType type)
    {
        IComparer<Card> comparer = new TypeSortByKnownName();
        this.Cards.Sort(comparer);
    }

    public void Sort(Deck deck)
    {
        IComparer<Card> comparer = new PrioritySortByName(deck);
        this.Cards.Sort(comparer);
    }

    public void Sort(string[] ids)
    {
        if ((ids != null) && (ids.Length <= this.Cards.Count))
        {
            for (int i = 0; i < ids.Length; i++)
            {
                if (this.Cards[i].ID != ids[i])
                {
                    for (int j = i; j < this.Cards.Count; j++)
                    {
                        if (this.Cards[j].ID == ids[i])
                        {
                            Card card = this.Cards[i];
                            this.Cards[i] = this.Cards[j];
                            this.Cards[j] = card;
                        }
                    }
                }
            }
        }
    }

    public void Sort(CardFilter filter)
    {
        IComparer<Card> comparer = new PriortySortByFilter(filter);
        this.Cards.Sort(comparer);
    }

    public void Swap(int indexOne, int indexTwo)
    {
        if (indexOne != indexTwo)
        {
            this.Cards.Swap<Card>(indexOne, indexTwo);
        }
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteInt(1);
        bs.WriteInt(this.Cards.Count);
        for (int i = 0; i < this.Cards.Count; i++)
        {
            CardData.CardToStream(this.Cards[i], bs);
        }
    }

    public int Count =>
        this.Cards.Count;

    public int CountUnique
    {
        get
        {
            int num = 0;
            string iD = null;
            for (int i = 0; i < this.Cards.Count; i++)
            {
                if (this.Cards[i].ID != iD)
                {
                    iD = this.Cards[i].ID;
                    num++;
                }
            }
            return num;
        }
    }

    public Card this[int index] =>
        this.Cards[index];

    public Card this[string ID]
    {
        get
        {
            for (int i = 0; i < this.Cards.Count; i++)
            {
                if (this.Cards[i].ID == ID)
                {
                    return this.Cards[i];
                }
            }
            return null;
        }
    }

    public Card this[Guid guid]
    {
        get
        {
            for (int i = 0; i < this.Cards.Count; i++)
            {
                if (this.Cards[i].GUID == guid)
                {
                    return this.Cards[i];
                }
            }
            return null;
        }
    }

    public GuiLayout Layout { get; set; }

    private class AlphaSortByName : IComparer<Card>
    {
        public int Compare(Card x, Card y) => 
            x.DisplayName.CompareTo(y.DisplayName);
    }

    public delegate void EventHandlerDeckChanged(object sender, EventArgsCard e);

    private class PrioritySortByName : IComparer<Card>
    {
        private Deck first;

        public PrioritySortByName(Deck deck)
        {
            this.first = deck;
        }

        public int Compare(Card x, Card y)
        {
            if (this.first.Contains(x.ID) && !this.first.Contains(y.ID))
            {
                return -1;
            }
            if (!this.first.Contains(x.ID) && this.first.Contains(y.ID))
            {
                return 1;
            }
            return x.DisplayName.CompareTo(y.DisplayName);
        }
    }

    private class PriortySortByFilter : IComparer<Card>
    {
        private CardFilter filter;

        public PriortySortByFilter(CardFilter filter)
        {
            this.filter = filter;
        }

        public int Compare(Card x, Card y)
        {
            if (this.filter.Match(x) && this.filter.Match(y))
            {
                return x.DisplayName.CompareTo(y.DisplayName);
            }
            if (!this.filter.Match(x) && !this.filter.Match(y))
            {
                return x.DisplayName.CompareTo(y.DisplayName);
            }
            return (!this.filter.Match(x) ? 1 : -1);
        }
    }

    private class TypeSortByKnownName : Deck.TypeSortByName
    {
        public override int Compare(Card x, Card y)
        {
            bool flag = this.IsKnown(x);
            bool flag2 = this.IsKnown(y);
            if (flag && !flag2)
            {
                return -1;
            }
            if (!flag && flag2)
            {
                return 1;
            }
            return base.Compare(x, y);
        }

        private bool IsKnown(Card card)
        {
            if (CardTable.LookupCardBooster(card.ID) && !Collection.Contains(card.ID))
            {
                return false;
            }
            return true;
        }
    }

    private class TypeSortByName : IComparer<Card>
    {
        public virtual int Compare(Card x, Card y)
        {
            if (x.Type != y.Type)
            {
                if (x.Type < y.Type)
                {
                    return -1;
                }
                if (x.Type > y.Type)
                {
                    return 1;
                }
            }
            return x.DisplayName.CompareTo(y.DisplayName);
        }
    }
}

