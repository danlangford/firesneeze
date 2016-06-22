using System;
using UnityEngine;

public class BlockMovePlayer : Block
{
    [Tooltip("the move event")]
    public EventCallbackMovePlayer Callback;
    [Tooltip("which locations are valid?")]
    public LocationSelector LocationSelector;

    public override void Invoke()
    {
        if ((this.Callback != null) && (this.LocationSelector.Random(Turn.Character) != null))
        {
            Event[] components = base.Card.GetComponents<Event>();
            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].Type == this.Callback.Type)
                    {
                        EventCallback callback = new EventCallback {
                            CallbackType = EventCallbackType.Card,
                            CallbackID = base.Card.ID,
                            CallbackEvent = EventType.OnCallback,
                            CallbackPosition = i,
                            Stateless = this.Callback.Stateless,
                            CallbackCardId = null
                        };
                        Game.Events.Top(callback);
                        break;
                    }
                }
            }
        }
    }

    public override bool Stateless =>
        false;
}

