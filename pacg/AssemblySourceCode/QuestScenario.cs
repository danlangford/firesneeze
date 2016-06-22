using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class QuestScenario : Scenario
{
    protected override void BuildBox()
    {
        int tier = Party.Tier;
        int level = Party.Level;
        Campaign.Box.Clear();
        Campaign.LoadBox("QB", "B");
        for (int i = 1; i <= tier; i++)
        {
            string set = i.ToString();
            Campaign.LoadBox("Q" + set, set);
        }
        int b = (3 * base.GetNumLocations()) - 1;
        for (int j = 0; j <= tier; j++)
        {
            string str2 = (j != 0) ? j.ToString() : "B";
            foreach (CollectionEntry entry in Collection.GetEntries(str2))
            {
                int num6 = Mathf.Min(entry.quantity, b);
                for (int m = 0; m < num6; m++)
                {
                    CardIdentity identity = new CardIdentity(entry.id, str2);
                    Campaign.Box.Push(identity, true);
                }
            }
        }
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_CH11))
        {
            Campaign.LoadBox("1C", "C");
        }
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_BUNDLE_ROTR))
        {
            Campaign.LoadBox("1P", "P");
        }
        Campaign.Box.Shuffle();
        for (int k = 0; k < Party.Characters.Count; k++)
        {
            for (int n = 0; n < Party.Characters[k].Deck.Count; n++)
            {
                Campaign.Box.Remove(Party.Characters[k].Deck[n]);
            }
        }
        if (tier >= 3)
        {
            float num10 = ((tier - 3) * 0.5f) + (((level + 1) - (tier * Game.Rewards.LevelsPerTier)) * 0.05f);
            num10 = Mathf.Clamp(num10, 0f, 0.9f);
            Campaign.Box.Cull(num10, CardRankType.Basic);
        }
        if (tier >= 5)
        {
            float num11 = ((tier - 5) * 0.5f) + (((level + 1) - (tier * Game.Rewards.LevelsPerTier)) * 0.05f);
            num11 = Mathf.Clamp(num11, 0f, 0.9f);
            Campaign.Box.Cull(num11, CardRankType.Elite);
        }
        int num12 = Campaign.Box.Filter(CardType.Blessing, BoosterFilterType.On);
        if (num12 < Constants.MIN_BLESSINGS_IN_BOX)
        {
            CardIdentity identity2 = new CardIdentity("BL1B_BlessingOfTheGods");
            for (int num13 = 0; num13 < (Constants.MIN_BLESSINGS_IN_BOX - num12); num13++)
            {
                Campaign.Box.Push(identity2, true);
            }
        }
    }

    public bool Generate()
    {
        QuestAdventure current = Adventure.Current as QuestAdventure;
        if (current != null)
        {
            QuestTemplateScenario questTemplate = current.GetQuestTemplate();
            if (questTemplate != null)
            {
                questTemplate.Apply(this);
                return true;
            }
        }
        return false;
    }

    public override void OnLoadData()
    {
        byte[] buffer;
        if (Game.GetObjectData(this.ID_EXT, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            if (bs != null)
            {
                bs.ReadInt();
                base.Map = bs.ReadString();
                base.Villains = bs.ReadStringArray();
                base.Henchmen = bs.ReadStringArray();
                int num = bs.ReadInt();
                base.RandomPowers = new ScenarioPowerValueType[num];
                for (int i = 0; i < num; i++)
                {
                    base.RandomPowers[i] = new ScenarioPowerValueType();
                    base.RandomPowers[i].FromStream(bs);
                }
                num = bs.ReadInt();
                base.StartingPowers = new ScenarioPowerValueType[num];
                for (int j = 0; j < num; j++)
                {
                    base.StartingPowers[j] = new ScenarioPowerValueType();
                    base.StartingPowers[j].FromStream(bs);
                }
                num = bs.ReadInt();
                base.Locations = new LocationValueType[num];
                for (int k = 0; k < num; k++)
                {
                    base.Locations[k] = new LocationValueType();
                    base.Locations[k].FromStream(bs);
                }
            }
        }
        base.OnLoadData();
    }

    public override void OnSaveData()
    {
        ByteStream bs = new ByteStream();
        if (bs != null)
        {
            bs.WriteInt(1);
            bs.WriteString(base.Map);
            bs.WriteStringArray(base.Villains);
            bs.WriteStringArray(base.Henchmen);
            bs.WriteInt(base.RandomPowers.Length);
            for (int i = 0; i < base.RandomPowers.Length; i++)
            {
                base.RandomPowers[i].ToStream(bs);
            }
            bs.WriteInt(base.StartingPowers.Length);
            for (int j = 0; j < base.StartingPowers.Length; j++)
            {
                base.StartingPowers[j].ToStream(bs);
            }
            bs.WriteInt(base.Locations.Length);
            for (int k = 0; k < base.Locations.Length; k++)
            {
                base.Locations[k].ToStream(bs);
            }
            Game.SetObjectData(this.ID_EXT, bs.ToArray());
        }
        base.OnSaveData();
    }

    private string ID_EXT =>
        (base.ID + "_EXT");

    public string Template { get; set; }
}

