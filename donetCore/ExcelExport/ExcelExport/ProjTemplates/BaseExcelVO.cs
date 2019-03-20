using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using KLib;
using Newtonsoft.Json;
using UnityEngine;

public class BaseExcelVO
{
    protected virtual void AfterInit()
    {

    }

    [JsonIgnore]
    public virtual string DetailString
    {
        get
        {
            return this.ToString();
        }
    }
    /*
    protected Dictionary<string, object> dic_dynamic = new Dictionary<string, object>();

    public object this[string key]
    {
        get
        {
            if (dic_dynamic.ContainsKey(key))
                return dic_dynamic[key];
#if UNITY_EDITOR
            l.LogError("对[" + this + "]访问不存在的动态属性:" + key);
            l.LogError(DetailString);
#endif
            return null;
        }
    }
    */
    public virtual void Decode(EndianBinaryReader binReader)
    {

    }

}

public class BaseExcelVOGeneric<TKey, TClass> : BaseExcelVO where TClass : BaseExcelVOGeneric<TKey, TClass>, new()
{
    static public string TableName { get; protected set; }
    static public string ClassName { get; protected set; }
    static public string PrimaryKeyName { get; protected set; }

    /// <summary>主键的值  子类必须覆写实现</summary>
    public virtual TKey PrimaryValue { get { throw new Exception(ClassName + "没有覆写PrimaryValue属性！"); } }

    /// <summary>数据在表中的索引</summary>
    public int Index { get; protected set; }

    /// <summary>上一条数据</summary>
    [JsonIgnore]
    public TClass Prev { get; protected set; }

    /// <summary>下一条数据</summary>
    [JsonIgnore]
    public TClass Next { get; protected set; }

    static public ReadOnlyCollection<string> HeaderNames { get; private set; }

    static public ReadOnlyCollection<string> HeaderTypes { get; private set; }

    static private Action<TClass, EndianBinaryReader>[] DecodeFuncList;
    static protected Func<string, Action<TClass, EndianBinaryReader>> GetDecodeFunc;

    //static protected void UnknowFieldDecoder(TClass vo, EndianBinaryReader binReader)
    //{
    //    var fieldLen = binReader.ReadUInt16();
    //    binReader.BaseStream.Seek(fieldLen, SeekOrigin.Current);
    //}

    static protected bool hasFillData;
    static public void AutoFillData()
    {
        //l.Log("AutoFill ExcelData", TableName);
        if (hasFillData)
            return;
        var byteAsset = ResourceManager.Load<TextAsset>(@"excel/data/" + TableName + ".bytes");
        var bytes = byteAsset.bytes;

        if (Application.isPlaying)
        {
            UnityThreadRunner.RunAsync(() =>
            {
                Fill(bytes);
                AutoInitExcelData.ItemInitDone();
            });
        }
        else
        {
            Fill(bytes);
            AutoInitExcelData.ItemInitDone();
        }
    }

    static public void Fill(byte[] bytes)
    {
        MainTable = new ExcelTable(TableName, TableName);
        MainTable.Fill(bytes);
    }

    static public void AutoAddTables()
    {
        //l.Log("AutoFill ExcelData", TableName);
        var listAsset = ResourceManager.Load<TextAsset>(@"excel/data/shareMode/" + TableName + ".bytes");

        var binReader = new EndianBinaryReader(Endian.LittleEndian, new MemoryStream(listAsset.bytes));
        binReader.Endian = binReader.ReadBoolean() ? Endian.LittleEndian : Endian.BigEndian;
        var count = binReader.ReadInt32();
        restCount = count;
        for (int i = 0; i < count; i++)
        {
            var tbName = binReader.ReadUTF();
            var len = binReader.ReadInt32();
            var tableReader = new EndianBinaryReader(Endian.LittleEndian, new MemoryStream(listAsset.bytes, false));
            tableReader.BaseStream.Position = binReader.BaseStream.Position;
            if (Application.isPlaying)
            {
                UnityThreadRunner.RunAsync(() =>
                {
                    AddTable(tableReader, tbName);
                    onTableFill();
                });
            }
            else
            {
                AddTable(tableReader, tbName);
                onTableFill();
            }
            binReader.BaseStream.Seek(len, SeekOrigin.Current);
        }
    }

    static private int restCount;
    static private object lockObj = new object();
    static private void onTableFill()
    {
        lock (lockObj)
        {
            restCount--;
            if (restCount == 0)
            {
                //UnityThreadRunner.RunOnMainThread(() =>
                //{
                AutoAddExcelTables.ItemInitDone();
                //});
            }
            else if (restCount < 0)
            {
                throw new Exception(string.Format("{0} onTableFill restCount < 0 !!!", typeof(TClass).FullName));
            }
        }
    }

    static private void AddTable(EndianBinaryReader binReader, string tableName)
    {
        var table = new ExcelTable(tableName, TableName + "(" + tableName + ")");
        table.Fill(binReader);
        dic_table[tableName] = table;
    }

    static protected ExcelTable MainTable;

    static public ReadOnlyCollection<TClass> VOList
    {
        get
        {
            return MainTable.VOList;
        }
    }

    static public TClass GetVO(TKey primaryKey)
    {
        return MainTable.GetVO(primaryKey);
    }

    static public TClass TryGetVO(TKey primaryKey)
    {
        return MainTable.TryGetVO(primaryKey);
    }

    static public bool HasVO(TKey primaryKey)
    {
        return MainTable.HasVO(primaryKey);
    }

    static public List<TClass> Where(Func<TClass, bool> filter)
    {
        return MainTable.Where(filter);
    }

    [JsonIgnore]
    override public string DetailString
    {
        get
        {
#if UNITY_EDITOR
            return ClassName + JsonConvert.SerializeObject(this);
#else
            return ClassName + " " + PrimaryKeyName + ":" + PrimaryValue;
#endif
        }
    }

    //====share mode=====
    static private Dictionary<string, ExcelTable> dic_table = new Dictionary<string, ExcelTable>();

    static public TClass GetVO(string tableName, TKey primaryKey)
    {
        return GetTable(tableName).GetVO(primaryKey);
    }

    static public TClass TryGetVO(string tableName, TKey primaryKey)
    {
        return GetTable(tableName).TryGetVO(primaryKey);
    }

    static public bool HasVO(string tableName, TKey primaryKey)
    {
        return GetTable(tableName).HasVO(primaryKey);
    }

    static public ReadOnlyCollection<TClass> GetVOList(string tableName)
    {
        return GetTable(tableName).VOList;
    }

    static public List<TClass> Where(string tableName, Func<TClass, bool> filter)
    {
        return GetTable(tableName).Where(filter);
    }

    static public bool HasTable(string tableName)
    {
        return dic_table.ContainsKey(tableName);
    }

    static public ExcelTable GetTable(string tableName)
    {
        ExcelTable table = null;
        if (dic_table.TryGetValue(tableName, out table) == false)
        {
            throw new Exception(string.Format("类型{0}不存在表{1}！", typeof(TClass).FullName, tableName));
        }
        return table;
    }

    static public ExcelTable TryGetTable(string tableName)
    {
        ExcelTable table = null;
        dic_table.TryGetValue(tableName, out table);
        return table;
    }

    public class ExcelTable
    {
        public string Id { get; protected set; }
        public string Name { get; protected set; }

        public ExcelTable(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        protected Dictionary<TKey, TClass> dic_vo;
        protected List<TClass> list_vo;
        protected ReadOnlyCollection<TClass> list_voReadOnly;

        public ReadOnlyCollection<TClass> VOList
        {
            get
            {
                return list_voReadOnly;
            }
        }

        public TClass GetVO(TKey primaryKey)
        {
            TClass vo = null;
            if (dic_vo.TryGetValue(primaryKey, out vo) == false)
            {
                throw new Exception(string.Format("{0}表没有{1}为{2}的记录！", Name, PrimaryKeyName, primaryKey));
            }
            return vo;
        }

        public TClass TryGetVO(TKey primaryKey)
        {
            TClass vo = null;
            dic_vo.TryGetValue(primaryKey, out vo);
            return vo;
        }

        public bool HasVO(TKey primaryKey)
        {
            return dic_vo.ContainsKey(primaryKey);
        }

        public List<TClass> Where(Func<TClass, bool> filter)
        {
            return list_vo.Where(filter).ToList();
        }

        public void Fill(byte[] bytes)
        {
            var binReader = new EndianBinaryReader(Endian.LittleEndian, new MemoryStream(bytes));
            Fill(binReader);
        }

        public void Fill(EndianBinaryReader binReader)
        {
            var tmp_dic_vo = new Dictionary<TKey, TClass>();
            var tmp_list_vo = new List<TClass>();
            var tmp_headerNames = new List<string>();
            var tmp_headerTypes = new List<string>();

            binReader.Endian = binReader.ReadBoolean() ? Endian.LittleEndian : Endian.BigEndian;
            var jumpPos = binReader.ReadInt32();

            //跳过表头信息
            //binReader.BaseStream.Position = jumpPos;

            var tmp_DecodeFuncList = new List<Action<TClass, EndianBinaryReader>>();

            var headerCount = binReader.ReadInt32();
            for (var i = 0; i < headerCount; i++)
            {
                var fieldName = binReader.ReadUTF();
                var fieldlType = binReader.ReadUTF();
                var isArray = binReader.ReadBoolean();

                var decodeFunc = GetDecodeFunc(fieldName);
                if (decodeFunc != null)
                {
                    if (ExcelFieldReader.HasFieldLen(fieldlType))
                    {
                        tmp_DecodeFuncList.Add((vo, bReader) => ExcelFieldReader.SeekFieldLen(bReader));
                    }
                    tmp_DecodeFuncList.Add(decodeFunc);
                }
                else
                {
                    var unknowFieldDecoder = ExcelFieldReader.GetFieldReader(fieldlType, isArray);
                    tmp_DecodeFuncList.Add((vo, bReader) => { unknowFieldDecoder(bReader); });
                }

                if (isArray)
                    fieldlType += "[]";

                tmp_headerNames.Add(fieldName);
                tmp_headerTypes.Add(fieldlType);
            }
            DecodeFuncList = tmp_DecodeFuncList.ToArray();

            //if (HeaderNames != null)
            //{
            //    var isDiff = false;
            //    if (headerCount != HeaderNames.Count)
            //    {
            //        isDiff = true;
            //    }
            //    else
            //    {
            //        for (var i = 0; i < headerCount; i++)
            //        {
            //            if (tmp_headerNames[i] != HeaderNames[i] || tmp_headerTypes[i] != HeaderTypes[i])
            //            {
            //                isDiff = true;
            //                break;
            //            }
            //        }
            //    }

            //    if (isDiff)
            //        throw new Exception(string.Format("表{0}填充数据时异常:数据与表结构不一致！", TableName));
            //}

            var count = binReader.ReadInt32();
            TClass lastVO = null;
            for (int i = 0; i < count; i++)
            {
                var vo = new TClass();
                foreach (var decodeFunc in DecodeFuncList)
                {
                    decodeFunc(vo, binReader);
                }
                vo.AfterInit();
                //vo.Decode(binReader);
                vo.Index = i;

                if (tmp_dic_vo.ContainsKey(vo.PrimaryValue))
                    throw new Exception(string.Format("{0}表有重复{1}:{2}", Name, PrimaryKeyName, vo.PrimaryValue));

                tmp_list_vo.Add(vo);
                tmp_dic_vo.Add(vo.PrimaryValue, vo);

                if (lastVO != null)
                {
                    lastVO.Next = vo;
                    vo.Prev = lastVO;
                }
                lastVO = vo;
            }

            list_vo = tmp_list_vo;
            dic_vo = tmp_dic_vo;
            HeaderNames = new ReadOnlyCollection<string>(tmp_headerNames);
            HeaderTypes = new ReadOnlyCollection<string>(tmp_headerTypes);
            list_voReadOnly = new ReadOnlyCollection<TClass>(list_vo);

        }
    }
}
