using UnityEngine;

public abstract class BossEffect : ScriptableObject
{
    [Header("Display Info")]
    public string Name;
    [TextArea] public string Description;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(Name))
        {
            Name = name;
        }
    }

    public virtual void OnRoundStart(GameManager manager) { }
    public virtual void OnHandPlayStart(GameManager manager) { }
    public virtual void OnHandPlayEnd(GameManager manager) { }
}