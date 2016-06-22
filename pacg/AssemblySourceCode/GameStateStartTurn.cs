using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameStateStartTurn : GameState
{
    public override void Enter()
    {
        base.Enter();
        if (!Scenario.Current.IsLocationExplored(Location.Current.ID))
        {
            ScenarioPropertyCutscene component = Scenario.Current.GetComponent<ScenarioPropertyCutscene>();
            if ((component != null) && component.IsValid())
            {
                Game.Instance.StartCoroutine(this.ShowCutscene(component));
                return;
            }
            Scenario.Current.SetLocationExplored(Location.Current.ID, true);
        }
        Turn.InitialLocation = Location.Current.ID;
        bool flag = this.LocationHasTurnStartedEvent();
        bool flag2 = this.ScenarioHasTurnStartedEvent();
        bool flag3 = Rules.IsAnyActionPossible();
        if ((flag3 || flag) || flag2)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                base.Message(0x54);
                window.ShowCancelButton(false);
                window.commandsPanel.ShowDiscardButton(false);
                window.commandsPanel.ShowEndTurnButton(false);
                window.commandsPanel.ShowGiveButton(false);
                window.commandsPanel.ShowMoveButton(false);
                if (flag2)
                {
                    window.powersPanel.ShowScenarioPowerProceedButton(true);
                    if (!flag3)
                    {
                        window.shadePanel.Show(window.shadePanel.LocationShade, true);
                    }
                }
                else if (flag)
                {
                    window.powersPanel.ShowLocationPowerProceedButton(true);
                    if (!flag3)
                    {
                        window.shadePanel.Show(window.shadePanel.LocationShade, true);
                    }
                }
                else
                {
                    window.ShowProceedButton(true);
                }
            }
        }
        else
        {
            this.Proceed();
        }
    }

    public override void Exit(GameStateType nextState)
    {
        base.Message((string) null);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.powersPanel.ShowScenarioPowerProceedButton(false);
            window.powersPanel.ShowLocationPowerProceedButton(false);
        }
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        card.IsActionAllowed(action);

    private bool LocationHasTurnStartedEvent()
    {
        if (!Turn.Started)
        {
            Event[] components = Scenario.Current.GetLocationPowersRoot(Location.Current.ID).GetComponents<Event>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnTurnStarted) && components[i].IsEventValid(Turn.Card))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override void Proceed()
    {
        if (!Turn.Started)
        {
            Turn.Started = true;
            Turn.CombatSkill = Turn.Owner.GetCombatSkill();
            Turn.InitialCharacter = Turn.Current;
            Turn.BlackBoard.Set<int>("GameStateRecharge_Iterator", Turn.Current);
            Game.Save();
            if (Turn.InitialLocation == Location.Current.ID)
            {
                Location.Current.OnTurnStarted();
                Scenario.Current.OnTurnStarted();
                if (Turn.State != GameStateType.StartTurn)
                {
                    return;
                }
            }
        }
        Turn.ClearStartTurnData();
        Turn.State = GameStateType.Setup;
    }

    private bool ScenarioHasTurnStartedEvent()
    {
        if (!Turn.Started)
        {
            for (int i = 0; i < Scenario.Current.Powers.Count; i++)
            {
                Event[] components = Scenario.Current.Powers[i].GetComponents<Event>();
                for (int j = 0; j < components.Length; j++)
                {
                    if ((components[j].Type == EventType.OnTurnStarted) && components[j].IsEventValid(Turn.Card))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    [DebuggerHidden]
    private IEnumerator ShowCutscene(ScenarioPropertyCutscene cutscene) => 
        new <ShowCutscene>c__Iterator41 { 
            cutscene = cutscene,
            <$>cutscene = cutscene
        };

    public override GameStateType Type =>
        GameStateType.StartTurn;

    [CompilerGenerated]
    private sealed class <ShowCutscene>c__Iterator41 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal ScenarioPropertyCutscene <$>cutscene;
        internal GuiWindowLocation <window>__0;
        internal ScenarioPropertyCutscene cutscene;

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
                    if (this.<window>__0 != null)
                    {
                        this.<window>__0.Pause(true);
                        this.<window>__0.shadePanel.Show(this.<window>__0.shadePanel.LocationShade, true);
                    }
                    this.$current = new WaitForSeconds(1.25f);
                    this.$PC = 1;
                    return true;

                case 1:
                    Scenario.Current.SetLocationExplored(Location.Current.ID, true);
                    this.cutscene.Play();
                    this.$PC = -1;
                    break;
            }
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

