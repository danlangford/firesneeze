using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class VisualEffect
{
    public static GameObject Apply(Card card, VisualEffectType vfx)
    {
        if (card != null)
        {
            string vfxBlueprintName = GetVfxBlueprintName(vfx);
            if ((vfxBlueprintName != null) && !Has(card, vfx))
            {
                Transform vfxRoot = GetVfxRoot(card);
                if (vfxRoot != null)
                {
                    GameObject prefab = Resources.Load<GameObject>("Art/VFX/" + vfxBlueprintName);
                    if (prefab != null)
                    {
                        GameObject obj3 = Game.Instance.Create(prefab);
                        if (obj3 != null)
                        {
                            obj3.transform.parent = vfxRoot;
                            obj3.transform.localPosition = Vector3.zero;
                            obj3.transform.localScale = Vector3.one;
                            return obj3;
                        }
                    }
                }
            }
        }
        return null;
    }

    public static GameObject ApplyToCard(VisualEffectType vfx, Card card, float duration)
    {
        GameObject obj2 = null;
        if (card != null)
        {
            obj2 = Game.UI.ShowVfx(vfx, card.transform.position, card.transform.rotation, card.transform.localScale, duration);
        }
        return obj2;
    }

    public static GameObject ApplyToPlayer(VisualEffectType vfx, float duration)
    {
        Vector3 position = new Vector3(0f, -4f, 0f);
        return Game.UI.ShowVfx(vfx, position, Quaternion.identity, Vector3.one, duration);
    }

    public static GameObject ApplyToScreen(VisualEffectType vfx, float duration)
    {
        Vector3 zero = Vector3.zero;
        return Game.UI.ShowVfx(vfx, zero, Quaternion.identity, Vector3.one, duration);
    }

    public static void Fade(GameObject vfx, float alpha, float time)
    {
        SpriteRenderer[] componentsInChildren = vfx.GetComponentsInChildren<SpriteRenderer>(true);
        if (alpha >= 1f)
        {
            for (int k = 0; k < componentsInChildren.Length; k++)
            {
                LeanTween.alpha(componentsInChildren[k].gameObject, 0f, 0.01f);
            }
        }
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            LeanTween.alpha(componentsInChildren[i].gameObject, alpha, time);
        }
        ParticleSystem[] systemArray = vfx.GetComponentsInChildren<ParticleSystem>(true);
        for (int j = 0; j < systemArray.Length; j++)
        {
            systemArray[j].emission.enabled = alpha > 0f;
        }
    }

    private static string GetVfxBlueprintName(VisualEffectType vfx)
    {
        if (vfx == VisualEffectType.GlowSpecial)
        {
            return "Hilite_Card_Special";
        }
        if (vfx == VisualEffectType.CardRestrictStart)
        {
            return "Vfx_Card_Restricted_Start";
        }
        if (vfx == VisualEffectType.CardRestrictStop)
        {
            return "Vfx_Card_Restricted_End";
        }
        return null;
    }

    private static Transform GetVfxRoot(Card card)
    {
        if (card != null)
        {
            return card.transform.FindChild("Front");
        }
        return null;
    }

    private static bool Has(Card card, VisualEffectType vfx)
    {
        if (card != null)
        {
            string vfxBlueprintName = GetVfxBlueprintName(vfx);
            if (vfxBlueprintName != null)
            {
                Transform vfxRoot = GetVfxRoot(card);
                if (vfxRoot != null)
                {
                    return (vfxRoot.FindChild(vfxBlueprintName) != null);
                }
            }
        }
        return false;
    }

    public static void Remove(Card card, VisualEffectType vfx)
    {
        if (card != null)
        {
            string vfxBlueprintName = GetVfxBlueprintName(vfx);
            if (vfxBlueprintName != null)
            {
                Transform vfxRoot = GetVfxRoot(card);
                if (vfxRoot != null)
                {
                    Transform transform2 = vfxRoot.FindChild(vfxBlueprintName);
                    if (transform2 != null)
                    {
                        UnityEngine.Object.Destroy(transform2.gameObject);
                    }
                }
            }
        }
    }

    public static void Shuffle(DeckType deck)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (deck == DeckType.Location)
            {
                window.layoutExplore.Shuffle(Location.Current.Deck.Count);
            }
            if (deck == DeckType.Character)
            {
                window.layoutRecharge.Shuffle(Turn.Character.Deck.Count);
            }
        }
    }

    public static void Shuffle(float delay, DeckType deck)
    {
        <Shuffle>c__AnonStorey129 storey = new <Shuffle>c__AnonStorey129 {
            deck = deck
        };
        LeanTween.delayedCall(delay, new Action(storey.<>m__16A));
    }

    public static void Sorting(GameObject vfx, int layer, int order)
    {
        ParticleSystemRenderer[] componentsInChildren = vfx.GetComponentsInChildren<ParticleSystemRenderer>(true);
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            componentsInChildren[i].sortingLayerID = layer;
            componentsInChildren[i].sortingOrder = order;
        }
    }

    public static void Start(GameObject vfx)
    {
        if (vfx != null)
        {
            vfx.SetActive(true);
            ParticleSystem[] componentsInChildren = vfx.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].emission.enabled = true;
                componentsInChildren[i].Play();
            }
        }
    }

    public static void Stop(GameObject vfx)
    {
        if (vfx != null)
        {
            ParticleSystem[] componentsInChildren = vfx.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].emission.enabled = false;
            }
        }
    }

    public static void Warmup(GameObject vfx)
    {
        if (vfx != null)
        {
            ParticleSystem[] componentsInChildren = vfx.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].Simulate(0.25f);
            }
        }
    }

    [CompilerGenerated]
    private sealed class <Shuffle>c__AnonStorey129
    {
        internal DeckType deck;

        internal void <>m__16A()
        {
            VisualEffect.Shuffle(this.deck);
        }
    }
}

