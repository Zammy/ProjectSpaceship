using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System;
using System.Collections.Generic;

namespace Networking
{
    public class CoreNetwork : MonoBehaviour, ICoreNetwork
    {
        //Set through Unity
        public ExtendedNetworkDiscovery NetworkDiscovery;
        //

        public event Action Client_ConnectedToHost;

        public event Action<int> Host_ClientConnected;
        public event Action<int> Host_ClientDisconnected;
        
        string hostIP;
        const int HOSTPORT = 16661;

        List<IMessageReceiver> receivers = new List<IMessageReceiver>();
        Dictionary<int, Allegiance> allegiances;

        bool isHost = false;

        ConnectionConfig connectionConfig;
        int reliableChannelId;

        int hostId = -1;
        bool isConnected = false;
        List<int> connectionIds = new List<int>();

        private static CoreNetwork _instance = null;
        public static ICoreNetwork Instance
        {
            get
            {
                if (_instance == null)
                {
                    #if UNITY_EDITOR
                    return new MockCoreNetwork();
                    #else
                    throw new UnityException("CoreNetwork is null!!!!!");
                    #endif
                }
                return _instance;
            }
        }
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject) ;

            _instance = this;

            MessageHandler.Init();

            this.InitNetwork();
        }

    	// Use this for initialization
    	void Start () 
        {
            this.NetworkDiscovery.ReceivedBroadcast += this.OnReceivedBroadcast;
    	}

        void Update()
        {
            this.ReceiveNetwork();
        }

        public void HostAndBroadcast()
        {
            this.NetworkDiscovery.StartAsServer();

            this.Host();
        }

        public void ListenAsClientAndConnectToHost(string ip = null)
        {
            Debug.Log("ListenAsClientAndConnectToHost " + ip);
            if (this.isConnected)
            {
                return;
            }
            if (ip != null)
            {
                this.hostIP = ip;
                this.Connect();
            }
            else
            {
                this.NetworkDiscovery.StartAsClient();
            }
        }

        public void StopHostBroadcast()
        {
            this.NetworkDiscovery.StopBroadcast();
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
            if (this.isHost)
            {
                Debug.LogFormat("[HOST] Sending to clients : {0}", msg);
            }
            else
            {
                Debug.LogFormat("[Client] Sending to host : {0}", msg);
            }

            byte[] data = MessageHandler.Serialize(msg);

            this.SendToAll( data );
        }

        void OnReceivedBroadcast(string fromAddress, string _)
        {
            Debug.Log("OnReceivedBroadcast " + fromAddress);
            if (this.isConnected)
            {
                return;
            }
            this.hostIP = fromAddress;
            this.Connect();
        }

        void InitNetwork()
        {
            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            this.reliableChannelId = config.AddChannel(QosType.Reliable);
            this.connectionConfig = config;
        }

        void Host()
        {
            this.isHost = true;
            this.allegiances = new Dictionary<int, Allegiance>();

            var hostTopology = new HostTopology(connectionConfig, 6);
            this.hostId = NetworkTransport.AddHost(hostTopology, HOSTPORT);

            if (this.hostId < 0)
            {
                Debug.LogErrorFormat("[HOST] Could not open socket on {0}", HOSTPORT);
            }
            else
            {
                Debug.LogFormat("[HOST] Hosting on port {0}", HOSTPORT);
            }
        }

        void Connect()
        {
            this.isHost = false;

            var hostTopology = new HostTopology(connectionConfig, 1);
            this.hostId = NetworkTransport.AddHost(hostTopology);

            string ipaddress = this.hostIP;
            int port = HOSTPORT;

            byte error;
            int connectionId = NetworkTransport.Connect(this.hostId, ipaddress, port, 0, out error);
            if (connectionId != 0)
            {
                connectionIds.Add(connectionId);
            }
            Debug.LogFormat("[CLIENT] Connecting to {0}:{1}  connectionId:{2}", ipaddress, port, connectionId);

            if ((NetworkError) error != NetworkError.Ok || connectionId == 0)
            {
                Debug.LogErrorFormat("[CLIENT] Could not connect to {0}:{1} because {2}", ipaddress, port, (NetworkError)error);
            }
        }

        const int BUFFER_SIZE = 1024;
        byte[] buffer = new byte[BUFFER_SIZE];

        void ReceiveNetwork()
        {
            if (this.hostId == -1) 
                return;

            NetworkEventType recData;
            do 
            {
                Array.Clear(buffer, 0, 1024);

                int recHostId; 
                int connectionId; 
                int channelId; 
                int dataSize;
                byte error;
                recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, buffer, BUFFER_SIZE, out dataSize, out error);

                if ((NetworkError) error != NetworkError.Ok)
                {
                    Debug.LogWarningFormat("Error while receiving {0} ", (NetworkError) error);
                    return;
                }

                switch (recData)
                {
                    case NetworkEventType.Nothing:
                    {
//                        Debug.Log("Nothing...");
                        break;
                    }
                    case NetworkEventType.ConnectEvent:
                    {
                        if (this.isHost)
                        {
                            Debug.LogFormat("[HOST] Connection established to client {0}", connectionId);
                            connectionIds.Add(connectionId);
                            if (this.Host_ClientConnected != null)
                            {
                                this.Host_ClientConnected( connectionId );
                            }
                        }
                        else
                        {
                            Debug.LogFormat("[CLIENT] Connected connectionId {0}", connectionId);
                            this.isConnected = true;
                            this.RaiseClientConnectedToHost(connectionId); 
                        }
                        break;
                    }
                    case NetworkEventType.DataEvent:
                    {
                        var msg = MessageHandler.Deserialize(buffer);
                        if (this.isHost)
                        {
                            this.SetAllegiance(msg, connectionId);
                        }
                        this.PushMessage(connectionId, msg);
                        break;
                    }
                    case NetworkEventType.DisconnectEvent:
                    {
                        if (this.isHost)
                        {
                            Debug.LogFormat("[HOST] Client disconnected {0}", connectionId);
                            connectionIds.Remove(connectionId);
                            if (this.Host_ClientDisconnected != null)
                            {
                                this.Host_ClientDisconnected(connectionId);
                            }
                        }
                        else
                        {
                            Debug.LogFormat("[CLIENT] Disconnected connectionId {0}", connectionId);
                            this.isConnected = false;
                            //TODO: add client disconnect event
                        }
                        break;
                    }
                }
            }
            while(recData != NetworkEventType.Nothing);
        }

        void SendToAll(byte[] data)
        {
            foreach (var connectionId in this.connectionIds)
            {
                byte error;
                NetworkTransport.Send(this.hostId, connectionId, reliableChannelId, data, data.Length, out error);

                if ((NetworkError) error != NetworkError.Ok)
                {
                    Debug.LogErrorFormat("Failed to send message [{0}]", (NetworkError) error );
                }
            }
        }

        void PushMessage(int connectionId, INetMsg msg)
        {
            Debug.LogFormat("Received message from {0} , {1}, {2}", connectionId, msg, msg.Allegiance);

            if (!this.isHost && msg.Allegiance != Global.Allegiance)
            {
                Debug.LogFormat("Message ignored, different allegiance");
                return;
            }

            foreach(var receiver in this.receivers)
            {
                receiver.ReceiveMsg(connectionId, msg);
            }
        }

        void RaiseClientConnectedToHost(int connectionId)
        {
            if (this.Client_ConnectedToHost != null)
            {
                this.Client_ConnectedToHost();
            }
        }

        void SetAllegiance(INetMsg msg, int connectionId)
        {
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