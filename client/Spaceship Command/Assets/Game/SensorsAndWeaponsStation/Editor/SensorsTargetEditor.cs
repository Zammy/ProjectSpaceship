using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SensorsTarget))]
public class SensorsTargetEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Enable"))
        {
            ((SensorsTarget)target).Target = new ScanTarget()
            {
                Distance = 23.324f,
                Direction = Vector2.right,
                Type = ScanTargetType.Asteroid
            };
        }
    }
}
