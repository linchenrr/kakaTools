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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KLib;

namespace protocol
{
    /// <summary>
    /// 请求玩家列表
    /// </summary>
    public class RequestPlayers : BaseProtocolVOGeneric<RequestPlayers>
    {
        
        
        
        public RequestPlayers() : base(MessageType.RequestPlayers)
        {

        }
        
        override public void decode(ProtocolBinaryReader binReader)
        {
        
        }
        
        override public void encode(ProtocolBinaryWriter binWriter)
        {
        
        }
        
    }

}
