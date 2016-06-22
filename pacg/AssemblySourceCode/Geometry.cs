using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Geometry
{
    public static void AddChild(GameObject parent, GameObject child)
    {
        child.transform.parent = parent.transform;
        child.transform.localScale = Vector3.one;
        child.transform.localPosition = Vector3.zero;
    }

    public static float ConvertScreenDistanceToWorldDistance(float d)
    {
        Vector3 a = UI.Camera.ScreenToWorldPoint(Vector3.zero);
        Vector3 b = UI.Camera.ScreenToWorldPoint(Vector3.right);
        float num = Vector3.Distance(a, b);
        return (d * num);
    }

    public static GameObject CreateChild(GameObject parent, string name)
    {
        GameObject obj2 = new GameObject();
        if (obj2 != null)
        {
            obj2.name = name;
            obj2.transform.parent = parent.transform;
            obj2.transform.localPosition = Vector3.zero;
            obj2.transform.localRotation = Quaternion.identity;
        }
        return obj2;
    }

    public static GameObject CreateChildObject(GameObject parent, GameObject prefab, string name)
    {
        if (prefab != null)
        {
            GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(prefab);
            if (obj2 != null)
            {
                obj2.name = name;
                obj2.transform.parent = parent.transform;
                obj2.transform.localPosition = Vector3.zero;
                obj2.transform.localRotation = Quaternion.identity;
                return obj2;
            }
        }
        return null;
    }

    public static GameObject CreateObject(GameObject prefab, string name)
    {
        if (prefab != null)
        {
            GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(prefab);
            if (obj2 != null)
            {
                obj2.name = name;
                obj2.transform.localPosition = Vector3.zero;
                obj2.transform.localRotation = Quaternion.identity;
                return obj2;
            }
        }
        return null;
    }

    public static void DestroyAllChildren(GameObject parent)
    {
        if (parent != null)
        {
            for (int i = parent.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.transform.GetChild(i);
                if (child != null)
                {
                    UnityEngine.Object.Destroy(child.gameObject);
                }
            }
        }
    }

    private static float Det2(float x1, float x2, float y1, float y2) => 
        ((x1 * y2) - (y1 * x2));

    public static Transform GetChild(Transform t, string name)
    {
        if (t != null)
        {
            foreach (Transform transform in t.GetComponentsInChildren(typeof(Transform), true))
            {
                if (transform.gameObject.name == name)
                {
                    return transform;
                }
            }
        }
        return null;
    }

    public static TKRect GetColliderScreenBounds(BoxCollider2D bc)
    {
        if (bc != null)
        {
            Vector3 vector = bc.transform.position + new Vector3(bc.offset.x * bc.transform.localScale.x, bc.offset.y * bc.transform.localScale.y, 0f);
            Vector3 position = new Vector3(vector.x - ((bc.size.x / 2f) * bc.transform.localScale.x), vector.y - ((bc.size.y / 2f) * bc.transform.localScale.y));
            Vector3 vector3 = new Vector3(vector.x + ((bc.size.x / 2f) * bc.transform.localScale.x), vector.y + ((bc.size.y / 2f) * bc.transform.localScale.y));
            Vector3 vector4 = UI.Camera.WorldToScreenPoint(position);
            Vector3 vector5 = UI.Camera.WorldToScreenPoint(vector3);
            return new TKRect(vector4.x, vector4.y, vector5.x - vector4.x, vector5.y - vector4.y);
        }
        return new TKRect();
    }

    public static Vector3[] GetCurve(Vector3 start, Vector3 end, float h = 0)
    {
        Vector3[] vectorArray = new Vector3[4];
        vectorArray[0] = start;
        vectorArray[3] = end;
        float num = Vector3.Distance(start, end);
        if (num < 0.25f)
        {
            vectorArray[1] = start;
            vectorArray[2] = end;
        }
        if (h <= 0f)
        {
            h = num / 8f;
        }
        Vector3 vector = new Vector3((0.66f * start.x) + (0.33f * end.x), (0.66f * start.y) + (0.33f * end.y), start.z);
        Vector3 vector2 = new Vector3((0.33f * start.x) + (0.66f * end.x), (0.33f * start.y) + (0.66f * end.y), start.z);
        float num2 = Mathf.Abs((float) (end.x - start.x));
        float num3 = Mathf.Abs((float) (end.y - start.y));
        if (num2 >= num3)
        {
            vectorArray[1] = new Vector3(vector.x, vector.y + h, vector.z);
            vectorArray[2] = new Vector3(vector2.x, vector2.y + h, vector2.z);
            return vectorArray;
        }
        vectorArray[1] = new Vector3(vector.x + h, vector.y + h, vector.z);
        vectorArray[2] = new Vector3(vector2.x + h, vector2.y + h, vector2.z);
        return vectorArray;
    }

    public static Vector2 GetLineIntersectionPoint(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        float num = 1E-06f;
        float f = Det2(v1.x - v2.x, v1.y - v2.y, v3.x - v4.x, v3.y - v4.y);
        if (Mathf.Abs(f) >= Mathf.Epsilon)
        {
            float num3 = Det2(v1.x, v1.y, v2.x, v2.y);
            float num4 = Det2(v3.x, v3.y, v4.x, v4.y);
            float x = Det2(num3, v1.x - v2.x, num4, v3.x - v4.x) / f;
            float y = Det2(num3, v1.y - v2.y, num4, v3.y - v4.y) / f;
            if ((x < (Mathf.Min(v1.x, v2.x) - num)) || (x > (Mathf.Max(v1.x, v2.x) + num)))
            {
                return Vector2.zero;
            }
            if ((y < (Mathf.Min(v1.y, v2.y) - num)) || (y > (Mathf.Max(v1.y, v2.y) + num)))
            {
                return Vector2.zero;
            }
            if ((x < (Mathf.Min(v3.x, v4.x) - num)) || (x > (Mathf.Max(v3.x, v4.x) + num)))
            {
                return Vector2.zero;
            }
            if ((y >= (Mathf.Min(v3.y, v4.y) - num)) && (y <= (Mathf.Max(v3.y, v4.y) + num)))
            {
                return new Vector2(x, y);
            }
        }
        return Vector2.zero;
    }

    public static float GetMidPoint(int n)
    {
        if (n == 0)
        {
            return 0f;
        }
        return (((float) (n - 1)) / 2f);
    }

    public static float GetPanDistance(float leftMargin, float width, float rightMargin, Transform first, Vector2 deltaTranslation, Transform last)
    {
        float num = ConvertScreenDistanceToWorldDistance(deltaTranslation.x);
        Vector3 position = new Vector3(first.position.x - (width / 2f), first.position.y, first.position.z);
        Vector3 vector2 = new Vector3(last.position.x + (width / 2f), last.position.y, last.position.z);
        Vector3 vector3 = UI.Camera.WorldToViewportPoint(position);
        Vector3 vector4 = UI.Camera.WorldToViewportPoint(vector2);
        float num2 = 0.1f;
        if ((vector3.x > leftMargin) && (vector4.x < rightMargin))
        {
            return 0f;
        }
        if (num > 0f)
        {
            if (vector3.x <= leftMargin)
            {
                return num;
            }
            return ((vector3.x <= (leftMargin + num2)) ? (0.1f * num) : 0f);
        }
        if ((num < 0f) && (vector4.x < rightMargin))
        {
            return ((vector4.x >= (rightMargin - num2)) ? (0.1f * num) : 0f);
        }
        return num;
    }

    public static float GetPanStopDistance(float leftMargin, float width, float rightMargin, Transform first, Vector2 deltaTranslation, Transform last)
    {
        float num = ConvertScreenDistanceToWorldDistance(deltaTranslation.x);
        Vector3 position = new Vector3(first.position.x - (width / 2f), first.position.y, first.position.z);
        Vector3 vector2 = new Vector3(last.position.x + (width / 2f), last.position.y, last.position.z);
        Vector3 vector3 = UI.Camera.WorldToViewportPoint(position);
        Vector3 vector4 = UI.Camera.WorldToViewportPoint(vector2);
        if ((vector3.x > leftMargin) && (vector4.x < rightMargin))
        {
            return 0f;
        }
        if (num < 0f)
        {
            if ((vector4.x >= rightMargin) || (vector3.x >= leftMargin))
            {
                return num;
            }
            return (UI.Camera.ViewportToWorldPoint(new Vector3(rightMargin, 0f, 0f)).x - vector2.x);
        }
        if (((num > 0f) && (vector4.x > rightMargin)) && (vector3.x > leftMargin))
        {
            return (UI.Camera.ViewportToWorldPoint(new Vector3(leftMargin, 0f, 0f)).x - position.x);
        }
        return num;
    }

    public static T GetSubComponent<T>(MonoBehaviour self) where T: Component
    {
        T component = null;
        int childCount = self.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = self.transform.GetChild(i);
            if (child != null)
            {
                component = child.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
        }
        return component;
    }

    public static float GetTweenTime(Vector3 start, Vector3 end, float time)
    {
        float min = time / 2f;
        float max = time * 2f;
        float num3 = Vector3.Distance(start, end);
        if (num3 >= 5f)
        {
            time += 0.05f * num3;
        }
        return Mathf.Clamp(time, min, max);
    }

    public static bool IsNear(GameObject go1, GameObject go2)
    {
        float num = 0.1f;
        float num2 = Mathf.Abs((float) (go1.transform.position.x - go2.transform.position.x));
        float num3 = Mathf.Abs((float) (go1.transform.position.y - go2.transform.position.y));
        return ((num2 < num) && (num3 < num));
    }

    public static Vector2 ScreenToWorldPoint(Vector2 screenPoint)
    {
        Vector3 vector = UI.Camera.ScreenToWorldPoint((Vector3) screenPoint);
        return new Vector2(vector.x, vector.y);
    }

    public static void SetLayerRecursively(GameObject go, int newLayer)
    {
        if (go != null)
        {
            go.layer = newLayer;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                SetLayerRecursively(go.transform.GetChild(i).gameObject, newLayer);
            }
        }
    }

    public static void SetLayerRecursively(GameObject go, string newLayerName, int newLayerOrder)
    {
        if (go != null)
        {
            SpriteRenderer component = go.GetComponent<SpriteRenderer>();
            if (component != null)
            {
                component.sortingLayerName = newLayerName;
                component.sortingOrder = newLayerOrder;
            }
            for (int i = 0; i < go.transform.childCount; i++)
            {
                SetLayerRecursively(go.transform.GetChild(i).gameObject, newLayerName, newLayerOrder);
            }
        }
    }

    public static Vector2 WorldToScreenPoint(Vector3 worldPoint)
    {
        Vector3 vector = UI.Camera.WorldToScreenPoint(worldPoint);
        return new Vector2(vector.x, vector.y);
    }
}

