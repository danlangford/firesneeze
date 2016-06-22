using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AnimationEventAudio : MonoBehaviour
{
    [Tooltip("The audio source that you want to control")]
    public AudioSource audioSource;

    [DebuggerHidden]
    private IEnumerator FadeVolumeCoroutine(bool isAudible, float duration) => 
        new <FadeVolumeCoroutine>c__IteratorAB { 
            isAudible = isAudible,
            duration = duration,
            <$>isAudible = isAudible,
            <$>duration = duration,
            <>f__this = this
        };

    public void FadeVolumeIn(float duration)
    {
        base.StartCoroutine(this.FadeVolumeCoroutine(true, duration));
    }

    public void FadeVolumeOut(float duration)
    {
        base.StartCoroutine(this.FadeVolumeCoroutine(false, duration));
    }

    public void PlaySound(SoundEffectType sound)
    {
        UI.Sound.Play(sound);
    }

    [CompilerGenerated]
    private sealed class <FadeVolumeCoroutine>c__IteratorAB : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>duration;
        internal bool <$>isAudible;
        internal AnimationEventAudio <>f__this;
        internal float duration;
        internal bool isAudible;

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
                    if (!this.isAudible)
                    {
                        goto Label_0117;
                    }
                    this.<>f__this.audioSource.volume = 0f;
                    this.<>f__this.audioSource.Play();
                    break;

                case 1:
                    break;

                case 2:
                    goto Label_0117;

                default:
                    goto Label_016D;
            }
            if ((this.<>f__this.audioSource.volume < 1f) && (this.duration > 0f))
            {
                this.<>f__this.audioSource.volume += Time.deltaTime * this.duration;
                this.$current = new WaitForEndOfFrame();
                this.$PC = 1;
                goto Label_016F;
            }
            this.<>f__this.audioSource.volume = 1f;
            goto Label_0166;
        Label_0117:
            while ((this.<>f__this.audioSource.volume > 0f) && (this.duration > 0f))
            {
                this.<>f__this.audioSource.volume -= Time.deltaTime * this.duration;
                this.$current = new WaitForEndOfFrame();
                this.$PC = 2;
                goto Label_016F;
            }
            this.<>f__this.audioSource.volume = 0f;
            this.<>f__this.audioSource.Stop();
        Label_0166:
            this.$PC = -1;
        Label_016D:
            return false;
        Label_016F:
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

