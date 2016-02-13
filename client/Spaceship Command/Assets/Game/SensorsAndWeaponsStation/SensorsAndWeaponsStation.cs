using UnityEngine;
using System.Collections;
using Networking;
using System.Collections.Generic;

public class SensorsAndWeaponsStation : MonoBehaviour , IMessageReceiver
{
    //Set through Unity
    public Transform TargetsTrans;

    public GameObject SensorsTargetPrefab;
    //

    void Start()
    {
        CoreNetwork.Instance.Subscribe(this);
    }

    void OnDestroy()
    {
        CoreNetwork.Instance.Unsubscribe(this);
    }

    List<SensorsTarget> targetsObjs = new List<SensorsTarget>();
    void OrderTargets()
    {
        this.targetsObjs.Clear();
        for (int i = this.TargetsTrans.childCount - 1; i >= 0; i--) 
        {
            Transform child = this.TargetsTrans.transform.GetChild(i);
            targetsObjs.Add(child.GetComponent<SensorsTarget>());
            child.SetParent(null);
        }

        targetsObjs.Sort((SensorsTarget t1, SensorsTarget t2) => 
        {
            return (int) (t1.Target.Distance - t2.Target.Distance);
        });

        foreach (var target in this.targetsObjs) 
        {
            target.transform.SetParent(this.TargetsTrans);
        }
    }

    #region IMessageReceiver implementation

    public void ReceiveMsg(INetMsg msg, int _)
    {
        var scanTargetMsg = msg as ScanTargetMsg;
        if (scanTargetMsg == null)
            return;

        Debug.Log("[SensorsAndWeapons] Received :" + scanTargetMsg);
        switch (scanTargetMsg.Action)
        {
            case ScanTargetMsg.Type.Add:
            {
                var newSensorTargetObj = (GameObject)Instantiate(this.SensorsTargetPrefab, Vector3.zero, Quaternion.identity);
                newSensorTargetObj.GetComponent<SensorsTarget>().Target = scanTargetMsg.ScanTarget;
                newSensorTargetObj.transform.SetParent( this.TargetsTrans );
                newSensorTargetObj.transform.localScale = Vector3.one;
                break;
            }
            case ScanTargetMsg.Type.Remove:
            {
                foreach(Transform sesnorTargetObj in this.TargetsTrans)
                {
                    var target = sesnorTargetObj.GetComponent<SensorsTarget>().Target;
                    if (target.ID == scanTargetMsg.ScanTarget.ID)
                    {
                        Destroy( sesnorTargetObj.gameObject );
                        break;
                    }
                }
                break;
            }
            case ScanTargetMsg.Type.Update:
            {
                foreach(Transform sensorTargetObj in this.TargetsTrans)
                {
                    var sensorTarget = sensorTargetObj.GetComponent<SensorsTarget>();
                    if (sensorTarget.Target.ID == scanTargetMsg.ScanTarget.ID)
                    {
                        sensorTarget.Target =  scanTargetMsg.ScanTarget;
                        break;
                    }
                }
                break;
            }
            default:
                break;
        }

        this.OrderTargets();
    }

    #endregion
}
