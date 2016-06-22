using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Campaign
{
    private static BlackBoard blackBoard = new BlackBoard("_CA_BB");
    private static List<string> cardDistributions = new List<string>(0x19);
    private static List<string> completedAdventures = new List<string>(6);
    private static List<string> deadCharacters = new List<string>(6);
    private static List<string> discoveredCards = new List<string>(2);
    private static List<string> encounteredCards = new List<string>(50);
    private static readonly string ID = "_CAMPAIGN";
    private static Box myBox = new Box("_BOX");
    private static Dictionary<string, CampaignScenario> scenarioCache = new Dictionary<string, CampaignScenario>(50);
    private static List<string> unlockedAdventures = new List<string>(6);

    public static void Audit()
    {
        if ((Game.GameMode == GameModeType.Story) && !Started)
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                Character character = Party.Characters[i];
                for (int j = 0; j < AdventureTable.Count; j++)
                {
                    AdventureTableEntry entry = AdventureTable.Get(j);
                    if (character.HasCompleted(entry.id))
                    {
                        SetAdventureComplete(entry.id);
                    }
                }
                for (int k = 0; k < ScenarioTable.Count; k++)
                {
                    ScenarioTableEntry entry2 = ScenarioTable.Get(k);
                    if (character.HasCompleted(entry2.id))
                    {
                        SetScenarioComplete(entry2.id);
                        SetRewarded(entry2.id);
                    }
                }
            }
        }
    }

    public static void Clear()
    {
        blackBoard.Clear();
        myBox.Clear();
        scenarioCache.Clear();
        completedAdventures.Clear();
        unlockedAdventures.Clear();
        deadCharacters.Clear();
        cardDistributions.Clear();
        encounteredCards.Clear();
        discoveredCards.Clear();
        PermaDeath = false;
        Started = false;
    }

    public static string GetScenarioChampion(string ID)
    {
        if (scenarioCache.ContainsKey(ID))
        {
            return scenarioCache[ID].Champion;
        }
        return null;
    }

    public static int GetScenarioDifficulty(string ID)
    {
        if (IsScenarioComplete(ID) && scenarioCache.ContainsKey(ID))
        {
            return scenarioCache[ID].Difficulty;
        }
        return -1;
    }

    public static string[] GetUnlockedAdventures()
    {
        if (unlockedAdventures.Count == 0)
        {
            return null;
        }
        return unlockedAdventures.ToArray();
    }

    public static bool IsAdventureComplete(string ID) => 
        ((completedAdventures != null) && completedAdventures.Contains(ID));

    public static bool IsCardEncountered(string ID) => 
        ((encounteredCards != null) && encounteredCards.Contains(ID));

    public static bool IsRewarded(string ID) => 
        (scenarioCache.ContainsKey(ID) && scenarioCache[ID].Rewarded);

    public static bool IsScenarioComplete(string ID) => 
        (scenarioCache.ContainsKey(ID) && scenarioCache[ID].Completed);

    public static void LoadBox(string id, string set)
    {
        string file = "Tables/BoxTable_" + id;
        myBox.Load(file, set);
    }

    public static void OnLoadData()
    {
        byte[] buffer;
        myBox.OnLoadData();
        if (Game.GetObjectData(ID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            if (bs != null)
            {
                bs.ReadInt();
                PermaDeath = bs.ReadBool();
                Started = bs.ReadBool();
                int capacity = bs.ReadInt();
                for (int i = 0; i < capacity; i++)
                {
                    completedAdventures.Add(bs.ReadString());
                }
                capacity = bs.ReadInt();
                for (int j = 0; j < capacity; j++)
                {
                    unlockedAdventures.Add(bs.ReadString());
                }
                capacity = bs.ReadInt();
                for (int k = 0; k < capacity; k++)
                {
                    deadCharacters.Add(bs.ReadString());
                }
                capacity = bs.ReadInt();
                for (int m = 0; m < capacity; m++)
                {
                    cardDistributions.Add(bs.ReadString());
                }
                capacity = bs.ReadInt();
                for (int n = 0; n < capacity; n++)
                {
                    encounteredCards.Add(bs.ReadString());
                }
                capacity = bs.ReadInt();
                for (int num7 = 0; num7 < capacity; num7++)
                {
                    discoveredCards.Add(bs.ReadString());
                }
                capacity = bs.ReadInt();
                scenarioCache = new Dictionary<string, CampaignScenario>(capacity);
                for (int num8 = 0; num8 < capacity; num8++)
                {
                    string key = bs.ReadString();
                    CampaignScenario scenario = CampaignScenario.FromStream(bs);
                    scenarioCache.Add(key, scenario);
                }
            }
        }
        blackBoard.OnLoadData();
    }

    public static void OnSaveData()
    {
        myBox.OnSaveData();
        ByteStream bs = new ByteStream();
        if (bs != null)
        {
            bs.WriteInt(1);
            bs.WriteBool(PermaDeath);
            bs.WriteBool(Started);
            bs.WriteInt(completedAdventures.Count);
            for (int i = 0; i < completedAdventures.Count; i++)
            {
                bs.WriteString(completedAdventures[i]);
            }
            bs.WriteInt(unlockedAdventures.Count);
            for (int j = 0; j < unlockedAdventures.Count; j++)
            {
                bs.WriteString(unlockedAdventures[j]);
            }
            bs.WriteInt(Deaths.Count);
            for (int k = 0; k < deadCharacters.Count; k++)
            {
                bs.WriteString(deadCharacters[k]);
            }
            bs.WriteInt(cardDistributions.Count);
            for (int m = 0; m < cardDistributions.Count; m++)
            {
                bs.WriteString(cardDistributions[m]);
            }
            bs.WriteInt(encounteredCards.Count);
            for (int n = 0; n < encounteredCards.Count; n++)
            {
                bs.WriteString(encounteredCards[n]);
            }
            bs.WriteInt(discoveredCards.Count);
            for (int num6 = 0; num6 < discoveredCards.Count; num6++)
            {
                bs.WriteString(discoveredCards[num6]);
            }
            bs.WriteInt(scenarioCache.Count);
            foreach (string str in scenarioCache.Keys)
            {
                bs.WriteString(str);
                scenarioCache[str].ToStream(bs);
            }
            Game.SetObjectData(ID, bs.ToArray());
        }
        blackBoard.OnSaveData();
    }

    public static void OnScenarioStarted()
    {
        Started = true;
    }

    public static void SetAdventureComplete(string ID)
    {
        if (!completedAdventures.Contains(ID))
        {
            completedAdventures.Add(ID);
        }
    }

    public static void SetAdventureUnlocked(string ID, bool isUnlocked)
    {
        if (isUnlocked)
        {
            if (!unlockedAdventures.Contains(ID))
            {
                unlockedAdventures.Add(ID);
            }
        }
        else
        {
            unlockedAdventures.Remove(ID);
        }
    }

    public static void SetCardEncountered(string ID)
    {
        if (!encounteredCards.Contains(ID))
        {
            encounteredCards.Add(ID);
        }
    }

    public static void SetRewarded(Scenario scenario)
    {
        if (scenario != null)
        {
            if (!scenarioCache.ContainsKey(scenario.ID))
            {
                CampaignScenario scenario2 = new CampaignScenario();
                scenarioCache.Add(scenario.ID, scenario2);
            }
            if (scenarioCache.ContainsKey(scenario.ID) && scenarioCache[scenario.ID].Completed)
            {
                scenarioCache[scenario.ID].Rewarded = true;
            }
        }
    }

    private static void SetRewarded(string ID)
    {
        if (!scenarioCache.ContainsKey(ID))
        {
            CampaignScenario scenario = new CampaignScenario();
            scenarioCache.Add(ID, scenario);
        }
        if (scenarioCache.ContainsKey(ID) && scenarioCache[ID].Completed)
        {
            scenarioCache[ID].Rewarded = true;
        }
    }

    public static void SetScenarioChampion(Scenario scenario, string characterID)
    {
        if ((scenario != null) && !string.IsNullOrEmpty(characterID))
        {
            if (!scenarioCache.ContainsKey(scenario.ID))
            {
                CampaignScenario scenario2 = new CampaignScenario();
                scenarioCache.Add(scenario.ID, scenario2);
            }
            if ((scenarioCache.ContainsKey(scenario.ID) && scenarioCache[scenario.ID].Completed) && string.IsNullOrEmpty(scenarioCache[scenario.ID].Champion))
            {
                scenarioCache[scenario.ID].Champion = characterID;
            }
        }
    }

    public static void SetScenarioComplete(Scenario scenario)
    {
        if (scenario != null)
        {
            if (!scenarioCache.ContainsKey(scenario.ID))
            {
                CampaignScenario scenario2 = new CampaignScenario();
                scenarioCache.Add(scenario.ID, scenario2);
            }
            if (scenarioCache.ContainsKey(scenario.ID))
            {
                scenarioCache[scenario.ID].Completed = true;
                if (scenarioCache[scenario.ID].Difficulty < scenario.Difficulty)
                {
                    scenarioCache[scenario.ID].Difficulty = scenario.Difficulty;
                }
                Conquests.Complete(scenario.ID, scenario.Difficulty);
            }
        }
    }

    public static void SetScenarioComplete(string ID)
    {
        if (!scenarioCache.ContainsKey(ID))
        {
            CampaignScenario scenario = new CampaignScenario();
            scenarioCache.Add(ID, scenario);
        }
        if (scenarioCache.ContainsKey(ID))
        {
            scenarioCache[ID].Completed = true;
        }
    }

    public static void Start(string box)
    {
        Clear();
        LoadBox("1B", "B");
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_CH11))
        {
            LoadBox("1C", "C");
        }
        if (LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_BUNDLE_ROTR))
        {
            LoadBox("1P", "P");
        }
    }

    public static BlackBoard BlackBoard =>
        blackBoard;

    public static Box Box =>
        myBox;

    public static List<string> Deaths =>
        deadCharacters;

    public static List<string> Distributions =>
        cardDistributions;

    public static List<string> GalleryCards =>
        discoveredCards;

    public static bool PermaDeath
    {
        [CompilerGenerated]
        get => 
            <PermaDeath>k__BackingField;
        [CompilerGenerated]
        set
        {
            <PermaDeath>k__BackingField = value;
        }
    }

    public static bool Started
    {
        [CompilerGenerated]
        get => 
            <Started>k__BackingField;
        [CompilerGenerated]
        private set
        {
            <Started>k__BackingField = value;
        }
    }
}

