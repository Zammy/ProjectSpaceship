using System;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;

namespace Network
{
    public static class MessageHandler
    {
        static Dictionary<Type, uint> msgIndexer = new Dictionary<Type, uint>();

        static uint GetMsgIndexFromType(Type type)
        {
            return msgIndexer[type];
        }

        static Type GetMsgTypeFromIndex(uint index)
        {
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
                        && type.Namespace == "Network")
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

        public static byte[] Serialize(MessageBase msg)
        {
            var networkWriter = new NetworkWriter();
            uint index = GetMsgIndexFromType( msg.GetType() );
            networkWriter.WritePackedUInt32( index );
            msg.Serialize(networkWriter);
            return networkWriter.AsArray();
        }

        public static MessageBase Deserialize(byte[] buffer)
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
            return msg;
        }
    }

    public class StationSelectMsg : MessageBase
    {
        public Allegiance Allegiance;
        public Stations Station;

        public StationSelectMsg()
        {
        }

        public StationSelectMsg( Allegiance allegiance, Stations station)
        {
            this.Allegiance = allegiance;
            this.Station = station;
        }
        public override string ToString()
        {
            return string.Format("[StationSelect] Allegiance={0} Station={1}", this.Allegiance, this.Station);
        }
    }
}