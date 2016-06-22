using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Event : MonoBehaviour
{
    [Tooltip("this event only triggers when the condition is true")]
    public PowerConditionType[] Conditions;

    protected Event()
    {
    }

    public static void Done()
    {
        Game.Events.Next();
    }

    protected void Done(GameStateType state)
    {
        if (Game.Events.ContainsStatefulEvent())
        {
            Turn.State = state;
        }
        Done();
    }

    public static void DonePost(GameStateType state)
    {
        EventCallbackManager events = Game.Events;
        events.Post++;
        Turn.State = state;
    }

    public virtual void EndGameIfNecessary(Card card)
    {
    }

    public static bool Finished() => 
        !Game.Events.ContainsStatefulEvent();

    public virtual int GetBaseCheckModifier() => 
        0;

    public virtual int GetCheckBonus() => 
        0;

    public virtual DiceType GetCheckDice() => 
        DiceType.D0;

    public virtual int GetCheckModifier() => 
        0;

    public virtual int GetDiceModifier(DiceType type) => 
        0;

    protected void GlowCardText(bool isGlowing)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutLocation.GlowText(isGlowing);
        }
    }

    public virtual bool HasPostHordeEvent()
    {
        Event[] components = base.GetComponents<Event>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i].IsEventImplemented(EventType.OnPostHorde))
            {
                return true;
            }
        }
        return false;
    }

    public virtual bool IsCardEvent() => 
        (base.GetComponent<Card>() != null);

    protected bool IsConditionValid(Card card) => 
        PowerCondition.Evaluate(card, this.Conditions);

    public virtual bool IsEventImplemented(EventType type) => 
        (this.Type == type);

    public virtual bool IsEventValid(Card card) => 
        true;

    protected bool IsScenarioEvent() => 
        ((base.GetComponent<ScenarioPower>() != null) || (base.GetComponent<Scenario>() != null));

    public virtual void OnAfterExplore()
    {
        Done();
    }

    public virtual void OnBeforeAct()
    {
        Done();
    }

    public virtual void OnBeforeTurnStart()
    {
        Done();
    }

    public virtual void OnCallback()
    {
    }

    public virtual void OnCardActivated(Card card)
    {
    }

    public virtual void OnCardBuried(Card card)
    {
        Done();
    }

    public virtual void OnCardDeactivated(Card card)
    {
    }

    public virtual void OnCardDefeated(Card card)
    {
        Done();
    }

    public virtual void OnCardDestroyed(Card card)
    {
    }

    public virtual void OnCardDiscarded(Card card)
    {
    }

    public virtual void OnCardEncountered(Card card)
    {
        Done();
    }

    public virtual void OnCardEvaded(Card card)
    {
        Done();
    }

    public virtual void OnCardPlayed(Card card)
    {
        Done();
    }

    public virtual void OnCardRecharged(Card card)
    {
    }

    public virtual void OnCardUndefeated(Card card)
    {
        Done();
    }

    public virtual void OnCardUndefeatedSequence(Card card)
    {
        Done();
    }

    public virtual bool OnCombatEnd(Card card) => 
        true;

    public virtual void OnCombatResolved()
    {
        Done();
    }

    public virtual void OnDamageTaken(Card card)
    {
        Done();
    }

    public virtual void OnDiceRolled()
    {
        Done();
    }

    public virtual void OnEndOfTurnEnded()
    {
        Done();
    }

    public virtual void OnExamineAnyLocation()
    {
        Done();
    }

    public virtual void OnHandReset()
    {
        Done();
    }

    public virtual void OnLocationChange()
    {
        Done();
    }

    public virtual void OnLocationCloseAttempted()
    {
        Done();
    }

    public virtual void OnLocationClosed()
    {
        Done();
    }

    public virtual void OnLocationExplored(Card card)
    {
    }

    public virtual void OnPlayerDamaged(Card card)
    {
        Done();
    }

    public virtual void OnPostAct()
    {
        Done();
    }

    public virtual void OnPostHorde(Card card)
    {
        Done();
    }

    public virtual void OnScenarioExit()
    {
    }

    public virtual void OnSecondCombat(Card card)
    {
        Done();
    }

    public virtual void OnTurnEnded()
    {
        Done();
    }

    public virtual void OnTurnStarted()
    {
        Done();
    }

    public virtual void OnVillainIntroduced(Card card)
    {
    }

    protected void RefreshDicePanel()
    {
        Turn.DiceTarget = 0;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Refresh();
        }
    }

    [DebuggerHidden]
    protected IEnumerator WaitForTime(float time) => 
        new <WaitForTime>c__Iterator1D { 
            time = time,
            <$>time = time
        };

    public virtual TurnStateCallbackType CallbackType
    {
        get
        {
            if (this.IsCardEvent())
            {
                return TurnStateCallbackType.Card;
            }
            if (base.GetComponent<ScenarioPower>() != null)
            {
                return TurnStateCallbackType.Scenario;
            }
            if (base.GetComponent<CharacterPower>() != null)
            {
                return TurnStateCallbackType.Character;
            }
            return TurnStateCallbackType.Location;
        }
    }

    protected Card Card =>
        base.GetComponent<Card>();

    public virtual bool Stateless =>
        true;

    public virtual bool TriggerOnEachCheck =>
        false;

    public virtual EventType Type =>
        EventType.None;

    [CompilerGenerated]
    private sealed class <WaitForTime>c__Iterator1D : IDisposable, IEnumerator, IEnumerator<object>
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

