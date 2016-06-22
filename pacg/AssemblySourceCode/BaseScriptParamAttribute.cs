using System;
using System.Runtime.CompilerServices;

public abstract class BaseScriptParamAttribute : Attribute
{
    public BaseScriptParamAttribute(string displayName, string description, string defaultValue)
    {
        this.DisplayName = displayName;
        this.Description = description;
        this.DefaultValue = defaultValue;
        this.Browser = Scripts.BrowserType.None;
    }

    public BaseScriptParamAttribute(string displayName, string description, string defaultValue, Scripts.BrowserType browser)
    {
        this.DisplayName = displayName;
        this.Description = description;
        this.DefaultValue = defaultValue;
        this.Browser = browser;
    }

    public Scripts.BrowserType Browser { get; private set; }

    public string DefaultValue { get; private set; }

    public string Description { get; private set; }

    public string DisplayName { get; private set; }
}

