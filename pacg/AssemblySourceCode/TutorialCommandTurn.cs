using System;
using UnityEngine;

public class TutorialCommandTurn : TutorialCommand
{
    [Tooltip("ID of character whose turn it will be")]
    public string Character;

    public override void Invoke()
    {
        int index = Party.IndexOf(this.Character);
        if (index >= 0)
        {
            Turn.Current = index;
            Turn.Number = index;
        }
    }
}

