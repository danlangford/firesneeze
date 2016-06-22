using System;
using UnityEngine;

public class GlossaryPopulator : UIPopulator
{
    [Tooltip("populates all the GlossaryEntry from the MasterList")]
    public GlossaryCategory Category;

    protected override GameObject ActivateClone(int index)
    {
        GameObject obj2 = base.ActivateClone(index);
        if (this.Category.GetEntryArray()[index] != null)
        {
            obj2.GetComponent<GuiLabel>().Text = this.Category.GetEntryArray()[index].Title.ToString();
        }
        GlossaryEntryButton component = obj2.GetComponent<GlossaryEntryButton>();
        component.Category = this.Category;
        component.Index = index;
        return obj2;
    }

    public override void Load(GameObject go)
    {
        base.Populate(this.Category.GetEntryArray().Length);
    }
}

