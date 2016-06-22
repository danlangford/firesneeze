using System;

public class DebugSettings
{
    private GameType _GameType;
    private bool _GodMode;
    private bool _OnLoginPanels = true;
    private bool _PeonMode;
    private bool _StoryMode = true;
    private string _Summons;
    private string _Wildcard1;
    private string _Wildcard2;
    private string _Wildcard3;

    private void Add(Deck deck, params string[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            if (ids[i].ToLower() == "vl")
            {
                ids[i] = Scenario.Current.Villain;
            }
            if (ids[i].ToLower() == "he")
            {
                ids[i] = Scenario.Current.Henchmen[Scenario.Current.Henchmen.Length - 1];
            }
            deck.Add(CardTable.Create(ids[i]), DeckPositionType.Top);
        }
    }

    private void AddCharacterToParty(string id, string loc)
    {
        Character member = CharacterTable.Create(id);
        member.Initialize();
        member.Location = loc;
        Party.Add(member);
    }

    public void OnLocationLoaded()
    {
    }

    public bool Play() => 
        false;

    private void Set(Deck deck, params string[] ids)
    {
        deck.Clear();
        this.Add(deck, ids);
    }

    public void SetupBlessingsDeck(Deck deck)
    {
    }

    public void SetupGame()
    {
        this.AddCharacterToParty("CH1B_Merisiel", "LO1B_Woods");
        this.AddCharacterToParty("CH1B_Kyra", "LO1B_Farmhouse");
        this.StartDebugGame("AD1B_PerilsOfTheLostCoast", "SC1B_Brigandoom");
    }

    public void SetupLicenses()
    {
    }

    public void SetupLocationDeck(Deck deck)
    {
    }

    public void SetupPlayerDeck(Deck deck)
    {
    }

    public void SetupPlayerHand(Deck deck)
    {
    }

    private void SetXP(string id, int basexp, int gainxp)
    {
        Character character = Party.Find(id);
        character.XP = basexp;
        character.XPX = gainxp;
    }

    private void StartDebugGame(string adventure, string scenario)
    {
        AdventurePath.Current = AdventurePathTable.Create("AP1B_RiseOfTheRuneLords");
        Adventure.Current = AdventureTable.Create(adventure);
        Scenario.Current = ScenarioTable.Create(scenario);
    }

    private void StartDebugQuest()
    {
        AdventurePath.Current = AdventurePathTable.Create("AP1Q_Quest");
        Adventure.Current = AdventureTable.Create("AD1Q_Quest");
        Scenario.Current = ScenarioTable.Create("SC1Q_Quest");
        (Scenario.Current as QuestScenario).Generate();
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Party.Characters[i].Location = Scenario.Current.FirstLocation;
        }
        Game.GameMode = GameModeType.Quest;
    }

    public bool DemoMode =>
        false;

    public GameType GameType
    {
        get => 
            this._GameType;
        set
        {
            this._GameType = value;
        }
    }

    public bool GodMode
    {
        get => 
            this._GodMode;
        set
        {
            this._GodMode = value;
        }
    }

    public bool OnLoginPanels
    {
        get => 
            this._OnLoginPanels;
        set
        {
            this._OnLoginPanels = value;
        }
    }

    public bool PeonMode
    {
        get => 
            this._PeonMode;
        set
        {
            this._PeonMode = value;
        }
    }

    public bool StoryMode
    {
        get => 
            this._StoryMode;
        set
        {
            this._StoryMode = value;
        }
    }

    public string Summons
    {
        get => 
            this._Summons;
        set
        {
            this._Summons = value;
        }
    }

    public string Wildcard1
    {
        get => 
            this._Wildcard1;
        set
        {
            this._Wildcard1 = value;
        }
    }

    public string Wildcard2
    {
        get => 
            this._Wildcard2;
        set
        {
            this._Wildcard2 = value;
        }
    }

    public string Wildcard3
    {
        get => 
            this._Wildcard3;
        set
        {
            this._Wildcard3 = value;
        }
    }
}

