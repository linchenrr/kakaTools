using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace protocol
{

    public partial class ProtocolCenter
    {

        static public event Action<BaseProtocolVO> OnDispatchMessage;

        static private Dictionary<int, Func<BaseProtocolVO>> dic_creater = new Dictionary<int, Func<BaseProtocolVO>>();

        static private Dictionary<MessageType, MessageHandlerGroup<BaseProtocolVO>> dic_handlerGroup = new Dictionary<MessageType, MessageHandlerGroup<BaseProtocolVO>>();

        static private MessageHandlerGroup<BaseProtocolVO> globalMessageHandlerGroup = new MessageHandlerGroup<BaseProtocolVO>();

        static public BaseProtocolVO CreateProtocolVO(int messageId)
        {
            return dic_creater[messageId]();
        }

        static public BaseProtocolVO CreateProtocolVO(MessageType messageType)
        {
            return CreateProtocolVO((int)messageType);
        }

        static public void RegisterCreater(MessageType messageType, Func<BaseProtocolVO> packageCreateFun)
        {
            dic_creater[(int)messageType] = packageCreateFun;
        }

        static public void RegisterGlobalMessageHandler(Action<BaseProtocolVO> handler)
        {
            globalMessageHandlerGroup.RegisterHandler(handler);
        }

        static public void RemoveGlobalMessageHandler(Action<BaseProtocolVO> handler)
        {
            globalMessageHandlerGroup.RemoveHandler(handler);
        }

        static public void RegisterMessageHandler(MessageType messageType, Action<BaseProtocolVO> handler)
        {

            MessageHandlerGroup<BaseProtocolVO> handlerGroup;
            if (dic_handlerGroup.TryGetValue(messageType, out handlerGroup) == false)
            {
                handlerGroup = dic_handlerGroup[messageType] = new MessageHandlerGroup<BaseProtocolVO>();
            }
            handlerGroup.RegisterHandler(handler);

        }

        static public void RemoveHandler(MessageType messageType, Action<BaseProtocolVO> handler)
        {
            MessageHandlerGroup<BaseProtocolVO> handlerGroup;
            if (dic_handlerGroup.TryGetValue(messageType, out handlerGroup))
            {
                handlerGroup.RemoveHandler(handler);
            }
        }

        static public void DispatchMessage(BaseProtocolVO msg)
        {

            globalMessageHandlerGroup.CallHandler(msg);

            MessageHandlerGroup<BaseProtocolVO> handlerGroup;
            if (dic_handlerGroup.TryGetValue(msg.MessageType, out handlerGroup))
            {
                handlerGroup.CallHandler(msg);
            }

            if (OnDispatchMessage != null)
                OnDispatchMessage(msg);
            //DispatchProtocolClassMessage(msg);

        }

    }
}
