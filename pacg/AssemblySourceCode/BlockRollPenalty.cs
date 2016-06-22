using System;
using UnityEngine;

public class BlockRollPenalty : Block
{
    [Tooltip("number of penalty cards")]
    public int Amount = 1;
    [Tooltip("destination after penalty ahs been paid")]
    public GameStateType Destination = GameStateType.Dispose;
    [Tooltip("dice used to compute the penalty amount")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the penalty (total not per dice)")]
    public int DiceBonus;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("type of penalty to be performed by the player")]
    public ActionType Penalty = ActionType.Discard;

    private void BlockRollPenalty_Done()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Turn.RollReason = RollType.PlayerControlled;
            Turn.EmptyLayoutDecks = true;
            window.ProcessLayoutDecks();
        }
        Turn.PushStateDestination(this.Destination);
        Turn.State = GameStateType.Recharge;
    }

    private void BlockRollPenalty_Penalty()
    {
        this.Amount = Turn.DiceTotal;
        this.ClearDicePanel();
        Turn.SetStateData(new TurnStateData(this.Penalty, this.Amount));
        Turn.PushStateDestination(new TurnStateCallback(Turn.Card, "BlockRollPenalty_Done"));
        Turn.State = GameStateType.Penalty;
    }

    private void ClearDicePanel()
    {
        Turn.ClearCheckData();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Clear();
        }
    }

    public override void Invoke()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus += this.DiceBonus;
        Turn.RollReason = RollType.EnemyDamage;
        base.RefreshDicePanel();
        Turn.Checks = null;
        Turn.Check = SkillCheckType.None;
        Turn.EmptyLayoutDecks = false;
        Turn.PushStateDestination(new TurnStateCallback(Turn.Card, "BlockRollPenalty_Penalty"));
        Turn.State = GameStateType.Roll;
    }

    public override bool Stateless =>
        false;
}

