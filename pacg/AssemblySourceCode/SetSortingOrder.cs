using System;
using UnityEngine;

public class SetSortingOrder : MonoBehaviour
{
    [SortingLayer]
    public int SortingLayer = 1;
    public int SortingOrder = 1;

    private void Awake()
    {
        if (base.GetComponent<Renderer>() != null)
        {
            base.GetComponent<Renderer>().sortingOrder = this.SortingOrder;
            base.GetComponent<Renderer>().sortingLayerID = this.SortingLayer;
        }
    }
}

