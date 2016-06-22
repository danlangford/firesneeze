using System;
using System.Collections.Generic;

public class TurnStateIteratorStack
{
    private static readonly int DEFAULT_STACK_SIZE = 7;
    private List<TurnStateIterator> stack = new List<TurnStateIterator>(DEFAULT_STACK_SIZE);

    public void Clear()
    {
        this.stack.Clear();
    }

    public static TurnStateIteratorStack FromStream(ByteStream bs)
    {
        if (!bs.ReadBool())
        {
            return null;
        }
        bs.ReadInt();
        int num = bs.ReadInt();
        TurnStateIteratorStack stack = new TurnStateIteratorStack();
        for (int i = 0; i < num; i++)
        {
            TurnStateIterator item = TurnStateIterator.FromStream(bs);
            stack.stack.Add(item);
        }
        return stack;
    }

    public void Invoke()
    {
        if (this.stack.Count > 0)
        {
            this.stack[this.stack.Count - 1].Invoke();
        }
    }

    private bool IsConfirmationNeeded()
    {
        if (this.stack.Count <= 0)
        {
            return false;
        }
        if (!this.stack[this.stack.Count - 1].HasNext())
        {
            return false;
        }
        return !Turn.BlackBoard.Get<bool>("ConfirmContinue");
    }

    public bool IsRunning(TurnStateIteratorType type)
    {
        for (int i = 0; i < this.stack.Count; i++)
        {
            if (this.stack[i].IsType(type))
            {
                return true;
            }
        }
        return false;
    }

    public void Next()
    {
        if (this.stack.Count > 0)
        {
            if (this.IsConfirmationNeeded())
            {
                Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "Iterator_Next"));
                Turn.SetStateData(new TurnStateData(StringTableManager.GetUIText(0x1bf)));
                Turn.State = GameStateType.ConfirmPowerUse;
            }
            else
            {
                TurnStateIterator iterator = this.stack[this.stack.Count - 1];
                if (iterator.Next())
                {
                    Turn.State = GameStateType.Switch;
                }
                else
                {
                    this.stack.RemoveAt(this.stack.Count - 1);
                    iterator.End();
                }
                this.SetConfirmationNeeded(true);
            }
        }
    }

    public void Next(TurnStateIteratorType type)
    {
        this.Next();
    }

    public void Remove(TurnStateIteratorType type)
    {
        for (int i = 0; i < this.stack.Count; i++)
        {
            if (this.stack[i].IsType(type))
            {
                this.stack.RemoveAt(i);
                return;
            }
        }
    }

    private void SetConfirmationNeeded(bool needed)
    {
        Turn.BlackBoard.Set<bool>("ConfirmContinue", !needed);
    }

    public void Start(TurnStateIteratorType type)
    {
        this.Start(type, TurnStateCallbackType.Card);
    }

    public void Start(TurnStateIteratorType type, TurnStateCallbackType callback)
    {
        TurnStateIterator item = TurnStateIteratorFactory.Create(type);
        item.CallBackType = callback;
        if ((item != null) && item.IsValid())
        {
            this.stack.Add(item);
            item.Start();
        }
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteBool(true);
        bs.WriteInt(1);
        bs.WriteInt(this.stack.Count);
        for (int i = 0; i < this.stack.Count; i++)
        {
            this.stack[i].ToStream(bs);
        }
    }

    public int Count =>
        this.stack.Count;

    public TurnStateIterator Current
    {
        get
        {
            if (this.stack.Count > 0)
            {
                return this.stack[this.stack.Count - 1];
            }
            return null;
        }
    }
}

