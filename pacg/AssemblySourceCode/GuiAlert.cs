using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GuiAlert : GuiElement
{
    [Tooltip("additional keys to check for")]
    public string[] AdditionalAlerts;
    [Tooltip("predefined alert types")]
    public AlertManager.AlertType AlertType;
}

