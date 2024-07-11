/****************************************************************
'* Change history : #001 2017/11/24 HiepNH4
'* : add param 
'*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using WebService;

namespace QA.API_WebService
{
    public class Database
    {
        public static DataSet ProcessRequest(RequestCollection requests, string ConnectionStringName = "DB")
        {
            string connectionString = WebConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            var response = new DataSet();

            SqlTransaction transaction = null;
            string error = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    try
                    {
                        foreach (var request in requests)
                        {
                            var category = request["Attributes"]["Category"].Value;
                            var command = request["Attributes"]["Command"].Value;
                            var parameters = request["Parameters"];

                            /*1.0.1.1*/
                            string param = "";
                            foreach (var parameter in parameters)
                            {
                                var name = parameter.Name;
                                var value = parameter.Value;

                                name = Misc.SafeSqlName(name);

                                param += "@" + name + "='" + value + "',";

                                if (parameter.IsNull)
                                {
                                    value = null;
                                }
                            }

                            error += command + " " + param;


                            /*1.0.1.1*/

                            if (ProcessNative(category, command, parameters, response) == false)
                            {
                                ProcessSql(connection, transaction, category, command, parameters, response);
                            }
                        }

                        transaction.Commit();
                    }
                    catch (SqlException eSql)
                    {
                        if (eSql.Number == -2)
                        {
                            var table = new DataTable("Error");
                            table.Columns.Add("Message");
                            table.Columns.Add("MessageDev"); //1.0.1.1
                            table.Columns.Add("Source");
                            table.Columns.Add("StackTrace");
                            table.Columns.Add("HelpLink");

                            var row = table.NewRow();
                            row["Message"] = "Đường truyền bị gián đoạn. Vui lòng khởi động lại phần mềm. Và chờ trong giây lát.";
                            row["MessageDev"] = error;// error; //1.0.1.1
                            row["Source"] = eSql.Source;
                            row["StackTrace"] = eSql.StackTrace;
                            row["HelpLink"] = eSql.HelpLink;

                            table.Rows.Add(row);

                            response = new DataSet();
                            response.Tables.Add(table);

                        }
                        else
                            throw eSql;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw e;
                    }

                }
            }
            catch (Exception e)
            {
                var table = new DataTable("Error");
                table.Columns.Add("Message");
                table.Columns.Add("MessageDev"); //1.0.1.1
                table.Columns.Add("Source");
                table.Columns.Add("StackTrace");
                table.Columns.Add("HelpLink");

                //var resultError = "";
                //if (error.Length > 0)
                //{
                //    int indexSubstring = error.LastIndexOf(",ws_");
                //    resultError = error.Substring(indexSubstring, error.Length - indexSubstring);
                //}
                var row = table.NewRow();
                row["Message"] = e.Message;
                row["MessageDev"] = error;// error; //1.0.1.1
                row["Source"] = e.Source;
                row["StackTrace"] = e.StackTrace;
                row["HelpLink"] = e.HelpLink;

                table.Rows.Add(row);

                response = new DataSet();
                response.Tables.Add(table);
            }


            return response;
        }


        static void ProcessSql(SqlConnection connection, SqlTransaction transaction, string category, string command, NameValueCollection parameters, DataSet response)
        {
            using (SqlCommand cmd = new SqlCommand(category + ".." + command, connection))
            {
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;// 5 minutes

                foreach (var parameter in parameters)
                {
                    var name = parameter.Name;
                    var value = parameter.Value;

                    name = Misc.SafeSqlName(name);

                    if (parameter.IsNull)
                    {
                        value = null;
                    }

                    cmd.Parameters.Add("@" + name, SqlDbType.NVarChar).Value = value;
                }

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd))
                {
                    var result = new DataSet();
                    dataAdapter.Fill(result);
                    AppendDataSet(response, result);
                }
            }
        }


        static bool ProcessNative(string category, string command, NameValueCollection parameters, DataSet response)
        {
            return false;
        }


        static void AppendDataSet(DataSet target, DataSet source)
        {
            var source_tables = source.Tables.Cast<DataTable>().ToArray();

            foreach (DataTable table in source_tables)
            {
                source.Tables.Remove(table);

                string tableName = "Table";
                if (target.Tables.Count > 0)
                    tableName += target.Tables.Count;

                table.TableName = tableName;
                target.Tables.Add(table);
            }
        }
    }
}