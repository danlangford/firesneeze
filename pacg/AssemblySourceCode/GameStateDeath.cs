using System;

public class GameStateDeath : GameState
{
    public override void Enter()
    {
        while (Turn.ReturnState != GameStateType.None)
        {
            Turn.PopReturnState();
        }
        UI.Sound.Play(SoundEffectType.CharacterDied);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.deathPanel.Show(true);
        }
        UI.Sound.MusicStop();
    }

    public override void Proceed()
    {
        UI.Sound.MusicResume();
        Turn.GotoStateDestination();
    }

    public override GameStateType Type =>
        GameStateType.Death;
}

