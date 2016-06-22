using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnPowerList
{
    private List<TurnPowerListEntry> list = new List<TurnPowerListEntry>(5);

    public void Add(Power power)
    {
        if (((this.list != null) && (power != null)) && !this.Contains(power.ID))
        {
            TurnPowerListEntry item = new TurnPowerListEntry {
                ID = power.ID,
                Cooldown = power.Cooldown
            };
            this.list.Add(item);
        }
    }

    public void CancelCharacterPowers(bool allCharacters, bool includeAutomatic)
    {
        for (int i = this.list.Count - 1; i >= 0; i--)
        {
            int index = 0;
            if (allCharacters)
            {
                for (int j = 0; j < Party.CountActiveMembers(); j++)
                {
                    if (Party.Characters[index].HasPower(this.list[i].ID))
                    {
                        CharacterPower power = Party.Characters[index].GetPower(this.list[i].ID) as CharacterPower;
                        if (((power != null) && power.Cancellable) && (includeAutomatic || !power.Automatic))
                        {
                            power.Deactivate();
                            break;
                        }
                    }
                    index = Party.GetNextLivingMemberIndex(index);
                }
            }
            else
            {
                index = Turn.Number;
                if (Party.Characters[index].HasPower(this.list[i].ID))
                {
                    CharacterPower power2 = Party.Characters[index].GetPower(this.list[i].ID) as CharacterPower;
                    if (((power2 != null) && power2.Cancellable) && (includeAutomatic || !power2.Automatic))
                    {
                        power2.Deactivate();
                    }
                }
            }
        }
    }

    public void CancelLocationPowers()
    {
        for (int i = this.list.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < Location.Current.Powers.Count; j++)
            {
                if ((Location.Current.Powers[j].ID == this.list[i].ID) && Location.Current.Powers[j].Cancellable)
                {
                    Location.Current.Powers[j].Deactivate();
                    break;
                }
            }
        }
    }

    public void Clear()
    {
        if (this.list != null)
        {
            this.list.Clear();
        }
    }

    public void Clear(PowerCooldownType cooldown)
    {
        if (this.list != null)
        {
            for (int i = this.list.Count - 1; i >= 0; i--)
            {
                if (this.list[i].Cooldown == cooldown)
                {
                    this.list.RemoveAt(i);
                }
            }
        }
    }

    public bool Contains(string ID)
    {
        if (this.list != null)
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                if (this.list[i].ID == ID)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static TurnPowerList FromStream(ByteStream bs)
    {
        bs.ReadInt();
        if (!bs.ReadBool())
        {
            return null;
        }
        int num = bs.ReadInt();
        TurnPowerList list = new TurnPowerList();
        for (int i = 0; i < num; i++)
        {
            TurnPowerListEntry item = new TurnPowerListEntry {
                ID = bs.ReadString(),
                Cooldown = (PowerCooldownType) bs.ReadInt()
            };
            list.list.Add(item);
        }
        return list;
    }

    public void Remove(Power power)
    {
        if ((this.list != null) && (power != null))
        {
            for (int i = this.list.Count - 1; i >= 0; i--)
            {
                if (this.list[i].ID == power.ID)
                {
                    this.list.RemoveAt(i);
                }
            }
        }
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteInt(1);
        bs.WriteBool(this.list != null);
        if (this.list != null)
        {
            bs.WriteInt(this.list.Count);
            for (int i = 0; i < this.list.Count; i++)
            {
                bs.WriteString(this.list[i].ID);
                bs.WriteInt((int) this.list[i].Cooldown);
            }
        }
    }

    private class TurnPowerListEntry
    {
        [Tooltip("power Cooldown")]
        public PowerCooldownType Cooldown;
        [Tooltip("power ID")]
        public string ID;
    }
}

