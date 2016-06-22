using System;
using UnityEngine;

public class CardPowerGiveCard : CardPower
{
    [Tooltip("who can receive the card?")]
    public TargetType Target = TargetType.Another;

    public override void Activate(Card card)
    {
        Turn.Character.MarkCardType(card.Type, true);
        Turn.PushReturnState();
        Turn.PushStateDestination(Turn.ReturnState);
        Turn.PushCancelDestination(Turn.PopReturnState());
        Turn.TargetType = this.Target;
        Turn.SetStateData(new TurnStateData(ActionType.Give, 1));
        Turn.State = GameStateType.Give;
    }

    public override void Deactivate(Card card)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.Character.MarkCardType(card.Type, false);
            Turn.TargetType = TargetType.None;
            Turn.ReturnToReturnState();
        }
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (!base.IsAidPossible(card))
        {
            return false;
        }
        if (Turn.Character.IsCardTypeMarked(card.Type))
        {
            return false;
        }
        if (Party.CountLivingMembers() <= 1)
        {
            return false;
        }
        if ((Turn.State != GameStateType.Setup) && (Turn.State != GameStateType.Finish))
        {
            return false;
        }
        return true;
    }

    public override bool IsValidationRequired() => 
        false;
}

