using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterToken : MonoBehaviour
{
    [Tooltip("reference to the \"avatar\" sprite in this scene")]
    public GameObject Avatar;
    [Tooltip("the ID of the character for this token")]
    public string ID;
    private bool isLocked;
    private Character myCharacter;
    private GameObject myDecoration;
    private CharacterTokenText myText;
    [Tooltip("Sound played when you select this token")]
    public AudioClip SelectedSound;
    [Tooltip("which deck license is required to use this character? (\"B\" or \"C\")")]
    public string Set = "B";
    [Tooltip("reference to the \"shadow\" sprite in this hierarchy")]
    public GameObject Shadow;
    private int startOrder;
    private Vector3 startPosition;

    private void AddDecoration(string prefabName)
    {
        if ((this.myDecoration == null) || (this.myDecoration.name != prefabName))
        {
            this.ClearDecorations();
            GameObject original = Resources.Load<GameObject>("Blueprints/Gui/" + prefabName);
            if (original != null)
            {
                this.myDecoration = UnityEngine.Object.Instantiate(original, base.transform.position, Quaternion.identity) as GameObject;
                if (this.myDecoration != null)
                {
                    this.myDecoration.name = original.name;
                    this.myDecoration.transform.parent = base.transform;
                }
            }
        }
    }

    private void Awake()
    {
        this.startPosition = base.transform.position;
        this.startOrder = this.SortingOrder;
        this.Shadow.SetActive(false);
        this.Default = true;
    }

    public void Buy()
    {
        if (!Settings.Debug.DemoMode)
        {
            Game.UI.ShowStoreWindow(Constants.IAP_LICENSE_CH_PREFIX + this.ID, LicenseType.Character);
        }
    }

    private void ClearDecorations()
    {
        if (this.myDecoration != null)
        {
            UnityEngine.Object.Destroy(this.myDecoration);
            this.myDecoration = null;
        }
    }

    public static CharacterToken Create(string ID)
    {
        GameObject original = Resources.Load<GameObject>("Blueprints/Tokens/" + ID);
        if (original != null)
        {
            GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
            if (obj3 != null)
            {
                obj3.name = original.name;
                return obj3.GetComponent<CharacterToken>();
            }
        }
        return null;
    }

    private Vector3 GetLastPosition()
    {
        if (this.Slot != null)
        {
            return this.Slot.transform.position;
        }
        if (this.Layout != null)
        {
            return this.Layout.GetTokenPosition(this);
        }
        return this.startPosition;
    }

    public void OnGuiDrag()
    {
        UI.Sound.Play(SoundEffectType.CharacterMoved);
        if (this.Home != null)
        {
            this.Shadow.SetActive(true);
            this.Shadow.transform.parent = this.Home.transform;
        }
        this.SortingOrder = Constants.SPRITE_SORTING_DRAG;
        if (this.Slot != null)
        {
            LeanTween.scale(base.gameObject, this.Slot.Scale, 0.2f).setEase(LeanTweenType.easeOutQuad);
        }
    }

    public void OnGuiDrop(CharacterTokenSlot slot)
    {
        if (slot == null)
        {
            if ((this.Home != null) && (Mathf.Abs((float) (base.transform.position.y - this.Home.transform.position.y)) <= 4f))
            {
                this.Home.OnDrop(this);
                LeanTween.cancel(base.gameObject);
                float time = Geometry.GetTweenTime(base.gameObject.transform.position, this.Home.Position, 0.25f);
                LeanTween.move(base.gameObject, this.Home.Position, time).setEase(LeanTweenType.easeOutQuad).setOnComplete(new Action(this.ResetStackingOrder));
                LeanTween.scale(base.gameObject, this.Home.Scale, time).setEase(LeanTweenType.easeOutQuad);
                this.Slot = this.Home;
            }
            else
            {
                LeanTween.cancel(base.gameObject);
                float num2 = Geometry.GetTweenTime(base.gameObject.transform.position, this.GetLastPosition(), 0.25f);
                Vector3[] to = Geometry.GetCurve(base.gameObject.transform.position, this.GetLastPosition(), 0f);
                LeanTween.move(base.gameObject, to, num2).setEase(LeanTweenType.easeOutQuad).setOnComplete(new Action(this.ResetStackingOrder));
            }
        }
        else
        {
            if (slot.DropAnimations)
            {
                if (slot.Bounce > 0f)
                {
                    LeanTween.cancel(base.gameObject);
                    float num3 = Geometry.GetTweenTime(base.gameObject.transform.position, slot.transform.position, 0.25f);
                    Vector3[] vectorArray2 = Geometry.GetCurve(base.gameObject.transform.position, slot.transform.position, slot.Bounce);
                    LeanTween.move(base.gameObject, vectorArray2, num3).setEase(LeanTweenType.easeOutQuad).setOnComplete(new Action(this.ResetStackingOrder));
                    LeanTween.scale(base.gameObject, slot.Scale, num3).setEase(LeanTweenType.easeOutQuad);
                }
                else
                {
                    LeanTween.cancel(base.gameObject);
                    float num4 = Geometry.GetTweenTime(base.gameObject.transform.position, slot.transform.position, 0.25f);
                    LeanTween.move(base.gameObject, slot.transform.position, num4).setEase(LeanTweenType.easeOutQuad).setOnComplete(new Action(this.ResetStackingOrder));
                    LeanTween.scale(base.gameObject, slot.Scale, num4).setEase(LeanTweenType.easeOutQuad);
                }
            }
            UI.Sound.Play(SoundEffectType.CharacterMovedFinish);
            this.Slot = slot;
        }
    }

    public void OnGuiSlot(CharacterTokenSlot slot, bool showShadow)
    {
        if (showShadow && (this.Home != null))
        {
            this.Shadow.SetActive(true);
            this.Shadow.transform.parent = this.Home.transform;
        }
        base.transform.position = slot.transform.position;
        base.transform.localScale = slot.Scale;
        this.Slot = slot;
        slot.Token = this;
    }

    private void RefreshDecorations()
    {
        if (this.Locked)
        {
            this.AddDecoration("Decoration_Token_Locked");
        }
        else
        {
            this.ClearDecorations();
        }
    }

    public void RefreshSprite()
    {
        if (this.Character != null)
        {
            if (Campaign.Deaths.Contains(this.Character.NickName) && !this.Default)
            {
                base.GetComponent<SpriteRenderer>().sprite = this.Character.PortraitSmallDead;
            }
            else if (!this.Licensed)
            {
                base.GetComponent<SpriteRenderer>().sprite = this.Character.PortraitSmallUnavailable;
            }
            else
            {
                base.GetComponent<SpriteRenderer>().sprite = this.Character.PortraitSmall;
            }
            this.RefreshDecorations();
        }
    }

    public void Reset()
    {
        float z = this.Shadow.transform.localPosition.z;
        this.Shadow.SetActive(false);
        this.Shadow.transform.parent = base.transform;
        this.Shadow.transform.localPosition = new Vector3(0f, 0f, z);
    }

    private void ResetStackingOrder()
    {
        if (this.Slot != null)
        {
            if (this.Slot.Shadows)
            {
                float z = this.Shadow.transform.localPosition.z;
                this.Shadow.transform.parent = base.transform;
                this.Shadow.transform.localPosition = new Vector3(0f, 0f, z);
                this.Shadow.transform.localScale = Vector3.one;
            }
            this.Slot.Refresh();
        }
        this.SortingOrder = this.startOrder;
    }

    public void Select(bool isSelected)
    {
        if (isSelected)
        {
            UI.Sound.Play(this.SelectedSound);
        }
        if (this.Avatar != null)
        {
            this.Avatar.SetActive(isSelected);
        }
    }

    public void Show(bool isVisible)
    {
        base.gameObject.SetActive(isVisible);
        if (this.Shadow != null)
        {
            this.Shadow.SetActive(isVisible);
        }
    }

    public Character Character
    {
        get => 
            this.myCharacter;
        set
        {
            this.myCharacter = value;
            if (this.myCharacter != null)
            {
                if (this.myText == null)
                {
                    this.myText = CharacterTokenText.Create(this);
                }
                if (this.myText != null)
                {
                    this.myText.Refresh(this);
                }
                if (Campaign.Deaths.Contains(this.myCharacter.NickName) && !this.Default)
                {
                    this.Locked = true;
                }
                this.RefreshSprite();
            }
        }
    }

    public bool Default { get; set; }

    public CharacterTokenSlot Home { get; set; }

    public CharacterTokenLayout Layout { get; set; }

    public bool Licensed =>
        LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_CH_PREFIX + this.ID);

    public string LicenseText =>
        StringTableManager.GetHelperText(0x2b);

    public bool Locked
    {
        get
        {
            BoxCollider2D component = base.GetComponent<BoxCollider2D>();
            if (component != null)
            {
                return !component.enabled;
            }
            return true;
        }
        set
        {
            this.isLocked = value;
            BoxCollider2D component = base.GetComponent<BoxCollider2D>();
            if (component != null)
            {
                component.enabled = !this.isLocked;
            }
            this.RefreshSprite();
        }
    }

    public Vector2 Size =>
        base.GetComponent<Renderer>().bounds.size;

    public CharacterTokenSlot Slot { get; set; }

    public int SortingOrder
    {
        get
        {
            Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
            if (componentsInChildren.Length > 0)
            {
                return componentsInChildren[0].sortingOrder;
            }
            return 0;
        }
        set
        {
            Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].sortingOrder = value;
            }
        }
    }

    public CharacterTokenText Text =>
        this.myText;
}

