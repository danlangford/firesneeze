using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class GameStateHorde : GameState
{
    public override void Enter()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowExploreButton(false);
            window.ShowProceedButton(false);
            window.ShowCancelButton(false);
            window.ShowEncounterButton(true);
        }
        base.Message(this.GetHelpText());
        bool evadeDeclined = Turn.EvadeDeclined;
        Turn.ClearEncounterData();
        if (evadeDeclined)
        {
            Turn.EvadeDeclined = evadeDeclined;
            this.Proceed();
        }
    }

    protected override string GetHelpText() => 
        StringTableManager.GetHelperText(0x48);

    public override bool IsActionAllowed(ActionType action, Card card) => 
        card.IsActionAllowed(action);

    public override void Proceed()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowEncounterButton(false);
        }
        base.Message((string) null);
        if (Turn.Evade)
        {
            Turn.Iterators.Remove(TurnStateIteratorType.Horde);
            Turn.SummonsType = SummonsType.None;
            Party.OnCheckCompleted();
            if (base.IsCurrentState())
            {
                Turn.State = GameStateType.Post;
            }
        }
        else if (Turn.Defeat)
        {
            Turn.Card.OnDefeated();
            Turn.SummonsType = SummonsType.None;
            Turn.State = GameStateType.Dispose;
        }
        else
        {
            Game.Instance.StartCoroutine(this.SummonMonster(Turn.SummonsMonster));
        }
    }

    [DebuggerHidden]
    private IEnumerator SummonMonster(string ID) => 
        new <SummonMonster>c__Iterator3D { 
            ID = ID,
            <$>ID = ID
        };

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Horde;

    [CompilerGenerated]
    private sealed class <SummonMonster>c__Iterator3D : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal string <$>ID;
        internal string <summoner>__0;
        internal GuiWindowLocation <window>__1;
        internal string ID;

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
                    this.<summoner>__0 = Turn.Card.ID;
                    if (Rules.IsSummonerBanished(Turn.Card))
                    {
                        Campaign.Box.Add(Turn.Card, false);
                    }
                    Turn.OptionalTarget = TargetPanelType.Next;
                    this.<window>__1 = UI.Window as GuiWindowLocation;
                    if (this.<window>__1 == null)
                    {
                        break;
                    }
                    this.<window>__1.layoutLocation.Show(false);
                    Turn.Card.Show(false);
                    this.<window>__1.layoutLocation.ShowPreludeFX(false);
                    this.<window>__1.layoutLocation.Refresh();
                    this.<window>__1.layoutSummoner.Show(this.<summoner>__0);
                    this.<window>__1.layoutSummoner.Animate(ActionType.Display);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.4f));
                    this.$PC = 1;
                    goto Label_0189;

                case 1:
                    Turn.SummonsSource = this.<summoner>__0;
                    if (!Turn.Iterators.IsRunning(TurnStateIteratorType.Horde))
                    {
                        this.<window>__1.layoutSummoner.Summon(this.ID);
                        Turn.SummonsSource = this.<summoner>__0;
                        this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.3f));
                        this.$PC = 2;
                        goto Label_0189;
                    }
                    Turn.Iterators.Next(TurnStateIteratorType.Horde);
                    break;

                case 2:
                    Turn.State = GameStateType.Encounter;
                    break;

                default:
                    goto Label_0187;
            }
            this.$PC = -1;
        Label_0187:
            return false;
        Label_0189:
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

