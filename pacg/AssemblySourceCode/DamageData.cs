using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class DamageData
{
    public DamageData()
    {
    }

    public DamageData(ByteStream bs)
    {
        this.Damage = bs.ReadInt();
        int capacity = bs.ReadInt();
        this.DamageTraits = new List<TraitType>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            this.DamageTraits.Add((TraitType) bs.ReadInt());
        }
        this.DamageReduction = bs.ReadBool();
        this.PriorityCardType = (CardType) bs.ReadInt();
        this.DamageTargetType = (DamageTargetType) bs.ReadInt();
        this.DamageFromEnemy = bs.ReadBool();
    }

    public void SaveData(ByteStream bs)
    {
        bs.WriteInt(this.Damage);
        bs.WriteInt(this.DamageTraits.Count);
        for (int i = 0; i < this.DamageTraits.Count; i++)
        {
            bs.WriteInt(this.DamageTraits[i]);
        }
        bs.WriteBool(this.DamageReduction);
        bs.WriteInt((int) this.PriorityCardType);
        bs.WriteInt((int) this.DamageTargetType);
        bs.WriteBool(this.DamageFromEnemy);
    }

    public int Damage { get; set; }

    public bool DamageFromEnemy { get; set; }

    public bool DamageReduction { get; set; }

    public DamageTargetType DamageTargetType { get; set; }

    public List<TraitType> DamageTraits { get; set; }

    public CardType PriorityCardType { get; set; }
}

