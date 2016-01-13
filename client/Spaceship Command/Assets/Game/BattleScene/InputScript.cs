using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputScript : MonoBehaviour 
{
    private Dictionary<ThrusterType, KeyCode> keymap = new Dictionary<ThrusterType, KeyCode>()
    {
        { ThrusterType.ControlLeft, KeyCode.Q },
        { ThrusterType.ControlRight, KeyCode.E },
        { ThrusterType.MainLeft , KeyCode.A },
        { ThrusterType.MainRight , KeyCode.D },
    };

    void Update()
    {
        foreach(var kvp in keymap)
        {
            if (Input.GetKeyDown(kvp.Value))
            {
                PlayerClient.Instance.FireThruster(kvp.Key);
            }
            else if (Input.GetKeyUp(kvp.Value))
            {
                PlayerClient.Instance.StopThruster(kvp.Key);
            }
        }
    }
}
