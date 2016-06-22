using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiLayoutTrayCounter : MonoBehaviour
{
    [Tooltip("reference to the textmesh background in my hierarchy")]
    public SpriteRenderer Background;
    private int number;
    [Tooltip("reference to textmesh in my hierarchy")]
    public TextMesh Text;

    public void Fade(bool isVisible, float time)
    {
        if (isVisible)
        {
            this.Background.color = new Color(this.Background.color.r, this.Background.color.g, this.Background.color.b, 0f);
            this.Text.GetComponent<Renderer>().material.color = new Color(this.Text.color.r, this.Text.color.g, this.Text.color.b, 0f);
            LeanTween.alpha(this.Background.gameObject, 1f, time);
            LeanTween.alpha(this.Text.gameObject, 1f, time);
        }
        else
        {
            LeanTween.alpha(this.Background.gameObject, 0f, time);
            LeanTween.alpha(this.Text.gameObject, 0f, time);
        }
    }

    public string ID { get; set; }

    public int Number
    {
        get => 
            this.number;
        set
        {
            this.number = value;
            if (this.number > 1)
            {
                this.Text.text = this.number.ToString();
            }
        }
    }

    public bool Rendered { get; set; }

    public int SortingOrder
    {
        get => 
            this.Text.GetComponent<Renderer>().sortingOrder;
        set
        {
            this.Text.GetComponent<Renderer>().sortingOrder = value;
            this.Background.sortingOrder = value - 1;
        }
    }
}

