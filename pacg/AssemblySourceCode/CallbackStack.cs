using System;
using System.Collections.Generic;
using UnityEngine;

public class CallbackStack
{
    private static readonly int DEFAULT_STACK_SIZE = 7;
    [Tooltip("number of entries before this stack will stop growing (fail safe in case of infinite loops)")]
    public static readonly int MAX_STACK_SIZE = 0x20;
    private List<TurnStateCallback> stack;

    public CallbackStack()
    {
        this.stack = new List<TurnStateCallback>(DEFAULT_STACK_SIZE);
    }

    public CallbackStack(int stackSize)
    {
        this.stack = new List<TurnStateCallback>(stackSize);
    }

    public void Clear()
    {
        this.stack.Clear();
    }

    public static CallbackStack FromStream(ByteStream bs)
    {
        if (!bs.ReadBool())
        {
            return null;
        }
        bs.ReadInt();
        int num = bs.ReadInt();
        CallbackStack stack = new CallbackStack();
        for (int i = 0; i < num; i++)
        {
            TurnStateCallback callback = TurnStateCallback.FromStream(bs);
            stack.Push(callback);
        }
        return stack;
    }

    public TurnStateCallback Peek()
    {
        if (this.stack.Count > 0)
        {
            return this.stack[this.stack.Count - 1];
        }
        return null;
    }

    public TurnStateCallback Pop()
    {
        if (this.stack.Count > 0)
        {
            TurnStateCallback callback = this.stack[this.stack.Count - 1];
            this.stack.RemoveAt(this.stack.Count - 1);
            return callback;
        }
        return null;
    }

    public void Push(TurnStateCallback callback)
    {
        if ((callback != null) && (this.stack.Count <= MAX_STACK_SIZE))
        {
            this.stack.Add(callback);
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
}

