using UnityEngine;
using System.Collections;
using Networking;
using UnityEngine.Networking;

public class ThrusterController : MonoBehaviour, IMessageReceiver
{
    public Thruster[] Thrusters;
    public Allegiance Allegiance;

    void Start()
    {
        CoreNetwork.Instance.Subscribe(this);
    }

    void OnDestroy()
    {
        CoreNetwork.Instance.Unsubscribe(this);
    }
        

    public void ReceiveMsg(int connectionId, INetMsg msg)
    {
        var thrusterMsg = msg as ThrusterMsg;
        if (thrusterMsg != null)
        {
            if (thrusterMsg.Allegiance != this.Allegiance)
            {
                return;
            }
            var thruster = this.Thrusters[(int)thrusterMsg.type];
            thruster.IsActive = thrusterMsg.activate;
        }
    }
}
