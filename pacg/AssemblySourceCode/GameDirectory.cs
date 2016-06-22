using System;
using System.IO;

internal class GameDirectory
{
    public static void Clear(int slot)
    {
        if (!Empty(slot))
        {
            GameSaveFile gameSave = new GameSaveFile(slot);
            Game.Network.DeleteGameSave(gameSave);
        }
        foreach (string str2 in Directory.GetFiles(GetSlotPath(slot)))
        {
            File.Delete(str2);
        }
        if (ActiveSlot == slot)
        {
            ActiveSlot = 0;
        }
    }

    public static void Copy(int fromSlot, int toSlot)
    {
        if ((fromSlot != toSlot) && !Empty(fromSlot))
        {
            Clear(toSlot);
            string slotPath = GetSlotPath(fromSlot);
            string str2 = GetSlotPath(toSlot);
            foreach (string str3 in Directory.GetFiles(slotPath))
            {
                string fileName = Path.GetFileName(str3);
                string destFileName = Path.Combine(str2, fileName);
                File.Copy(str3, destFileName, true);
            }
        }
    }

    private static void CreateEmptySlots()
    {
        for (int i = FirstSlot; i <= LastSlot; i++)
        {
            string str = GetSlotPath(i);
            if (!Directory.Exists(str))
            {
                Directory.CreateDirectory(str);
            }
        }
        string slotPath = GetSlotPath(Constants.SAVE_SLOT_QUEST);
        if (!Directory.Exists(slotPath))
        {
            Directory.CreateDirectory(slotPath);
        }
        string path = GetSlotPath(Constants.SAVE_SLOT_TUTORIAL);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static bool Empty(int slot) => 
        (Directory.GetFiles(GetSlotPath(slot)).Length == 0);

    public static int FindEmptySlot()
    {
        for (int i = FirstSlot; i <= LastSlot; i++)
        {
            if (Empty(i))
            {
                return i;
            }
        }
        return -1;
    }

    private static string GetBaseDirectory() => 
        Device.GetDocumentFolderPath();

    public static string GetConquestsPath() => 
        Path.Combine(GetBaseDirectory(), "conquests");

    public static string GetHistoryPath() => 
        Path.Combine(GetBaseDirectory(), "history");

    public static string GetLicensesPath() => 
        Path.Combine(GetBaseDirectory(), "licenses");

    public static string GetSettingsPath() => 
        Path.Combine(GetBaseDirectory(), "settings");

    private static string GetSlotPath(int slot)
    {
        if ((slot >= 0) && (slot <= 9))
        {
            return Path.Combine(GetBaseDirectory(), "slot0" + slot);
        }
        return Path.Combine(GetBaseDirectory(), "slot" + slot);
    }

    public static string GetVaultPath() => 
        Path.Combine(GetBaseDirectory(), "vault");

    public static void Init()
    {
        string baseDirectory = GetBaseDirectory();
        if (!Directory.Exists(baseDirectory))
        {
            Directory.CreateDirectory(baseDirectory);
        }
        CreateEmptySlots();
    }

    public static string PathToFile(int slot, string fileName) => 
        Path.Combine(GetSlotPath(slot), fileName);

    public static int ActiveSlot
    {
        get => 
            Settings.ActiveSaveSlot;
        set
        {
            if (Settings.ActiveSaveSlot != value)
            {
                Settings.ActiveSaveSlot = value;
                Settings.Save();
            }
        }
    }

    public static int FirstSlot =>
        1;

    public static int LastSlot =>
        ((FirstSlot + Constants.NUM_SAVE_SLOTS) - 1);
}

