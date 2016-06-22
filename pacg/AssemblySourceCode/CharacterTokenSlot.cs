using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterTokenSlot : MonoBehaviour
{
    [Tooltip("amount of tween bounce")]
    public float Bounce;
    [Tooltip("set to false if this slot should not tween on drop")]
    public bool DropAnimations = true;
    public AudioClip DropSound;
    private bool isLocked;
    [Tooltip("only allow a certain token ID in this slot")]
    public CharacterToken Owner;
    [Tooltip("true if the token shadow should be moved on drop")]
    public bool Shadows;
    [Tooltip("position within the Party")]
    public int Slot;
    [Tooltip("scale of tokens in this slot")]
    public float TokenScale = 1.5f;

    private void Awake()
    {
        if (this.Owner != null)
        {
            this.Owner.Home = this;
        }
    }

    public virtual bool OnDrop(CharacterToken token)
    {
        if (this.Locked)
        {
            return false;
        }
        if (token == null)
        {
            return false;
        }
        if ((this.Owner != null) && (token != this.Owner))
        {
            return false;
        }
        UI.Sound.Play(this.DropSound);
        if (token.Slot != null)
        {
            token.Slot.Token = null;
        }
        this.Token = token;
        return true;
    }

    public virtual void Refresh()
    {
    }

    public virtual void Show(bool isVisible)
    {
        base.gameObject.SetActive(isVisible);
    }

    public CharacterTokenSlotLayout Layout { get; set; }

    public bool Locked
    {
        get => 
            this.isLocked;
        set
        {
            this.isLocked = value;
        }
    }

    public Vector3 Position =>
        base.transform.position;

    public virtual Vector3 Scale =>
        new Vector3(this.TokenScale, this.TokenScale, 1f);

    public CharacterToken Token { get; set; }
}

