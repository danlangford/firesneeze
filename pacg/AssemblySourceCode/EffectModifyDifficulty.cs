using System;

public class EffectModifyDifficulty : Effect
{
    public EffectModifyDifficulty(string source, int duration, int amount, SkillCheckType skillCheck, CardFilter filter, int checkSequence, bool combatOnly) : base(source, duration, filter)
    {
        base.genericParameters = new int[4];
        this.Amount = amount;
        this.SkillCheck = skillCheck;
        this.CheckSequence = checkSequence;
        this.CombatOnly = combatOnly;
    }

    public override bool Equals(object obj)
    {
        EffectModifyDifficulty difficulty = obj as EffectModifyDifficulty;
        if (difficulty == null)
        {
            return false;
        }
        if (base.duration != difficulty.duration)
        {
            return false;
        }
        if (this.Amount != difficulty.Amount)
        {
            return false;
        }
        if (this.SkillCheck != difficulty.SkillCheck)
        {
            return false;
        }
        if (!base.filter.Equals(difficulty.filter))
        {
            return false;
        }
        if (this.CheckSequence != difficulty.CheckSequence)
        {
            return false;
        }
        if (this.CombatOnly != difficulty.CombatOnly)
        {
            return false;
        }
        return true;
    }

    public int GetCheckModifier(Card card)
    {
        if ((base.filter != null) && !base.filter.Match(card))
        {
            return 0;
        }
        if ((this.SkillCheck != SkillCheckType.None) && (Turn.Check != this.SkillCheck))
        {
            return 0;
        }
        if ((this.CheckSequence > 0) && (this.CheckSequence != Turn.CombatCheckSequence))
        {
            return 0;
        }
        if (this.CombatOnly && !Rules.IsCheckToDefeat())
        {
            return 0;
        }
        return this.Amount;
    }

    public override string GetDisplayText() => 
        ("Modify Difficulty: " + base.ConvertBonusToText(this.Amount));

    public override int GetHashCode() => 
        (base.GetHashCode() ^ base.filter.GetHashCode());

    private bool MatchesFilter()
    {
        if (base.filter != null)
        {
            return base.filter.Match(Turn.Card);
        }
        return true;
    }

    public override bool RemoveAfterCheck() => 
        (this.MatchesFilter() && base.RemoveAfterCheck());

    public override bool RemoveAfterEncounter() => 
        (this.MatchesFilter() && base.RemoveAfterEncounter());

    public override bool Stack(Effect e)
    {
        EffectModifyDifficulty difficulty = e as EffectModifyDifficulty;
        if ((((difficulty != null) && (difficulty.duration == base.duration)) && (difficulty.filter.Equals(base.filter) && (difficulty.SkillCheck == this.SkillCheck))) && ((difficulty.CheckSequence == this.CheckSequence) && (difficulty.CombatOnly == this.CombatOnly)))
        {
            this.Amount += difficulty.Amount;
            base.sources.AddRange(difficulty.sources);
            return true;
        }
        return false;
    }

    public int Amount
    {
        get => 
            base.genericParameters[0];
        set
        {
            base.genericParameters[0] = value;
        }
    }

    public int CheckSequence
    {
        get => 
            base.genericParameters[2];
        set
        {
            base.genericParameters[2] = value;
        }
    }

    public bool CombatOnly
    {
        get => 
            (base.genericParameters[3] != 0);
        set
        {
            if (value)
            {
                base.genericParameters[3] = 1;
            }
            else
            {
                base.genericParameters[3] = 0;
            }
        }
    }

    public SkillCheckType SkillCheck
    {
        get => 
            ((SkillCheckType) base.genericParameters[1]);
        set
        {
            base.genericParameters[1] = (int) value;
        }
    }

    public override bool Stacking
    {
        get
        {
            if ((base.source == null) || ((!base.source.StartsWith("HE") && !base.source.StartsWith("MO")) && (!base.source.StartsWith("BX") && !base.source.StartsWith("VL"))))
            {
                return false;
            }
            return true;
        }
    }

    public override EffectType Type =>
        EffectType.ModifyDifficulty;
}

