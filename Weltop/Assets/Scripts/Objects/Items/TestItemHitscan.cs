using UnityEngine;

public class TestItemHitscan : Item
{
    public override void OnHitOpponentHitscan(GameObject playerHit)
    {
        Debug.Log($"Hit Player {playerHit.name}");
    }
}