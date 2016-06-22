using System;
using UnityEngine;

public class TurnStateData
{
    [Tooltip("list of actions that are valid in this state")]
    public ActionType[] Actions;
    [Tooltip("optional: deck to select cards from")]
    public ActionType Deck;
    [Tooltip("optional: filter used to select cards from a deck")]
    public CardFilter Filter;
    [Tooltip("optional: max number of cards. negative or int.Max means infinite")]
    public int MaxNumCards;
    [Tooltip("optional: text message displayed to the player")]
    public string Message;
    [Tooltip("number of cards (min)")]
    public int NumCards;
    [Tooltip("automatic: the turn owner at construction")]
    public string Owner;

    private TurnStateData()
    {
        this.NumCards = 1;
        this.MaxNumCards = 1;
    }

    public TurnStateData(StrRefType strRef)
    {
        this.NumCards = 1;
        this.MaxNumCards = 1;
        this.Deck = ActionType.None;
        this.Actions = new ActionType[0];
        this.Filter = new CardFilter();
        this.Message = StringTableManager.Get(strRef);
        this.NumCards = 0;
        this.MaxNumCards = this.NumCards;
        this.Owner = Turn.Character.ID;
    }

    public TurnStateData(string message)
    {
        this.NumCards = 1;
        this.MaxNumCards = 1;
        this.Deck = ActionType.None;
        this.Actions = new ActionType[0];
        this.Filter = new CardFilter();
        this.Message = message;
        this.NumCards = 0;
        this.MaxNumCards = this.NumCards;
        this.Owner = Turn.Character.ID;
    }

    public TurnStateData(ActionType action, int numCards)
    {
        this.NumCards = 1;
        this.MaxNumCards = 1;
        this.Deck = ActionType.None;
        this.Actions = new ActionType[] { action };
        this.Filter = new CardFilter();
        this.Message = null;
        this.NumCards = numCards;
        this.MaxNumCards = this.NumCards;
        this.Owner = Turn.Character.ID;
    }

    public TurnStateData(ActionType action1, ActionType action2, int numCards)
    {
        this.NumCards = 1;
        this.MaxNumCards = 1;
        this.Deck = ActionType.None;
        this.Actions = new ActionType[] { action1, action2 };
        this.Filter = new CardFilter();
        this.Message = null;
        this.NumCards = numCards;
        this.MaxNumCards = this.NumCards;
        this.Owner = Turn.Character.ID;
    }

    public TurnStateData(ActionType action, CardFilter filter, int numCards)
    {
        this.NumCards = 1;
        this.MaxNumCards = 1;
        this.Deck = action;
        this.Actions = new ActionType[] { action };
        this.Filter = filter;
        this.Message = null;
        this.NumCards = numCards;
        this.MaxNumCards = this.NumCards;
        this.Owner = Turn.Character.ID;
    }

    public TurnStateData(ActionType from, CardFilter filter, ActionType to, int numCards)
    {
        this.NumCards = 1;
        this.MaxNumCards = 1;
        this.Deck = from;
        this.Actions = new ActionType[] { to };
        this.Filter = filter;
        this.Message = null;
        this.NumCards = numCards;
        this.MaxNumCards = this.NumCards;
        this.Owner = Turn.Character.ID;
    }

    public TurnStateData(ActionType from, CardFilter filter, ActionType to, int numCards, int maxNumCards)
    {
        this.NumCards = 1;
        this.MaxNumCards = 1;
        this.Deck = from;
        this.Actions = new ActionType[] { to };
        this.Filter = filter;
        this.Message = null;
        this.NumCards = numCards;
        this.MaxNumCards = maxNumCards;
        this.Owner = Turn.Character.ID;
    }

    public static TurnStateData FromStream(ByteStream bs)
    {
        if (!bs.ReadBool())
        {
            return null;
        }
        bs.ReadInt();
        TurnStateData data = new TurnStateData {
            NumCards = bs.ReadInt(),
            MaxNumCards = bs.ReadInt(),
            Actions = new ActionType[bs.ReadInt()]
        };
        for (int i = 0; i < data.Actions.Length; i++)
        {
            data.Actions[i] = (ActionType) bs.ReadInt();
        }
        data.Deck = (ActionType) bs.ReadInt();
        data.Message = bs.ReadString();
        data.Filter = CardFilter.FromStream(bs);
        data.Owner = bs.ReadString();
        return data;
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteBool(true);
        bs.WriteInt(1);
        bs.WriteInt(this.NumCards);
        bs.WriteInt(this.MaxNumCards);
        bs.WriteInt(this.Actions.Length);
        for (int i = 0; i < this.Actions.Length; i++)
        {
            bs.WriteInt((int) this.Actions[i]);
        }
        bs.WriteInt((int) this.Deck);
        bs.WriteString(this.Message);
        this.Filter.ToStream(bs);
        bs.WriteString(this.Owner);
    }

    public bool Proceed =>
        (this.NumCards != this.MaxNumCards);
}

