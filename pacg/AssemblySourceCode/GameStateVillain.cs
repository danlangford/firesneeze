using System;

public class GameStateVillain : GameState
{
    private void CloseLocations()
    {
        Turn.Iterators.Start(TurnStateIteratorType.Close);
    }

    public static void Encounter(Card villain)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutLocation.Show(true);
            window.layoutLocation.ShowPreludeFX(true);
        }
        Turn.State = GameStateType.EvadeOption;
    }

    public override void Enter()
    {
        if (!Campaign.IsCardEncountered(Turn.Card.ID))
        {
            Campaign.SetCardEncountered(Turn.Card.ID);
            if (Cutscene.Exists(CutsceneType.Villain))
            {
                this.ShowCutscene();
                return;
            }
        }
        Scenario.Current.OnVillainIntroduced(Turn.Card);
        if (base.IsCurrentState())
        {
            if (Rules.IsTemporaryClosePossible())
            {
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (window != null)
                {
                    window.ShowExploreButton(false);
                    window.ShowProceedButton(false);
                    window.ShowCancelButton(false);
                }
                this.CloseLocations();
            }
            else
            {
                Encounter(Turn.Card);
            }
        }
    }

    private void ShowCutscene()
    {
        Location.Current.OnSaveData();
        UI.Window.Pause(true);
        Game.UI.ShowCutsceneScene();
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Villain;
}

