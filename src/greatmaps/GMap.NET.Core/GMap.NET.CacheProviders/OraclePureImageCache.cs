/**************************************************************************
 * 类名：OraclePureImageCache.cs
 * 描述：Oracle瓦片地图缓存类
 * 作者：Allen
 * 日期：Nov 8,2016
 * 
 * ************************************************************************/

using GMap.NET.MapProviders;
using System;
using System.Data.OracleClient;
using System.Diagnostics;

namespace GMap.NET.CacheProviders
{
    public class OraclePureImageCache : PureImageCache, IDisposable
    {
        string connectionString = string.Empty;
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                if (connectionString != value)
                {
                    connectionString = value;

                    if (Initialized)
                    {
                        Dispose();
                        Initialize();
                    }
                }
            }
        }

        OracleCommand cmdInsert;
        OracleCommand cmdFetch;
        OracleConnection cnGet;
        OracleConnection cnSet;

        bool initialized = false;

        /// <summary>
        /// is cache initialized
        /// </summary>
        public bool Initialized
        {
            get
            {
                lock (this)
                {
                    return initialized;
                }
            }
            private set
            {
                lock (this)
                {
                    initialized = value;
                }
            }
        }

        /// <summary>
        /// inits connection to server
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            lock (this)
            {
                if (!initialized)
                {
                    #region prepare oracle & cache table
                    try
                    {
                        this.cnGet = new OracleConnection(connectionString);
                        this.cnGet.Open();
                        this.cnSet = new OracleConnection(connectionString);
                        this.cnSet.Open();

                        {
                            using (OracleCommand cmd = new OracleCommand(
                                @" CREATE TABLE IF NOT EXISTS `gmapnetcache` (
                             `Type` int(10) NOT NULL,
                             `Zoom` int(10) NOT NULL,
                             `X` int(10) NOT NULL,
                             `Y` int(10) NOT NULL,
                             `Tile` longblob NOT NULL,
                             PRIMARY KEY (`Type`,`Zoom`,`X`,`Y`)
                           ) ENGINE=InnoDB DEFAULT CHARSET=utf8;", cnGet))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }

                        this.cmdFetch = new OracleCommand("SELECT Tile FROM `gmapnetcache` WHERE Type=@type AND Zoom=@zoom AND X=@x AND Y=@y", cnGet);
                        this.cmdFetch.Parameters.Add("@type", OracleType.Int32);
                        this.cmdFetch.Parameters.Add("@zoom", OracleType.Int32);
                        this.cmdFetch.Parameters.Add("@x", OracleType.Int32);
                        this.cmdFetch.Parameters.Add("@y", OracleType.Int32);
                        this.cmdFetch.Prepare();

                        this.cmdInsert = new OracleCommand("INSERT INTO `gmapnetcache` ( Type, Zoom, X, Y, Tile ) VALUES ( @type, @zoom, @x, @y, @tile )", cnSet);
                        this.cmdInsert.Parameters.Add("@type", OracleType.Int32);
                        this.cmdInsert.Parameters.Add("@zoom", OracleType.Int32);
                        this.cmdInsert.Parameters.Add("@x", OracleType.Int32);
                        this.cmdInsert.Parameters.Add("@y", OracleType.Int32);

                        Initialized = true;
                    }
                    catch (Exception ex)
                    {
                        this.initialized = false;
                        Debug.WriteLine(ex.Message);
                    }
                    #endregion
                }
                return initialized;
            }
        }

        #region PureImageCache Members

        public bool PutImageToCache(byte[] tile, int type, GPoint pos, int zoom)
        {
            bool ret = true;
            {
                if (Initialize())
                {
                    try
                    {
                        lock (cmdInsert)
                        {
                            if (cnSet.State != System.Data.ConnectionState.Open)
                            {
                                cnSet.Open();
                            }

                            cmdInsert.Parameters["@type"].Value = type;
                            cmdInsert.Parameters["@zoom"].Value = zoom;
                            cmdInsert.Parameters["@x"].Value = pos.X;
                            cmdInsert.Parameters["@y"].Value = pos.Y;
                            cmdInsert.Parameters["@tile"].Value = tile;
                            cmdInsert.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                        ret = false;
                        Dispose();
                    }
                }
            }
            return ret;
        }

        public PureImage GetImageFromCache(int type, GPoint pos, int zoom)
        {
            PureImage ret = null;
            {
                if (Initialize())
                {
                    try
                    {
                        object odata = null;
                        lock (cmdFetch)
                        {
                            if (cnGet.State != System.Data.ConnectionState.Open)
                            {
                                cnGet.Open();
                            }

                            cmdFetch.Parameters["@type"].Value = type;
                            cmdFetch.Parameters["@zoom"].Value = zoom;
                            cmdFetch.Parameters["@x"].Value = pos.X;
                            cmdFetch.Parameters["@y"].Value = pos.Y;
                            odata = cmdFetch.ExecuteScalar();
                        }

                        if (odata != null && odata != DBNull.Value)
                        {
                            byte[] tile = (byte[])odata;
                            if (tile != null && tile.Length > 0)
                            {
                                if (GMapProvider.TileImageProxy != null)
                                {
                                    ret = GMapProvider.TileImageProxy.FromArray(tile);
                                }
                            }
                            tile = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                        ret = null;
                        Dispose();
                    }
                }
            }
            return ret;
        }

        public int DeleteOlderThan(DateTime date, int? type)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            lock (cmdInsert)
            {
                if (cmdInsert != null)
                {
                    cmdInsert.Dispose();
                    cmdInsert = null;
                }

                if (cnSet != null)
                {
                    cnSet.Dispose();
                    cnSet = null;
                }
            }

            lock (cmdFetch)
            {
                if (cmdFetch != null)
                {
                    cmdFetch.Dispose();
                    cmdFetch = null;
                }

                if (cnGet != null)
                {
                    cnGet.Dispose();
                    cnGet = null;
                }
            }
            Initialized = false;
        }
        #endregion

    }
}
