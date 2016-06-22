using System;

[Serializable]
public class CardValueType
{
    public CardType card;
    public int rank;

    public CardValueType(CardType card, int rank)
    {
        this.card = card;
        this.rank = rank;
    }
}

