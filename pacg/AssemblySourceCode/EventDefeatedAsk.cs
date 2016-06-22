using System;
using UnityEngine;

public class EventDefeatedAsk : Event
{
    [Tooltip("Pressing the buttons will activate the corresponding blocks.")]
    public Block[] Blocks;
    [Tooltip("Text on buttons matching the blocks.")]
    public StrRefType[] Messages;

    private void EventDefeatedAsk_Callback_0()
    {
        this.EventDefeatedAsk_Finish(0);
    }

    private void EventDefeatedAsk_Callback_1()
    {
        this.EventDefeatedAsk_Finish(1);
    }

    private void EventDefeatedAsk_Finish(int i)
    {
        this.Blocks[i].Invoke();
        Event.Done();
    }

    private TurnStateCallback GetCallback(int i)
    {
        switch (this.CallbackType)
        {
            case TurnStateCallbackType.Location:
            case TurnStateCallbackType.Global:
            case TurnStateCallbackType.Scenario:
                return new TurnStateCallback(this.CallbackType, "EventDefeatedAsk_Callback_" + i.ToString());

            case TurnStateCallbackType.Card:
                return new TurnStateCallback(base.Card, "EventDefeatedAsk_Callback_" + i.ToString());

            case TurnStateCallbackType.Character:
                return new TurnStateCallback(base.GetComponent<CharacterPower>(), "EventDefeatedAsk_Callback_" + i.ToString());
        }
        return null;
    }

    public override void OnCardDefeated(Card card)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Popup.Clear();
            for (int i = 0; i < this.Blocks.Length; i++)
            {
                TurnStateCallback callback = this.GetCallback(i);
                if (callback != null)
                {
                    window.Popup.Add(this.Messages[i].ToString(), callback);
                }
            }
            window.Popup.SetDeckPosition(DeckType.Location);
            Turn.State = GameStateType.Popup;
        }
        else
        {
            Event.Done();
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardDefeated;
}

