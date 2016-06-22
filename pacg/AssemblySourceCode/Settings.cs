using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Settings
{
    private static int _activeSaveSlot;
    private static int _graphicsLevel;
    private static bool _music;
    private static float _musicVolume;
    private static int _tutorialLevel;
    private static bool _useCollectionCardsInStoryMode;
    private static float _volume;
    private static DebugSettings debugger;

    private static void Initialize()
    {
        _music = true;
        _musicVolume = 1f;
        _volume = 1f;
        _graphicsLevel = 1;
        _tutorialLevel = 1;
        _useCollectionCardsInStoryMode = true;
        _activeSaveSlot = 0;
    }

    public static void Load()
    {
        Initialize();
        try
        {
            using (FieldStream stream = new FieldStream(GameDirectory.GetSettingsPath(), FileMode.Open))
            {
                if (stream.CanRead)
                {
                    stream.ReadInt(1);
                    _music = stream.ReadBool(_music);
                    _musicVolume = stream.ReadFloat(_musicVolume);
                    _volume = stream.ReadFloat(_volume);
                    _tutorialLevel = stream.ReadByte((byte) _tutorialLevel);
                    stream.ReadBool(false);
                    _activeSaveSlot = stream.ReadByte((byte) _activeSaveSlot);
                    stream.ReadBool(false);
                    _graphicsLevel = stream.ReadByte((byte) _graphicsLevel);
                    _useCollectionCardsInStoryMode = stream.ReadBool(_useCollectionCardsInStoryMode);
                }
            }
        }
        catch (Exception exception)
        {
            UnityEngine.Debug.Log("Settings Load Failed: " + exception);
        }
    }

    public static void Save()
    {
        try
        {
            using (FieldStream stream = new FieldStream(GameDirectory.GetSettingsPath(), FileMode.Create))
            {
                if (stream.CanWrite)
                {
                    stream.WriteInt(1);
                    stream.WriteBool(_music);
                    stream.WriteFloat(_musicVolume);
                    stream.WriteFloat(_volume);
                    stream.WriteByte((byte) _tutorialLevel);
                    stream.WriteBool(false);
                    stream.WriteByte((byte) _activeSaveSlot);
                    stream.WriteBool(false);
                    stream.WriteByte((byte) _graphicsLevel);
                    stream.WriteBool(_useCollectionCardsInStoryMode);
                }
            }
        }
        catch (Exception exception)
        {
            UnityEngine.Debug.Log("Settings Save Failed: " + exception);
        }
    }

    public static int ActiveSaveSlot
    {
        get => 
            _activeSaveSlot;
        set
        {
            _activeSaveSlot = value;
        }
    }

    public static DebugSettings Debug
    {
        get
        {
            if (debugger == null)
            {
                debugger = new DebugSettings();
            }
            return debugger;
        }
    }

    public static bool DebugMode =>
        Application.isEditor;

    public static int GraphicsLevel
    {
        get => 
            _graphicsLevel;
        set
        {
            _graphicsLevel = value;
        }
    }

    public static bool Music
    {
        get => 
            _music;
        set
        {
            _music = value;
        }
    }

    public static float MusicVolume
    {
        get => 
            _musicVolume;
        set
        {
            _musicVolume = value;
            UI.Sound.SetMusicVolume(_musicVolume);
        }
    }

    public static bool QuestModeUnlocked
    {
        [CompilerGenerated]
        get => 
            <QuestModeUnlocked>k__BackingField;
        [CompilerGenerated]
        set
        {
            <QuestModeUnlocked>k__BackingField = value;
        }
    }

    public static int TutorialLevel
    {
        get => 
            _tutorialLevel;
        set
        {
            _tutorialLevel = value;
        }
    }

    public static bool TutorialScenarioPlayed
    {
        [CompilerGenerated]
        get => 
            <TutorialScenarioPlayed>k__BackingField;
        [CompilerGenerated]
        set
        {
            <TutorialScenarioPlayed>k__BackingField = value;
        }
    }

    public static bool UseCollectionCardsInStoryMode
    {
        get => 
            _useCollectionCardsInStoryMode;
        set
        {
            _useCollectionCardsInStoryMode = value;
        }
    }

    public static float Volume
    {
        get => 
            _volume;
        set
        {
            _volume = value;
            UI.Sound.SetSoundVolume(_volume);
        }
    }
}

