using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private bool isShaking;

    public void Shake(int cycles, float distance, float speed)
    {
        if (!this.isShaking)
        {
            base.StartCoroutine(this.ShakeCoroutine(cycles, distance, speed));
        }
    }

    [DebuggerHidden]
    private IEnumerator ShakeCoroutine(int cycles, float distance, float speed) => 
        new <ShakeCoroutine>c__Iterator44 { 
            cycles = cycles,
            distance = distance,
            speed = speed,
            <$>cycles = cycles,
            <$>distance = distance,
            <$>speed = speed,
            <>f__this = this
        };

    public int Dimensions
    {
        set
        {
            GameObject obj2 = GameObject.Find("/~UI/Camera/Camera - 3D");
            if (obj2 != null)
            {
                obj2.GetComponent<Camera>().enabled = value >= 3;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <ShakeCoroutine>c__Iterator44 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal int <$>cycles;
        internal float <$>distance;
        internal float <$>speed;
        internal CameraManager <>f__this;
        internal int <i>__0;
        internal Vector3 <offset>__1;
        internal int cycles;
        internal float distance;
        internal float speed;

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
                    this.<>f__this.isShaking = true;
                    this.<i>__0 = 0;
                    break;

                case 1:
                    LeanTween.moveLocal(this.<>f__this.gameObject, Vector3.zero, this.speed).setEase(LeanTweenType.easeInOutQuad);
                    this.$current = new WaitForSeconds(this.speed);
                    this.$PC = 2;
                    goto Label_010B;

                case 2:
                    this.<i>__0++;
                    break;

                default:
                    goto Label_0109;
            }
            if (this.<i>__0 < this.cycles)
            {
                this.<offset>__1 = (Vector3) (UnityEngine.Random.insideUnitCircle * this.distance);
                LeanTween.moveLocal(this.<>f__this.gameObject, this.<offset>__1, this.speed).setEase(LeanTweenType.easeInOutQuad);
                this.$current = new WaitForSeconds(this.speed);
                this.$PC = 1;
                goto Label_010B;
            }
            this.<>f__this.isShaking = false;
            this.$PC = -1;
        Label_0109:
            return false;
        Label_010B:
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

