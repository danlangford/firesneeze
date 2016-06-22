using System;
using UnityEngine;

public class QuestMapIcon : MonoBehaviour
{
    [Tooltip("sound played when this icon is touched")]
    public AudioClip ClickSound;
    [Tooltip("the difficulty for this quest")]
    public int Difficulty;
    [Tooltip("the adventure ID represented by this icon")]
    public string ID;
    private bool isBusy;

    public void FadeIn(float time)
    {
        SpriteRenderer componentInChildren = base.GetComponentInChildren<SpriteRenderer>();
        if (componentInChildren != null)
        {
            componentInChildren.color = new Color(componentInChildren.color.r, componentInChildren.color.g, componentInChildren.color.b, 0f);
            LeanTween.alpha(componentInChildren.gameObject, 1f, time).setEase(LeanTweenType.easeInOutBounce);
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
}

