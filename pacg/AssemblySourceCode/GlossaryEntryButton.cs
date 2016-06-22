using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(GuiLabel)), RequireComponent(typeof(BoxCollider2D))]
public class GlossaryEntryButton : MonoBehaviour
{
    public GlossaryCategory Category { get; set; }

    public int Index { get; set; }
}

