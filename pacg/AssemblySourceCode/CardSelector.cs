using System;
using UnityEngine;

public class CardSelector : Selector
{
    [Tooltip("any of these IDs")]
    public string[] CardIDs;
    [Tooltip("any of these traits")]
    public TraitType[] CardTraits;
    [Tooltip("any of these types")]
    public CardType[] CardTypes;
    [Tooltip("choose the type of comparison to match the card to (all, precise, none)")]
    public SelectorComparisonType ComparisonType = SelectorComparisonType.All;
    private Guid excludedCard;
    [Tooltip("exclude all of these types")]
    public CardType[] ExcludedCardTypes;
    [Tooltip("exclude this instance from the filter")]
    public bool ExcludeThisInstance;
    [Tooltip("never match any of these IDs")]
    public string[] Exclusions;
    private DeckPositionType Position;
    [Tooltip("set to false to allow card subtypes instead of just types")]
    public bool Strict = true;

    public int Filter(Deck deck)
    {
        int num = 0;
        if (deck != null)
        {
            for (int i = 0; i < deck.Count; i++)
            {
                if (this.Match(deck[i]))
                {
                    num++;
                }
            }
        }
        return num;
    }

    protected override bool IsEmpty() => 
        ((((this.CardTypes.Length == 0) && (this.CardTraits.Length == 0)) && ((this.CardIDs.Length == 0) && !this.ExcludeThisInstance)) && (this.ExcludedCardTypes.Length == 0));

    public bool IsExcluded(Card card) => 
        this.IsExcluded(card.ID);

    public bool IsExcluded(string cardID)
    {
        if (this.Exclusions != null)
        {
            for (int i = 0; i < this.Exclusions.Length; i++)
            {
                if (cardID == this.Exclusions[i])
                {
                    return true;
                }
            }
        }
        if (this.ExcludedCardTypes != null)
        {
            CardType cardType = CardTable.LookupCardType(cardID);
            for (int j = 0; j < this.ExcludedCardTypes.Length; j++)
            {
                if (cardType == this.ExcludedCardTypes[j])
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override bool Match() => 
        this.Match(Turn.Card);

    public bool Match(Card card)
    {
        if (card == null)
        {
            return false;
        }
        return (this.IsEmpty() || CardFilter.Comparison(card, this));
    }

    public bool Match(CardType type)
    {
        for (int i = 0; i < this.CardTypes.Length; i++)
        {
            if (type == this.CardTypes[i])
            {
                return true;
            }
        }
        return false;
    }

    public bool Match(string cardID)
    {
        if (string.IsNullOrEmpty(cardID))
        {
            return false;
        }
        return (this.IsEmpty() || CardFilter.Comparison(cardID, this));
    }

    public CardFilter ToFilter()
    {
        CardFilter filter = new CardFilter();
        if (filter != null)
        {
            filter.CardTypes = this.CardTypes;
            filter.CardTraits = this.CardTraits;
            filter.CardIDs = this.CardIDs;
            filter.Position = this.Position;
            filter.Strict = this.Strict;
            filter.ComparisonType = this.ComparisonType;
            filter.Exclusions = this.Exclusions;
            if (this.ExcludeThisInstance)
            {
                this.excludedCard = base.GetComponent<Card>().GUID;
            }
            filter.ExcludedCard = this.excludedCard;
            filter.ExcludedCardTypes = this.ExcludedCardTypes;
        }
        return filter;
    }
}

