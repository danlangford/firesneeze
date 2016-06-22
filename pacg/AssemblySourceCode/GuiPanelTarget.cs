using System;
using UnityEngine;

public class GuiPanelTarget : GuiPanel
{
    [Tooltip("reference to the bar background in the UI frame")]
    public SpriteRenderer background;
    [Tooltip("pointer to the sprites for backgrounds based on # chars in party (1-6)")]
    public Sprite[] backgroundSprites;
    [Tooltip("reference to character button array in our hierarchy")]
    public GuiButton[] characterButtons;
    [Tooltip("reference to character button highlights in our hierarchy")]
    public GameObject[] highlightButtons;
    [Tooltip("reference to the none button in our hierarchy")]
    public GuiButton noneButton;
    [Tooltip("pointer to sprites that show up if the Turn.Optionaltarget is set to something besides none")]
    public Sprite[] optionalSprites;
    [Tooltip("reference to the title label in our hierarchy")]
    public GuiLabel titleLabel;

    private void AssignBackground(int numButtons)
    {
        if (numButtons <= 7)
        {
            this.background.sprite = this.backgroundSprites[2];
        }
        if (numButtons <= 5)
        {
            this.background.sprite = this.backgroundSprites[1];
        }
        if (numButtons <= 3)
        {
            this.background.sprite = this.backgroundSprites[0];
        }
    }

    public float Bump(int n)
    {
        Vector3 to = new Vector3(1.25f, 1.25f, 1f);
        LeanTween.scale(this.characterButtons[n].gameObject, to, 0.2f).setLoopPingPong(1);
        return 0.4f;
    }

    private int ButtonCount()
    {
        int num = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (Party.Characters[i].Active != ActiveType.Inactive)
            {
                num++;
            }
        }
        if (Turn.OptionalTarget != TargetPanelType.None)
        {
            num++;
        }
        return num;
    }

    private Vector3 GetLayoutPosition(GuiButton button, int n, int max)
    {
        float num = 1.35f;
        float num2 = 0f;
        if (max <= 7)
        {
            num2 = -0.85f;
        }
        if (max <= 5)
        {
            num2 = -0.8f;
        }
        if (max == 3)
        {
            num2 = -0.65f;
        }
        if (max <= 2)
        {
            num2 = -0.5f;
        }
        float num3 = (num2 * (((float) max) / 2f)) * num;
        return new Vector3(num3 + (n * num), button.transform.localPosition.y, button.transform.localPosition.z);
    }

    public override void Initialize()
    {
        this.AssignBackground(this.ButtonCount());
        this.noneButton.Show(false);
        this.Show(false);
    }

    private bool IsSelectionValid(int n)
    {
        if (UI.Window.Paused)
        {
            return false;
        }
        if (Turn.State == GameStateType.Give)
        {
            return false;
        }
        if (n >= Party.Characters.Count)
        {
            return false;
        }
        if (!Party.Characters[n].Alive)
        {
            return false;
        }
        if (Party.Characters[n].Active != ActiveType.Active)
        {
            return false;
        }
        return true;
    }

    private void Layout()
    {
        int n = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            switch (Party.Characters[i].Active)
            {
                case ActiveType.Inactive:
                    this.characterButtons[i].Show(false);
                    break;

                case ActiveType.Locked:
                    this.characterButtons[i].Show(true);
                    this.characterButtons[i].transform.localPosition = this.GetLayoutPosition(this.characterButtons[i], n, this.ButtonCount());
                    this.characterButtons[i].Tint(Color.gray);
                    this.characterButtons[i].Locked = true;
                    this.characterButtons[i].Refresh();
                    n++;
                    break;

                case ActiveType.Active:
                    this.characterButtons[i].Show(true);
                    this.characterButtons[i].transform.localPosition = this.GetLayoutPosition(this.characterButtons[i], n, this.ButtonCount());
                    this.characterButtons[i].Tint(Color.white);
                    this.characterButtons[i].Locked = false;
                    this.characterButtons[i].Refresh();
                    n++;
                    break;
            }
        }
        if (Turn.OptionalTarget != TargetPanelType.None)
        {
            this.noneButton.Show(true);
            this.noneButton.transform.localPosition = this.GetLayoutPosition(this.noneButton, n, this.ButtonCount());
            this.noneButton.Image = this.optionalSprites[((int) Turn.OptionalTarget) - 1];
            this.noneButton.Refresh();
        }
    }

    private int NextValidSelection()
    {
        for (int i = Turn.Number; i < Party.Characters.Count; i++)
        {
            if (this.IsSelectionValid(i))
            {
                return i;
            }
        }
        for (int j = 0; j < Turn.Number; j++)
        {
            if (this.IsSelectionValid(j))
            {
                return j;
            }
        }
        return -1;
    }

    private void OnCharacter1ButtonPushed()
    {
        this.TargetCharacter(0);
    }

    private void OnCharacter2ButtonPushed()
    {
        this.TargetCharacter(1);
    }

    private void OnCharacter3ButtonPushed()
    {
        this.TargetCharacter(2);
    }

    private void OnCharacter4ButtonPushed()
    {
        this.TargetCharacter(3);
    }

    private void OnCharacter5ButtonPushed()
    {
        this.TargetCharacter(4);
    }

    private void OnCharacter6ButtonPushed()
    {
        this.TargetCharacter(5);
    }

    private void OnCharacterNoneButtonPushed()
    {
        if ((Turn.OptionalTarget == TargetPanelType.Next) && (this.NextValidSelection() >= 0))
        {
            this.TargetCharacter(this.NextValidSelection());
        }
        else
        {
            Turn.Proceed();
        }
    }

    public override void Refresh()
    {
        if ((Turn.State == GameStateType.Target) || (Turn.State == GameStateType.Give))
        {
            this.AssignBackground(this.ButtonCount());
            this.background.enabled = true;
            this.Show(true);
            this.Layout();
            this.RefreshHighlights(true);
        }
        else
        {
            for (int i = 0; i < this.characterButtons.Length; i++)
            {
                this.characterButtons[i].Show(false);
                this.characterButtons[i].Refresh();
            }
            this.noneButton.Show(false);
            this.noneButton.Refresh();
            this.background.enabled = false;
            this.Show(false);
            this.RefreshHighlights(false);
        }
    }

    private void RefreshHighlights(bool visible)
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            this.highlightButtons[i].gameObject.SetActive(visible && Party.Characters[i].Selected);
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            for (int i = 0; i < this.characterButtons.Length; i++)
            {
                if (i < Party.Characters.Count)
                {
                    this.characterButtons[i].Image = Party.Characters[i].PortraitSmall;
                }
                else
                {
                    this.characterButtons[i].Show(false);
                }
            }
        }
    }

    private void TargetCharacter(int n)
    {
        if (this.IsSelectionValid(n))
        {
            if ((Turn.TargetType == TargetType.MultipleAnotherAtLocation) || (Turn.TargetType == TargetType.MultipleAtLocation))
            {
                Party.Characters[n].Selected = !Party.Characters[n].Selected;
                this.highlightButtons[n].SetActive(Party.Characters[n].Selected);
                Turn.Refresh();
            }
            else
            {
                Turn.Target = n;
                Turn.Proceed();
            }
        }
    }

    public string Title
    {
        get => 
            this.titleLabel.Text;
        set
        {
            this.titleLabel.Text = value;
        }
    }
}

