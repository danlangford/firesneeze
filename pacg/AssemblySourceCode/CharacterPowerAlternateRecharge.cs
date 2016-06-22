using System;
using UnityEngine;

public class CharacterPowerAlternateRecharge : CharacterPower
{
    [Tooltip("additional recharge options to be available")]
    public ActionType[] AdditionalOptions;
    [Tooltip("Selector determines which cards these options are available")]
    public CardSelector Selector;

    public override bool IsValid()
    {
        if (!this.Selector.Match(Turn.Card))
        {
            return false;
        }
        if (!Rules.IsRechargeCheck())
        {
            return false;
        }
        return true;
    }
}

