using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameStateForfeit : GameState
{
    [DebuggerHidden]
    private IEnumerator BurnDownBlessings() => 
        new <BurnDownBlessings>c__Iterator3B { <>f__this = this };

    private static int CountRemainingPenaltyCards(TurnStateData data)
    {
        int b = 0;
        if (data != null)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                for (int i = 0; i < data.Actions.Length; i++)
                {
                    b += window.GetNumCardsInLayout(data.Actions[i]);
                }
            }
            b = data.NumCards - b;
        }
        return Mathf.Max(0, b);
    }

    public override void Enter()
    {
        base.Enter();
        this.KillCharactersWhoAreUnderHandSize();
        Game.Instance.StartCoroutine(this.BurnDownBlessings());
    }

    public override void Exit(GameStateType nextState)
    {
        Penalty = 0;
    }

    public static void Initialize()
    {
        Penalty = 0;
        if (Turn.State == GameStateType.Penalty)
        {
            TurnStateData stateData = Turn.GetStateData();
            Penalty += CountRemainingPenaltyCards(stateData);
        }
        if ((Turn.State == GameStateType.Damage) || (Turn.State == GameStateType.Ambush))
        {
            Penalty += Rules.GetCardsToDiscardCount();
        }
    }

    private void KillCharactersWhoAreUnderHandSize()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            int numberDiscardableCards = Party.Characters[i].GetNumberDiscardableCards();
            if (Turn.Owner.ID == Party.Characters[i].ID)
            {
                numberDiscardableCards -= Penalty;
                numberDiscardableCards = Mathf.Max(0, numberDiscardableCards);
            }
            if (numberDiscardableCards < Party.Characters[i].HandSize)
            {
                int num3 = Party.Characters[i].HandSize - numberDiscardableCards;
                if (num3 > Party.Characters[i].Deck.Count)
                {
                    Party.Characters[i].Alive = false;
                }
            }
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.partyPanel.Refresh();
        }
    }

    public override void Proceed()
    {
        Scenario.Current.Forfeit = true;
        Turn.State = GameStateType.End;
    }

    private static int Penalty
    {
        [CompilerGenerated]
        get => 
            <Penalty>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Penalty>k__BackingField = value;
        }
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Forfeit;

    [CompilerGenerated]
    private sealed class <BurnDownBlessings>c__Iterator3B : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GameStateForfeit <>f__this;
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
                    if ((this.<window>__0 == null) || (Scenario.Current.Blessings.Count <= 0))
                    {
                        break;
                    }
                    this.<window>__0.blessingsPanel.BurnStart();
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 1;
                    goto Label_00E1;

                case 1:
                case 2:
                    if (Scenario.Current.Blessings.Count > 0)
                    {
                        this.<window>__0.blessingsPanel.Burn();
                        this.$current = new WaitForSeconds(0.3f);
                        this.$PC = 2;
                        goto Label_00E1;
                    }
                    break;

                default:
                    goto Label_00DF;
            }
            this.<>f__this.Proceed();
            this.$PC = -1;
        Label_00DF:
            return false;
        Label_00E1:
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

