using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class CardPower : MonoBehaviour
{
    [Tooltip("which action is used to invoke this power (reveal, discard, etc.)")]
    public ActionType Action;
    [Tooltip("can this power be used to aid another character's check?")]
    public AidType Aid;
    [Tooltip("can only use this power when this condition is true")]
    public PowerConditionType[] Conditions;
    [Tooltip("show on pop up menus when more than one power can be invoked")]
    public string DisplayText;
    [Tooltip("can this power be used by other characters?")]
    public UseType Use;

    protected CardPower()
    {
    }

    public virtual void Activate(Card card)
    {
        if (Rules.GetMarkedType(card) == CardType.Blessing)
        {
            if (((this.Action == ActionType.Recharge) && (Scenario.Current.Discard.Count > 0)) && (Scenario.Current.Discard[0].ID == this.Card.ID))
            {
                UI.Sound.Play(SoundEffectType.BlessingSameBonus);
            }
            else
            {
                UI.Sound.Play(SoundEffectType.BlessingNormal);
            }
        }
    }

    public virtual void Deactivate(Card card)
    {
    }

    public virtual string GetCardDecoration(Card card) => 
        null;

    public bool IsActionAllowed(ActionType action, Card card)
    {
        if (Rules.IsUnlimitedPlayPossible(card, this.Action))
        {
            return ((action == ActionType.Recharge) && this.IsPowerAllowed(card));
        }
        if (action != this.Action)
        {
            return false;
        }
        return this.IsPowerAllowed(card);
    }

    public bool IsActionPossible(ActionType action, Card card)
    {
        if (Turn.Operation == TurnOperationType.Validation)
        {
            return this.IsActionValid(action, card);
        }
        return this.IsActionAllowed(action, card);
    }

    public bool IsActionValid(ActionType action, Card card)
    {
        if (Rules.IsUnlimitedPlayPossible(card, this.Action))
        {
            return ((action == ActionType.Recharge) && this.IsPowerValid(card));
        }
        if (action == this.Action)
        {
            return this.IsPowerValid(card);
        }
        return ((this.Action == ActionType.Reveal) && (card.PlayedPowers.Count > 1));
    }

    protected bool IsAidPossible(Card card)
    {
        if (!Rules.IsTurnOwner())
        {
            if (this.Aid == AidType.None)
            {
                return false;
            }
            if ((this.Aid == AidType.LocalOther) || (this.Aid == AidType.Local))
            {
                return (Party.Characters[Turn.Current].Location == Party.Characters[Turn.Number].Location);
            }
            if (this.Aid == AidType.Remote)
            {
                return (Party.Characters[Turn.Current].Location != Party.Characters[Turn.Number].Location);
            }
        }
        else
        {
            if (this.Aid == AidType.Remote)
            {
                return false;
            }
            if (this.Aid == AidType.LocalOther)
            {
                return false;
            }
        }
        return true;
    }

    protected bool IsCardInPlay(Card card) => 
        ((this.Action != ActionType.None) && (card.PlayedPower >= 0));

    protected bool IsConditionValid(Card card)
    {
        if (!this.IsEvadeOrDefeatPowerValid())
        {
            return false;
        }
        return PowerCondition.Evaluate(card, this.Conditions);
    }

    public virtual bool IsEqualOrBetter(CardPower x) => 
        false;

    protected virtual bool IsEvadeOrDefeatPowerValid()
    {
        if ((((this.Type != PowerType.Evade) && Turn.Evade) || Turn.Defeat) && Rules.IsCombatCheck())
        {
            return false;
        }
        return true;
    }

    protected bool IsLockInDisplayNecessary() => 
        ((this.Action == ActionType.Display) && !Rules.IsCheck());

    protected virtual bool IsPowerActivationAllowed(Card card) => 
        this.IsPowerAllowed(card);

    protected virtual bool IsPowerAllowed(Card card) => 
        false;

    public virtual bool IsPowerDeactivationAllowed(Card card) => 
        true;

    protected virtual bool IsPowerValid(Card card) => 
        this.IsPowerAllowed(card);

    public bool IsValid(Card card) => 
        this.IsPowerValid(card);

    public virtual bool IsValidationRequired() => 
        true;

    protected void LockInDisplayed(bool confirm)
    {
        if (this.IsLockInDisplayNecessary())
        {
            this.Card.Locked = confirm;
        }
    }

    protected void ReturnControlToInitialCharacter()
    {
        string iD = Party.Characters[Turn.InitialCharacter].Location;
        if (Location.Current.ID != iD)
        {
            Location.Load(iD);
            GuiPanelSwitch.Animations = false;
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(false);
        }
        Turn.SwitchType = SwitchType.None;
        Turn.SwitchCharacter(Turn.InitialCharacter);
        Turn.Current = Turn.InitialCharacter;
        GuiPanelSwitch.Animations = true;
    }

    protected void SetupTurnSkillCheck(SkillCheckSituationType situation, SkillCheckType[] checks)
    {
        if (Turn.Operation != TurnOperationType.Validation)
        {
            for (int i = 0; i < checks.Length; i++)
            {
                if (Rules.IsSkillCompatible(Turn.Owner, checks[i], Turn.Check))
                {
                    return;
                }
            }
            if (((situation == SkillCheckSituationType.Any) || ((situation == SkillCheckSituationType.Combat) && (Turn.Check == SkillCheckType.Combat))) || ((situation == SkillCheckSituationType.NonCombat) && (Turn.Check != SkillCheckType.Combat)))
            {
                for (int k = 0; k < Turn.CheckParticipants.Count; k++)
                {
                    for (int m = 0; m < checks.Length; m++)
                    {
                        if (checks[m] == ((SkillCheckType) Turn.CheckParticipants[k]))
                        {
                            return;
                        }
                    }
                }
            }
            for (int j = 0; j < Turn.Checks.Length; j++)
            {
                for (int n = 0; n < checks.Length; n++)
                {
                    if (Rules.IsSkillCompatible(Turn.Character, checks[n], Turn.Checks[j].skill))
                    {
                        GuiWindowLocation window = UI.Window as GuiWindowLocation;
                        if (window != null)
                        {
                            if (Turn.State == GameStateType.Combat)
                            {
                                window.dicePanel.SetCheck(Turn.Card, Turn.Checks, Turn.Checks[j].skill);
                            }
                            else
                            {
                                window.dicePanel.SetCheck(null, Turn.Checks, Turn.Checks[j].skill);
                            }
                            return;
                        }
                    }
                }
            }
            if (Turn.Check == SkillCheckType.Combat)
            {
                for (int num6 = 0; num6 < checks.Length; num6++)
                {
                    if (checks[num6] == SkillCheckType.Melee)
                    {
                        Turn.CombatSkill = SkillCheckType.Melee;
                        return;
                    }
                }
            }
        }
    }

    protected void ShowDice(bool isVisible)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Show(isVisible);
        }
    }

    protected void ShowEncounterButton(bool isVisible)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowEncounterButton(isVisible);
        }
    }

    protected void ShowProceedButton(bool isVisible)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(isVisible);
        }
    }

    [DebuggerHidden]
    protected IEnumerator WaitForTime(float time) => 
        new <WaitForTime>c__Iterator12 { 
            time = time,
            <$>time = time
        };

    protected Card Card =>
        base.GetComponent<Card>();

    protected SkillCheckType Check
    {
        get
        {
            if (Turn.Operation == TurnOperationType.Validation)
            {
                return Turn.LastCheck;
            }
            return Turn.Check;
        }
    }

    public virtual ActionType RechargeAction =>
        this.Action;

    public virtual PowerType Type =>
        PowerType.None;

    [CompilerGenerated]
    private sealed class <WaitForTime>c__Iterator12 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>time;
        internal GuiWindowLocation <window>__0;
        internal float time;

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
                    if (this.<window>__0 == null)
                    {
                        this.$current = new WaitForSeconds(this.time);
                        this.$PC = 2;
                        goto Label_00BB;
                    }
                    break;

                case 1:
                    break;

                case 2:
                    goto Label_00B2;

                default:
                    goto Label_00B9;
            }
            if (this.time > 0f)
            {
                if (this.<window>__0.Visible)
                {
                    this.time -= Time.deltaTime;
                }
                this.$current = null;
                this.$PC = 1;
                goto Label_00BB;
            }
        Label_00B2:
            this.$PC = -1;
        Label_00B9:
            return false;
        Label_00BB:
            return true;
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

