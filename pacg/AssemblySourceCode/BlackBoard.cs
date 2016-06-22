using System;
using System.Collections.Generic;
using System.IO;

public class BlackBoard
{
    private Dictionary<string, bool> boardBool = new Dictionary<string, bool>(5);
    private Dictionary<string, int> boardInt = new Dictionary<string, int>(5);
    private Dictionary<string, string> boardStr = new Dictionary<string, string>(5);
    private string ID;

    public BlackBoard(string ID)
    {
        this.ID = ID;
    }

    public void Add<T>(string name, int k)
    {
        if (typeof(T) == typeof(int))
        {
            int num = this.Get<int>(name) + k;
            this.Set<int>(name, num);
        }
    }

    public void Clear()
    {
        this.boardStr.Clear();
        this.boardInt.Clear();
        this.boardBool.Clear();
    }

    public void ClearBitFlag(string name, int n)
    {
        if (n < 0x20)
        {
            int num = 0;
            if (this.boardInt.ContainsKey(name))
            {
                num = this.boardInt[name];
            }
            this.boardInt[name] = num & ~(((int) 1) << n);
        }
    }

    public T Get<T>(string name)
    {
        if ((typeof(T) == typeof(string)) && this.boardStr.ContainsKey(name))
        {
            return (T) this.boardStr[name];
        }
        if ((typeof(T) == typeof(int)) && this.boardInt.ContainsKey(name))
        {
            return (T) this.boardInt[name];
        }
        if ((typeof(T) == typeof(bool)) && this.boardBool.ContainsKey(name))
        {
            return (T) this.boardBool[name];
        }
        return default(T);
    }

    public bool GetBitFlag(string name, int n)
    {
        if ((n < 0x20) && this.boardInt.ContainsKey(name))
        {
            int num = this.boardInt[name];
            return ((num & (((int) 1) << n)) != 0);
        }
        return false;
    }

    public void OnLoadData()
    {
        byte[] buffer;
        if (Game.GetObjectData(this.ID, out buffer))
        {
            ByteStream stream = new ByteStream(buffer);
            if (stream != null)
            {
                stream.ReadInt();
                int num = stream.ReadInt();
                for (int i = 0; i < num; i++)
                {
                    string str = stream.ReadString();
                    this.boardStr[str] = stream.ReadString();
                }
                num = stream.ReadInt();
                for (int j = 0; j < num; j++)
                {
                    string str2 = stream.ReadString();
                    this.boardInt[str2] = stream.ReadInt();
                }
                num = stream.ReadInt();
                for (int k = 0; k < num; k++)
                {
                    string str3 = stream.ReadString();
                    this.boardBool[str3] = stream.ReadBool();
                }
            }
        }
    }

    public void OnSaveData()
    {
        using (new MemoryStream())
        {
            ByteStream stream2 = new ByteStream();
            if (stream2 != null)
            {
                stream2.WriteInt(1);
                stream2.WriteInt(this.boardStr.Count);
                foreach (string str in this.boardStr.Keys)
                {
                    stream2.WriteString(str);
                    stream2.WriteString(this.boardStr[str]);
                }
                stream2.WriteInt(this.boardInt.Count);
                foreach (string str2 in this.boardInt.Keys)
                {
                    stream2.WriteString(str2);
                    stream2.WriteInt(this.boardInt[str2]);
                }
                stream2.WriteInt(this.boardBool.Count);
                foreach (string str3 in this.boardBool.Keys)
                {
                    stream2.WriteString(str3);
                    stream2.WriteBool(this.boardBool[str3]);
                }
                Game.SetObjectData(this.ID, stream2.ToArray());
            }
        }
    }

    public void Set<T>(string name, T value)
    {
        if (typeof(T) == typeof(string))
        {
            this.boardStr[name] = value as string;
        }
        if (typeof(T) == typeof(int))
        {
            this.boardInt[name] = (int) value;
        }
        if (typeof(T) == typeof(bool))
        {
            this.boardBool[name] = (bool) value;
        }
    }

    public void SetBitFlag(string name, int n)
    {
        if (n < 0x20)
        {
            int num = 0;
            if (this.boardInt.ContainsKey(name))
            {
                num = this.boardInt[name];
            }
            this.boardInt[name] = num | (((int) 1) << n);
        }
    }
}

