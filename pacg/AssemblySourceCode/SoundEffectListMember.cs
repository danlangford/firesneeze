using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct SoundEffectListMember
{
    [Tooltip("possible sfx to return. If more than one return one randomly.")]
    public AudioClip[] Clips;
    [Tooltip("what causes the sound to play")]
    public SoundEffectType Trigger;
}

