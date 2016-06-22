using System;
using UnityEngine;

public class TutorialCommandGlow : TutorialCommand
{
    [Tooltip("glow the character sheet button on scenario screen")]
    public bool CharacterSheetButton;
    [Tooltip("glow the location deck on the location screen")]
    public bool LocationExploreButton;

    private void GlowLocationSceneObjects()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowExploreButton(this.LocationExploreButton);
        }
    }

    private void GlowScenarioSceneObjects()
    {
        GuiWindowScenario window = UI.Window as GuiWindowScenario;
        if (window != null)
        {
            window.ViewCharacterButton.Glow(this.CharacterSheetButton);
            window.ViewCharacterOverlay.SetActive(this.CharacterSheetButton);
        }
    }

    public override void Invoke()
    {
        this.GlowScenarioSceneObjects();
        this.GlowLocationSceneObjects();
    }
}

