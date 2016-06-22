using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugCommandReport : DebugCommand
{
    public static string GetCrashBuddyRegionAreaArguments()
    {
        string name = string.Empty;
        string str2 = "N/A";
        if (!Application.isPlaying)
        {
            name = SceneManager.GetActiveScene().name;
        }
        else
        {
            name = SceneManager.GetActiveScene().name;
        }
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }
        name = Path.GetFileNameWithoutExtension(name).ToLower();
        int result = -1;
        Regex regex = new Regex(@"[^\d]");
        int.TryParse(regex.Replace(Game.Instance.BuildNumber, string.Empty), out result);
        object[] objArray1 = new object[] { "-region \"", str2, "\" -area \"", name, "\" -buildnum ", result, " -autoupdatepath \"\\\\oeitools\\oeitools\\crash buddy\"" };
        return string.Concat(objArray1);
    }

    public override string Run(string[] args)
    {
        string[] textArray1 = new string[] { "Bug", "bug" };
        if (!base.IsArgValid(args[0], textArray1))
        {
            return base.Error("strange parameter " + base.Parameter(args[0]));
        }
        return base.Error("not unity standalone windows or editor");
    }

    public override string Command =>
        "bug";

    public override string HelpText =>
        "Launching Crash Buddy";
}

