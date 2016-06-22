using System;
using System.IO;

public class FieldStream : IDisposable
{
    private bool disposed;
    private Stream file;
    private BinaryReader reader;
    private BinaryWriter writer;

    public FieldStream(string filename, FileMode mode)
    {
        if ((mode == FileMode.Open) && File.Exists(filename))
        {
            this.file = File.Open(filename, mode);
            this.reader = new BinaryReader(this.file);
        }
        if (mode == FileMode.Create)
        {
            this.file = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            this.writer = new BinaryWriter(this.file);
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        try
        {
            if (!this.disposed && disposing)
            {
                if (this.file != null)
                {
                    this.file.Dispose();
                }
                if (this.reader != null)
                {
                    this.reader.Close();
                }
                if (this.writer != null)
                {
                    this.writer.Close();
                }
            }
        }
        finally
        {
            this.file = null;
            this.reader = null;
            this.writer = null;
            this.disposed = true;
        }
    }

    ~FieldStream()
    {
        this.Dispose(false);
    }

    public bool ReadBool(bool defaultValue)
    {
        try
        {
            if (this.CanRead)
            {
                return this.reader.ReadBoolean();
            }
        }
        catch
        {
        }
        return defaultValue;
    }

    public byte ReadByte(byte defaultValue)
    {
        try
        {
            if (this.CanRead)
            {
                return this.reader.ReadByte();
            }
        }
        catch
        {
        }
        return defaultValue;
    }

    public float ReadFloat(float defaultValue)
    {
        try
        {
            if (this.CanRead)
            {
                return this.reader.ReadSingle();
            }
        }
        catch
        {
        }
        return defaultValue;
    }

    public int ReadInt(int defaultValue)
    {
        try
        {
            if (this.CanRead)
            {
                return this.reader.ReadInt32();
            }
        }
        catch
        {
        }
        return defaultValue;
    }

    public void WriteBool(bool dataValue)
    {
        try
        {
            if (this.CanWrite)
            {
                this.writer.Write(dataValue);
            }
        }
        catch
        {
        }
    }

    public void WriteByte(byte dataValue)
    {
        try
        {
            if (this.CanWrite)
            {
                this.writer.Write(dataValue);
            }
        }
        catch
        {
        }
    }

    public void WriteFloat(float dataValue)
    {
        try
        {
            if (this.CanWrite)
            {
                this.writer.Write(dataValue);
            }
        }
        catch
        {
        }
    }

    public void WriteInt(int dataValue)
    {
        try
        {
            if (this.CanWrite)
            {
                this.writer.Write(dataValue);
            }
        }
        catch
        {
        }
    }

    public bool CanRead =>
        (this.reader != null);

    public bool CanWrite =>
        (this.writer != null);
}

