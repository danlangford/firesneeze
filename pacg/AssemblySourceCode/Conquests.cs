using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Conquests
{
    private static Dictionary<string, int> table;

    public static void Clear(string id)
    {
        if (table.ContainsKey(id))
        {
            table.Remove(id);
        }
    }

    public static void Complete(string id)
    {
        Complete(id, 0);
    }

    public static void Complete(string id, int level)
    {
        if (level >= 0)
        {
            int num;
            if (table.TryGetValue(id, out num))
            {
                if (level > num)
                {
                    table[id] = level;
                    Save();
                }
            }
            else
            {
                table.Add(id, level);
                Save();
            }
        }
    }

    public static int GetComplete(string id)
    {
        int num;
        if (table.TryGetValue(id, out num))
        {
            return num;
        }
        return -1;
    }

    public static bool IsComplete(string id) => 
        IsComplete(id, 0);

    public static bool IsComplete(string id, int level)
    {
        int num;
        return (table.TryGetValue(id, out num) && (num >= level));
    }

    public static void Load()
    {
        table = new Dictionary<string, int>();
        try
        {
            string conquestsPath = GameDirectory.GetConquestsPath();
            if (File.Exists(conquestsPath))
            {
                using (Stream stream = File.Open(conquestsPath, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        reader.ReadInt32();
                        int num = reader.ReadInt32();
                        for (int i = 0; i < num; i++)
                        {
                            string key = reader.ReadString();
                            int num3 = reader.ReadInt32();
                            table.Add(key, num3);
                        }
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Debug.Log("Conquests Load Failed: " + exception);
        }
    }

    private static void Save()
    {
        try
        {
            using (Stream stream = File.Open(GameDirectory.GetConquestsPath(), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(1);
                    writer.Write(table.Count);
                    foreach (string str2 in table.Keys)
                    {
                        writer.Write(str2);
                        writer.Write(table[str2]);
                    }
                }
            }
            Synchronize();
        }
        catch (Exception exception)
        {
            Debug.Log("Conquests Save Failed: " + exception);
        }
    }

    private static void Synchronize()
    {
        Game.Network.SynchronizeConquests(table);
    }

    public static void Synchronize(Dictionary<string, int> data)
    {
        bool flag = false;
        if (data != null)
        {
            foreach (string str in data.Keys)
            {
                int num = data[str];
                if (num >= 0)
                {
                    int num2;
                    if (table.TryGetValue(str, out num2))
                    {
                        if (num > num2)
                        {
                            table[str] = num;
                            flag = true;
                        }
                    }
                    else
                    {
                        table.Add(str, num);
                        flag = true;
                    }
                }
            }
        }
        if (flag)
        {
            Save();
        }
    }
}

