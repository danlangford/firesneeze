using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    [Tooltip("optional checks parameter")]
    public SkillCheckValueType[] checks;
    [Tooltip("number of turns/checks that this effect lasts including other player turns")]
    public int duration;
    [Tooltip("effect lasts only for one check")]
    public static readonly int DurationCheck;
    [Tooltip("effect lasts only for one encounter")]
    public static readonly int DurationEncounter = 1;
    [Tooltip("effect lasts for entire scenario")]
    public static readonly int DurationPermament = 0x3e8;
    [Tooltip("effect lasts only for one turn")]
    public static readonly int DurationTurn = 2;
    [Tooltip("optional cardfilter parameter")]
    public CardFilter filter;
    [Tooltip("generic parameters")]
    public int[] genericParameters;
    [Tooltip("the actual sources. When stacking effects it's possible to extend the number of sources. Non-stacking effects will typically have one source")]
    public List<string> sources;

    public Effect(string source, int duration)
    {
        this.sources = new List<string>(1);
        this.sources.Add(source);
        this.duration = duration;
        this.checks = null;
        this.filter = null;
    }

    public Effect(string source, int duration, CardFilter filter)
    {
        this.sources = new List<string>(1);
        this.sources.Add(source);
        this.duration = duration;
        this.checks = null;
        this.filter = filter;
    }

    public Effect(string source, int duration, SkillCheckValueType[] checks, CardFilter filter)
    {
        this.sources = new List<string>(1);
        this.sources.Add(source);
        this.duration = duration;
        this.checks = checks;
        this.filter = filter;
    }

    public virtual void Clear()
    {
    }

    protected string ConvertBonusToText(int bonus)
    {
        if (bonus >= 0)
        {
            return ("+ " + bonus.ToString());
        }
        return bonus.ToString();
    }

    public override bool Equals(object obj)
    {
        Effect effect = obj as Effect;
        if (effect == null)
        {
            return false;
        }
        if (this.duration != effect.duration)
        {
            return false;
        }
        if (this.filter != null)
        {
            if (!this.filter.Equals(effect.filter))
            {
                return false;
            }
        }
        else if (effect.filter != null)
        {
            return false;
        }
        if (this.Type != effect.Type)
        {
            return false;
        }
        return true;
    }

    public virtual string GetCardDecoration(Card card) => 
        null;

    public virtual string GetDisplayText() => 
        null;

    public virtual CardType GetEffectButtonIcon() => 
        CardTable.LookupCardType(this.source);

    public static string GetEffectID(MonoBehaviour go)
    {
        if (go != null)
        {
            Card component = go.GetComponent<Card>();
            if (component != null)
            {
                return component.ID;
            }
            Character componentInParent = go.GetComponentInParent<Character>();
            if (componentInParent != null)
            {
                Power power = go.GetComponent<Power>();
                if (power != null)
                {
                    return (componentInParent.ID + "/" + power.ID);
                }
                return componentInParent.ID;
            }
            Location location = go.GetComponent<Location>();
            if (location != null)
            {
                return location.ID;
            }
            Scenario scenario = go.GetComponent<Scenario>();
            if (scenario != null)
            {
                return scenario.ID;
            }
        }
        return null;
    }

    public override int GetHashCode()
    {
        int hashCode = this.duration.GetHashCode();
        for (int i = 0; i < this.genericParameters.Length; i++)
        {
            hashCode ^= this.genericParameters[i].GetHashCode();
        }
        return hashCode;
    }

    public virtual void Invoke()
    {
    }

    protected virtual bool IsEffectValid() => 
        true;

    public virtual bool IsInvokePossible() => 
        true;

    public virtual void OnEffectFinished()
    {
    }

    public virtual void OnEffectStarted(Character character)
    {
    }

    public virtual void OnEffectStarted(Scenario scenario)
    {
    }

    public virtual bool RemoveAfterCheck() => 
        (this.duration <= DurationCheck);

    public virtual bool RemoveAfterEncounter() => 
        (this.duration <= DurationEncounter);

    public virtual bool RemoveAfterTurn() => 
        (this.duration <= DurationTurn);

    public virtual bool Resolve() => 
        true;

    public virtual bool Stack(Effect e) => 
        (e.source == this.source);

    public virtual bool ShowSources =>
        true;

    public virtual bool Single =>
        false;

    public string source
    {
        get => 
            this.sources[0];
        set
        {
            this.sources[0] = value;
        }
    }

    public virtual bool Stacking =>
        false;

    public virtual EffectType Type =>
        EffectType.None;
}

