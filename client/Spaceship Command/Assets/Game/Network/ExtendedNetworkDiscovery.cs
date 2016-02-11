using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class ExtendedNetworkDiscovery : NetworkDiscovery 
{
    public event Action<string, string> ReceivedBroadcast;

    void Awake()
    {
        this.useNetworkManager = false;

        this.Initialize();
    }

    public override void OnReceivedBroadcast (string fromAddress, string data)
    {
        base.OnReceivedBroadcast (fromAddress, data);

        this.ReceivedBroadcast(fromAddress, data);
    }
}
