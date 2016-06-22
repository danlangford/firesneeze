using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ScenarioLocation
{
    private int[] cardCounts;
    private string id;

    private ScenarioLocation()
    {
        this.Powers = new List<LocationPower>(5);
    }

    public ScenarioLocation(string ID)
    {
        this.id = ID;
        this.Closed = CloseType.None;
        this.cardCounts = new int[Constants.NUM_CARD_TYPES];
        LocationTableEntry entry = LocationTable.Get(ID);
        if (entry != null)
        {
            this.cardCounts[0] = entry.GetCardCount(CardType.None);
            this.cardCounts[7] = entry.GetCardCount(CardType.Monster);
            this.cardCounts[3] = entry.GetCardCount(CardType.Barrier);
            this.cardCounts[10] = entry.GetCardCount(CardType.Weapon);
            this.cardCounts[8] = entry.GetCardCount(CardType.Spell);
            this.cardCounts[2] = entry.GetCardCount(CardType.Armor);
            this.cardCounts[6] = entry.GetCardCount(CardType.Item);
            this.cardCounts[1] = entry.GetCardCount(CardType.Ally);
            this.cardCounts[4] = entry.GetCardCount(CardType.Blessing);
        }
        this.Powers = new List<LocationPower>(5);
    }

    public void Clear()
    {
        for (int i = 0; i < this.cardCounts.Length; i++)
        {
            this.cardCounts[i] = 0;
        }
    }

    public static ScenarioLocation FromStream(ByteStream bs)
    {
        ScenarioLocation location = new ScenarioLocation();
        bs.ReadInt();
        location.id = bs.ReadString();
        location.Closed = (CloseType) bs.ReadInt();
        location.Explored = bs.ReadBool();
        int num = bs.ReadInt();
        location.cardCounts = new int[num];
        for (int i = 0; i < num; i++)
        {
            location.cardCounts[i] = bs.ReadInt();
        }
        return location;
    }

    public int GetCardCount()
    {
        int num = 0;
        for (int i = 0; i < this.cardCounts.Length; i++)
        {
            num += this.cardCounts[i];
        }
        return num;
    }

    public int GetCardCount(CardType type) => 
        this.cardCounts[(int) type];

    public void Refresh(Deck deck)
    {
        this.cardCounts[7] = deck.Filter(CardType.Monster);
        this.cardCounts[3] = deck.Filter(CardType.Barrier);
        this.cardCounts[10] = deck.Filter(CardType.Weapon);
        this.cardCounts[8] = deck.Filter(CardType.Spell);
        this.cardCounts[2] = deck.Filter(CardType.Armor);
        this.cardCounts[6] = deck.Filter(CardType.Item);
        this.cardCounts[1] = deck.Filter(CardType.Ally);
        this.cardCounts[4] = deck.Filter(CardType.Blessing);
        this.cardCounts[9] = deck.Filter(CardType.Villain);
        this.cardCounts[5] = deck.Filter(CardType.Henchman);
    }

    public void SetCardCount(CardType type, int number)
    {
        this.cardCounts[(int) type] = number;
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteInt(1);
        bs.WriteString(this.id);
        bs.WriteInt((int) this.Closed);
        bs.WriteBool(this.Explored);
        bs.WriteInt(this.cardCounts.Length);
        for (int i = 0; i < this.cardCounts.Length; i++)
        {
            bs.WriteInt(this.cardCounts[i]);
        }
    }

    public override string ToString()
    {
        string str = "Cards: ";
        for (int i = 0; i < this.cardCounts.Length; i++)
        {
            str = str + this.cardCounts[i] + " ";
        }
        return (this.ID + ": " + str);
    }

    public CloseType Closed { get; set; }

    public bool Explored { get; set; }

    public string ID =>
        this.id;

    public List<LocationPower> Powers { get; set; }

    public GameObject PowersRoot { get; set; }
}

