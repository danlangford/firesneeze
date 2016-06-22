using System;
using UnityEngine;

public class RewardCard : RewardCardBase
{
    [Tooltip("each character gets a specific card")]
    public RewardCardValueType[] Cards = new RewardCardValueType[6];

    protected override void Generate()
    {
        string cardID = this.GetCardID(Turn.Character);
        if (cardID != null)
        {
            if ((base.myCard != null) && (base.myCard.ID != cardID))
            {
                Campaign.Box.Add(base.myCard, false);
                base.myCard = null;
            }
            if (base.myCard == null)
            {
                base.myCard = Campaign.Box.Draw(cardID);
            }
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

    private string GetCardID(Character character)
    {
        for (int i = 0; i < this.Cards.Length; i++)
        {
            if (this.Cards[i].character == character.ID)
            {
                return this.Cards[i].card;
            }
        }
        return null;
    }

    protected override string GetRewardPanelName() => 
        "Reward_Prefab_Card";
}

