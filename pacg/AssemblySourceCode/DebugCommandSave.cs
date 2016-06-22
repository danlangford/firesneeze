using System;
using System.IO;
using UnityEngine;

public class DebugCommandSave : DebugCommand
{
    public override string Run(string[] args)
    {
        Game.Save();
        string sourceFileName = GameDirectory.PathToFile(Game.SaveSlot, "game.sav");
        if (args.Length > 1)
        {
            try
            {
                File.Copy(sourceFileName, args[1], true);
                sourceFileName = args[1];
            }
            catch (Exception exception)
            {
                Debug.Log(exception);
            }
        }
        return ("saved to : " + sourceFileName);
    }

    public override string Command =>
        "save";

    public override string HelpText =>
        (this.Command + "optional:FileLocation or optional:savefilelocation");
}

