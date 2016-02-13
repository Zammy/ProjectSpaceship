using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Networking;

public enum ScanTargetType
{
    Asteroid
}

[Serializable]
public class ScanTarget
{
    public int ID;
    public float Distance;
    public Vector2 Direction;
    public ScanTargetType Type;
    public GameObject Object;

    public override string ToString()
    {
        return string.Format("[ScanTarget ID={4}, Distance={0}, Direction={1}, Type={2}, Object={3}]", Distance, Direction, Type, Object, ID);
    }
}

public class ScanCore : MonoBehaviour 
{
    //Set through Unity
    public Transform ShipTransform;
    //

    Allegiance allegiance;

    List<ScanTarget> objectsInRange = new List<ScanTarget>();

    const float SCAN_RANGE = 250f;

    const float UPDATE_SEND_RATE = 1f;
    float nextTargetsUpdate;

    int objIdCounter = 0;

	// Use this for initialization
	void Start () 
    {
        this.allegiance = this.GetComponent<AllegianceDesignator>().Allegiance;

        this.nextTargetsUpdate = Time.time + UPDATE_SEND_RATE;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        GameObject[] allScanObjects = GameObject.FindGameObjectsWithTag("ScanSignature");

        Vector3 shipPos = this.ShipTransform.position;
        foreach (var obj in allScanObjects)
        {
            ScanTarget objScanTarget = this.GetScanTargetForObject(obj);

            Vector3 objPos = obj.transform.position;
            Vector3 diffVec = objPos - shipPos;
            float distance = diffVec.magnitude;

            Vector2 direction = Quaternion.Inverse( ShipTransform.rotation ) * diffVec.normalized;
            if (distance < SCAN_RANGE)
            {
                if (objScanTarget == null)
                {
                    ScanTargetType type = obj.GetComponent<ScanSignature>().ScanTargetType;
                    var newScanTarget = new ScanTarget()
                    {
                        ID = this.objIdCounter++,
                        Distance = distance,
                        Direction = direction,
                        Type = type,
                        Object = obj
                    };
                    this.objectsInRange.Add(newScanTarget);

                    CoreNetwork.Instance.Send( new ScanTargetMsg()
                    {
                        ScanTarget = newScanTarget,
                        Action = ScanTargetMsg.Type.Add,
                        Allegiance = this.allegiance
                    });
                }
                else
                {
                    objScanTarget.Distance = distance;
                    objScanTarget.Direction = direction;
                }
            }
            else if (distance > SCAN_RANGE && objScanTarget != null)
            {
                CoreNetwork.Instance.Send( new ScanTargetMsg()
                {
                    ScanTarget = objScanTarget,
                    Action = ScanTargetMsg.Type.Remove,
                    Allegiance = this.allegiance
                });

                this.objectsInRange.Remove(objScanTarget);
            }
        }

        if (Time.time >this.nextTargetsUpdate)
        {
            var msg = new ScanTargetMsg()
            {
                ScanTarget = null,
                Action = ScanTargetMsg.Type.Update,
                Allegiance = this.allegiance
            };

            foreach(var scanTarget in this.objectsInRange)
            {
               msg.ScanTarget = scanTarget ;
               CoreNetwork.Instance.Send(msg);
            }
            this.nextTargetsUpdate = Time.time + UPDATE_SEND_RATE;
        }
	}

    ScanTarget GetScanTargetForObject(GameObject newObj)
    {
        foreach(var scanTarget in this.objectsInRange)
        {
            if (scanTarget.Object == newObj)
            {
                return scanTarget;
            }
        }
        return null;
    }

}
