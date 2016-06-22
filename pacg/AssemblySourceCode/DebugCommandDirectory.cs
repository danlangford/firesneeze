using System;
using System.IO;
using UnityEngine;

public class DebugCommandDirectory : DebugCommand
{
    public override string Run(string[] args)
    {
        if (args.Length != 2)
        {
            return this.HelpText;
        }
        string[] textArray1 = new string[] { "clear" };
        if (!base.IsArgValid(args[1], textArray1))
        {
            return base.Error("strange parameter " + base.Parameter(args[1]));
        }
        if (args[1] == "clear")
        {
            PlayerPrefs.DeleteAll();
            for (int i = 1; i < Constants.NUM_SAVE_SLOTS; i++)
            {
                GameDirectory.Clear(i);
            }
            GameDirectory.Clear(Constants.SAVE_SLOT_DEBUG);
            GameDirectory.Clear(Constants.SAVE_SLOT_QUEST);
            FileInfo info = new FileInfo(GameDirectory.GetHistoryPath());
            info.Delete();
            info = new FileInfo(GameDirectory.GetLicensesPath());
            info.Delete();
            info = new FileInfo(GameDirectory.GetSettingsPath());
            info.Delete();
            info = new FileInfo(GameDirectory.GetVaultPath());
            info.Delete();
            new FileInfo(GameDirectory.GetConquestsPath()).Delete();
            Settings.ActiveSaveSlot = 0;
            Settings.Save();
        }
        return base.Success("all game files removed");
    }

    public override string Command =>
        "dir";

    public override string HelpText =>
        "Syntax: dir [clear]";
}

