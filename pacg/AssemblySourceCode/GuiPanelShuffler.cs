using System;
using UnityEngine;

public class GuiPanelShuffler : GuiPanel
{
    [Tooltip("references to the card back children in this animation")]
    public GameObject[] Cards;
    [Tooltip("sound played when shuffling")]
    public AudioClip ShuffleSound;

    public float Shuffle(int numCards)
    {
        Animator component = base.GetComponent<Animator>();
        if (component != null)
        {
            for (int i = 0; i < this.Cards.Length; i++)
            {
                this.Cards[i].SetActive(i < numCards);
            }
            component.SetTrigger("Start");
            UI.Sound.Play(this.ShuffleSound);
        }
        return 1.5f;
    }

    public int SortingOrder
    {
        set
        {
            for (int i = 0; i < this.Cards.Length; i++)
            {
                SpriteRenderer componentInChildren = this.Cards[i].GetComponentInChildren<SpriteRenderer>();
                if (componentInChildren != null)
                {
                    componentInChildren.sortingOrder = value;
                }
            }
        }
    }
}

