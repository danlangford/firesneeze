using System;
using System.Text;
using UnityEngine;

public class DebugCommandInfo : DebugCommand
{
    private static GameObject debugOverlayObject;

    private void CreateDebugInfo()
    {
        System.Type[] components = new System.Type[] { typeof(DebugCommandDisplay) };
        debugOverlayObject = new GameObject("Debug Overlay", components);
    }

    private void DestroyDebugInfo()
    {
        if (debugOverlayObject != null)
        {
            UnityEngine.Object.Destroy(debugOverlayObject);
        }
        debugOverlayObject = null;
    }

    private bool HasDebugInfo() => 
        (debugOverlayObject != null);

    public override string Run(string[] args)
    {
        if (this.HasDebugInfo())
        {
            this.DestroyDebugInfo();
        }
        else
        {
            this.CreateDebugInfo();
        }
        StringBuilder builder = new StringBuilder();
        builder.Append("State: ").Append(base.Accent(Turn.State.ToString()));
        return builder.ToString();
    }

    public override string Command =>
        "info";

    public override string HelpText =>
        "Syntax: info";
}

