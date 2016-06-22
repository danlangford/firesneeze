using System;
using UnityEngine;

public class GuiCamera : MonoBehaviour
{
    private void Awake()
    {
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
    }
}

