using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameSaveIcon : GuiPanel
{
    [Tooltip("reference to the adventure art container in our hierarchy")]
    public GameObject AdventureArtHolder;
    [Tooltip("reference to a label in our hierarchy")]
    public GuiLabel AdventureName;
    [Tooltip("reference to a label in our hierarchy")]
    public GuiLabel[] CharacterClasses;
    [Tooltip("reference to an image in our hierarchy")]
    public GuiImage[] CharacterIcons;
    [Tooltip("reference to a label in our hierarchy")]
    public GuiLabel[] CharacterNames;
    [Tooltip("reference to the delete button in our hierarchy")]
    public GuiButton DeleteButton;
    [Tooltip("sound played when this save is deleted")]
    public AudioClip DeleteSound;
    private GameSaveFile file;
    [Tooltip("reference to the play button in our hierarchy")]
    public GuiButton PlayButton;
    [Tooltip("reference to the portrait container in our hierarchy")]
    public GameObject PortraitHolder;
    private string profile;
    [Tooltip("reference to a label in our hierarchy")]
    public GuiLabel SaveDate;
    [Tooltip("reference to a label in our hierarchy")]
    public GuiLabel SaveTime;
    [Tooltip("reference to a label in our hierarchy")]
    public GuiLabel ScenarioName;
    private int slot;

    public override void Clear()
    {
        this.slot = 0;
        this.file = null;
        this.profile = null;
        if (this.ScenarioName != null)
        {
            this.ScenarioName.Clear();
        }
        if (this.SaveTime != null)
        {
            this.SaveTime.Clear();
        }
        if (this.SaveDate != null)
        {
            this.SaveDate.Clear();
        }
        for (int i = 0; i < this.CharacterNames.Length; i++)
        {
            this.CharacterNames[i].Clear();
        }
        for (int j = 0; j < this.CharacterClasses.Length; j++)
        {
            this.CharacterClasses[j].Clear();
        }
        for (int k = 0; k < this.CharacterIcons.Length; k++)
        {
            this.CharacterIcons[k].Clear();
        }
        this.ClearBubbleNumbers("SetA");
        this.ClearBubbleNumbers("SetS");
    }

    private void ClearBubbleNumbers(string set)
    {
        Transform transform = base.transform.FindChild(set);
        if (transform != null)
        {
            for (int i = 0; i < transform.transform.childCount; i++)
            {
                transform.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void Delete()
    {
        if (!this.Empty)
        {
            UI.Sound.Play(this.DeleteSound);
            this.file.Delete();
            this.file = null;
            this.Owner.Refresh();
        }
    }

    private void Delete_No_Callback()
    {
    }

    private void Delete_Yes_Callback()
    {
        this.Delete();
        this.Owner.Show(true);
    }

    private static string GetBubbleName(string set, string num)
    {
        if (num == "B")
        {
            return (set + "/numbers_letters_deck_0");
        }
        if (num == "C")
        {
            return (set + "/numbers_letters_deck_C");
        }
        if (num == "0")
        {
            return (set + "/numbers_letters_deck_0");
        }
        if (num == "1")
        {
            return (set + "/numbers_letters_deck_1");
        }
        if (num == "2")
        {
            return (set + "/numbers_letters_deck_2");
        }
        if (num == "3")
        {
            return (set + "/numbers_letters_deck_3");
        }
        if (num == "4")
        {
            return (set + "/numbers_letters_deck_4");
        }
        if (num == "5")
        {
            return (set + "/numbers_letters_deck_5");
        }
        if (num == "6")
        {
            return (set + "/numbers_letters_deck_6");
        }
        return null;
    }

    public void Load(int slot)
    {
        this.Clear();
        this.slot = slot;
        if (!GameDirectory.Empty(slot))
        {
            this.file = new GameSaveFile(slot);
            if (this.file.Header != null)
            {
                this.LoadAdventureArt(this.file.Header.AdventureID);
                this.AdventureName.Text = this.file.Header.AdventureName;
                this.ScenarioName.Text = this.file.Header.ScenarioName;
                this.SaveTime.Text = this.file.Header.Time;
                this.SaveDate.Text = this.file.Header.Date;
                if (this.file.Header.IsValid())
                {
                    for (int i = 0; i < this.file.Header.CharacterNames.Length; i++)
                    {
                        if (i < this.CharacterIcons.Length)
                        {
                            this.CharacterIcons[i].Image = this.LoadCharacterArt(this.file.Header.CharacterIDs[i]);
                        }
                        if (i < this.CharacterNames.Length)
                        {
                            this.CharacterNames[i].Text = this.file.Header.CharacterNames[i];
                        }
                        if (i < this.CharacterClasses.Length)
                        {
                            this.CharacterClasses[i].Text = this.file.Header.CharacterClasses[i].ToText();
                        }
                    }
                }
                this.ShowBubbleNumber("SetA", this.file.Header.AdventureNumber.ToString());
                this.ShowBubbleNumber("SetS", this.file.Header.ScenarioNumber.ToString());
            }
        }
    }

    public void Load(string profile, ProfileTableEntry entry)
    {
        this.Clear();
        this.slot = 0;
        if (entry != null)
        {
            this.profile = profile;
            this.LoadAdventureArt(entry.AdventureID);
            this.AdventureName.Text = entry.AdventureName;
            this.ScenarioName.Text = entry.ScenarioName;
            this.SaveTime.Text = string.Empty;
            this.SaveDate.Text = string.Empty;
            for (int i = 0; i < entry.CharacterNames.Length; i++)
            {
                this.CharacterIcons[i].Image = this.LoadCharacterArt(entry.CharacterIDs[i]);
                this.CharacterNames[i].Text = entry.CharacterNames[i];
                this.CharacterClasses[i].Text = entry.CharacterClasses[i];
            }
            this.ShowBubbleNumber("SetA", entry.AdventurePathSet);
            this.ShowBubbleNumber("SetS", entry.ScenarioNumber);
        }
        this.DeleteButton.Show(false);
    }

    private void LoadAdventureArt(string ID)
    {
        GameObject prefab = Resources.Load<GameObject>("Art/Adventures/" + ID);
        if (prefab != null)
        {
            GameObject obj3 = Game.Instance.Create(prefab);
            if (obj3 != null)
            {
                obj3.transform.parent = this.AdventureArtHolder.transform;
                obj3.transform.localScale = Vector3.one;
                obj3.transform.localPosition = Vector3.zero;
            }
        }
    }

    private Sprite LoadCharacterArt(string ID)
    {
        Transform child = Geometry.GetChild(this.PortraitHolder.transform, ID);
        if (child != null)
        {
            SpriteRenderer component = child.GetComponent<SpriteRenderer>();
            if (component != null)
            {
                return component.sprite;
            }
        }
        return null;
    }

    private void OnDeleteButtonPushed()
    {
        if (!this.Empty && !UI.Busy)
        {
            this.Owner.Popup.Owner = this;
            this.Owner.Popup.Show(true);
            this.Owner.Popup.MessageText = UI.Text(0x12d);
            this.Owner.Popup.YesButtonText = UI.Text(0x12e);
            this.Owner.Popup.YesButtonCallback = "Delete_Yes_Callback";
            this.Owner.Popup.NoButtonText = UI.Text(0x12f);
            this.Owner.Popup.NoButtonCallback = "Delete_No_Callback";
        }
    }

    private void OnPlayButtonPushed()
    {
        if (!UI.Busy)
        {
            UI.Busy = true;
            if (!this.Play())
            {
                UI.Busy = false;
            }
        }
    }

    public override void Pause(bool isPaused)
    {
        this.Owner.Pause(isPaused);
    }

    public bool Play()
    {
        bool flag = true;
        if (this.Empty)
        {
            Game.Play(GameType.LocalSinglePlayer, this.slot, WindowType.CreateParty, null, false);
            return flag;
        }
        if (this.file != null)
        {
            return Game.Load(this.file.Slot);
        }
        if (this.profile != null)
        {
            DebugProfile profile = new DebugProfile();
            if (profile.Load(this.profile))
            {
                profile.Run();
            }
        }
        return flag;
    }

    public override void Refresh()
    {
        if (this.DeleteButton != null)
        {
            this.DeleteButton.Refresh();
        }
        if (this.PlayButton != null)
        {
            this.PlayButton.Refresh();
        }
    }

    private void ShowBubbleNumber(string set, string num)
    {
        string bubbleName = GetBubbleName(set, num);
        Transform transform = base.transform.FindChild(bubbleName);
        if (transform != null)
        {
            transform.gameObject.SetActive(true);
        }
    }

    public bool Empty =>
        ((this.file == null) && (this.profile == null));

    public GuiPanelLoad Owner { get; set; }

    public int Slot =>
        this.slot;
}

