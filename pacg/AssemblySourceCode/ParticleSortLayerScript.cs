using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem)), RequireComponent(typeof(SpriteRenderer))]
public class ParticleSortLayerScript : MonoBehaviour
{
    private void Start()
    {
        SpriteRenderer component = base.GetComponent<SpriteRenderer>();
        if (component != null)
        {
            base.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerID = component.sortingLayerID;
            base.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = component.sortingOrder;
        }
    }
}

