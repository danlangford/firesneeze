using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventEncounteredReplace : Event
{
    [Tooltip("the possible cards that will replace this card")]
    public string[] IDs;
    [Tooltip("the strings to display on the dice roll animation")]
    public StrRefType[] PowerStrings;

    [DebuggerHidden]
    private IEnumerator EventEncounteredReplace_CleanUp() => 
        new <EventEncounteredReplace_CleanUp>c__Iterator25 { <>f__this = this };

    private void EventEncounteredReplace_Finish()
    {
        if (((Turn.DiceTotal - 1) >= 0) && ((Turn.DiceTotal - 1) < this.IDs.Length))
        {
            Card card = CardTable.Create(this.IDs[Turn.DiceTotal - 1]);
            card.GUID = base.Card.GUID;
            base.Card.Deck.Add(card, DeckPositionType.Top);
            card.transform.localScale = base.Card.transform.localScale;
            card.transform.position = base.Card.transform.position;
            card.Show(true);
            if (Rules.IsCardSummons(base.Card))
            {
                card.Decorations.Add("Blueprints/Gui/Vfx_Card_Notice_Summoned", CardSideType.Front, null, 0f);
            }
        }
        base.Card.Deck.Remove(base.Card);
        base.Card.Show(true);
        VisualEffect.ApplyToCard(VisualEffectType.CardBanishToBox, base.Card, 2f);
        Game.Instance.StartCoroutine(this.EventEncounteredReplace_CleanUp());
    }

    public override void OnCardEncountered(Card card)
    {
        Turn.RollReason = RollType.EnemyRandomPower;
        Turn.Dice.Add((DiceType) this.IDs.Length);
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventEncounteredReplace_Finish"));
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Refresh();
            window.dicePanel.SetDiceText(this.PowerStrings);
        }
        Turn.State = GameStateType.Roll;
    }

    public override EventType Type =>
        EventType.OnCardEncountered;

    [CompilerGenerated]
    private sealed class <EventEncounteredReplace_CleanUp>c__Iterator25 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal EventEncounteredReplace <>f__this;

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
                    Campaign.Box.Add(this.<>f__this.Card, false);
                    this.$current = new WaitForSeconds(2f);
                    this.$PC = 1;
                    return true;

                case 1:
                    GameStateEncounter.Continue();
                    Event.Done();
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

