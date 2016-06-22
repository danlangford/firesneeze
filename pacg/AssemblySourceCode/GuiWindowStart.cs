using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuiWindowStart : MonoBehaviour
{
    [Tooltip("black background texture will be stretched to full screen")]
    public Texture BlackBackground;

    private void Load(int slot)
    {
        Game.SlotToLoad = -1;
        Game.Load(slot);
    }

    private void OnGUI()
    {
        Rect position = new Rect(0f, (float) Screen.height, (float) Screen.width, (float) Screen.height);
        GUI.DrawTexture(position, this.BlackBackground);
    }

    private void Start()
    {
        if (Game.SlotToLoad >= 0)
        {
            this.Load(Game.SlotToLoad);
        }
        else
        {
            Settings.Debug.SetupLicenses();
            SceneManager.LoadScene("menu");
        }
    }
}

