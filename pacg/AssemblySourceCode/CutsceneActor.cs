using System;
using UnityEngine;

public class CutsceneActor : MonoBehaviour
{
    [Tooltip("length of tween in/out animation")]
    public static readonly float AnimationDuration = 0.3f;
    [Tooltip("\"concerned\" sprite for this actor")]
    public Sprite ArtMoodConcerned;
    [Tooltip("\"neutral\" sprite for this actor")]
    public Sprite ArtMoodNeutral;
    [Tooltip("actor's name displayed on the screen")]
    public string DisplayName;
    [Tooltip("this is the \"CH1B...\" ID used to identify party members")]
    public string ID;
    private static readonly float offScreenLeft = -10f;
    private static readonly float offScreenRight = 10f;
    [Tooltip("x,y offset of actor at start")]
    public Vector2 Offset = Vector2.zero;
    [Tooltip("true when this actor is a player character")]
    public bool PlayerCharacter;
    private ActorPositionType position;
    private float startLocalX;

    public ActorPositionType GetTalkPosition()
    {
        if (this.PlayerCharacter)
        {
            return ActorPositionType.Left;
        }
        return ActorPositionType.Right;
    }

    public void Initialize()
    {
        this.startLocalX = base.transform.localPosition.x + this.Offset.x;
        if (this.GetTalkPosition() == ActorPositionType.Left)
        {
            base.transform.localPosition = new Vector3(offScreenLeft - (this.Size.x / 2f), base.transform.localPosition.y + this.Offset.y, base.transform.localPosition.z);
            base.transform.Rotate((float) 0f, 180f, (float) 0f);
            this.position = ActorPositionType.None;
        }
        else
        {
            base.transform.localPosition = new Vector3(offScreenRight + (this.Size.x / 2f), base.transform.localPosition.y + this.Offset.y, base.transform.localPosition.z);
            this.position = ActorPositionType.None;
        }
        this.Mood = ActorMoodType.Neutral;
        this.Show(false);
    }

    public bool IsPartyMember()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].ID == this.ID)
            {
                return true;
            }
        }
        return false;
    }

    public void Show(ActorPositionType newPosition)
    {
        if (newPosition != this.position)
        {
            if (newPosition == ActorPositionType.None)
            {
                if (this.position == ActorPositionType.Left)
                {
                    LeanTween.moveLocalX(base.gameObject, offScreenLeft - (this.Size.x / 2f), AnimationDuration).setEase(LeanTweenType.easeInQuad);
                }
                if (this.position == ActorPositionType.Right)
                {
                    LeanTween.moveLocalX(base.gameObject, offScreenRight + (this.Size.x / 2f), AnimationDuration).setEase(LeanTweenType.easeInQuad);
                }
                this.position = ActorPositionType.None;
            }
            else if (newPosition == ActorPositionType.Left)
            {
                this.Show(true);
                this.position = ActorPositionType.Left;
                LeanTween.moveLocalX(base.gameObject, this.startLocalX, AnimationDuration).setEase(LeanTweenType.easeOutQuad);
            }
            else if (newPosition == ActorPositionType.Right)
            {
                this.Show(true);
                this.position = ActorPositionType.Right;
                LeanTween.moveLocalX(base.gameObject, this.startLocalX, AnimationDuration).setEase(LeanTweenType.easeOutQuad);
            }
        }
    }

    private void Show(bool isVisible)
    {
        Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            componentsInChildren[i].enabled = isVisible;
        }
    }

    public ActorMoodType Mood
    {
        set
        {
            SpriteRenderer componentInChildren = base.GetComponentInChildren<SpriteRenderer>();
            if (componentInChildren != null)
            {
                if (value == ActorMoodType.Concerned)
                {
                    if (this.ArtMoodConcerned != null)
                    {
                        componentInChildren.sprite = this.ArtMoodConcerned;
                    }
                }
                else if (this.ArtMoodNeutral != null)
                {
                    componentInChildren.sprite = this.ArtMoodNeutral;
                }
            }
        }
    }

    public Vector2 Size
    {
        get
        {
            SpriteRenderer componentInChildren = base.GetComponentInChildren<SpriteRenderer>();
            if (componentInChildren != null)
            {
                return new Vector2(componentInChildren.bounds.size.x, componentInChildren.bounds.size.y);
            }
            return Vector2.zero;
        }
    }
}

