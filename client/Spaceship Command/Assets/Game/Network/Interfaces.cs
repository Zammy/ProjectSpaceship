using System;

namespace Networking
{
    public interface IMessageReceiver
    {
        void ReceiveMsg(int connectionId, INetMsg msg);
    }

    public interface ICoreNetwork 
    {
        void Subscribe(IMessageReceiver msgReceiver);
        void Unsubscribe(IMessageReceiver msgReceiver);
        void Send(INetMsg msg);

        void HostAndBroadcast();
        void ListenAsClientAndConnectToHost(string ip = null);
        void StopHostBroadcast();

        event Action Client_ConnectedToHost;
        event Action<int> Host_ClientConnected;
        event Action<int> Host_ClientDisconnected;
    }

    public class MockCoreNetwork : ICoreNetwork
    {
        public void Subscribe(IMessageReceiver msgReceiver){}
        public void Unsubscribe(IMessageReceiver msgReceiver){}
        public void Send(INetMsg msg){}

        public event Action Client_ConnectedToHost;
        public event Action<int> Host_ClientDisconnected;
        public event Action<int> Host_ClientConnected;

        public void HostAndBroadcast() {}
        public void ListenAsClientAndConnectToHost(string ip) {}
        public void StopHostBroadcast() {}

    }
}