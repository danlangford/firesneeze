using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    private static EventCallbackManager eventQueue;
    private static GameSaveFile file;
    private static GameModeType gameModeType;
    private static GameType gameType;
    private static readonly string ID = "_GAME";
    private static bool isPaused;
    private static bool LoadAdventure;
    private static bool LoadAdventurePath;
    private static string LoadLevelName;
    private static bool LoadScenario;
    private static Cache myCache;
    private static Game myInstance;
    private static NetworkManager myNetwork;
    private static UI myUI;
    private static int normalSleepTimeoutValue;
    private static int saveSlot = 1;
    public static int SlotToLoad = -1;
    private Timer timeSpentInGame = new Timer();

    private void Awake()
    {
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        eventQueue = new EventCallbackManager();
        Settings.Load();
        Conquests.Load();
        this.ConfigureEngine();
        GameDirectory.Init();
        QuestManager.Start();
        ScenarioPowerTable.Load();
        CharacterTable.Load();
        LocationTable.Load();
        ScenarioTable.Load();
        AdventureTable.Load();
        AdventurePathTable.Load();
        CardTable.Load();
        PowerTable.Load();
        RoleTable.Load();
        ProfileTable.Load();
        LicenseTable.Load();
        UbersTable.Load();
        Vault.Load();
        LicenseManager.Start();
        Tutorial.Initialize("TU1B_Tutorial");
        StringTableManager.Load("UI");
        StringTableManager.Load("HelperText");
        AnalyticsManager.OnStartGame();
        this.timeSpentInGame.Start(0);
    }

    private static void Clear()
    {
        Instance.StopAllCoroutines();
        LeanTween.cancelAll(false);
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Party.Characters[i].ResetDeck();
        }
        if (file != null)
        {
            file.Clear();
        }
        Turn.Reset();
        Campaign.Clear();
        Party.Clear();
        UI.Reset();
        Rules.Reset();
        Cache.Clear();
        eventQueue.Clear();
        Geometry.DestroyAllChildren(Instance.gameObject);
        GuiWindowScenario.StartScreen = 0;
        Adventure.Current = null;
        AdventurePath.Current = null;
        Scenario.Current = null;
    }

    private static void ClearTemporaryLoadData()
    {
        LoadLevelName = null;
        LoadAdventurePath = false;
        LoadAdventure = false;
        LoadScenario = false;
    }

    private void ConfigureEngine()
    {
        Application.targetFrameRate = 60;
        QualitySettings.antiAliasing = 8;
        LeanTween.init(800);
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        AudioListener.volume = Settings.Volume;
    }

    public GameObject Create(UnityEngine.Object prefab)
    {
        GameObject obj2 = UnityEngine.Object.Instantiate(prefab) as GameObject;
        if (obj2 != null)
        {
            obj2.name = prefab.name;
            obj2.transform.parent = base.transform;
        }
        return obj2;
    }

    public static bool GetObjectData(string id, out byte[] data)
    {
        if ((file != null) && file.Data.ContainsKey(id))
        {
            data = file.Data[id];
            return true;
        }
        data = null;
        return false;
    }

    public static bool Load(int slot)
    {
        Clear();
        file = new GameSaveFile(slot);
        if (file.Load())
        {
            OnLoadData();
            if (!string.IsNullOrEmpty(LoadLevelName))
            {
                UI.OnLoadData();
                Turn.OnLoadData();
                Tutorial.OnLoadData();
                Campaign.OnLoadData();
                if (LoadAdventurePath)
                {
                    AdventurePath.Current.OnLoadData();
                }
                if (LoadAdventure)
                {
                    Adventure.Current.OnLoadData();
                }
                if (LoadScenario)
                {
                    Scenario.Current.OnLoadData();
                }
                Party.OnLoadData();
                Events.OnLoadData();
                LoadLevel(LoadLevelName);
                ClearTemporaryLoadData();
                return true;
            }
        }
        return false;
    }

    private static void LoadLevel(string levelName)
    {
        if (!string.IsNullOrEmpty(levelName) && (levelName != SceneManager.GetActiveScene().name))
        {
            if (levelName == "location")
            {
                string location = Party.Characters[Turn.Current].Location;
                UI.ShowLocationScene(location, Turn.Map);
            }
            else
            {
                SceneManager.LoadScene(levelName);
            }
        }
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            AnalyticsManager.OnEndGame((float) this.timeSpentInGame.Stop());
        }
        else
        {
            AnalyticsManager.OnStartGame();
            this.timeSpentInGame.Start(0);
        }
    }

    private void OnApplicationQuit()
    {
        AnalyticsManager.OnEndGame((float) this.timeSpentInGame.Stop());
    }

    private static void OnLoadData()
    {
        byte[] buffer;
        if (GetObjectData(ID, out buffer))
        {
            ByteStream stream = new ByteStream(buffer);
            if (stream != null)
            {
                stream.ReadInt();
                LoadLevelName = stream.ReadString();
                LoadAdventurePath = stream.ReadBool();
                LoadAdventure = stream.ReadBool();
                LoadScenario = stream.ReadBool();
                stream.ReadBool();
                GameType = (GameType) stream.ReadInt();
                GameMode = (GameModeType) stream.ReadInt();
                SaveSlot = stream.ReadInt();
                if (LoadAdventurePath)
                {
                    AdventurePath.Current = AdventurePathTable.Create(stream.ReadString());
                }
                if (LoadAdventure)
                {
                    Adventure.Current = AdventureTable.Create(stream.ReadString());
                }
                if (LoadScenario)
                {
                    Scenario.Current = ScenarioTable.Create(stream.ReadString());
                }
            }
        }
    }

    private static void OnSaveData()
    {
        ByteStream stream = new ByteStream();
        if (stream != null)
        {
            stream.WriteInt(1);
            stream.WriteString(SceneManager.GetActiveScene().name);
            stream.WriteBool(AdventurePath.Current != null);
            stream.WriteBool(Adventure.Current != null);
            stream.WriteBool(Scenario.Current != null);
            stream.WriteBool(Location.Current != null);
            stream.WriteInt((int) GameType);
            stream.WriteInt((int) GameMode);
            stream.WriteInt(SaveSlot);
            if (AdventurePath.Current != null)
            {
                stream.WriteString(AdventurePath.Current.ID);
            }
            if (Adventure.Current != null)
            {
                stream.WriteString(Adventure.Current.ID);
            }
            if (Scenario.Current != null)
            {
                stream.WriteString(Scenario.Current.ID);
            }
            SetObjectData(ID, stream.ToArray());
        }
    }

    public static void Play(GameType gameType, int slot, WindowType windowType, string startLocation, bool showMap)
    {
        if (Device.GetIsApplicationGenuine())
        {
            GameType = gameType;
            SaveSlot = slot;
            GameDirectory.Clear(slot);
            file = new GameSaveFile(slot);
            if (windowType == WindowType.CreateParty)
            {
                Campaign.Start("1B");
                Campaign.SetAdventureUnlocked("AD1B_PerilsOfTheLostCoast", true);
                UI.ShowCreatePartyScene();
            }
            if (windowType == WindowType.Quest)
            {
                Campaign.Start("1B");
                UI.ShowCreatePartyScene();
            }
            if (windowType == WindowType.Cutscene)
            {
                Campaign.Start("1T");
                UI.ShowCutsceneScene();
            }
            if (windowType == WindowType.Location)
            {
                Campaign.Start("1B");
                UI.ShowLocationScene(Party.Characters[0].Location, showMap);
            }
            if (windowType == WindowType.Adventure)
            {
                Campaign.SetAdventureUnlocked("AD1B_PerilsOfTheLostCoast", true);
                UI.ShowAdventureScene();
            }
        }
    }

    public bool Reload() => 
        ((file != null) && Load(file.Slot));

    public static void Restart()
    {
        Clear();
        UI.ShowMainMenu();
    }

    public static void Save()
    {
        OnSaveData();
        UI.OnSaveData();
        UI.Window.OnSaveData();
        Turn.OnSaveData();
        Tutorial.OnSaveData();
        Campaign.OnSaveData();
        if (AdventurePath.Current != null)
        {
            AdventurePath.Current.OnSaveData();
        }
        if (Adventure.Current != null)
        {
            Adventure.Current.OnSaveData();
        }
        if (Scenario.Current != null)
        {
            Scenario.Current.OnSaveData();
        }
        if (Location.Current != null)
        {
            Location.Current.OnSaveData();
        }
        Party.OnSaveData();
        Events.OnSaveData();
        if (file != null)
        {
            file.Save();
            GameDirectory.ActiveSlot = SaveSlot;
        }
    }

    public static bool SetObjectData(string id, byte[] data)
    {
        if ((file == null) || string.IsNullOrEmpty(id))
        {
            return false;
        }
        if (data != null)
        {
            file.Data[id] = data;
        }
        else
        {
            file.Data.Remove(ID);
        }
        return true;
    }

    public static void SetScreenSaver(bool isEnabled)
    {
        if (!isEnabled)
        {
            normalSleepTimeoutValue = Screen.sleepTimeout;
            Screen.sleepTimeout = -1;
        }
        else
        {
            Screen.sleepTimeout = normalSleepTimeoutValue;
        }
    }

    public static void Synchronize()
    {
        if (file != null)
        {
            Network.SynchronizeGameSave(file);
        }
    }

    private void Update()
    {
        this.timeSpentInGame.Tick(Time.deltaTime);
    }

    public string BuildNumber
    {
        get
        {
            BuildTimeStamp component = base.GetComponent<BuildTimeStamp>();
            if (component != null)
            {
                return component.Date;
            }
            return null;
        }
    }

    public static Cache Cache
    {
        get
        {
            if (myCache == null)
            {
                GameObject obj2 = GameObject.Find("/~Cache");
                if (obj2 != null)
                {
                    myCache = obj2.GetComponent<Cache>();
                }
            }
            return myCache;
        }
    }

    public static EventCallbackManager Events =>
        eventQueue;

    public static GameModeType GameMode
    {
        get => 
            gameModeType;
        set
        {
            gameModeType = value;
        }
    }

    public static GameType GameType
    {
        get => 
            gameType;
        set
        {
            gameType = value;
        }
    }

    public static Game Instance
    {
        get
        {
            if (myInstance == null)
            {
                GameObject obj2 = GameObject.Find("/~Game");
                if (obj2 != null)
                {
                    myInstance = obj2.GetComponent<Game>();
                }
            }
            return myInstance;
        }
    }

    public static NetworkManager Network
    {
        get
        {
            if (myNetwork == null)
            {
                GameObject obj2 = GameObject.Find("/~Network");
                if (obj2 != null)
                {
                    myNetwork = obj2.GetComponent<NetworkManager>();
                }
            }
            return myNetwork;
        }
    }

    public static bool Paused
    {
        get => 
            isPaused;
        set
        {
            isPaused = value;
        }
    }

    public static QuestRewards Rewards =>
        QuestManager.Rewards;

    public static int SaveSlot
    {
        get => 
            saveSlot;
        private set
        {
            saveSlot = value;
        }
    }

    public static UI UI
    {
        get
        {
            if (myUI == null)
            {
                GameObject obj2 = GameObject.Find("/~UI");
                if (obj2 != null)
                {
                    myUI = obj2.GetComponent<UI>();
                }
            }
            return myUI;
        }
    }
}

