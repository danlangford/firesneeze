using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class DebugCommandPhase : DebugCommand
{
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map1;

    public override string Run(string[] args)
    {
        object[] objArray1;
        if (args.Length <= 1)
        {
            return base.Success("current phase: " + Turn.Phase.ToString());
        }
        args[1] = args[1].ToLower();
        string key = args[1];
        if (key != null)
        {
            int num;
            if (<>f__switch$map1 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(3) {
                    { 
                        "end",
                        0
                    },
                    { 
                        "setup",
                        1
                    },
                    { 
                        "finish",
                        2
                    }
                };
                <>f__switch$map1 = dictionary;
            }
            if (<>f__switch$map1.TryGetValue(key, out num))
            {
                switch (num)
                {
                    case 0:
                        Turn.End = true;
                        Turn.State = GameStateType.Finish;
                        goto Label_00E5;

                    case 1:
                        Turn.Phase = TurnPhaseType.Give;
                        Turn.State = GameStateType.Setup;
                        goto Label_00E5;

                    case 2:
                        Turn.State = GameStateType.Finish;
                        goto Label_00E5;
                }
            }
        }
        return base.Error("Unrecognized: " + args[1]);
    Label_00E5:
        objArray1 = new object[] { "Changed state to : ", Turn.State, " with phase : ", Turn.Phase };
        return base.Success(string.Concat(objArray1));
    }

    public override string Command =>
        "phase";

    public override string HelpText =>
        "Syntax: phase [end|setup|finish]";
}

