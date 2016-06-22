using System;
using UnityEngine;

public class ScenarioPropertyCutscene : ScenarioProperty
{
    [Tooltip("name and location of the cutscenes in this scenario")]
    public CutsceneValueType[] Cutscenes;

    private string GetCutsceneForLocation(string id)
    {
        for (int i = 0; i < this.Cutscenes.Length; i++)
        {
            if (this.Cutscenes[i].Location == id)
            {
                return this.Cutscenes[i].Cutscene;
            }
        }
        return null;
    }

    public bool IsValid()
    {
        string iD = Location.Current.ID;
        string cutsceneForLocation = this.GetCutsceneForLocation(iD);
        if (string.IsNullOrEmpty(cutsceneForLocation))
        {
            return false;
        }
        if (Game.GameMode == GameModeType.Quest)
        {
            return false;
        }
        if (Scenario.Current.IsLocationExplored(iD))
        {
            return false;
        }
        if (!Cutscene.Exists(cutsceneForLocation))
        {
            return false;
        }
        return true;
    }

    public void Play()
    {
        string iD = Location.Current.ID;
        Cutscene.Queue = this.GetCutsceneForLocation(iD);
        Location.Current.OnSaveData();
        UI.Window.Pause(true);
        Game.UI.ShowCutsceneScene();
    }
}

