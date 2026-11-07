using UnityEngine;

public class FloorDoor : BuildableObject
{
    public override void OnStateChange()
    {
        Debug.Log("State changed");
        if (currentState == 0)
        {
            animator.SetBool("Open", false);
        }
        else
        {
            animator.SetBool("Open", true);
        }
    }
}
