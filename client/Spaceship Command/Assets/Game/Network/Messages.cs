using System;
using UnityEngine.Networking;
using UnityEngine;

namespace Network
{
    public enum MessageType
    {
        DerivedData = 0,
        Message2 = 1,
    }

    public abstract class ExMessageBase : MessageBase
    {
        protected abstract MessageType GetMessageType() ;

        public override void Serialize(NetworkWriter writer)
        {
            writer.WritePackedUInt32((uint) this.GetMessageType());
            
            base.Serialize(writer);
        }

        public static Type GetTypeOfMessage(MessageType type)
        {
            switch (type) {
            case MessageType.DerivedData:
            {
                return typeof(SampleMessage);
            }
            default:
                return null;
            }
        }

        public static ExMessageBase Deserialize(byte[] buffer)
        {
            var networkReader = new NetworkReader(buffer);
            var messageType = (MessageType)networkReader.ReadPackedUInt32();
            var type = ExMessageBase.GetTypeOfMessage(messageType);
            var msg = System.Activator.CreateInstance(type) as ExMessageBase;
            msg.Deserialize(networkReader);
            return msg;
        }
    }

    public class SampleMessage : ExMessageBase
    {
        int a = 1;
        string b = "2";
        Vector3 c = new Vector3(3, 3, 3);

        protected override MessageType GetMessageType()
        {
            return MessageType.DerivedData;
        }

        public override string ToString()
        {
            return string.Format("[DerivedData] a[{0}] b[{1}] c[{2}]", a, b, c);
        }
    }

//    DerivedData d = new DerivedData();
//    var networkWritter = new NetworkWriter();
//    d.Serialize(networkWritter);
}