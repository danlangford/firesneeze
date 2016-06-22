using System;
using UnityEngine;

public class RewardCardType : RewardCardBase
{
    [Tooltip("a random reward of this type will be chosen")]
    public CardType CardType;

    protected override void Generate()
    {
        if (base.myCard == null)
        {
            base.myCard = Campaign.Box.Draw(this.CardType);
        }
        if (base.myCard != null)
        {
            Transform child = Geometry.GetChild(base.myPanel.transform, "reward_card");
            if (child != null)
            {
                base.myCard.transform.parent = child;
                base.myCard.transform.localScale = Vector3.one;
                base.myCard.transform.localPosition = Vector3.zero;
                base.myCard.transform.localRotation = Quaternion.identity;
                base.myCard.Animations(false);
                base.myCard.Show(true);
            }
        }
    }

    protected override string GetRewardPanelName() => 
        "Reward_Prefab_Card";
}

