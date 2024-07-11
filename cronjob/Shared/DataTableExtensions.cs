using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace System
{
    public static class DataTableExtensions
    {
        public static IEnumerable<T> Convert<T>(this DataTable table)
        {
            List<T> data = new List<T>();

            var type = typeof(T);
            var fields = type.GetFields();

            foreach (DataRow row in table.Rows)
            {
                var item = (T)type.GetConstructor(Type.EmptyTypes).Invoke(null);

                foreach (var field in fields)
                {
                    var value = row[field.Name];
                    if (value == DBNull.Value)
                        value = null;

                    if (field.FieldType == typeof(int))
                    {
                        field.SetValue(item, System.Convert.ToInt32(value));
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(item, System.Convert.ToBoolean(value));
                    }
                    else
                    {
                        field.SetValue(item, value);
                    }
                }

                data.Add(item);
            }

            return data;
        }
    }
}
