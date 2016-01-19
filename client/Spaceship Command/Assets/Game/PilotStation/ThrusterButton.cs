using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Networking;

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

        CoreNetwork.Instance.Send( new ThrusterMsg(true, this.ThrusterType) );
    }

    public void PointerUp()
    {
        Debug.Log("PointerUp() "  + this.ThrusterType);

        this.GetComponent<Image>().color = Color.white;

        CoreNetwork.Instance.Send( new ThrusterMsg(false, this.ThrusterType) );
    }
}
