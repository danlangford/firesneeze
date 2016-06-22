using System;

public class GameStateFactory
{
    public static GameState Create(GameStateType type)
    {
        if (type == GameStateType.Switch)
        {
            return new GameStateSwitch();
        }
        if (type == GameStateType.Blessing)
        {
            return new GameStateBlessing();
        }
        if (type == GameStateType.Setup)
        {
            return new GameStateSetup();
        }
        if (type == GameStateType.Acquire)
        {
            return new GameStateAcquire();
        }
        if (type == GameStateType.Ambush)
        {
            return new GameStateAmbush();
        }
        if (type == GameStateType.Horde)
        {
            return new GameStateHorde();
        }
        if (type == GameStateType.Villain)
        {
            return new GameStateVillain();
        }
        if (type == GameStateType.Combat)
        {
            return new GameStateCombat();
        }
        if (type == GameStateType.Reroll)
        {
            return new GameStateReroll();
        }
        if (type == GameStateType.Recharge)
        {
            return new GameStateRecharge();
        }
        if (type == GameStateType.Damage)
        {
            return new GameStateDamage();
        }
        if (type == GameStateType.Finish)
        {
            return new GameStateFinish();
        }
        if (type == GameStateType.AskClose)
        {
            return new GameStateAskClose();
        }
        if (type == GameStateType.Close)
        {
            return new GameStateClose();
        }
        if (type == GameStateType.Undisplay)
        {
            return new GameStateUndisplay();
        }
        if (type == GameStateType.Discard)
        {
            return new GameStateDiscard();
        }
        if (type == GameStateType.Reset)
        {
            return new GameStateReset();
        }
        if (type == GameStateType.ResetHand)
        {
            return new GameStateResetHand();
        }
        if (type == GameStateType.Death)
        {
            return new GameStateDeath();
        }
        if (type == GameStateType.End)
        {
            return new GameStateEnd();
        }
        if (type == GameStateType.Penalty)
        {
            return new GameStatePenalty();
        }
        if (type == GameStateType.Power)
        {
            return new GameStatePower();
        }
        if (type == GameStateType.Pick)
        {
            return new GameStatePick();
        }
        if (type == GameStateType.PickHand)
        {
            return new GameStatePickHand();
        }
        if (type == GameStateType.Give)
        {
            return new GameStateGive();
        }
        if (type == GameStateType.Target)
        {
            return new GameStateTarget();
        }
        if (type == GameStateType.Roll)
        {
            return new GameStateRoll();
        }
        if (type == GameStateType.RollAgain)
        {
            return new GameStateRollAgain();
        }
        if (type == GameStateType.SelectType)
        {
            return new GameStateSelectType();
        }
        if (type == GameStateType.Popup)
        {
            return new GameStatePopup();
        }
        if (type == GameStateType.Flee)
        {
            return new GameStateFlee();
        }
        if (type == GameStateType.Done)
        {
            return new GameStateDone();
        }
        if (type == GameStateType.Dispose)
        {
            return new GameStateDispose();
        }
        if (type == GameStateType.Examine)
        {
            return new GameStateExamine();
        }
        if (type == GameStateType.EndTurn)
        {
            return new GameStateEndTurn();
        }
        if (type == GameStateType.Evade)
        {
            return new GameStateEvade();
        }
        if (type == GameStateType.Encounter)
        {
            return new GameStateEncounter();
        }
        if (type == GameStateType.EncounterAgain)
        {
            return new GameStateEncounterAgain();
        }
        if (type == GameStateType.Post)
        {
            return new GameStatePost();
        }
        if (type == GameStateType.Null)
        {
            return new GameStateNull();
        }
        if (type == GameStateType.StartTurn)
        {
            return new GameStateStartTurn();
        }
        if (type == GameStateType.Sacrifice)
        {
            return new GameStateSacrifice();
        }
        if (type == GameStateType.Closing)
        {
            return new GameStateClosing();
        }
        if (type == GameStateType.Confirm)
        {
            return new GameStateConfirm();
        }
        if (type == GameStateType.VillainIntro)
        {
            return new GameStateVillainIntro();
        }
        if (type == GameStateType.Move)
        {
            return new GameStateMove();
        }
        if (type == GameStateType.PreEnd)
        {
            return new GameStatePreEnd();
        }
        if (type == GameStateType.ClosePrompt)
        {
            return new GameStateClosePrompt();
        }
        if (type == GameStateType.Proceed)
        {
            return new GameStateProceed();
        }
        if (type == GameStateType.Share)
        {
            return new GameStateShare();
        }
        if (type == GameStateType.EvadeOption)
        {
            return new GameStateEvadeOption();
        }
        if (type == GameStateType.Henchman)
        {
            return new GameStateHenchman();
        }
        if (type == GameStateType.HenchmanIntro)
        {
            return new GameStateHenchmanIntro();
        }
        if (type == GameStateType.ConfirmProceed)
        {
            return new GameStateConfirmProceed();
        }
        if (type == GameStateType.ConfirmPowerUse)
        {
            return new GameStateConfirmPowerUse();
        }
        if (type == GameStateType.TempClose)
        {
            return new GameStateTempClose();
        }
        if (type == GameStateType.DiscardHand)
        {
            return new GameStateDiscardHand();
        }
        if (type == GameStateType.Forfeit)
        {
            return new GameStateForfeit();
        }
        return null;
    }
}

