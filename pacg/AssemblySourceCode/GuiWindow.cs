using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GuiWindow : MonoBehaviour
{
    [Tooltip("list of alerts in this window")]
    public List<GuiAlert> Alerts;
    private static GuiWindow currentWindow;
    private Dictionary<GameObject, GameObject> glowList;
    private bool isPaused;
    [Tooltip("background music for this window")]
    public AudioClip Music;
    [Tooltip("the music of this menu will always start from the beginning if true. It will also always start at 100% volume as a consequence of not unpausing the old music.")]
    public bool MusicAlwaysStartAtBeginning;
    [Tooltip("by default all music loops, but sometimes we don't want it to loop")]
    public bool MusicLoop = true;
    [Tooltip("preferred location of toasts to appear")]
    public Vector3 PreferredToastLocation;

    protected GuiWindow()
    {
    }

    protected virtual void Awake()
    {
        TouchKit.removeAllGestureRecognizers();
        Game.UI.ShowLoadScreen(false);
        Current = this;
    }

    public virtual void Close()
    {
        UI.Sound.MusicStop();
        Current = null;
    }

    public void Glow(GuiButton button, ButtonType buttonType, bool isGlowing)
    {
        if (this.glowList == null)
        {
            this.glowList = new Dictionary<GameObject, GameObject>(5);
        }
        if (this.glowList != null)
        {
            if (!isGlowing)
            {
                if (this.glowList.ContainsKey(button.gameObject))
                {
                    GameObject obj3 = this.glowList[button.gameObject];
                    if (obj3 != null)
                    {
                        UnityEngine.Object.Destroy(obj3);
                    }
                    this.glowList.Remove(button.gameObject);
                }
            }
            else if (!this.glowList.ContainsKey(button.gameObject))
            {
                GameObject obj2 = null;
                switch (buttonType)
                {
                    case ButtonType.Select:
                        obj2 = VfxButtonGlow.ApplyToSelect(button);
                        break;

                    case ButtonType.Portrait:
                        obj2 = VfxButtonGlow.ApplyToPortrait(button);
                        break;

                    case ButtonType.Tab:
                        obj2 = VfxButtonGlow.ApplyToTab(button);
                        break;

                    default:
                        obj2 = VfxButtonGlow.ApplyToButton(button);
                        break;
                }
                if (obj2 != null)
                {
                    this.glowList[button.gameObject] = obj2;
                }
            }
        }
    }

    protected virtual void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name.ToLower().Equals("loading"))
        {
            AlertManager.SetAlerts(null);
        }
        else
        {
            AlertManager.SetAlerts(this.Alerts);
        }
    }

    public virtual void OnLoadData()
    {
    }

    public virtual void OnSaveData()
    {
    }

    public virtual void Pause(bool isPaused)
    {
        this.Paused = isPaused;
    }

    public virtual void Refresh()
    {
    }

    public virtual void Reset()
    {
    }

    protected Vector2 ScreenToWorldPoint(Vector2 screenPoint)
    {
        Vector3 vector = this.Camera.ScreenToWorldPoint((Vector3) screenPoint);
        return new Vector2(vector.x, vector.y);
    }

    public virtual void Show(bool isVisible)
    {
        base.gameObject.SetActive(isVisible);
    }

    protected virtual void Start()
    {
        UI.Reset();
        if (this.MusicAlwaysStartAtBeginning)
        {
            UI.Sound.MusicClear();
        }
        UI.Sound.MusicPlay(this.Music, this.MusicLoop, false);
    }

    [DebuggerHidden]
    protected IEnumerator WaitForTime(float time) => 
        new <WaitForTime>c__Iterator83 { 
            time = time,
            <$>time = time,
            <>f__this = this
        };

    protected Vector2 WorldToScreenPoint(Vector3 worldPoint)
    {
        Vector3 vector = this.Camera.WorldToScreenPoint(worldPoint);
        return new Vector2(vector.x, vector.y);
    }

    public UnityEngine.Camera Camera =>
        UI.Camera;

    public static GuiWindow Current
    {
        get
        {
            if (currentWindow == null)
            {
                currentWindow = UnityEngine.Object.FindObjectOfType<GuiWindow>();
            }
            return currentWindow;
        }
        private set
        {
            currentWindow = value;
        }
    }

    public bool Paused
    {
        get => 
            this.isPaused;
        private set
        {
            this.isPaused = value;
        }
    }

    public virtual WindowType Type =>
        WindowType.None;

    public bool Visible =>
        base.gameObject.activeInHierarchy;

    [CompilerGenerated]
    private sealed class <WaitForTime>c__Iterator83 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>time;
        internal GuiWindow <>f__this;
        internal float time;

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
                case 1:
                    if (this.time > 0f)
                    {
                        if (this.<>f__this.Visible)
                        {
                            this.time -= Time.deltaTime;
                        }
                        this.$current = null;
                        this.$PC = 1;
                        return true;
                    }
                    this.$PC = -1;
                    break;
            }
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
}

