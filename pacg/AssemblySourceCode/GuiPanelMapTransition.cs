using System;
using UnityEngine;

public class GuiPanelMapTransition : GuiPanel
{
    [Tooltip("reference to the animator in our hierarchy")]
    public UnityEngine.Animator Animator;
    [Tooltip("reference to the location icon sprite in our hierarchy")]
    public SpriteRenderer Emblem;

    private void CenterEmblem(ScenarioMapIcon icon)
    {
        if (icon != null)
        {
            Transform parent = this.Emblem.transform.parent;
            parent.position = new Vector3(icon.transform.position.x, icon.transform.position.y, parent.position.z);
            Vector3 to = UI.Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            to = new Vector3(to.x, to.y, parent.position.z);
            LeanTween.move(parent.gameObject, to, 0.5f).setEase(LeanTweenType.easeInBack);
        }
    }

    public override void Initialize()
    {
        this.Show(true);
    }

    private void LoadEmblem(string id)
    {
        string path = "Art/MapEmblems/" + id;
        this.Emblem.sprite = Resources.Load<Sprite>(path);
    }

    public void ZoomIn(ScenarioMapIcon icon)
    {
        if (icon != null)
        {
            this.LoadEmblem(icon.ID);
            this.CenterEmblem(icon);
            this.Animator.SetTrigger("Start");
        }
    }

    public void ZoomOut(ScenarioMapIcon icon)
    {
        if (icon != null)
        {
            this.Animator.SetTrigger("Loaded");
        }
    }
}

