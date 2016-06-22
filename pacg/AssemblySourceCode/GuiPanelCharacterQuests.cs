using System;
using UnityEngine;

public class GuiPanelCharacterQuests : GuiPanel
{
    protected Character currentCharacter;
    [Tooltip("reference to the current level marker on this panel")]
    public GuiImage currentLevelMarker;
    [Tooltip("intro text label on this panel")]
    public GuiLabel IntroLabel;
    [Tooltip("pointer to the white line template")]
    public GameObject lineTemplate1;
    [Tooltip("pointer to the black line template")]
    public GameObject lineTemplate2;
    [Tooltip("amount of horizontal space in front of each line (indent)")]
    public float marginLeft = -2.96f;
    [Tooltip("amount of vertical space at the top of the list")]
    public float marginTop = -2f;
    [Tooltip("intro text displayed at the top of this panel")]
    public StrRefType Message;
    private GuiQuestLine[] questLines;
    [Tooltip("reference to the scrolling region on this panel")]
    public GuiScrollRegion rewardScroller;
    [Tooltip("reference to the tab button that brought us here")]
    public GuiButton TabButton;
    [Tooltip("amount of vertical space between each line")]
    public float yPadding = 0.65f;

    public override void Initialize()
    {
        if (Rules.IsQuestRewardAllowed())
        {
            this.rewardScroller.Owner = this;
            this.rewardScroller.Initialize();
            float marginTop = this.marginTop;
            this.questLines = new GuiQuestLine[Game.Rewards.LevelCap + 1];
            for (int i = 1; i <= Game.Rewards.LevelCap; i++)
            {
                GameObject original = ((i % 2) != 1) ? this.lineTemplate2 : this.lineTemplate1;
                GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
                if (obj3 != null)
                {
                    this.rewardScroller.Add(obj3.transform, -this.marginLeft, marginTop);
                    marginTop += this.yPadding;
                    this.questLines[i] = obj3.GetComponent<GuiQuestLine>();
                    this.questLines[i].Initialize(i, Game.Rewards.Rewards[i]);
                }
            }
            float y = this.rewardScroller.Min.y + (Game.Rewards.LevelCap * this.yPadding);
            this.rewardScroller.Max = new Vector2(0f, y);
            this.IntroLabel.Text = string.Format(this.Message.ToString(), Game.Rewards.LevelCap);
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        this.rewardScroller.Pause(isPaused);
    }

    public override void Refresh()
    {
        if (this.questLines != null)
        {
            int level = this.Character.Level;
            for (int i = 1; i < this.questLines.Length; i++)
            {
                if (this.questLines[i].Level == level)
                {
                    this.currentLevelMarker.transform.position = new Vector3(this.currentLevelMarker.transform.position.x, this.questLines[i].transform.position.y, this.currentLevelMarker.transform.position.z);
                }
                this.questLines[i].Complete = this.questLines[i].Level <= level;
            }
        }
        this.rewardScroller.Top();
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.TabButton.Glow(isVisible);
        this.rewardScroller.Pause(!isVisible);
        if (isVisible)
        {
            this.Refresh();
        }
        if (!isVisible)
        {
            this.rewardScroller.Top();
        }
    }

    public Character Character
    {
        get
        {
            if (this.currentCharacter != null)
            {
                return this.currentCharacter;
            }
            if (Party.Characters.Count > Turn.Number)
            {
                return Party.Characters[Turn.Number];
            }
            return null;
        }
        set
        {
            this.currentCharacter = value;
        }
    }

    public override uint zIndex =>
        (Constants.ZINDEX_PANEL_FULL + 10);
}

