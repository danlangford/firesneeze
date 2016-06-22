using System;

[Serializable]
public class LocationValueType
{
    public string[] Links;
    public string LocationName;
    public LocationLinkType[] Omits;
    public int PlayerCount;

    public void FromStream(ByteStream bs)
    {
        bs.ReadInt();
        this.LocationName = bs.ReadString();
        this.PlayerCount = bs.ReadInt();
        this.Links = bs.ReadStringArray();
        int num = bs.ReadInt();
        this.Omits = new LocationLinkType[num];
        for (int i = 0; i < num; i++)
        {
            this.Omits[i] = new LocationLinkType();
            this.Omits[i].FromStream(bs);
        }
    }

    public bool IsLinked(string linkID)
    {
        if (linkID == this.LocationName)
        {
            return true;
        }
        if (Scenario.Current != null)
        {
            for (int j = 0; j < Scenario.Current.Locations.Length; j++)
            {
                LocationValueType type = Scenario.Current.Locations[j];
                if ((type != null) && Scenario.Current.IsLocationValid(type.LocationName))
                {
                    for (int k = 0; k < type.Omits.Length; k++)
                    {
                        if ((type.Omits[k].Start == this.LocationName) && (type.Omits[k].End == linkID))
                        {
                            return false;
                        }
                        if ((type.Omits[k].Start == linkID) && (type.Omits[k].End == this.LocationName))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < this.Links.Length; i++)
        {
            if (this.Links[i] == linkID)
            {
                return true;
            }
        }
        if (Scenario.Current != null)
        {
            for (int m = 0; m < Scenario.Current.Locations.Length; m++)
            {
                LocationValueType type2 = Scenario.Current.Locations[m];
                if ((type2 != null) && (type2.LocationName == linkID))
                {
                    for (int n = 0; n < type2.Links.Length; n++)
                    {
                        if (type2.Links[n] == this.LocationName)
                        {
                            return true;
                        }
                    }
                    break;
                }
            }
        }
        return false;
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteInt(1);
        bs.WriteString(this.LocationName);
        bs.WriteInt(this.PlayerCount);
        bs.WriteStringArray(this.Links);
        bs.WriteInt(this.Omits.Length);
        for (int i = 0; i < this.Omits.Length; i++)
        {
            this.Omits[i].ToStream(bs);
        }
    }
}

