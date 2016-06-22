using System;
using UnityEngine;

public abstract class PowerCondition : MonoBehaviour
{
    [Tooltip("only used in the toolset to help connect conditions to powers")]
    public string Label;

    protected PowerCondition()
    {
    }

    public virtual bool Evaluate(Card card) => 
        true;

    public static bool Evaluate(Card card, PowerConditionType[] Conditions)
    {
        if ((Conditions == null) || (Conditions.Length == 0))
        {
            return true;
        }
        bool flag = Conditions[0].Operator == MetaBoolOperator.And;
        for (int i = 0; i < Conditions.Length; i++)
        {
            if (Conditions[i].Condition != null)
            {
                bool flag2 = Conditions[i].Condition.Evaluate(card);
                if (Conditions[i].Not)
                {
                    flag2 = !flag2;
                }
                if (Conditions[i].Operator == MetaBoolOperator.And)
                {
                    flag = flag && flag2;
                }
                if (Conditions[i].Operator == MetaBoolOperator.Or)
                {
                    flag = flag || flag2;
                }
            }
        }
        return flag;
    }
}

