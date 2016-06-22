using System;
using UnityEngine;

public abstract class GuiElement : MonoBehaviour
{
    private Vector3 startPosition;

    protected GuiElement()
    {
    }

    protected virtual void Awake()
    {
        this.startPosition = base.transform.position;
    }

    public AudioClip GetResourceSfx(int n)
    {
        GuiResource component = base.GetComponent<GuiResource>();
        if (((component != null) && (n >= 0)) && (n < component.Sounds.Length))
        {
            return component.Sounds[n];
        }
        return null;
    }

    public Sprite GetResourceSprite(int n)
    {
        GuiResource component = base.GetComponent<GuiResource>();
        if (((component != null) && (n >= 0)) && (n < component.Sprites.Length))
        {
            return component.Sprites[n];
        }
        return null;
    }

    public virtual void Refresh()
    {
    }

    public virtual void Show(bool isVisible)
    {
        base.gameObject.SetActive(isVisible);
    }

    protected virtual void Start()
    {
    }

    public Vector3 StartPosition =>
        this.startPosition;

    public virtual bool Visible =>
        base.gameObject.activeInHierarchy;
}

