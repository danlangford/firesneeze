using System;
using UnityEngine;

public class VfxButtonGlow : MonoBehaviour
{
    private static void ApplyGlowToButton(GameObject glow, GuiButton button)
    {
        SpriteRenderer componentInChildren = button.GetComponentInChildren<SpriteRenderer>();
        if (componentInChildren != null)
        {
            VisualEffect.Sorting(glow, componentInChildren.sortingLayerID, componentInChildren.sortingOrder);
            SpriteRenderer[] componentsInChildren = glow.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].sortingLayerID = componentInChildren.sortingLayerID;
                componentsInChildren[i].sortingOrder = componentInChildren.sortingOrder;
            }
        }
        glow.transform.parent = button.transform;
        glow.transform.localPosition = Vector3.zero;
        glow.transform.localScale = Vector3.one;
    }

    public static GameObject ApplyToButton(GuiButton button)
    {
        GameObject original = Resources.Load<GameObject>("Art/VFX/vfx_ButtonGeneric_Highlight");
        if (original != null)
        {
            GameObject glow = UnityEngine.Object.Instantiate(original, button.transform.position, Quaternion.identity) as GameObject;
            if (glow != null)
            {
                ApplyGlowToButton(glow, button);
                return glow;
            }
        }
        return null;
    }

    public static GameObject ApplyToPortrait(GuiButton button)
    {
        GameObject original = Resources.Load<GameObject>("Art/VFX/vfx_CharacterCircle_Highlight");
        if (original != null)
        {
            GameObject glow = UnityEngine.Object.Instantiate(original, button.transform.position, Quaternion.identity) as GameObject;
            if (glow != null)
            {
                ApplyGlowToButton(glow, button);
                return glow;
            }
        }
        return null;
    }

    public static GameObject ApplyToSelect(GuiButton button)
    {
        GameObject original = Resources.Load<GameObject>("Art/VFX/vfx_ButtonCardSelect_Highlight");
        if (original != null)
        {
            GameObject glow = UnityEngine.Object.Instantiate(original, button.transform.position, Quaternion.identity) as GameObject;
            if (glow != null)
            {
                ApplyGlowToButton(glow, button);
                return glow;
            }
        }
        return null;
    }

    public static GameObject ApplyToTab(GuiButton button)
    {
        GameObject original = Resources.Load<GameObject>("Art/VFX/vfx_Tab_Highlight");
        if (original != null)
        {
            GameObject glow = UnityEngine.Object.Instantiate(original, button.transform.position, Quaternion.identity) as GameObject;
            if (glow != null)
            {
                ApplyGlowToButton(glow, button);
                return glow;
            }
        }
        return null;
    }
}

