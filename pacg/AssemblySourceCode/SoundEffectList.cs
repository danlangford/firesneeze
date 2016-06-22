using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectList : ScriptableObject
{
    private Dictionary<SoundEffectType, AudioClip[]> dictionary;
    [Tooltip("data our audio guy should edit")]
    public SoundEffectListMember[] List;

    public AudioClip GetSfx(SoundEffectType type)
    {
        AudioClip[] clipArray;
        if (this.dictionary.TryGetValue(type, out clipArray) && (clipArray.Length > 0))
        {
            return clipArray[UnityEngine.Random.Range(0, clipArray.Length)];
        }
        return null;
    }

    public void Initialize()
    {
        this.dictionary = new Dictionary<SoundEffectType, AudioClip[]>(this.List.Length);
        for (int i = 0; i < this.List.Length; i++)
        {
            this.dictionary.Add(this.List[i].Trigger, this.List[i].Clips);
        }
    }
}

