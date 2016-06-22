using System;
using UnityEngine;

[Serializable]
public class ScenarioPipLayout
{
    [Tooltip("references to the pip icons in the scene")]
    public GuiImage[] Pips;

    public void Show(bool isVisible)
    {
        for (int i = 0; i < this.Pips.Length; i++)
        {
            this.Pips[i].Show(isVisible);
        }
    }

    public void ShowAvailable(string ID, Sprite availableSprite, Sprite completedSprite, Sprite unavailableSprite)
    {
        bool flag = Campaign.IsScenarioComplete(ID);
        int scenarioDifficulty = Campaign.GetScenarioDifficulty(ID);
        int complete = Conquests.GetComplete(ID);
        for (int i = 0; i < this.Pips.Length; i++)
        {
            if ((i <= scenarioDifficulty) && flag)
            {
                this.Pips[i].Image = completedSprite;
                this.Pips[i].Show(true);
            }
            else if ((i == (scenarioDifficulty + 1)) || (i == 0))
            {
                this.Pips[i].Image = availableSprite;
                this.Pips[i].Show(true);
            }
            else if ((i > (scenarioDifficulty + 1)) && (i <= (complete + 1)))
            {
                this.Pips[i].Image = unavailableSprite;
                this.Pips[i].Show(true);
            }
            else
            {
                this.Pips[i].Show(false);
            }
        }
    }

    public void ShowUnavailable(string ID, Sprite lockedSprite)
    {
        this.Pips[0].Image = lockedSprite;
        this.Pips[0].Show(true);
        for (int i = 1; i < this.Pips.Length; i++)
        {
            this.Pips[i].Show(false);
        }
    }
}

