using System;
using UnityEngine;

public class PrizeUnlockQuests : Prize
{
    public override void Deliver()
    {
        Conquests.Complete(Constants.QUEST_MODE_UNLOCKED);
        GameObject rewardPanel = base.GetRewardPanel("Reward_Prefab_UnlockQuests");
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(true);
        }
    }

    public override bool HasPrize() => 
        Conquests.IsComplete(Constants.QUEST_MODE_UNLOCKED);

    public override bool IsPrizeAllowed() => 
        !this.HasPrize();
}

