using System;
using UnityEngine;

public class ScenarioPropertyChampion : ScenarioProperty
{
    [Tooltip("ordered list of character IDs")]
    public string[] Characters;

    public string GetChampion()
    {
        for (int i = 0; i < this.Characters.Length; i++)
        {
            Character character = Party.Characters[this.Characters[i]];
            if ((character != null) && character.Alive)
            {
                return character.ID;
            }
        }
        return null;
    }
}

