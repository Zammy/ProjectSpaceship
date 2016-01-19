using System;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;

namespace Networking
{
    public static class MessageHandler
    {
        static Dictionary<Type, uint> msgIndexer = new Dictionary<Type, uint>();

        static uint GetMsgIndexFromType(Type type)
        {
            Debug.LogFormat("GetMsgIndexFromType() for type {0}", type);
            return msgIndexer[type];
        }

        static Type GetMsgTypeFromIndex(uint index)
        {
            Debug.LogFormat("GetMsgTypeFromIndex() for index {0}", index);

            foreach(var kvp in msgIndexer)
            {
                if (kvp.Value == index)
                {
                    return kvp.Key;
                }
            }

            return null;
        }

        public static void Init()
        {
            List<Type> allNetworkMessages = new List<Type>();
            foreach( var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly.GetTypes();
                foreach(var type in types)
                {
                    if (type.IsSubclassOf(typeof(MessageBase)) 
                        && type.Namespace == "Networking")
                    {
                        allNetworkMessages.Add(type);
                    }
                }
            }

            for (uint i = 0; i < allNetworkMessages.Count; i++)
            {
                Debug.LogFormat("Adding type {0} on index {1}", allNetworkMessages[(int)i], i);
                msgIndexer.Add(allNetworkMessages[(int)i], i);
            }
        }

        public static byte[] Serialize(INetMsg msg)
        {
            var networkWriter = new NetworkWriter();
            uint index = GetMsgIndexFromType( msg.GetType() );
            networkWriter.WritePackedUInt32( index );
            ((MessageBase)msg).Serialize(networkWriter);
            return networkWriter.AsArray();
        }

        public static INetMsg Deserialize(byte[] buffer)
        {
            var networkReader = new NetworkReader(buffer);
            uint messageIndex = networkReader.ReadPackedUInt32();
            Type type = GetMsgTypeFromIndex(messageIndex);
            if (type == null)
            {
                Debug.LogErrorFormat("Could not find type with index {0}", messageIndex);
                return null;
            }
            var msg = System.Activator.CreateInstance(type) as MessageBase;
            msg.Deserialize(networkReader);
            return (INetMsg)msg;
        }
    }

    public interface INetMsg
    {
        Allegiance Allegiance { get; set; }
    }

    public class StationSelectMsg : MessageBase, INetMsg
    {
        public Allegiance Allegiance 
        {
            get
            {
                return this.allegiance;
            }
            set
            {
                this.allegiance = value;
            }
        }

        public Allegiance allegiance;
        public Stations station;

        public StationSelectMsg()
        {
        }

        public StationSelectMsg( Allegiance allegiance, Stations station)
        {
            this.allegiance = allegiance;
            this.station = station;
        }
        public override string ToString()
        {
            return string.Format("[StationSelect] Allegiance={0} Station={1}", this.allegiance, this.station);
        }
    }

    public class StartBattleMsg : MessageBase, INetMsg
    {
        public Allegiance Allegiance { get; set; }
    }

    public class ThrusterMsg : MessageBase, INetMsg
    {
        public Allegiance Allegiance
        {
            get; set;
        }

        public bool activate;
        public ThrusterType type;

        public ThrusterMsg()
        {   
        }

        public ThrusterMsg(bool activate , ThrusterType type)
        {
            this.activate = activate;
            this.type = type;
        }

        public override string ToString()
        {
            return string.Format("[ThrusterMsg] Activate={0} Type={1}", activate, type);
        }
    }
}