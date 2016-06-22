using System;
using UnityEngine;

public class CardPropertyAlternateCheck : CardProperty
{
    [Tooltip("text to display in top right")]
    public StrRefType CheckText;

    public int GetNumberOfLinesInCheck()
    {
        int num = 1;
        foreach (char ch in this.CheckText.ToString())
        {
            if (ch == '\n')
            {
                num++;
            }
        }
        return num;
    }
}

