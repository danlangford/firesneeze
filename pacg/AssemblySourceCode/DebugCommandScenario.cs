using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class DebugCommandScenario : DebugCommand
{
    private string Difficulty(string s)
    {
        int result = 0;
        if (!int.TryParse(s, out result))
        {
            return base.Error("strange parameter " + base.Parameter(s));
        }
        result = Mathf.Clamp(result, 0, 2);
        Scenario.Current.Difficulty = result;
        return base.Success("difficulty of " + base.Parameter(Scenario.Current.ID) + " is " + base.Parameter(result.ToString()));
    }

    public override string Run(string[] args)
    {
        if (args.Length < 2)
        {
            return this.HelpText;
        }
        if (args[1] == "unlock")
        {
            return this.Unlock();
        }
        if (args[1] == "wildcard")
        {
            if (args.Length < 3)
            {
                return this.HelpText;
            }
            if (args.Length == 3)
            {
                return this.Wildcards(args[2], null, null);
            }
            if (args.Length == 4)
            {
                return this.Wildcards(args[2], args[3], null);
            }
            return this.Wildcards(args[2], args[3], args[4]);
        }
        if (Scenario.Current == null)
        {
            return base.Error("scenario not loaded");
        }
        if (args[1] == "difficulty")
        {
            if (args.Length < 3)
            {
                return this.HelpText;
            }
            return this.Difficulty(args[2]);
        }
        if ((Turn.Character == null) || (Location.Current == null))
        {
            return base.Error("location not loaded");
        }
        string[] textArray1 = new string[] { "win", "lose" };
        if (!base.IsArgValid(args[1], textArray1))
        {
            return base.Error("strange parameter " + base.Parameter(args[1]));
        }
        if (args[1] == "win")
        {
            Scenario.Current.Complete = true;
        }
        Game.UI.OptionsPanel.Show(false);
        Turn.State = GameStateType.End;
        return base.Success("scenario " + base.Parameter(args[1]));
    }

    private string Unlock()
    {
        Settings.Debug.StoryMode = false;
        return base.Success("unlocked all scenarios");
    }

    private string Wildcards(string wc1, string wc2 = null, string wc3 = null)
    {
        if (wc1 == "clear")
        {
            Settings.Debug.Wildcard1 = null;
            wc1 = null;
        }
        if (wc2 == "clear")
        {
            Settings.Debug.Wildcard2 = null;
            wc2 = null;
        }
        if (wc3 == "clear")
        {
            Settings.Debug.Wildcard3 = null;
            wc3 = null;
        }
        if (!string.IsNullOrEmpty(wc1))
        {
            Settings.Debug.Wildcard1 = wc1;
        }
        if (!string.IsNullOrEmpty(wc2))
        {
            Settings.Debug.Wildcard2 = wc2;
        }
        if (!string.IsNullOrEmpty(wc3))
        {
            Settings.Debug.Wildcard3 = wc3;
        }
        string[] textArray1 = new string[] { "wildcards:\n1=", base.Parameter(Settings.Debug.Wildcard1), "\n2=", base.Parameter(Settings.Debug.Wildcard2), "\n3=", base.Parameter(Settings.Debug.Wildcard3) };
        return base.Success(string.Concat(textArray1));
    }

    public override string Command =>
        "scenario";

    public override string HelpText =>
        "Syntax: scenario [win|lose|unlock|difficulty|wildcard]";
}

