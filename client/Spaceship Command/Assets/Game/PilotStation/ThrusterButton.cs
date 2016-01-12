using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ThrusterButton : MonoBehaviour
{
    public ThrusterType ThrusterType;

    PlayerClient playerClient = null;
    PlayerClient PlayerClient
    {
        get
        {
            if (this.playerClient == null)
            {
                this.playerClient = FindObjectOfType<PlayerClient>();
            }

            return this.playerClient;
        }
    }

    void Start()
    {
    }

    void Update()
    {
        switch (this.ThrusterType)
        {
            case ThrusterType.MainLeft:
            {
                if (Input.GetKey(KeyCode.A))
                {
                    this.PointerDown();
                }
                if (Input.GetKeyUp(KeyCode.A))
                {
                    this.PointerUp();
                }
                break;
            }
            case ThrusterType.MainRight:
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    this.PointerDown();
                }
                if (Input.GetKeyUp(KeyCode.D))
                {
                    this.PointerUp();
                }
                break;
            }
            default:
                break;
        }
    }

    public void PointerDown()
    {
        this.PlayerClient.ThrusterActivated(this.ThrusterType);
    }

    public void PointerUp()
    {
        this.PlayerClient.ThrusterDeactivated(this.ThrusterType);
    }
}
