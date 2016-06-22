using System;

public class TurnStackFrame
{
    private static readonly string ID = "_TURN_S0";

    public static void Clear()
    {
        Game.SetObjectData(ID, null);
    }

    private static void OnLoadData()
    {
        byte[] buffer;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((window != null) && Game.GetObjectData(ID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            if (bs != null)
            {
                bs.ReadInt();
                window.layoutDiscard.Deck.FromStream(bs);
                window.layoutRecharge.Deck.FromStream(bs);
                window.layoutBury.Deck.FromStream(bs);
                window.layoutBanish.Deck.FromStream(bs);
                ReadRevealList(bs);
                Turn.OnLoadData(bs);
            }
        }
    }

    private static void OnSaveData()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            ByteStream bs = new ByteStream();
            if (bs != null)
            {
                bs.WriteInt(1);
                window.layoutDiscard.Deck.ToStream(bs);
                window.layoutRecharge.Deck.ToStream(bs);
                window.layoutBury.Deck.ToStream(bs);
                window.layoutBanish.Deck.ToStream(bs);
                WriteRevealedList(bs);
                Turn.OnSaveData(bs);
                Game.SetObjectData(ID, bs.ToArray());
            }
        }
    }

    public static void Pop()
    {
        Turn.Dice.Clear();
        OnLoadData();
        Clear();
    }

    public static void Push()
    {
        OnSaveData();
    }

    private static void ReadRevealList(ByteStream bs)
    {
        int num = bs.ReadInt();
        for (int i = 0; i < num; i++)
        {
            Guid guid = bs.ReadGuid();
            for (int j = 0; j < Party.Characters.Count; j++)
            {
                if (Party.Characters[j].Hand[guid] != null)
                {
                    Party.Characters[j].Hand[guid].Revealed = true;
                }
            }
        }
    }

    private static void WriteCheckArray(ByteStream bs, SkillCheckValueType[] checks)
    {
        if (checks == null)
        {
            bs.WriteInt(0);
        }
        else
        {
            bs.WriteInt(checks.Length);
            for (int i = 0; i < checks.Length; i++)
            {
                bs.WriteInt((int) checks[i].skill);
                bs.WriteInt(checks[i].rank);
            }
        }
    }

    private static void WriteDiceArray(ByteStream bs, DiceType[] dice)
    {
        if (dice == null)
        {
            bs.WriteInt(0);
        }
        else
        {
            bs.WriteInt(dice.Length);
            for (int i = 0; i < dice.Length; i++)
            {
                bs.WriteInt((int) dice[i]);
            }
        }
    }

    private static void WriteRevealedList(ByteStream bs)
    {
        int data = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            for (int k = 0; k < Party.Characters[i].Hand.Count; k++)
            {
                if (Party.Characters[i].Hand[k].Revealed)
                {
                    data++;
                }
            }
        }
        bs.WriteInt(data);
        for (int j = 0; j < Party.Characters.Count; j++)
        {
            for (int m = 0; m < Party.Characters[j].Hand.Count; m++)
            {
                if (Party.Characters[j].Hand[m].Revealed)
                {
                    bs.WriteGuid(Party.Characters[j].Hand[m].GUID);
                }
            }
        }
    }
}

