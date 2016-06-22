using System;
using UnityEngine;

public class RewardCardChooseType : RewardCardBase
{
    private CardType CardType;

    public override void Deliver()
    {
        base.Deliver();
        this.CardType = CardType.None;
        base.myPanel = base.GetRewardPanel(this.GetRewardPanelName());
    }

    public override void Display()
    {
        base.Display();
        if (this.CardType == CardType.None)
        {
            base.tapRecognizer.enabled = false;
        }
    }

    protected override void Generate()
    {
        if (this.CardType != CardType.None)
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
    }

    protected override string GetRewardPanelName() => 
        "Reward_Prefab_CardType";

    public void SetCardType(CardType type)
    {
        this.Show(false);
        this.CardType = type;
        base.myPanel = base.GetRewardPanel("Reward_Prefab_Card");
        base.tapRecognizer.enabled = true;
        base.Locked = false;
        this.Show(true);
    }
}

