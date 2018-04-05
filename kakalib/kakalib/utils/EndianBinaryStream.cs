using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KLib
{
    public class EndianBinaryWriter : BinaryWriter
    {

        private Endian endian = Endian.LittleEndian;

        public Endian Endian
        {
            get { return endian; }
            set
            {
                endian = value;
            }
        }

        public EndianBinaryWriter(Endian endian, Stream output)
            : base(output)
        {
            Endian = endian;
        }

        public EndianBinaryWriter(Endian endian, Stream output, Encoding encoding)
            : base(output, encoding)
        {
            Endian = endian;
        }

        public EndianBinaryWriter(Endian endian, Stream output, Encoding encoding, bool leaveOpen)
            : base(output, encoding, leaveOpen)
        {
            Endian = endian;
        }

        override public void Write(int value)
        {
            value = NetUtils.ConvertToEndian(value, endian);
            base.Write(value);
        }

        override public void Write(uint value)
        {
            value = NetUtils.ConvertToEndian(value, endian);
            base.Write(value);
        }

        override public void Write(short value)
        {
            value = NetUtils.ConvertToEndian(value, endian);
            base.Write(value);
        }

        override public void Write(ushort value)
        {
            value = NetUtils.ConvertToEndian(value, endian);
            base.Write(value);
        }

        override public void Write(long value)
        {
            value = NetUtils.ConvertToEndian(value, endian);
            base.Write(value);
        }

        override public void Write(ulong value)
        {
            value = NetUtils.ConvertToEndian(value, endian);
            base.Write(value);
        }

        public void WriteUTF(string value)
        {
            if (value == null)
                value = "";
            var bytes_string = Encoding.UTF8.GetBytes(value);
            Write((ushort)bytes_string.Length);
            Write(bytes_string);
        }

        public void WriteDate(DateTime value)
        {
            Write(TimeUtils.DateTimeToSeconds(value));
        }

    }

    public class EndianBinaryReader : BinaryReader
    {

        private Endian endian = Endian.LittleEndian;

        public Endian Endian
        {
            get { return endian; }
            set
            {
                endian = value;
            }
        }

        public EndianBinaryReader(Endian endian, Stream output)
            : base(output)
        {
            Endian = endian;
        }

        public EndianBinaryReader(Endian endian, Stream output, Encoding encoding)
            : base(output, encoding)
        {
            Endian = endian;
        }

        public EndianBinaryReader(Endian endian, Stream output, Encoding encoding, bool leaveOpen)
            : base(output, encoding, leaveOpen)
        {
            Endian = endian;
        }

        override public short ReadInt16()
        {
            var num = base.ReadInt16();
            num = NetUtils.ConvertToEndian(num, endian);
            return num;
        }

        override public ushort ReadUInt16()
        {
            var num = base.ReadUInt16();
            num = NetUtils.ConvertToEndian(num, endian);
            return num;
        }

        override public int ReadInt32()
        {
            var num = base.ReadInt32();
            num = NetUtils.ConvertToEndian(num, endian);
            return num;
        }

        override public uint ReadUInt32()
        {
            var num = base.ReadUInt32();
            num = NetUtils.ConvertToEndian(num, endian);
            return num;
        }

        override public long ReadInt64()
        {
            var num = base.ReadInt64();
            num = NetUtils.ConvertToEndian(num, endian);
            return num;
        }

        override public ulong ReadUInt64()
        {
            var num = base.ReadUInt64();
            num = NetUtils.ConvertToEndian(num, endian);
            return num;
        }

        public string ReadUTF()
        {
            return Encoding.UTF8.GetString(ReadBytes(ReadUInt16()));
        }

        public double ReadFloat()
        {
            var Integer = ReadInt32();
            var Decimal = ReadInt32();
            var str = Integer + "." + Decimal;
            return Convert.ToDouble(str);
            /*
            var str = ReadUTF();
            return Convert.ToDouble(str);*/
        }

        public DateTime ReadDate()
        {
            return TimeUtils.SecondsToDateTime(ReadUInt32());
        }

    }

}
