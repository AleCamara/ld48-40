using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlaceDancePositions))]
public class PlaceDancePositionsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlaceDancePositions placeDancePositions = (target as PlaceDancePositions);
        if (GUILayout.Button(new GUIContent("Place Dance Positions", "Places all dance positions around the designated area")))
        {
            placeDancePositions.PlacePositions();
        }
        if (GUILayout.Button(new GUIContent("Remove Dance Positions", "Removes all dance positions around the designated area")))
        {
            placeDancePositions.RemovePositions();
        }
    }
}
