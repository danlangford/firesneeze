using System;
using System.Linq;

public class CardFilter
{
    public string[] CardIDs = new string[0];
    public TraitType[] CardTraits = new TraitType[0];
    public CardType[] CardTypes = new CardType[0];
    public SelectorComparisonType ComparisonType = SelectorComparisonType.None;
    public Guid ExcludedCard = Guid.Empty;
    public CardType[] ExcludedCardTypes = new CardType[0];
    public string[] Exclusions = new string[0];
    public DeckPositionType Position = DeckPositionType.None;
    public bool Strict = true;

    private static bool All(Card card, CardType[] CardTypes, TraitType[] CardTraits, string[] CardIDs, DeckPositionType Position, bool Strict)
    {
        for (int i = 0; i < CardTraits.Length; i++)
        {
            if (!card.HasTrait(CardTraits[i]) && (CardTraits[i] != TraitType.None))
            {
                return false;
            }
        }
        for (int j = 0; j < CardTypes.Length; j++)
        {
            if (((card.Type != CardTypes[j]) && (card.SubType != CardTypes[j])) && (CardTypes[j] != CardType.None))
            {
                if (!Strict)
                {
                    if (card.SubType != CardTypes[j])
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        for (int k = 0; k < CardIDs.Length; k++)
        {
            if (CardIDs[k] != card.ID)
            {
                return false;
            }
        }
        return true;
    }

    private static bool All(string CardID, CardType[] CardTypes, TraitType[] CardTraits, string[] CardIDs, DeckPositionType Position, bool Strict)
    {
        for (int i = 0; i < CardTraits.Length; i++)
        {
            if (CardTraits[i] == TraitType.None)
            {
                continue;
            }
            bool flag = false;
            TraitType[] cardTraits = CardTable.LookupCardTraits(CardID);
            if (cardTraits != null)
            {
                for (int m = 0; m < cardTraits.Length; m++)
                {
                    if (cardTraits[m] == CardTraits[i])
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (!flag)
            {
                return false;
            }
        }
        CardType cardType = CardTable.LookupCardType(CardID);
        for (int j = 0; j < CardTypes.Length; j++)
        {
            if ((cardType != CardTypes[j]) && (CardTypes[j] != CardType.None))
            {
                return false;
            }
        }
        for (int k = 0; k < CardIDs.Length; k++)
        {
            if (CardIDs[k] != CardID)
            {
                return false;
            }
        }
        return true;
    }

    private static bool And1(Card card, CardType[] CardTypes, TraitType[] CardTraits, string[] CardIDs, DeckPositionType Position, bool Strict)
    {
        bool flag = CardTypes.Length <= 0;
        for (int i = 0; i < CardTypes.Length; i++)
        {
            if ((!Strict && (card.SubType == CardTypes[i])) || (card.Type == CardTypes[i]))
            {
                flag = true;
                break;
            }
        }
        bool flag2 = CardTraits.Length <= 0;
        for (int j = 0; j < CardTraits.Length; j++)
        {
            if (card.HasTrait(CardTraits[j]))
            {
                flag2 = true;
                break;
            }
        }
        bool flag3 = CardIDs.Length <= 0;
        for (int k = 0; k < CardIDs.Length; k++)
        {
            if (card.ID == CardIDs[k])
            {
                flag3 = true;
                break;
            }
        }
        return ((flag && flag2) && flag3);
    }

    private static bool And1(string CardID, CardType[] CardTypes, TraitType[] CardTraits, string[] CardIDs, DeckPositionType Position, bool Strict)
    {
        bool flag = CardTypes.Length <= 0;
        CardType cardType = CardTable.LookupCardType(CardID);
        for (int i = 0; i < CardTypes.Length; i++)
        {
            if (cardType == CardTypes[i])
            {
                flag = true;
                break;
            }
        }
        bool flag2 = CardTraits.Length <= 0;
        TraitType[] cardTraits = CardTable.LookupCardTraits(CardID);
        for (int j = 0; (j < CardTraits.Length) && !flag2; j++)
        {
            for (int m = 0; (m < cardTraits.Length) && !flag2; m++)
            {
                if (CardTraits[j] == cardTraits[m])
                {
                    flag2 = true;
                }
            }
        }
        bool flag3 = CardIDs.Length <= 0;
        for (int k = 0; k < CardIDs.Length; k++)
        {
            if (CardID == CardIDs[k])
            {
                flag3 = true;
                break;
            }
        }
        return ((flag && flag2) && flag3);
    }

    public static bool Comparison(Card card, CardFilter filter)
    {
        if (filter.IsExcluded(card))
        {
            return false;
        }
        if (filter.ComparisonType == SelectorComparisonType.All)
        {
            return All(card, filter.CardTypes, filter.CardTraits, filter.CardIDs, filter.Position, filter.Strict);
        }
        if (filter.ComparisonType == SelectorComparisonType.Precise)
        {
            return Precise(card, filter.CardTypes, filter.CardTraits, filter.CardIDs, filter.Position, filter.Strict);
        }
        if (filter.ComparisonType == SelectorComparisonType.None)
        {
            return Loose(card, filter.CardTypes, filter.CardTraits, filter.CardIDs, filter.Position, filter.Strict);
        }
        return ((filter.ComparisonType == SelectorComparisonType.And1) && And1(card, filter.CardTypes, filter.CardTraits, filter.CardIDs, filter.Position, filter.Strict));
    }

    public static bool Comparison(Card card, CardSelector selector)
    {
        if (selector.IsExcluded(card))
        {
            return false;
        }
        if (selector.ComparisonType == SelectorComparisonType.All)
        {
            return All(card, selector.CardTypes, selector.CardTraits, selector.CardIDs, DeckPositionType.None, selector.Strict);
        }
        if (selector.ComparisonType == SelectorComparisonType.Precise)
        {
            return Precise(card, selector.CardTypes, selector.CardTraits, selector.CardIDs, DeckPositionType.None, selector.Strict);
        }
        if (selector.ComparisonType == SelectorComparisonType.None)
        {
            return Loose(card, selector.CardTypes, selector.CardTraits, selector.CardIDs, DeckPositionType.None, selector.Strict);
        }
        return ((selector.ComparisonType == SelectorComparisonType.And1) && And1(card, selector.CardTypes, selector.CardTraits, selector.CardIDs, DeckPositionType.None, selector.Strict));
    }

    public static bool Comparison(string cardID, CardSelector selector)
    {
        if (selector.IsExcluded(cardID))
        {
            return false;
        }
        if (selector.ComparisonType == SelectorComparisonType.All)
        {
            return All(cardID, selector.CardTypes, selector.CardTraits, selector.CardIDs, DeckPositionType.None, selector.Strict);
        }
        if (selector.ComparisonType == SelectorComparisonType.Precise)
        {
            return Precise(cardID, selector.CardTypes, selector.CardTraits, selector.CardIDs, DeckPositionType.None, selector.Strict);
        }
        if (selector.ComparisonType == SelectorComparisonType.None)
        {
            return Loose(cardID, selector.CardTypes, selector.CardTraits, selector.CardIDs, DeckPositionType.None, selector.Strict);
        }
        return ((selector.ComparisonType == SelectorComparisonType.And1) && And1(cardID, selector.CardTypes, selector.CardTraits, selector.CardIDs, DeckPositionType.None, selector.Strict));
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        CardFilter filter = obj as CardFilter;
        if (filter == null)
        {
            return false;
        }
        if (!this.CardTypes.SequenceEqual<CardType>(filter.CardTypes))
        {
            return false;
        }
        if (!this.CardTraits.SequenceEqual<TraitType>(filter.CardTraits))
        {
            return false;
        }
        if (!this.CardIDs.SequenceEqual<string>(filter.CardIDs))
        {
            return false;
        }
        if (this.Position != filter.Position)
        {
            return false;
        }
        if (this.ComparisonType != filter.ComparisonType)
        {
            return false;
        }
        if (this.Strict != filter.Strict)
        {
            return false;
        }
        if (!this.Exclusions.SequenceEqual<string>(filter.Exclusions))
        {
            return false;
        }
        if (!this.ExcludedCard.Equals(filter.ExcludedCard))
        {
            return false;
        }
        if (!this.ExcludedCardTypes.SequenceEqual<CardType>(filter.ExcludedCardTypes))
        {
            return false;
        }
        return true;
    }

    public static CardFilter FromStream(ByteStream bs)
    {
        if (!bs.ReadBool())
        {
            return null;
        }
        bs.ReadInt();
        CardFilter filter = new CardFilter {
            CardTypes = new CardType[bs.ReadInt()]
        };
        for (int i = 0; i < filter.CardTypes.Length; i++)
        {
            filter.CardTypes[i] = (CardType) bs.ReadInt();
        }
        filter.CardTraits = new TraitType[bs.ReadInt()];
        for (int j = 0; j < filter.CardTraits.Length; j++)
        {
            filter.CardTraits[j] = (TraitType) bs.ReadInt();
        }
        filter.CardIDs = new string[bs.ReadInt()];
        for (int k = 0; k < filter.CardIDs.Length; k++)
        {
            filter.CardIDs[k] = bs.ReadString();
        }
        filter.Exclusions = new string[bs.ReadInt()];
        for (int m = 0; m < filter.Exclusions.Length; m++)
        {
            filter.Exclusions[m] = bs.ReadString();
        }
        filter.ExcludedCardTypes = new CardType[bs.ReadInt()];
        for (int n = 0; n < filter.ExcludedCardTypes.Length; n++)
        {
            filter.ExcludedCardTypes[n] = (CardType) bs.ReadInt();
        }
        filter.Position = (DeckPositionType) bs.ReadInt();
        filter.ComparisonType = (SelectorComparisonType) bs.ReadInt();
        filter.Strict = bs.ReadBool();
        filter.ExcludedCard = bs.ReadGuid();
        return filter;
    }

    public override int GetHashCode() => 
        base.GetHashCode();

    private bool IsEmpty() => 
        ((((this.CardTypes.Length == 0) && (this.CardTraits.Length == 0)) && ((this.CardIDs.Length == 0) && (this.Position == DeckPositionType.None))) && (this.ExcludedCard == Guid.Empty));

    public bool IsExcluded(Card card)
    {
        for (int i = 0; i < this.Exclusions.Length; i++)
        {
            if (card.ID == this.Exclusions[i])
            {
                return true;
            }
        }
        if ((this.ExcludedCard == card.GUID) && (this.ExcludedCard != Guid.Empty))
        {
            return true;
        }
        for (int j = 0; j < this.ExcludedCardTypes.Length; j++)
        {
            if (card.Type == this.ExcludedCardTypes[j])
            {
                return true;
            }
        }
        return false;
    }

    private bool IsFilteredType(CardType type)
    {
        for (int i = 0; i < this.CardTypes.Length; i++)
        {
            if (this.CardTypes[i] == type)
            {
                return true;
            }
        }
        return false;
    }

    private static bool Loose(Card card, CardType[] CardTypes, TraitType[] CardTraits, string[] CardIDs, DeckPositionType Position, bool Strict)
    {
        for (int i = 0; i < CardTypes.Length; i++)
        {
            if (CardTypes[i] == CardType.None)
            {
                return true;
            }
            if (card.Type == CardTypes[i])
            {
                return true;
            }
            if (!Strict && (card.SubType == CardTypes[i]))
            {
                return true;
            }
        }
        for (int j = 0; j < CardTraits.Length; j++)
        {
            if (CardTraits[j] == TraitType.None)
            {
                return true;
            }
            if (card.HasTrait(CardTraits[j]))
            {
                return true;
            }
        }
        for (int k = 0; k < CardIDs.Length; k++)
        {
            if (card.ID == CardIDs[k])
            {
                return true;
            }
        }
        if (((Position != DeckPositionType.None) && (card.Deck != null)) && (card.Deck.Count > 0))
        {
            if (Position == DeckPositionType.Top)
            {
                return (card.Deck[0].ID == card.ID);
            }
            if (Position == DeckPositionType.Bottom)
            {
                return (card.Deck[card.Deck.Count - 1].ID == card.ID);
            }
        }
        return false;
    }

    private static bool Loose(string CardID, CardType[] CardTypes, TraitType[] CardTraits, string[] CardIDs, DeckPositionType Position, bool Strict)
    {
        CardType cardType = CardTable.LookupCardType(CardID);
        for (int i = 0; i < CardTypes.Length; i++)
        {
            if (CardTypes[i] == CardType.None)
            {
                return true;
            }
            if (cardType == CardTypes[i])
            {
                return true;
            }
        }
        TraitType[] cardTraits = CardTable.LookupCardTraits(CardID);
        for (int j = 0; j < CardTraits.Length; j++)
        {
            if (CardTraits[j] == TraitType.None)
            {
                return true;
            }
            for (int m = 0; m < cardTraits.Length; m++)
            {
                if (CardTraits[j] == cardTraits[m])
                {
                    return true;
                }
            }
        }
        for (int k = 0; k < CardIDs.Length; k++)
        {
            if (CardID == CardIDs[k])
            {
                return true;
            }
        }
        return false;
    }

    public bool Match(Card card)
    {
        if (card == null)
        {
            return false;
        }
        return (this.IsEmpty() || Comparison(card, this));
    }

    private static bool Precise(Card card, CardType[] CardTypes, TraitType[] CardTraits, string[] CardIDs, DeckPositionType Position, bool Strict)
    {
        if (CardTypes.Length > 0)
        {
            for (int i = 0; i < CardTypes.Length; i++)
            {
                if ((card.Type == CardTypes[i]) || (CardTypes[i] == CardType.None))
                {
                    if (CardTraits.Length <= i)
                    {
                        return true;
                    }
                    if (card.HasTrait(CardTraits[i]) || (CardTraits[i] == TraitType.None))
                    {
                        return true;
                    }
                }
                if (((!Strict && (card.SubType == CardTypes[i])) && (CardTraits.Length > i)) && (card.HasTrait(CardTraits[i]) || (CardTraits[i] == TraitType.None)))
                {
                    return true;
                }
            }
        }
        else
        {
            for (int j = 0; j < CardTraits.Length; j++)
            {
                if (card.HasTrait(CardTraits[j]) || (CardTraits[j] == TraitType.None))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool Precise(string CardID, CardType[] CardTypes, TraitType[] CardTraits, string[] CardIDs, DeckPositionType Position, bool Strict)
    {
        if (CardTypes.Length > 0)
        {
            CardType cardType = CardTable.LookupCardType(CardID);
            for (int i = 0; i < CardTypes.Length; i++)
            {
                if ((cardType == CardTypes[i]) || (CardTypes[i] == CardType.None))
                {
                    if (CardTraits.Length <= i)
                    {
                        return true;
                    }
                    if (CardTraits[i] == TraitType.None)
                    {
                        return true;
                    }
                    TraitType[] cardTraits = CardTable.LookupCardTraits(CardID);
                    for (int j = 0; j < cardTraits.Length; j++)
                    {
                        if (CardTraits[i] == cardTraits[j])
                        {
                            return true;
                        }
                    }
                }
            }
        }
        else
        {
            for (int k = 0; k < CardTraits.Length; k++)
            {
                if (CardTraits[k] == TraitType.None)
                {
                    return true;
                }
                TraitType[] typeArray2 = CardTable.LookupCardTraits(CardID);
                for (int m = 0; m < typeArray2.Length; m++)
                {
                    if (typeArray2[m] == CardTraits[k])
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteBool(true);
        bs.WriteInt(1);
        bs.WriteInt(this.CardTypes.Length);
        for (int i = 0; i < this.CardTypes.Length; i++)
        {
            bs.WriteInt((int) this.CardTypes[i]);
        }
        bs.WriteInt(this.CardTraits.Length);
        for (int j = 0; j < this.CardTraits.Length; j++)
        {
            bs.WriteInt((int) this.CardTraits[j]);
        }
        bs.WriteInt(this.CardIDs.Length);
        for (int k = 0; k < this.CardIDs.Length; k++)
        {
            bs.WriteString(this.CardIDs[k]);
        }
        bs.WriteInt(this.Exclusions.Length);
        for (int m = 0; m < this.Exclusions.Length; m++)
        {
            bs.WriteString(this.Exclusions[m]);
        }
        bs.WriteInt(this.ExcludedCardTypes.Length);
        for (int n = 0; n < this.ExcludedCardTypes.Length; n++)
        {
            bs.WriteInt((int) this.ExcludedCardTypes[n]);
        }
        bs.WriteInt((int) this.Position);
        bs.WriteInt((int) this.ComparisonType);
        bs.WriteBool(this.Strict);
        bs.WriteGuid(this.ExcludedCard);
    }

    public string ToText()
    {
        if (((this.IsFilteredType(CardType.Ally) && this.IsFilteredType(CardType.Armor)) && (this.IsFilteredType(CardType.Blessing) && this.IsFilteredType(CardType.Item))) && (this.IsFilteredType(CardType.Spell) && this.IsFilteredType(CardType.Weapon)))
        {
            return "Boon";
        }
        return null;
    }

    public static CardFilter Bane =>
        new CardFilter { CardTypes=new CardType[] { 
            CardType.Barrier,
            CardType.Henchman,
            CardType.Monster,
            CardType.Villain
        } };

    public static CardFilter Boon =>
        new CardFilter { CardTypes=new CardType[] { 
            CardType.Ally,
            CardType.Armor,
            CardType.Blessing,
            CardType.Item,
            CardType.Spell,
            CardType.Weapon
        } };

    public static CardFilter Empty =>
        new CardFilter();
}

