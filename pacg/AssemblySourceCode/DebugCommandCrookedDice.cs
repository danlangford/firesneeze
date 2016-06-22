using System;

internal class DebugCommandCrookedDice : DebugCommand
{
    public override string Run(string[] args)
    {
        if (args.Length < 3)
        {
            return base.Error("Not enough parameters to skew dice.");
        }
        try
        {
            string str = args[1].ToUpper();
            DiceType dice = (DiceType) ((int) Enum.Parse(typeof(DiceType), str));
            int roll = int.Parse(args[2]);
            Rules.CrookedDice(dice, roll);
            Settings.Debug.GodMode = false;
            Settings.Debug.PeonMode = false;
            object[] objArray1 = new object[] { "Skewed : ", dice, " to : ", roll };
            return base.Success(string.Concat(objArray1));
        }
        catch (Exception exception)
        {
            return base.Error("Exception : " + exception);
        }
    }

    public override string Command =>
        "skew";

    public override string HelpText =>
        "skew [DICETYPE] [RESULT]";
}

