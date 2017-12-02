using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlaceDancePositions : MonoBehaviour
{
    public Transform parentTransform = null;
    public int numPositionsX = 5;
    public int numPositionsY = 15;
    public float areaWidth = 3f;
    public float areaHeight = 6f;
    public float randomOffsetX = 0.1f;
    public float randomOffsetY = 0.1f;

#if UNITY_EDITOR
    public void RemovePositions()
    {
        int numChildren = parentTransform.childCount;
        for (int childIdx = numChildren - 1; childIdx >= 0; --childIdx)
        {
            Undo.DestroyObjectImmediate(parentTransform.GetChild(childIdx).gameObject);
        }
    }

    public void PlacePositions()
    {
        RemovePositions();

        for (int y = 0; y < numPositionsY; ++y)
        {
            for (int x = 0; x < numPositionsX; ++x)
            {
                PlacePosition(x, y);
            }
        }
    }

    private void PlacePosition(int x, int y)
    {
        Vector3 localPosition = Vector3.zero;
        float midHeight = (numPositionsY - 1) * 0.5f;
        float midWidth = (numPositionsX - 1) * 0.5f;
        localPosition.x = Mathf.Clamp((midWidth - x + Random.Range(-randomOffsetX, randomOffsetX)) / (numPositionsX - 1), -0.5f, 0.5f) * areaWidth / transform.lossyScale.x;
        localPosition.y = Mathf.Clamp((midHeight - y + Random.Range(-randomOffsetY, randomOffsetY)) / (numPositionsY - 1), -0.5f, 0.5f) * areaHeight / transform.lossyScale.y;

        string positionName = "position_" + x.ToString("00") + "_" + y.ToString("00");
        GameObject newDancePositionGO = new GameObject(positionName, new System.Type[] { typeof(DancePosition) });
        Undo.RegisterCreatedObjectUndo(newDancePositionGO, "CreatePositions");

        Transform newDancePositionTransform = newDancePositionGO.transform;
        newDancePositionTransform.SetParent(parentTransform);
        newDancePositionTransform.localPosition = localPosition;
        newDancePositionTransform.localRotation = Quaternion.identity;
    }
#endif

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position, new Vector3(areaWidth, areaHeight, 0.1f));
    }
}
