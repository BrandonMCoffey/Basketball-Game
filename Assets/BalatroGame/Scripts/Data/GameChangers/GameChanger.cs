using UnityEngine;

public abstract class GameChanger : ScriptableObject
{
    [Header("Display Info")]
    public string Name;
    [TextArea] public string Description;

    [Header("Shop Info")]
    public int Price = 100;

    protected GameChangerUI uiElement;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(Name))
        {
            Name = name;
        }
    }

    public virtual void Initialize(GameChangerUI ui)
    {
        uiElement = ui;
    }

    public virtual void OnHandPlayStart() { }
    public virtual bool OnScoreCalculationStart(ScoreData data) { return false; }
    public virtual bool OnCardContributesScore(CardUI card, ScoreData data) { return false; }
    public virtual bool OnScoreCalculationEnd(ScoreData data) { return false; }
}