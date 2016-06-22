using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DecorationManager
{
    private const string BackHolder = "Back/Art";
    private Dictionary<string, GameObject> Decorations;
    private const string FrontHolder = "Front/Art";

    public GameObject Add(string decoration, CardSideType side, string path, float scale)
    {
        if (this.IsDecorationPossible(decoration))
        {
            if (this.Decorations.ContainsKey(decoration))
            {
                return this.Decorations[decoration];
            }
            GameObject obj2 = Game.Cache.Checkout(decoration);
            if (obj2 != null)
            {
                obj2.transform.parent = null;
                obj2.transform.localScale = Vector3.one;
                obj2.transform.parent = this.GetDecorationPath(this.Owner.transform, side, path);
                obj2.transform.localPosition = Vector3.zero;
                obj2.transform.localScale = new Vector3(1f + scale, 1f + scale, 1f);
                this.Decorations.Add(decoration, obj2);
                return obj2;
            }
        }
        return null;
    }

    public void Clear()
    {
        if (this.Decorations != null)
        {
            List<string> list = new List<string>(this.Decorations.Keys);
            foreach (string str in list)
            {
                this.Remove(str);
            }
            this.Decorations.Clear();
            this.Decorations = null;
        }
    }

    public bool Contains(string decoration) => 
        ((this.Decorations != null) && this.Decorations.ContainsKey(decoration));

    public GameObject Get(string decoration)
    {
        if ((this.Decorations != null) && this.Decorations.ContainsKey(decoration))
        {
            return this.Decorations[decoration];
        }
        return null;
    }

    private Transform GetDecorationPath(Transform root, CardSideType side, string parent)
    {
        Transform transform = null;
        string name = "Front/Art";
        if (side == CardSideType.Back)
        {
            name = "Back/Art";
        }
        if (!string.IsNullOrEmpty(parent))
        {
            name = name + "/" + parent;
        }
        transform = root.FindChild(name);
        if (transform != null)
        {
            return transform;
        }
        return root;
    }

    private bool IsDecorationPossible(string decoration)
    {
        if (string.IsNullOrEmpty(decoration))
        {
            return false;
        }
        if (this.Owner == null)
        {
            return false;
        }
        if (this.Decorations == null)
        {
            this.Decorations = new Dictionary<string, GameObject>(10);
        }
        return (this.Decorations != null);
    }

    public void Remove(string decoration)
    {
        if (this.IsDecorationPossible(decoration) && this.Decorations.ContainsKey(decoration))
        {
            Game.Cache.Checkin(decoration, this.Decorations[decoration]);
            this.Decorations.Remove(decoration);
        }
    }

    public GameObject Owner { get; set; }
}

