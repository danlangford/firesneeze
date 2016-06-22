using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

internal class PersistentTable
{
    private string fileName;
    private Dictionary<string, byte[]> table = new Dictionary<string, byte[]>();

    public PersistentTable(string file)
    {
        this.fileName = file;
    }

    public void Clear()
    {
        this.table.Clear();
    }

    public bool ContainsKey(string s) => 
        this.table.ContainsKey(s);

    public bool Load()
    {
        this.table.Clear();
        if (!File.Exists(this.fileName))
        {
            return false;
        }
        using (Stream stream = File.Open(this.fileName, FileMode.Open))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                reader.ReadInt32();
                int num = reader.ReadInt32();
                for (int i = 0; i < num; i++)
                {
                    string str = reader.ReadString();
                    int count = reader.ReadInt32();
                    byte[] buffer = reader.ReadBytes(count);
                    this.table[str] = buffer;
                }
            }
        }
        return true;
    }

    public void Remove(string s)
    {
        if (this.table.ContainsKey(s))
        {
            this.table.Remove(s);
        }
    }

    public bool Save()
    {
        using (Stream stream = File.Open(this.fileName, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(1);
                writer.Write(this.Count);
                IEnumerator enumerator = this.Keys.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        string current = (string) enumerator.Current;
                        writer.Write(current);
                        byte[] buffer = this.table[current];
                        writer.Write(buffer.Length);
                        writer.Write(buffer, 0, buffer.Length);
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
            }
        }
        return true;
    }

    public int Count =>
        this.table.Count;

    public byte[] this[string s]
    {
        get => 
            this.table[s];
        set
        {
            this.table[s] = value;
        }
    }

    public ICollection Keys =>
        this.table.Keys;
}

