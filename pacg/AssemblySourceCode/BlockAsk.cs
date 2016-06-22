using System;
using UnityEngine;

public class BlockAsk : Block
{
    [Tooltip("blocks to show in the menu; should be 2")]
    public Block[] blocks;
    [Tooltip("if true, before invoking each block set up the cancel destination")]
    public bool Cancellable;
    [Tooltip("text on buttons matching the blocks; should be 2")]
    public StrRefType[] Messages;
    [Tooltip("after the blocks are processed go to damage state?")]
    public bool PostBlockDamage = true;

    private void BlockAsk_Block_0()
    {
        this.SetCancelDestination();
        this.blocks[0].Invoke();
        this.End();
    }

    private void BlockAsk_Block_1()
    {
        this.SetCancelDestination();
        this.blocks[1].Invoke();
        this.End();
    }

    private void BlockAsk_StartPopup()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Popup.Clear();
            for (int i = 0; i < this.blocks.Length; i++)
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
    }

    private void End()
    {
        if (this.PostBlockDamage)
        {
            Turn.Card.Show(false);
            Turn.State = GameStateType.Damage;
        }
        if (this.Cancellable)
        {
            UI.Window.Refresh();
        }
    }

    private TurnStateCallback GetCallback(int i)
    {
        Card component = base.GetComponent<Card>();
        if (component != null)
        {
            return new TurnStateCallback(component, "BlockAsk_Block_" + i);
        }
        if (base.GetComponent<Location>() != null)
        {
            return new TurnStateCallback(TurnStateCallbackType.Location, "BlockAsk_Block_" + i);
        }
        return null;
    }

    public override void Invoke()
    {
        this.BlockAsk_StartPopup();
    }

    private void SetCancelDestination()
    {
        if (this.Cancellable)
        {
            if (base.Card != null)
            {
                Turn.PushCancelDestination(new TurnStateCallback(base.Card, "BlockAsk_StartPopup"));
            }
            else
            {
                Turn.PushCancelDestination(new TurnStateCallback(base.GetCallbackType(), "BlockAsk_StartPopup"));
            }
        }
    }

    public override bool Stateless =>
        false;
}

