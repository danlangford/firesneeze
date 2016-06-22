using System;

public class ScriptParam3Attribute : BaseScriptParamAttribute
{
    public ScriptParam3Attribute(string displayName, string description, string defaultValue) : base(displayName, description, defaultValue)
    {
    }

    public ScriptParam3Attribute(string displayName, string description, string defaultValue, Scripts.BrowserType browser) : base(displayName, description, defaultValue, browser)
    {
    }
}

