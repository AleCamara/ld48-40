using UnityEngine;

[ExecuteInEditMode]
public class LineRendererSynchronizer : MonoBehaviour
{
    public LineRenderer lineRenderer = null;
    public Transform[] lineRendererPoints = new Transform[0];

    private void Update()
    {
        int numPoints = lineRendererPoints.Length;
        Vector3[] points = new Vector3[numPoints];
        for (int pointIdx = 0; pointIdx < numPoints; ++pointIdx)
        {
            points[pointIdx] = lineRendererPoints[pointIdx].position;
        }
        lineRenderer.positionCount = numPoints;
        lineRenderer.SetPositions(points);

        if (Application.isPlaying)
        {
            lineRenderer.enabled = true;
        }
    }
}
