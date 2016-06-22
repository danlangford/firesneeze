using System;
using UnityEngine;

public class GuiAlignment : MonoBehaviour
{
    [Tooltip("align in this direction")]
    public ScreenAlignmentType Alignment;
    [Tooltip("... with this pixel offset")]
    public Vector2 Offset = Vector2.zero;
    [Tooltip("... with this percentage offset")]
    public Vector2 OffsetPercent = Vector2.zero;
    private static float screenHeight;
    private static float screenWidth;

    private void Awake()
    {
        if (screenWidth == 0f)
        {
            screenWidth = this.GetScreenWidth();
        }
        if (screenHeight == 0f)
        {
            screenHeight = this.GetScreenHeight();
        }
        float num = this.Offset.x + (this.OffsetPercent.x * screenWidth);
        float num2 = this.Offset.y + (this.OffsetPercent.y * screenHeight);
        if (this.Alignment == ScreenAlignmentType.Left)
        {
            Vector3 vector = UI.Camera.ViewportToWorldPoint(Vector3.zero);
            base.transform.position = new Vector3(vector.x + num, base.transform.position.y + num2, base.transform.position.z);
        }
        else if (this.Alignment == ScreenAlignmentType.Right)
        {
            Vector3 vector2 = UI.Camera.ViewportToWorldPoint(Vector3.one);
            base.transform.position = new Vector3(vector2.x + num, base.transform.position.y + num2, base.transform.position.z);
        }
        else if (this.Alignment == ScreenAlignmentType.Top)
        {
            Vector3 vector3 = UI.Camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
            base.transform.position = new Vector3(base.transform.position.x + num, vector3.y + num2, base.transform.position.z);
        }
        else if (this.Alignment == ScreenAlignmentType.TopLeft)
        {
            Vector3 vector4 = UI.Camera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f));
            base.transform.position = new Vector3(vector4.x + num, vector4.y + num2, base.transform.position.z);
        }
        else if (this.Alignment == ScreenAlignmentType.TopRight)
        {
            Vector3 vector5 = UI.Camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
            base.transform.position = new Vector3(vector5.x + num, vector5.y + num2, base.transform.position.z);
        }
        else if (this.Alignment == ScreenAlignmentType.Bottom)
        {
            Vector3 vector6 = UI.Camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
            base.transform.position = new Vector3(base.transform.position.x + num, vector6.y + num2, base.transform.position.z);
        }
        else if (this.Alignment == ScreenAlignmentType.BottomLeft)
        {
            Vector3 vector7 = UI.Camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
            base.transform.position = new Vector3(vector7.x + num, vector7.y + num2, base.transform.position.z);
        }
        else if (this.Alignment == ScreenAlignmentType.BottomRight)
        {
            Vector3 vector8 = UI.Camera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f));
            base.transform.position = new Vector3(vector8.x + num, vector8.y + num2, base.transform.position.z);
        }
        else if (this.Alignment == ScreenAlignmentType.Center)
        {
            Vector3 vector9 = UI.Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            base.transform.position = new Vector3(vector9.x + num, vector9.y + num2, base.transform.position.z);
        }
    }

    private float GetScreenHeight()
    {
        Vector3 vector = UI.Camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 vector2 = UI.Camera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f));
        return Mathf.Abs((float) (vector.y - vector2.y));
    }

    private float GetScreenWidth()
    {
        Vector3 vector = UI.Camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 vector2 = UI.Camera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f));
        return Mathf.Abs((float) (vector.x - vector2.x));
    }

    public void Refresh()
    {
        this.Awake();
    }
}

