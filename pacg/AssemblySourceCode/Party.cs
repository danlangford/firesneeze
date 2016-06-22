using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Party
{
    private static readonly string ID = "_PARTY";
    private static CharacterIndexer Indexer;
    private static List<Character> Members = new List<Character>();

    public static void Add(Character member)
    {
        if ((member != null) && (Find(member.ID) == null))
        {
            Members.Add(member);
        }
    }

    public static void ApplyEffect(Effect e)
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            Characters[i].ApplyEffect(e);
        }
    }

    public static bool Audit()
    {
        bool flag = true;
        if (Collection.Loaded)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>(Collection.Count);
            foreach (CollectionEntry entry in Collection.Entries)
            {
                if (!dictionary.ContainsKey(entry.id))
                {
                    dictionary.Add(entry.id, entry.quantity);
                }
            }
            for (int i = 0; i < Campaign.Distributions.Count; i++)
            {
                string id = Campaign.Distributions[i];
                if (CardTable.LookupCardBooster(id))
                {
                    if (!dictionary.ContainsKey(id) || (dictionary[id] <= 0))
                    {
                        Campaign.Distributions.RemoveAt(i);
                    }
                    else
                    {
                        dictionary[id] -= 1;
                    }
                }
            }
            for (int j = 0; j < Characters.Count; j++)
            {
                for (int k = 0; k < Characters[j].Deck.Count; k++)
                {
                    Card card = Characters[j].Deck[k];
                    if (CardTable.LookupCardBooster(card.ID))
                    {
                        if (!dictionary.ContainsKey(card.ID) || (dictionary[card.ID] <= 0))
                        {
                            Characters[j].Deck.Remove(card);
                            card.Destroy();
                            flag = false;
                        }
                        else
                        {
                            dictionary[card.ID] -= 1;
                        }
                    }
                }
            }
        }
        return flag;
    }

    public static void AutoActivateAbilities()
    {
        if (!Turn.Canceling)
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                Character character = Characters[i];
                if (character.Alive)
                {
                    for (int j = 0; j < character.Powers.Count; j++)
                    {
                        CharacterPower power = character.Powers[j];
                        if (power.Automatic && power.IsValid())
                        {
                            power.Activate();
                            GuiWindowLocation window = UI.Window as GuiWindowLocation;
                            if (window != null)
                            {
                                window.powersPanel.ShowPowerVFX(power);
                            }
                        }
                    }
                }
            }
        }
    }

    public static void Clear()
    {
        for (int i = 0; i < Members.Count; i++)
        {
            Members[i].Clear();
        }
        Members.Clear();
    }

    public static bool Contains(Character character)
    {
        for (int i = 0; i < Members.Count; i++)
        {
            if (Members[i] == character)
            {
                return true;
            }
        }
        return false;
    }

    public static int CountActiveMembers()
    {
        int num = 0;
        for (int i = 0; i < Members.Count; i++)
        {
            if (Members[i].Active == ActiveType.Active)
            {
                num++;
            }
        }
        return num;
    }

    public static int CountLivingMembers()
    {
        int num = 0;
        for (int i = 0; i < Members.Count; i++)
        {
            if (Members[i].Alive)
            {
                num++;
            }
        }
        return num;
    }

    public static Character Find(string ID)
    {
        for (int i = 0; i < Members.Count; i++)
        {
            if (Members[i].ID == ID)
            {
                return Members[i];
            }
        }
        return null;
    }

    public static Character FindByNickName(string nickname)
    {
        for (int i = 0; i < Members.Count; i++)
        {
            if (Members[i].NickName == nickname)
            {
                return Members[i];
            }
        }
        return null;
    }

    private static void GetCardOwners(string id, ref List<Character> owners)
    {
        owners.Clear();
        for (int i = 0; i < Members.Count; i++)
        {
            for (int j = 0; j < Members[i].Deck.Count; j++)
            {
                if (Members[i].Deck[j].ID == id)
                {
                    owners.Add(Members[i]);
                }
            }
        }
    }

    public static string[] GetMemberList()
    {
        string[] strArray = new string[Members.Count];
        for (int i = 0; i < Members.Count; i++)
        {
            strArray[i] = Members[i].ID;
        }
        return strArray;
    }

    public static int GetNextLivingMemberIndex(int index)
    {
        int num = 0;
        int nextPartyMemberIndex = index;
        while (num++ <= Members.Count)
        {
            nextPartyMemberIndex = GetNextPartyMemberIndex(nextPartyMemberIndex);
            if (Members[nextPartyMemberIndex].Alive)
            {
                index = nextPartyMemberIndex;
                return index;
            }
        }
        return index;
    }

    public static int GetNextPartyMemberIndex(int index)
    {
        int num = ++index;
        if (num >= Members.Count)
        {
            num = 0;
        }
        return num;
    }

    public static int IndexOf(string ID)
    {
        for (int i = 0; i < Members.Count; i++)
        {
            if (Members[i].ID == ID)
            {
                return i;
            }
        }
        return -1;
    }

    public static void OnCardEncountered(Card card)
    {
        for (int i = 0; i < Members.Count; i++)
        {
            Members[i].OnCardEncountered(card);
        }
    }

    public static void OnCheckCompleted()
    {
        Scenario.Current.OnCheckCompleted();
        for (int i = 0; i < Members.Count; i++)
        {
            Members[i].OnCheckCompleted();
        }
    }

    public static void OnLoadData()
    {
        byte[] buffer;
        if (Game.GetObjectData(ID, out buffer))
        {
            ByteStream stream = new ByteStream(buffer);
            if (stream != null)
            {
                stream.ReadInt();
                int num = stream.ReadInt();
                for (int i = 0; i < num; i++)
                {
                    Character item = CharacterTable.Create(stream.ReadString());
                    if (item != null)
                    {
                        Members.Add(item);
                    }
                }
                foreach (Character character2 in Members)
                {
                    character2.OnLoadData();
                }
            }
        }
    }

    public static void OnLocationExplored(Card card)
    {
        for (int i = 0; i < Members.Count; i++)
        {
            Members[i].OnLocationExplored(card);
        }
    }

    public static void OnSaveData()
    {
        ByteStream stream = new ByteStream();
        if (stream != null)
        {
            stream.WriteInt(1);
            stream.WriteInt(Members.Count);
            foreach (Character character in Members)
            {
                stream.WriteString(character.ID);
            }
            Game.SetObjectData(ID, stream.ToArray());
            foreach (Character character2 in Members)
            {
                character2.OnSaveData();
            }
        }
    }

    public static void OnStepCompleted()
    {
        for (int i = 0; i < Members.Count; i++)
        {
            Members[i].OnStepCompleted();
        }
    }

    public static void OnTurnCompleted()
    {
        for (int i = 0; i < Members.Count; i++)
        {
            Members[i].OnTurnCompleted();
        }
    }

    public static void Remove(Character member)
    {
        if (member != null)
        {
            Members.Remove(member);
        }
    }

    public static void RemoveEffect(Effect e)
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            Characters[i].RemoveEffect(e);
        }
    }

    public static void RemoveEffect(string ID)
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            Characters[i].RemoveEffect(ID);
        }
    }

    public static void Rest()
    {
        for (int i = 0; i < Members.Count; i++)
        {
            for (int j = 0; j < Members[i].Deck.Count; j++)
            {
                Card card = Members[i].Deck[j];
                for (int k = 0; k < UbersTable.Count; k++)
                {
                    if (UbersTable.Get(k).to == card.ID)
                    {
                        Members[i].Deck.Remove(card);
                        Card card2 = CardTable.Create(UbersTable.Get(k).from);
                        Members[i].Deck.Add(card2);
                    }
                }
            }
        }
    }

    public static void RewardExperience(int amount)
    {
        for (int i = 0; i < Members.Count; i++)
        {
            if ((Members[i].Alive && (amount > 0)) && ((0x7fffffff - amount) > (Members[i].XP + Members[i].XPX)))
            {
                Character local1 = Members[i];
                local1.XPX += amount;
            }
        }
    }

    public static void RewardGold(int amount)
    {
        if ((amount > 0) && (Scenario.Current != null))
        {
            Scenario current = Scenario.Current;
            current.GPX += amount;
        }
    }

    public static void Swap(Character c1, Character c2)
    {
        if (c1 != c2)
        {
            int index = Members.IndexOf(c1);
            int num2 = Members.IndexOf(c2);
            if ((index >= 0) && (num2 >= 0))
            {
                Character character = Members[index];
                Members[index] = Members[num2];
                Members[num2] = character;
            }
        }
    }

    public static void Validate()
    {
        List<Character> owners = new List<Character>(Constants.MAX_PARTY_MEMBERS);
        for (int i = 0; i < UbersTable.Count; i++)
        {
            UbersTableEntry entry = UbersTable.Get(i);
            GetCardOwners(entry.from, ref owners);
            if (owners.Count > 1)
            {
                owners.Shuffle<Character>();
                owners.RemoveAt(0);
                for (int j = 0; j < owners.Count; j++)
                {
                    Card card = owners[j].Deck[entry.from];
                    owners[j].Deck.Remove(card);
                    Card card2 = CardTable.Create(entry.to);
                    owners[j].Deck.Add(card2);
                    owners[j].Deck.Shuffle();
                }
            }
        }
    }

    public static CharacterIndexer Characters
    {
        get
        {
            if (Indexer == null)
            {
            }
            return (Indexer = new CharacterIndexer());
        }
    }

    public static int Level
    {
        get
        {
            int a = 0;
            for (int i = 0; i < Members.Count; i++)
            {
                a = Mathf.Max(a, Members[i].Level);
            }
            return a;
        }
    }

    public static int Tier
    {
        get
        {
            int a = 0;
            for (int i = 0; i < Members.Count; i++)
            {
                a = Mathf.Max(a, Members[i].Tier);
            }
            return a;
        }
    }

    public sealed class CharacterIndexer
    {
        public int Count =>
            Party.Members.Count;

        public Character this[int index] =>
            Party.Members[index];

        public Character this[string id] =>
            Party.Find(id);
    }
}

