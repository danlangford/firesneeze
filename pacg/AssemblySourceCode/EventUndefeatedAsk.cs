using System;
using UnityEngine;

public class EventUndefeatedAsk : Event
{
    [Tooltip("blocks to show in the menu; should be 2")]
    public Block[] blocks;
    [Tooltip("text on buttons matching the blocks; should be 2")]
    public StrRefType[] Messages;

    private void End()
    {
        if (Turn.State == GameStateType.Popup)
        {
            Turn.Card.Show(false);
            Turn.State = GameStateType.Dispose;
        }
        Event.Done();
    }

    private void EventUndefeatedAsk_Block_0()
    {
        Turn.Card.Disposition = DispositionType.None;
        this.blocks[0].Invoke();
        this.End();
    }

    private void EventUndefeatedAsk_Block_1()
    {
        Turn.Card.Disposition = DispositionType.None;
        this.blocks[1].Invoke();
        this.End();
    }

    public override void OnCardUndefeated(Card card)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.Card.Disposition = DispositionType.None;
            window.Popup.Clear();
            for (int i = 0; i < this.blocks.Length; i++)
            {
                TurnStateCallback callback = new TurnStateCallback(base.Card, "EventUndefeatedAsk_Block_" + i);
                window.Popup.Add(this.Messages[i].ToString(), callback);
            }
            window.Popup.SetDeckPosition(DeckType.Location);
            Turn.State = GameStateType.Popup;
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

