using System;

public class CardData
{
    public BlockerType Blocker;
    public bool Clone;
    public bool Displayed;
    public DispositionType Disposition;
    public Guid GUID;
    public string ID;
    public bool Known;
    public bool Locked;
    public string PlayedOwner;
    public Guid[] PlayedPowerGuids;
    public string[] PlayedPowerOwnerIds;
    public int[] PlayedPowers;
    public bool Revealed;
    public string Set;
    public bool Shared;
    public string SharedPower;
    public int SharedPriority;

    public CardData()
    {
        this.ID = null;
        this.GUID = Guid.NewGuid();
        this.Set = null;
        this.Disposition = DispositionType.None;
        this.Clone = false;
        this.Known = false;
        this.Blocker = BlockerType.None;
        this.Shared = false;
        this.SharedPriority = 0;
        this.SharedPower = null;
        this.Revealed = false;
        this.Displayed = false;
        this.PlayedOwner = null;
        this.PlayedPowers = null;
        this.PlayedPowerGuids = null;
        this.PlayedPowerOwnerIds = null;
        this.Locked = false;
    }

    public CardData(string id, string set)
    {
        this.ID = id;
        this.GUID = Guid.NewGuid();
        this.Set = set;
        this.Disposition = DispositionType.None;
        this.Clone = false;
        this.Known = false;
        this.Blocker = BlockerType.None;
        this.Shared = false;
        this.SharedPriority = 0;
        this.SharedPower = null;
        this.Revealed = false;
        this.Displayed = false;
        this.PlayedOwner = null;
        this.PlayedPowers = null;
        this.PlayedPowerGuids = null;
        this.PlayedPowerOwnerIds = null;
        this.Locked = false;
    }

    public static Card CardFromStream(ByteStream bs)
    {
        bs.ReadInt();
        string iD = bs.ReadString();
        string set = bs.ReadString();
        Card card = CardTable.Create(iD, set, null);
        card.GUID = bs.ReadGuid();
        card.Disposition = (DispositionType) bs.ReadInt();
        card.Clone = bs.ReadBool();
        card.Known = bs.ReadBool();
        card.Blocker = (BlockerType) bs.ReadByte();
        card.Shared = bs.ReadBool();
        card.SharedPriority = bs.ReadInt();
        card.SharedPower = bs.ReadString();
        card.Revealed = bs.ReadBool();
        card.Displayed = bs.ReadBool();
        card.PlayedOwner = bs.ReadString();
        int num = bs.ReadInt();
        for (int i = 0; i < num; i++)
        {
            card.SetPowerInfo(bs.ReadInt(), bs.ReadGuid(), bs.ReadString());
        }
        card.Locked = bs.ReadBool();
        return card;
    }

    public static void CardToStream(Card card, ByteStream bs)
    {
        bs.WriteInt(1);
        bs.WriteString(card.ID);
        bs.WriteString(card.Set);
        bs.WriteGuid(card.GUID);
        bs.WriteInt((int) card.Disposition);
        bs.WriteBool(card.Clone);
        bs.WriteBool(card.Known);
        bs.WriteByte((byte) card.Blocker);
        bs.WriteBool(card.Shared);
        bs.WriteInt(card.SharedPriority);
        bs.WriteString(card.SharedPower);
        bs.WriteBool(card.Revealed);
        bs.WriteBool(card.Displayed);
        bs.WriteString(card.PlayedOwner);
        bs.WriteInt(card.PlayedPowers.Count);
        for (int i = 0; i < card.PlayedPowers.Count; i++)
        {
            bs.WriteInt(card.PlayedPowers[i].PlayedPower);
            bs.WriteGuid(card.PlayedPowers[i].PlayedPowerOwnerGuid);
            bs.WriteString(card.PlayedPowers[i].PlayedPowerOwnerID);
        }
        bs.WriteBool(card.Locked);
    }

    public static CardData FromStream(ByteStream bs)
    {
        bs.ReadInt();
        CardData data = new CardData {
            ID = bs.ReadString(),
            Set = bs.ReadString(),
            GUID = bs.ReadGuid(),
            Disposition = (DispositionType) bs.ReadInt(),
            Clone = bs.ReadBool(),
            Known = bs.ReadBool(),
            Blocker = (BlockerType) bs.ReadByte(),
            Shared = bs.ReadBool(),
            SharedPriority = bs.ReadInt(),
            SharedPower = bs.ReadString(),
            Revealed = bs.ReadBool(),
            Displayed = bs.ReadBool(),
            PlayedOwner = bs.ReadString()
        };
        int num = bs.ReadInt();
        if (num > 0)
        {
            data.PlayedPowers = new int[num];
            data.PlayedPowerGuids = new Guid[num];
            data.PlayedPowerOwnerIds = new string[num];
            for (int i = 0; i < num; i++)
            {
                data.PlayedPowers[i] = bs.ReadInt();
                data.PlayedPowerGuids[i] = bs.ReadGuid();
                data.PlayedPowerOwnerIds[i] = bs.ReadString();
            }
        }
        data.Locked = bs.ReadBool();
        return data;
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteInt(1);
        bs.WriteString(this.ID);
        bs.WriteString(this.Set);
        bs.WriteGuid(this.GUID);
        bs.WriteInt((int) this.Disposition);
        bs.WriteBool(this.Clone);
        bs.WriteBool(this.Known);
        bs.WriteByte((byte) this.Blocker);
        bs.WriteBool(this.Shared);
        bs.WriteInt(this.SharedPriority);
        bs.WriteString(this.SharedPower);
        bs.WriteBool(this.Revealed);
        bs.WriteBool(this.Displayed);
        bs.WriteString(this.PlayedOwner);
        if (this.PlayedPowers == null)
        {
            bs.WriteInt(0);
        }
        else
        {
            bs.WriteInt(this.PlayedPowers.Length);
            for (int i = 0; i < this.PlayedPowers.Length; i++)
            {
                bs.WriteInt(this.PlayedPowers[i]);
                bs.WriteGuid(this.PlayedPowerGuids[i]);
                bs.WriteString(this.PlayedPowerOwnerIds[i]);
            }
        }
        bs.WriteBool(this.Locked);
    }
}

