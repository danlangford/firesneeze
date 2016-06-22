using System;
using UnityEngine;
using UnityEngine.Rendering;

public class TextOutline : MonoBehaviour
{
    public Color outlineColor = Color.black;
    public float pixelSize = 1f;

    private Vector3 GetOffset(int i)
    {
        switch ((i % 8))
        {
            case 0:
                return new Vector3(0f, 1f, 0f);

            case 1:
                return new Vector3(1f, 1f, 0f);

            case 2:
                return new Vector3(1f, 0f, 0f);

            case 3:
                return new Vector3(1f, -1.5f, 0f);

            case 4:
                return new Vector3(0f, -1.5f, 0f);

            case 5:
                return new Vector3(-1f, -1.5f, 0f);

            case 6:
                return new Vector3(-1f, 0f, 0f);

            case 7:
                return new Vector3(-1f, 1f, 0f);
        }
        return Vector3.zero;
    }

    public void Initialize(GuiLabel label)
    {
        MeshRenderer component = label.GetComponent<MeshRenderer>();
        for (int i = 0; i < 8; i++)
        {
            System.Type[] components = new System.Type[] { typeof(TextMesh) };
            MeshRenderer renderer2 = new GameObject("outline", components) { transform = { 
                parent = base.transform,
                localScale = new Vector3(1f, 1f, 1f)
            } }.GetComponent<MeshRenderer>();
            renderer2.material = new Material(component.material);
            renderer2.shadowCastingMode = ShadowCastingMode.Off;
            renderer2.receiveShadows = false;
            renderer2.sortingLayerID = component.sortingLayerID;
            renderer2.sortingLayerName = component.sortingLayerName;
        }
    }

    public void Refresh(GuiLabel label)
    {
        TextMesh component = label.GetComponent<TextMesh>();
        Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
        this.outlineColor.a = component.color.a * component.color.a;
        for (int i = 0; i < base.transform.childCount; i++)
        {
            TextMesh mesh2 = base.transform.GetChild(i).GetComponent<TextMesh>();
            mesh2.color = this.outlineColor;
            mesh2.text = component.text;
            mesh2.alignment = component.alignment;
            mesh2.anchor = component.anchor;
            mesh2.characterSize = component.characterSize;
            mesh2.font = component.font;
            mesh2.fontSize = component.fontSize;
            mesh2.fontStyle = component.fontStyle;
            mesh2.richText = component.richText;
            mesh2.tabSize = component.tabSize;
            mesh2.lineSpacing = component.lineSpacing;
            mesh2.offsetZ = component.offsetZ;
            Vector3 vector2 = (Vector3) (this.GetOffset(i) * this.pixelSize);
            Vector3 vector3 = Camera.main.ScreenToWorldPoint(vector + vector2);
            mesh2.transform.position = vector3;
            MeshRenderer renderer = base.transform.GetChild(i).GetComponent<MeshRenderer>();
            renderer.sortingOrder = label.SortingOrder - 1;
            renderer.sortingLayerID = label.SortingLayer;
        }
    }
}

