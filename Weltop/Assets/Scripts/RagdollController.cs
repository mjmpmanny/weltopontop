using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField]
    private Vector3 _testRagdollHitFrom;
    [SerializeField]
    private bool _testRagdoll;
    private void Update()
    {
        if (_testRagdoll)
        {
            _testRagdoll = false;
            Ragdoll(transform.position, _testRagdollHitFrom);
        }
    }
    public void Ragdoll(Vector3 hitpoint, Vector3 force)
    {
        GetComponentInChildren<PlayerAnimationController>().TriggerRagdoll(hitpoint, force);
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }

    public void GetUp()
    {
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = false;
    }
}
