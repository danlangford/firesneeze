using System;
using UnityEngine;

public class ScrollingUVs : MonoBehaviour
{
    public int materialIndex;
    public string textureName = "_MainTex";
    public Vector2 uvAnimationRate = new Vector2(1f, 0f);
    private Vector2 uvOffset = Vector2.zero;

    private void LateUpdate()
    {
        this.uvOffset += (Vector2) (this.uvAnimationRate * Time.deltaTime);
        if (base.GetComponent<Renderer>().enabled)
        {
            base.GetComponent<Renderer>().materials[this.materialIndex].SetTextureOffset(this.textureName, this.uvOffset);
        }
    }
}

