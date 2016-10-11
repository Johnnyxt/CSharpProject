using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows;

namespace JW18001
{
    internal class SqlHelper
    {
        private static string connString;
        public static string TableName = "dbo.TTestValue";

        public SqlHelper(string serverName, string dbName, string userId, string password)
        {
            connString = string.Format("SERVER={0};database={1};user={2};pwd={3}", serverName, dbName, userId, password);
            var conn = new SqlConnection(connString);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                IsConnect = false;
                return;
            }
            IsConnect = true;
        }

        public bool IsConnect { get; set; }

        public void ExecuteSql(string sqlString)
        {
            var conn = new SqlConnection(connString);
            var cmd = new SqlCommand(sqlString, conn);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public static DataSet SelectToDataSet(string cmdText)
        {
            var conn = new SqlConnection(connString);
            var ds = new DataSet();
            try
            {
                var da = new SqlDataAdapter
                {
                    SelectCommand = new SqlCommand
                    {
                        Connection = conn,
                        CommandText = cmdText
                    }
                };
                da.Fill(ds);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }

        public static void WriteToServer(DataTable table, string tableName)
        {
            //string connString = "Data Source=LocalHost;Integrated Security=SSPI;Initial Catalog=Product;";
            //SqlConnection conn = new SqlConnection(SqlHelper.connString);
            //conn.Open();

            SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connString) { DestinationTableName = tableName };

            for (int i = 0; i < table.Columns.Count; i++)
            {
                sqlbulkcopy.ColumnMappings.Add(table.Columns[i].ColumnName, table.Columns[i].ColumnName);
            }
            sqlbulkcopy.WriteToServer(table);
            sqlbulkcopy.Close();
        }

        public static string InsertData(string strTableName, IList<object> lstValue)
        {
            var lstSqlString = new List<object> { "INSERT INTO ", strTableName, "VALUES", "(" };

            for (var j = 0; j < lstValue.Count; j++)
            {
                if (lstValue[j] is string)
                {
                    lstSqlString.Add("'" + lstValue[j] + "'");
                }
                else
                {
                    lstSqlString.Add(lstValue[j]);
                }
                if (j != lstValue.Count - 1)
                {
                    lstSqlString.Add(",");
                }
            }
            lstSqlString.Add(")");
            var sqlString = string.Join(" ", lstSqlString.ToArray());

            return sqlString;
        }
    }
}