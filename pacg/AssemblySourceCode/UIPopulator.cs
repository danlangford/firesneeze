using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPopulator : MonoBehaviour
{
    protected List<GameObject> m_Clones = new List<GameObject>();
    [Tooltip("The object to parent the clones to (default: attached object).")]
    public GameObject ParentObject;
    [Tooltip("The object to clone.")]
    public GameObject RootPopulatedObject;

    protected UIPopulator()
    {
    }

    protected virtual GameObject ActivateClone(int index)
    {
        GameObject obj2 = null;
        if ((index < this.m_Clones.Count) && (this.m_Clones[index] != null))
        {
            obj2 = this.m_Clones[index];
        }
        else
        {
            obj2 = this.AddClone(index, this.RootPopulatedObject, this.ParentObject);
        }
        if (obj2 != null)
        {
            obj2.SetActive(true);
        }
        return obj2;
    }

    protected GameObject AddClone(int index, GameObject prefab, GameObject parent)
    {
        GameObject clone = UnityEngine.Object.Instantiate<GameObject>(prefab);
        clone.layer = parent.layer;
        clone.transform.SetParent(parent.transform, false);
        if (this.ResetTransform)
        {
            clone.transform.localPosition = prefab.transform.localPosition;
            clone.transform.localScale = prefab.transform.localScale;
        }
        clone.name = clone.name + index.ToString("D3");
        while (this.m_Clones.Count < index)
        {
            this.m_Clones.Add(null);
        }
        this.InitClone(clone);
        this.m_Clones.Insert(index, clone);
        return clone;
    }

    protected virtual void Awake()
    {
        this.OnValidate();
        this.RootPopulatedObject.SetActive(true);
        this.RootPopulatedObject.SetActive(false);
    }

    public GameObject GetCloneWithComponent<T>(IEquatable<T> component) where T: Component
    {
        foreach (GameObject obj2 in this.m_Clones)
        {
            T other = obj2.GetComponent<T>();
            if ((other != null) && component.Equals(other))
            {
                return obj2;
            }
        }
        return null;
    }

    protected virtual void InitClone(GameObject clone)
    {
    }

    public abstract void Load(GameObject go);
    protected void OnValidate()
    {
        if (this.ParentObject == null)
        {
            this.ParentObject = base.gameObject;
        }
    }

    protected void Populate(int items)
    {
        this.OnValidate();
        int index = 0;
        while (index < items)
        {
            this.ActivateClone(index);
            index++;
        }
        while (index < this.m_Clones.Count)
        {
            if (this.m_Clones[index] != null)
            {
                this.m_Clones[index].SetActive(false);
            }
            index++;
        }
    }

    protected virtual void RemoveClone(int index)
    {
        if ((index < this.m_Clones.Count) && (this.m_Clones[index] != null))
        {
            UnityEngine.Object.Destroy(this.m_Clones[index]);
            this.m_Clones.RemoveAt(index);
        }
    }

    public bool IsEmpty =>
        ((this.m_Clones.Count == 0) || !this.m_Clones[0].activeSelf);

    protected virtual bool ResetTransform =>
        true;
}

