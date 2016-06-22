using System;
using UnityEngine;

public abstract class EventCallbackMovePlayer : Event
{
    [Tooltip("which locations are valid?")]
    public LocationSelector LocationSelector;

    protected EventCallbackMovePlayer()
    {
    }

    public override bool IsEventValid(Card card) => 
        base.IsConditionValid(card);

    public abstract override void OnCallback();

    public override EventType Type =>
        EventType.OnCallback;
}

