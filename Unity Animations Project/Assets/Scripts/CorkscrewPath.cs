using System.Collections.Generic;
using UnityEngine;

public class CorkscrewPath : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject arrowPrefab; // 3D model of an arrow
    public int numberOfArrows = 10;
    public float arrowSpeed = 2f;
    public float pathLength = 10f;
    public float corkscrewRadius = 1f;
    public float corkscrewFrequency = 1f;
    public float TargetcorkscrewRadius = 1f;
    public float TargetcorkscrewFrequency = 1f;
    public int pathResolution = 100;
    public float arrowScale;

    private List<GameObject> arrows = new List<GameObject>();
    private float[] arrowProgress;

    void Start()
    {
        // Initialize arrows and their progress along the path
        arrowProgress = new float[numberOfArrows];
        float offset = 1f / numberOfArrows; // Calculate the offset for each arrow
        for (int i = 0; i < numberOfArrows; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.transform.localScale = Vector3.zero; // Start with zero scale
            arrows.Add(arrow);
            arrowProgress[i] = i * offset; // Initialize with evenly spaced progress
        }
    }

    void Update()
    {
        // Create the corkscrew path
        CreateCorkscrewPath();

        for (int i = 0; i < numberOfArrows; i++)
        {
            // Update the progress of each arrow along the path
            arrowProgress[i] += Time.deltaTime * arrowSpeed;
            if (arrowProgress[i] > 1f) arrowProgress[i] -= 1f;

            // Get the position on the path based on progress
            Vector3 position = GetPointOnCorkscrew(arrowProgress[i]);

            // Apply the parent's rotation to the position
            position = transform.rotation * position;

            // Update arrow position
            arrows[i].transform.position = transform.position + position;

            // Update arrow scale (zero at ends, one at center)
            float scale = Mathf.Sin(arrowProgress[i] * Mathf.PI) * arrowScale;
            arrows[i].transform.localScale = new Vector3(scale * 9.652435f, scale * 9.652435f, scale * 32.36402f);

            // Align arrow to the path direction
            Vector3 direction = GetDirectionOnCorkscrew(arrowProgress[i]);
            direction = transform.rotation * direction;
            arrows[i].transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void CreateCorkscrewPath()
    {
        Vector3[] points = new Vector3[pathResolution];
        for (int i = 0; i < pathResolution; i++)
        {
            float t = (float)i / (pathResolution - 1);
            points[i] = GetPointOnCorkscrew(t);
            points[i] = transform.rotation * points[i];
        }
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    Vector3 GetPointOnCorkscrew(float t)
    {
        float z = t * pathLength - pathLength / 2f;
        float angle = t * corkscrewFrequency * Mathf.PI * 2f;
        float x = Mathf.Cos(angle) * corkscrewRadius;
        float y = Mathf.Sin(angle) * corkscrewRadius;
        return new Vector3(x, y, z);
    }

    Vector3 GetDirectionOnCorkscrew(float t)
    {
        Vector3 point = GetPointOnCorkscrew(t);
        Vector3 nextPoint = GetPointOnCorkscrew(t + 0.01f); // Small step forward
        return (nextPoint - point).normalized;
    }
}
