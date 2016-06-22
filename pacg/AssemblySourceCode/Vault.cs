using System;
using System.Collections;
using System.Collections.Generic;

public class Vault
{
    private static PersistentTable table;

    public static bool Add(string nickname, Character character)
    {
        if ((CountByMode(Game.GameMode) >= Capacity) && !table.ContainsKey(nickname))
        {
            return false;
        }
        ByteStream bs = new ByteStream();
        new VaultEntry(character).ToStream(bs);
        table[nickname] = bs.ToArray();
        table.Save();
        return true;
    }

    public static void Audit()
    {
        bool flag = false;
        List<string> list = new List<string>(Constants.MAX_PARTY_MEMBERS * Constants.NUM_SAVE_SLOTS);
        for (int i = GameDirectory.FirstSlot; i <= GameDirectory.LastSlot; i++)
        {
            if (!GameDirectory.Empty(i))
            {
                GameSaveFile file = new GameSaveFile(i);
                if ((file.Header != null) && file.Header.IsValid())
                {
                    for (int j = 0; j < file.Header.CharacterNicks.Length; j++)
                    {
                        list.Add(file.Header.CharacterNicks[j]);
                    }
                }
            }
        }
        if (!GameDirectory.Empty(Constants.SAVE_SLOT_QUEST))
        {
            GameSaveFile file2 = new GameSaveFile(Constants.SAVE_SLOT_QUEST);
            if ((file2.Header != null) && (file2.Header.CharacterNicks != null))
            {
                for (int k = 0; k < file2.Header.CharacterNicks.Length; k++)
                {
                    list.Add(file2.Header.CharacterNicks[k]);
                }
            }
        }
        string[] array = new string[table.Count];
        table.Keys.CopyTo(array, 0);
        foreach (string str in array)
        {
            ByteStream bs = new ByteStream(table[str]);
            VaultEntry entry = VaultEntry.FromStream(bs);
            if (entry.locked && !list.Contains(str))
            {
                entry.locked = false;
                ByteStream stream2 = new ByteStream();
                entry.ToStream(stream2);
                table[str] = stream2.ToArray();
                flag = true;
            }
        }
        if (flag)
        {
            table.Save();
        }
    }

    public static void Clear()
    {
        table.Clear();
        table.Save();
    }

    public static bool Contains(string nickname) => 
        table.ContainsKey(nickname);

    public static int CountByMode(GameModeType mode)
    {
        int num = 0;
        IEnumerator enumerator = table.Keys.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                string current = (string) enumerator.Current;
                ByteStream bs = new ByteStream(table[current]);
                if (VaultEntry.FromStream(bs).mode == mode)
                {
                    num++;
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
        return num;
    }

    public static string CreateNickname(Character character)
    {
        string displayName = character.DisplayName;
        for (int i = 1; i <= Capacity; i++)
        {
            if (!table.ContainsKey(displayName))
            {
                return displayName;
            }
            displayName = character.DisplayName + " " + i;
        }
        return displayName;
    }

    public static Character Get(string nickname)
    {
        if (table.ContainsKey(nickname))
        {
            ByteStream bs = new ByteStream(table[nickname]);
            VaultEntry entry = VaultEntry.FromStream(bs);
            Character character = CharacterTable.Create(entry.id);
            ByteStream stream2 = new ByteStream(entry.data);
            character.OnLoadData(stream2);
            return character;
        }
        return null;
    }

    public static bool IsLocked(string nickname)
    {
        if (table.ContainsKey(nickname))
        {
            ByteStream bs = new ByteStream(table[nickname]);
            return VaultEntry.FromStream(bs).locked;
        }
        return false;
    }

    public static ICollection List(GameModeType mode)
    {
        List<string> list = new List<string>(table.Count);
        IEnumerator enumerator = table.Keys.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                string current = (string) enumerator.Current;
                ByteStream bs = new ByteStream(table[current]);
                if (VaultEntry.FromStream(bs).mode == mode)
                {
                    list.Add(current);
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
        return list;
    }

    public static void Load()
    {
        table = new PersistentTable(GameDirectory.GetVaultPath());
        table.Load();
    }

    public static bool Lock(string nickname, bool isLocked)
    {
        if (table.ContainsKey(nickname))
        {
            ByteStream bs = new ByteStream(table[nickname]);
            VaultEntry entry = VaultEntry.FromStream(bs);
            entry.locked = isLocked;
            ByteStream stream2 = new ByteStream();
            entry.ToStream(stream2);
            table[nickname] = stream2.ToArray();
            table.Save();
            return true;
        }
        return false;
    }

    public static bool Mode(string nickname, GameModeType mode)
    {
        if (table.ContainsKey(nickname))
        {
            ByteStream bs = new ByteStream(table[nickname]);
            VaultEntry entry = VaultEntry.FromStream(bs);
            entry.mode = mode;
            ByteStream stream2 = new ByteStream();
            entry.ToStream(stream2);
            table[nickname] = stream2.ToArray();
            table.Save();
            return true;
        }
        return false;
    }

    public static bool Remove(string nickname)
    {
        if (table.ContainsKey(nickname))
        {
            table.Remove(nickname);
            table.Save();
            return true;
        }
        return false;
    }

    public static void Save()
    {
        table.Save();
    }

    public static int Capacity =>
        20;

    public static int Count =>
        table.Count;
}

