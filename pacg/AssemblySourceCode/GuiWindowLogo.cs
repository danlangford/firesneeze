using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuiWindowLogo : MonoBehaviour
{
    [Tooltip("reference to the busy box in this scene")]
    public GuiPanelBusy busyBox;
    [Tooltip("minimum number of seconds to display this screen")]
    public float Duration = 3f;
    private bool isLoading;
    private float totalTime;
    [Tooltip("reference to the unity logo in this scene")]
    public GuiImage UnityLogo;

    private void Continue()
    {
        SceneManager.LoadSceneAsync("start");
    }

    private void Update()
    {
        this.busyBox.Tick();
        if (!this.isLoading)
        {
            this.totalTime += Time.deltaTime;
            if (this.totalTime >= this.Duration)
            {
                this.isLoading = true;
                this.Continue();
            }
        }
    }
}

