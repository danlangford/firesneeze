using System;
using System.IO;
using UnityEngine;

public class ByteStream
{
    private byte[] bytes;
    private MemoryStream ms;
    private int pos;

    public ByteStream()
    {
        this.ms = new MemoryStream();
    }

    public ByteStream(byte[] b)
    {
        this.bytes = b;
        this.pos = 0;
    }

    public bool ReadBool()
    {
        try
        {
            bool flag = BitConverter.ToBoolean(this.bytes, this.pos);
            this.pos++;
            return flag;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool[] ReadBoolArray()
    {
        try
        {
            int num = BitConverter.ToInt32(this.bytes, this.pos);
            this.pos += 4;
            bool[] flagArray = new bool[num];
            for (int i = 0; i < num; i++)
            {
                flagArray[i] = this.ReadBool();
            }
            return flagArray;
        }
        catch (Exception)
        {
            return new bool[0];
        }
    }

    public byte ReadByte()
    {
        try
        {
            byte num = this.bytes[this.pos];
            this.pos++;
            return num;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public byte[] ReadByteArray()
    {
        try
        {
            int num = BitConverter.ToInt32(this.bytes, this.pos);
            this.pos += 4;
            byte[] buffer = new byte[num];
            for (int i = 0; i < num; i++)
            {
                buffer[i] = this.bytes[this.pos];
                this.pos++;
            }
            return buffer;
        }
        catch (Exception)
        {
            return new byte[0];
        }
    }

    public float ReadFloat()
    {
        try
        {
            float num = BitConverter.ToSingle(this.bytes, this.pos);
            this.pos += 4;
            return num;
        }
        catch (Exception)
        {
            return 0f;
        }
    }

    public Guid ReadGuid()
    {
        try
        {
            return new Guid(this.ReadByteArray());
        }
        catch
        {
            return Guid.Empty;
        }
    }

    public int ReadInt()
    {
        try
        {
            int num = BitConverter.ToInt32(this.bytes, this.pos);
            this.pos += 4;
            return num;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public int[] ReadIntArray()
    {
        try
        {
            int num = BitConverter.ToInt32(this.bytes, this.pos);
            this.pos += 4;
            int[] numArray = new int[num];
            for (int i = 0; i < num; i++)
            {
                numArray[i] = this.ReadInt();
            }
            return numArray;
        }
        catch (Exception)
        {
            return new int[0];
        }
    }

    public string ReadString()
    {
        try
        {
            int num = BitConverter.ToInt32(this.bytes, this.pos);
            this.pos += 4;
            if (num <= 0)
            {
                return null;
            }
            char[] chArray = new char[num];
            for (int i = 0; i < num; i++)
            {
                chArray[i] = (char) this.bytes[this.pos];
                this.pos++;
            }
            return new string(chArray);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string[] ReadStringArray()
    {
        try
        {
            int num = BitConverter.ToInt32(this.bytes, this.pos);
            this.pos += 4;
            string[] strArray = new string[num];
            for (int i = 0; i < num; i++)
            {
                strArray[i] = this.ReadString();
            }
            return strArray;
        }
        catch (Exception)
        {
            return new string[0];
        }
    }

    public Vector3 ReadVector3()
    {
        try
        {
            float x = BitConverter.ToSingle(this.bytes, this.pos);
            this.pos += 4;
            float y = BitConverter.ToSingle(this.bytes, this.pos);
            this.pos += 4;
            float z = BitConverter.ToSingle(this.bytes, this.pos);
            this.pos += 4;
            return new Vector3(x, y, z);
        }
        catch (Exception)
        {
            return Vector3.zero;
        }
    }

    public byte[] ToArray() => 
        this.ms.ToArray();

    public void WriteBool(bool data)
    {
        this.ms.Write(BitConverter.GetBytes(data), 0, 1);
    }

    public void WriteBoolArray(bool[] data)
    {
        this.ms.Write(BitConverter.GetBytes(data.Length), 0, 4);
        for (int i = 0; i < data.Length; i++)
        {
            this.WriteBool(data[i]);
        }
    }

    public void WriteByte(byte data)
    {
        this.ms.WriteByte(data);
    }

    public void WriteByteArray(byte[] data)
    {
        this.ms.Write(BitConverter.GetBytes(data.Length), 0, 4);
        for (int i = 0; i < data.Length; i++)
        {
            this.ms.WriteByte(data[i]);
        }
    }

    public void WriteFloat(float data)
    {
        this.ms.Write(BitConverter.GetBytes(data), 0, 4);
    }

    public void WriteGuid(Guid guid)
    {
        this.WriteByteArray(guid.ToByteArray());
    }

    public void WriteInt(int data)
    {
        this.ms.Write(BitConverter.GetBytes(data), 0, 4);
    }

    public void WriteIntArray(int[] data)
    {
        if (data == null)
        {
            this.ms.Write(BitConverter.GetBytes(0), 0, 4);
        }
        else
        {
            this.ms.Write(BitConverter.GetBytes(data.Length), 0, 4);
            for (int i = 0; i < data.Length; i++)
            {
                this.WriteInt(data[i]);
            }
        }
    }

    public void WriteString(string data)
    {
        if (data == null)
        {
            this.ms.Write(BitConverter.GetBytes(0), 0, 4);
        }
        else
        {
            this.ms.Write(BitConverter.GetBytes(data.Length), 0, 4);
            for (int i = 0; i < data.Length; i++)
            {
                this.ms.Write(BitConverter.GetBytes(data[i]), 0, 1);
            }
        }
    }

    public void WriteStringArray(string[] data)
    {
        int num = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == null)
            {
                break;
            }
            num++;
        }
        this.ms.Write(BitConverter.GetBytes(num), 0, 4);
        for (int j = 0; j < num; j++)
        {
            if (data[j] == null)
            {
                break;
            }
            this.WriteString(data[j]);
        }
    }

    public void WriteVector3(Vector3 data)
    {
        this.ms.Write(BitConverter.GetBytes(data.x), 0, 4);
        this.ms.Write(BitConverter.GetBytes(data.y), 0, 4);
        this.ms.Write(BitConverter.GetBytes(data.z), 0, 4);
    }
}

