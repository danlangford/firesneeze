using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class Box
{
    [CompilerGenerated]
    private static Func<BoxCard, BoxCard> <>f__am$cache3;
    private List<BoxCard> Cards = new List<BoxCard>(500);
    private string ID;
    private List<string> myTables = new List<string>(10);

    public Box(string ID)
    {
        this.ID = ID;
    }

    public void Add(Card card, bool force = false)
    {
        if (card != null)
        {
            if (card.Deck != null)
            {
                card.Deck.Remove(card);
            }
            if (force || ((Adventure.Current != null) && (Adventure.Current.GetBoxDisposition(card) == DispositionType.Box)))
            {
                BoxCard item = new BoxCard(card);
                this.Cards.Add(item);
            }
            card.Destroy();
        }
    }

    public void Clear()
    {
        this.Cards.Clear();
        this.Tables.Clear();
    }

    public void Combine(Deck deck)
    {
        for (int i = deck.Count - 1; i >= 0; i--)
        {
            this.Add(deck[i], false);
        }
    }

    public void Cull(float percent, CardRankType rank)
    {
        int num = 0;
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.IsFilterMatch(this.Cards[i], rank))
            {
                num++;
            }
        }
        int num3 = Mathf.CeilToInt(num * percent);
        for (int j = 0; j < this.Cards.Count; j++)
        {
            if (num3 <= 0)
            {
                break;
            }
            if (this.IsFilterMatch(this.Cards[j], rank))
            {
                this.Cards.RemoveAt(j);
                num3--;
            }
        }
    }

    public Card Draw()
    {
        Card card = null;
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].Type != CardType.Loot)
            {
                card = this.Cards[i].Create();
                this.Cards.RemoveAt(i);
                return card;
            }
        }
        return card;
    }

    public Card Draw(CardSelector selector)
    {
        Card card = null;
        if (selector != null)
        {
            for (int i = 0; i < this.Cards.Count; i++)
            {
                if (selector.Match(this.Cards[i].ID))
                {
                    card = this.Cards[i].Create();
                    this.Cards.RemoveAt(i);
                    return card;
                }
            }
        }
        return card;
    }

    public Card Draw(CardType cardType)
    {
        if (cardType == CardType.None)
        {
            return this.Draw();
        }
        Card card = null;
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].Type == cardType)
            {
                card = this.Cards[i].Create();
                this.Cards.RemoveAt(i);
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
                card = this.Cards[i].Create();
                this.Cards.RemoveAt(i);
                return card;
            }
        }
        return card;
    }

    public Card Draw(CardType cardType, TraitType[] traits)
    {
        Card card = null;
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].Type == cardType)
            {
                int num2 = 0;
                for (int j = 0; j < traits.Length; j++)
                {
                    if (this.HasTrait(this.Cards[i].ID, traits[j]))
                    {
                        num2++;
                    }
                }
                if (num2 >= traits.Length)
                {
                    card = this.Cards[i].Create();
                    this.Cards.RemoveAt(i);
                    return card;
                }
            }
        }
        return card;
    }

    public string Dump(string title, bool verbose)
    {
        string str = string.Empty;
        string[] cardList = this.GetCardList(CardType.Weapon, CardRankType.None);
        if (verbose)
        {
            Debug.Log(string.Concat(new object[] { "Weapon: (", cardList.Length, ") ", string.Join(" ", cardList) }));
        }
        else
        {
            str = str + " WP:" + cardList.Length;
        }
        cardList = this.GetCardList(CardType.Item, CardRankType.None);
        if (verbose)
        {
            Debug.Log(string.Concat(new object[] { "Item: (", cardList.Length, ") ", string.Join(" ", cardList) }));
        }
        else
        {
            str = str + " IT:" + cardList.Length;
        }
        cardList = this.GetCardList(CardType.Blessing, CardRankType.None);
        if (verbose)
        {
            Debug.Log(string.Concat(new object[] { "Blessing: (", cardList.Length, ") ", string.Join(" ", cardList) }));
        }
        else
        {
            str = str + " BL:" + cardList.Length;
        }
        cardList = this.GetCardList(CardType.Spell, CardRankType.None);
        if (verbose)
        {
            Debug.Log(string.Concat(new object[] { "Spell: (", cardList.Length, ") ", string.Join(" ", cardList) }));
        }
        else
        {
            str = str + " SP:" + cardList.Length;
        }
        cardList = this.GetCardList(CardType.Armor, CardRankType.None);
        if (verbose)
        {
            Debug.Log(string.Concat(new object[] { "Armor: (", cardList.Length, ") ", string.Join(" ", cardList) }));
        }
        else
        {
            str = str + " AR:" + cardList.Length;
        }
        cardList = this.GetCardList(CardType.Ally, CardRankType.None);
        if (verbose)
        {
            Debug.Log(string.Concat(new object[] { "Ally: (", cardList.Length, ") ", string.Join(" ", cardList) }));
        }
        else
        {
            str = str + " AL:" + cardList.Length;
        }
        cardList = this.GetCardList(CardType.Barrier, CardRankType.None);
        if (verbose)
        {
            Debug.Log(string.Concat(new object[] { "Barrier: (", cardList.Length, ") ", string.Join(" ", cardList) }));
        }
        else
        {
            str = str + " BX:" + cardList.Length;
        }
        cardList = this.GetCardList(CardType.Monster, CardRankType.None);
        if (verbose)
        {
            Debug.Log(string.Concat(new object[] { "Monster: (", cardList.Length, ") ", string.Join(" ", cardList) }));
        }
        else
        {
            str = str + " MO:" + cardList.Length;
        }
        cardList = this.GetCardList(CardType.Henchman, CardRankType.None);
        if (verbose)
        {
            Debug.Log(string.Concat(new object[] { "Henchman: (", cardList.Length, ") ", string.Join(" ", cardList) }));
        }
        else
        {
            str = str + " HE:" + cardList.Length;
        }
        cardList = this.GetCardList(CardType.Villain, CardRankType.None);
        if (verbose)
        {
            Debug.Log(string.Concat(new object[] { "Villain: (", cardList.Length, ") ", string.Join(" ", cardList) }));
        }
        else
        {
            str = str + " VL:" + cardList.Length;
        }
        cardList = this.GetCardList(CardType.Loot, CardRankType.None);
        if (verbose)
        {
            Debug.Log(string.Concat(new object[] { "Loot: (", cardList.Length, ") ", string.Join(" ", cardList) }));
        }
        else
        {
            str = str + " LT:" + cardList.Length;
        }
        if (string.IsNullOrEmpty(title) && (Location.Current != null))
        {
            title = Location.Current.ID;
        }
        object[] objArray12 = new object[] { "[", title, "] Box (", this.Count, ") : ", str };
        string message = string.Concat(objArray12);
        if (!verbose)
        {
            Debug.Log(message);
        }
        return message;
    }

    public int Filter(CardType type, BoosterFilterType boosterType)
    {
        int num = 0;
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if ((this.Cards[i].Type == type) || (type == CardType.None))
            {
                bool cardBooster = CardTable.LookupCardBooster(this.Cards[i].ID);
                switch (boosterType)
                {
                    case BoosterFilterType.Off:
                        if (!cardBooster)
                        {
                            num++;
                        }
                        break;

                    case BoosterFilterType.On:
                        num++;
                        break;

                    case BoosterFilterType.Only:
                        if (cardBooster)
                        {
                            num++;
                        }
                        break;

                    case BoosterFilterType.Owned:
                        if (!cardBooster)
                        {
                            num++;
                        }
                        if (cardBooster && Collection.Contains(this.Cards[i].ID))
                        {
                            num++;
                        }
                        break;
                }
            }
        }
        return num;
    }

    public string[] GetBoxList(string file, string prefix)
    {
        List<string> list = new List<string>();
        TextAsset asset = (TextAsset) Resources.Load(file, typeof(TextAsset));
        if (asset != null)
        {
            StringReader reader = new StringReader(asset.text);
            while (true)
            {
                string str = reader.ReadLine();
                if (str == null)
                {
                    break;
                }
                if (!str.StartsWith("//"))
                {
                    char[] separator = new char[] { '\t' };
                    string[] strArray = str.Split(separator);
                    if ((strArray.Length == 2) && strArray[1].StartsWith(prefix))
                    {
                        list.Add(strArray[1]);
                    }
                }
            }
        }
        return list.ToArray();
    }

    public string[] GetCardList(CardType type, CardRankType filter)
    {
        List<string> list = new List<string>();
        for (int i = 0; i < this.Cards.Count; i++)
        {
            BoxCard card = this.Cards[i];
            if ((card.Type == type) && this.IsFilterMatch(card, filter))
            {
                list.Add(card.ID);
            }
        }
        return list.ToArray();
    }

    public string[] GetCardList(CardType type, int set)
    {
        List<string> list = new List<string>();
        for (int i = 0; i < this.Cards.Count; i++)
        {
            BoxCard card = this.Cards[i];
            if ((card.Type == type) && this.IsFilterMatch(card, set))
            {
                list.Add(card.ID);
            }
        }
        return list.ToArray();
    }

    private bool HasTrait(string ID, TraitType trait)
    {
        TraitType[] cardTraits = CardTable.LookupCardTraits(ID);
        if (cardTraits != null)
        {
            for (int i = 0; i < cardTraits.Length; i++)
            {
                if (cardTraits[i] == trait)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsFilterMatch(BoxCard card, CardRankType filter) => 
        ((filter == CardRankType.None) || (((filter == CardRankType.Basic) && this.HasTrait(card.ID, TraitType.Basic)) || ((filter == CardRankType.Elite) && this.HasTrait(card.ID, TraitType.Elite))));

    private bool IsFilterMatch(BoxCard card, int filter)
    {
        if (card.Set == "B")
        {
            return true;
        }
        if (card.Set == "C")
        {
            return true;
        }
        if (card.Set == "1")
        {
            return (filter >= 1);
        }
        if (card.Set == "2")
        {
            return (filter >= 2);
        }
        if (card.Set == "3")
        {
            return (filter >= 3);
        }
        if (card.Set == "4")
        {
            return (filter >= 4);
        }
        if (card.Set == "5")
        {
            return (filter >= 5);
        }
        return ((card.Set == "6") && (filter >= 6));
    }

    public void Load(string file, string set)
    {
        string str;
        TextAsset asset = (TextAsset) Resources.Load(file, typeof(TextAsset));
        if (asset == null)
        {
            return;
        }
        if (!this.Tables.Contains(file))
        {
            this.Tables.Add(file);
        }
        StringReader reader = new StringReader(asset.text);
    Label_004B:
        str = reader.ReadLine();
        if (str == null)
        {
            this.Shuffle();
        }
        else
        {
            if (!str.StartsWith("//"))
            {
                char[] separator = new char[] { '\t' };
                string[] strArray = str.Split(separator);
                if (strArray.Length == 2)
                {
                    int num = int.Parse(strArray[0]);
                    string id = strArray[1];
                    CardType cardType = CardTable.LookupCardType(id);
                    for (int i = 0; i < num; i++)
                    {
                        BoxCard item = new BoxCard(id, cardType, set);
                        this.Cards.Add(item);
                    }
                }
            }
            goto Label_004B;
        }
    }

    public void OnLoadData()
    {
        byte[] buffer;
        if (Game.GetObjectData(this.ID, out buffer))
        {
            ByteStream stream = new ByteStream(buffer);
            if (stream != null)
            {
                stream.ReadInt();
                int num = stream.ReadInt();
                for (int i = 0; i < num; i++)
                {
                    this.Tables.Add(stream.ReadString());
                }
                num = stream.ReadInt();
                for (int j = 0; j < num; j++)
                {
                    string iD = stream.ReadString();
                    CardType type = (CardType) stream.ReadInt();
                    string set = stream.ReadString();
                    BoxCard item = new BoxCard(iD, type, set);
                    this.Cards.Add(item);
                }
            }
        }
    }

    public void OnSaveData()
    {
        ByteStream stream = new ByteStream();
        if (stream != null)
        {
            stream.WriteInt(1);
            stream.WriteInt(this.Tables.Count);
            for (int i = 0; i < this.Tables.Count; i++)
            {
                stream.WriteString(this.Tables[i]);
            }
            stream.WriteInt(this.Cards.Count);
            for (int j = 0; j < this.Cards.Count; j++)
            {
                stream.WriteString(this.Cards[j].ID);
                stream.WriteInt((int) this.Cards[j].Type);
                stream.WriteString(this.Cards[j].Set);
            }
            Game.SetObjectData(this.ID, stream.ToArray());
        }
    }

    public CardIdentity Pull(CardType cardType)
    {
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].Type == cardType)
            {
                CardIdentity identity = new CardIdentity(this.Cards[i].ID, this.Cards[i].Set);
                this.Cards.RemoveAt(i);
                return identity;
            }
        }
        return null;
    }

    public CardIdentity Pull(string id)
    {
        for (int i = 0; i < this.Cards.Count; i++)
        {
            if (this.Cards[i].ID == id)
            {
                CardIdentity identity = new CardIdentity(this.Cards[i].ID, this.Cards[i].Set);
                this.Cards.RemoveAt(i);
                return identity;
            }
        }
        return null;
    }

    public void Push(Card card, bool force = false)
    {
        CardIdentity identity = new CardIdentity(card);
        this.Push(identity, force);
    }

    public void Push(CardIdentity identity, bool force = false)
    {
        if (force || ((Adventure.Current != null) && (Adventure.Current.GetBoxDisposition(identity.ID) == DispositionType.Box)))
        {
            CardType cardType = CardTable.LookupCardType(identity.ID);
            if (cardType != CardType.None)
            {
                BoxCard item = new BoxCard(identity.ID, cardType, identity.Set);
                this.Cards.Add(item);
            }
        }
    }

    public void Remove(Card card)
    {
        if (card != null)
        {
            for (int i = 0; i < this.Cards.Count; i++)
            {
                if (this.Cards[i].ID == card.ID)
                {
                    this.Cards.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void Shuffle()
    {
        this.Cards.Shuffle<BoxCard>();
    }

    public int Count =>
        this.Cards.Count;

    public int CountUnique
    {
        get
        {
            int num = 0;
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = id => id;
            }
            IEnumerator<IGrouping<BoxCard, BoxCard>> enumerator = this.Cards.GroupBy<BoxCard, BoxCard>(<>f__am$cache3).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IGrouping<BoxCard, BoxCard> current = enumerator.Current;
                    num += current.Count<BoxCard>();
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return num;
        }
    }

    public List<string> Tables =>
        this.myTables;

    private class BoxCard
    {
        public string ID;
        public string Set;
        public CardType Type;

        public BoxCard(Card card)
        {
            this.ID = card.ID;
            this.Type = card.Type;
            this.Set = card.Set;
        }

        public BoxCard(string ID, CardType type, string set)
        {
            this.ID = ID;
            this.Type = type;
            this.Set = set;
        }

        public Card Create() => 
            CardTable.Create(this.ID, this.Set, null);
    }
}

