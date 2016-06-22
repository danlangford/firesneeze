using System;
using UnityEngine;

public abstract class Prize : MonoBehaviour
{
    protected Prize()
    {
    }

    public virtual void Deliver()
    {
    }

    protected GameObject GetRewardPanel(string panelName)
    {
        GameObject obj2 = GameObject.Find("/Animations");
        if (obj2 != null)
        {
            for (int i = 0; i < obj2.transform.childCount; i++)
            {
                Transform child = obj2.transform.GetChild(i);
                if ((child != null) && (child.name == panelName))
                {
                    return child.gameObject;
                }
            }
        }
        return null;
    }

    public virtual bool HasPrize() => 
        false;

    public virtual bool IsPrizeAllowed() => 
        true;
}

