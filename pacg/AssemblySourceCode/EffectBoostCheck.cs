using System;

public class EffectBoostCheck : Effect
{
    public EffectBoostCheck(string source, int duration, CardFilter filter, DiceType diceType, TraitType traitType, SkillCheckType skill, int diceBonus, int diceCount) : base(source, duration, filter)
    {
        base.genericParameters = new int[5];
        this.DiceType = diceType;
        this.TraitType = traitType;
        this.Skill = skill;
        this.DiceBonus = diceBonus;
        this.DiceCount = diceCount;
    }

    public override bool Equals(object obj)
    {
        EffectBoostCheck check = obj as EffectBoostCheck;
        if (check == null)
        {
            return false;
        }
        if (!base.Equals(check))
        {
            return false;
        }
        if (this.DiceType != check.DiceType)
        {
            return false;
        }
        if (this.TraitType != check.TraitType)
        {
            return false;
        }
        if (base.source != check.source)
        {
            return false;
        }
        return true;
    }

    public override string GetDisplayText()
    {
        string name = this.DiceType.ToText() + ((this.DiceBonus == 0) ? string.Empty : (" + " + this.DiceBonus)) + ((this.TraitType == TraitType.None) ? string.Empty : (" and " + this.TraitType.ToText() + " added to check"));
        if (GuiPanelEffects.IsCharacterEffect(this))
        {
            char[] separator = new char[] { '/' };
            string[] strArray = base.source.Split(separator);
            if (strArray.Length >= 2)
            {
                PowerTableEntry entry = PowerTable.Get(strArray[1]);
                if (entry != null)
                {
                    name = entry.Name;
                }
            }
            return name;
        }
        CardTableEntry entry2 = CardTable.Get(base.source);
        if (entry2 != null)
        {
            name = entry2.Name;
        }
        return name;
    }

    public override int GetHashCode() => 
        (base.GetHashCode() ^ base.filter.GetHashCode());

    public int GetSkillModifier(params SkillCheckType[] skills)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if ((skills[i] == this.Skill) || (this.Skill == SkillCheckType.None))
            {
                return this.DiceBonus;
            }
        }
        return 0;
    }

    public bool Match(Card card, SkillCheckType skill)
    {
        if ((base.filter != null) && !base.filter.Match(card))
        {
            return false;
        }
        if ((this.Skill != skill) && (this.Skill != SkillCheckType.None))
        {
            return false;
        }
        if (Rules.IsImmune(card, this.TraitType))
        {
            return false;
        }
        return true;
    }

    public override void OnEffectFinished()
    {
        base.OnEffectFinished();
        Turn.DamageTraits.Remove(this.TraitType);
        Rules.ApplyCombatAdjustments();
    }

    public int DiceBonus
    {
        get => 
            base.genericParameters[3];
        set
        {
            base.genericParameters[3] = value;
        }
    }

    public int DiceCount
    {
        get => 
            base.genericParameters[4];
        set
        {
            base.genericParameters[4] = value;
        }
    }

    public DiceType DiceType
    {
        get => 
            ((DiceType) base.genericParameters[0]);
        set
        {
            base.genericParameters[0] = (int) value;
        }
    }

    public override bool Single =>
        false;

    public SkillCheckType Skill
    {
        get => 
            ((SkillCheckType) base.genericParameters[2]);
        set
        {
            base.genericParameters[2] = (int) value;
        }
    }

    public TraitType TraitType
    {
        get => 
            ((TraitType) base.genericParameters[1]);
        set
        {
            base.genericParameters[1] = (int) value;
        }
    }

    public override EffectType Type =>
        EffectType.BoostCheck;
}

