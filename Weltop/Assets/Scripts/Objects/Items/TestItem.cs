using UnityEngine;

public class TestItem : Item
{
    public override void OnHitOpponentMelee(GameObject playerHit)
    {
        Debug.Log($"Successfully hit {playerHit.name}");
    }
}