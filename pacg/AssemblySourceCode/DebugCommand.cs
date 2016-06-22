using System;

public abstract class DebugCommand
{
    protected DebugCommand()
    {
    }

    protected string Accent(string s) => 
        ("<color=yellow>" + s + "</color>");

    protected string Error(string s) => 
        ("<color=red>Error: </color>" + s);

    protected bool IsArgValid(string arg, params string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == arg)
            {
                return true;
            }
        }
        return false;
    }

    protected string Parameter(string s) => 
        ("<color=yellow>" + s + "</color>");

    public virtual bool Parse(string command) => 
        (this.Command == command);

    public virtual string Run(string[] args) => 
        null;

    protected string Success(string s) => 
        ("<color=green>Success: </color>" + s);

    public virtual string Command =>
        null;

    public virtual string HelpText =>
        null;
}

