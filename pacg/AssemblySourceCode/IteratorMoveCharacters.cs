using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class IteratorMoveCharacters : TurnStateIterator
{
    public override void End()
    {
        base.End();
        Location.Load(base.InitialLocation);
        Turn.DamageTargetType = DamageTargetType.Player;
        if (Event.Finished())
        {
            if (Turn.PeekStateDestination() != null)
            {
                Turn.GotoStateDestination();
            }
            else if (Turn.ReturnState != GameStateType.None)
            {
                Turn.ReturnToReturnState();
            }
            else
            {
                Turn.State = GameStateType.Dispose;
            }
        }
        Event.Done();
    }

    public override void Invoke()
    {
        Turn.Current = Turn.Number;
        Turn.Target = Turn.Current;
        Location.Load(Turn.Owner.Location);
        Turn.PushReturnState(GameStateType.Dispose);
        Turn.PushCancelDestination(GameStateType.Dispose);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((window != null) && window.mapPanel.IsMoveRestrictionRequired())
        {
            window.mapPanel.Center(Turn.BlackBoard.Get<string>(Turn.Character.ID + "_destination"));
            Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "MapMove_FinishMove"));
            Turn.State = GameStateType.Move;
        }
        else
        {
            Turn.PopReturnState();
            Turn.State = GameStateType.Null;
            Game.Instance.StartCoroutine(this.MoveSingle_Animation(Turn.Character));
        }
    }

    public override bool IsValid()
    {
        if ((Turn.DamageTargetType == DamageTargetType.Location) && (Location.CountCharactersAtLocation(Location.Current.ID) < 1))
        {
            return false;
        }
        if (Party.CountLivingMembers() <= 0)
        {
            return false;
        }
        return base.IsValid();
    }

    [DebuggerHidden]
    private IEnumerator MoveSingle_Animation(Character character) => 
        new <MoveSingle_Animation>c__Iterator93 { 
            character = character,
            <$>character = character
        };

    public override bool Next()
    {
        if (Turn.DamageTargetType == DamageTargetType.Location)
        {
            return base.NextCharacterAtLocation(base.InitialLocation);
        }
        if (Turn.DamageTargetType == DamageTargetType.Party)
        {
            return base.NextCharacterInParty();
        }
        return ((Turn.DamageTargetType == DamageTargetType.Selected) && base.NextSelectedCharacterInParty());
    }

    public override TurnStateIteratorType Type =>
        TurnStateIteratorType.Move;

    [CompilerGenerated]
    private sealed class <MoveSingle_Animation>c__Iterator93 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Character <$>character;
        internal Event[] <$s_162>__5;
        internal int <$s_163>__6;
        internal bool <enterLocationPowerActivated>__0;
        internal Event <ev>__7;
        internal Event[] <events>__4;
        internal int <i>__3;
        internal List<ScenarioPower> <powers>__2;
        internal GuiWindowLocation <window>__1;
        internal Character character;

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
                    this.<enterLocationPowerActivated>__0 = false;
                    this.<window>__1 = UI.Window as GuiWindowLocation;
                    if ((this.<window>__1 == null) || (this.character == null))
                    {
                        goto Label_01EF;
                    }
                    Turn.Card.Show(false);
                    this.<window>__1.layoutLocation.GlowText(false);
                    this.<window>__1.ShowMap(true);
                    Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "Iterator_Next"));
                    this.$current = Game.Instance.StartCoroutine(this.<window>__1.mapPanel.MoveCharacter(this.character, Turn.BlackBoard.Get<string>(this.character.ID + "_destination")));
                    this.$PC = 1;
                    return true;

                case 1:
                    Location.Load(this.character.Location);
                    this.<powers>__2 = Scenario.Current.Powers;
                    this.<i>__3 = 0;
                    goto Label_01C9;

                default:
                    goto Label_01F6;
            }
        Label_01BB:
            this.<i>__3++;
        Label_01C9:
            if (this.<i>__3 < this.<powers>__2.Count)
            {
                this.<events>__4 = this.<powers>__2[this.<i>__3].GetComponents<Event>();
                this.<$s_162>__5 = this.<events>__4;
                this.<$s_163>__6 = 0;
                while (this.<$s_163>__6 < this.<$s_162>__5.Length)
                {
                    this.<ev>__7 = this.<$s_162>__5[this.<$s_163>__6];
                    if (this.<ev>__7.IsEventImplemented(EventType.OnLocationChange) && this.<ev>__7.IsEventValid(Turn.Card))
                    {
                        this.<enterLocationPowerActivated>__0 = true;
                        this.<i>__3 = this.<powers>__2.Count;
                        break;
                    }
                    this.<$s_163>__6++;
                }
                goto Label_01BB;
            }
            if (!this.<enterLocationPowerActivated>__0)
            {
                Turn.GotoStateDestination();
            }
        Label_01EF:
            this.$PC = -1;
        Label_01F6:
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

