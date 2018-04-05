using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace KLib
{
    public class NetUtils
    {

        static private bool isLittleEndian;
        static NetUtils()
        {
            isLittleEndian = BitConverter.IsLittleEndian;
        }

        static private bool NeedConvert(Endian endian)
        {
            if (endian == Endian.LittleEndian)
                return !isLittleEndian;
            else
                return isLittleEndian;
        }

        public static int ConvertToEndian(int num, Endian endian)
        {
            if (NeedConvert(endian))
                num = ConvertEndian(num);
            return num;
        }

        public static uint ConvertToEndian(uint num, Endian endian)
        {
            if (NeedConvert(endian))
                num = (uint)ConvertEndian((int)num);
            return num;
        }

        public static short ConvertToEndian(short num, Endian endian)
        {
            if (NeedConvert(endian))
                num = ConvertEndian(num);
            return num;
        }

        public static ushort ConvertToEndian(ushort num, Endian endian)
        {
            if (NeedConvert(endian))
                num = (ushort)ConvertEndian((short)num);
            return num;
        }

        public static long ConvertToEndian(long num, Endian endian)
        {
            if (NeedConvert(endian))
                num = ConvertEndian(num);
            return num;
        }

        public static ulong ConvertToEndian(ulong num, Endian endian)
        {
            if (NeedConvert(endian))
                num = (ulong)ConvertEndian((long)num);
            return num;
        }

        public static long ConvertEndian(long host)
        {
            return (((long)ConvertEndian((int)host) & 0xFFFFFFFF) << 32)
                    | ((long)ConvertEndian((int)(host >> 32)) & 0xFFFFFFFF);
        }
        public static int ConvertEndian(int host)
        {
            return (((int)ConvertEndian((short)host) & 0xFFFF) << 16)
                    | ((int)ConvertEndian((short)(host >> 16)) & 0xFFFF);
        }
        public static short ConvertEndian(short host)
        {
            return (short)((((int)host & 0xFF) << 8) | (int)((host >> 8) & 0xFF));
        }

    }

    public enum Endian
    {
        BigEndian,
        LittleEndian
    }
}
