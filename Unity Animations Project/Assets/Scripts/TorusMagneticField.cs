using System.Collections.Generic;
using UnityEngine;

public class TorusMagneticField : MonoBehaviour
{
    public GameObject lineRendererPrefab; // Prefab with a LineRenderer component
    public GameObject arrowPrefab;        // 3D model of an arrow
    public int numberOfLines = 50;
    public int pointsPerLine = 100;
    public float torusRadius = 5f;
    public float tubeRadius = 2f;
    public float arrowSpeed = 2f;
    public Color lineColor = Color.blue;
    public int arrowsPerCircle = 10;      // Number of arrows per circle
    public float arrowScale;

    private List<GameObject[]> arrows = new List<GameObject[]>();
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private float[][] arrowProgress;

    void Start()
    {
        arrowProgress = new float[numberOfLines][];

        for (int i = 0; i < numberOfLines; i++)
        {
            // Instantiate LineRenderer and set its properties
            GameObject lineRendererObj = Instantiate(lineRendererPrefab, transform);
            LineRenderer lineRenderer = lineRendererObj.GetComponent<LineRenderer>();
            lineRenderer.positionCount = pointsPerLine;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;

            // Calculate the magnetic field line path around the torus
            CreateMagneticFieldLine(lineRenderer, i);

            // Store LineRenderer and instantiate corresponding arrows
            lineRenderers.Add(lineRenderer);
            GameObject[] arrowArray = new GameObject[arrowsPerCircle];
            arrowProgress[i] = new float[arrowsPerCircle];

            for (int j = 0; j < arrowsPerCircle; j++)
            {
                GameObject arrow = Instantiate(arrowPrefab, transform); // Parent the arrow to the same object
                arrow.transform.localScale = Vector3.zero;
                arrowArray[j] = arrow;

                // Initialize arrow progress so that arrows are evenly spread out
                arrowProgress[i][j] = j / (float)arrowsPerCircle;
            }

            arrows.Add(arrowArray);
        }

        // Apply the parent object's rotation to all line points
        ApplyParentRotation();

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        for (int i = 0; i < numberOfLines; i++)
        {
            for (int j = 0; j < arrowsPerCircle; j++)
            {
                // Update the progress of each arrow along the path
                arrowProgress[i][j] += Time.deltaTime * arrowSpeed;
                if (arrowProgress[i][j] > 1f) arrowProgress[i][j] = 0f;

                // Update arrow position and scale along the magnetic field line
                UpdateArrow(arrows[i][j], arrowProgress[i][j], lineRenderers[i]);
            }
        }
    }

    void CreateMagneticFieldLine(LineRenderer lineRenderer, int lineIndex)
    {
        Vector3[] points = new Vector3[pointsPerLine];
        float theta = (lineIndex / (float)numberOfLines) * Mathf.PI * 2f; // Angle around the torus

        for (int i = 0; i < pointsPerLine; i++)
        {
            float t = (i / (float)(pointsPerLine - 1)) * Mathf.PI * 2f; // Angle along the torus tube
            points[i] = GetPointOnTorus(theta, t);
        }

        lineRenderer.SetPositions(points);
    }

    Vector3 GetPointOnTorus(float theta, float t)
    {
        float x = (torusRadius + tubeRadius * Mathf.Cos(t)) * Mathf.Cos(theta);
        float y = (torusRadius + tubeRadius * Mathf.Cos(t)) * Mathf.Sin(theta);
        float z = tubeRadius * Mathf.Sin(t);
        return new Vector3(x, y, z);
    }

    void UpdateArrow(GameObject arrow, float progress, LineRenderer lineRenderer)
    {
        // Calculate the position on the line based on progress
        int index = Mathf.RoundToInt(progress * (lineRenderer.positionCount - 1));
        Vector3 position = lineRenderer.GetPosition(index);

        // Transform the position based on the parent object's rotation
        position = transform.TransformPoint(position);

        // Update arrow position
        arrow.transform.position = position;

        // Update arrow scale (zero at ends, one at center)
        float scale = Mathf.Sin(progress * Mathf.PI) * arrowScale;
        arrow.transform.localScale = new Vector3(scale * 9f, scale * 9f, scale * 25f);

        // Align arrow to the direction of the magnetic field line
        int nextIndex = Mathf.Min(index + 1, lineRenderer.positionCount - 1);
        Vector3 direction = (lineRenderer.GetPosition(nextIndex) - lineRenderer.GetPosition(index)).normalized;
        direction = transform.TransformDirection(direction); // Apply the parent's rotation to the direction
        arrow.transform.rotation = Quaternion.LookRotation(direction);
    }

    void ApplyParentRotation()
    {
        foreach (var lineRenderer in lineRenderers)
        {
            Vector3[] points = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(points);

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = transform.TransformPoint(points[i]);
            }

            lineRenderer.SetPositions(points);
        }
    }
}
