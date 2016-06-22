using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(CutsceneConversation))]
public class Cutscene : MonoBehaviour
{
    [Tooltip("the background art prefab to load for this cutscene")]
    public GameObject Background;
    [Tooltip("optional epilogue for this cutscene (plays after the scene)")]
    public CutsceneIntertitle Epilogue;
    [Tooltip("optional introduction for this cutscene (plays before the prologue)")]
    public CutsceneAlert Introduction;
    [Tooltip("the background music for this cutscene")]
    public AudioClip Music;
    [Tooltip("optional prologue for this cutscene (plays before the scene)")]
    public CutsceneIntertitle Prologue;
    [Tooltip("is this an intro or an outro scene?")]
    public CutsceneType Type;

    private void Awake()
    {
        this.Conversation = base.GetComponent<CutsceneConversation>();
        this.LoadBackground();
        UI.Sound.MusicPlay(this.Music);
    }

    public static Cutscene Create(Scenario scenario)
    {
        string cutsceneName = GetCutsceneName(scenario);
        GameObject original = (GameObject) Resources.Load("Blueprints/Cutscenes/" + cutsceneName, typeof(GameObject));
        if (original != null)
        {
            GameObject obj3 = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
            return obj3.GetComponent<Cutscene>();
        }
        return null;
    }

    public static bool Exists(CutsceneType type) => 
        Exists(GetCutsceneName(Scenario.Current));

    public static bool Exists(string file)
    {
        GameObject obj2 = (GameObject) Resources.Load("Blueprints/Cutscenes/" + file, typeof(GameObject));
        return (obj2 != null);
    }

    private static string GetCutsceneName(Scenario scenario)
    {
        if (!string.IsNullOrEmpty(Queue))
        {
            return Queue;
        }
        string str = null;
        if (scenario == null)
        {
            return str;
        }
        if ((Turn.State == GameStateType.Villain) || (Turn.State == GameStateType.VillainIntro))
        {
            return (scenario.ID + "_Villain");
        }
        if (scenario.Complete)
        {
            return (scenario.ID + "_End");
        }
        return (scenario.ID + "_Begin");
    }

    private void LoadBackground()
    {
        if (this.Background != null)
        {
            Geometry.CreateChildObject(base.gameObject, this.Background, "Background");
        }
        else if ((this.Type == CutsceneType.Villain) || (this.Type == CutsceneType.Henchman))
        {
            GameObject prefab = (GameObject) Resources.Load("Blueprints/Locations/" + Location.Current.ID, typeof(GameObject));
            if (prefab != null)
            {
                Geometry.CreateChildObject(base.gameObject, prefab, "Background");
            }
        }
        else if (this.Type == CutsceneType.Outro)
        {
            GameObject obj3 = (GameObject) Resources.Load("Blueprints/Locations/" + Scenario.Current.EndLocation, typeof(GameObject));
            if (obj3 != null)
            {
                Geometry.CreateChildObject(base.gameObject, obj3, "Background");
                (UI.Window as GuiWindowCutscene).locationClosedOverlay.SetActive(true);
            }
        }
    }

    private void LoadSpeakers()
    {
        string[] speakerList = this.Conversation.GetSpeakerList();
        if (speakerList != null)
        {
            Transform transform = base.transform.Find("/Actors");
            if (transform != null)
            {
                for (int i = 0; i < speakerList.Length; i++)
                {
                    GameObject prefab = (GameObject) Resources.Load("Blueprints/Actors/" + speakerList[i], typeof(GameObject));
                    if (prefab != null)
                    {
                        GameObject obj3 = Geometry.CreateChildObject(transform.gameObject, prefab, speakerList[i]);
                        if (obj3 != null)
                        {
                            CutsceneActor component = obj3.GetComponent<CutsceneActor>();
                            if (component != null)
                            {
                                component.Initialize();
                            }
                        }
                    }
                }
            }
        }
    }

    public void Play()
    {
        if ((this.Introduction != null) && !this.Introduction.Complete)
        {
            this.Introduction.Play();
        }
        else if ((this.Prologue != null) && !this.Prologue.Complete)
        {
            this.Prologue.Play();
        }
        else
        {
            if (this.Conversation != null)
            {
                this.Conversation.Load();
                this.LoadSpeakers();
            }
            GuiWindowCutscene window = UI.Window as GuiWindowCutscene;
            if (window != null)
            {
                window.Play();
            }
            Queue = null;
        }
    }

    public void Stop()
    {
        if ((this.Epilogue != null) && !this.Epilogue.Complete)
        {
            this.Epilogue.Play();
        }
        else
        {
            Queue = null;
            if (this.Conversation != null)
            {
                this.Conversation.Unload();
            }
            UI.Sound.MusicStop();
            switch (this.Type)
            {
                case CutsceneType.Intro:
                    Game.UI.ShowSetupScene();
                    break;

                case CutsceneType.Outro:
                    if (!Scenario.Current.Rewardable)
                    {
                        Scenario.Current.Exit();
                        break;
                    }
                    Game.UI.ShowRewardScene();
                    break;

                case CutsceneType.Villain:
                case CutsceneType.Henchman:
                case CutsceneType.Flavor:
                    Game.UI.ShowLocationScene(Location.Current.ID, false);
                    break;
            }
        }
    }

    public CutsceneConversation Conversation { get; private set; }

    public static string Queue
    {
        [CompilerGenerated]
        get => 
            <Queue>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Queue>k__BackingField = value;
        }
    }
}

