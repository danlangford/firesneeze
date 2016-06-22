using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ScenarioPower : Power
{
    [Tooltip("Check to see if any of these locations are in the scenario, if yes then check valid locations")]
    public LocationSelector ScenarioNeedsLocation;
    [Tooltip("Locations that the power applies to")]
    public LocationSelector ValidLocations;

    public static ScenarioPower[] Create(string ID)
    {
        GameObject original = Resources.Load<GameObject>("Blueprints/Powers/" + ID);
        if (original != null)
        {
            GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
            if (obj3 != null)
            {
                obj3.name = original.name;
                return obj3.GetComponents<ScenarioPower>();
            }
        }
        return null;
    }

    protected override void GlowText(bool isGlowing)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (!isGlowing)
            {
                window.locationPanel.GlowText(TextHilightType.None);
            }
            else
            {
                window.locationPanel.GlowText(TextHilightType.AtThisLocation);
            }
        }
    }

    public bool IsHiddenPower() => 
        (this.ScenarioNeedsLocation != null);

    public virtual bool IsLocationValid(string LocID)
    {
        bool flag = false;
        if (this.ScenarioNeedsLocation != null)
        {
            for (int i = 0; i < this.ScenarioNeedsLocation.LocationIDs.Length; i++)
            {
                if (Scenario.Current.IsLocationValid(this.ScenarioNeedsLocation.LocationIDs[i]))
                {
                    flag = true;
                }
            }
        }
        else
        {
            flag = true;
        }
        if (flag && (this.ValidLocations != null))
        {
            for (int j = 0; j < this.ValidLocations.LocationIDs.Length; j++)
            {
                if (LocID == this.ValidLocations.LocationIDs[j])
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override string Description
    {
        get
        {
            ScenarioPowerTableEntry entry = ScenarioPowerTable.Get(base.ID);
            if ((entry != null) && (entry.descriptionStrRef != 0))
            {
                return entry.Description;
            }
            return Scenario.Current.DisplayText;
        }
    }

    public override string Name
    {
        get
        {
            ScenarioPowerTableEntry entry = ScenarioPowerTable.Get(base.ID);
            if ((entry != null) && (entry.nameStrRef != 0))
            {
                return entry.Name;
            }
            return Scenario.Current.DisplayName;
        }
    }

    public virtual PowerType Type =>
        PowerType.None;

    public bool Wildcard { get; set; }
}

