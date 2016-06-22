using System;
using System.Runtime.CompilerServices;

public class CampaignScenario
{
    public static CampaignScenario FromStream(ByteStream bs)
    {
        CampaignScenario scenario = new CampaignScenario();
        bs.ReadInt();
        scenario.ID = bs.ReadString();
        scenario.Completed = bs.ReadBool();
        scenario.Difficulty = bs.ReadInt();
        scenario.Champion = bs.ReadString();
        scenario.Rewarded = bs.ReadBool();
        return scenario;
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteInt(1);
        bs.WriteString(this.ID);
        bs.WriteBool(this.Completed);
        bs.WriteInt(this.Difficulty);
        bs.WriteString(this.Champion);
        bs.WriteBool(this.Rewarded);
    }

    public string Champion { get; set; }

    public bool Completed { get; set; }

    public int Difficulty { get; set; }

    public string ID { get; set; }

    public bool Rewarded { get; set; }
}

