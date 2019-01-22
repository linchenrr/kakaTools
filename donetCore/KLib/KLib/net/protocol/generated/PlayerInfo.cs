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
    /// 
    /// </summary>
    public class PlayerInfo : BaseProtocolVOGeneric<PlayerInfo>
    {
        
        
        /// <summary>
        /// 
        /// </summary>
        public long uid;
        
        /// <summary>
        /// 名字
        /// </summary>
        public string name;
        
        /// <summary>
        /// 状态
        /// </summary>
        public bool status;
        
        /// <summary>
        /// 类型
        /// </summary>
        public PlayerType type;
        
        /// <summary>
        /// 
        /// </summary>
        public float fff;
        
        /// <summary>
        /// 最大可重置次数（根据VIP等级计算）
        /// </summary>
        public int maxResetTimes;
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime;
        
        /// <summary>
        /// 道具id列表
        /// </summary>
        public int[] items;
        
        
        public PlayerInfo() : base(MessageType.None)
        {

        }
        
        override public void decode(ProtocolBinaryReader binReader)
        {
        
            uid = binReader.ReadInt64();
            
            name = binReader.ReadString();
            
            status = binReader.ReadBoolean();
            
            type = (PlayerType)binReader.ReadInt32();
            
            fff = binReader.ReadSingle();
            
            maxResetTimes = binReader.ReadInt32();
            
            createTime = binReader.ReadDate();
            
            var len_items = binReader.ReadInt32();
            items = new int[len_items];
            for (int i = 0; i < len_items; i++)
            {
                items[i] = binReader.ReadInt32();
            }
            
        }
        
        override public void encode(ProtocolBinaryWriter binWriter)
        {
        
            binWriter.Write(uid);
            
            binWriter.Write(name);
            
            binWriter.Write(status);
            
            binWriter.Write((int)type);
            
            binWriter.Write(fff);
            
            binWriter.Write(maxResetTimes);
            
            binWriter.WriteDate(createTime);
            
            int len_items = items.Length;
            binWriter.Write(len_items);
            for (int i = 0; i < len_items; i++)
            {
                binWriter.Write(items[i]);
            }
            
        }
        
    }

}