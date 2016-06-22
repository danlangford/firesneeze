using PlayFab.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameSaveFile
{
    private GameSaveHeader header;
    public const string SAVE_FILE_NAME = "game.sav";
    private Dictionary<string, byte[]> table;

    public GameSaveFile(int slot)
    {
        this.Slot = slot;
        this.FileName = GameDirectory.PathToFile(this.Slot, "game.sav");
        this.header = new GameSaveHeader();
        this.table = new Dictionary<string, byte[]>();
    }

    [JsonConstructor]
    public GameSaveFile(Dictionary<string, byte[]> data, GameSaveHeader header)
    {
        this.table = data;
        this.header = header;
    }

    public void Clear()
    {
        if (this.Header != null)
        {
            this.Header.Clear();
        }
        if (this.Data != null)
        {
            this.Data.Clear();
        }
    }

    public bool Delete()
    {
        try
        {
            for (int i = 0; i < this.Header.CharacterNicks.Length; i++)
            {
                Vault.Lock(this.Header.CharacterNicks[i], false);
            }
            GameDirectory.Clear(this.Slot);
            this.Clear();
            return true;
        }
        catch (Exception exception)
        {
            Debug.Log("Delete Game Failed: " + exception);
        }
        try
        {
            GameDirectory.Clear(this.Slot);
            this.Clear();
        }
        catch (Exception exception2)
        {
            Debug.Log("Delete Game Failed in Recovery: " + exception2);
        }
        return false;
    }

    public bool Load()
    {
        this.table.Clear();
        try
        {
            if (File.Exists(this.FileName))
            {
                using (Stream stream = File.Open(this.FileName, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        reader.ReadInt32();
                        this.header.Load(reader);
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
        }
        catch (Exception exception)
        {
            Debug.Log("Load Game Failed: " + exception);
        }
        return false;
    }

    public void LoadFromOnlineSave(GameSaveFile fileToCopy)
    {
        this.header = fileToCopy.header;
        this.table = fileToCopy.Data;
    }

    private void LoadHeader()
    {
        try
        {
            if (File.Exists(this.FileName))
            {
                using (Stream stream = File.Open(this.FileName, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        reader.ReadInt32();
                        this.header.Load(reader);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Debug.Log("Load Header Failed: " + exception);
            this.header = null;
        }
    }

    public bool Save()
    {
        try
        {
            using (Stream stream = File.Open(this.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(1);
                    this.header.Save(writer);
                    writer.Write(this.table.Count);
                    foreach (string str in this.table.Keys)
                    {
                        writer.Write(str);
                        byte[] buffer = this.table[str];
                        writer.Write(buffer.Length);
                        writer.Write(buffer, 0, buffer.Length);
                    }
                    return true;
                }
            }
        }
        catch (Exception exception)
        {
            Debug.Log("Save Game Failed: " + exception);
        }
        return false;
    }

    public bool SaveInternal()
    {
        try
        {
            using (Stream stream = File.Open(this.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(1);
                    this.header.SaveInternal(writer);
                    writer.Write(this.table.Count);
                    foreach (string str in this.table.Keys)
                    {
                        writer.Write(str);
                        byte[] buffer = this.table[str];
                        writer.Write(buffer.Length);
                        writer.Write(buffer, 0, buffer.Length);
                    }
                    return true;
                }
            }
        }
        catch (Exception exception)
        {
            Debug.Log("Save Game Failed: " + exception);
        }
        return false;
    }

    public Dictionary<string, byte[]> Data =>
        this.table;

    public string FileName { get; private set; }

    public GameSaveHeader Header
    {
        get
        {
            if ((this.header != null) && (this.header.Length <= 0))
            {
                this.LoadHeader();
            }
            return this.header;
        }
    }

    public int Slot { get; private set; }
}

