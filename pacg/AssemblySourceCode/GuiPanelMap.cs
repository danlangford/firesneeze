using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelMap : GuiPanel
{
    private SpriteRenderer[] backgroundSprites;
    private bool cancelButtonState;
    private TKPanRecognizer dragRecognizer;
    [Tooltip("reference to the enter button in this scene")]
    public ScenarioMapButton enterButton;
    private bool isDragBusy;
    private bool isDraggingMap;
    [Tooltip("reference to the location panel in this scene")]
    public GuiPanelLocation locationPanel;
    private Transform mapButtons;
    private List<ScenarioMapIcon> mapIcons;
    private Transform mapImage;
    private List<GuiPanelMapLine> mapLines;
    private ScenarioMap mapProperties;
    [Tooltip("reference to the offscreen arrow \"bottom\" in this scene")]
    public GameObject offscreenArrowBottom;
    [Tooltip("reference to the offscreen arrow \"left\" in this scene")]
    public GameObject offscreenArrowLeft;
    [Tooltip("reference to the offscreen arrow \"right\" in this scene")]
    public GameObject offscreenArrowRight;
    [Tooltip("reference to the offscreen arrow \"top\" in this scene")]
    public GameObject offscreenArrowTop;
    [Tooltip("the buttons can scroll faster/slower than the background")]
    public float parallax;
    [Tooltip("reference to the peek button in this scene")]
    public ScenarioMapButton peekButton;
    private bool proceedButtonState;
    private ScenarioMapIcon selectedIcon;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("total seconds it takes to move to a new location")]
    public const float TotalMoveTime = 1.9f;
    [Tooltip("reference to the map transition panel in this scene (optional)")]
    public GuiPanelMapTransition transitionPanel;
    private List<Card> zoomedCards;
    [Tooltip("seconds it takes for a zoom into location to finish")]
    public const float ZoomTime = 1.25f;

    [DebuggerHidden]
    public IEnumerator Animate(Character[] characters, string[] locations) => 
        new <Animate>c__Iterator62 { 
            characters = characters,
            locations = locations,
            <$>characters = characters,
            <$>locations = locations,
            <>f__this = this
        };

    [DebuggerHidden]
    public IEnumerator Animate(Card card, string fromLocation, string toLocation) => 
        new <Animate>c__Iterator63 { 
            fromLocation = fromLocation,
            toLocation = toLocation,
            card = card,
            <$>fromLocation = fromLocation,
            <$>toLocation = toLocation,
            <$>card = card,
            <>f__this = this
        };

    private void BeginMove()
    {
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Global, "MapMove_FinishMove"));
        Turn.State = GameStateType.Move;
    }

    public void CancelMove()
    {
        this.ClearMapLocks();
        Turn.Map = false;
        this.Mode = MapModeType.Move;
        if (Turn.Phase == TurnPhaseType.Move)
        {
            Turn.Phase = TurnPhaseType.Give;
            if (!Rules.IsGiveCardPossible())
            {
                Turn.Phase = TurnPhaseType.Move;
            }
        }
        if (Turn.State == GameStateType.Move)
        {
            Turn.PopCancelDestination();
            Turn.Proceed();
        }
        else
        {
            Turn.GotoCancelDestination();
            Turn.PopReturnState();
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowMap(false);
            window.Refresh();
            window.ShowMapButton(Rules.IsMapLookPossible());
            window.commandsPanel.ShowGiveButton(Turn.Phase == TurnPhaseType.Give);
            window.commandsPanel.ShowMoveButton(true);
        }
    }

    private bool CanSelectIcon(ScenarioMapIcon icon)
    {
        if (icon == null)
        {
            return false;
        }
        if (icon.Locked)
        {
            return false;
        }
        return (((((this.Mode == MapModeType.Move) || (this.Mode == MapModeType.Choose)) || ((this.Mode == MapModeType.Examine) || (this.Mode == MapModeType.Look))) || ((this.Mode == MapModeType.EvadeThenChoose) || (this.Mode == MapModeType.MoveMultipleIgnoreRestrictions))) || ((this.Mode == MapModeType.Peek) && !Scenario.Current.IsLocationClosed(icon.ID)));
    }

    public void Center()
    {
        int num = 0;
        float distanceSum = this.GetDistanceSum(this.mapIcons[0]);
        for (int i = 1; i < this.mapIcons.Count; i++)
        {
            float num4 = this.GetDistanceSum(this.mapIcons[i]);
            if (num4 < distanceSum)
            {
                num = i;
                distanceSum = num4;
            }
        }
        this.Focus(this.mapIcons[num], false);
    }

    public void Center(string id)
    {
        this.locationPanel.Show(id);
        ScenarioMapIcon mapIcon = this.GetMapIcon(id);
        if (mapIcon != null)
        {
            this.SelectLocation(mapIcon);
            this.Focus(mapIcon, false);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Didn't find icon to move to: " + id);
        }
    }

    public void CenterAllIcons()
    {
        this.Center();
        this.UpdateIconArrows();
    }

    public void CenterAllIcons(string id)
    {
        this.Center(id);
        this.UpdateIconArrows();
    }

    public void ChangeBackgroundColor(Color color, float time)
    {
        if (this.backgroundSprites == null)
        {
            this.backgroundSprites = this.mapImage.gameObject.GetComponentsInChildren<SpriteRenderer>(true);
        }
        Vector3 from = new Vector3(this.backgroundSprites[0].color.r, this.backgroundSprites[0].color.g, this.backgroundSprites[0].color.b);
        Vector3 to = new Vector3(color.r, color.g, color.b);
        if (time > 0f)
        {
            LeanTween.value(base.gameObject, new Action<Vector3>(this.OnBackgroundColorChanged), from, to, time);
        }
        else
        {
            this.OnBackgroundColorChanged(to);
        }
    }

    public void ClearMapLocks()
    {
        for (int i = 0; i < this.mapIcons.Count; i++)
        {
            this.mapIcons[i].Locked = false;
        }
    }

    public void Close()
    {
        UI.Busy = false;
        UI.Zoomed = false;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowMap(false);
            if (this.Mode == MapModeType.Look)
            {
                this.locationPanel.Show(Location.Current.ID);
                window.ShowProceedButton(this.proceedButtonState);
                window.ShowCancelButton(this.cancelButtonState);
                window.ShowMapButton(Rules.IsMapLookPossible());
            }
            window.Refresh();
        }
        this.ClearMapLocks();
        Turn.Map = false;
        this.Mode = MapModeType.Move;
    }

    public void EnterSelectedLocation()
    {
        if (this.selectedIcon != null)
        {
            Turn.Number = Turn.Current;
            switch (this.Mode)
            {
                case MapModeType.Move:
                case MapModeType.Choose:
                case MapModeType.MoveMultipleIgnoreRestrictions:
                    this.OnMoveButtonPushedComplete();
                    break;

                case MapModeType.Peek:
                    this.peekButton.Tap();
                    break;

                case MapModeType.Examine:
                    this.OnExamineButtonPushed(Turn.Character);
                    break;

                case MapModeType.EvadeThenChoose:
                {
                    bool evade = Turn.Evade;
                    bool defeat = Turn.Defeat;
                    if (!Rules.IsCardSummons(Turn.Card))
                    {
                        Location.Current.Deck.Shuffle();
                    }
                    else
                    {
                        Campaign.Box.Add(Turn.Card, false);
                    }
                    this.Mode = MapModeType.Choose;
                    Turn.ClearEncounterData();
                    Turn.Defeat = defeat;
                    Turn.Evade = evade;
                    this.OnMoveButtonPushedComplete();
                    return;
                }
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator ExamineLocation(Character character, ScenarioMapIcon icon) => 
        new <ExamineLocation>c__Iterator5F { 
            icon = icon,
            character = character,
            <$>icon = icon,
            <$>character = character,
            <>f__this = this
        };

    public void Fade(bool isVisible, float duration)
    {
        float to = !isVisible ? ((float) 0) : ((float) 1);
        SpriteRenderer[] componentsInChildren = this.mapImage.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            LeanTween.alpha(componentsInChildren[i].gameObject, to, duration);
        }
        for (int j = 0; j < this.mapIcons.Count; j++)
        {
            this.mapIcons[j].Fade(to, duration);
        }
        for (int k = 0; k < this.mapLines.Count; k++)
        {
            this.mapLines[k].Fade(to, duration);
        }
    }

    public void FinishMove()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (Party.Characters[Turn.Target].Location != this.selectedIcon.ID)
            {
                window.ShowMap(true);
                base.StartCoroutine(this.Move(Party.Characters[Turn.Target], this.selectedIcon, true));
            }
            else
            {
                this.FinishMoveAdvanceTurn();
            }
        }
    }

    private void FinishMoveAdvanceTurn()
    {
        if ((Turn.State == GameStateType.Move) || (Turn.State == GameStateType.Null))
        {
            Turn.EmptyLayoutDecks = true;
            if (Turn.PeekStateDestination() == null)
            {
                Turn.ReturnToReturnState();
            }
            else
            {
                Turn.GotoStateDestination();
            }
        }
        Turn.PopCancelDestination();
        this.Mode = MapModeType.Move;
    }

    private void Focus(ScenarioMapIcon icon, bool isAnimated)
    {
        Vector3 vector = new Vector3(-icon.StartOffset.x, -icon.StartOffset.y, 0f);
        float num = 0f;
        float num2 = 0f;
        Vector3 position = new Vector3(vector.x - this.mapProperties.mapBoundsLeft, 0f, 0f);
        if (UI.Camera.WorldToViewportPoint(position).x >= 0f)
        {
            num = UI.Camera.ViewportToWorldPoint(Vector3.zero).x - position.x;
        }
        Vector3 vector3 = new Vector3(vector.x + this.mapProperties.mapBoundsRight, 0f, 0f);
        if (UI.Camera.WorldToViewportPoint(vector3).x <= 1f)
        {
            num = UI.Camera.ViewportToWorldPoint(Vector3.one).x - vector3.x;
        }
        Vector3 vector4 = new Vector3(0f, vector.y + this.mapProperties.mapBoundsTop, 0f);
        if (UI.Camera.WorldToViewportPoint(vector4).y <= 1f)
        {
            num2 = UI.Camera.ViewportToWorldPoint(Vector3.one).y - vector4.y;
        }
        Vector3 vector5 = new Vector3(0f, vector.y - this.mapProperties.mapBoundsBottom, 0f);
        if (UI.Camera.WorldToViewportPoint(vector5).y >= 0f)
        {
            num2 = UI.Camera.ViewportToWorldPoint(Vector3.zero).y - vector5.y;
        }
        Vector3 to = new Vector3(vector.x + num, vector.y + num2, this.mapImage.transform.position.z);
        Vector3 vector7 = new Vector3(vector.x + num, vector.y + num2, this.mapButtons.transform.position.z);
        if (isAnimated)
        {
            LeanTween.move(this.mapImage.gameObject, to, 0.3f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.move(this.mapButtons.gameObject, vector7, 0.3f).setEase(LeanTweenType.easeInOutQuad);
        }
        else
        {
            this.mapImage.position = to;
            this.mapButtons.position = vector7;
        }
    }

    private float GetDistanceSum(ScenarioMapIcon icon)
    {
        float num = 0f;
        for (int i = 0; i < this.mapIcons.Count; i++)
        {
            num += Vector3.SqrMagnitude(icon.transform.position - this.mapIcons[i].transform.position);
        }
        return num;
    }

    public Vector3 GetIconPosition(ScenarioMapIcon icon) => 
        (this.mapImage.transform.position + icon.StartOffset);

    public ScenarioMapIcon GetMapIcon(string ID)
    {
        if (this.mapIcons != null)
        {
            for (int i = 0; i < this.mapIcons.Count; i++)
            {
                if (this.mapIcons[i].ID == ID)
                {
                    return this.mapIcons[i];
                }
            }
        }
        return null;
    }

    private Vector3 GetScreenPosition(ScenarioMapIcon icon)
    {
        Vector3 origin = this.mapImage.transform.position + icon.StartOffset;
        Vector3 direction = UI.Camera.ViewportToWorldPoint((Vector3) new Vector2(0.5f, 0.5f)) - origin;
        RaycastHit2D hitd = Physics2D.Raycast(origin, direction, direction.magnitude, Constants.LAYER_MASK_SCREEN);
        if (hitd.collider != null)
        {
            Vector3 vector3 = new Vector3(hitd.point.x, hitd.point.y, 0f) + ((Vector3) (direction.normalized * 0.8f));
            return new Vector3(vector3.x, vector3.y, origin.z);
        }
        return origin;
    }

    private Vector3 GetZoomPosition(Card card, int position, int total)
    {
        if ((total == 1) && (position == 0))
        {
            return new Vector3(0f, 0f, 0f);
        }
        if ((total == 2) && (position == 0))
        {
            return new Vector3(-3f, 0f, 0f);
        }
        if ((total == 2) && (position == 1))
        {
            return new Vector3(3f, 0f, 0f);
        }
        if ((total == 3) && (position == 0))
        {
            return new Vector3(-5f, 0f, 0f);
        }
        if ((total == 3) && (position == 1))
        {
            return new Vector3(0f, 0f, 0f);
        }
        if ((total == 3) && (position == 2))
        {
            return new Vector3(5f, 0f, 0f);
        }
        return Vector3.zero;
    }

    public override void Initialize()
    {
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.gestureRecognizedEvent += r => this.OnGuiTap(this.tapRecognizer.touchLocation());
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.dragRecognizer = new TKPanRecognizer(Device.GetMinimumDragDistance());
        this.dragRecognizer.gestureRecognizedEvent += delegate (TKPanRecognizer r) {
            if (this.IsDragAllowed())
            {
                if (!this.isDraggingMap)
                {
                    this.OnGuiDragStart(this.dragRecognizer.touchLocation());
                }
                else
                {
                    this.OnGuiDrag(this.dragRecognizer.deltaTranslation);
                }
            }
        };
        this.dragRecognizer.gestureCompleteEvent += r => this.OnGuiDragEnd();
        TouchKit.addGestureRecognizer(this.dragRecognizer);
        GameObject prefab = Resources.Load<GameObject>("Blueprints/Maps/" + Scenario.Current.Map);
        if (prefab != null)
        {
            GameObject obj3 = Geometry.CreateChildObject(base.gameObject, prefab, "Map");
            if (obj3 != null)
            {
                this.mapProperties = obj3.GetComponent<ScenarioMap>();
                this.mapButtons = Geometry.GetChild(obj3.transform, "Buttons");
                this.mapImage = Geometry.GetChild(obj3.transform, "Art");
                this.enterButton.transform.parent = this.mapButtons;
                this.peekButton.transform.parent = this.mapButtons;
            }
        }
        ScenarioMapIcon[] componentsInChildren = base.GetComponentsInChildren<ScenarioMapIcon>(true);
        this.mapIcons = new List<ScenarioMapIcon>(componentsInChildren.Length);
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            if (Scenario.Current.IsLocationValid(componentsInChildren[i].ID))
            {
                componentsInChildren[i].Initialize();
                this.mapIcons.Add(componentsInChildren[i]);
            }
            else
            {
                UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
            }
        }
        this.mapLines = new List<GuiPanelMapLine>(this.mapIcons.Count * this.mapIcons.Count);
        for (int j = 0; j < this.mapIcons.Count; j++)
        {
            for (int k = 0; k < this.mapIcons.Count; k++)
            {
                if (this.IsLineAllowed(this.mapIcons[j], this.mapIcons[k]))
                {
                    GuiPanelMapLine item = GuiPanelMapLine.Create(this.mapIcons[j], this.mapIcons[k]);
                    if (item != null)
                    {
                        item.transform.parent = base.transform;
                        item.transform.localPosition = Vector3.zero;
                        this.mapLines.Add(item);
                    }
                }
            }
        }
        if (this.transitionPanel != null)
        {
            this.transitionPanel.Initialize();
        }
        this.zoomedCards = new List<Card>(3);
        this.Mode = MapModeType.Move;
    }

    public bool IsChooseAllowed()
    {
        if ((this.Mode != MapModeType.Choose) && (this.Mode != MapModeType.EvadeThenChoose))
        {
            return false;
        }
        return true;
    }

    private bool IsDragAllowed()
    {
        if (this.isDragBusy)
        {
            return false;
        }
        if (Turn.Dice.Count > 0)
        {
            return false;
        }
        return true;
    }

    public bool IsIconVisible(ScenarioMapIcon icon)
    {
        Vector3 origin = this.mapImage.transform.position + icon.StartOffset;
        if (Physics2D.Raycast(origin, Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_SCREEN).collider != null)
        {
            return true;
        }
        Vector3 vector2 = origin - icon.transform.position;
        return (vector2.sqrMagnitude < 1f);
    }

    private bool IsLineAllowed(ScenarioMapIcon icon1, ScenarioMapIcon icon2)
    {
        if (((icon1 == null) || (icon2 == null)) || (this.mapLines == null))
        {
            return false;
        }
        if ((icon1.ID == null) || (icon2.ID == null))
        {
            return false;
        }
        if (icon1.ID == icon2.ID)
        {
            return false;
        }
        if (!Scenario.Current.IsLocationLinked(icon1.ID, icon2.ID))
        {
            return false;
        }
        for (int i = 0; i < this.mapLines.Count; i++)
        {
            if (this.mapLines[i].Match(icon1, icon2))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsMoveAllowed(string from, string to) => 
        ((((this.Mode == MapModeType.Peek) || (this.Mode == MapModeType.Examine)) || ((this.Mode == MapModeType.Look) || (this.Mode == MapModeType.MoveMultipleIgnoreRestrictions))) || (!Scenario.Current.Linear || Scenario.Current.IsLocationLinked(from, to)));

    public bool IsMoveRestrictionRequired() => 
        (this.Mode != MapModeType.MoveMultipleIgnoreRestrictions);

    public bool IsPeekAllowed() => 
        (this.Mode == MapModeType.Peek);

    public bool IsTouchHandled(Vector2 touchPos)
    {
        if (this.Visible)
        {
            RaycastHit2D hitd = Physics2D.Raycast(Geometry.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_MAP);
            if ((hitd != 0) && (hitd.collider.transform.GetComponent<ScenarioMapIcon>() != null))
            {
                return true;
            }
        }
        return false;
    }

    public void LockMapIcon(string location, bool locked)
    {
        for (int i = 0; i < Scenario.Current.Locations.Length; i++)
        {
            if (Scenario.Current.Locations[i].LocationName == location)
            {
                this.GetMapIcon(location).Locked = locked;
            }
        }
    }

    private void Move(Character character)
    {
        if (character.Location != this.selectedIcon.ID)
        {
            this.BeginMove();
        }
        else
        {
            this.CancelMove();
        }
    }

    [DebuggerHidden]
    private IEnumerator Move(Character character, ScenarioMapIcon icon, bool proceed) => 
        new <Move>c__Iterator60 { 
            character = character,
            icon = icon,
            proceed = proceed,
            <$>character = character,
            <$>icon = icon,
            <$>proceed = proceed,
            <>f__this = this
        };

    [DebuggerHidden]
    public IEnumerator MoveCharacter(Character character, string locID) => 
        new <MoveCharacter>c__Iterator61 { 
            locID = locID,
            character = character,
            <$>locID = locID,
            <$>character = character,
            <>f__this = this
        };

    public void MoveSelectedCharacters(string locID)
    {
        Turn.Iterators.Start(TurnStateIteratorType.Move);
        bool flag = false;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].Selected)
            {
                Turn.BlackBoard.Set<string>(Party.Characters[i].ID + "_destination", locID);
                if (!flag)
                {
                    Turn.Number = i;
                    flag = true;
                }
            }
        }
        Turn.DamageTargetType = DamageTargetType.Selected;
        Turn.Iterators.Invoke();
    }

    private void OnBackgroundColorChanged(Vector3 v)
    {
        for (int i = 0; i < this.backgroundSprites.Length; i++)
        {
            this.backgroundSprites[i].color = new Color(v.x, v.y, v.z);
        }
    }

    private void OnEnterButtonPushed()
    {
        if (!this.selectedIcon.Locked)
        {
            this.selectedIcon.Tap();
            string iD = Location.Current.ID;
            if ((this.Mode == MapModeType.Choose) || (this.Mode == MapModeType.EvadeThenChoose))
            {
                iD = Party.Characters[Turn.Target].Location;
            }
            if (this.IsMoveAllowed(iD, this.selectedIcon.ID))
            {
                this.Pause(true);
                base.Pause(false);
                this.peekButton.Show(false);
                this.enterButton.Show(false);
                LeanTween.delayedCall(0.3f, new Action(this.EnterSelectedLocation));
            }
        }
    }

    private void OnExamineButtonPushed(Character character)
    {
        base.StartCoroutine(this.ExamineLocation(character, this.selectedIcon));
    }

    private void OnGuiDrag(Vector2 deltaTranslation)
    {
        float x = deltaTranslation.x;
        float y = deltaTranslation.y;
        Vector3 position = UI.Camera.WorldToScreenPoint(this.mapImage.position) + new Vector3(x, y, 0f);
        Vector3 vector2 = UI.Camera.ScreenToWorldPoint(position);
        Vector3 vector3 = new Vector3(vector2.x - this.mapProperties.mapBoundsLeft, 0f, 0f);
        if (UI.Camera.WorldToViewportPoint(vector3).x > 0f)
        {
            float num3 = UI.Camera.WorldToViewportPoint(vector3).x;
            x = (num3 < (Device.GetMarginLeft() + 0.05f)) ? (x + (x * ((20f * num3) + 1f))) : 0f;
        }
        Vector3 vector4 = new Vector3(vector2.x + this.mapProperties.mapBoundsRight, 0f, 0f);
        if (UI.Camera.WorldToViewportPoint(vector4).x < 1f)
        {
            float num4 = UI.Camera.WorldToViewportPoint(vector4).x;
            x = (num4 > (Device.GetMarginRight() - 0.05f)) ? (x + (x * ((20f * num4) - 19f))) : 0f;
        }
        Vector3 vector5 = new Vector3(0f, vector2.y + this.mapProperties.mapBoundsTop, 0f);
        if (UI.Camera.WorldToViewportPoint(vector5).y < 1f)
        {
            float num5 = UI.Camera.WorldToViewportPoint(vector5).y;
            y = (num5 > 0.95f) ? (y + (y * ((20f * num5) - 19f))) : 0f;
        }
        Vector3 vector6 = new Vector3(0f, vector2.y - this.mapProperties.mapBoundsBottom, 0f);
        if (UI.Camera.WorldToViewportPoint(vector6).y > 0f)
        {
            float num6 = UI.Camera.WorldToViewportPoint(vector6).y;
            y = (num6 < 0.05f) ? (y + (y * ((20f * num6) + 1f))) : 0f;
        }
        Vector3 vector7 = UI.Camera.WorldToScreenPoint(this.mapImage.position) + new Vector3(x, y, 0f);
        this.mapImage.position = UI.Camera.ScreenToWorldPoint(vector7);
        Vector3 vector8 = UI.Camera.WorldToScreenPoint(this.mapButtons.position) + new Vector3(x + (this.parallax * x), y + (this.parallax * y), 0f);
        this.mapButtons.position = UI.Camera.ScreenToWorldPoint(vector8);
    }

    private void OnGuiDragEnd()
    {
        this.isDragBusy = false;
        this.isDraggingMap = false;
        float x = 0f;
        float y = 0f;
        Vector3 position = new Vector3(this.mapImage.position.x - this.mapProperties.mapBoundsLeft, 0f, 0f);
        if (UI.Camera.WorldToViewportPoint(position).x >= Device.GetMarginLeft())
        {
            x = UI.Camera.ViewportToWorldPoint(Vector3.zero).x - position.x;
        }
        Vector3 vector2 = new Vector3(this.mapImage.position.x + this.mapProperties.mapBoundsRight, 0f, 0f);
        if (UI.Camera.WorldToViewportPoint(vector2).x <= Device.GetMarginRight())
        {
            x = UI.Camera.ViewportToWorldPoint(Vector3.one).x - vector2.x;
        }
        Vector3 vector3 = new Vector3(0f, this.mapImage.position.y + this.mapProperties.mapBoundsTop, 0f);
        if (UI.Camera.WorldToViewportPoint(vector3).y <= 1f)
        {
            y = UI.Camera.ViewportToWorldPoint(Vector3.one).y - vector3.y;
        }
        Vector3 vector4 = new Vector3(0f, this.mapImage.position.y - this.mapProperties.mapBoundsBottom, 0f);
        if (UI.Camera.WorldToViewportPoint(vector4).y >= 0f)
        {
            y = UI.Camera.ViewportToWorldPoint(Vector3.zero).y - vector4.y;
        }
        if ((x != 0f) || (y != 0f))
        {
            Vector3 to = this.mapImage.position + new Vector3(x, y, 0f);
            LeanTween.move(this.mapImage.gameObject, to, 0.3f).setEase(LeanTweenType.easeOutQuad);
            Vector3 vector6 = this.mapButtons.position + new Vector3(x, y, 0f);
            LeanTween.move(this.mapButtons.gameObject, vector6, 0.3f).setEase(LeanTweenType.easeOutQuad);
        }
    }

    private void OnGuiDragStart(Vector2 touchPos)
    {
        if (Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_CARD | Constants.LAYER_MASK_MAP).collider != null)
        {
            this.isDragBusy = true;
            this.isDraggingMap = false;
        }
        else
        {
            this.isDragBusy = false;
            this.isDraggingMap = true;
        }
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (!UI.Zoomed)
        {
            RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_MAP);
            if (hitd != 0)
            {
                ScenarioMapIcon component = hitd.collider.transform.GetComponent<ScenarioMapIcon>();
                if (component != null)
                {
                    if (!component.Locked)
                    {
                        if (!this.IsIconVisible(component))
                        {
                            this.Focus(component, true);
                        }
                        if (this.selectedIcon == component)
                        {
                            if (this.Mode == MapModeType.Peek)
                            {
                                this.peekButton.Tap();
                            }
                            if (((this.Mode == MapModeType.Move) || (this.Mode == MapModeType.Choose)) || (((this.Mode == MapModeType.Examine) || (this.Mode == MapModeType.EvadeThenChoose)) || (this.Mode == MapModeType.MoveMultipleIgnoreRestrictions)))
                            {
                                this.enterButton.Tap();
                            }
                        }
                        else
                        {
                            this.OnLocationIconTap(component);
                        }
                    }
                }
                else
                {
                    ScenarioMapButton button = hitd.collider.transform.GetComponent<ScenarioMapButton>();
                    if (button != null)
                    {
                        button.Tap();
                    }
                }
            }
        }
        else
        {
            if (this.Mode == MapModeType.Peek)
            {
                Turn.GotoStateDestination();
            }
            if (this.zoomedCards.Count > 0)
            {
                this.UnZoomCards();
            }
            UI.Zoomed = false;
        }
    }

    private void OnLocationIconTap(ScenarioMapIcon icon)
    {
        icon.Tap();
        this.SelectLocation(icon);
    }

    private void OnMoveButtonPushedComplete()
    {
        for (int i = 0; i < this.Icons.Count; i++)
        {
            this.Icons[i].Face(false);
        }
        if (this.Mode == MapModeType.Choose)
        {
            this.Move(Party.Characters[Turn.Target]);
        }
        else if (this.Mode == MapModeType.MoveMultipleIgnoreRestrictions)
        {
            this.MoveSelectedCharacters(this.selectedIcon.ID);
        }
        else
        {
            Turn.Target = Turn.Current;
            this.Move(Turn.Owner);
        }
    }

    private void OnPeekButtonPushed()
    {
        if (!UI.Zoomed)
        {
            string[] ids = Location.Peek(this.selectedIcon.ID, this.ModeCards);
            this.ZoomCards(ids);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if ((window != null) && (ids != null))
            {
                window.messagePanel.Clear();
            }
        }
        else
        {
            this.UnZoomCards();
        }
    }

    private void OnPlayerMoved()
    {
        if (Turn.Phase == TurnPhaseType.Move)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                Turn.BlackBoard.Set<int>("GiveCardCount", 1);
                window.commandsPanel.ShowGiveButton(false);
                Turn.BlackBoard.Set<int>("MoveLocationCount", 1);
                window.commandsPanel.ShowMoveButton(false);
            }
        }
        if (Turn.State == GameStateType.StartTurn)
        {
            Turn.Proceed();
        }
        if (Turn.State == GameStateType.EndTurn)
        {
            Turn.Proceed();
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        if (this.tapRecognizer != null)
        {
            this.tapRecognizer.enabled = !isPaused;
        }
        if (this.dragRecognizer != null)
        {
            this.dragRecognizer.enabled = !isPaused;
        }
        this.enterButton.Show(false);
        this.peekButton.Show(false);
    }

    public void Peek(int numCards)
    {
        this.ModeCards = numCards;
        this.Mode = MapModeType.Peek;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowMap(true);
            window.messagePanel.Show(StringTableManager.GetHelperText(0x1c));
        }
    }

    public void Recenter()
    {
        if (this.selectedIcon != null)
        {
            this.Center(this.selectedIcon.ID);
        }
        else
        {
            this.Center(Location.Current.ID);
        }
    }

    public override void Refresh()
    {
        for (int i = 0; i < this.mapIcons.Count; i++)
        {
            if (this.mapIcons[i] != null)
            {
                this.mapIcons[i].Refresh(false);
            }
        }
    }

    public void RefreshIconLines()
    {
        this.UpdateIconLines();
    }

    public override void Reset()
    {
        this.mapImage.transform.localPosition = Vector3.zero;
        this.mapButtons.transform.localPosition = Vector3.zero;
        for (int i = 0; i < this.mapIcons.Count; i++)
        {
            this.mapIcons[i].transform.localPosition = this.mapIcons[i].StartOffset;
        }
    }

    public void Seek(string id)
    {
        this.locationPanel.Show(id);
        ScenarioMapIcon mapIcon = this.GetMapIcon(id);
        this.SelectLocation(mapIcon);
        if (!this.IsIconVisible(mapIcon))
        {
            this.Focus(mapIcon, true);
        }
    }

    private void SelectLocation(ScenarioMapIcon icon)
    {
        if (icon != null)
        {
            if (this.locationPanel != null)
            {
                this.locationPanel.Show(icon.ID);
            }
            for (int i = 0; i < this.mapLines.Count; i++)
            {
                this.mapLines[i].Glow(false);
            }
            if (this.CanSelectIcon(icon))
            {
                this.selectedIcon = icon;
                if ((((this.Mode == MapModeType.Move) || (this.Mode == MapModeType.Choose)) || ((this.Mode == MapModeType.Examine) || (this.Mode == MapModeType.Look))) || ((this.Mode == MapModeType.EvadeThenChoose) || (this.Mode == MapModeType.MoveMultipleIgnoreRestrictions)))
                {
                    GuiWindowLocation window = UI.Window as GuiWindowLocation;
                    if (window != null)
                    {
                        window.ShowProceedButton(false);
                        window.ShowCancelButton(false);
                        this.enterButton.transform.position = icon.transform.position;
                        this.enterButton.Show(true);
                    }
                }
                if (this.Mode == MapModeType.Peek)
                {
                    this.peekButton.transform.position = icon.transform.position;
                    this.peekButton.Show(true);
                }
                for (int j = 0; j < this.mapLines.Count; j++)
                {
                    if (this.mapLines[j].Match(icon))
                    {
                        this.mapLines[j].Glow(true);
                    }
                }
            }
            else
            {
                this.peekButton.Show(false);
                this.enterButton.Show(false);
            }
        }
    }

    public override void Show(bool isVisible)
    {
        if ((this.Mode == MapModeType.Look) && isVisible)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                this.proceedButtonState = window.proceedButton.Visible;
                window.ShowProceedButton(false);
                this.cancelButtonState = window.cancelButton.Visible;
                window.ShowCancelButton(false);
            }
        }
        bool map = Turn.Map;
        base.gameObject.SetActive(isVisible);
        this.Pause(!isVisible);
        this.offscreenArrowBottom.SetActive(false);
        this.offscreenArrowLeft.SetActive(false);
        this.offscreenArrowRight.SetActive(false);
        this.offscreenArrowTop.SetActive(false);
        if (isVisible)
        {
            SpriteRenderer[] componentsInChildren = this.mapImage.GetComponentsInChildren<SpriteRenderer>();
            for (int j = 0; j < componentsInChildren.Length; j++)
            {
                componentsInChildren[j].color = new Color(componentsInChildren[j].color.r, componentsInChildren[j].color.g, componentsInChildren[j].color.b, 1f);
            }
        }
        if (isVisible)
        {
            for (int k = 0; k < this.mapIcons.Count; k++)
            {
                if (this.mapIcons[k] != null)
                {
                    this.mapIcons[k].gameObject.SetActive(true);
                    this.mapIcons[k].transform.position = this.mapImage.transform.position + this.mapIcons[k].StartOffset;
                    this.mapIcons[k].Refresh(false);
                    this.mapIcons[k].Fade(1f, 0.3f);
                }
            }
        }
        Turn.Map = isVisible;
        UI.Zoomed = false;
        this.isDragBusy = false;
        this.isDraggingMap = false;
        if (isVisible)
        {
            this.SelectLocation(this.selectedIcon);
            UI.Sound.MusicPlay(this.mapProperties.Music);
        }
        else if (Location.Current != null)
        {
            UI.Sound.MusicPlay(Location.Current.Music, true, true);
        }
        for (int i = 0; i < this.zoomedCards.Count; i++)
        {
            UnityEngine.Object.Destroy(this.zoomedCards[i].gameObject);
        }
        this.zoomedCards.Clear();
        if ((isVisible && !map) && (Turn.Phase == TurnPhaseType.Move))
        {
            Tutorial.Notify(TutorialEventType.ScreenMapShown);
        }
        if (!isVisible && map)
        {
            Tutorial.Notify(TutorialEventType.ScreenWasClosed);
        }
    }

    private void UnZoomCards()
    {
        for (int i = 0; i < this.zoomedCards.Count; i++)
        {
            float time = 0.5f + (0.15f * i);
            this.zoomedCards[i].MoveCard(this.selectedIcon.transform.position, time).setEase(LeanTweenType.easeOutQuad);
            LeanTween.scale(this.zoomedCards[i].gameObject, new Vector3(0f, 0f, 1f), time).setEase(LeanTweenType.easeOutQuad);
        }
        float delayTime = 0.65f + (0.15f * this.zoomedCards.Count);
        LeanTween.delayedCall(delayTime, new Action(this.Close));
        UI.Busy = true;
        this.Mode = MapModeType.None;
    }

    private void Update()
    {
        if (!base.Paused)
        {
            if (this.mapIcons != null)
            {
                this.UpdateIconArrows();
                this.UpdateIconLines();
            }
            if ((this.selectedIcon != null) && this.enterButton.Visible)
            {
                this.enterButton.transform.position = this.selectedIcon.transform.position;
            }
        }
    }

    private void UpdateIconArrows()
    {
        for (int i = 0; i < this.mapIcons.Count; i++)
        {
            if (this.mapIcons[i] != null)
            {
                Vector3 screenPosition = this.GetScreenPosition(this.mapIcons[i]);
                this.mapIcons[i].gameObject.transform.position = screenPosition;
                if (this.IsIconVisible(this.mapIcons[i]))
                {
                    this.mapIcons[i].Face(false);
                }
                else
                {
                    this.mapIcons[i].Face(this.mapImage.transform.position + this.mapIcons[i].StartOffset);
                }
            }
        }
    }

    private void UpdateIconLines()
    {
        if (((Scenario.Current != null) && Scenario.Current.Linear) && (this.mapLines != null))
        {
            for (int i = 0; i < this.mapLines.Count; i++)
            {
                this.mapLines[i].Refresh();
            }
        }
    }

    private void UpdatePanArrows()
    {
        this.offscreenArrowBottom.SetActive(false);
        this.offscreenArrowLeft.SetActive(false);
        this.offscreenArrowRight.SetActive(false);
        this.offscreenArrowTop.SetActive(false);
        for (int i = 0; i < this.mapIcons.Count; i++)
        {
            if (this.mapIcons[i] != null)
            {
                Vector3 vector = UI.Camera.WorldToViewportPoint(this.mapIcons[i].transform.position);
                if (vector.x < 0f)
                {
                    this.offscreenArrowLeft.SetActive(true);
                }
                if (vector.x > 1f)
                {
                    this.offscreenArrowRight.SetActive(true);
                }
                if (vector.y < 0f)
                {
                    this.offscreenArrowBottom.SetActive(true);
                }
                if (vector.y > 1f)
                {
                    this.offscreenArrowTop.SetActive(true);
                }
            }
        }
    }

    private void ZoomCards(string[] ids)
    {
        if (ids != null)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] != null)
                {
                    Card card = CardTable.Create(ids[i]);
                    card.SortingOrder = 200;
                    card.SortingLayer = "UI";
                    Animator componentInChildren = card.GetComponentInChildren<Animator>();
                    if (componentInChildren != null)
                    {
                        componentInChildren.enabled = false;
                    }
                    card.transform.position = this.selectedIcon.transform.position;
                    card.transform.localScale = new Vector3(0f, 0f, 1f);
                    card.Show(true);
                    Vector3 destination = this.GetZoomPosition(card, i, ids.Length);
                    float time = 0.5f + (0.15f * i);
                    card.MoveCard(destination, time).setEase(LeanTweenType.easeOutQuad);
                    LeanTween.scale(card.gameObject, new Vector3(0.65f, 0.65f, 1f), time).setEase(LeanTweenType.easeOutQuad);
                    this.zoomedCards.Add(card);
                    UI.Zoomed = true;
                }
            }
        }
    }

    public List<ScenarioMapIcon> Icons =>
        this.mapIcons;

    public List<GuiPanelMapLine> Links =>
        this.mapLines;

    public MapModeType Mode { get; set; }

    private int ModeCards { get; set; }

    [CompilerGenerated]
    private sealed class <Animate>c__Iterator62 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Character[] <$>characters;
        internal string[] <$>locations;
        internal GuiPanelMap <>f__this;
        internal int <i>__0;
        internal ScenarioMapIcon <icon>__1;
        internal Character[] characters;
        internal string[] locations;

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
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 1;
                    goto Label_014D;

                case 1:
                    this.<i>__0 = 0;
                    goto Label_0105;

                case 2:
                    this.characters[this.<i>__0].Location = this.locations[this.<i>__0];
                    break;

                case 3:
                    this.$PC = -1;
                    goto Label_014B;

                default:
                    goto Label_014B;
            }
        Label_00F7:
            this.<i>__0++;
        Label_0105:
            if (this.<i>__0 < this.characters.Length)
            {
                if (this.<i>__0 >= this.locations.Length)
                {
                    goto Label_00F7;
                }
                this.<icon>__1 = this.<>f__this.GetMapIcon(this.locations[this.<i>__0]);
                if (this.<icon>__1 == null)
                {
                    goto Label_00F7;
                }
                this.$current = this.<>f__this.StartCoroutine(this.<icon>__1.Move(this.characters[this.<i>__0]));
                this.$PC = 2;
            }
            else
            {
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                this.$PC = 3;
            }
            goto Label_014D;
        Label_014B:
            return false;
        Label_014D:
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

    [CompilerGenerated]
    private sealed class <Animate>c__Iterator63 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal string <$>fromLocation;
        internal string <$>toLocation;
        internal GuiPanelMap <>f__this;
        internal ScenarioMapIcon <fromIcon>__0;
        internal ScenarioMapIcon <toIcon>__1;
        internal Card card;
        internal string fromLocation;
        internal string toLocation;

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
                    this.<fromIcon>__0 = this.<>f__this.GetMapIcon(this.fromLocation);
                    this.<toIcon>__1 = this.<>f__this.GetMapIcon(this.toLocation);
                    if ((this.<fromIcon>__0 == null) || (this.<toIcon>__1 == null))
                    {
                        break;
                    }
                    this.card.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
                    this.card.transform.position = this.<fromIcon>__0.transform.position;
                    this.card.SortingOrder = Constants.SPRITE_SORTING_ZOOM;
                    this.card.Show(true);
                    LeanTween.move(this.card.gameObject, this.<toIcon>__1.transform.position, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 1;
                    goto Label_01BF;

                case 1:
                    LeanTween.scale(this.card.gameObject, Vector3.zero, 0.2f).setEase(LeanTweenType.easeInOutQuad);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.2f));
                    this.$PC = 2;
                    goto Label_01BF;

                case 2:
                    this.card.Show(false);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.1f));
                    this.$PC = 3;
                    goto Label_01BF;

                case 3:
                    break;

                default:
                    goto Label_01BD;
            }
            this.$PC = -1;
        Label_01BD:
            return false;
        Label_01BF:
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

    [CompilerGenerated]
    private sealed class <ExamineLocation>c__Iterator5F : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Character <$>character;
        internal ScenarioMapIcon <$>icon;
        internal GuiPanelMap <>f__this;
        internal GuiWindowLocation <window>__0;
        internal Character character;
        internal ScenarioMapIcon icon;

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
                    this.character.Location = this.icon.ID;
                    this.<>f__this.transitionPanel.ZoomIn(this.icon);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1.25f));
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<window>__0.ShowMap(false);
                    if (Location.Current != null)
                    {
                        Location.Load(this.icon.ID);
                        break;
                    }
                    Game.UI.ShowLocationScene(this.icon.ID, false);
                    break;

                default:
                    goto Label_013C;
            }
            Location.Current.OnExamineAnyLocation();
            if (Turn.State == GameStateType.Move)
            {
                Turn.Proceed();
            }
            UI.Sound.Play(SoundEffectType.ZoomInLocationToken);
            this.<>f__this.transitionPanel.ZoomOut(this.icon);
            this.<window>__0.Refresh();
            this.<>f__this.ClearMapLocks();
            Turn.Map = false;
            this.<>f__this.Mode = MapModeType.Move;
            this.$PC = -1;
        Label_013C:
            return false;
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

    [CompilerGenerated]
    private sealed class <Move>c__Iterator60 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Character <$>character;
        internal ScenarioMapIcon <$>icon;
        internal bool <$>proceed;
        internal GuiPanelMap <>f__this;
        internal Card <followCard>__1;
        internal bool <isTurnOwnerMoving>__2;
        internal bool <locChangePenalty>__3;
        internal GuiWindowLocation <window>__0;
        internal Character character;
        internal ScenarioMapIcon icon;
        internal bool proceed;

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
                    this.<followCard>__1 = null;
                    if (((((this.<window>__0.layoutSummoner.Card != null) && (this.<window>__0.layoutSummoner.Card.Type == CardType.Villain)) && (Turn.SummonsSource == this.<window>__0.layoutSummoner.Card.ID)) && (((Location.Current.Deck.Count > 0) && (Location.Current.Deck[0].ID == this.<window>__0.layoutSummoner.Card.ID)) || ((Location.Current.Deck.Count > 1) && (Location.Current.Deck[1].ID == this.<window>__0.layoutSummoner.Card.ID)))) && (Turn.Current == Turn.InitialCharacter))
                    {
                        if (Turn.Card.ID == Turn.SummonsSource)
                        {
                            this.<followCard>__1 = Turn.Card;
                        }
                        else
                        {
                            this.<followCard>__1 = Location.Current.Deck[1];
                        }
                        this.<followCard>__1.Deck.Remove(this.<followCard>__1);
                    }
                    this.<>f__this.Pause(true);
                    UI.Window.Pause(true);
                    this.<isTurnOwnerMoving>__2 = this.character.ID == Party.Characters[Turn.InitialCharacter].ID;
                    this.$current = this.<>f__this.StartCoroutine(this.icon.Move(this.character));
                    this.$PC = 1;
                    goto Label_03A5;

                case 1:
                    if (!this.<isTurnOwnerMoving>__2 || Turn.IsIteratorInProgress())
                    {
                        break;
                    }
                    this.<>f__this.transitionPanel.ZoomIn(this.icon);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1.25f));
                    this.$PC = 2;
                    goto Label_03A5;

                case 2:
                    break;

                default:
                    goto Label_03A3;
            }
            this.<window>__0.ShowMap(false);
            if (Location.Current == null)
            {
                Game.UI.ShowLocationScene(this.icon.ID, false);
            }
            else
            {
                Location.Load(this.icon.ID);
            }
            if (this.<isTurnOwnerMoving>__2)
            {
                Turn.Number = Turn.Current;
            }
            this.<locChangePenalty>__3 = Scenario.Current.OnLocationChange();
            if (this.<isTurnOwnerMoving>__2)
            {
                this.<>f__this.OnPlayerMoved();
                Turn.Close = false;
            }
            if (this.<isTurnOwnerMoving>__2 && !Turn.IsIteratorInProgress())
            {
                UI.Sound.Play(SoundEffectType.ZoomInLocationToken);
                this.<>f__this.transitionPanel.ZoomOut(this.icon);
            }
            if (this.<followCard>__1 != null)
            {
                Location.Current.Deck.Add(this.<followCard>__1, DeckPositionType.Top);
            }
            this.<>f__this.ClearMapLocks();
            Turn.Map = false;
            this.<>f__this.Pause(false);
            UI.Window.Pause(false);
            if (!this.<locChangePenalty>__3)
            {
                this.<window>__0.Refresh();
                if (this.proceed)
                {
                    this.<>f__this.FinishMoveAdvanceTurn();
                }
            }
            else
            {
                Game.Events.Next();
            }
            this.$PC = -1;
        Label_03A3:
            return false;
        Label_03A5:
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

    [CompilerGenerated]
    private sealed class <MoveCharacter>c__Iterator61 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Character <$>character;
        internal string <$>locID;
        internal GuiPanelMap <>f__this;
        internal Character character;
        internal string locID;

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
                    this.<>f__this.Center(this.locID);
                    this.$current = this.<>f__this.StartCoroutine(this.<>f__this.Move(this.character, this.<>f__this.selectedIcon, false));
                    this.$PC = 1;
                    return true;

                case 1:
                    this.$PC = -1;
                    break;
            }
            return false;
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

