using UnityEngine;
using System.Collections;
using UnityEditor;
using Networking;

[CustomEditor(typeof(SensorsAndWeaponsStation))]
public class SensorsAndWeaponsStationEditor : Editor 
{
    int counter = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Add"))
        {
            ((SensorsAndWeaponsStation)target).ReceiveMsg(new ScanTargetMsg()
            {
                Action = ScanTargetMsg.Type.Add,
                ScanTarget = new ScanTarget()
                {
                    ID = counter++,
                    Distance = Random.Range(1f, 342f),
                    Direction = Vector2.up,
                    Type = ScanTargetType.Asteroid
                }
            }, 1);
        }
    }
}

