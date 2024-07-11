using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace System
{
    public static class DataSetExtensions
    {
        public static void ApplyDataSet(this DataSet target, DataSet source)
        {
            int tableCount = Math.Min(target.Tables.Count, source.Tables.Count);
            for (int tableIndex = 0; tableIndex < tableCount; tableIndex++)
            {
                var table_dest = target.Tables[tableIndex];
                var table_src = source.Tables[tableIndex];

                var columns_dest = table_dest.Columns;
                var columns_src = table_src.Columns;

                foreach (DataRow row_src in table_src.Rows)
                {
                    var row_dest = table_dest.NewRow();

                    CopyRowData(row_dest, row_src, columns_dest, columns_src);

                    table_dest.Rows.Add(row_dest);  
                }
            }
        }


        static void CopyRowData(DataRow row_dest, DataRow row_src, DataColumnCollection columns_dest, DataColumnCollection columns_src)
        {
            foreach (DataColumn column_src in columns_src)
            {
                var columnName = column_src.ColumnName;

                if (columns_dest.Contains(columnName))
                {
                    var column_dest = columns_dest[columnName];

                    row_dest[column_dest] = row_src[column_src];
                }
            }
        }
    }
}
