using System.Collections.Generic;
using UnityEngine;

public class AxisPath : MonoBehaviour
{
    public LineRenderer xLineRenderer;
    public LineRenderer yLineRenderer;
    public LineRenderer zLineRenderer;
    public GameObject arrowPrefab; // 3D model of an arrow
    public int numberOfArrows = 10;
    public float arrowSpeed = 2f;
    public float pathLength = 10f;
    public int pathResolution = 100;
    public float arrowScale;

    private List<GameObject> xArrows = new List<GameObject>();
    private List<GameObject> yArrows = new List<GameObject>();
    private List<GameObject> zArrows = new List<GameObject>();
    private float[] arrowProgress;

    public Material x;
    public Material y;
    public Material z;

    void Start()
    {
        // Initialize arrows and their progress along the paths
        for (int i = 0; i < numberOfArrows; i++)
        {
            GameObject xArrow = Instantiate(arrowPrefab);
            xArrow.transform.localScale = Vector3.zero;
            xArrow.GetComponent<MeshRenderer>().material = x;
            xArrows.Add(xArrow);

            GameObject yArrow = Instantiate(arrowPrefab);
            yArrow.transform.localScale = Vector3.zero;
            yArrow.GetComponent<MeshRenderer>().material = y;
            yArrows.Add(yArrow);

            GameObject zArrow = Instantiate(arrowPrefab);
            zArrow.transform.localScale = Vector3.zero;
            zArrow.GetComponent<MeshRenderer>().material = z;
            zArrows.Add(zArrow);
        }

        arrowProgress = new float[numberOfArrows];

        // Create the straight paths along X, Y, and Z axes
        CreateStraightPath(xLineRenderer, Vector3.right);
        CreateStraightPath(yLineRenderer, Vector3.up);
        CreateStraightPath(zLineRenderer, Vector3.forward);
    }

    void Update()
    {
        for (int i = 0; i < numberOfArrows; i++)
        {
            // Update the progress of each arrow along the paths
            arrowProgress[i] += Time.deltaTime * arrowSpeed;
            if (arrowProgress[i] > 1f) arrowProgress[i] = 0f;

            // Update arrow positions and scales along each axis
            UpdateArrow(xArrows[i], arrowProgress[i], Vector3.right);
            UpdateArrow(yArrows[i], arrowProgress[i], Vector3.up);
            UpdateArrow(zArrows[i], arrowProgress[i], Vector3.forward);
        }
    }

    void CreateStraightPath(LineRenderer lineRenderer, Vector3 direction)
    {
        Vector3[] points = new Vector3[pathResolution];
        for (int i = 0; i < pathResolution; i++)
        {
            float t = (float)i / (pathResolution - 1);
            points[i] = GetPointOnStraightPath(t, direction);
        }
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    Vector3 GetPointOnStraightPath(float t, Vector3 direction)
    {
        return direction * (t * pathLength - pathLength / 2f);
    }

    void UpdateArrow(GameObject arrow, float progress, Vector3 direction)
    {
        // Get the position on the straight path based on progress
        Vector3 position = GetPointOnStraightPath(progress, direction);

        // Update arrow position
        arrow.transform.position = position;

        // Update arrow scale (zero at ends, one at center)
        float scale = Mathf.Sin(progress * Mathf.PI) * arrowScale;
        arrow.transform.localScale = new Vector3(scale * 9f, scale * 9f, scale * 25f);

        // Align arrow to the direction of the path
        arrow.transform.rotation = Quaternion.LookRotation(direction);
    }
}
