using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SoundPool
{
    private AudioSource[] audios;
    private int currentAvailableSound;

    public SoundPool(AudioSource template, int size)
    {
        this.audios = new AudioSource[size];
        this.audios[0] = template;
        for (int i = 1; i < size; i++)
        {
            this.audios[i] = template.gameObject.AddComponent<AudioSource>();
            this.audios[i].playOnAwake = false;
            this.audios[i].priority = template.priority;
            this.audios[i].spatialBlend = template.spatialBlend;
            this.audios[i].dopplerLevel = template.dopplerLevel;
            this.audios[i].spread = template.spread;
        }
        this.isPaused = false;
    }

    private AudioSource GetAudioSource()
    {
        AudioSource source = null;
        for (int i = 0; i < this.audios.Length; i++)
        {
            source = this.audios[this.currentAvailableSound];
            this.currentAvailableSound++;
            if (this.currentAvailableSound >= this.audios.Length)
            {
                this.currentAvailableSound = 0;
            }
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return this.audios[this.currentAvailableSound];
    }

    private AudioSource GetPrevAudioSource()
    {
        int index = this.currentAvailableSound - 1;
        if (index < 0)
        {
            index = this.audios.Length - 1;
        }
        return this.audios[index];
    }

    public void Play(AudioClip clip)
    {
        if ((!this.isPaused && (clip != null)) && (((this.GetPrevAudioSource().clip != clip) || (this.GetPrevAudioSource().time > 0f)) || !this.GetPrevAudioSource().isPlaying))
        {
            AudioSource audioSource = this.GetAudioSource();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void SetVolume(float volume)
    {
        for (int i = 0; i < this.audios.Length; i++)
        {
            this.audios[i].volume = volume;
        }
    }

    public void Stop(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource source = null;
            for (int i = 0; i < this.audios.Length; i++)
            {
                source = this.audios[i];
                if (source.clip.name.Equals(clip.name))
                {
                    if (source.isPlaying)
                    {
                        this.currentAvailableSound = i;
                        source.Stop();
                    }
                    return;
                }
            }
        }
    }

    public bool isPaused { private get; set; }
}

