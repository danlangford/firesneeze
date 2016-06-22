using System;
using UnityEngine;

public class PowerConditionCard : PowerCondition
{
    [Tooltip("opposing card must have this base difficulty")]
    public int Difficulty;
    [Tooltip("boolean opeator used to compare difficulty")]
    public MetaCompareOperator DifficultyOperator;
    [Tooltip("opposing card must have any of these dispositions")]
    public DispositionType[] Dispositions;
    [Tooltip("opposing card must have any of these ids")]
    public string[] IDs;
    [Tooltip("set false to change \"opposing card must have any of these types\" to \"opposing card must have any of these types or these types in their subtype\"")]
    public bool Strict = true;
    [Tooltip("opposing card must have any of these subtypes")]
    public CardType[] SubTypes;
    [Tooltip("opposing card must have any of these traits")]
    public TraitType[] Traits;
    [Tooltip("opposing card must have any of these types")]
    public CardType[] Types;
    [Tooltip("use Turn.Card for all evaluations")]
    public bool UseOpposingCard = true;

    public override bool Evaluate(Card card)
    {
        if (this.UseOpposingCard)
        {
            if ((Turn.Card != null) && (Turn.Card.Side == CardSideType.Front))
            {
                card = Turn.Card;
            }
            else
            {
                card = null;
            }
        }
        if (card == null)
        {
            return false;
        }
        if (this.Types.Length > 0)
        {
            bool flag = false;
            for (int i = 0; i < this.Types.Length; i++)
            {
                if (card.Type == this.Types[i])
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                if (this.Strict)
                {
                    return false;
                }
                for (int j = 0; j < this.Types.Length; j++)
                {
                    if (card.SubType == this.Types[j])
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }
        }
        if (this.SubTypes.Length > 0)
        {
            bool flag2 = false;
            for (int k = 0; k < this.SubTypes.Length; k++)
            {
                if (card.SubType == this.SubTypes[k])
                {
                    flag2 = true;
                    break;
                }
            }
            if (!flag2)
            {
                return false;
            }
        }
        if (this.Traits.Length > 0)
        {
            bool flag3 = false;
            for (int m = 0; m < this.Traits.Length; m++)
            {
                if ((this.Traits[m] != TraitType.None) && card.HasTrait(this.Traits[m]))
                {
                    flag3 = true;
                    break;
                }
            }
            if (!flag3)
            {
                return false;
            }
        }
        if (this.IDs.Length > 0)
        {
            bool flag4 = false;
            for (int n = 0; n < this.IDs.Length; n++)
            {
                if (this.IDs[n] == card.ID)
                {
                    flag4 = true;
                    break;
                }
            }
            if (!flag4)
            {
                return false;
            }
        }
        if (this.Dispositions.Length > 0)
        {
            bool flag5 = false;
            for (int num6 = 0; num6 < this.Dispositions.Length; num6++)
            {
                if (card.Disposition == this.Dispositions[num6])
                {
                    flag5 = true;
                    break;
                }
            }
            if (!flag5)
            {
                return false;
            }
        }
        if (this.DifficultyOperator == MetaCompareOperator.None)
        {
            return true;
        }
        int highestCheck = card.GetHighestCheck();
        if (this.DifficultyOperator == MetaCompareOperator.Equals)
        {
            return (highestCheck == this.Difficulty);
        }
        if (this.DifficultyOperator == MetaCompareOperator.Less)
        {
            return (highestCheck < this.Difficulty);
        }
        if (this.DifficultyOperator == MetaCompareOperator.LessOrEqual)
        {
            return (highestCheck <= this.Difficulty);
        }
        if (this.DifficultyOperator == MetaCompareOperator.More)
        {
            return (highestCheck > this.Difficulty);
        }
        return ((this.DifficultyOperator == MetaCompareOperator.MoreOrEqual) && (highestCheck >= this.Difficulty));
    }
}

