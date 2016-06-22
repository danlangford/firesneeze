using System;

public class ScriptParam0Attribute : BaseScriptParamAttribute
{
    public ScriptParam0Attribute(string displayName, string description, string defaultValue) : base(displayName, description, defaultValue)
    {
    }

    public ScriptParam0Attribute(string displayName, string description, string defaultValue, Scripts.BrowserType browser) : base(displayName, description, defaultValue, browser)
    {
    }
}

