using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestAdventure : Adventure
{
    [Tooltip("all quests that can happen in this adventure (randomly selected)")]
    public string[] Quests;

    public QuestTemplateScenario GetQuestTemplate()
    {
        int max = 0;
        List<QuestTemplateScenario> list = new List<QuestTemplateScenario>(this.Quests.Length);
        for (int i = 0; i < this.Quests.Length; i++)
        {
            QuestTemplateScenario item = QuestTemplateScenario.Create(this.Quests[i]);
            if (item.IsValid())
            {
                list.Add(item);
                max += item.Weight;
            }
        }
        if (list.Count > 0)
        {
            list.Shuffle<QuestTemplateScenario>();
            int num3 = 0;
            int num4 = UnityEngine.Random.Range(0, max);
            for (int j = 0; j < list.Count; j++)
            {
                num3 += list[j].Weight;
                if (num4 <= num3)
                {
                    return list[j];
                }
            }
            return list[0];
        }
        if (this.Quests.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, this.Quests.Length);
            return QuestTemplateScenario.Create(this.Quests[index]);
        }
        return null;
    }

    public override int Rank =>
        Party.Tier;
}

