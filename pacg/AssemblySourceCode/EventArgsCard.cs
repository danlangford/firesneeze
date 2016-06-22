using System;
using System.Runtime.CompilerServices;

public class EventArgsCard : EventArgs
{
    public EventArgsCard(CardType cardType, int changeInSize)
    {
        this.CardType = cardType;
        this.ChangeInSize = changeInSize;
    }

    public CardType CardType { get; private set; }

    public int ChangeInSize { get; private set; }
}

