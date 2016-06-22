using System;
using System.Collections.Generic;
using System.Reflection;

public class EffectList
{
    private List<Effect> list;

    public EffectList() : this(10)
    {
    }

    public EffectList(int initialCapacity)
    {
        this.list = new List<Effect>(initialCapacity);
    }

    public void Add(Effect newEffect)
    {
        List<Effect> list = this.list;
        lock (list)
        {
            if (newEffect.Stacking)
            {
                for (int i = 0; i < this.list.Count; i++)
                {
                    if ((this.list[i].Type == newEffect.Type) && this.list[i].Stack(newEffect))
                    {
                        goto Label_00C8;
                    }
                }
            }
            if (newEffect.Single)
            {
                for (int j = 0; j < this.list.Count; j++)
                {
                    if (this.list[j].Equals(newEffect))
                    {
                        goto Label_00C8;
                    }
                }
            }
            this.list.Insert(0, newEffect);
        Label_00C8:;
        }
    }

    public void Clear()
    {
        List<Effect> list = this.list;
        lock (list)
        {
            this.list.Clear();
        }
    }

    public bool Contains(EffectType type)
    {
        for (int i = 0; i < this.list.Count; i++)
        {
            if (this.list[i].Type == type)
            {
                return true;
            }
        }
        return false;
    }

    public void FromArray(byte[] data)
    {
        ByteStream bs = new ByteStream(data);
        bs.ReadInt();
        int num = bs.ReadInt();
        for (int i = 0; i < num; i++)
        {
            Effect effect;
            EffectType type = (EffectType) bs.ReadInt();
            string[] strArray = bs.ReadStringArray();
            int duration = bs.ReadInt();
            SkillCheckValueType[] checks = null;
            int num4 = bs.ReadInt();
            if (num4 > 0)
            {
                checks = new SkillCheckValueType[num4];
                for (int j = 0; j < num4; j++)
                {
                    checks[j] = new SkillCheckValueType((SkillCheckType) bs.ReadInt(), bs.ReadInt());
                }
            }
            int[] genericParameters = bs.ReadIntArray();
            CardFilter filter = null;
            if (bs.ReadBool())
            {
                filter = CardFilter.FromStream(bs);
            }
            if (strArray.Length > 0)
            {
                effect = EffectFactory.Create(type, strArray[0], duration, checks, filter, genericParameters);
            }
            else
            {
                effect = EffectFactory.Create(type, null, duration, checks, filter, genericParameters);
            }
            if (effect != null)
            {
                for (int k = 1; k < strArray.Length; k++)
                {
                    effect.sources.Add(strArray[k]);
                }
                this.list.Add(effect);
            }
        }
    }

    public Effect Get(EffectType type)
    {
        for (int i = 0; i < this.list.Count; i++)
        {
            if (this.list[i].Type == type)
            {
                return this.list[i];
            }
        }
        return null;
    }

    private bool IsBaneEffect(string id) => 
        (((id.StartsWith("HE") || id.StartsWith("VL")) || id.StartsWith("MO")) || id.StartsWith("BX"));

    public void OnCheckCompleted()
    {
        bool flag = false;
        List<Effect> list = this.list;
        lock (list)
        {
            for (int i = this.list.Count - 1; i >= 0; i--)
            {
                if (this.list[i].RemoveAfterCheck() && (!this.IsBaneEffect(this.list[i].source) || (this.list[i].source == Turn.Card.ID)))
                {
                    this.list.Extract<Effect>(i).OnEffectFinished();
                    flag = true;
                }
            }
        }
        if (flag)
        {
            this.UpdateEffectsPanel();
        }
    }

    public void OnEncounterComplete()
    {
        bool flag = false;
        List<Effect> list = this.list;
        lock (list)
        {
            for (int i = this.list.Count - 1; i >= 0; i--)
            {
                if (this.list[i].RemoveAfterEncounter())
                {
                    this.list.Extract<Effect>(i).OnEffectFinished();
                    flag = true;
                }
            }
        }
        if (flag)
        {
            this.UpdateEffectsPanel();
        }
    }

    public void OnTurnCompleted()
    {
        bool flag = false;
        List<Effect> list = this.list;
        lock (list)
        {
            for (int i = this.list.Count - 1; i >= 0; i--)
            {
                if (this.list[i].RemoveAfterTurn())
                {
                    this.list.Extract<Effect>(i).OnEffectFinished();
                    flag = true;
                }
                else if (this.list[i].duration < Effect.DurationPermament)
                {
                    Effect local1 = this.list[i];
                    local1.duration--;
                }
            }
        }
        if (flag)
        {
            this.UpdateEffectsPanel();
        }
    }

    public void Remove(Effect e)
    {
        bool flag = false;
        List<Effect> list = this.list;
        lock (list)
        {
            for (int i = this.list.Count - 1; i >= 0; i--)
            {
                if (this.list[i].Equals(e))
                {
                    this.list.Extract<Effect>(i).OnEffectFinished();
                    flag = true;
                    break;
                }
            }
        }
        if (flag)
        {
            this.UpdateEffectsPanel();
        }
    }

    public void RemoveAt(int index)
    {
        List<Effect> list = this.list;
        lock (list)
        {
            this.list.Extract<Effect>(index).OnEffectFinished();
        }
        this.UpdateEffectsPanel();
    }

    public byte[] ToArray()
    {
        ByteStream bs = new ByteStream();
        bs.WriteInt(1);
        bs.WriteInt(this.list.Count);
        for (int i = 0; i < this.list.Count; i++)
        {
            Effect effect = this.list[i];
            bs.WriteInt((int) effect.Type);
            bs.WriteStringArray(effect.sources.ToArray());
            bs.WriteInt(effect.duration);
            if (effect.checks != null)
            {
                bs.WriteInt(effect.checks.Length);
                for (int j = 0; j < effect.checks.Length; j++)
                {
                    bs.WriteInt((int) effect.checks[j].skill);
                    bs.WriteInt(effect.checks[j].rank);
                }
            }
            else
            {
                bs.WriteInt(0);
            }
            bs.WriteIntArray(effect.genericParameters);
            bs.WriteBool(effect.filter != null);
            if (effect.filter != null)
            {
                effect.filter.ToStream(bs);
            }
        }
        return bs.ToArray();
    }

    public void UpdateEffectsPanel()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.effectsPanel.Refresh();
        }
    }

    public int Count =>
        this.list.Count;

    public Effect this[int index] =>
        this.list[index];
}

