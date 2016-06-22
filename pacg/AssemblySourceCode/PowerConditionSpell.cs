using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerConditionSpell : PowerCondition
{
    [Tooltip("only check myself")]
    public bool PlayedByMe;
    [Tooltip("if the casted spell ability has any of these traits return true")]
    public List<TraitType> Traits;

    private bool CharacterPlayedAbilitySpell(Character character)
    {
        for (int i = 0; i < character.Powers.Count; i++)
        {
            CharacterPowerSpell spell = character.Powers[i] as CharacterPowerSpell;
            if ((spell != null) && Turn.IsPowerActive(spell.ID))
            {
                if (this.Traits.Count == 0)
                {
                    return true;
                }
                for (int j = 0; j < spell.Traits.Length; j++)
                {
                    if (this.Traits.Contains(spell.Traits[j]))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public override bool Evaluate(Card card)
    {
        if (this.PlayedByMe)
        {
            if (!string.IsNullOrEmpty(Turn.Spell) && this.CharacterPlayedAbilitySpell(Turn.Character))
            {
                return true;
            }
            if (Turn.Character.IsCardTypeMarked(CardType.Spell))
            {
                return true;
            }
        }
        else
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (!string.IsNullOrEmpty(Turn.Spell) && this.CharacterPlayedAbilitySpell(Party.Characters[i]))
                {
                    return true;
                }
                if (Party.Characters[i].IsCardTypeMarked(CardType.Spell))
                {
                    return true;
                }
            }
        }
        return false;
    }
}

