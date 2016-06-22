using System;

public class TurnStateIteratorFactory
{
    public static TurnStateIterator Create(TurnStateIteratorType type)
    {
        if (type == TurnStateIteratorType.Aid)
        {
            return new IteratorAid();
        }
        if (type == TurnStateIteratorType.Check)
        {
            return new IteratorCheck();
        }
        if (type == TurnStateIteratorType.Close)
        {
            return new IteratorClose();
        }
        if (type == TurnStateIteratorType.Damage)
        {
            return new IteratorDamage();
        }
        if (type == TurnStateIteratorType.DamageRoll)
        {
            return new IteratorDamageRoll();
        }
        if (type == TurnStateIteratorType.Defeat)
        {
            return new IteratorDefeat();
        }
        if (type == TurnStateIteratorType.Encounter)
        {
            return new IteratorEncounter();
        }
        if (type == TurnStateIteratorType.Horde)
        {
            return new IteratorHorde();
        }
        if (type == TurnStateIteratorType.Recharge)
        {
            return new IteratorRecharge();
        }
        if (type == TurnStateIteratorType.Move)
        {
            return new IteratorMoveCharacters();
        }
        if (type == TurnStateIteratorType.HordeThenEncounter)
        {
            return new IteratorHordeThenEncounter();
        }
        if (type == TurnStateIteratorType.Restrict)
        {
            return new IteratorRestrict();
        }
        return null;
    }
}

