// These Codes are generated by kakaTools ProtocolGenerater v1.4
// do NOT EDIT it.
// ------------------------------------------------------------------
//
// Copyright (c) 2010——2017 linchen.
// All rights reserved.
//
// Email: superkaka.org@gmail.com
//
// ------------------------------------------------------------------

namespace protocol
{
    
    public partial class ProtocolCenter
    {
        
        static ProtocolCenter()
        {
            
            RegisterCreater(MessageType.RequestWorldTalk, RequestWorldTalk.CreateInstance);
            
            RegisterCreater(MessageType.ResponseWorldTalk, ResponseWorldTalk.CreateInstance);
            
            RegisterCreater(MessageType.RequestPlayers, RequestPlayers.CreateInstance);
            
            RegisterCreater(MessageType.ResponsePlayers, ResponsePlayers.CreateInstance);
            
            
            OnDispatchMessage += DispatchProtocolClassMessage;
        }
        
        static public void DispatchProtocolClassMessage(BaseProtocolVO msg)
        {
            
            switch (msg.MessageType)
            {

                case MessageType.RequestWorldTalk:
                    RequestWorldTalk.CallHandler((RequestWorldTalk)msg);
                    break;

                case MessageType.ResponseWorldTalk:
                    ResponseWorldTalk.CallHandler((ResponseWorldTalk)msg);
                    break;

                case MessageType.RequestPlayers:
                    RequestPlayers.CallHandler((RequestPlayers)msg);
                    break;

                case MessageType.ResponsePlayers:
                    ResponsePlayers.CallHandler((ResponsePlayers)msg);
                    break;

            }
        }
        
        
        
    }
}
