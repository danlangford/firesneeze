using System;
using UnityEngine;

public class ScenarioMap : MonoBehaviour
{
    [Tooltip("world-space distance from center to bottom edge of map")]
    public float mapBoundsBottom = 7f;
    [Tooltip("world-space distance from center to left edge of map")]
    public float mapBoundsLeft = 12f;
    [Tooltip("world-space distance from center to right edge of map")]
    public float mapBoundsRight = 10f;
    [Tooltip("world-space distance from center to top edge of map")]
    public float mapBoundsTop = 17f;
    [Tooltip("world-space music")]
    public AudioClip Music;
}

