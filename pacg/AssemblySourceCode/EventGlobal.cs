using System;
using UnityEngine;

public class EventGlobal : MonoBehaviour
{
    private void CardRestrictionPending_Resolve()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Character restrictedCharacter = EffectCardRestrictionPending.GetRestrictedCharacter();
            if (restrictedCharacter != null)
            {
                bool flag = false;
                EffectCardRestrictionPending effect = restrictedCharacter.GetEffect(EffectType.CardRestrictionPending) as EffectCardRestrictionPending;
                if (effect != null)
                {
                    restrictedCharacter.RemoveEffect(effect);
                    flag = effect.Resolve();
                }
                Turn.State = Turn.PopReturnState();
                Turn.Park(false);
                if (flag)
                {
                    effect.Play();
                }
                Turn.Current = Turn.InitialCharacter;
                window.Refresh();
            }
        }
    }

    private void ClearTextGlow()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.locationPanel.GlowText(TextHilightType.None);
        }
    }

    private void Death_GetNextCharacter()
    {
        Turn.Next();
        if (Turn.Character.Alive)
        {
            Turn.State = GameStateType.Switch;
        }
        else
        {
            Turn.State = GameStateType.End;
        }
    }

    private void EventDone()
    {
        Turn.RollReason = RollType.PlayerControlled;
        int count = Game.Events.Count;
        Event.Done();
        if (count <= 0)
        {
            Turn.Proceed();
        }
    }

    private void EventEncounteredSummon_Finish()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (((Location.Current.ID != Turn.EncounteredLocation) && (window.layoutSummoner.Card != null)) && (window.layoutSummoner.Card.Type != CardType.Villain))
            {
                Location.Load(Turn.EncounteredLocation);
            }
            window.layoutSummoner.Clear();
            window.layoutLocation.Show(true);
            if (Turn.SummonsType == SummonsType.Target)
            {
                Turn.SwitchCharacter(Turn.InitialCharacter);
                Turn.Current = Turn.Number;
                this.RefreshLocationWindow();
            }
            if ((Turn.Card.Disposition != DispositionType.None) && (Turn.SummonsType == SummonsType.Target))
            {
                Turn.State = GameStateType.Dispose;
            }
            else if (Turn.SummonsType == SummonsType.Target)
            {
                Location.Load(Turn.Character.Location);
                Turn.EvadeDeclined = true;
                Turn.EncounteredGuid = Turn.Card.GUID;
                Turn.State = GameStateType.Combat;
            }
            else
            {
                Turn.State = GameStateType.Finish;
            }
            Turn.SummonsType = SummonsType.None;
        }
    }

    private void EventHorde_Target()
    {
        Turn.SwitchCharacter(Turn.Target);
        Turn.Current = Turn.Number;
        Character character = Turn.Character;
        character.HordeFightLeft--;
        Turn.ClearCombatData();
        Location.Load(Turn.Character.Location);
        this.RefreshLocationWindow();
        Turn.Dice.Clear();
        Turn.SummonsType = SummonsType.Horde;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutSummoner.Summon(Turn.SummonsMonster);
        }
        Turn.State = GameStateType.Encounter;
    }

    private void EventLocationCloseAsk_Banish()
    {
        Turn.BlackBoard.Set<int>("CloseLocationMenuChoice", 1);
        Turn.State = GameStateType.Close;
    }

    private void EventLocationCloseAsk_Checks()
    {
        Turn.BlackBoard.Set<int>("CloseLocationMenuChoice", 2);
        Turn.State = GameStateType.Close;
    }

    private void EventLocationCloseAsk_Summons()
    {
        Turn.BlackBoard.Set<int>("CloseLocationMenuChoice", 3);
        Turn.State = GameStateType.Close;
    }

    private void EventLocationCloseBanish_Finish()
    {
        this.ClearTextGlow();
        if (Turn.CloseType == CloseType.Temporary)
        {
            Location.Current.Closed = true;
            Turn.Iterators.Next(TurnStateIteratorType.Close);
        }
        else
        {
            if ((Turn.State == GameStateType.Close) || (Turn.State == GameStateType.Penalty))
            {
                Turn.PushStateDestination(GameStateType.Done);
            }
            else
            {
                Turn.PushStateDestination(Turn.State);
            }
            Turn.State = GameStateType.Closing;
        }
    }

    private void EventLocationCloseSummons_Finish()
    {
        this.ClearTextGlow();
        if (Turn.CloseType == CloseType.Temporary)
        {
            if (Turn.LastCombatResult == CombatResultType.Win)
            {
                Location.Current.Closed = true;
            }
            Turn.Iterators.Next(TurnStateIteratorType.Close);
        }
        else
        {
            Turn.SummonsType = SummonsType.None;
            Turn.EncounterType = EncounterType.None;
            if (((Turn.State == GameStateType.Close) || (Turn.State == GameStateType.Recharge)) || (Turn.State == GameStateType.Damage))
            {
                Turn.PushStateDestination(GameStateType.Done);
            }
            else
            {
                Turn.PushStateDestination(Turn.State);
            }
            if (Turn.LastCombatResult == CombatResultType.Win)
            {
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (((window != null) && (window.layoutSummoner.Card != null)) && (window.layoutSummoner.Card.Disposition != DispositionType.Banish))
                {
                    if (Turn.Close)
                    {
                        Turn.CloseType = CloseType.None;
                    }
                    Turn.Close = false;
                    Turn.GotoStateDestination();
                }
                else
                {
                    Turn.State = GameStateType.Closing;
                }
            }
            else
            {
                if (Turn.Close)
                {
                    Turn.CloseType = CloseType.None;
                }
                Turn.Close = false;
                Turn.GotoStateDestination();
            }
        }
    }

    private void EventLocationCloseViaAcquire_Finish()
    {
        this.ClearTextGlow();
        Turn.Close = true;
        if (Turn.State == GameStateType.Recharge)
        {
            Turn.State = GameStateType.Dispose;
        }
    }

    private void EventPassCheck_Cancel()
    {
        Turn.Pass = true;
        Turn.SwitchCharacter(Turn.InitialCharacter);
        Turn.Current = Turn.InitialCharacter;
        Turn.TargetType = TargetType.None;
        Turn.State = GameStateType.Combat;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.PassButton.Show(true);
        }
        this.RefreshLocationWindow();
    }

    private void EventPassCheck_Proceed()
    {
        Turn.SwitchCharacter(Turn.Target);
        Turn.Current = Turn.Target;
        Turn.CombatSkill = Turn.Owner.GetCombatSkill();
        Turn.TargetType = TargetType.None;
        Turn.State = GameStateType.Combat;
        this.RefreshLocationWindow();
    }

    private void EventPlayerDamage_Damage()
    {
        Turn.Damage = Turn.DiceTotal;
        if (Turn.Damage > 0)
        {
            VisualEffectType cardLoseVfx = Rules.GetCardLoseVfx(Turn.DamageTraits);
            UI.Sound.Play(cardLoseVfx.ToSoundtype());
            VisualEffect.ApplyToPlayer(cardLoseVfx, 1.3f);
            Turn.Card.Animate(AnimationType.Attack, true);
        }
        Turn.EmptyLayoutDecks = true;
        Turn.RollReason = RollType.PlayerControlled;
        Turn.DiceBonus = 0;
        if ((Turn.CombatStage == TurnCombatStageType.PreEncounter) || (Turn.CombatStage == TurnCombatStageType.None))
        {
            Turn.State = GameStateType.Ambush;
        }
        else
        {
            Turn.State = GameStateType.Damage;
        }
    }

    private void GameStateFinish_Back()
    {
        Turn.State = GameStateType.Finish;
        Turn.EmptyLayoutDecks = true;
        Turn.Explore = true;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowExploreButton(Rules.IsExplorePossible());
        }
    }

    private void GameStateFinish_Continue()
    {
        Turn.End = true;
        this.GameStateFinish_ForfeitExplore();
    }

    private void GameStateFinish_Explore()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.Card.Show(CardSideType.Back);
            window.layoutLocation.OnGuiDrop(Turn.Card);
        }
    }

    private void GameStateFinish_ForfeitExplore()
    {
        Turn.EmptyLayoutDecks = true;
        Turn.Explore = false;
        Turn.State = GameStateType.Finish;
    }

    private void GameStateRecharge_Ask_No()
    {
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_No", true);
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_Yes", false);
        this.GameStateRecharge_Set(ActionType.Top, false);
        this.GameStateRecharge_Set(ActionType.Shuffle, false);
        Turn.State = GameStateType.Recharge;
    }

    private void GameStateRecharge_Ask_Shuffle()
    {
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_No", false);
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_Yes", false);
        this.GameStateRecharge_Set(ActionType.Top, false);
        this.GameStateRecharge_Set(ActionType.Shuffle, true);
        Turn.State = GameStateType.Recharge;
    }

    private void GameStateRecharge_Ask_Top()
    {
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_No", false);
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_Yes", false);
        this.GameStateRecharge_Set(ActionType.Top, true);
        this.GameStateRecharge_Set(ActionType.Shuffle, false);
        Turn.State = GameStateType.Recharge;
    }

    private void GameStateRecharge_Ask_Yes()
    {
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_No", false);
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_Yes", true);
        this.GameStateRecharge_Set(ActionType.Top, false);
        this.GameStateRecharge_Set(ActionType.Shuffle, false);
        Turn.State = GameStateType.Recharge;
    }

    private void GameStateRecharge_Set(ActionType action, bool value)
    {
        Turn.BlackBoard.Set<bool>("GameStateRecharge_Ask_" + action.ToString(), value);
    }

    private void Iterator_Next()
    {
        Turn.Iterators.Next();
    }

    private void MapMove_Cancel()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.mapPanel.CancelMove();
        }
    }

    private void MapMove_FinishMove()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.mapPanel.FinishMove();
        }
    }

    private void MapMove_Move()
    {
        Turn.Target = Turn.Current;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.mapPanel.Mode = MapModeType.Move;
        }
        Location.Current.Move();
    }

    private void RefreshLocationWindow()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessLayoutDecks();
            window.Refresh();
        }
    }

    private void State_FinishDamage()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessRechargableCards();
            window.ProcessLayoutDecks();
            Event.Done();
            Turn.ClearCheckData();
            Turn.State = GameStateType.Damage;
        }
    }

    private void State_FinishTurn()
    {
        Turn.PushStateDestination(GameStateType.Post);
        Turn.State = GameStateType.Recharge;
        Event.Done();
    }

    private void Temp_CancelClose()
    {
        if (Turn.Iterators.Current is IteratorClose)
        {
            Turn.Iterators.Invoke();
        }
    }
}

