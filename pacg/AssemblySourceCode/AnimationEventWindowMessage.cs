using System;
using UnityEngine;

public class AnimationEventWindowMessage : MonoBehaviour
{
    [Tooltip("Method name to invoke on the window")]
    public string Message;

    public void SendMessageToWindow()
    {
        UI.Window.SendMessage(this.Message);
    }
}

