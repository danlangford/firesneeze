using PlayFab.Json;
using System;
using System.IO;
using System.Runtime.CompilerServices;

public class GameSaveHeader
{
    public GameSaveHeader()
    {
        this.Length = 0;
        this.GUID = Guid.NewGuid().ToString();
        this.VersionStamp = 0;
    }

    [JsonConstructor]
    public GameSaveHeader(string guid, int versionStamp, GameType gameType, GameModeType gameMode, CampaignType campaign, string adventurePathId, string adventurePathName, string adventurePathSet, string adventureId, string adventureName, int adventureNumber, string scenarioId, string scenarioName, int scenarioNumber, int scenarioDifficulty, int scenarioOptions, string[] characterIds, string[] characterNicks, string[] characterNames, ClassType[] characterClasses, string date, string time)
    {
        this.GUID = guid;
        this.VersionStamp = versionStamp;
        this.GameType = gameType;
        this.GameMode = gameMode;
        this.Campaign = campaign;
        this.AdventurePathID = adventurePathId;
        this.AdventurePathName = adventurePathName;
        this.AdventurePathSet = adventurePathSet;
        this.AdventureID = adventureId;
        this.AdventureName = adventureName;
        this.AdventureNumber = adventureNumber;
        this.ScenarioID = scenarioId;
        this.ScenarioName = scenarioName;
        this.ScenarioNumber = scenarioNumber;
        this.ScenarioDifficulty = scenarioDifficulty;
        this.ScenarioOptions = scenarioOptions;
        this.CharacterIDs = characterIds;
        this.CharacterNicks = characterNicks;
        this.CharacterNames = characterNames;
        this.CharacterClasses = characterClasses;
        char[] separator = new char[] { '-' };
        string[] strArray = date.Split(separator);
        this.TimeDay = Convert.ToInt16(strArray[0]);
        this.TimeMonth = Convert.ToInt16(strArray[1]);
        this.TimeYear = Convert.ToInt16(strArray[2]);
        char[] chArray2 = new char[] { ':' };
        string[] strArray2 = time.Split(chArray2);
        this.TimeHour = Convert.ToInt16(strArray2[0]);
        this.TimeMinute = Convert.ToInt16(strArray2[1]);
    }

    public void Clear()
    {
        this.Length = 0;
    }

    public bool IsValid()
    {
        if (this.CharacterIDs == null)
        {
            return false;
        }
        if (this.CharacterNames == null)
        {
            return false;
        }
        if (this.CharacterNicks == null)
        {
            return false;
        }
        if (this.CharacterClasses == null)
        {
            return false;
        }
        return true;
    }

    public void Load(BinaryReader reader)
    {
        reader.ReadInt32();
        reader.ReadString();
        reader.ReadBoolean();
        reader.ReadInt32();
        reader.ReadString();
        reader.ReadString();
        this.VersionStamp = reader.ReadInt32();
        this.GUID = reader.ReadString();
        this.GameType = (GameType) reader.ReadInt16();
        this.GameMode = (GameModeType) reader.ReadInt16();
        this.Campaign = (CampaignType) reader.ReadInt16();
        this.AdventurePathID = reader.ReadString();
        this.AdventurePathName = reader.ReadString();
        this.AdventurePathSet = reader.ReadString();
        this.AdventureID = reader.ReadString();
        this.AdventureName = reader.ReadString();
        this.AdventureNumber = reader.ReadByte();
        this.ScenarioID = reader.ReadString();
        this.ScenarioName = reader.ReadString();
        this.ScenarioNumber = reader.ReadByte();
        this.ScenarioDifficulty = reader.ReadByte();
        this.ScenarioOptions = reader.ReadInt32();
        this.TimeDay = reader.ReadInt16();
        this.TimeMonth = reader.ReadInt16();
        this.TimeYear = reader.ReadInt16();
        this.TimeHour = reader.ReadInt16();
        this.TimeMinute = reader.ReadInt16();
        int num = reader.ReadInt32();
        this.CharacterIDs = new string[num];
        this.CharacterNames = new string[num];
        this.CharacterNicks = new string[num];
        this.CharacterClasses = new ClassType[num];
        for (int i = 0; i < num; i++)
        {
            this.CharacterIDs[i] = reader.ReadString();
            this.CharacterNicks[i] = reader.ReadString();
            this.CharacterNames[i] = reader.ReadString();
            this.CharacterClasses[i] = (ClassType) reader.ReadInt16();
        }
        this.Length = 1;
    }

    public void Save(BinaryWriter writer)
    {
        this.VersionStamp++;
        if (string.IsNullOrEmpty(this.GUID))
        {
            this.GUID = Guid.NewGuid().ToString();
        }
        writer.Write(1);
        writer.Write("X");
        writer.Write(BitConverter.IsLittleEndian);
        writer.Write(0);
        if (Game.Instance != null)
        {
            writer.Write(Game.Instance.BuildNumber);
        }
        else
        {
            writer.Write(string.Empty);
        }
        writer.Write(string.Empty);
        writer.Write(this.VersionStamp);
        writer.Write(this.GUID);
        writer.Write((short) Game.GameType);
        writer.Write((short) Game.GameMode);
        writer.Write((short) 0);
        if (AdventurePath.Current != null)
        {
            writer.Write(AdventurePath.Current.ID);
            writer.Write(AdventurePath.Current.DisplayName);
            writer.Write(AdventurePath.Current.Set);
        }
        else
        {
            writer.Write(string.Empty);
            writer.Write(string.Empty);
            writer.Write(string.Empty);
        }
        if (Adventure.Current != null)
        {
            writer.Write(Adventure.Current.ID);
            writer.Write(Adventure.Current.DisplayName);
            writer.Write((byte) Adventure.Current.Number);
        }
        else
        {
            writer.Write(string.Empty);
            writer.Write(string.Empty);
            writer.Write((byte) 0);
        }
        if (Scenario.Current != null)
        {
            writer.Write(Scenario.Current.ID);
            writer.Write(Scenario.Current.DisplayName);
            writer.Write((byte) Scenario.Current.Number);
            writer.Write((byte) Scenario.Current.Difficulty);
            writer.Write(0);
        }
        else
        {
            writer.Write(string.Empty);
            writer.Write(string.Empty);
            writer.Write((byte) 0);
            writer.Write((byte) 0);
            writer.Write(0);
        }
        writer.Write((short) DateTime.Now.Day);
        writer.Write((short) DateTime.Now.Month);
        writer.Write((short) DateTime.Now.Year);
        writer.Write((short) DateTime.Now.Hour);
        writer.Write((short) DateTime.Now.Minute);
        writer.Write(Party.Characters.Count);
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            writer.Write(Party.Characters[i].ID);
            writer.Write(Party.Characters[i].NickName);
            writer.Write(Party.Characters[i].DisplayName);
            writer.Write((short) Party.Characters[i].Class);
        }
    }

    public void SaveInternal(BinaryWriter writer)
    {
        writer.Write(1);
        writer.Write("X");
        writer.Write(BitConverter.IsLittleEndian);
        writer.Write(0);
        if (Game.Instance != null)
        {
            writer.Write(Game.Instance.BuildNumber);
        }
        else
        {
            writer.Write(string.Empty);
        }
        writer.Write(string.Empty);
        writer.Write(this.VersionStamp);
        writer.Write(this.GUID);
        writer.Write((short) this.GameType);
        writer.Write((short) this.GameMode);
        writer.Write((short) this.Campaign);
        writer.Write(this.AdventurePathID);
        writer.Write(this.AdventurePathName);
        writer.Write(this.AdventurePathSet);
        writer.Write(this.AdventureID);
        writer.Write(this.AdventureName);
        writer.Write((byte) this.AdventureNumber);
        writer.Write(this.ScenarioID);
        writer.Write(this.ScenarioName);
        writer.Write((byte) this.ScenarioNumber);
        writer.Write((byte) this.ScenarioDifficulty);
        writer.Write(this.ScenarioOptions);
        writer.Write(this.TimeDay);
        writer.Write(this.TimeMonth);
        writer.Write(this.TimeYear);
        writer.Write(this.TimeHour);
        writer.Write(this.TimeMinute);
        writer.Write(this.CharacterIDs.Length);
        for (int i = 0; i < this.CharacterIDs.Length; i++)
        {
            writer.Write(this.CharacterIDs[i]);
            writer.Write(this.CharacterNicks[i]);
            writer.Write(this.CharacterNames[i]);
            writer.Write((short) this.CharacterClasses[i]);
        }
    }

    public string AdventureID { get; private set; }

    public string AdventureName { get; private set; }

    public int AdventureNumber { get; private set; }

    public string AdventurePathID { get; private set; }

    public string AdventurePathName { get; private set; }

    public string AdventurePathSet { get; private set; }

    public CampaignType Campaign { get; private set; }

    public ClassType[] CharacterClasses { get; private set; }

    public string[] CharacterIDs { get; private set; }

    public string[] CharacterNames { get; private set; }

    public string[] CharacterNicks { get; private set; }

    public string Date =>
        $"{this.TimeDay:D2}-{this.TimeMonth:D2}-{this.TimeYear:D4}";

    public GameModeType GameMode { get; private set; }

    public GameType GameType { get; private set; }

    public string GUID { get; private set; }

    public int Length { get; private set; }

    public int ScenarioDifficulty { get; private set; }

    public string ScenarioID { get; private set; }

    public string ScenarioName { get; private set; }

    public int ScenarioNumber { get; private set; }

    public int ScenarioOptions { get; private set; }

    public string Time =>
        $"{this.TimeHour:D2}:{this.TimeMinute:D2}";

    private short TimeDay { get; set; }

    private short TimeHour { get; set; }

    private short TimeMinute { get; set; }

    private short TimeMonth { get; set; }

    private short TimeYear { get; set; }

    public int VersionStamp { get; private set; }
}

