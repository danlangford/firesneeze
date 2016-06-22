using System;
using UnityEngine;

public class GuiAlignmentCollider : MonoBehaviour
{
    [Tooltip("align our collider's left edge with this point")]
    public Transform Left;
    [Tooltip("align our collider's right edge with this point")]
    public Transform Right;

    private int GetClosestPoint(Vector2[] points, Vector2 point)
    {
        float maxValue = float.MaxValue;
        int num2 = 0;
        for (int i = 1; i < points.Length; i++)
        {
            float num4 = Vector2.SqrMagnitude(points[i] - point);
            if (num4 < maxValue)
            {
                maxValue = num4;
                num2 = i;
            }
        }
        return num2;
    }

    public void Start()
    {
        BoxCollider2D component = base.GetComponent<BoxCollider2D>();
        if (component != null)
        {
            float x = Mathf.Abs((float) (this.Left.position.x - this.Right.position.x));
            component.size = new Vector2(x, component.size.y);
            x = (this.Left.position.x + ((this.Right.position.x - this.Left.position.x) / 2f)) - base.transform.position.x;
            component.offset = new Vector2(x, component.offset.y);
        }
        else
        {
            PolygonCollider2D colliderd2 = base.GetComponent<PolygonCollider2D>();
            if (colliderd2 != null)
            {
                Vector2[] points = colliderd2.points;
                int closestPoint = this.GetClosestPoint(points, this.Left.position);
                points[closestPoint] = new Vector2(this.Left.position.x, points[closestPoint].y);
                closestPoint = this.GetClosestPoint(points, this.Right.position);
                points[closestPoint] = new Vector2(this.Right.position.x, points[closestPoint].y);
                colliderd2.points = points;
            }
        }
    }
}

