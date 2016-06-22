using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ScenarioMapButton : MonoBehaviour
{
    [Tooltip("sound to play when button is pushed")]
    public AudioClip ClickSound;
    [Tooltip("name of event to send to invoke")]
    public string EventName;
    private GuiPanel myPanel;

    public void Awake()
    {
        this.myPanel = this.GetParentPanel();
    }

    private GuiPanel GetParentPanel()
    {
        GuiPanel component = null;
        if (base.transform.parent != null)
        {
            component = base.transform.parent.GetComponent<GuiPanel>();
            if ((component == null) && (base.transform.parent.parent != null))
            {
                component = base.transform.parent.parent.GetComponent<GuiPanel>();
            }
        }
        return component;
    }

    public void Show(bool isVisible)
    {
        base.gameObject.SetActive(isVisible);
        Animator component = base.GetComponent<Animator>();
        if (component != null)
        {
            component.SetBool("Glow", isVisible);
        }
        this.Visible = isVisible;
    }

    public void Tap()
    {
        UI.Sound.Play(this.ClickSound);
        if (this.myPanel != null)
        {
            this.myPanel.SendMessage(this.EventName);
        }
    }

    public bool Visible { get; private set; }
}

