using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class GuiGrid : MonoBehaviour
{
    [Tooltip("the direction of the grid")]
    public GridDirections Direction;
    [Tooltip("height of each object")]
    public float Height;
    [Tooltip("should account for deactivated objs")]
    public bool IncludeDeactivatedObjs;
    [Tooltip("offset of first obj")]
    public Vector2 InitialOffset;
    private int totalObjects;
    [Tooltip("width of each object")]
    public float Width;

    private void Refresh()
    {
        this.totalObjects = 0;
        Vector2 zero = Vector2.zero;
        Vector2 initialOffset = this.InitialOffset;
        switch (this.Direction)
        {
            case GridDirections.Down:
            case GridDirections.Vertical:
                zero.y = 1f;
                break;

            case GridDirections.Up:
                zero.y = -1f;
                break;

            case GridDirections.Left:
                zero.x = -1f;
                break;

            case GridDirections.Right:
            case GridDirections.Horizontal:
                zero.x = 1f;
                break;
        }
        if ((this.Direction == GridDirections.Horizontal) || (this.Direction == GridDirections.Vertical))
        {
            IEnumerator enumerator = base.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (this.IncludeDeactivatedObjs || current.gameObject.activeInHierarchy)
                    {
                        this.totalObjects++;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            if (this.Direction == GridDirections.Vertical)
            {
                initialOffset += new Vector2(0f, this.TotalHeight / 2f);
            }
            if (this.Direction == GridDirections.Horizontal)
            {
                initialOffset += new Vector2(-this.TotalWidth / 2f, 0f);
            }
        }
        zero = this.Reposition(zero, initialOffset);
    }

    private Vector2 Reposition(Vector2 direction, Vector2 initialOffset)
    {
        this.totalObjects = 0;
        IEnumerator enumerator = base.transform.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform) enumerator.Current;
                if (this.IncludeDeactivatedObjs || current.gameObject.activeInHierarchy)
                {
                    current.localPosition = new Vector3(initialOffset.x + ((this.totalObjects * this.Width) * direction.x), initialOffset.y + ((this.totalObjects * this.Height) * direction.y), current.localPosition.z);
                    this.totalObjects++;
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
        return direction;
    }

    public void Show(bool visible)
    {
        base.gameObject.SetActive(visible);
        this.Refresh();
    }

    public float ActualHeight =>
        Mathf.Abs((float) (base.transform.childCount * this.Height));

    public float TotalHeight =>
        Mathf.Abs((float) (this.totalObjects * this.Height));

    public float TotalWidth =>
        Mathf.Abs((float) (this.totalObjects * this.Width));
}

