using System;
using UnityEngine;

public class AdventureMapIcon : MonoBehaviour
{
    [Tooltip("icon displayed when the adventure is available for play")]
    public Sprite AvailableIcon;
    [Tooltip("sound played when this icon is touched")]
    public AudioClip ClickSound;
    [Tooltip("icon displayed when the adventure is completed")]
    public Sprite CompletedIcon;
    [Tooltip("the adventure ID represented by this icon")]
    public string ID;
    private bool isBusy;
    private Adventure myAdventure;
    [Tooltip("icon displayed when the adventure is purchased")]
    public Sprite PurchasedIcon;
    [Tooltip("icon displayed when the adventure is not supported")]
    public Sprite UnavailableIcon;
    [Tooltip("icon displayed when the adventure is unpurchased")]
    public Sprite UnpurchasedIcon;

    public float AnimateUnlock(Animator anim)
    {
        Campaign.SetAdventureUnlocked(this.ID, false);
        anim.transform.position = base.transform.position;
        anim.SetTrigger("Show");
        return 3f;
    }

    private void Awake()
    {
        this.myAdventure = AdventureTable.Create(this.ID);
        if (this.myAdventure != null)
        {
            this.myAdventure.transform.parent = base.transform;
        }
    }

    public void FadeIn(float time)
    {
        SpriteRenderer componentInChildren = base.GetComponentInChildren<SpriteRenderer>();
        if (componentInChildren != null)
        {
            componentInChildren.color = new Color(componentInChildren.color.r, componentInChildren.color.g, componentInChildren.color.b, 0f);
            LeanTween.alpha(componentInChildren.gameObject, 1f, time).setEase(LeanTweenType.easeInOutBounce);
        }
    }

    public void Refresh()
    {
        if (this.myAdventure != null)
        {
            SpriteRenderer component = base.GetComponent<SpriteRenderer>();
            if (component != null)
            {
                if (!this.myAdventure.Supported)
                {
                    component.sprite = this.UnavailableIcon;
                }
                else if (!this.myAdventure.Purchased)
                {
                    component.sprite = this.UnpurchasedIcon;
                }
                else if (this.myAdventure.Completed)
                {
                    component.sprite = this.CompletedIcon;
                }
                else if (this.myAdventure.Available)
                {
                    component.sprite = this.AvailableIcon;
                }
                else if (this.myAdventure.Purchased)
                {
                    component.sprite = this.PurchasedIcon;
                }
            }
        }
    }

    private void ResetBusyFlag()
    {
        this.isBusy = false;
    }

    public void Show(bool isVisible)
    {
        SpriteRenderer componentInChildren = base.GetComponentInChildren<SpriteRenderer>();
        componentInChildren.color = new Color(componentInChildren.color.r, componentInChildren.color.g, componentInChildren.color.b, 0f);
    }

    private void Start()
    {
        this.Refresh();
        this.Show(false);
        this.FadeIn(0.3f);
        Geometry.SetLayerRecursively(base.gameObject, Constants.LAYER_MAP);
    }

    public void Tap()
    {
        if (!this.isBusy)
        {
            this.isBusy = true;
            Vector3 to = new Vector3(1.15f * base.transform.localScale.x, 1.15f * base.transform.localScale.y, 1f);
            LeanTween.scale(base.gameObject, to, 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.ResetBusyFlag));
            UI.Sound.Play(this.ClickSound);
        }
    }

    public Adventure Adventure =>
        this.myAdventure;
}

