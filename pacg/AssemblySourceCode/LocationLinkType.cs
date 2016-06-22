using System;

[Serializable]
public class LocationLinkType
{
    public string End;
    public string Start;

    public void FromStream(ByteStream bs)
    {
        bs.ReadInt();
        this.Start = bs.ReadString();
        this.End = bs.ReadString();
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteInt(1);
        bs.WriteString(this.Start);
        bs.WriteString(this.End);
    }
}

