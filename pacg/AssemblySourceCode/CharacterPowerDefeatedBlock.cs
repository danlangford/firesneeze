using System;
using UnityEngine;

public class CharacterPowerDefeatedBlock : CharacterPower
{
    [Tooltip("block to activate")]
    public Block Block;
    [Tooltip("do not activate the block")]
    public StrRefType NoActivate;
    [Tooltip("the block may activate depending on the player's choice")]
    public bool Optional = true;
    [Tooltip("activate the block")]
    public StrRefType YesActivate;

    public override void Activate()
    {
        if (this.Block != null)
        {
            Turn.Number = Party.IndexOf(base.Character.ID);
            this.Block.Invoke();
        }
        base.Activate();
    }

    public override bool IsValid() => 
        base.IsConditionValid(Turn.Card);

    public override void OnCardDefeated(Card card)
    {
        if (this.IsValid())
        {
            if (this.Optional)
            {
                Event[] components = base.GetComponents<Event>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].IsEventImplemented(EventType.OnCardDefeated))
                    {
                        Game.Events.Add(EventCallbackType.CharacterPower, base.ID, EventType.OnCardDefeated, i, false);
                        break;
                    }
                }
            }
            else
            {
                this.Activate();
            }
        }
    }
}

