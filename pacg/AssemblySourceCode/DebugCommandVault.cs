using System;
using System.Collections;
using System.Text;

public class DebugCommandVault : DebugCommand
{
    private string Clear()
    {
        int count = Vault.Count;
        Vault.Clear();
        return base.Success("Removed " + base.Parameter(count.ToString()) + " vault entries");
    }

    private string List()
    {
        StringBuilder builder = new StringBuilder();
        IEnumerator enumerator = Vault.List(GameModeType.Quest).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                string current = (string) enumerator.Current;
                builder.Append(current);
                builder.Append("[Q]");
                if (Vault.IsLocked(current))
                {
                    builder.Append("L");
                }
                builder.Append(" ");
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
        IEnumerator enumerator2 = Vault.List(GameModeType.Story).GetEnumerator();
        try
        {
            while (enumerator2.MoveNext())
            {
                string str2 = (string) enumerator2.Current;
                builder.Append(str2);
                builder.Append("[S]");
                if (Vault.IsLocked(str2))
                {
                    builder.Append("L");
                }
                builder.Append(" ");
            }
        }
        finally
        {
            IDisposable disposable2 = enumerator2 as IDisposable;
            if (disposable2 == null)
            {
            }
            disposable2.Dispose();
        }
        return builder.ToString();
    }

    public override string Run(string[] args)
    {
        if (args.Length >= 2)
        {
            if (args[1] == "clear")
            {
                return this.Clear();
            }
            if (args[1] == "list")
            {
                return this.List();
            }
        }
        return this.HelpText;
    }

    public override string Command =>
        "vault";

    public override string HelpText =>
        "Syntax: vault [clear|list]";
}

