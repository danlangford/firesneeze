using System;
using UnityEngine;

public class CardPowerMirrorImage : CardPower
{
    [Tooltip("the type of dice to roll")]
    public DiceType[] Dice;
    [Tooltip("the duration of this effect")]
    public int Duration = 1;
    [Tooltip("mirror image is discarded if the recharge check fails/not attempted")]
    public ActionType FinalDestination = ActionType.Discard;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("rolling this number or higher will avoid the damage")]
    public int TargetNumber = 2;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.layoutDiscard.ReturnCards(false);
            }
            base.LockInDisplayed(true);
            EffectMirrorImage e = new EffectMirrorImage(card.ID, this.Duration);
            Turn.Character.ApplyEffect(e);
            if (e.IsInvokePossible())
            {
                Turn.BlackBoard.Set<int>("CardPowerMirrorImage_Played", Turn.BlackBoard.Get<int>("CardPowerMirrorImage_Played") + 1);
                e.Invoke();
            }
        }
    }

    private void CardPowerMirrorImage_Resolve()
    {
        Turn.BlackBoard.Set<int>("EffectMirrorImage_Rolled", Turn.BlackBoard.Get<int>("EffectMirrorImage_Rolled") + 1);
        Turn.DequeueData();
        if (Turn.DiceTotal >= this.TargetNumber)
        {
            Turn.Damage = 0;
        }
        Turn.Dice.Clear();
        Turn.ReturnToReturnState();
    }

    private void CardPowerMirrorImage_Roll()
    {
        Turn.Dice.Clear();
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus = 0;
        Turn.DiceTarget = this.TargetNumber;
        this.RefreshDicePanel();
        Turn.Checks = null;
        Turn.EnqueueDamageData();
        Turn.PushReturnState();
        Turn.PushCancelDestination(Turn.State);
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "CardPowerMirrorImage_Resolve"));
        Turn.State = GameStateType.Roll;
    }

    public override string GetCardDecoration(Card card)
    {
        if (((Turn.State == GameStateType.Damage) || (Turn.State == GameStateType.Ambush)) && !this.IsPowerAllowed(Turn.Card))
        {
            return "Blueprints/Gui/Vfx_Card_Notice_NotAllowed";
        }
        return null;
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
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Rules.GetCardsToDiscardCount() <= 0)
        {
            return false;
        }
        if (!Turn.DamageFromEnemy)
        {
            return false;
        }
        if (!Rules.IsDamageReductionPossible())
        {
            return false;
        }
        return ((Turn.State == GameStateType.Damage) || (Turn.State == GameStateType.Ambush));
    }

    private void RefreshDicePanel()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Refresh();
        }
    }

    public override ActionType RechargeAction =>
        this.FinalDestination;
}

