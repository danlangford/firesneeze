using System;

public class DebugCommandTutorial : DebugCommand
{
    private void Refresh()
    {
        if (UI.Window.Type == WindowType.MainMenu)
        {
            UI.Window.Refresh();
        }
    }

    public override string Run(string[] args)
    {
        if (args.Length != 2)
        {
            return this.HelpText;
        }
        string[] textArray1 = new string[] { "on", "off" };
        if (!base.IsArgValid(args[1], textArray1))
        {
            return base.Error("strange parameter " + base.Parameter(args[1]));
        }
        if (args[1] == "off")
        {
            Conquests.Complete(Constants.STORY_MODE_UNLOCKED);
            Conquests.Complete(Constants.QUEST_MODE_UNLOCKED);
            Settings.TutorialLevel = 0;
            Settings.Save();
            this.Refresh();
        }
        if (args[1] == "on")
        {
            Conquests.Clear(Constants.STORY_MODE_UNLOCKED);
            Conquests.Clear(Constants.QUEST_MODE_UNLOCKED);
            Settings.TutorialLevel = 1;
            Settings.Save();
            this.Refresh();
        }
        return base.Success("tutorial scenario is " + base.Parameter(args[1]));
    }

    public override string Command =>
        "tutorial";

    public override string HelpText =>
        "Syntax: tutorial [on|off]";
}

