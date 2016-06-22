using System;
using UnityEngine;

public class CharacterPowerDefeatedExplore : CharacterPower
{
    [Tooltip("any of these traits will trigger the event")]
    public TraitType[] Traits;

    public override void Activate()
    {
        Turn.Explore = true;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.powersPanel.ShowPowerVFX(this);
        }
    }

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (!Turn.BlackBoard.Get<bool>("CharacterPowerDefeatedExplore_Explored"))
        {
            return false;
        }
        return base.IsConditionValid(Turn.Card);
    }

    public override void OnCardDefeated(Card card)
    {
        if (this.IsValid())
        {
            for (int i = 0; i < this.Traits.Length; i++)
            {
                if (card.HasTrait(this.Traits[i]))
                {
                    this.Activate();
                    break;
                }
            }
        }
        Turn.BlackBoard.Set<bool>("CharacterPowerDefeatedExplore_Explored", false);
    }

    public override void OnCardEvaded(Card card)
    {
        Turn.BlackBoard.Set<bool>("CharacterPowerDefeatedExplore_Explored", false);
    }

    public override void OnCardUndefeated(Card card)
    {
        Turn.BlackBoard.Set<bool>("CharacterPowerDefeatedExplore_Explored", false);
    }

    public override void OnLocationExplored(Card card)
    {
        Turn.BlackBoard.Set<bool>("CharacterPowerDefeatedExplore_Explored", true);
    }
}

