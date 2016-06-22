using System;
using UnityEngine;

public class QuestTemplateVillain : MonoBehaviour
{
    [Tooltip("henchmen that might accompany this villain (random)")]
    public HenchmenGroupType[] Henchmen;
    [Tooltip("villains only fight parties within these levels")]
    public ValueRangeType Level;
    [Tooltip("the one villain specified by this template")]
    public string Villain;

    public void Apply(QuestScenario scenario)
    {
        scenario.Villains = new string[] { this.Villain };
        if (this.Henchmen.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, this.Henchmen.Length);
            scenario.Henchmen = new string[this.Henchmen[index].Henchmen.Length];
            for (int i = 0; i < this.Henchmen[index].Henchmen.Length; i++)
            {
                scenario.Henchmen[i] = this.Henchmen[index].Henchmen[i];
            }
        }
    }

    public bool IsValid() => 
        (((this.Level.min == 0) && (this.Level.max == 0)) || ((Party.Level >= this.Level.min) && (Party.Level <= this.Level.max)));
}

