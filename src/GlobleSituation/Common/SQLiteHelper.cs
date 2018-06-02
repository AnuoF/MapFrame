
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;
using GlobleSituation.Model;
using System.IO;
using System.Data.Common;

namespace GlobleSituation.Common
{
    /// <summary>
    /// 数据库操作类（当数据库本地路径中含有中文字符时，会报"open data file fail"的异常）
    /// </summary>
    public class SQLiteHelper
    {
        private static string connStr = "Data Source=Data\\data.db";
        private static SQLiteConnection conn = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        private SQLiteHelper()
        {

        }

        ~SQLiteHelper()
        {
            conn.Clone();
            conn.Dispose();
        }

        /// <summary> 
        /// 检查数据库文件是否存在，若不存在则创建
        /// </summary>
        public static void CreateDB()
        {
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Data"))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Data");

                string dbPath = AppDomain.CurrentDomain.BaseDirectory + "Data\\data.db";

                if (File.Exists(dbPath))
                {
                    string connstr = "Data Source=" + dbPath;
                    conn = new SQLiteConnection(connstr);
                    conn.Open();
                }
                else
                {
                    conn = new SQLiteConnection(connStr);
                    conn.Open();

                    using (SQLiteCommand command = new SQLiteCommand(conn))
                    {
                        command.CommandText = @"CREATE TABLE RealData(
                                            id integer NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
                                            TargetNum DOUBLE,   
                                            InformationSource integer,
                                            Country integer,
                                            TargetProperty integer,
                                            TargetType integer,
                                            EquipModelNumber varchar(100),
                                            PositionDate DOUBLE,
                                            Longitude DOUBLE,
                                            Latitude DOUBLE,
                                            Altitude DOUBLE,
                                            ScanRange DOUBLE,
                                            ActionRange DOUBLE)";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(SQLiteHelper), ex);
            }
        }

        /// <summary>
        /// 实时数据入库
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int Data2Db(List<RealData> lst)
        {
            try
            {
                using (DbTransaction tr = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var data in lst)
                        {
                            if (data == null) continue;

                            StringBuilder strBuilder = new StringBuilder();

                            strBuilder.AppendFormat(@"insert into RealData (TargetNum,InformationSource,Country,TargetProperty,TargetType,EquipModelNumber,PositionDate,Longitude,Latitude,Altitude,ScanRange,ActionRange) 
                    values({0},{1},{2},{3},{4},'{5}',{6},{7},{8},{9},{10},{11});", data.TargetNum, data.InformationSource, data.Country, data.TargetProperty, data.TargetType, data.EquipModelNumber.Trim('\0'), data.PositionDate, data.Longitude, data.Latitude, data.Altitude,
                                       data.ScanRange, data.ActionRange);

                            using (SQLiteCommand cmd = new SQLiteCommand(conn))
                            {
                                cmd.CommandText = strBuilder.ToString();
                                var ret = cmd.ExecuteNonQuery();
                            }
                        }
                        tr.Commit();
                    }
                    catch (Exception ex)
                    {
                        tr.Rollback();
                        return -1;
                    }
                }
                return lst.Count;
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(SQLiteHelper), ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// 查询 Shortcut method to execute dataset from SQL Statement and object[] arrray of  parameter values
        /// </summary>
        /// <param name="commandText">Command text.</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string commandText)
        {
            try
            {
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = commandText;

                DataSet ds = new DataSet();

                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                da.Fill(ds);
                da.Dispose();
                cmd.Dispose();

                return ds;
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(SQLiteHelper), ex);
                return null;
            }
        }

        /// <summary>
        /// 实时数据入库
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int Data2Db(RealData data)
        {
            if (data == null) return -1;

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.CommandText = string.Format(@"insert into RealData (TargetNum,InformationSource,Country,TargetProperty,TargetType,EquipModelNumber,PositionDate,Longitude,Latitude,Altitude,ScanRange,ActionRange) 
                    values({0},{1},{2},{3},{4},'{5}',{6},{7},{8},{9},{10},{11})", data.TargetNum, data.InformationSource, data.Country, data.TargetProperty, data.TargetType, data.EquipModelNumber.Trim('\0'), data.PositionDate, data.Longitude, data.Latitude, data.Altitude,
                    data.ScanRange, data.ActionRange);
                var ret = cmd.ExecuteNonQuery();
                return ret;
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(SQLiteHelper), ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// 执行Sql语句
        /// </summary>
        /// <param name="sql">sql</param>
        public static void ExecuteNonQuery(string sql)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(SQLiteHelper), ex);
            }
        }


        #region MyRegion

        //        /// <summary>
        //        /// 数据库路径
        //        /// </summary>
        //        private static string connStr = "Data Source=Data\\data.db";    // " + AppDomain.CurrentDomain.BaseDirectory + "

        //        /// <summary>
        //        /// 构造函数
        //        /// </summary>
        //        private SQLiteHelper()
        //        {
        //        }

        //        /// <summary> 
        //        /// 检查数据库文件是否存在，若不存在则创建
        //        /// </summary>
        //        public static void CreateDB()
        //        {
        //            string dbPath = AppDomain.CurrentDomain.BaseDirectory + "Data\\data.db";
        //            if (File.Exists(dbPath)) return;

        //            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Data"))
        //                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Data");

        //            try
        //            {
        //                using (SQLiteConnection conn = new SQLiteConnection(connStr))
        //                {
        //                    conn.Open();

        //                    using (SQLiteCommand command = new SQLiteCommand(conn))
        //                    {
        //                        command.CommandText = @"CREATE TABLE RealData(
        //                                            id integer NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
        //                                            TargetNum DOUBLE,   
        //                                            InformationSource integer,
        //                                            Country integer,
        //                                            TargetProperty integer,
        //                                            TargetType integer,
        //                                            EquipModelNumber varchar(100),
        //                                            PositionDate DOUBLE,
        //                                            Longitude DOUBLE,
        //                                            Latitude DOUBLE,
        //                                            Altitude DOUBLE,
        //                                            ScanRange DOUBLE,
        //                                            ActionRange DOUBLE)";
        //                        command.ExecuteNonQuery();
        //                    }
        //                    conn.Close();
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Log4Allen.WriteLog(typeof(SQLiteHelper), ex);
        //            }
        //        }

        //        /// <summary>
        //        /// 实时数据入库
        //        /// </summary>
        //        /// <param name="data"></param>
        //        /// <returns></returns>
        //        public static int Data2Db(RealData data)
        //        {
        //            if (data == null) return -1;

        //            using (SQLiteConnection conn = new SQLiteConnection(connStr))
        //            {
        //                conn.Open();
        //                SQLiteCommand cmd = new SQLiteCommand(conn);
        //                cmd.CommandText = string.Format(@"insert into RealData (TargetNum,InformationSource,Country,TargetProperty,TargetType,EquipModelNumber,PositionDate,Longitude,Latitude,Altitude,ScanRange,ActionRange) 
        //                    values({0},{1},{2},{3},{4},'{5}',{6},{7},{8},{9},{10},{11})", data.TargetNum, data.InformationSource, data.Country, data.TargetProperty, data.TargetType, data.EquipModelNumber.Trim('\0'), data.PositionDate, data.Longitude, data.Latitude, data.Altitude,
        //                    data.ScanRange, data.ActionRange);
        //                var ret = cmd.ExecuteNonQuery();
        //                conn.Close();
        //                return ret;
        //            }
        //        }


        //        /// <summary>
        //        /// 实时数据入库
        //        /// </summary>
        //        /// <param name="data"></param>
        //        /// <returns></returns>
        //        public static int Data2Db(List<RealData> lst)
        //        {
        //            try
        //            {
        //                using (SQLiteConnection cn = new SQLiteConnection(connStr))
        //                {
        //                    cn.Open();
        //                    {
        //                        using (DbTransaction tr = cn.BeginTransaction())
        //                        {
        //                            try
        //                            {
        //                                foreach (var data in lst)
        //                                {
        //                                    if (data == null) continue;

        //                                    StringBuilder strBuilder = new StringBuilder();

        //                                    strBuilder.AppendFormat(@"insert into RealData (TargetNum,InformationSource,Country,TargetProperty,TargetType,EquipModelNumber,PositionDate,Longitude,Latitude,Altitude,ScanRange,ActionRange) 
        //                    values({0},{1},{2},{3},{4},'{5}',{6},{7},{8},{9},{10},{11});", data.TargetNum, data.InformationSource, data.Country, data.TargetProperty, data.TargetType, data.EquipModelNumber.Trim('\0'), data.PositionDate, data.Longitude, data.Latitude, data.Altitude,
        //                                               data.ScanRange, data.ActionRange);

        //                                    using (SQLiteCommand cmd = new SQLiteCommand(cn))
        //                                    {
        //                                        cmd.CommandText = strBuilder.ToString();
        //                                        var ret = cmd.ExecuteNonQuery();
        //                                    }
        //                                }
        //                                tr.Commit();
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                tr.Rollback();
        //                                return -1;
        //                            }
        //                        }
        //                    }
        //                    cn.Close();
        //                    return lst.Count;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Log4Allen.WriteLog(typeof(SQLiteHelper), ex.Message);
        //                return -1;
        //            }
        //        }

        //        /// <summary>
        //        /// 查询 Shortcut method to execute dataset from SQL Statement and object[] arrray of  parameter values
        //        /// </summary>
        //        /// <param name="commandText">Command text.</param>
        //        /// <returns></returns>
        //        public static DataSet ExecuteDataSet(string commandText)
        //        {
        //            try
        //            {
        //                using (SQLiteConnection conn = new SQLiteConnection(connStr))
        //                {
        //                    conn.Open();

        //                    SQLiteCommand cmd = conn.CreateCommand();
        //                    cmd.CommandText = commandText;

        //                    DataSet ds = new DataSet();

        //                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
        //                    da.Fill(ds);
        //                    da.Dispose();
        //                    cmd.Dispose();
        //                    conn.Close();

        //                    return ds;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Log4Allen.WriteLog(typeof(SQLiteHelper), ex);
        //                return null;
        //            }
        //        }

        //        /// <summary>
        //        /// 执行Sql语句
        //        /// </summary>
        //        /// <param name="sql">sql</param>
        //        public static void ExecuteNonQuery(string sql)
        //        {
        //            try
        //            {
        //                using (SQLiteConnection conn = new SQLiteConnection(connStr))
        //                {
        //                    conn.Open();

        //                    SQLiteCommand cmd = new SQLiteCommand(conn);
        //                    cmd.CommandText = sql;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    conn.Close();
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Log4Allen.WriteLog(typeof(SQLiteHelper), ex);
        //            }
        //        }

        #endregion

        #region abandon

        ///// <summary>
        ///// 从DDS文件获取机器号
        ///// </summary>
        ///// <param name="ddsFileName">dds文件</param>
        //public static int GetMachineId(string ddsFileName)
        //{
        //    try
        //    {
        //        //StreamReader sr = new StreamReader(ddsFileName);
        //        //string line = sr.ReadLine();
        //        //DDSFrame frame = new DDSFrame(line);
        //        //return frame.machine_id;
        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4Allen.WriteLog(typeof(SQLiteHelper), ex);
        //        return 0;
        //    }
        //}

        ///// <summary>
        ///// 获取操作ID
        ///// </summary>
        ///// <returns></returns>
        //public static int GetNextOperationID()
        //{
        //    try
        //    {
        //        using (SQLiteConnection conn = new SQLiteConnection(connStr))
        //        {
        //            conn.Open();
        //            SQLiteCommand cmd = new SQLiteCommand(conn);
        //            cmd.CommandText = "select * from QACDR_VIDEO order by operation_id desc limit 1";
        //            SQLiteDataReader reader = cmd.ExecuteReader();
        //            int operation_id_video = 1;
        //            while (reader.Read())
        //                operation_id_video = Convert.ToInt32(reader["operation_id"]) + 1;
        //            reader.Close();

        //            cmd.CommandText = "select * from QACDR_AUDIO order by operation_id desc limit 1";
        //            reader = cmd.ExecuteReader();
        //            int operation_id_audio = 1;
        //            while (reader.Read())
        //                operation_id_audio = Convert.ToInt32(reader["operation_id"]) + 1;
        //            reader.Close();

        //            return operation_id_video > operation_id_audio ? operation_id_video : operation_id_audio;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4Allen.WriteLog(typeof(SQLiteHelper), ex);
        //        return 0;
        //    }
        //}
        #endregion

    }
}
