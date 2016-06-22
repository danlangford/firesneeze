using System;
using UnityEngine;

public class GuiPanelScenarioWildcard : GuiPanel
{
    [Tooltip("reference to this panel's description")]
    public GuiLabel DescriptionLabel;
    [Tooltip("reference to this panel's title")]
    public GuiLabel TitleLabel;

    public override void Initialize()
    {
        base.Show(false);
    }

    public void Show(bool isVisible, float duration)
    {
        if (isVisible)
        {
            base.Show(true);
            Vector3 to = new Vector3(base.transform.position.x, base.transform.position.y - 4f, base.transform.position.z);
            LeanTween.move(base.gameObject, to, 0.5f).setEase(LeanTweenType.easeOutBounce);
        }
        else
        {
            Vector3 vector2 = new Vector3(base.transform.position.x, base.transform.position.y + 4f, base.transform.position.z);
            LeanTween.move(base.gameObject, vector2, 0.3f).setEase(LeanTweenType.easeInQuad);
        }
    }

    public void Show(string id, float duration)
    {
        ScenarioPowerTableEntry entry = ScenarioPowerTable.Get(id);
        if (entry != null)
        {
            this.TitleLabel.Text = entry.Name;
            this.DescriptionLabel.Text = entry.Description;
            this.Show(true, duration);
        }
    }
}

