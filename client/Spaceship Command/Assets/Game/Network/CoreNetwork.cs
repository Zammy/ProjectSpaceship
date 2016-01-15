using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic; 

namespace Network
{
    public class CoreNetwork : MonoBehaviour 
    {
        public Text Text;
        public ExtendedNetworkDiscovery NetworkDiscoverySecurity;
        public ExtendedNetworkDiscovery NetworkDiscoveryPirates;

        string[] hosts = new string[2];

        const int HOSTPORT = 16661;

        NetServer netServer;
        NetClient[] netClients = new NetClient[2];

    	// Use this for initialization
    	void Start () 
        {
            this.NetworkDiscoverySecurity.ReceivedBroadcast += this.OnReceivedBroadcastFromSecurity;
            this.NetworkDiscoveryPirates.ReceivedBroadcast += this.OnReceivedBroadcastFromPirates;

            NetManager.Init ();
    	}

        void Update()
        {
            NetManager.PollEvents();
        }

        #region NetworkDiscovery
     
        public void HostAsMain()
        {
            this.Text.text += "Advertisizing as main host...\n";

            this.NetworkDiscoverySecurity.StartAsServer();

            this.Host(HOSTPORT);
        }

        public void HostAsSecondary()
        {
            this.Text.text += "Advertisizing as second host...\n";

            this.NetworkDiscoveryPirates.StartAsServer(); 

            this.Host(HOSTPORT + 1);
        }

        public void ListenAsClient()
        {
            this.Text.text += "Listening for hosts...\n";

            this.NetworkDiscoverySecurity.StartAsClient();
            this.NetworkDiscoveryPirates.StartAsClient();
        }

        public void OnReceivedBroadcastFromSecurity(string fromAddress, string _)
        {
            Debug.Log("OnReceivedBroadcastFromSecurity " + fromAddress);

            this.ReceivedIPAddressOfHostOnIndex(fromAddress, 0);
        }

        public void OnReceivedBroadcastFromPirates(string fromAddress, string _)
        {
            Debug.Log("OnReceivedBroadcastFromPirates " + fromAddress);

            this.ReceivedIPAddressOfHostOnIndex(fromAddress, 1);
        }

        void ReceivedIPAddressOfHostOnIndex(string ip, int index)
        {
            if (hosts[index] == null)
            {
                hosts[index] = ip;
            }

            if (hosts[0] != null && hosts[1] != null)
            {
                this.Connect(0);
                this.Connect(1);
            }
        }
        #endregion

        void Host(int port)
        {
            this.netServer = NetManager.CreateServer(6, port);
            netServer.OnMessage = this.OnServerReceive;
        }

        void Connect(int index)
        {
            this.netClients[index] = NetManager.CreateClient();
            this.netClients[index].OnMessage = this.OnClientReceive;

            string ipaddress = this.hosts[index];
            int port = HOSTPORT + index;
            netClients[index].Connect(this.hosts[index], port);

            string msg = "Connecting to  " + ipaddress + ":" + port +"\n";
            this.Text.text += msg;
            Debug.Log(msg);
        }

        public void SendToServer()
        {
            foreach (var client in netClients)
            {
                if (client != null)
                {
                    client.SendMessage( new SampleMessage() );
                }
            }
        }

        void OnServerReceive( NetworkEventType net , int connectionId , int channelId , byte[] buffer , int datasize )
        {
            if (net == NetworkEventType.Nothing)
                return;

            string x = "Received " + net.ToString() + " on connectionId " + connectionId + " channelId " + channelId + "\n";

            if (net == NetworkEventType.DataEvent)
            {
                var msg = ExMessageBase.Deserialize(buffer);
                Debug.Log("Received :" + msg.ToString());
            }

            Debug.Log(x);

            this.Text.text += x;
        }

        void OnClientReceive ( NetworkEventType net , int connectionId , int channelId , byte[] buffer , int datasize )
        {
            if (net == NetworkEventType.Nothing)
                return;

            string x = "Received " + net.ToString() + " on connectionId " + connectionId + " channelId " + channelId + "\n";

            if (net == NetworkEventType.DataEvent)
            {
                var msg = ExMessageBase.Deserialize(buffer);
                Debug.Log("Received :" + msg.ToString());
            }

            Debug.Log(x);

            this.Text.text += x;
        }
    }

}