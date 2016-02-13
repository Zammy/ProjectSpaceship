using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Networking;

public class InputScript : MonoBehaviour 
{
    public ThrusterController Controller;

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
                var msg = SpawnMsg();
                msg.activate = true;
                msg.type = kvp.Key;
                Controller.ReceiveMsg( msg, 42 );
            }
            else if (Input.GetKeyUp(kvp.Value))
            {
                var msg = SpawnMsg();
                msg.activate = false;
                msg.type = kvp.Key;
                Controller.ReceiveMsg(msg, 42);            
            }
        }
    }

    ThrusterMsg SpawnMsg()
    {
        var msg = new ThrusterMsg();
        msg.Allegiance = Allegiance.Security;
        return msg;
    }

}
