using UnityEngine;

[CreateAssetMenu(fileName = "BE_ReduceDiscards", menuName = "Balatro/Boss Effects/Reduce Discards")]
public class ReduceDiscardsEffect : BossEffect
{
    public override void OnRoundStart(BalatroManager manager)
    {
        manager.ModifyDiscards(-2);
    }
}