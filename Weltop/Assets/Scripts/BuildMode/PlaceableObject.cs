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
        boxCollider = GetComponent<BoxCollider>();
        origialBoxColliderSize = boxCollider.size;
        ybound = boxCollider.bounds.min.y;
        boxCollider.size = new Vector3(
            (boxCollider.size.x*transform.localScale.x - 0.1f)/ transform.localScale.x,
            (boxCollider.size.y * transform.localScale.y - 0.1f) / transform.localScale.y,
            (boxCollider.size.z * transform.localScale.z - 0.1f) / transform.localScale.z
            );

        foreach (Transform tr in GetComponentsInChildren<Transform>())
        {
            tr.gameObject.layer = 3;
        }

        normalMat = GetComponent<Renderer>().material;
    }
}