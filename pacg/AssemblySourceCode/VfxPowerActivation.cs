using System;
using UnityEngine;

public class VfxPowerActivation : MonoBehaviour
{
    [Tooltip("list of all renderers that should display the power icon")]
    public SpriteRenderer[] Icons;

    public void Show(bool isVisible)
    {
        base.gameObject.SetActive(isVisible);
        if (isVisible)
        {
            base.GetComponent<Animator>().SetTrigger("Show");
        }
    }

    public Sprite Icon
    {
        set
        {
            for (int i = 0; i < this.Icons.Length; i++)
            {
                this.Icons[i].sprite = value;
            }
        }
    }
}

