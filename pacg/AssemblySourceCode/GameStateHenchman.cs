using System;

public class GameStateHenchman : GameState
{
    public static void Encounter(Card henchman)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutLocation.Show(true);
            window.layoutLocation.ShowPreludeFX(true);
            window.layoutLocation.Refresh();
        }
        Turn.State = GameStateType.Null;
        bool flag = Game.Events.ContainsStatefulEvent();
        Event.Done();
        if (!flag)
        {
            Turn.State = GameStateType.Combat;
        }
    }

    public override void Enter()
    {
        if (!string.IsNullOrEmpty(Cutscene.Queue))
        {
            this.ShowCutscene();
        }
        else
        {
            Encounter(Turn.Card);
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
        GameStateType.Henchman;
}

