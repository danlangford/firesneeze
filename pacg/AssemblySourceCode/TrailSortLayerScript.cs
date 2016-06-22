using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(TrailRenderer))]
public class TrailSortLayerScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;

    private void Start()
    {
        this.spriteRenderer = base.GetComponent<SpriteRenderer>();
        this.trailRenderer = base.GetComponent<TrailRenderer>();
        if ((this.spriteRenderer != null) && (this.trailRenderer != null))
        {
            this.trailRenderer.sortingLayerName = this.spriteRenderer.sortingLayerName;
        }
    }

    private void Update()
    {
        if ((this.spriteRenderer != null) && (this.trailRenderer != null))
        {
            this.trailRenderer.sortingOrder = this.spriteRenderer.sortingOrder - 1;
        }
    }
}

