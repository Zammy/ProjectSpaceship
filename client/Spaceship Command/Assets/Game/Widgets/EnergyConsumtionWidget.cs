using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Networking;

public class EnergyConsumtionWidget : MonoBehaviour, IMessageReceiver
{
    //Set through Unity
    public Text Text;
    public Image Circle;
    public Image Spinner;
    public Stations Station;
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
                this.Text.color = Color.white;
            }
            else
            {
                this.Text.color = Color.red;
            }
        }
    }

	// Use this for initialization
	void Start () 
    {
	    CoreNetwork.Instance.Subscribe(this);
	}

    void OnDestroy()
    {
        CoreNetwork.Instance.Unsubscribe(this);
    }
	
	// Update is called once per frame
	void Update () 
    {
	    if (this.isOnline)
        {
            var rotation = this.Spinner.transform.rotation;
            var angles = rotation.eulerAngles;
            angles.z -= 1f;
            rotation.eulerAngles = angles;
            this.Spinner.transform.rotation = rotation;
        }
	}

    #region IMessageReceiver implementation

    public void ReceiveMsg(int connectionId, INetMsg msg)
    {
        var energyCons = msg as EnergyConsumtionMsg;
        if (energyCons != null && energyCons.Station == this.Station)
        {
            this.Text.text = string.Format("{0}", energyCons.EnergyConsumed.ToString("F1"));
        }
    }

    #endregion
}
