using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterPowerMove : CharacterPower
{
    [Tooltip("should this move power bypass the location's move restrictions?")]
    public bool BypassMoveRestrictions;
    [Tooltip("message displayed when moving yourself")]
    public StrRefType MessageMove;
    [Tooltip("message displayed when moving another to your location")]
    public StrRefType MessageMoveAnother;
    [Tooltip("the distance this power can target")]
    public TargetType Range;

    public override void Activate()
    {
        base.PowerBegin();
        base.Activate();
        Turn.Explore = false;
        Turn.PushReturnState();
        if (!Turn.Owner.CanMove)
        {
            this.MovePlayer_Target();
        }
        else
        {
            this.MovePlayer_Activate();
        }
    }

    private int CountMoveTargets()
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (this.IsMoveTargetPossible(Party.Characters[i]))
            {
                num++;
            }
        }
        return num;
    }

    public override bool IsLegalActivation() => 
        true;

    private bool IsMoveTargetPossible(Character target)
    {
        if (!target.Alive)
        {
            return false;
        }
        if (!target.CanMove)
        {
            return false;
        }
        if (target.ID == Turn.Owner.ID)
        {
            return false;
        }
        if (target.Location == Turn.Owner.Location)
        {
            return false;
        }
        if (Scenario.Current.Linear && !Scenario.Current.IsLocationLinked(target.Location, Turn.Owner.Location))
        {
            return false;
        }
        return true;
    }

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if ((!this.BypassMoveRestrictions && !Turn.Owner.CanMove) && (this.Range == TargetType.None))
        {
            return false;
        }
        if (Turn.End && (Turn.EndReason == GameReasonType.MonsterForced))
        {
            return false;
        }
        return true;
    }

    private void MovePlayer_Activate()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.mapPanel.Mode = MapModeType.Choose;
            window.messagePanel.Show(this.MessageMove.ToString());
            Turn.PushCancelDestination(new TurnStateCallback(this, "MovePlayer_Cancel"));
            Turn.PushStateDestination(new TurnStateCallback(this, "MovePlayer_Target"));
            Turn.Target = Turn.Current;
            Location.Current.Move();
            if (this.Cancellable)
            {
                window.ShowCancelButton(true);
            }
            if (this.BypassMoveRestrictions)
            {
                Turn.Defeat = true;
            }
        }
    }

    private void MovePlayer_Cancel()
    {
        Turn.MarkPowerActive(this, false);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.Map = false;
            window.ShowMap(false);
            window.messagePanel.Clear();
            Turn.OptionalTarget = TargetPanelType.None;
            Turn.PopStateDestination();
            Turn.ReturnToReturnState();
        }
        Turn.Defeat = false;
        this.PowerEnd();
    }

    private void MovePlayer_Finish()
    {
        Turn.Current = Turn.InitialCharacter;
        Turn.Number = Turn.Current;
        Location.Load(Turn.Character.Location);
        Turn.Defeat = false;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessRechargableCards();
            window.ProcessLayoutDecks();
            window.messagePanel.Clear();
        }
        Turn.CheckBoard.Set<bool>("GameStateRecharge_KeepLayout", false);
        Turn.Explore = false;
        if (Turn.State != GameStateType.Penalty)
        {
            Turn.State = GameStateType.EndTurn;
        }
        this.PowerEnd();
    }

    private void MovePlayer_MoveTarget()
    {
        if (Turn.Target != Turn.Current)
        {
            Location.Load(Party.Characters[Turn.Target].Location);
            Turn.PushStateDestination(new TurnStateCallback(this, "MovePlayer_MoveTargetAnimate"));
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.mapPanel.Mode = MapModeType.Choose;
            }
            Turn.State = GameStateType.Move;
            if ((window != null) && (Turn.State != GameStateType.Move))
            {
                Turn.Number = Turn.Current;
                window.Refresh();
            }
        }
        else
        {
            this.MovePlayer_Finish();
        }
    }

    private void MovePlayer_MoveTargetAnimate()
    {
        base.StartCoroutine(this.MovePlayer_MoveTargetAnimateCoroutine());
    }

    [DebuggerHidden]
    private IEnumerator MovePlayer_MoveTargetAnimateCoroutine() => 
        new <MovePlayer_MoveTargetAnimateCoroutine>c__Iterator18 { <>f__this = this };

    private void MovePlayer_Target()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.messagePanel.Clear();
            if ((this.Range == TargetType.None) || (this.CountMoveTargets() < 1))
            {
                if (Turn.State == Turn.ReturnState)
                {
                    this.MovePlayer_Cancel();
                }
                else
                {
                    this.MovePlayer_Finish();
                }
            }
            else
            {
                Turn.Target = Turn.Current;
                Turn.OptionalTarget = TargetPanelType.Optional;
                Turn.TargetType = this.Range;
                Turn.PushStateDestination(new TurnStateCallback(this, "MovePlayer_MoveTarget"));
                GameStateTarget.DisplayText = this.MessageMoveAnother.ToString();
                this.SetupTargetPanel();
                Turn.State = GameStateType.Target;
                window.ShowCancelButton(false);
                window.ShowProceedButton(false);
            }
        }
    }

    private void SetupTargetPanel()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (!this.IsMoveTargetPossible(Party.Characters[i]))
            {
                Party.Characters[i].Active = ActiveType.Inactive;
            }
        }
    }

    public override bool Cancellable =>
        (base.Cancellable && (Turn.PeekCancelDestination() != null));

    [CompilerGenerated]
    private sealed class <MovePlayer_MoveTargetAnimateCoroutine>c__Iterator18 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal CharacterPowerMove <>f__this;
        internal Character[] <characters>__1;
        internal string[] <locations>__2;
        internal GuiWindowLocation <window>__0;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    if ((this.<window>__0 == null) || (Party.Characters[Turn.Target].Location == Party.Characters[Turn.InitialCharacter].Location))
                    {
                        break;
                    }
                    this.<window>__0.ShowMap(true);
                    this.<characters>__1 = new Character[] { Party.Characters[Turn.Target] };
                    this.<locations>__2 = new string[] { Party.Characters[Turn.InitialCharacter].Location };
                    this.$current = Game.Instance.StartCoroutine(this.<window>__0.mapPanel.Animate(this.<characters>__1, this.<locations>__2));
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<window>__0.ShowMap(false);
                    break;

                default:
                    goto Label_0121;
            }
            this.<>f__this.MovePlayer_Finish();
            this.$PC = -1;
        Label_0121:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }
}

