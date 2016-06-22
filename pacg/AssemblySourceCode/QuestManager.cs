using System;
using UnityEngine;

public class QuestManager
{
    private static QuestRewards myRewards;

    public static void Start()
    {
        GameObject prefab = Resources.Load("Blueprints/Quests/Rewards/QU1B_Rewards") as GameObject;
        if (prefab != null)
        {
            GameObject obj3 = Game.Instance.Create(prefab);
            if (obj3 != null)
            {
                myRewards = obj3.GetComponent<QuestRewards>();
            }
        }
    }

    public static QuestRewards Rewards =>
        myRewards;
}

