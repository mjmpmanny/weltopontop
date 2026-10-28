using UnityEngine;

public static class GlobalParameters
{
    //Player Stats
    public static float MoveSpeed = 5f;
    public static float JumpPower = 250f;
    public static float ShovePower = 100f;
    public static float ShoveCooldownSeconds = 1f;
    public static float InvincibilityFramesSeconds = 2f;
    public static float DelayBetweenPointsSeconds = 0.25f;
    public static float RespawnDelaySeconds = 2f;
    public static float RecoverAfterShoveDelaySeconds = 1f;
    
    //Round State Settings
    public static float ParkourRoundTimeSeconds = 120f;//120f;
    public static float BuildRoundTimeSeconds = 120f;//120f;
    public static float CountdownToGameSeconds = 5f;
    public static float StartRoundCountDownAtXSeconds = 10f; //At how many seconds will the big countdown to next round appear?
    
    //Feedback
    public static float ShoveCameraShakeDurationSeconds = 0.15f;
    public static float ShoveCameraShakeStrength = 2.25f;

}
