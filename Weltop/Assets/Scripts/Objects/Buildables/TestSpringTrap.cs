using UnityEngine;

public class TestSpringTrap : BuildableObject
{
    public override void OnTouch(GameObject playerTarget)
    {
        base.OnTouch(playerTarget);
        
        Debug.Log("Wowza! You touched a spring!");
    }
}
