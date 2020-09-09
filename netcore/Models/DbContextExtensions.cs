using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace netcore_ef.Models
{
    public static class DbContextExtensions 
    {
        private static void CombineParams(ref DbCommand command, DatabaseFacade facade, params object[] parameters)
        {
            if (parameters != null)
            {
                //if (facade.IsOracle())
                //{
                //    foreach (OracleParameter parameter in parameters)
                //    {
                //        if (!parameter.ParameterName.Contains(":"))
                //            parameter.ParameterName = $":{parameter.ParameterName}";
                //        command.Parameters.Add(parameter);
                //    }
                //}
                if (facade.IsSqlServer())
                {
                    foreach (SqlParameter parameter in parameters)
                    {
                        if (!parameter.ParameterName.Contains("@"))
                            parameter.ParameterName = $"@{parameter.ParameterName}";
                        command.Parameters.Add(parameter);
                    }
                }
                if (facade.IsMySql())
                {
                    foreach (MySqlParameter parameter in parameters)
                    {
                        if (!parameter.ParameterName.Contains("?"))
                            parameter.ParameterName = $"?{parameter.ParameterName}";
                        command.Parameters.Add(parameter);
                    }
                }
            }
        }

        private static DbCommand CreateCommand(DatabaseFacade facade, string sql, out DbConnection dbConn, params object[] parameters)
        {
            var conn = facade.GetDbConnection();
            dbConn = conn;
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            CombineParams(ref cmd, facade, parameters);
            //if (facade.IsOracle())
            //{
            //    cmd.CommandText = sql;
            //    CombineParams(ref cmd, facade, parameters);
            //}
            return cmd;
        }
        private static DbCommand CreateProcCommand(DatabaseFacade facade, string procName, out DbConnection dbConn, params object[] parameters)
        {
            var conn = facade.GetDbConnection();
            dbConn = conn;
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procName;
            CombineParams(ref cmd, facade, parameters);
            return cmd;
        }

        public static DataSet RunProcCur(DatabaseFacade facade,string procName,params object[] parameters)
        {
            var cmd = CreateProcCommand(facade,procName,out DbConnection conn,parameters);
            SqlDataAdapter sda = new SqlDataAdapter((SqlCommand)cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            sda.Dispose();
            conn.Close();
            return ds;
        }

        public static IEnumerable<TElement> SqlQueryProc<TElement>(this DatabaseFacade facade, string procName, params object[] parameters) where TElement : new()
        {
            var cmd = CreateProcCommand(facade, procName, out DbConnection conn, parameters);
            var dr = cmd.ExecuteReader();
            var columnSchema = dr.GetColumnSchema();
            var data = new List<TElement>();
            while (dr.Read())
            {
                TElement item = new TElement();
                Type type = item.GetType();
                foreach (var kv in columnSchema)
                {
                    var propertyInfo = type.GetProperty(kv.ColumnName);
                    if (kv.ColumnOrdinal.HasValue && propertyInfo != null)
                    {
                        //注意需要转换数据库中的DBNull类型
                        var value = dr.IsDBNull(kv.ColumnOrdinal.Value) ? null : dr.GetValue(kv.ColumnOrdinal.Value);
                        propertyInfo.SetValue(item, value);
                    }
                }
                data.Add(item);
            }
            dr.Dispose();
            return data;
        }

        public static DataTable SqlQuery(this DatabaseFacade facade, string sql, params object[] parameters)
        {
            var cmd = CreateCommand(facade, sql, out DbConnection conn, parameters);
            var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            conn.Close();
            return dt;
        }

        public static IEnumerable<T> SqlQuery<T>(this DatabaseFacade facade, string sql, params object[] parameters) where T : class, new()
        {
            var dt = SqlQuery(facade, sql, parameters);
            return dt.ToEnumerable<T>();
        }

        public static IEnumerable<T> ToEnumerable<T>(this DataTable dt) where T : class, new()
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            T[] ts = new T[dt.Rows.Count];
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                T t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        p.SetValue(t, row[p.Name], null);
                }
                ts[i] = t;
                i++;
            }
            return ts;
        }
    }
}
