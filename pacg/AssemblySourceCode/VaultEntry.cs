using System;
using UnityEngine;

public class VaultEntry
{
    [Tooltip("the character data")]
    public byte[] data;
    [Tooltip("the character's ID (not unique within the vault)")]
    public string id;
    [Tooltip("entries are locked when the character is in a party")]
    public bool locked;
    [Tooltip("this character is only available in this game mode")]
    public GameModeType mode;

    public VaultEntry()
    {
    }

    public VaultEntry(Character character)
    {
        this.id = character.ID;
        this.locked = false;
        this.mode = Game.GameMode;
        ByteStream bs = new ByteStream();
        character.OnSaveData(bs);
        this.data = bs.ToArray();
    }

    public static VaultEntry FromStream(ByteStream bs)
    {
        VaultEntry entry = new VaultEntry();
        bs.ReadInt();
        entry.id = bs.ReadString();
        entry.locked = bs.ReadBool();
        entry.mode = (GameModeType) bs.ReadInt();
        entry.data = bs.ReadByteArray();
        return entry;
    }

    public void ToStream(ByteStream bs)
    {
        bs.WriteInt(1);
        bs.WriteString(this.id);
        bs.WriteBool(this.locked);
        bs.WriteInt((int) this.mode);
        bs.WriteByteArray(this.data);
    }
}

