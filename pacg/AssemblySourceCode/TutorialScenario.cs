using System;

public class TutorialScenario : Scenario
{
    protected override void BuildBox()
    {
        Campaign.Box.Clear();
        Campaign.LoadBox("1T", "B");
    }

    public override void Exit()
    {
        if (base.Complete)
        {
            Conquests.Complete(Constants.STORY_MODE_UNLOCKED);
        }
        if (!GameDirectory.Empty(Constants.SAVE_SLOT_TUTORIAL))
        {
            new GameSaveFile(Constants.SAVE_SLOT_TUTORIAL).Delete();
        }
        base.Exit();
    }

    protected override void ExitToNextScene(bool isAnyoneDead)
    {
        int slot = GameDirectory.FindEmptySlot();
        if (slot >= GameDirectory.FirstSlot)
        {
            Game.Play(GameType.LocalSinglePlayer, slot, WindowType.CreateParty, null, false);
        }
        else
        {
            Game.Restart();
        }
    }

    public override bool Rewardable =>
        !Conquests.IsComplete(Constants.STORY_MODE_UNLOCKED);
}

