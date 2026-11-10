using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    private float GridSize = GlobalParameters.GridSize;
    private Vector3 offset = GlobalParameters.GridOffset;

    public Material incorrectMat;
    private Material normalMat;

    public bool followGrid = true;
    public bool IsPlaced { get; private set; }
    public bool IsBeingPlaced => placementStarted && !IsPlaced;

    private bool placementStarted;
    private int insideObject;
    private BoxCollider boxCollider;
    private Vector3 origialBoxColliderSize;
    private float ybound;
    public bool DeleteBoxColliderOnPlace;

    public void BeginPlacement()
    {
        placementStarted = true;

        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = 3;
        }
    }
    
    public bool Place()
    {
        if (insideObject > 0)
            return false;


        foreach (Transform tr in GetComponentsInChildren<Transform>())
        {
            tr.TryGetComponent<Collider>(out Collider cl);
            if (cl != null && cl.isTrigger && tr.gameObject != gameObject){ 
                tr.gameObject.layer = 1; 
            } else{
                tr.gameObject.layer = 0; 
            } 
        }
        IsPlaced = true;
        Destroy(GetComponent<Rigidbody>());
        if (DeleteBoxColliderOnPlace) {
            Destroy(boxCollider);
        } 
        else { 
            boxCollider.size = origialBoxColliderSize; 
        };
        return true;
    }

    public void EndOfRoundPlace()
    {
        if (insideObject == 0)
        {
            Place();
        }
        else
        {
            Debug.Log("destroyed object");
            Destroy(gameObject);
        }
    }

    public void MoveTo(Vector3 pos)
    {
        if (followGrid)
        {
            transform.position = new Vector3(
                GridSize * Mathf.Round(pos.x / GridSize),
                GridSize * Mathf.Round(pos.y / GridSize) - ybound - GridSize,
                GridSize * Mathf.Round(pos.z / GridSize)) + offset;
        }
        else
        {
            transform.position = pos;
        }
    }

    public void Rotate()
    {
        transform.Rotate(0, 90, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 0)
            insideObject++;

        Debug.Log("collision" + collision.contactCount);

        if (insideObject > 0)
            GetComponent<Renderer>().material = incorrectMat;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == 0)
            insideObject--;

        if (insideObject == 0)
            GetComponent<Renderer>().material = normalMat;
    }

    private void Start()
    {
        normalMat = GetComponent<Renderer>().material;

        if (!IsBeingPlaced)
            return;

        boxCollider = GetComponent<BoxCollider>();
        origialBoxColliderSize = boxCollider.size;
        
        Vector3 halfSize = boxCollider.size * 0.5f;
        Vector3 centerOffset = transform.TransformVector(boxCollider.center);

        float verticalHalfExtent =
            Mathf.Abs(transform.TransformVector(Vector3.right * halfSize.x).y) +
            Mathf.Abs(transform.TransformVector(Vector3.up * halfSize.y).y) +
            Mathf.Abs(transform.TransformVector(Vector3.forward * halfSize.z).y);

        ybound = centerOffset.y - verticalHalfExtent;

        Vector3 scale = transform.localScale;
        boxCollider.size = new Vector3(
            (boxCollider.size.x * scale.x - 0.1f) / scale.x,
            (boxCollider.size.y * scale.y - 0.1f) / scale.y,
            (boxCollider.size.z * scale.z - 0.1f) / scale.z);
    }
}