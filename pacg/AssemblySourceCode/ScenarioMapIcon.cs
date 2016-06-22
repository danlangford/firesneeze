using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public class ScenarioMapIcon : MonoBehaviour
{
    [Tooltip("sound played when this icon is touched")]
    public AudioClip ClickSound;
    public Sprite ClosedIcon;
    public Sprite ClosedImpossibleIcon;
    public Sprite ClosedTempIcon;
    [Tooltip("the location ID represented by this icon")]
    public string ID;
    private bool isBusy;
    public Sprite LockedIcon;
    private GameObject myDecoration;
    public Sprite OpenIcon;
    private Vector3 startOffset;
    private Vector3 startPosition;
    private Vector3 startScale;

    private void AddDecoration(string prefabName)
    {
        if ((this.myDecoration == null) || (this.myDecoration.name != prefabName))
        {
            if (this.myDecoration != null)
            {
                UnityEngine.Object.Destroy(this.myDecoration);
            }
            GameObject original = Resources.Load<GameObject>("Blueprints/Gui/" + prefabName);
            if (original != null)
            {
                this.myDecoration = UnityEngine.Object.Instantiate(original, base.transform.position, Quaternion.identity) as GameObject;
                if (this.myDecoration != null)
                {
                    this.myDecoration.name = original.name;
                    this.myDecoration.transform.parent = base.transform.parent;
                    this.myDecoration.GetComponent<Animator>().SetTrigger("Glow");
                }
            }
        }
    }

    private void ClearCharacterIcons()
    {
        Transform transform = base.transform.FindChild("Heads");
        if (transform != null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child != null)
                {
                    CharacterTokenMap component = child.GetComponent<CharacterTokenMap>();
                    if (component != null)
                    {
                        component.Clear();
                    }
                }
            }
        }
    }

    private int CountCharactersAtLocation(string locID)
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if ((Party.Characters[i].Location == locID) && Party.Characters[i].Alive)
            {
                num++;
            }
        }
        return num;
    }

    private CharacterTokenMap CreateCharacterToken(Character character)
    {
        Transform transform = base.transform.FindChild("Heads");
        if (transform != null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child != null)
                {
                    CharacterTokenMap component = child.GetComponent<CharacterTokenMap>();
                    if ((component != null) && component.Empty)
                    {
                        component.Character = character;
                        component.Interactive = false;
                        return component;
                    }
                }
            }
        }
        return null;
    }

    public void Face(bool isVisible)
    {
        Transform transform = base.transform.FindChild("Arrow/MapArrow");
        if (transform != null)
        {
            transform.gameObject.SetActive(isVisible);
        }
    }

    public void Face(Vector3 direction)
    {
        Transform transform = base.transform.FindChild("Arrow/MapArrow");
        if (transform != null)
        {
            transform.gameObject.SetActive(true);
            Vector3 vector = direction - transform.position;
            float angle = (Mathf.Atan2(vector.y, vector.x) * 57.29578f) - 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.localPosition = this.GetArrowIconPosition(direction);
        }
    }

    public void Fade(float alpha, float duration)
    {
        LeanTween.alpha(base.gameObject, alpha, duration);
        Transform transform = base.transform.FindChild("Heads");
        if (transform != null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child != null)
                {
                    LeanTween.alpha(child.gameObject, alpha, duration);
                }
            }
        }
        Transform transform3 = base.transform.FindChild("Shadow/Shadow1");
        if (transform3 != null)
        {
            LeanTween.alpha(transform3.gameObject, alpha, duration);
        }
        Transform transform4 = base.transform.FindChild("Shadow/Shadow2");
        if (transform4 != null)
        {
            LeanTween.alpha(transform4.gameObject, alpha, duration);
        }
        Transform transform5 = base.transform.FindChild("Arrow/MapArrow");
        if (transform5 != null)
        {
            LeanTween.alpha(transform5.gameObject, alpha, duration);
        }
    }

    public void FadeIn(float time)
    {
        SpriteRenderer component = base.GetComponent<SpriteRenderer>();
        if (component != null)
        {
            component.color = new Color(component.color.r, component.color.g, component.color.b, 0f);
            LeanTween.alpha(component.gameObject, 1f, time).setEase(LeanTweenType.easeInOutBounce);
        }
    }

    public GameObject Flash()
    {
        string path = "Art/VFX/vfx_location_updated";
        GameObject original = Resources.Load<GameObject>(path);
        if (original != null)
        {
            GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
            if (obj3 != null)
            {
                obj3.transform.position = base.transform.position;
                return obj3;
            }
        }
        return null;
    }

    private Vector3 GetArrowIconPosition(Vector3 direction)
    {
        float num = 0.75f;
        float num2 = Mathf.Sqrt(Mathf.Pow(direction.x - base.transform.position.x, 2f) + Mathf.Pow(direction.y - base.transform.position.y, 2f));
        float num3 = num * (direction.x - base.transform.position.x);
        float num4 = num * (direction.y - base.transform.position.y);
        return new Vector3(num3 / num2, num4 / num2, -0.01f);
    }

    private Vector3 GetCharacterIconPosition(int index, int max)
    {
        float num = 0.575f;
        float characterIconSpacing = this.GetCharacterIconSpacing(Party.Characters.Count);
        float num3 = (characterIconSpacing * (max - 1)) / 2f;
        float num4 = (180f - num3) + ((index - 1) * characterIconSpacing);
        float x = num * Mathf.Sin(num4 * 0.01745329f);
        return new Vector3(x, num * Mathf.Cos(num4 * 0.01745329f), -0.01f);
    }

    private float GetCharacterIconSpacing(int partySize)
    {
        if (partySize <= 4)
        {
            return 75f;
        }
        if (partySize == 5)
        {
            return 65f;
        }
        return 60f;
    }

    public CharacterTokenMap GetCharacterToken(Character character)
    {
        Transform transform = base.transform.FindChild("Heads");
        if (transform != null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child != null)
                {
                    CharacterTokenMap component = child.GetComponent<CharacterTokenMap>();
                    if ((component != null) && (component.ID == character.ID))
                    {
                        return component;
                    }
                }
            }
        }
        return null;
    }

    public void Initialize()
    {
        this.startPosition = base.transform.position;
        this.startScale = base.transform.localScale;
        this.startOffset = base.transform.localPosition;
        Geometry.SetLayerRecursively(base.gameObject, Constants.LAYER_MAP);
        this.Decorations = true;
        this.Face(false);
        this.Show(false);
    }

    [DebuggerHidden]
    public IEnumerator Move(Character character) => 
        new <Move>c__IteratorA2 { 
            character = character,
            <$>character = character,
            <>f__this = this
        };

    private void PaintCharacterIcons(bool isAnimated)
    {
        this.ClearCharacterIcons();
        int max = this.CountCharactersAtLocation(this.ID);
        int num2 = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            CharacterTokenMap map = this.CreateCharacterToken(Party.Characters[i]);
            if (map != null)
            {
                if ((Party.Characters[i].Location == this.ID) && Party.Characters[i].Alive)
                {
                    map.Show(true);
                    if (isAnimated)
                    {
                        LeanTween.moveLocal(map.gameObject, this.GetCharacterIconPosition(++num2, max), 0.2f).setEase(LeanTweenType.easeInOutQuad);
                    }
                    else
                    {
                        map.transform.localPosition = this.GetCharacterIconPosition(++num2, max);
                    }
                }
                else
                {
                    map.Show(false);
                }
            }
        }
    }

    private void PaintDecorationIcons(bool isAnimated)
    {
        if ((isAnimated && this.Decorations) && ((Scenario.Current.GetLocationCloseType(this.ID) == CloseType.Permanent) && (Scenario.Current.GetCardCount(this.ID) > 0)))
        {
            if (Scenario.Current.GetCardCount(this.ID, CardType.Villain) > 0)
            {
                this.AddDecoration("Decoration_MapIcon_ClosedVillain");
            }
            else
            {
                this.AddDecoration("Decoration_MapIcon_ClosedLoot");
            }
        }
        else if (this.myDecoration != null)
        {
            UnityEngine.Object.Destroy(this.myDecoration);
        }
    }

    public void Refresh(bool isAnimated)
    {
        SpriteRenderer component = base.GetComponent<SpriteRenderer>();
        if (component != null)
        {
            switch (Scenario.Current.GetLocationCloseType(this.ID))
            {
                case CloseType.None:
                    component.sprite = this.OpenIcon;
                    break;

                case CloseType.Permanent:
                    component.sprite = this.ClosedIcon;
                    break;

                case CloseType.Temporary:
                    component.sprite = this.ClosedTempIcon;
                    break;

                case CloseType.Impossible:
                    component.sprite = this.ClosedImpossibleIcon;
                    break;
            }
            if (this.Locked)
            {
                component.sprite = this.LockedIcon;
            }
        }
        this.PaintDecorationIcons(isAnimated);
        this.PaintCharacterIcons(isAnimated);
    }

    private void Reset()
    {
        this.isBusy = false;
    }

    public void Show(bool isVisible)
    {
        SpriteRenderer component = base.GetComponent<SpriteRenderer>();
        component.color = new Color(component.color.r, component.color.g, component.color.b, 0f);
    }

    public void Tap()
    {
        if (!this.isBusy)
        {
            this.isBusy = true;
            UI.Sound.Play(this.ClickSound);
            LeanTween.scale(base.gameObject, new Vector3(1.15f, 1.15f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.Reset));
        }
    }

    public bool Decorations { get; set; }

    public bool Locked { get; set; }

    public Vector2 Size =>
        base.GetComponent<Renderer>().bounds.size;

    public int SortingOrder
    {
        set
        {
            SpriteRenderer component = base.GetComponent<SpriteRenderer>();
            if (component != null)
            {
                component.sortingOrder = value;
            }
            Transform transform = base.transform.FindChild("Heads");
            if (transform != null)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child != null)
                    {
                        component = child.GetComponent<SpriteRenderer>();
                        if (component != null)
                        {
                            component.sortingOrder = value + 2;
                        }
                    }
                }
            }
        }
    }

    public Vector3 StartOffset =>
        this.startOffset;

    public Vector3 StartPosition =>
        this.startPosition;

    public Vector3 StartScale =>
        this.startScale;

    [CompilerGenerated]
    private sealed class <Move>c__IteratorA2 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Character <$>character;
        internal ScenarioMapIcon <>f__this;
        internal int <i>__2;
        internal ScenarioMapIcon <oldIcon>__1;
        internal CharacterTokenMap <token>__3;
        internal GuiWindowLocation <window>__0;
        internal Character character;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    if (this.<window>__0 == null)
                    {
                        goto Label_01DA;
                    }
                    this.<oldIcon>__1 = null;
                    this.<i>__2 = 0;
                    while (this.<i>__2 < this.<window>__0.mapPanel.Icons.Count)
                    {
                        if (this.<window>__0.mapPanel.Icons[this.<i>__2].ID == this.character.Location)
                        {
                            this.<oldIcon>__1 = this.<window>__0.mapPanel.Icons[this.<i>__2];
                            break;
                        }
                        this.<i>__2++;
                    }
                    break;

                case 1:
                    this.character.Location = this.<>f__this.ID;
                    this.<token>__3.transform.localPosition = Vector3.zero;
                    this.<oldIcon>__1.Refresh(true);
                    this.<>f__this.Refresh(true);
                    UI.Sound.Play(SoundEffectType.CharacterMovedFinish);
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 2;
                    goto Label_01E3;

                case 2:
                    goto Label_01DA;

                default:
                    goto Label_01E1;
            }
            if (this.<oldIcon>__1 != null)
            {
                this.<token>__3 = this.<oldIcon>__1.GetCharacterToken(this.character);
                if (this.<token>__3 != null)
                {
                    LeanTween.move(this.<token>__3.gameObject, this.<>f__this.transform.position, 0.35f).setEase(LeanTweenType.easeInOutQuad);
                    UI.Sound.Play(SoundEffectType.CharacterMoved);
                    this.$current = new WaitForSeconds(0.35f);
                    this.$PC = 1;
                    goto Label_01E3;
                }
            }
        Label_01DA:
            this.$PC = -1;
        Label_01E1:
            return false;
        Label_01E3:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }
}

