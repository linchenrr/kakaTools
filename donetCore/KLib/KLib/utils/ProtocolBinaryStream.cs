using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KLib
{
    public class ProtocolBinaryWriter : BinaryWriter
    {

        public void WriteVarintInt(int value)
        {
            WriteVarintUInt(EncodeZigZag32(value));
        }

        public void WriteVarintLong(long value)
        {
            WriteVarintULong(EncodeZigZag64(value));
        }

        static public uint EncodeZigZag32(int n)
        {
            // Note:  the right-shift must be arithmetic
            return (uint)((n << 1) ^ (n >> 31));
        }
        static public ulong EncodeZigZag64(long n)
        {
            return (ulong)((n << 1) ^ (n >> 63));
        }

        public void WriteVarintUInt(uint value)
        {
            while (value > 127)
            {
                WriteRawByte((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
            WriteRawByte((byte)value);
        }

        public void WriteVarintULong(ulong value)
        {
            while (value > 127)
            {
                WriteRawByte((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
            WriteRawByte((byte)value);
        }

        private void WriteRawByte(byte value)
        {
            base.Write(value);
        }

        public ProtocolBinaryWriter(Stream output)
            : base(output)
        {

        }

        public ProtocolBinaryWriter(Stream output, Encoding encoding)
            : base(output, encoding)
        {

        }

        override public void Write(int value)
        {
            WriteVarintInt(value);
        }

        override public void Write(uint value)
        {
            WriteVarintUInt(value);
        }

        override public void Write(short value)
        {
            WriteVarintInt(value);
        }

        override public void Write(ushort value)
        {
            WriteVarintUInt(value);
        }

        override public void Write(long value)
        {
            WriteVarintLong(value);
        }

        override public void Write(ulong value)
        {
            WriteVarintULong(value);
        }

        override public void Write(string value)
        {
            if (value == null)
                value = "";
            var bytes_string = Encoding.UTF8.GetBytes(value);
            Write(bytes_string.Length);
            Write(bytes_string);
        }

        public void WriteFloat(float value)
        {
            var str = Convert.ToString(value);
            Write(str);
        }

        public void WriteDate(DateTime value)
        {
            Write(TimeUtils.DateTimeToSeconds(value));
        }

    }

    public class ProtocolBinaryReader : BinaryReader
    {

        static public int DecodeZigZag32(uint n)
        {
            return (int)(n >> 1) ^ -(int)(n & 1);
        }

        static public long DecodeZigZag64(ulong n)
        {
            return (long)(n >> 1) ^ -(long)(n & 1);
        }

        public int ReadVarintInt()
        {
            return DecodeZigZag32(ReadVarintUInt());
        }

        public long ReadVarintLong()
        {
            return DecodeZigZag64(ReadVarintULong());
        }

        public uint ReadVarintUInt()
        {
            int shift = 0;
            uint result = 0;
            while (shift < 32)
            {
                byte b = ReadByte();
                result |= (uint)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    return result;
                }
                shift += 7;
            }
            throw new Exception("ReadVarintUInt 长度超出预期");
        }

        public ulong ReadVarintULong()
        {
            int shift = 0;
            ulong result = 0;
            while (shift < 64)
            {
                byte b = ReadByte();
                result |= (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    return result;
                }
                shift += 7;
            }
            throw new Exception("ReadVarintULong 长度超出预期");
        }

        public ProtocolBinaryReader(Stream output)
            : base(output)
        {

        }

        public ProtocolBinaryReader(Stream output, Encoding encoding)
            : base(output, encoding)
        {

        }

        override public short ReadInt16()
        {
            return (short)ReadVarintInt();
        }

        override public ushort ReadUInt16()
        {
            return (ushort)ReadVarintUInt();
        }

        override public int ReadInt32()
        {
            return ReadVarintInt();
        }

        override public uint ReadUInt32()
        {
            return ReadVarintUInt();
        }

        override public long ReadInt64()
        {
            return ReadVarintLong();
        }

        override public ulong ReadUInt64()
        {
            return ReadVarintULong();
        }

        override public string ReadString()
        {
            return Encoding.UTF8.GetString(ReadBytes(ReadInt32()));
        }

        public float ReadFloat()
        {
            var str = ReadString();
            return Convert.ToSingle(str);
        }

        public DateTime ReadDate()
        {
            return TimeUtils.SecondsToDateTime(ReadUInt32());
        }

    }

}
