using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SensorsTarget : MonoBehaviour 
{
    //Set through Unity
    public Text Name;
    public Text Distance;
    public Image Direction;
    //

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
}
