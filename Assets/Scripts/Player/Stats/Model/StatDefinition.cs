using UnityEngine;

[CreateAssetMenu(fileName = "StatDefinition", menuName = "Scriptable Objects/Stats/Stat Definition")]
public class StatDefinition : ScriptableObject
{
    public string statName = "Health";
    public float maxValue = 100f;
    public float minValue = 0f;
    public float startValue = 100f;

    [Header("Regeneration")]
    public bool regenerates = false;
    public float regenRate = 1f;
    public float regenDelay = 3f;

    [Header("UI")]
    public Color barColor = Color.red;
}
