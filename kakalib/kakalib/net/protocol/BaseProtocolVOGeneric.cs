using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace protocol
{
    public abstract class BaseProtocolVOGeneric<T> : BaseProtocolVO where T : new()
    {

        static private MessageHandlerGroup<T> handlerGroup = new MessageHandlerGroup<T>();

        static public T CreateInstance()
        {
            return new T();
        }

        public BaseProtocolVOGeneric(MessageType protocolType)
            : base(protocolType)
        {

        }

        static public void RegisterHandler(Action<T> handler)
        {
            handlerGroup.RegisterHandler(handler);
        }

        static public void RemoveHandler(Action<T> handler)
        {
            handlerGroup.RemoveHandler(handler);
        }

        static public void CallHandler(T msg)
        {
            handlerGroup.CallHandler(msg);
        }

    }

}
