using System;
using UnityEngine;

public class RewardChampionOfScenario : RewardCardBase
{
    [Tooltip("the alternate card type if you aren't champion/high enough priority")]
    public CardType Alternate;
    [Tooltip("the champion will receive this reward even if the card is not in the box")]
    public RewardCardLootValueType ChampionReward;
    [Tooltip("this reward will be added to the Gallery because it usually doesn't come from the box")]
    public bool GalleryCard = true;
    [Tooltip("the scenario the champion must come from")]
    public string Scenario;
    private string scenarioChampion;

    private Card CreateCard(string ID)
    {
        Card card = Campaign.Box.Draw(ID);
        if (card == null)
        {
            card = CardTable.Create(ID);
        }
        return card;
    }

    protected override void Generate()
    {
        if (base.myCard == null)
        {
            Character character = Party.Find(this.ScenarioChamp);
            if ((character != null) && character.Alive)
            {
                if (this.ScenarioChamp == Turn.Character.ID)
                {
                    base.myCard = this.CreateCard(this.ChampionReward.ID);
                }
            }
            else if (this.ChampionReward.HasPriority(Turn.Character))
            {
                base.myCard = this.CreateCard(this.ChampionReward.ID);
            }
        }
        if (base.myCard == null)
        {
            base.myCard = Campaign.Box.Draw(this.Alternate);
        }
        if (base.myCard != null)
        {
            if (this.GalleryCard)
            {
                Campaign.GalleryCards.Add(base.myCard.ID);
            }
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

    private string ScenarioChamp
    {
        get
        {
            if (string.IsNullOrEmpty(this.scenarioChampion))
            {
                this.scenarioChampion = Campaign.GetScenarioChampion(this.Scenario);
            }
            return this.scenarioChampion;
        }
    }
}

