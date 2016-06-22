using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ScenarioLocationCache : MonoBehaviour
{
    private List<ScenarioLocation> table = new List<ScenarioLocation>(Constants.NUM_SCENARIO_LOCATIONS);

    public void Add(string id)
    {
        ScenarioLocation sloc = new ScenarioLocation(id);
        this.Add(id, sloc);
    }

    private void Add(string id, ScenarioLocation sloc)
    {
        GameObject parent = Geometry.CreateChild(base.gameObject, id);
        if (parent != null)
        {
            string locationPowerID = this.GetLocationPowerID(id);
            if (locationPowerID != null)
            {
                sloc.PowersRoot = LocationPower.Create(locationPowerID, parent);
                LocationPower[] components = sloc.PowersRoot.GetComponents<LocationPower>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].LocationID = sloc.ID;
                }
                sloc.Powers.AddRange(components);
            }
        }
        this.table.Add(sloc);
    }

    public void Clear()
    {
        Geometry.DestroyAllChildren(base.gameObject);
        this.table.Clear();
    }

    public void FromStream(ByteStream bs)
    {
        this.table.Clear();
        bs.ReadInt();
        int num = bs.ReadInt();
        for (int i = 0; i < num; i++)
        {
            string id = bs.ReadString();
            ScenarioLocation sloc = ScenarioLocation.FromStream(bs);
            this.Add(id, sloc);
        }
    }

    private string GetLocationPowerID(string id)
    {
        if ((id != null) && (id.Length >= 3))
        {
            return ("PL" + id.Substring(2));
        }
        return null;
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteInt(1);
        bs.WriteInt(this.table.Count);
        for (int i = 0; i < this.table.Count; i++)
        {
            bs.WriteString(this.table[i].ID);
            this.table[i].ToStream(bs);
        }
    }

    public int Count =>
        this.table.Count;

    public ScenarioLocation this[int index]
    {
        get
        {
            if ((index >= 0) && (index < this.table.Count))
            {
                return this.table[index];
            }
            return null;
        }
    }

    public ScenarioLocation this[string ID]
    {
        get
        {
            for (int i = 0; i < this.table.Count; i++)
            {
                if (this.table[i].ID == ID)
                {
                    return this.table[i];
                }
            }
            return null;
        }
    }
}

