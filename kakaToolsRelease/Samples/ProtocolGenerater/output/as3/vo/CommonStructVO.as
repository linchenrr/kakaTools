// These Codes are generated by kakaTools ProtocolGenerater v1.3
// ------------------------------------------------------------------
//
// Copyright (c) 2015 linchen.
// All rights reserved.
//
// Email: superkaka.org@gmail.com
//
// ------------------------------------------------------------------

package protocol.vo
{
	import flash.utils.ByteArray;
  import protocol.*;
	import org.superkaka.KLib.net.protocol.*;

    //公用结构体
    public class CommonStructVO
    {
        
        //
        public var m_ushort:uint;
        //
        public var m_int:int;
        //
        public var m_uint:uint;
        //
        public var m_Boolean:Boolean;
        //
        public var m_Binary:ByteArray;
        //
        public var m_string:String;
        
        
        public function decode(binReader:ProtocolByteArray):void
        {
		
            m_ushort = binReader.readUnsignedShort();
            
            m_int = binReader.readInt();
            
            m_uint = binReader.readUnsignedInt();
            
            m_Boolean = binReader.readBoolean();
            
            m_Binary = new ByteArray();
            binReader.readBytes(m_Binary, 0, binReader.readInt());
            
            m_string = binReader.readUTF();
            
        }
        
        public function encode(binWriter:ProtocolByteArray):void
        {
		
            binWriter.writeShort(m_ushort);
            
            binWriter.writeInt(m_int);
            
            binWriter.writeUnsignedInt(m_uint);
            
            binWriter.writeBoolean(m_Boolean);
            
            binWriter.writeInt(m_Binary.length);
            binWriter.writeBytes(m_Binary);
            
            binWriter.writeUTF(m_string);
            
        }
        
    }

}
