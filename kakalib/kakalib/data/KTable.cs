using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace KLib
{
    public class KTable
    {
        /// <summary>
        /// 工作薄显示名
        /// </summary>
        public String name;

        /// <summary>
        /// 字段名列表
        /// </summary>
        public String[] header;
        public String[] comments;
        public String[] types;

        /// <summary>
        /// 主键索引
        /// </summary>
        public int primaryKeyIndex = 0;

        public String primaryKey
        {
            get
            {
                return dataTable.Columns[primaryKeyIndex].ColumnName;
            }
            set
            {
                DataColumnCollection dcc = dataTable.Columns;
                int i = 0;
                int len = dcc.Count;
                while (i < len)
                {

                    if (dcc[i].ColumnName == value)
                    {
                        primaryKeyIndex = i;
                        return;
                    }
                    i++;
                }

                throw new Exception("尝试将主键值设置为不存在的列名:" + value);
            }
        }


        /// <summary>
        /// 数据表对象
        /// </summary>
        public DataTable dataTable;
    }
}
