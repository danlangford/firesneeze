using System;
using UnityEngine;

public class GuiPanelMenuNetwork : GuiPanel
{
    [Tooltip("reference to the sprite that shows we have network connection")]
    public Sprite HasNetworkConnectionSprite;
    [Tooltip("reference to the login bonus")]
    public GameObject LoginBonus;
    [Tooltip("reference to the network connectivity sprite renderer in this scene")]
    public SpriteRenderer NetworkConnectivitySR;
    [Tooltip("reference to the sprite that shows we don't have network connection")]
    public Sprite NoNetworkConnectionSprite;

    public override void Initialize()
    {
        this.Show(true);
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
    }

    private void Update()
    {
        if (this.NetworkConnectivitySR != null)
        {
            if (!this.NetworkConnectivitySR.gameObject.activeInHierarchy)
            {
                this.NetworkConnectivitySR.gameObject.SetActive(false);
            }
            this.NetworkConnectivitySR.sprite = !Game.Network.HasNetworkConnection ? this.NoNetworkConnectionSprite : this.HasNetworkConnectionSprite;
        }
        if (this.LoginBonus.activeInHierarchy)
        {
            this.LoginBonus.gameObject.SetActive(false);
        }
    }
}

