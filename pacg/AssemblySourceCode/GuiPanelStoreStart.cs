using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class GuiPanelStoreStart : GuiPanelBackStack
{
    [Tooltip("reference to the scroll region in this panel")]
    public GuiScrollRegion Carousel;
    private GoldButtonFunctionality currentButtonFunctionality;
    private LicenseOperationType currentOperation;
    [Tooltip("reference to the blue daily gold button in this panel")]
    public GuiButton DailyGoldButton;
    [Tooltip("reference to the daily gold button sparkles particle object")]
    public GameObject DailySparkle;
    [Tooltip("reference to the larger daily gold button sparkles particle object")]
    public GameObject DailySparkleLarge;
    [Tooltip("left margin of items in scroll list (half of the item width)")]
    public float listMarginLeft;
    [Tooltip("top margin of items in the scroll list (half of the item height)")]
    public float listMarginTop;
    private float m_pickupRequestTimer;
    [Tooltip("number of carousel items")]
    public int numberOfItems;
    private const float PickupRequestTime = 5f;
    private bool recentlyRefreshedLicenses;
    [Tooltip("reference to the restore button in this panel")]
    public GuiButton RestoreButton;
    [Tooltip("width of each carousel item")]
    public float specialWidth = 7f;
    [Tooltip("reference to the store manager window in the scene")]
    public GuiWindowStore StoreManager;
    private TKTapRecognizer tapRecognizer;

    public static void CalculateGoldCountdown(ref int hours, ref int minutes, ref int seconds)
    {
        long num = DateTime.Now.ToFileTimeUtc();
        long goldResetTimeSet = Game.Network.CurrentUser.GoldResetTimeSet;
        DateTime time = new DateTime(num - goldResetTimeSet);
        int num4 = (int) (Game.Network.CurrentUser.GoldSubTimeTillReset * 60.0);
        int num5 = (((time.Hour * 0x18) * 60) + (time.Minute * 60)) + time.Second;
        int num6 = num4 - num5;
        hours = num6 / 0xe10;
        num6 -= hours * 0xe10;
        minutes = num6 / 60;
        num6 -= minutes * 60;
        seconds = num6;
    }

    public override void Clear()
    {
        base.Clear();
    }

    public static string GetGoldButtonCountdownText(int sumHours, int sumMins, int sumSecs)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Daily Gold");
        if (sumHours < 0)
        {
            sumHours = 0;
        }
        if (sumMins < 0)
        {
            sumMins = 0;
        }
        if (sumSecs < 0)
        {
            sumSecs = 0;
        }
        string str = sumHours + string.Empty;
        if (sumHours < 10)
        {
            str = "0" + str;
        }
        string str2 = sumMins + string.Empty;
        if (sumMins < 10)
        {
            str2 = "0" + sumMins;
        }
        string str3 = sumSecs + string.Empty;
        if (sumSecs < 10)
        {
            str3 = "0" + str3;
        }
        builder.Append(str + ":" + str2 + ":" + str3);
        return builder.ToString();
    }

    public override void Initialize()
    {
        this.Carousel.Initialize();
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.zIndex = 3;
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
        this.Show(false);
    }

    private void OnDailyGoldButtonPushed()
    {
        if (!UI.Busy)
        {
            if (!Game.Network.Connected)
            {
                Game.UI.NetworkTooltip.ShowMessage(GuiPanelNetworkTooltip.MessageType.MustBeLoggedIn);
            }
            else if ((this.currentButtonFunctionality == GoldButtonFunctionality.PickUp) && (this.m_pickupRequestTimer <= 0f))
            {
                this.m_pickupRequestTimer = 5f;
                Game.Network.RedeemGoldSubscription();
            }
            else if (this.currentButtonFunctionality == GoldButtonFunctionality.Subscribe)
            {
                GuiPanelStoreGold.ShowLicense(Constants.IAP_LICENSE_GOLD_SUBSCRIPTION_TIER1);
                this.StoreManager.overlayPanel.ButtonGlow(GuiWindowStore.StorePanelType.Gold);
                this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Gold);
            }
        }
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (!UI.Busy)
        {
            Vector2 origin = base.ScreenToWorldPoint(touchPos);
            if (!this.Carousel.Contains((Vector3) origin) || (Physics2D.Raycast(origin, Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_ICON) == 0))
            {
            }
        }
    }

    private void OnPageButtonPushed()
    {
        if (!UI.Busy)
        {
        }
    }

    private void OnRestoreButtonPushed()
    {
        if (!UI.Busy && (Application.platform == RuntimePlatform.IPhonePlayer))
        {
            UI.Busy = true;
            this.currentOperation = LicenseOperationType.Restore;
            LicenseManager.RestoreLicenses();
            Game.UI.Toast.Show("Now restoring!");
        }
    }

    private void OnSpecialsButtonPushed()
    {
        this.StoreManager.overlayPanel.ExternalSpecialsButton();
    }

    public override void Refresh()
    {
        this.Clear();
        if (Game.Network.Connected && (Game.Network.CurrentUser.GoldSubDaysRemaining > 0))
        {
            if (Game.Network.CurrentUser.GoldSubAvailable)
            {
                this.DailySparkle.SetActive(true);
                this.DailySparkleLarge.SetActive(true);
                this.currentButtonFunctionality = GoldButtonFunctionality.PickUp;
                this.DailyGoldButton.Text = "Pick up Daily Gold!";
                this.DailyGoldButton.Disable(false);
            }
            else
            {
                this.DailySparkle.SetActive(false);
                this.DailySparkleLarge.SetActive(false);
                this.currentButtonFunctionality = GoldButtonFunctionality.Timer;
                this.DailyGoldButton.Disable(true);
            }
        }
        else
        {
            this.DailySparkle.SetActive(true);
            this.DailySparkleLarge.SetActive(true);
            this.currentButtonFunctionality = GoldButtonFunctionality.Subscribe;
            this.DailyGoldButton.Text = "Sign Up for Daily Gold!";
            this.DailyGoldButton.Disable(false);
        }
        float num = this.listMarginLeft + (this.specialWidth * this.numberOfItems);
        float x = (this.Carousel.Min.x + num) - this.Carousel.Size.x;
        this.Carousel.Max = new Vector2(x, 0f);
        this.Carousel.Top();
        AlertManager.HandleAlerts();
    }

    [DebuggerHidden]
    private IEnumerator RefreshDailyGoldData() => 
        new <RefreshDailyGoldData>c__Iterator71 { <>f__this = this };

    private void SetGoldButtonCountdown()
    {
        int hours = 0;
        int minutes = 0;
        int seconds = 0;
        CalculateGoldCountdown(ref hours, ref minutes, ref seconds);
        if (hours < 0)
        {
            hours = 0;
        }
        if (minutes < 0)
        {
            minutes = 0;
        }
        if (seconds < 0)
        {
            seconds = 0;
        }
        this.DailyGoldButton.Text = GetGoldButtonCountdownText(hours, minutes, seconds);
        if (((hours == 0) && (minutes == 0)) && ((seconds == 0) && !this.recentlyRefreshedLicenses))
        {
            base.StartCoroutine(this.RefreshDailyGoldData());
        }
    }

    private void SetGoldButtonPickUp()
    {
        this.DailyGoldButton.Text = "Pick up Daily Gold!";
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.currentOperation = LicenseOperationType.None;
        this.Carousel.Pause(true);
        this.tapRecognizer.enabled = isVisible;
        if (isVisible)
        {
            this.Refresh();
            this.RestoreButton.Show(Application.platform == RuntimePlatform.IPhonePlayer);
            this.m_pickupRequestTimer = 0f;
            AlertManager.SeenAlert(AlertManager.AlertType.SeenStoreStart);
        }
        else
        {
            this.Clear();
            UI.Busy = false;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (this.m_pickupRequestTimer > 0f)
        {
            this.m_pickupRequestTimer -= Time.deltaTime;
        }
        if (this.currentButtonFunctionality == GoldButtonFunctionality.Timer)
        {
            this.SetGoldButtonCountdown();
        }
        else if (this.currentButtonFunctionality == GoldButtonFunctionality.PickUp)
        {
            this.SetGoldButtonPickUp();
        }
        if ((this.currentOperation != LicenseOperationType.None) && (LicenseManager.GetResult(this.currentOperation) != null))
        {
            this.currentOperation = LicenseOperationType.None;
            UI.Busy = false;
        }
    }

    [CompilerGenerated]
    private sealed class <RefreshDailyGoldData>c__Iterator71 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelStoreStart <>f__this;

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
                    this.<>f__this.recentlyRefreshedLicenses = true;
                    Game.Network.RefreshUserData(NetworkManager.UserField.Licenses);
                    this.$current = new WaitForSeconds(15f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.recentlyRefreshedLicenses = false;
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

    public enum GoldButtonFunctionality
    {
        Subscribe,
        PickUp,
        Timer
    }
}

