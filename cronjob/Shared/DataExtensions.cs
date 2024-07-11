using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace System
{
    public static class DataExtensions
    {
        //public static DataTable FirstTable(this DataSet dataSet)
        //{
        //    if (dataSet == null) return null;
        //    if (dataSet.Tables.Count == 0) return null;

        //    return dataSet.Tables[0];
        //}


        //public static DataTable SecondTable(this DataSet dataSet)
        //{
        //    if (dataSet == null) return null;
        //    if (dataSet.Tables.Count < 2) return null;

        //    return dataSet.Tables[1];
        //}


        public static string[] ToArray(this DataTable table, string columnName)
        {
            if (table == null) return null;

            DataRow[] rows = table.Distinct(columnName).Select();

            var items = from row in rows
                        select row[columnName].ToString();

            return items.ToArray();
        }


        public static DataTable OrderBy(this DataTable table, string column)
        {
            if (table == null) return null;
            if (column.Length == 0) return null;

            var view = new DataView(table);
            view.Sort = column;

            return view.ToTable();
        }


        public static DataTable Filter(this DataTable table, string filter)
        {
            if (table == null) return null;

            var view = new DataView(table);
            view.RowFilter = filter;

            return view.ToTable();
        }


        public static DataTable Distinct(this DataTable table, params string[] columnNames)
        {
            if (table == null) return null;
            if (columnNames.Length == 0) return null;

            return new DataView(table).ToTable(true, columnNames);
        }


        //public static DataRow FirstRow(this DataTable dataTable)
        //{
        //    if (dataTable == null) return null;
        //    if (dataTable.Rows.Count == 0) return null;

        //    return dataTable.Rows[0];
        //}


        //public static DataRow FirstRow(this DataSet dataSet)
        //{
        //    return dataSet.FirstTable().FirstRow();
        //}


        //public static string FirstValue(this DataSet dataSet)
        //{
        //    return dataSet.FirstTable().FirstValue();
        //}


        //public static string FirstValue(this DataTable dataTable)
        //{
        //    return dataTable.FirstRow().FirstValue();
        //}


        //public static string FirstValue(this DataRow dataRow)
        //{
        //    if (dataRow == null) return "";
        //    if (dataRow.ItemArray.Length == 0) return "";

        //    return dataRow[0] + "";
        //}

        public static DataRow FirstRow(this DataTable dataTable)
        {
            if (dataTable == null) return null;
            if (dataTable.Rows.Count == 0) return null;

            return dataTable.Rows[0];
        }

        public static DataRow FirstRow(this DataSet dataSet)
        {
            return dataSet.FirstTable().FirstRow();
        }

        public static string FirstValue(this DataSet dataSet)
        {
            return dataSet.FirstTable().FirstValue();
        }

        public static string FirstValue(this DataTable dataTable)
        {
            return dataTable.FirstRow().FirstValue();
        }

        public static string FirstValue(this DataRow dataRow)
        {
            if (dataRow == null) return "";
            if (dataRow.ItemArray.Length == 0) return "";

            return dataRow[0] + "";
        }

        public static string Item(this DataRow row, int column)
        {
            if (row != null)
            {
                if (row.Table.Columns.Count > column)
                {
                    return row[column] + "";
                }
            }

            return string.Empty;
        }

        public static string Item(this DataRow row, string column)
        {
            if (row != null)
            {
                if (row.Table.Columns.Contains(column))
                {
                    if (row.RowState != DataRowState.Deleted)
                        return row[column] + "";
                }
            }

            return string.Empty;
        }

        public static DataTable FirstTable(this DataSet dataSet)
        {
            return dataSet.Tables(0);
        }

        public static DataTable Tables(this DataSet dataSet, int index)
        {
            if (dataSet == null) return null;
            if (dataSet.Tables.Count <= index) return null;

            return dataSet.Tables[index];
        }
    }
}