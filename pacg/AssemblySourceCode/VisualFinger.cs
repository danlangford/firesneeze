using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class VisualFinger : MonoBehaviour
{
    private void Awake()
    {
        if (this.IsFingerSupported())
        {
            UnityEngine.Object.DontDestroyOnLoad(this);
        }
        else
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    private bool IsFingerSupported() => 
        false;

    private void Update()
    {
        Vector3 vector = UI.Camera.ScreenToWorldPoint(Input.mousePosition);
        base.transform.position = new Vector3(vector.x, vector.y, 0f);
    }
}

