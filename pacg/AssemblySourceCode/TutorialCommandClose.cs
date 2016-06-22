using System;

public class TutorialCommandClose : TutorialCommand
{
    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Game.UI.TutorialPopup.Show(false);
        }
    }
}

