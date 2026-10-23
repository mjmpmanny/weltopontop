using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public bool CanBePlaced;
    private float GridSize = 1f;

    public bool followGrid = true;
    public bool Place()
    {
        gameObject.layer = 0;
        return true;
    }

    public void MoveTo(Vector3 pos)
    {
        if (followGrid)
        {
            transform.position = new Vector3(GridSize*Mathf.Round(pos.x/GridSize), GridSize * Mathf.Floor(pos.y / GridSize), GridSize * Mathf.Round(pos.z / GridSize));
        } else
        {
            transform.position = pos;
        }


    }
    public void Rotate()
    {
        transform.Rotate(0,90,0);
    }
}
