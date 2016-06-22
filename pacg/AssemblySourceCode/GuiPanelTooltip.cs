using System;
using UnityEngine;

[RequireComponent(typeof(AnimationEventAudio))]
public class GuiPanelTooltip : GuiPanel
{
    [Tooltip("reference to the animator in our hierarchy")]
    public UnityEngine.Animator Animator;
    [Tooltip("should the panel automatically close?")]
    public bool AutoClose;
    [Tooltip("optional reference to different backgrounds in our hierarchy")]
    public PanelBackgroundType[] Backgrounds;
    [Tooltip("reference to the label in our hierarchy")]
    public GuiLabel Label;

    private void OnCloseButtonPushed()
    {
        this.Show(false);
    }

    private void SetupBackgroundImage()
    {
        if (this.Text != null)
        {
            for (int i = 0; i < this.Backgrounds.Length; i++)
            {
                if ((this.Text.Length <= this.Backgrounds[i].MaxTextLength) || (i == (this.Backgrounds.Length - 1)))
                {
                    this.Backgrounds[i].Background.SetActive(true);
                    for (int j = 0; j < this.Backgrounds.Length; j++)
                    {
                        if (j != i)
                        {
                            this.Backgrounds[j].Background.SetActive(false);
                        }
                    }
                    return;
                }
            }
        }
    }

    public override void Show(bool isVisible)
    {
        if (isVisible)
        {
            base.Show(isVisible);
            this.SetupBackgroundImage();
            UI.Sound.Play(SoundEffectType.GenericLayoutTrayOpen);
            this.Animator.SetBool("StayOn", !this.AutoClose);
            this.Animator.SetTrigger("ToolTipOn");
        }
        else
        {
            UI.Sound.Play(SoundEffectType.GenericLayoutTrayClose);
            this.Animator.SetTrigger("ToolTipOff");
        }
    }

    public string Text
    {
        get => 
            this.Label.Text;
        set
        {
            this.Label.Text = value;
        }
    }
}

