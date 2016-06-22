using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestTemplateScenario : MonoBehaviour
{
    [Tooltip("description of this scenario")]
    public StrRefType Description;
    [Tooltip("the unique ID of this quest")]
    public string ID;
    [Tooltip("this quest only applies to parties within these levels")]
    public ValueRangeType Level;
    [Tooltip("map locations (random)")]
    public LocationValueType[] Locations;
    [Tooltip("map used in this scenario")]
    public string Map;
    [Tooltip("name of this scenario")]
    public StrRefType Name;
    [Tooltip("scenario powers (random)")]
    public ScenarioPowerValueType[] Powers;
    [Tooltip("wildcard powers assigned to the scenario")]
    public ScenarioPowerValueType[] RandomPowers;
    [Tooltip("villain (random)")]
    public QuestTemplateVillain[] Villains;
    [Tooltip("used to determine the probability of selecting this quest")]
    public int Weight = 10;

    public void Apply(QuestScenario scenario)
    {
        scenario.Template = this.ID;
        scenario.DisplayName = this.Name.ToString();
        scenario.DisplayText = this.Description.ToString();
        scenario.Map = this.Map;
        scenario.RandomPowers = this.RandomPowers;
        this.ChooseRandomPower(scenario);
        this.ChooseRandomLocations(scenario);
        this.ChooseRandomVillain(scenario);
    }

    private void ChooseRandomLocations(QuestScenario scenario)
    {
        List<int> list = new List<int>(this.Locations.Length);
        for (int i = 0; i < this.Locations.Length; i++)
        {
            list.Add(i);
        }
        list.Shuffle<int>();
        scenario.Locations = new LocationValueType[Constants.NUM_SCENARIO_LOCATIONS];
        for (int j = 0; j < Constants.NUM_SCENARIO_LOCATIONS; j++)
        {
            for (int k = 0; k < list.Count; k++)
            {
                if ((j == 0) || this.IsLinkedToGraph(scenario, this.Locations[list[k]]))
                {
                    scenario.Locations[j] = this.Locations[list[k]];
                    scenario.Locations[j].PlayerCount = this.GetPlayerCount(j);
                    list.RemoveAt(k);
                    break;
                }
            }
        }
    }

    private void ChooseRandomPower(QuestScenario scenario)
    {
        List<int> list = new List<int>(this.Powers.Length);
        for (int i = 0; i < this.Powers.Length; i++)
        {
            if (this.Powers[i].Active)
            {
                list.Add(i);
            }
        }
        list.Shuffle<int>();
        scenario.StartingPowers = new ScenarioPowerValueType[3];
        for (int j = 0; j < 3; j++)
        {
            scenario.StartingPowers[j] = this.GetOverridePower(j);
            if (scenario.StartingPowers[j] == null)
            {
                for (int k = 0; k < list.Count; k++)
                {
                    if (this.Powers[list[k]].Difficulty == j)
                    {
                        scenario.StartingPowers[j] = this.Powers[list[k]];
                        break;
                    }
                }
            }
        }
    }

    private void ChooseRandomVillain(QuestScenario scenario)
    {
        List<int> list = new List<int>(this.Villains.Length);
        for (int i = 0; i < this.Villains.Length; i++)
        {
            if (this.Villains[i].IsValid())
            {
                list.Add(i);
            }
        }
        if (list.Count > 0)
        {
            list.Shuffle<int>();
            this.Villains[list[0]].Apply(scenario);
        }
    }

    public static QuestTemplateScenario Create(string id)
    {
        QuestTemplateScenario component = null;
        GameObject obj2 = Resources.Load<GameObject>("Blueprints/Quests/Scenarios/" + id);
        if (obj2 != null)
        {
            component = obj2.GetComponent<QuestTemplateScenario>();
        }
        return component;
    }

    private ScenarioPowerValueType GetOverridePower(int difficulty)
    {
        if (Settings.Debug.Wildcard1 != null)
        {
            string id = Settings.Debug.Wildcard1;
            Settings.Debug.Wildcard1 = null;
            return new ScenarioPowerValueType(id, difficulty);
        }
        if (Settings.Debug.Wildcard2 != null)
        {
            string str2 = Settings.Debug.Wildcard2;
            Settings.Debug.Wildcard2 = null;
            return new ScenarioPowerValueType(str2, difficulty);
        }
        if (Settings.Debug.Wildcard3 != null)
        {
            string str3 = Settings.Debug.Wildcard3;
            Settings.Debug.Wildcard3 = null;
            return new ScenarioPowerValueType(str3, difficulty);
        }
        return null;
    }

    private int GetPlayerCount(int i)
    {
        if (i <= 1)
        {
            return 1;
        }
        return (i - 1);
    }

    private bool IsLinkedToGraph(QuestScenario scenario, LocationValueType location)
    {
        for (int i = 0; i < scenario.Locations.Length; i++)
        {
            if (scenario.Locations[i] != null)
            {
                for (int j = 0; j < location.Links.Length; j++)
                {
                    if (location.Links[j] == scenario.Locations[i].LocationName)
                    {
                        return true;
                    }
                }
                for (int k = 0; k < scenario.Locations[i].Links.Length; k++)
                {
                    if (scenario.Locations[i].Links[k] == location.LocationName)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool IsValid() => 
        (((this.Level.min == 0) && (this.Level.max == 0)) || ((Party.Level >= this.Level.min) && (Party.Level <= this.Level.max)));

    public void Validate()
    {
        if (this.ID != base.name)
        {
            Debug.LogError(this.ID + ": ID does not match prefab name '" + base.name + "'");
        }
        for (int i = 0; i < this.Locations.Length; i++)
        {
            UnityEngine.Object obj2 = Resources.Load("Blueprints/Locations/" + this.Locations[i].LocationName);
            if ((obj2 == null) || (obj2.name != this.Locations[i].LocationName))
            {
                Debug.LogError(string.Concat(new object[] { this.ID, ": location [", i, "] ", this.Locations[i].LocationName, " does not exist" }));
            }
            for (int n = 0; n < this.Locations[i].Links.Length; n++)
            {
                UnityEngine.Object obj3 = Resources.Load("Blueprints/Locations/" + this.Locations[i].Links[n]);
                if ((obj3 == null) || (obj3.name != this.Locations[i].Links[n]))
                {
                    Debug.LogError(string.Concat(new object[] { this.ID, ": link [", i, ",", n, "] ", this.Locations[i].Links[n], " does not exist" }));
                }
            }
        }
        int a = 100;
        int num4 = 0;
        for (int j = 0; j < this.Villains.Length; j++)
        {
            a = Mathf.Min(a, this.Villains[j].Level.min);
            num4 = Mathf.Max(num4, this.Villains[j].Level.max);
        }
        if (((a != 0) && (num4 != 0)) && ((a > this.Level.min) || (num4 < this.Level.max)))
        {
            Debug.Log(string.Concat(new object[] { this.ID, ": incomplete villain range: ", a, " - ", num4, " for quest range: ", this.Level.min, " - ", this.Level.max }));
        }
        if (this.Locations.Length < 8)
        {
            Debug.LogError(string.Concat(new object[] { this.ID, ": only has ", this.Locations.Length, " locations; should have at least 8" }));
        }
        for (int k = 0; k < this.Locations.Length; k++)
        {
            for (int num7 = 0; num7 < this.Locations.Length; num7++)
            {
                if ((k != num7) && (this.Locations[k].LocationName == this.Locations[num7].LocationName))
                {
                    Debug.Log(string.Concat(new object[] { this.ID, ": location[", k, "] is duplicated at ", num7 }));
                }
            }
        }
        for (int m = 0; m < this.Locations.Length; m++)
        {
            if (this.Locations[m].Omits.Length > 0)
            {
                Debug.Log(string.Concat(new object[] { this.ID, ": location [", m, "] has omits" }));
            }
        }
    }
}

