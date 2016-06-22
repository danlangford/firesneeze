using System;
using UnityEngine;

public class CardPowerCheck : CardPower
{
    [Tooltip("First checks to attempt")]
    public SkillCheckValueType[] Checks1;
    [Tooltip("Block to run if you fail check")]
    public Block FailBlock;
    [Tooltip("Block to run if you succeed check")]
    public Block SuccessBlock;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            Turn.Owner.MarkCardType(card.Type, true);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.GetLayoutDeck(base.Action).Deck.Remove(card);
                window.CancelAllPowers(true, true);
            }
            Turn.EmptyLayoutDecks = false;
            this.SetUpDicePanel();
            Turn.PushReturnState();
            Turn.PushCancelDestination(new TurnStateCallback(card, "CardPowerCheck_Cancel"));
            Turn.PushStateDestination(new TurnStateCallback(card, "CardPowerCheck_Finish"));
            Turn.FocusedCard = base.Card;
            card.Show(true);
            Turn.State = GameStateType.Roll;
            if (window != null)
            {
                window.ShowCancelButton(true);
                window.layoutLocation.ShowPreludeFX(false);
            }
        }
    }

    private void CardPowerCheck_Cancel()
    {
        int index = Party.IndexOf(base.Card.PlayedOwner);
        if ((index >= 0) && (index < Party.Characters.Count))
        {
            Character character = Party.Characters[index];
            character.MarkCardType(base.Card.Type, false);
            character.Hand.Add(base.Card);
        }
        Turn.FocusedCard = null;
        Turn.ReturnToReturnState();
        UI.Window.Refresh();
    }

    private void CardPowerCheck_Finish()
    {
        Turn.EmptyLayoutDecks = true;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.GetLayoutDeck(base.Action).Deck.Add(base.Card);
        }
        Turn.FocusedCard = null;
        if (Turn.IsResolveSuccess())
        {
            if (this.SuccessBlock != null)
            {
                this.SuccessBlock.Invoke();
            }
        }
        else if (this.FailBlock != null)
        {
            this.FailBlock.Invoke();
        }
        if (Turn.State == GameStateType.Roll)
        {
            Turn.ReturnToReturnState();
        }
        else
        {
            Turn.PopReturnState();
        }
        UI.Window.Refresh();
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (Turn.Character.IsCardTypeMarked(card.Type))
        {
            return false;
        }
        if ((this.Type == PowerType.Evade) && !Rules.IsEvadePossible(card))
        {
            return false;
        }
        return true;
    }

    public override bool IsPowerDeactivationAllowed(Card card) => 
        false;

    private void SetUpDicePanel()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        Turn.Check = Turn.Owner.GetBestSkillCheck(this.Checks1).skill;
        if (window != null)
        {
            window.dicePanel.SetCheck(base.Card, this.Checks1, Turn.Check);
        }
    }

    public override PowerType Type
    {
        get
        {
            if (this.SuccessBlock is BlockEvade)
            {
                return PowerType.Evade;
            }
            if (this.FailBlock is BlockEvade)
            {
                return PowerType.Evade;
            }
            return base.Type;
        }
    }
}

