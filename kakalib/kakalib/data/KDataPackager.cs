using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using KLib;
using System.Data;


namespace KLib
{
    public class KDataPackager
    {

        public const Boolean defaultWithTag = false;

        private MemoryStream ms;

        private BinaryReader binReader;
        private BinaryWriter binWriter;

        public Byte[] data
        {
            get
            {
                long position = ms.Position;
                Byte[] bytes = new Byte[(int)ms.Length];
                ms.Position = 0;
                ms.Read(bytes, 0, bytes.Length);
                ms.Position = position;
                return bytes;
            }
            set
            {
                ms.SetLength(0);
                ms.Write(value, 0, 0);
            }
        }

        public KDataPackager(Byte[] bytes = null)
        {

            if (null == bytes)
            {
                this.ms = new MemoryStream();
            }
            else
            {
                this.ms = new MemoryStream(bytes);
            }


            binReader = new BinaryReader(ms);
            binWriter = new BinaryWriter(ms);

        }

        public long position
        {
            get
            {
                return ms.Position;
            }
            set
            {
                ms.Position = value;
            }
        }

        public void resetPosition()
        {
            ms.Position = 0;
        }

        public void reset()
        {
            ms.SetLength(0);
        }

        public void writeByte(SByte value, Boolean withTag = defaultWithTag)
        {
            if (withTag) binWriter.Write(KDataFormat.BYTE);
            binWriter.Write(value);
        }

        public SByte readByte(Boolean withTag = defaultWithTag)
        {

            if (withTag) ms.Position += 1;
            return binReader.ReadSByte();

        }

        public void writeInt(int value, Boolean withTag = defaultWithTag)
        {
            if (withTag) binWriter.Write(KDataFormat.INT);
            binWriter.Write(value);
        }

        public int readInt(Boolean withTag = defaultWithTag)
        {
            if (withTag) ms.Position += 1;
            return binReader.ReadInt32();
        }

        public void writeUint(uint value, Boolean withTag = defaultWithTag)
        {
            if (withTag) binWriter.Write(KDataFormat.UINT);
            binWriter.Write(value);
        }

        public uint readUint(Boolean withTag = defaultWithTag)
        {
            if (withTag) ms.Position += 1;
            return binReader.ReadUInt32();
        }

        public void writeDouble(Double value, Boolean withTag = defaultWithTag)
        {
            if (withTag) binWriter.Write(KDataFormat.NUMBER);
            binWriter.Write(value);
        }

        public Double ReadDouble(Boolean withTag = defaultWithTag)
        {
            if (withTag) ms.Position += 1;
            return binReader.ReadDouble();
        }

        public void writeBoolean(Boolean value, Boolean withTag = defaultWithTag)
        {
            if (withTag) binWriter.Write(KDataFormat.BOOLEAN);
            binWriter.Write(value);
        }

        public Boolean readBoolean(Boolean withTag = defaultWithTag)
        {
            if (withTag) ms.Position += 1;
            return binReader.ReadBoolean();
        }

        public void writeString(String value, Boolean withTag = defaultWithTag)
        {
            if (withTag) binWriter.Write(KDataFormat.STRING);
            Byte[] ba = Encoding.UTF8.GetBytes(value);
            binWriter.Write((UInt16)ba.Length);
            binWriter.Write(ba);
        }

        public String readString(Boolean withTag = defaultWithTag)
        {
            if (withTag) ms.Position += 1;
            UInt16 len = binReader.ReadUInt16();
            return Encoding.UTF8.GetString(binReader.ReadBytes((Int32)len));
        }

        public void writeBinary(Byte[] value, Boolean withTag = defaultWithTag)
        {
            if (withTag) binWriter.Write(KDataFormat.BINARY);
            binWriter.Write((UInt32)value.Length);
            binWriter.Write(value);
        }

        public Byte[] readBinary(Boolean withTag = defaultWithTag)
        {
            if (withTag) ms.Position += 1;
            UInt32 len = binReader.ReadUInt32();
            return binReader.ReadBytes((Int32)len);
        }

        public void writeArray(ArrayList value, Boolean withTag = defaultWithTag)
        {
            if (withTag) binWriter.Write(KDataFormat.ARRAY);

            int i = 0;
            int len = value.Count;

            binWriter.Write((UInt32)len);

            while (i < len)
            {

                writeValue(value[i]);

                i++;
            }

        }

        public void writeArray(Object[] value, Boolean withTag = defaultWithTag)
        {
            if (withTag) binWriter.Write(KDataFormat.ARRAY);

            int i = 0;
            int len = value.Length;

            binWriter.Write((UInt32)len);

            while (i < len)
            {

                writeValue(value[i]);

                i++;
            }

        }

        public ArrayList readArray(Boolean withTag = defaultWithTag)
        {
            if (withTag) ms.Position += 1;

            int i = 0;
            int len = (Int32)binReader.ReadUInt32();

            ArrayList array = new ArrayList(len);

            while (i < len)
            {
                array.Add(readValue());

                i++;
            }

            return array;
        }


        public void writeObject(Hashtable value, Boolean withTag = defaultWithTag)
        {

            if (withTag) binWriter.Write(KDataFormat.OBJECT);

            binWriter.Write((UInt32)value.Count);

            foreach (String key in value.Keys)
            {

                writeString(key, false);
                writeValue(value[key]);

            }
        }

        public Hashtable readObject(Boolean withTag = defaultWithTag)
        {
            if (withTag) ms.Position += 1;

            Hashtable h = new Hashtable();

            int i = 0;
            UInt32 len = binReader.ReadUInt32();

            while (i < len)
            {

                h[readString(false)] = readValue();

                i++;
            }

            return h;

        }


        //===========自定义复杂类型===============

        public void writeError(KRPCError error, Boolean withTag = defaultWithTag)
        {

            if (withTag) binWriter.Write(KDataFormat.ERROR);

            binWriter.Write(error.id);
            writeString(error.message, false);

        }

        public KRPCError readError(Boolean withTag = defaultWithTag)
        {
            if (withTag) ms.Position += 1;

            return new KRPCError(binReader.ReadInt32(), readString(false));

        }

        //static private DateTime dt_1970 = new DateTime(1970, 1, 1);
        //1970年1月1日刻度
        private const long tk_1970 = 621355968000000000;

        public void writeDate(DateTime date, Boolean withTag = defaultWithTag)
        {

            if (withTag) binWriter.Write(KDataFormat.DATE);
            //date = date.ToUniversalTime();
            TimeSpan ts = new TimeSpan(date.Ticks - tk_1970);
            Double sec = ts.TotalMilliseconds;
            binWriter.Write(sec);

        }

        public DateTime readDate(Boolean withTag = defaultWithTag)
        {

            if (withTag) ms.Position += 1;
            long time_Long = (long)binReader.ReadDouble();//长整型日期，毫秒为单位
            long time_tricks = tk_1970 + time_Long * 10000;//日志日期刻度
            DateTime dt = new DateTime(time_tricks);//转化为DateTime

            //return dt.ToLocalTime();
            return dt;
        }

        public void writeTable(KTable table, Boolean withTag = defaultWithTag)
        {

            if (withTag) binWriter.Write(KDataFormat.TABLE);

            binWriter.Write(table.primaryKeyIndex);

            DataTable dt = table.dataTable;

            binWriter.Write(dt.Rows.Count);
            binWriter.Write(dt.Columns.Count);

            String[] header = table.header;

            int i = 0;
            int len = header.Length;
            while (i < len)
            {
                writeString(header[i]);
                i++;
            }

            int rowNum = dt.Rows.Count;
            int columnNum = dt.Columns.Count;

            int row = 0;
            while (row < rowNum)
            {
                int column = 0;
                while (column < columnNum)
                {

                    writeString(dt.Rows[row][column].ToString());
                    column++;
                }

                row++;
            }

        }

        public KTable readTable(Boolean withTag = defaultWithTag)
        {

            if (withTag) ms.Position += 1;

            KTable table = new KTable();
            DataTable dt = new DataTable();

            table.dataTable = dt;

            int primaryKeyIndex = binReader.ReadInt32();

            int rowNum = binReader.ReadInt32();
            int columnNum = binReader.ReadInt32();

            String[] header = new String[columnNum];

            int row = 0;
            int column = 0;

            while (column < columnNum)
            {
                String columnName = readString(false);
                header[column] = columnName;
                dt.Columns.Add(columnName, typeof(String));
                column++;
            }

            table.header = header;
            table.primaryKeyIndex = primaryKeyIndex;

            while (row < rowNum)
            {

                column = 0;

                DataRow dr = dt.NewRow();

                while (column < columnNum)
                {


                    dr[column] = readString(false);

                    column++;

                }

                dt.Rows.Add(dr);

                row++;

            }

            return table;

        }

        //===========自定义复杂类型===============

        public void writeValue(Object value)
        {

            Type type = value.GetType();

            if (type.Equals(typeof(UInt32))) writeUint((UInt32)value, true);
            else if (type.Equals(typeof(Int32))) writeInt((Int32)value, true);
            else if (type.Equals(typeof(Double))) writeDouble((Double)value, true);
            else if (type.Equals(typeof(Boolean))) writeBoolean((Boolean)value, true);
            else if (type.Equals(typeof(String))) writeString((String)value, true);
            else if (type.Equals(typeof(Byte[]))) writeBinary((Byte[])value, true);
            else if (type.Equals(typeof(KRPCError))) writeError((KRPCError)value, true);
            else if (type.Equals(typeof(DateTime))) writeDate((DateTime)value, true);
            else if (type.Equals(typeof(KTable))) writeTable((KTable)value, true);
            else if (type.Equals(typeof(ArrayList))) writeArray((ArrayList)value, true);
            else if (type.Equals(typeof(Object[]))) writeArray((Object[])value, true);
            else if (type.Equals(typeof(Hashtable))) writeObject((Hashtable)value, true);

        }

        public Object readValue()
        {

            SByte type = binReader.ReadSByte();

            switch (type)
            {

                case KDataFormat.BYTE:
                    return readByte(false);

                case KDataFormat.UINT:
                    return readUint(false);

                case KDataFormat.INT:
                    return readInt(false);

                case KDataFormat.NUMBER:
                    return ReadDouble(false);

                case KDataFormat.BOOLEAN:
                    return readBoolean(false);

                case KDataFormat.STRING:
                    return readString(false);

                case KDataFormat.ERROR:
                    return readError(false);

                case KDataFormat.DATE:
                    return readDate(false);

                case KDataFormat.TABLE:
                    return readTable(false);

                case KDataFormat.BINARY:
                    return readBinary(false);

                case KDataFormat.ARRAY:
                    return readArray(false);

                case KDataFormat.OBJECT:
                    return readObject(false);

            }

            throw new Exception("无效的数据类型:" + type.ToString());

        }

    }
}
