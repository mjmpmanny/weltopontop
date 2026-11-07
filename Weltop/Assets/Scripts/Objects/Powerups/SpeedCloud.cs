using UnityEngine;

public class SpeedCloud : PowerUp
{
    public override void OnDeactivate()
    {
        Debug.Log("Speed Cloud Deactivated");
    }

    public override void OnActivate()
    {
        Debug.Log("Speed Cloud Activated");
    }

    public override void OnPlayerLosePowerUp(PlayerObjectEffects playerEffects)
    {
        base.OnPlayerLosePowerUp(playerEffects);
        Debug.Log("Player Lost Speed Cloud Powerup");
    }
}
