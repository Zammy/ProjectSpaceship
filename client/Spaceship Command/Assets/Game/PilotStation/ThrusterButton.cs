using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ThrusterButton : MonoBehaviour
{
    public ThrusterType ThrusterType;

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
            if (this.ThrusterType != kvp.Key)
                continue;

            if (Input.GetKeyDown(kvp.Value))
            {
                this.PointerDown();
            }
            else if (Input.GetKeyUp(kvp.Value))
            {
                this.PointerUp();
            }
        }
    }

    public void PointerDown()
    {
        this.GetComponent<Image>().color = Color.red;

        Debug.Log("PointerDown() " + this.ThrusterType);

//        if (this.PlayerClient != null)
//        {
////            this.PlayerClient.ThrusterActivated(this.ThrusterType);
//            this.PlayerClient.CmdFireThruster(this.ThrusterType);
//        }
    }

    public void PointerUp()
    {
        Debug.Log("PointerUp() "  + this.ThrusterType);

        this.GetComponent<Image>().color = Color.white;

//        if (this.PlayerClient != null)
//        {
////            this.PlayerClient.ThrusterDeactivated(this.ThrusterType);
//            this.PlayerClient.CmdStopThruster(this.ThrusterType);
//        }
    }
}
