using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnergyConsumtionWidget))]
public class EnergyConsupmtionWidgetEditor : Editor 
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Enable"))
        {
            ((EnergyConsumtionWidget) target).IsOnline = true;
        }

        if (GUILayout.Button("Disable"))
        {
            ((EnergyConsumtionWidget) target).IsOnline = false;
        }
    }
    
}
