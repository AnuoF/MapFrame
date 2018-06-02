

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MgisTilesImportTool
{
    public class SQLiteHelper
    {
        private string ConnectionString;
        private static readonly string singleSqlInsert = "INSERT INTO main.Tiles(X, Y, Zoom, Type, CacheTime) VALUES(@p1, @p2, @p3, @p4, @p5)";
        private static readonly string singleSqlInsertLast = "INSERT INTO main.TilesData(id, Tile) VALUES((SELECT last_insert_rowid()), @p1)";
        private int preAllocationPing = 0;
        private string db;


        public SQLiteHelper()
        {

        }

        public void CreateEmptyDb(string dbPath)
        {
            string file = Path.Combine(dbPath, "Data.gmdb");
            db = file;

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            try
            {
                string dir = Path.GetDirectoryName(file);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (SQLiteConnection cn = new SQLiteConnection())
                {
                    ConnectionString = string.Format("Data Source=\"{0}\";FailIfMissing=False;Page Size=32768", file);
                    cn.ConnectionString = ConnectionString;
                    cn.Open();
                    {
                        using (DbTransaction tr = cn.BeginTransaction())
                        {
                            try
                            {
                                using (DbCommand cmd = cn.CreateCommand())
                                {
                                    cmd.Transaction = tr;
                                    cmd.CommandText = Properties.Resources.CreateTileDb;
                                    cmd.ExecuteNonQuery();
                                }
                                tr.Commit();
                            }
                            catch (Exception exx)
                            {
                                Debug.WriteLine("CreateEmptyDB: " + exx.ToString());
                                tr.Rollback();
                            }
                        }
                        cn.Close();
                    }
                }

                CheckPreAllocation();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CreateEmptyDB: " + ex.ToString());
                throw;
            }
        }

        public bool PutTileToCache(byte[] tile, int type, int x, int y, int zoom)
        {
            bool ret = true;

            try
            {
                using (SQLiteConnection cn = new SQLiteConnection())
                {
                    cn.ConnectionString = ConnectionString;
                    cn.Open();
                    {
                        using (DbTransaction tr = cn.BeginTransaction())
                        {
                            try
                            {
                                using (DbCommand cmd = cn.CreateCommand())
                                {
                                    cmd.Transaction = tr;
                                    cmd.CommandText = singleSqlInsert;

                                    cmd.Parameters.Add(new SQLiteParameter("@p1", x));
                                    cmd.Parameters.Add(new SQLiteParameter("@p2", y));
                                    cmd.Parameters.Add(new SQLiteParameter("@p3", zoom));
                                    cmd.Parameters.Add(new SQLiteParameter("@p4", type));
                                    cmd.Parameters.Add(new SQLiteParameter("@p5", DateTime.Now));

                                    cmd.ExecuteNonQuery();
                                }

                                using (DbCommand cmd = cn.CreateCommand())
                                {
                                    cmd.Transaction = tr;

                                    cmd.CommandText = singleSqlInsertLast;
                                    cmd.Parameters.Add(new SQLiteParameter("@p1", tile));

                                    cmd.ExecuteNonQuery();
                                }
                                tr.Commit();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("PutImageToCache: " + ex.ToString());

                                tr.Rollback();
                                ret = false;
                            }
                        }
                    }
                    cn.Close();
                }

                if (Interlocked.Increment(ref preAllocationPing) % 22 == 0)
                {
                    CheckPreAllocation();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PutImageToCache: " + ex.ToString());
                ret = false;
            }

            return ret;
        }

        public bool PutTileToCachePL(List<Tile> tileList)
        {
            bool ret = true;

            try
            {
                using (SQLiteConnection cn = new SQLiteConnection())
                {
                    cn.ConnectionString = ConnectionString;
                    cn.Open();
                    {
                        using (DbTransaction tr = cn.BeginTransaction())
                        {
                            try
                            {
                                foreach (Tile t in tileList)
                                {
                                    using (DbCommand cmd = cn.CreateCommand())
                                    {
                                        cmd.Transaction = tr;
                                        cmd.CommandText = singleSqlInsert;

                                        cmd.Parameters.Add(new SQLiteParameter("@p1", t.x));
                                        cmd.Parameters.Add(new SQLiteParameter("@p2", t.y));
                                        cmd.Parameters.Add(new SQLiteParameter("@p3", t.zoom));
                                        cmd.Parameters.Add(new SQLiteParameter("@p4", t.type));
                                        cmd.Parameters.Add(new SQLiteParameter("@p5", DateTime.Now));

                                        cmd.ExecuteNonQuery();
                                    }

                                    using (DbCommand cmd = cn.CreateCommand())
                                    {
                                        cmd.Transaction = tr;

                                        cmd.CommandText = singleSqlInsertLast;
                                        cmd.Parameters.Add(new SQLiteParameter("@p1", t.tile));

                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                tr.Commit();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("PutImageToCache: " + ex.ToString());

                                tr.Rollback();
                                ret = false;
                            }
                        }
                    }
                    cn.Close();
                }

                if (Interlocked.Increment(ref preAllocationPing) % 22 == 0)
                {
                    CheckPreAllocation();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PutImageToCache: " + ex.ToString());
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// pre-allocate 32MB free space 'ahead' if needed,
        /// decreases fragmentation
        /// </summary>
        void CheckPreAllocation()
        {
            {
                byte[] pageSizeBytes = new byte[2];
                byte[] freePagesBytes = new byte[4];

                lock (this)
                {
                    using (var dbf = File.Open(db, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        dbf.Seek(16, SeekOrigin.Begin);

                        dbf.Lock(16, 2);
                        dbf.Read(pageSizeBytes, 0, 2);
                        dbf.Unlock(16, 2);

                        dbf.Seek(36, SeekOrigin.Begin);

                        dbf.Lock(36, 4);
                        dbf.Read(freePagesBytes, 0, 4);
                        dbf.Unlock(36, 4);

                        dbf.Close();
                    }
                }

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(pageSizeBytes);
                    Array.Reverse(freePagesBytes);
                }
                UInt16 pageSize = BitConverter.ToUInt16(pageSizeBytes, 0);
                UInt32 freePages = BitConverter.ToUInt32(freePagesBytes, 0);

                var freeMB = (pageSize * freePages) / (1024.0 * 1024.0);

                int addSizeMB = 32;
                int waitUntilMB = 4;

                Debug.WriteLine("FreePageSpace in cache: " + freeMB + "MB | " + freePages + " pages");

                if (freeMB <= waitUntilMB)
                {
                    PreAllocateDB(db, addSizeMB);
                }
            }
        }

        public static bool PreAllocateDB(string file, int addSizeInMBytes)
        {
            bool ret = true;

            try
            {
                Debug.WriteLine("PreAllocateDB: " + file + ", +" + addSizeInMBytes + "MB");

                using (SQLiteConnection cn = new SQLiteConnection())
                {
                    cn.ConnectionString = string.Format("Data Source=\"{0}\";FailIfMissing=False;Page Size=32768", file);

                    cn.Open();
                    {
                        using (DbTransaction tr = cn.BeginTransaction())
                        {
                            try
                            {
                                using (DbCommand cmd = cn.CreateCommand())
                                {
                                    cmd.Transaction = tr;
                                    cmd.CommandText = string.Format("create table large (a); insert into large values (zeroblob({0})); drop table large;", addSizeInMBytes * 1024 * 1024);
                                    cmd.ExecuteNonQuery();
                                }
                                tr.Commit();
                            }
                            catch (Exception exx)
                            {
                                Debug.WriteLine("PreAllocateDB: " + exx.ToString());

                                tr.Rollback();
                                ret = false;
                            }
                        }
                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PreAllocateDB: " + ex.ToString());
                ret = false;
            }
            return ret;
        }

    }
}
