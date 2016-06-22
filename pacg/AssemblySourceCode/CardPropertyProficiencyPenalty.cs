using System;

public class CardPropertyProficiencyPenalty : CardProperty
{
    public int Penalty = 4;
    public bool RequiresWeaponProficiency = true;

    public int GetPenality(Character character)
    {
        if (this.RequiresWeaponProficiency && !character.ProficientWithWeapons)
        {
            return this.Penalty;
        }
        if (!this.RequiresWeaponProficiency && character.ProficientWithWeapons)
        {
            return this.Penalty;
        }
        return 0;
    }
}

