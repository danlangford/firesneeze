using System;

public class DebugCommandLicense : DebugCommand
{
    public override string Run(string[] args)
    {
        if (args.Length != 3)
        {
            return this.HelpText;
        }
        if (Settings.Debug.DemoMode)
        {
            return base.Error("restricted operation disallowed");
        }
        string[] textArray1 = new string[] { "revoke", "grant", "list" };
        if (!base.IsArgValid(args[1], textArray1))
        {
            return base.Error("strange parameter " + base.Parameter(args[1]));
        }
        if (args[1] == "revoke")
        {
            for (int j = 0; j < LicenseTable.Count; j++)
            {
                string iD = LicenseTable.Key(j);
                string nickname = LicenseTable.Get(iD).Nickname;
                if ((args[2] == "all") || (args[2] == nickname))
                {
                    LicenseManager.RevokeLicense(iD);
                }
            }
            return base.Success("revoked " + base.Parameter(args[2]));
        }
        if (args[1] == "grant")
        {
            for (int k = 0; k < LicenseTable.Count; k++)
            {
                string str3 = LicenseTable.Key(k);
                string str4 = LicenseTable.Get(str3).Nickname;
                if ((args[2] == "all") || (args[2] == str4))
                {
                    LicenseManager.GrantLicense(str3);
                }
            }
            return base.Success("granted " + base.Parameter(args[2]));
        }
        if (args[1] != "list")
        {
            return this.HelpText;
        }
        string s = "\n";
        for (int i = 0; i < LicenseTable.Count; i++)
        {
            string str6 = LicenseTable.Key(i);
            string str7 = LicenseTable.Get(str6).Nickname;
            if ((args[2] == "all") || (args[2] == str7))
            {
                string str8 = !LicenseManager.GetIsLicensed(str6) ? "N" : "Y";
                string[] textArray2 = new string[] { s, str7, "=", str8, " " };
                s = string.Concat(textArray2);
            }
        }
        return base.Success(base.Parameter(s));
    }

    public override string Command =>
        "license";

    public override string HelpText =>
        "Syntax: license [grant|revoke|list] [all|id]";
}

