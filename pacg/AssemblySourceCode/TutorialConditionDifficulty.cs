using System;
using UnityEngine;

public class TutorialConditionDifficulty : TutorialCondition
{
    [Tooltip("scenario must have this difficulty")]
    public int Difficulty;
    [Tooltip("boolean opeator used to compare difficulty")]
    public MetaCompareOperator Operator = MetaCompareOperator.Equals;

    public override bool Evaluate()
    {
        if (Scenario.Current == null)
        {
            return false;
        }
        return this.Operator.Evaluate(Scenario.Current.Difficulty, this.Difficulty);
    }
}

