using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuiWindowLoading : GuiWindow
{
    protected override void Awake()
    {
        TouchKit.removeAllGestureRecognizers();
    }

    private void HideAllCards()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            for (int j = 0; j < Party.Characters[i].Hand.Count; j++)
            {
                Party.Characters[i].Hand[j].Show(false);
            }
            for (int k = 0; k < Party.Characters[i].Deck.Count; k++)
            {
                Party.Characters[i].Deck[k].Show(false);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        this.HideAllCards();
        Game.UI.OptionsPanel.Show(false);
        Game.UI.NetworkTooltip.Show(false);
        Tutorial.Hide();
        GuiPanel.Unload();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        SceneManager.LoadScene(Level);
    }

    public static string Level
    {
        [CompilerGenerated]
        get => 
            <Level>k__BackingField;
        [CompilerGenerated]
        set
        {
            <Level>k__BackingField = value;
        }
    }

    public override WindowType Type =>
        WindowType.Loading;
}

