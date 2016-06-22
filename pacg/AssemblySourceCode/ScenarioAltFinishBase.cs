using System;
using UnityEngine;

public abstract class ScenarioAltFinishBase : MonoBehaviour
{
    protected ScenarioAltFinishBase()
    {
    }

    public abstract bool IsScenarioOver();
    public abstract bool IsScenarioWon();
    public abstract void ScenarioCleanup();
}

