using System;
using UnityEngine;

public class BaseCharacterPowerMod : CharacterPower
{
    [Tooltip("which family of powers is modified?")]
    public string Family;

    public override bool Modifies(string id) => 
        id.StartsWith(this.Family);
}

