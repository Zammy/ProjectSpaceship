using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnergyConsumtionWidget : MonoBehaviour 
{
    //Set through Unity
    public Text Text;
    public Image Image;
    //

    float consumtion = 0f;
    public float Consumtion
    {
        get
        {
            return this.consumtion;
        }
        set
        {
            this.consumtion = value;
            this.Text.text = value.ToString();
        }
    }

    bool isOnline = true;
    public bool IsOnline
    {
        get
        {
            return this.isOnline;
        }
        set
        {
            this.isOnline = value;
            if (value)
            {
                this.Image.color = Color.white;
            }
            else
            {
                this.Image.color = DISABLED_COLOR;
            }
        }
    }

    readonly Color DISABLED_COLOR = new Color(255,255,255,0.4f);

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (this.isOnline)
        {
            var rotation = this.Image.transform.rotation;
            var angles = rotation.eulerAngles;
            angles.z -= 1f;
            rotation.eulerAngles = angles;
            this.Image.transform.rotation = rotation;
        }
	}
}
