using System;
using UnityEngine;

public class BlockRollBlock : Block
{
    [Tooltip("invoked when the roll was not hit")]
    public Block BlockNo;
    [Tooltip("invoked when the roll was hit")]
    public Block BlockYes;
    [Tooltip("go to this destination if not None")]
    public GameStateType Destination;
    [Tooltip("dice used to compute the damage")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("display during roll state a message")]
    public StrRefType Message;
    [Tooltip("the number(s) to roll")]
    public int[] NumberToRoll = new int[] { 1 };
    [Tooltip("what color should the dice really be?")]
    public RollType RollReason = RollType.StandardEnemyDice;

    private void BlockRollBlock_Resolve()
    {
        Turn.RollReason = RollType.PlayerControlled;
        bool flag = false;
        for (int i = 0; i < this.NumberToRoll.Length; i++)
        {
            if (this.NumberToRoll[i] == Turn.DiceTotal)
            {
                flag = true;
                break;
            }
        }
        if (flag)
        {
            if (this.BlockYes != null)
            {
                this.BlockYes.Invoke();
            }
            else if (this.Destination == GameStateType.None)
            {
                Turn.ReturnToReturnState();
            }
        }
        else if (this.BlockNo != null)
        {
            this.BlockNo.Invoke();
        }
        else if (this.Destination == GameStateType.None)
        {
            Turn.ReturnToReturnState();
        }
        if (this.Destination != GameStateType.None)
        {
            Turn.State = this.Destination;
        }
    }

    public override void Invoke()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.RollReason = this.RollReason;
        Turn.Checks = null;
        base.RefreshDicePanel();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessRechargableCards();
            window.ProcessLayoutDecks();
        }
        if (!this.Message.IsNullOrEmpty())
        {
            Turn.SetStateData(new TurnStateData(this.Message.ToString()));
        }
        if (base.GetCallbackType() == TurnStateCallbackType.Card)
        {
            Turn.PushStateDestination(new TurnStateCallback(base.Card, "BlockRollBlock_Resolve"));
        }
        else
        {
            Turn.PushStateDestination(new TurnStateCallback(base.GetCallbackType(), "BlockRollBlock_Resolve"));
        }
        Turn.State = GameStateType.Roll;
    }

    public override bool Stateless =>
        false;
}

