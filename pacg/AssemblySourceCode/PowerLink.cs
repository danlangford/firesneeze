using System;
using UnityEngine;

public class PowerLink : MonoBehaviour
{
    [Tooltip("the ID of the linked power")]
    public string ID;

    public void Activate(Power power)
    {
        if (!Turn.IsPowerActive(this.ID))
        {
            Transform transform = power.transform.parent.FindChild(this.ID);
            if (transform != null)
            {
                Power component = transform.GetComponent<Power>();
                if (component != null)
                {
                    Turn.MarkPowerActive(component, true);
                }
            }
        }
    }

    public void Deactivate(Power power)
    {
        if (Turn.IsPowerActive(this.ID))
        {
            Transform transform = power.transform.parent.FindChild(this.ID);
            if (transform != null)
            {
                Power component = transform.GetComponent<Power>();
                if (component != null)
                {
                    Turn.MarkPowerActive(component, false);
                }
            }
        }
    }
}

