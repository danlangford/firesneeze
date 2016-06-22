using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private int currentMusicClip;
    private Coroutine currentMusicCoroutine;
    [Tooltip("fade time for music")]
    public float musicFadeTime = 1.5f;
    private bool[] musicSourceAmbient;
    [Tooltip("audio sources used to play background music")]
    public AudioSource[] musicSources;
    private SoundPool soundPool;

    private void Awake()
    {
        this.soundPool = new SoundPool(base.GetComponent<AudioSource>(), 5);
        this.musicSourceAmbient = new bool[this.musicSources.Length];
    }

    private float GetMaxVolume()
    {
        if ((this.currentMusicClip < this.musicSourceAmbient.Length) && this.musicSourceAmbient[this.currentMusicClip])
        {
            return Settings.Volume;
        }
        return Settings.MusicVolume;
    }

    private float GetMusicFadeRate(float totalTime)
    {
        if (totalTime == 0f)
        {
            return 100f;
        }
        return ((Settings.MusicVolume / totalTime) * Time.fixedDeltaTime);
    }

    private AudioSource GetMusicSource(int n) => 
        this.musicSources[n];

    public void MusicClear()
    {
        for (int i = 0; i < this.musicSources.Length; i++)
        {
            if (!this.musicSources[i].isPlaying)
            {
                this.musicSources[i].Stop();
                this.musicSources[i].clip = null;
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator MusicFadeOut(float fadeTime) => 
        new <MusicFadeOut>c__IteratorA3 { 
            fadeTime = fadeTime,
            <$>fadeTime = fadeTime,
            <>f__this = this
        };

    private bool MusicIsAtMax()
    {
        if ((this.currentMusicClip < this.musicSourceAmbient.Length) && this.musicSourceAmbient[this.currentMusicClip])
        {
            return (this.MusicSource.volume >= Settings.Volume);
        }
        return (this.MusicSource.volume >= Settings.MusicVolume);
    }

    public void MusicPlay(AudioClip clip)
    {
        this.MusicPlay(clip, true, false);
    }

    public void MusicPlay(AudioClip clip, bool loop, bool ambient)
    {
        if (((clip != null) && Settings.Music) && (((clip != this.MusicSource.clip) || !this.MusicSource.isPlaying) || !this.MusicIsAtMax()))
        {
            if (this.currentMusicCoroutine != null)
            {
                base.StopCoroutine(this.currentMusicCoroutine);
            }
            this.currentMusicCoroutine = base.StartCoroutine(this.MusicSwitch(clip, this.musicFadeTime, loop, ambient));
        }
    }

    public void MusicResume()
    {
        this.MusicPlay(this.MusicSource.clip, this.MusicSource.loop, this.musicSourceAmbient[this.currentMusicClip]);
    }

    public void MusicStop()
    {
        if (this.currentMusicCoroutine != null)
        {
            base.StopCoroutine(this.currentMusicCoroutine);
        }
        this.currentMusicCoroutine = base.StartCoroutine(this.MusicFadeOut(this.musicFadeTime));
    }

    [DebuggerHidden]
    private IEnumerator MusicSwitch(AudioClip track, float fadeTime, bool loop, bool ambient) => 
        new <MusicSwitch>c__IteratorA4 { 
            track = track,
            fadeTime = fadeTime,
            ambient = ambient,
            loop = loop,
            <$>track = track,
            <$>fadeTime = fadeTime,
            <$>ambient = ambient,
            <$>loop = loop,
            <>f__this = this
        };

    public void Pause(bool paused)
    {
        this.soundPool.isPaused = paused;
    }

    public void Play(SoundEffectType type)
    {
        this.Play(Game.UI.GetSfx(type));
    }

    public void Play(AudioClip clip)
    {
        if (this.soundPool != null)
        {
            this.soundPool.Play(clip);
        }
    }

    public void SetMusicVolume(float volume)
    {
        for (int i = 0; i < this.musicSources.Length; i++)
        {
            if (!this.musicSourceAmbient[i])
            {
                this.musicSources[i].volume = volume;
            }
        }
    }

    public void SetSoundVolume(float volume)
    {
        this.soundPool.SetVolume(volume);
        for (int i = 0; (i < this.musicSources.Length) && (i < this.musicSourceAmbient.Length); i++)
        {
            if (this.musicSourceAmbient[i])
            {
                this.musicSources[i].volume = volume;
            }
        }
    }

    public void Stop(AudioClip clip)
    {
        if (this.soundPool != null)
        {
            this.soundPool.Stop(clip);
        }
    }

    private void SwitchMusicSource()
    {
        this.currentMusicClip++;
        if (this.currentMusicClip >= this.musicSources.Length)
        {
            this.currentMusicClip = 0;
        }
    }

    private AudioSource MusicSource =>
        this.GetMusicSource(this.currentMusicClip);

    [CompilerGenerated]
    private sealed class <MusicFadeOut>c__IteratorA3 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>fadeTime;
        internal SoundManager <>f__this;
        internal float <fadeRate>__0;
        internal float fadeTime;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    if (!this.<>f__this.MusicSource.isPlaying)
                    {
                        goto Label_00B0;
                    }
                    this.<fadeRate>__0 = this.<>f__this.GetMusicFadeRate(this.fadeTime);
                    break;

                case 1:
                    break;

                default:
                    goto Label_00D8;
            }
            if (this.<>f__this.MusicSource.volume > 0f)
            {
                AudioSource musicSource = this.<>f__this.MusicSource;
                musicSource.volume -= this.<fadeRate>__0;
                this.$current = new WaitForEndOfFrame();
                this.$PC = 1;
                return true;
            }
            this.<>f__this.MusicSource.Pause();
        Label_00B0:
            this.<>f__this.MusicSource.volume = 0f;
            this.<>f__this.currentMusicCoroutine = null;
            this.$PC = -1;
        Label_00D8:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }

    [CompilerGenerated]
    private sealed class <MusicSwitch>c__IteratorA4 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal bool <$>ambient;
        internal float <$>fadeTime;
        internal bool <$>loop;
        internal AudioClip <$>track;
        internal SoundManager <>f__this;
        internal float <fadeRate>__0;
        internal bool ambient;
        internal float fadeTime;
        internal bool loop;
        internal AudioClip track;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    if ((this.track != null) && (((this.track != this.<>f__this.MusicSource.clip) || !this.<>f__this.MusicSource.isPlaying) || !this.<>f__this.MusicIsAtMax()))
                    {
                        this.<fadeRate>__0 = this.<>f__this.GetMusicFadeRate(this.fadeTime);
                        if (((this.track != this.<>f__this.MusicSource.clip) && this.<>f__this.MusicSource.isPlaying) && this.<>f__this.MusicSource.isPlaying)
                        {
                            break;
                        }
                        goto Label_0149;
                    }
                    goto Label_02CB;

                case 1:
                    break;

                case 2:
                    goto Label_023C;

                default:
                    goto Label_02CB;
            }
            while (this.<>f__this.MusicSource.volume > 0f)
            {
                AudioSource musicSource = this.<>f__this.MusicSource;
                musicSource.volume -= this.<fadeRate>__0;
                this.$current = new WaitForEndOfFrame();
                this.$PC = 1;
                goto Label_02CD;
            }
            this.<>f__this.MusicSource.Pause();
        Label_0149:
            if (this.<>f__this.MusicSource.clip != this.track)
            {
                this.<>f__this.SwitchMusicSource();
                this.<>f__this.musicSourceAmbient[this.<>f__this.currentMusicClip] = this.ambient;
            }
            if (this.<>f__this.musicSourceAmbient[this.<>f__this.currentMusicClip])
            {
                this.<>f__this.MusicSource.clip = this.track;
                this.<>f__this.MusicSource.Play();
            }
            if (this.<>f__this.MusicSource.clip != this.track)
            {
                this.<>f__this.MusicSource.Stop();
                this.<>f__this.MusicSource.clip = this.track;
                this.<>f__this.MusicSource.Play();
                goto Label_0287;
            }
            this.<>f__this.MusicSource.UnPause();
        Label_023C:
            while (!this.<>f__this.MusicIsAtMax())
            {
                AudioSource source2 = this.<>f__this.MusicSource;
                source2.volume += this.<fadeRate>__0;
                this.$current = new WaitForEndOfFrame();
                this.$PC = 2;
                goto Label_02CD;
            }
        Label_0287:
            this.<>f__this.MusicSource.volume = this.<>f__this.GetMaxVolume();
            this.<>f__this.MusicSource.loop = this.loop;
            this.<>f__this.currentMusicCoroutine = null;
            this.$PC = -1;
        Label_02CB:
            return false;
        Label_02CD:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }
}

