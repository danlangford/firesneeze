using System;
using UnityEngine;

public class CardPropertyBlocker : CardProperty
{
    [Tooltip("Does this card prevent exploring when blocking?")]
    public bool BlocksExplores;
    [Tooltip("Does this card prevent movement when blocking?")]
    public bool BlocksMovement;

    public BlockerType GetBlockerType()
    {
        if (this.BlocksExplores)
        {
            return BlockerType.Explore;
        }
        if (this.BlocksMovement)
        {
            return BlockerType.Movement;
        }
        return BlockerType.None;
    }

    public static bool IsExploreBlocked(Card card)
    {
        if (card.Blocker != BlockerType.None)
        {
            CardPropertyBlocker component = card.GetComponent<CardPropertyBlocker>();
            if (component != null)
            {
                return component.BlocksMovement;
            }
        }
        return false;
    }

    public static bool IsMovementBlocked(Card card)
    {
        if (card.Blocker != BlockerType.None)
        {
            CardPropertyBlocker component = card.GetComponent<CardPropertyBlocker>();
            if (component != null)
            {
                return component.BlocksMovement;
            }
        }
        return false;
    }
}

