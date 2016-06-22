using System;
using System.Runtime.CompilerServices;

public class ScriptAttribute : Attribute
{
    public ScriptAttribute(string name, string path)
    {
        this.Name = name;
        this.Path = path;
    }

    public string Name { get; private set; }

    public string Path { get; private set; }
}

