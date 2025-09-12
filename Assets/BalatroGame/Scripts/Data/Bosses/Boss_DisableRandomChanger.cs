using UnityEngine;

[CreateAssetMenu(fileName = "BE_DisableRandomChanger", menuName = "Balatro/Boss Effects/Disable Random Changer")]
public class DisableRandomChangerEffect : BossEffect
{
    private GameChangerUI _disabledChanger;

    public override void OnHandPlayStart(GameManager manager)
    {
        var activeChangers = manager.GameChangerDisplay.GetOrderedChangers();
        if (activeChangers.Count > 0)
        {
            _disabledChanger = activeChangers[Random.Range(0, activeChangers.Count)];
            _disabledChanger.SetVisualsDisabled(true);
        }
    }

    public override void OnHandPlayEnd(GameManager manager)
    {
        if (_disabledChanger != null)
        {
            _disabledChanger.SetVisualsDisabled(false);
            _disabledChanger = null;
        }
    }
}