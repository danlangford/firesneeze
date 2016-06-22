using System;
using UnityEngine;

public class RewardCardLoot : RewardCardBase
{
    [Tooltip("use one of these when a character does not get loot")]
    public RewardCardValueType[] Alternates = new RewardCardValueType[7];
    [Tooltip("loot cards prioritized by character")]
    public RewardCardLootValueType[] Cards = new RewardCardLootValueType[1];

    public override void Deliver()
    {
        if (base.myCard != null)
        {
            for (int i = 0; i < this.Cards.Length; i++)
            {
                if (this.Cards[i].ID == base.myCard.ID)
                {
                    this.Cards[i].Available = false;
                    break;
                }
            }
        }
        base.Deliver();
    }

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
            if (base.myCard == null)
            {
                base.myCard = CardTable.Create(cardID, Scenario.Current.Set, null);
            }
            if (base.myCard == null)
            {
                base.myCard = CardTable.Create("BL1B_BlessingOfTheGods");
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

    public override bool GetAllRewardsGiven()
    {
        bool flag = true;
        if (!Campaign.IsRewarded(base.ID))
        {
            for (int i = 0; i < this.Cards.Length; i++)
            {
                if (this.Cards[i].Available)
                {
                    flag = false;
                    for (int j = 0; j < base.selectedCard.Length; j++)
                    {
                        if (this.Cards[i].HasPriority(Party.Characters[j]))
                        {
                            base.selectedCard[j] = false;
                        }
                    }
                }
            }
        }
        return flag;
    }

    private string GetCardID(Character character)
    {
        if (!Campaign.IsRewarded(base.ID))
        {
            for (int j = 0; j < this.Cards.Length; j++)
            {
                if (this.Cards[j].Available && this.Cards[j].HasPriority(character))
                {
                    return this.Cards[j].ID;
                }
            }
        }
        for (int i = 0; i < this.Alternates.Length; i++)
        {
            if (this.Alternates[i].character == character.ID)
            {
                return this.Alternates[i].card;
            }
        }
        return null;
    }

    public override int GetNumRewards(Character character)
    {
        int num = 0;
        if (!Campaign.IsRewarded(base.ID))
        {
            for (int j = 0; j < this.Cards.Length; j++)
            {
                if (this.Cards[j].Available && this.Cards[j].HasPriority(character))
                {
                    num++;
                }
            }
        }
        if (num > 0)
        {
            return num;
        }
        for (int i = 0; i < this.Alternates.Length; i++)
        {
            if (this.Alternates[i].character == character.ID)
            {
                return 1;
            }
        }
        return 0;
    }

    protected override string GetRewardPanelName() => 
        "Reward_Prefab_Card";
}

