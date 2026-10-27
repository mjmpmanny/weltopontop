using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    private float GridSize = GlobalParameters.GridSize;
    private Vector3 offset = GlobalParameters.GridOffset;

    public Material incorrectMat;
    private Material normalMat;

    public bool followGrid = true;
    private int insideObject;
    public bool Place()
    {
        if (insideObject > 0) return false;
        gameObject.layer = 0;
        Destroy(GetComponent<Rigidbody>());
        return true;
    }

    public void MoveTo(Vector3 pos)
    {
        if (followGrid)
        {
            transform.position = (new Vector3(GridSize * Mathf.Round(pos.x / GridSize), GridSize * Mathf.Floor(pos.y / GridSize), GridSize * Mathf.Round(pos.z / GridSize)) + offset);
        } else
        {
            transform.position = pos;
        }


    }
    public void Rotate()
    {
        transform.Rotate(0,90,0);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 0)
        {
            insideObject++;
        }
        Debug.Log("collision" + collision.contactCount);
        if (insideObject > 0)
        {
            GetComponent<Renderer>().material = incorrectMat;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == 0)
        {
            insideObject--;
        }
        if (insideObject == 0)
        {
            GetComponent<Renderer>().material = normalMat;
        }
    }
    private void Start()
    {
        GetComponent<BoxCollider>().size -= Vector3.one *0.1f;
        normalMat = GetComponent<Renderer>().material;
    }
}
