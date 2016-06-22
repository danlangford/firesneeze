using System;

public class GuiPanelTray : GuiPanel
{
    public GuiLayoutTray Tray;

    public void OnCloseButtonPushed()
    {
        this.Tray.Show(false);
    }
}

