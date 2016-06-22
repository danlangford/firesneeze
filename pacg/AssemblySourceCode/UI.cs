using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [CompilerGenerated]
    private static Action <>f__am$cache3C;
    public GuiPanelBusy BusyBox;
    public GuiPanelContinue ContinuePanel;
    public GuiPanelGoldBanner GoldPanel;
    private static readonly string ID = "_UI";
    private static bool isBusy;
    private static bool isHideLoadInProgress;
    private static bool isHideWaitInProgress;
    private static bool isModal;
    private static bool isShowLoadInProgress;
    private static bool isShowWaitInProgress;
    private static bool isZoomed;
    [Header("Global Panels")]
    public Animator LoadingPanel;
    public static readonly float LoadScreenAnimationLength = 1f;
    [Header("Sound Effects")]
    public SoundEffectList MasterSfxList;
    private static UnityEngine.Camera myCamera;
    private static CameraManager myCameraManager;
    private static SoundManager mySoundManager;
    public GuiPanelNetworkTooltip NetworkTooltip;
    public GuiPanel OptionsPanel;
    public GuiPanelSwitch SwitchPanel;
    public GuiPanelTooltipToast Toast;
    public GuiPanelTutorial TutorialPopup;
    [HideInInspector]
    public GuiPanelTutorial TutorialPopupOverlay;
    public GameObject VfxCardBanishFromDiscard;
    public GameObject VfxCardBanishFromDisplay;
    public GameObject VfxCardBanishToBox;
    public GameObject VfxCardDefeatedBanish;
    public GameObject VfxCardLoseEnemy;
    public GameObject VfxCardLoseEnemyAcid;
    public GameObject VfxCardLoseEnemyCold;
    public GameObject VfxCardLoseEnemyElectricity;
    public GameObject VfxCardLoseEnemyFire;
    public GameObject VfxCardLoseEnemyForce;
    public GameObject VfxCardLoseEnemyMeleeBlunt;
    [Tooltip("obsolete")]
    public GameObject VfxCardLoseEnemyMeleePiercing;
    public GameObject VfxCardLoseEnemyMental;
    public GameObject VfxCardLoseEnemyPoison;
    [Tooltip("probably no such thing")]
    public GameObject VfxCardLoseEnemyRangedBlunt;
    [Tooltip("not actually piercing")]
    public GameObject VfxCardLoseEnemyRangedPiercing;
    public GameObject VfxCardSalvage;
    public GameObject VfxCardSummonFromBox;
    public GameObject VfxCardUndefeated;
    public GameObject VfxCardUndefeatedGhost;
    [Header("Visual Effects")]
    public GameObject VfxCardWinBoon;
    public GameObject VfxCardWinBoonDeck;
    public GameObject VfxCardWinBoonSetAside;
    public GameObject VfxCardWinEnemeyLiquid;
    public GameObject VfxCardWinEnemy;
    public GameObject VfxCardWinEnemyAcid;
    public GameObject VfxCardWinEnemyCold;
    public GameObject VfxCardWinEnemyElectricity;
    public GameObject VfxCardWinEnemyFire;
    public GameObject VfxCardWinEnemyForce;
    public GameObject VfxCardWinEnemyMeleeBlunt;
    public GameObject VfxCardWinEnemyMental;
    public GameObject VfxCardWinEnemyPoison;
    public GameObject VfxCardWinEnemyRangedPiercing;
    public GuiPanelVoucher VoucherPanel;
    public Animator WaitPanel;
    public static readonly float WaitScreenAnimationLength = 0.6f;

    private void Awake()
    {
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        this.SwitchPanel.Initialize();
        this.MasterSfxList.Initialize();
    }

    public AudioClip GetSfx(SoundEffectType type)
    {
        AudioClip sfx = this.MasterSfxList.GetSfx(type);
        if (sfx != null)
        {
            return sfx;
        }
        return null;
    }

    private GameObject GetVfx(VisualEffectType type)
    {
        if (type == VisualEffectType.CardWinBoon)
        {
            return this.VfxCardWinBoon;
        }
        if (type == VisualEffectType.CardWinBoonDeck)
        {
            return this.VfxCardWinBoonDeck;
        }
        if (type == VisualEffectType.CardWinBoonSetAside)
        {
            return this.VfxCardWinBoonSetAside;
        }
        if (type == VisualEffectType.CardWinEnemy)
        {
            return this.VfxCardWinEnemy;
        }
        if (type == VisualEffectType.CardWinEnemyFire)
        {
            return this.VfxCardWinEnemyFire;
        }
        if (type == VisualEffectType.CardWinEnemyForce)
        {
            return this.VfxCardWinEnemyForce;
        }
        if (type == VisualEffectType.CardWinEnemyCold)
        {
            return this.VfxCardWinEnemyCold;
        }
        if (type == VisualEffectType.CardWinEnemyAcid)
        {
            return this.VfxCardWinEnemyAcid;
        }
        if (type == VisualEffectType.CardWinEnemyPoison)
        {
            return this.VfxCardWinEnemyPoison;
        }
        if (type == VisualEffectType.CardWinEnemyElectricity)
        {
            return this.VfxCardWinEnemyElectricity;
        }
        if (type == VisualEffectType.CardWinEnemyMental)
        {
            return this.VfxCardWinEnemyMental;
        }
        if (type == VisualEffectType.CardWinEnemyMeleeBlunt)
        {
            return this.VfxCardWinEnemyMeleeBlunt;
        }
        if (type == VisualEffectType.CardWinEnemyRangedPiercing)
        {
            return this.VfxCardWinEnemyRangedPiercing;
        }
        if (type == VisualEffectType.CardWinEnemyLiquid)
        {
            return this.VfxCardWinEnemeyLiquid;
        }
        if (type == VisualEffectType.CardLoseEnemy)
        {
            return this.VfxCardLoseEnemy;
        }
        if (type == VisualEffectType.CardLoseEnemyAcid)
        {
            return this.VfxCardLoseEnemyAcid;
        }
        if (type == VisualEffectType.CardLoseEnemyCold)
        {
            return this.VfxCardLoseEnemyCold;
        }
        if (type == VisualEffectType.CardLoseEnemyFire)
        {
            return this.VfxCardLoseEnemyFire;
        }
        if (type == VisualEffectType.CardLoseEnemyElectricity)
        {
            return this.VfxCardLoseEnemyElectricity;
        }
        if (type == VisualEffectType.CardLoseEnemyForce)
        {
            return this.VfxCardLoseEnemyForce;
        }
        if (type == VisualEffectType.CardLoseEnemyMental)
        {
            return this.VfxCardLoseEnemyMental;
        }
        if (type == VisualEffectType.CardLoseEnemyPoison)
        {
            return this.VfxCardLoseEnemyPoison;
        }
        if (type == VisualEffectType.CardLoseEnemyMeleePiercing)
        {
            return this.VfxCardLoseEnemyMeleePiercing;
        }
        if (type == VisualEffectType.CardLoseEnemyMeleeBlunt)
        {
            return this.VfxCardLoseEnemyMeleeBlunt;
        }
        if (type == VisualEffectType.CardLoseEnemyRangedBlunt)
        {
            return this.VfxCardLoseEnemyRangedBlunt;
        }
        if (type == VisualEffectType.CardLoseEnemyRangedPiercing)
        {
            return this.VfxCardLoseEnemyRangedPiercing;
        }
        if (type == VisualEffectType.CardDefeatedBanish)
        {
            return this.VfxCardDefeatedBanish;
        }
        if (type == VisualEffectType.CardUndefeated)
        {
            return this.VfxCardUndefeated;
        }
        if (type == VisualEffectType.CardUndefeatedGhost)
        {
            return this.VfxCardUndefeatedGhost;
        }
        if (type == VisualEffectType.CardBanishToBox)
        {
            return this.VfxCardBanishToBox;
        }
        if (type == VisualEffectType.CardBanishFromDisplay)
        {
            return this.VfxCardBanishFromDisplay;
        }
        if (type == VisualEffectType.CardBanishFromDiscard)
        {
            return this.VfxCardBanishFromDiscard;
        }
        if (type == VisualEffectType.CardSummonFromBox)
        {
            return this.VfxCardSummonFromBox;
        }
        if (type == VisualEffectType.CardSalvage)
        {
            return this.VfxCardSalvage;
        }
        return null;
    }

    [DebuggerHidden]
    private IEnumerator HideLoadScreen_Coroutine() => 
        new <HideLoadScreen_Coroutine>c__Iterator7 { <>f__this = this };

    [DebuggerHidden]
    private IEnumerator HideWaitScreen_Coroutine() => 
        new <HideWaitScreen_Coroutine>c__Iterator5 { <>f__this = this };

    public bool IsLoadScreenVisible() => 
        isShowLoadInProgress;

    private void LoadLevel(string name)
    {
        if (!isShowLoadInProgress)
        {
            GuiWindowLoading.Level = name;
            base.StartCoroutine(this.LoadLevel_Coroutine(name));
        }
    }

    [DebuggerHidden]
    private IEnumerator LoadLevel_Coroutine(string name) => 
        new <LoadLevel_Coroutine>c__Iterator8 { <>f__this = this };

    public static void Lock(float duration)
    {
        Busy = true;
        if (<>f__am$cache3C == null)
        {
            <>f__am$cache3C = (Action) (() => (Busy = false));
        }
        LeanTween.delayedCall(duration, <>f__am$cache3C);
    }

    public static void OnLoadData()
    {
        byte[] buffer;
        if (Game.GetObjectData(ID, out buffer))
        {
            ByteStream stream = new ByteStream(buffer);
            if (stream != null)
            {
                stream.ReadInt();
            }
        }
    }

    public static void OnSaveData()
    {
        ByteStream stream = new ByteStream();
        if (stream != null)
        {
            stream.WriteInt(1);
            Game.SetObjectData(ID, stream.ToArray());
        }
    }

    public static void Reset()
    {
        Busy = false;
        Modal = false;
        Zoomed = false;
    }

    private void ResetVfx(GameObject vfx)
    {
        vfx.SetActive(false);
        vfx.transform.localScale = Vector3.one;
        vfx.transform.rotation = Quaternion.identity;
        vfx.transform.localScale = Vector3.one;
    }

    public static void Shake(int cycles, float distance, float speed)
    {
        CameraManager.Shake(cycles, distance, speed);
    }

    public void ShowAdventureScene()
    {
        this.LoadLevel("adventure");
    }

    public void ShowCollectionScene()
    {
        if (Window.Type != WindowType.Store)
        {
            GuiWindowCollection.ExitScene = Window.Type;
        }
        this.LoadLevel("collection");
    }

    public void ShowCreatePartyScene()
    {
        this.LoadLevel("createparty");
    }

    public void ShowCreditsScene()
    {
        this.LoadLevel("credits");
    }

    public void ShowCutsceneScene()
    {
        this.LoadLevel("cutscene");
    }

    public void ShowGameOverScene()
    {
        this.LoadLevel("death");
    }

    public void ShowLoadScreen(bool isVisible)
    {
        if (isVisible && !isShowLoadInProgress)
        {
            base.StartCoroutine(this.ShowLoadScreen_Coroutine());
        }
        if (!isVisible && !isHideLoadInProgress)
        {
            base.StartCoroutine(this.HideLoadScreen_Coroutine());
        }
    }

    [DebuggerHidden]
    private IEnumerator ShowLoadScreen_Coroutine() => 
        new <ShowLoadScreen_Coroutine>c__Iterator6 { <>f__this = this };

    public void ShowLocationScene(string ID, bool startInMap)
    {
        Location.Destination = ID;
        Location.StartInMap = startInMap;
        this.LoadLevel("location");
    }

    public void ShowMainMenu()
    {
        this.LoadLevel("menu");
    }

    public void ShowQuestScene()
    {
        this.LoadLevel("questmode");
    }

    public void ShowRewardScene()
    {
        this.LoadLevel("rewards");
    }

    public void ShowSelectCardScene()
    {
        GuiWindowSelectCards.ExitScene = Window.Type;
        this.LoadLevel("selectcards");
    }

    public void ShowSetupScene()
    {
        this.LoadLevel("scenario");
    }

    public void ShowStartScene()
    {
        this.LoadLevel("start");
    }

    public void ShowStoreWindow()
    {
        this.ShowStoreWindow(LicenseType.None);
    }

    public void ShowStoreWindow(LicenseType category)
    {
        if (!Game.Network.HasNetworkConnection)
        {
            Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.MustBeConnected);
            Busy = false;
        }
        else if (Game.Network.OutOfDate)
        {
            Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.OutOfDate);
            Busy = false;
        }
        else
        {
            GuiWindowStore.ShowLicenses(category, null);
            GuiWindowStore.ExitScene = Window.Type;
            this.LoadLevel("store");
        }
    }

    public void ShowStoreWindow(string productCode, LicenseType category)
    {
        if (!Game.Network.HasNetworkConnection)
        {
            Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.MustBeConnected);
        }
        else if (Game.Network.OutOfDate)
        {
            Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.OutOfDate);
        }
        else
        {
            GuiWindowStore.ShowLicenses(category, productCode);
            GuiWindowStore.ExitScene = Window.Type;
            this.LoadLevel("store");
        }
    }

    public GameObject ShowVfx(VisualEffectType type, Vector3 position, Quaternion rotation, Vector3 scale, float duration)
    {
        GameObject vfx = this.GetVfx(type);
        if (vfx != null)
        {
            vfx.transform.position = position;
            vfx.transform.rotation = rotation;
            vfx.transform.localScale = scale;
            VisualEffect.Start(vfx);
            if (duration > 0f)
            {
                base.StartCoroutine(this.StopVfxCoroutine(vfx, duration));
            }
        }
        return vfx;
    }

    public void ShowWaitScreen(bool isVisible)
    {
        if (isVisible && !isShowWaitInProgress)
        {
            base.StartCoroutine(this.ShowWaitScreen_Coroutine());
        }
        if (!isVisible && !isHideWaitInProgress)
        {
            base.StartCoroutine(this.HideWaitScreen_Coroutine());
        }
    }

    [DebuggerHidden]
    private IEnumerator ShowWaitScreen_Coroutine() => 
        new <ShowWaitScreen_Coroutine>c__Iterator4 { <>f__this = this };

    [DebuggerHidden]
    private IEnumerator StopVfxCoroutine(GameObject vfx, float delay) => 
        new <StopVfxCoroutine>c__Iterator3 { 
            delay = delay,
            vfx = vfx,
            <$>delay = delay,
            <$>vfx = vfx,
            <>f__this = this
        };

    public static string Text(int num) => 
        StringTableManager.Get("UI", num);

    public static bool Busy
    {
        get => 
            (isBusy || Modal);
        set
        {
            isBusy = value;
        }
    }

    public static UnityEngine.Camera Camera
    {
        get
        {
            if (myCamera == null)
            {
                GameObject obj2 = GameObject.Find("/~UI/Camera/Camera - 2D");
                if (obj2 != null)
                {
                    myCamera = obj2.GetComponent<UnityEngine.Camera>();
                }
            }
            return myCamera;
        }
    }

    public static CameraManager CameraManager
    {
        get
        {
            if (myCameraManager == null)
            {
                GameObject obj2 = GameObject.Find("/~UI/Camera");
                if (obj2 != null)
                {
                    myCameraManager = obj2.GetComponent<CameraManager>();
                }
            }
            return myCameraManager;
        }
    }

    public string Language =>
        "EN";

    public static bool Loading =>
        isShowLoadInProgress;

    public static bool Modal
    {
        get => 
            isModal;
        set
        {
            isModal = value;
        }
    }

    public static SoundManager Sound
    {
        get
        {
            if (mySoundManager == null)
            {
                GameObject obj2 = GameObject.Find("/~UI/Sound");
                if (obj2 != null)
                {
                    mySoundManager = obj2.GetComponent<SoundManager>();
                }
            }
            return mySoundManager;
        }
    }

    public float Width =>
        Vector3.Distance(Camera.ViewportToWorldPoint(Vector3.zero), Camera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)));

    public static GuiWindow Window =>
        GuiWindow.Current;

    public static bool Zoomed
    {
        get => 
            isZoomed;
        set
        {
            isZoomed = value;
        }
    }

    [CompilerGenerated]
    private sealed class <HideLoadScreen_Coroutine>c__Iterator7 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal UI <>f__this;

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
                    UI.isHideLoadInProgress = true;
                    this.<>f__this.LoadingPanel.gameObject.SetActive(true);
                    this.<>f__this.LoadingPanel.enabled = true;
                    this.<>f__this.LoadingPanel.Play("Loading_Popup_Hide");
                    UI.Sound.Play(SoundEffectType.LoadscreenWipeOut);
                    this.$current = new WaitForSeconds(UI.LoadScreenAnimationLength);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.LoadingPanel.enabled = false;
                    this.<>f__this.LoadingPanel.gameObject.SetActive(false);
                    UI.isHideLoadInProgress = false;
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

    [CompilerGenerated]
    private sealed class <HideWaitScreen_Coroutine>c__Iterator5 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal UI <>f__this;

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
                    UI.isHideWaitInProgress = true;
                    this.<>f__this.WaitPanel.gameObject.SetActive(true);
                    this.<>f__this.WaitPanel.Play("LoadingLoc_Hide");
                    this.$current = new WaitForSeconds(UI.WaitScreenAnimationLength);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.WaitPanel.gameObject.SetActive(false);
                    UI.isHideWaitInProgress = false;
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

    [CompilerGenerated]
    private sealed class <LoadLevel_Coroutine>c__Iterator8 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal UI <>f__this;
        internal bool <showNetworkPanel>__0;

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
                    this.<showNetworkPanel>__0 = Game.UI.NetworkTooltip.Visible;
                    if (this.<showNetworkPanel>__0)
                    {
                        Game.UI.NetworkTooltip.Show(false);
                    }
                    UI.isShowLoadInProgress = true;
                    this.<>f__this.LoadingPanel.gameObject.SetActive(true);
                    this.<>f__this.LoadingPanel.enabled = true;
                    this.<>f__this.LoadingPanel.Play("Loading_Popup_Show");
                    UI.Sound.MusicStop();
                    UI.Sound.Play(SoundEffectType.LoadscreenWipeIn);
                    this.$current = new WaitForSeconds(UI.LoadScreenAnimationLength);
                    this.$PC = 1;
                    return true;

                case 1:
                    UI.Window.Close();
                    this.<>f__this.LoadingPanel.enabled = false;
                    SceneManager.LoadScene(Constants.LOADSCREEN_LEVEL_NAME);
                    UI.isShowLoadInProgress = false;
                    if (this.<showNetworkPanel>__0)
                    {
                        Game.UI.NetworkTooltip.Show(true);
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

    [CompilerGenerated]
    private sealed class <ShowLoadScreen_Coroutine>c__Iterator6 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal UI <>f__this;

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
                    UI.isShowLoadInProgress = true;
                    this.<>f__this.LoadingPanel.gameObject.SetActive(true);
                    this.<>f__this.LoadingPanel.enabled = true;
                    this.<>f__this.LoadingPanel.Play("Loading_Popup_Show");
                    UI.Sound.MusicStop();
                    UI.Sound.Play(SoundEffectType.LoadscreenWipeIn);
                    this.$current = new WaitForSeconds(UI.LoadScreenAnimationLength);
                    this.$PC = 1;
                    return true;

                case 1:
                    UI.isShowLoadInProgress = false;
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

    [CompilerGenerated]
    private sealed class <ShowWaitScreen_Coroutine>c__Iterator4 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal UI <>f__this;

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
                    UI.isShowWaitInProgress = true;
                    this.<>f__this.WaitPanel.gameObject.SetActive(true);
                    this.<>f__this.WaitPanel.Play("LoadingLoc_Show");
                    this.$current = new WaitForSeconds(UI.WaitScreenAnimationLength);
                    this.$PC = 1;
                    return true;

                case 1:
                    UI.isShowWaitInProgress = false;
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

    [CompilerGenerated]
    private sealed class <StopVfxCoroutine>c__Iterator3 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>delay;
        internal GameObject <$>vfx;
        internal UI <>f__this;
        internal float delay;
        internal GameObject vfx;

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
                    this.$current = new WaitForSeconds(this.delay);
                    this.$PC = 1;
                    goto Label_0083;

                case 1:
                    VisualEffect.Stop(this.vfx);
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 2;
                    goto Label_0083;

                case 2:
                    this.<>f__this.ResetVfx(this.vfx);
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0083:
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

