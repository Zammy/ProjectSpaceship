using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Networking;

public class SensorsTarget : MonoBehaviour 
{
    //Set through Unity
    public Text Name;
    public Text Distance;
    public Image Direction;
    public Button LockCameraButton;
    //

    bool lockedCamera;
    public bool LockedCamera 
    { 
        get
        {
            return this.lockedCamera;
        }
        set
        {
            this.lockedCamera = value;

            if (value)
            {
                this.LockCameraButton.image.color = Color.black;
            }
            else
            {
                this.LockCameraButton.image.color = Color.white;
            }
        }
    }

    ScanTarget target;
    public ScanTarget Target 
    { 
        get 
        {
            return this.target;
        }
        set
        {
            this.target = value;

            this.Name.text = target.Type.ToString();

            this.Distance.text = target.Distance.ToString("F") + "m";

            Vector2 direction = target.Direction;
            Quaternion localRotation = new Quaternion();
            localRotation.SetFromToRotation(Vector2.up, direction);
            this.Direction.transform.localRotation = localRotation;
        }
    }

    public void LockTargetButtonClicked()
    {
        Debug.Log("LockTargetButtonClicked()");

        this.LockedCamera = !this.LockedCamera;

        var msg = new LockCameraMsg()
        {
            TargetID = this.Target.ID,
            Action = this.LockedCamera ? LockCameraMsg.Type.LockCamera : LockCameraMsg.Type.UnlockCamera
        };

        CoreNetwork.Instance.Send(msg);
    }
}
