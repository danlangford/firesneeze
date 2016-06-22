using System;

public class BlockExamineMoveCards : Block
{
    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutExamine.Deck.Move(0, window.layoutExamine.Deck.Count - 1);
            window.layoutExamine.Bottom++;
            window.layoutExamine.Top = 0;
            window.layoutExamine.Curve = true;
            window.layoutExamine.Refresh();
            window.layoutExamine.Curve = false;
        }
    }

    public override float Length =>
        0.3f;
}

