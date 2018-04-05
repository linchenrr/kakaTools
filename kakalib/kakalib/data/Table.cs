using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KLib
{
    public class Table : IEnumerable<TableRow>
    {

        private string[] headers;
        private string[] types;
        private List<TableRow> list_rows = new List<TableRow>();
        private Dictionary<string, int> dic_index = new Dictionary<string, int>();

        public IList<TableRow> Rows
        {
            get
            {
                return list_rows.AsReadOnly();
            }
        }

        public string[] Headers
        {
            get
            {
                return (string[])headers.Clone();
            }
        }

        public string[] FieldTypes
        {
            get
            {
                return (string[])types.Clone();
            }
        }

        public IEnumerator<TableRow> GetEnumerator()
        {
            return list_rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Fill(byte[] bytes)
        {
            var binReader = new EndianBinaryReader(Endian.LittleEndian, new MemoryStream(bytes));
            binReader.Endian = binReader.ReadBoolean() ? Endian.LittleEndian : Endian.BigEndian;


            //表头信息结束位置
            binReader.ReadInt32();


            var headerCount = binReader.ReadInt32();

            headers = new string[headerCount];
            types = new string[headerCount];
            for (var i = 0; i < headerCount; i++)
            {
                headers[i] = binReader.ReadUTF();
                types[i] = binReader.ReadUTF();

                dic_index[headers[i]] = i;
            }

            var count = binReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var dataRow = new object[headerCount];
                var row = new TableRow(dataRow, dic_index);
                list_rows.Add(row);
                for (int j = 0; j < types.Length; j++)
                {
                    dataRow[j] = binReader.ReadUTF();
                }
            }
        }

    }

    public class TableRow : IEnumerable
    {
        private object[] dataRow;
        private Dictionary<string, int> dic_index;

        public TableRow(object[] dataRow, Dictionary<string, int> dic_index)
        {
            this.dataRow = dataRow;
            this.dic_index = dic_index;
        }

        public object this[string key]
        {
            get
            {
                int index;
                if (dic_index.TryGetValue(key, out index))
                    return dataRow[index];
                return null;
            }
        }

        public object this[int index]
        {
            get
            {
                //if (index < 0 || index >= dataRow.Length - 1)
                //    return null;
                return dataRow[index];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return dataRow.GetEnumerator();
        }

    }

}
