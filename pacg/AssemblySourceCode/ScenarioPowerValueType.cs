using System;

[Serializable]
public class ScenarioPowerValueType
{
    public int Difficulty;
    public string ID;
    public bool Not;

    public ScenarioPowerValueType()
    {
        this.Not = false;
        this.ID = null;
        this.Difficulty = 0;
    }

    public ScenarioPowerValueType(string id, int difficulty)
    {
        this.Not = false;
        this.ID = id;
        this.Difficulty = difficulty;
    }

    public void FromStream(ByteStream bs)
    {
        bs.ReadInt();
        this.Not = bs.ReadBool();
        this.ID = bs.ReadString();
        this.Difficulty = bs.ReadInt();
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteInt(1);
        bs.WriteBool(this.Not);
        bs.WriteString(this.ID);
        bs.WriteInt(this.Difficulty);
    }

    public bool Active
    {
        get
        {
            if (string.IsNullOrEmpty(this.ID))
            {
                return false;
            }
            return !this.Not;
        }
    }

    public string Description
    {
        get
        {
            if (this.ID != "$Random")
            {
                ScenarioPowerTableEntry entry = ScenarioPowerTable.Get(this.ID);
                if (entry != null)
                {
                    return entry.Description;
                }
            }
            return null;
        }
    }
}

