using System;

public class DebugCommandGold : DebugCommand
{
    public override string Run(string[] args)
    {
        if (args.Length >= 2)
        {
            char[] chArray = args[1].ToCharArray();
            for (int i = 0; i < chArray.Length; i++)
            {
                if (!char.IsDigit(chArray[i]))
                {
                    return base.Error("strange parameter " + base.Parameter(args[1]));
                }
            }
            int amount = int.Parse(args[1]);
            if (amount > 0)
            {
                Game.Network.GiveGold(amount);
                return base.Success(base.Parameter(args[1]) + " gold added to user!");
            }
        }
        return this.HelpText;
    }

    public override string Command =>
        "gold";

    public override string HelpText =>
        "Syntax: gold [amount]";
}

