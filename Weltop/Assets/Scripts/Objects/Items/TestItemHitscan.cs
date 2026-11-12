using UnityEngine;

public class TestItemHitscan : Item
{
    public override void OnHitOpponentHitscan(GameObject playerHit)
    {
        Shove.ApplyShoveHit(
            playerHit, currentPlayer.GetComponent<Shove>().playerCam.transform.forward, 2f);
    }
}