using System;
using System.IO;
using KLib;

namespace protocol
{
    public abstract class BaseProtocolVO
    {

        public object customData;

        private MessageType messageType;
        private int messageId;

        public BaseProtocolVO(MessageType protocolType)
        {
            this.messageType = protocolType;
            this.messageId = (int)protocolType;
        }

        public MessageType MessageType
        {
            get { return messageType; }
        }

        public int MessageId
        {
            get { return messageId; }
        }

        public void decode(byte[] bytes)
        {
            var binReader = new ProtocolBinaryReader(new MemoryStream(bytes));
            decode(binReader);
        }

        //子类覆写
        public virtual void decode(ProtocolBinaryReader binReader)
        {

        }

        //子类覆写
        public virtual void encode(ProtocolBinaryWriter binWriter)
        {

        }

    }

}