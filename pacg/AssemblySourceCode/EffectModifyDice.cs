using System;

public class EffectModifyDice : Effect
{
    public EffectModifyDice(string source, int duration, int amount) : base(source, duration)
    {
        base.genericParameters = new int[1];
        this.Amount = amount;
    }

    public EffectModifyDice(string source, int duration, int amount, CardFilter filter) : this(source, duration, amount)
    {
        base.filter = filter;
    }

    public override bool Equals(object obj)
    {
        if (obj != null)
        {
            EffectModifyDice dice = obj as EffectModifyDice;
            if (dice != null)
            {
                return (dice.Amount == this.Amount);
            }
        }
        return false;
    }

    public int GetDiceModifier(DiceType dice)
    {
        if ((base.filter != null) && !base.filter.Match(Turn.Card))
        {
            return 0;
        }
        return this.Amount;
    }

    public override string GetDisplayText() => 
        ("Dice: " + base.ConvertBonusToText(this.Amount));

    public override int GetHashCode() => 
        base.GetHashCode();

    public override bool RemoveAfterCheck() => 
        ((((base.filter != null) && base.filter.Match(Turn.Card)) && Rules.IsEncounterOver()) || base.RemoveAfterCheck());

    public override bool Stack(Effect e)
    {
        EffectModifyDice dice = e as EffectModifyDice;
        if ((dice != null) && (dice.source == base.source))
        {
            this.Amount += dice.Amount;
            return true;
        }
        return false;
    }

    private int Amount
    {
        get => 
            base.genericParameters[0];
        set
        {
            base.genericParameters[0] = value;
        }
    }

    public override bool Stacking =>
        true;

    public override EffectType Type =>
        EffectType.ModifyDice;
}

