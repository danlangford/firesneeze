using System;
using System.Runtime.CompilerServices;

[Serializable]
public class RewardCardLootValueType
{
    public string ID;
    public string[] Priority;

    public RewardCardLootValueType()
    {
        this.Available = true;
    }

    private int GetPriority(Character character)
    {
        for (int i = 0; i < this.Priority.Length; i++)
        {
            if (this.Priority[i] == character.ID)
            {
                return Math.Abs((int) ((this.Priority.Length - 1) - i));
            }
        }
        return -1;
    }

    public bool HasPriority(Character character)
    {
        int priority = this.GetPriority(character);
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (((Party.Characters[i].ID != character.ID) && Party.Characters[i].Alive) && (this.GetPriority(Party.Characters[i]) > priority))
            {
                return false;
            }
        }
        return true;
    }

    public bool Available { get; set; }
}

