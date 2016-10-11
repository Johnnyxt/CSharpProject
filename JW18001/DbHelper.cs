using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;

namespace JW18001
{
    internal class DbHelper
    {
        public OleDbConnection Conn;
        public string ConnString; //连接字符串
        public static string DataSource = "data.mdb";
        public static string TableName = "TTestValue";

        /// <summary>
        ///
        /// </summary>
        /// <param name="strDbName">数据库名称</param>
        public DbHelper(string strDbName)
        {
            ConnString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strDbName;
            if (Conn == null)
            {
                Conn = new OleDbConnection(ConnString);
            }
            if (Conn.State != ConnectionState.Open)
            {
                Conn.Open();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="lstField"></param>
        /// <param name="lstFieldPara"></param>
        /// <returns></returns>
        public string InsertData(string strTableName, IList<string> lstField, IList<string> lstFieldPara)
        {
            var lstSqlString = new List<string> { "INSERT INTO ", strTableName, "(" };

            for (var j = 0; j < lstField.Count; j++)
            {
                lstSqlString.Add(lstField[j]);
                if (j != lstField.Count - 1)
                {
                    lstSqlString.Add(",");
                }
            }
            lstSqlString.Add(")");
            lstSqlString.Add("VALUES");
            lstSqlString.Add("(");
            for (var j = 0; j < lstFieldPara.Count; j++)
            {
                lstSqlString.Add(lstFieldPara[j]);
                if (j != lstFieldPara.Count - 1)
                {
                    lstSqlString.Add(",");
                }
            }
            lstSqlString.Add(")");
            var sqlString = string.Join(" ", lstSqlString.ToArray());

            return sqlString;
        }

        /// <summary>
        /// </summary>
        /// <param name="strTableName">表名称</param>
        /// <param name="lstValue">数据集</param>
        /// <returns></returns>
        public string InsertData(string strTableName, IList<object> lstValue)
        {
            var lstSqlString = new List<object> { "INSERT INTO ", strTableName, "VALUES", "(" };

            for (var j = 0; j < lstValue.Count; j++)
            {
                lstSqlString.Add(lstValue[j]);
                if (j != lstValue.Count - 1)
                {
                    lstSqlString.Add(",");
                }
            }
            lstSqlString.Add(")");
            var sqlString = string.Join(" ", lstSqlString.ToArray());

            return sqlString;
        }

        /// <summary>
        /// </summary>
        /// <param name="sqlString"></param>
        public void ExecuteSql(string sqlString)
        {
            int ret = 0;
            var conn = new OleDbConnection(ConnString);
            var cmd = new OleDbCommand(sqlString, conn);
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

        /// <summary>
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="parameters"></param>
        public void ExecuteSql(string sqlString, params OleDbParameter[] parameters)
        {
            var conn = new OleDbConnection(ConnString);
            var cmd = new OleDbCommand(sqlString, conn) { CommandType = CommandType.Text };
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    if ((p.Direction == ParameterDirection.Output) && p.Value == null) p.Value = DBNull.Value;
                    cmd.Parameters.Add(p);
                }
            }
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

        /// <summary>
        ///     返回数据库中所有表名称
        /// </summary>
        /// <returns></returns>
        public string[] GetShemaTableName()
        {
            try
            {
                var conn = new OleDbConnection(ConnString);
                //获取数据表

                conn.Open();
                var shemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                    new object[] { null, null, null, "TABLE" });
                var n = shemaTable.Rows.Count;
                var strTable = new string[n];
                var m = shemaTable.Columns.IndexOf("TABLE_NAME");
                for (var i = 0; i < n; i++)
                {
                    var dr = shemaTable.Rows[i];
                    strTable[i] = dr.ItemArray.GetValue(m).ToString();
                }
                return strTable;
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("指定的限制集无效:\n" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public DataSet SelectToDataSet(string cmdText)
        {
            var conn = new OleDbConnection(ConnString);
            var ds = new DataSet();
            try
            {
                var da = new OleDbDataAdapter();
                da.SelectCommand = new OleDbCommand();
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandText = cmdText;
                da.Fill(ds);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }

        /// <summary>
        /// 返回数据集行数
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public int GetDsRows(string cmdText)
        {
            var conn = new OleDbConnection(ConnString);
            var ds = new DataSet();
            try
            {
                var da = new OleDbDataAdapter();
                da.SelectCommand = new OleDbCommand();
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandText = cmdText;
                da.Fill(ds);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                conn.Close();
            }
            return ds.Tables[0].Rows.Count;
        }

        /// <summary>
        ///     根据SQL命令返回数据DataSet数据集
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="subtableName">在返回的数据集中所添加的表的名称</param>
        /// <returns></returns>
        public DataSet SelectToDataSet(string cmdText, string subtableName)
        {
            var adapter = new OleDbDataAdapter();
            var command = new OleDbCommand(cmdText, Conn);
            adapter.SelectCommand = command;
            var ds = new DataSet();
            ds.Tables.Add(subtableName);
            adapter.Fill(ds, subtableName);
            return ds;
        }

        //public static void WriteToServer(DataTable dataTable, string dataSource, string tableName)
        //{
        //    string connString =
        //        string.Format(
        //            "Provider = Microsoft.Jet.OLEDB.4.0; Data source = {0}; Persist Security Info = False; Jet OLEDB:Database Password = ",
        //            dataSource);
        //    OleDbConnection con = new OleDbConnection(connString);
        //    //  OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=Data.MDB;Persist Security Info=False;Jet OLEDB:Database Password=");
        //    try
        //    {
        //        con.Open();
        //        OleDbDataAdapter Bada = new OleDbDataAdapter("SELECT *  FROM TTestValue where 1 =2", con);//建立一个DataAdapter对象
        //        OleDbCommandBuilder cb = new OleDbCommandBuilder(Bada);//这里的CommandBuilder对象一定不要忘了,一般就是写在DataAdapter定义的后面
        //        cb.QuotePrefix = "[";
        //        cb.QuoteSuffix = "]";
        //        DataSet ds = new DataSet();//建立DataSet对象
        //        Bada.Fill(ds, "TTestValue");//填充DataSet
        //        foreach (DataRow tempRow in dataTable.Rows)
        //        {
        //            DataRow dr = ds.Tables["TTestValue"].NewRow();
        //            dr.ItemArray = tempRow.ItemArray;//行复制
        //            ds.Tables["TTestValue"].Rows.Add(dr);
        //        }
        //        Bada.Update(ds, "TTestValue");//用DataAdapter的Update()方法进行数据库的更新
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }
        //}

        public static void WriteToServer(DataTable dataTable, string dataSource, string tableName)
        {
            string connString =
                string.Format(
                    "Provider = Microsoft.Jet.OLEDB.4.0; Data source = {0}; Persist Security Info = False; Jet OLEDB:Database Password = ",
                    dataSource);
            OleDbConnection con = new OleDbConnection(connString);
            try
            {
                con.Open();
                string selectCmdText = string.Format("SELECT *FROM {0} WHERE 1=2", tableName);
                OleDbDataAdapter bada = new OleDbDataAdapter(selectCmdText, con);//建立一个DataAdapter对象
                OleDbCommandBuilder cb = new OleDbCommandBuilder(bada)
                {
                    QuotePrefix = "[",
                    QuoteSuffix = "]"
                }; //这里的CommandBuilder对象一定不要忘了,一般就是写在DataAdapter定义的后面
                DataSet ds = new DataSet();//建立DataSet对象
                bada.Fill(ds, tableName);//填充DataSet
                foreach (DataRow tempRow in dataTable.Rows)
                {
                    DataRow dr = ds.Tables[tableName].NewRow();
                    dr.ItemArray = tempRow.ItemArray;//行复制
                    ds.Tables[tableName].Rows.Add(dr);
                }
                bada.Update(ds, tableName);//用DataAdapter的Update()方法进行数据库的更新
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                con.Close();
            }
        }
    }
}