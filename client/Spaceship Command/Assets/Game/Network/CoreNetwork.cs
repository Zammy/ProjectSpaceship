﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System;
using System.Collections.Generic;

namespace Networking
{
    public interface IMessageReceiver
    {
        void ReceiveMsg(int connectionId, INetMsg msg);
    }

    public class CoreNetwork : MonoBehaviour 
    {
        //Set through Unity
        public ExtendedNetworkDiscovery NetworkDiscoverySecurity;
        public ExtendedNetworkDiscovery NetworkDiscoveryPirates;
        //

        public event Action<Allegiance> Client_ConnectedToHosts;
        public event Action<int> Host_ClientConnected;
        public event Action<int> Host_ClientDisconnected;

        string[] hosts = new string[2];
        const int HOSTPORT = 16661;
        Dictionary<int, int> connectionIdToHostIndex;

        List<IMessageReceiver> receivers = new List<IMessageReceiver>();

        public static CoreNetwork Instance;
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject) ;

            Instance = this;

            MessageHandler.Init();
        }

    	// Use this for initialization
    	void Start () 
        {
            this.NetworkDiscoverySecurity.ReceivedBroadcast += this.OnReceivedBroadcastFromSecurity;
            this.NetworkDiscoveryPirates.ReceivedBroadcast += this.OnReceivedBroadcastFromPirates;

            this.InitNetwork();
    	}

        void Update()
        {
            this.ReceiveNetwork();
        }

        public void HostAsSecurityAndBroadcast()
        {
            this.NetworkDiscoverySecurity.StartAsServer();

            this.Host(HOSTPORT);
        }

        public void HostAsPiratesAndBroadcast()
        {
            this.NetworkDiscoveryPirates.StartAsServer(); 

            this.Host(HOSTPORT + 1);
        }

        public void StopHostBroadcast()
        {
            this.NetworkDiscoverySecurity.StopBroadcast();
            this.NetworkDiscoveryPirates.StopBroadcast();
        }

        public void ListenAsClientAndConnectToHosts()
        {
            this.NetworkDiscoverySecurity.StartAsClient();
            this.NetworkDiscoveryPirates.StartAsClient();
        }

        public void Subscribe(IMessageReceiver msgReceiver)
        {
            if (this.receivers == null)
            {
                this.receivers = new List<IMessageReceiver>() ;   
            }

            foreach(var receiver in this.receivers)
            {
                if (receiver == msgReceiver)
                {
                    return;
                }
            }

            this.receivers.Add(msgReceiver);
        }

        public void Unsubscribe(IMessageReceiver msgReceiver)
        {
           this.receivers.Remove(msgReceiver);
        }

        public void Send(INetMsg msg)
        {
            byte[] data = MessageHandler.Serialize(msg);

            this.SendToAll( data );
        }

        public void SetHost(string ip, Allegiance allegiance)
        {
            this.hosts[(int)allegiance] = ip;
            this.Connect((int)allegiance);
        }

        void OnReceivedBroadcastFromSecurity(string fromAddress, string _)
        {
            Debug.Log("OnReceivedBroadcastFromSecurity " + fromAddress);

            this.ReceivedIPAddressOfHostOnIndex(fromAddress, 0);

            this.NetworkDiscoverySecurity.StopBroadcast();
        }

        void OnReceivedBroadcastFromPirates(string fromAddress, string _)
        {
            Debug.Log("OnReceivedBroadcastFromPirates " + fromAddress);

            this.ReceivedIPAddressOfHostOnIndex(fromAddress, 1);

            this.NetworkDiscoveryPirates.StopBroadcast();
        }

        void ReceivedIPAddressOfHostOnIndex(string ip, int index)
        {
            if (hosts[index] == null)
            {
                hosts[index] = ip;
                this.Connect(index);
            }
        }

        bool isHost;

        ConnectionConfig connectionConfig;
        int reliableChannelId;

        int hostId = -1;
        List<int> connectionIds = new List<int>();
        Dictionary<int, bool> isConnected;
        Dictionary<int, Allegiance> allegiances;

        void InitNetwork()
        {
            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            this.reliableChannelId = config.AddChannel(QosType.Reliable);
            this.connectionConfig = config;
        }

        void Host(int port)
        {
            this.isHost = true;

            var hostTopology = new HostTopology(connectionConfig, 6);
            this.hostId = NetworkTransport.AddHost(hostTopology, port);

            this.isConnected = new Dictionary<int, bool>();
            this.allegiances = new Dictionary<int, Allegiance>();

            Debug.LogFormat("[HOST] Hosting on port {0}", port);
        }

        void Connect(int index)
        {
            if (this.hostId == -1)
            {
                var hostTopology = new HostTopology(connectionConfig, 2);
                this.hostId = NetworkTransport.AddHost(hostTopology);

                this.isConnected = new Dictionary<int, bool>();
                this.connectionIdToHostIndex = new Dictionary<int, int>();
            }

            string ipaddress = this.hosts[index];
            int port = HOSTPORT + index;

            byte error;
            int connectionId = NetworkTransport.Connect(this.hostId, ipaddress, port, 0, out error);
            if (connectionId != 0)
            {
                this.connectionIds.Add(connectionId);
                this.connectionIdToHostIndex.Add(connectionId, index);
                this.isConnected[connectionId] = false;
            }

            Debug.LogFormat("[CLIENT] Connecting to {0}:{1}", ipaddress, port);

            if ((NetworkError) error != NetworkError.Ok)
            {
                Debug.LogErrorFormat("[CLIENT] Could not connect to {0}:{1} because {2}", ipaddress, port, (NetworkError)error);
            }
        }

        const int bufferSize = 1024;
        byte[] buffer = new byte[bufferSize]; 

        void ReceiveNetwork()
        {
            Array.Clear(buffer, 0, 1024);

            int recHostId; 
            int connectionId; 
            int channelId; 
            int dataSize;
            byte error;
            NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, buffer, bufferSize, out dataSize, out error);

            if ((NetworkError) error != NetworkError.Ok)
            {
                Debug.LogWarningFormat("Error while receiving {0} ", (NetworkError) error);
                if ((NetworkError) error == NetworkError.Timeout)
                {
                    this.ClientDisconnected(connectionId);
                }
                return;
            }

            switch (recData)
            {
                case NetworkEventType.ConnectEvent:
                {
                    if (this.isConnected.ContainsKey(connectionId))
                    {
                        Debug.LogFormat("[CLIENT] Connected connectionId {0}", connectionId);
                        this.isConnected[connectionId] = true;
                        this.RaiseClientConnectedToHost(connectionId); 
                    }
                    else
                    {
                        Debug.LogFormat("[HOST] Connection established to client {0}", connectionId);
                        connectionIds.Add(connectionId);
                        if (this.Host_ClientConnected != null)
                        {
                            this.Host_ClientConnected( connectionId );
                        }
                    }
                    break;
                }
                case NetworkEventType.DataEvent:
                {
                    var msg = MessageHandler.Deserialize(buffer);
                    this.SetAllegiance(msg, connectionId);
                    this.PushMessage(connectionId, msg);
                    break;
                }
                case NetworkEventType.DisconnectEvent:
                {
                    if (this.isConnected.ContainsKey(connectionId))
                    {
                        Debug.LogFormat("[CLIENT] Disconnected connectionId {0}", connectionId);
                        this.isConnected[connectionId] = false;
                    }
                    else
                    {
                        this.ClientDisconnected(connectionId);
                    }
                    break;
                }
            }
        }

        void SendToAll(byte[] data)
        {
            foreach (var connectionId in this.connectionIds)
            {
                byte error;
                NetworkTransport.Send(this.hostId, connectionId, reliableChannelId, data, data.Length, out error);

                if ((NetworkError) error != NetworkError.Ok)
                {
                    Debug.LogErrorFormat("Failed to send message {0}", (NetworkError) error );
                }
            }
        }

        void PushMessage(int connectionId, INetMsg msg)
        {
            Debug.LogFormat("Received message from {0} , {1}", connectionId, msg);
            foreach(var receiver in this.receivers)
            {
                receiver.ReceiveMsg(connectionId, msg);
            }
        }

        void RaiseClientConnectedToHost(int connectionId)
        {
            int hostIndex = this.connectionIdToHostIndex[connectionId];
            if (this.Client_ConnectedToHosts != null)
            {
                this.Client_ConnectedToHosts((Allegiance) hostIndex);
            }
        }

        void ClientDisconnected(int connectionId)
        {
            Debug.LogFormat("[HOST] Client disconnected {0}", connectionId);
            connectionIds.Remove(connectionId);
            if (this.Host_ClientDisconnected != null)
            {
                this.Host_ClientDisconnected(connectionId);
            }
        }

        void SetAllegiance(INetMsg msg, int connectionId)
        {
            if (!this.isHost)
                return;

            if (msg is StationSelectMsg)
            {
                this.allegiances[connectionId] = msg.Allegiance;
            }
            else
            {
                msg.Allegiance = this.allegiances[connectionId];
            }       
        }
    }

}