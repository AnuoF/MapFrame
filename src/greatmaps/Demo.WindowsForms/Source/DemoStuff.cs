﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using GMap.NET;
using System.Data.Common;
using GMap.NET.MapProviders;
using System.Text;
using System.Diagnostics;

#if !PocketPC
using System.Net.NetworkInformation;
#endif

#if !MONO
#if SQLite
using System.Data.SQLite;
#endif
#else
   using SQLiteConnection=Mono.Data.Sqlite.SqliteConnection;
   using SQLiteTransaction=Mono.Data.Sqlite.SqliteTransaction;
   using SQLiteCommand=Mono.Data.Sqlite.SqliteCommand;
   using SQLiteDataReader=Mono.Data.Sqlite.SqliteDataReader;
   using SQLiteParameter=Mono.Data.Sqlite.SqliteParameter;
#endif

namespace Demo.WindowsForms
{
   public struct VehicleData
   {
      public int Id;
      public double Lat;
      public double Lng;
      public string Line;
      public string LastStop;
      public string TrackType;
      public string AreaName;
      public string StreetName;
      public string Time;
      public double? Bearing;
   }

   public enum TransportType
   {
      Bus,
      TrolleyBus,
   }

   public struct FlightRadarData
   {
      public string name;
      public PointLatLng point;
      public int bearing;
      public string altitude;
      public string speed;
      public int Id;
   }

   public class Stuff
   {
#if !PocketPC
      public static bool PingNetwork(string hostNameOrAddress)
      {
         bool pingStatus = false;

         using(Ping p = new Ping())
         {
            byte[] buffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            int timeout = 4444; // 4s

            try
            {
               PingReply reply = p.Send(hostNameOrAddress, timeout, buffer);
               pingStatus = (reply.Status == IPStatus.Success);
            }
            catch(Exception)
            {
               pingStatus = false;
            }
         }

         return pingStatus;
      }
#endif

      /// <summary>
      /// gets routes from gpsd log file
      /// </summary>
      /// <param name="gpsdLogFile"></param>
      /// <param name="start">start time(UTC) of route, null to read from very start</param>
      /// <param name="end">end time(UTC) of route, null to read to the very end</param>
      /// <param name="maxPositionDilutionOfPrecision">max value of PositionDilutionOfPrecision, null to get all</param>
      /// <returns></returns>
      public static IEnumerable<List<GpsLog>> GetRoutesFromMobileLog(string gpsdLogFile, DateTime? start, DateTime? end, double? maxPositionDilutionOfPrecision)
      {
#if SQLite
         using(SQLiteConnection cn = new SQLiteConnection())
         {
#if !MONO
            cn.ConnectionString = string.Format("Data Source=\"{0}\";FailIfMissing=True;", gpsdLogFile);
#else
            cn.ConnectionString = string.Format("Version=3,URI=file://{0},FailIfMissing=True", gpsdLogFile);
#endif

            cn.Open();
            {
               using(DbCommand cmd = cn.CreateCommand())
               {
                  cmd.CommandText = "SELECT * FROM GPS ";

                  if(start.HasValue)
                  {
                     cmd.CommandText += "WHERE TimeUTC >= @t1 ";
                     SQLiteParameter lookupValue = new SQLiteParameter("@t1", start);
                     cmd.Parameters.Add(lookupValue);
                  }

                  if(end.HasValue)
                  {
                     if(!start.HasValue)
                     {
                        cmd.CommandText += "WHERE ";
                     }
                     else
                     {
                        cmd.CommandText += "AND ";
                     }

                     cmd.CommandText += "TimeUTC <= @t2 ";
                     SQLiteParameter lookupValue = new SQLiteParameter("@t2", end);
                     cmd.Parameters.Add(lookupValue);
                  }

                  if(maxPositionDilutionOfPrecision.HasValue)
                  {
                     if(!start.HasValue && !end.HasValue)
                     {
                        cmd.CommandText += "WHERE ";
                     }
                     else
                     {
                        cmd.CommandText += "AND ";
                     }

                     cmd.CommandText += "(PositionDilutionOfPrecision <= @p3)";
                     SQLiteParameter lookupValue = new SQLiteParameter("@p3", maxPositionDilutionOfPrecision);
                     cmd.Parameters.Add(lookupValue);
                  }

                  using(DbDataReader rd = cmd.ExecuteReader())
                  {
                     List<GpsLog> points = new List<GpsLog>();

                     long lastSessionCounter = -1;

                     while(rd.Read())
                     {
                        GpsLog log = new GpsLog();
                        {
                           log.TimeUTC = (DateTime)rd["TimeUTC"];
                           log.SessionCounter = (long)rd["SessionCounter"];
                           log.Delta = rd["Delta"] as double?;
                           log.Speed = rd["Speed"] as double?;
                           log.SeaLevelAltitude = rd["SeaLevelAltitude"] as double?;
                           log.EllipsoidAltitude = rd["EllipsoidAltitude"] as double?;
                           log.SatellitesInView = rd["SatellitesInView"] as System.Byte?;
                           log.SatelliteCount = rd["SatelliteCount"] as System.Byte?;
                           log.Position = new PointLatLng((double)rd["Lat"], (double)rd["Lng"]);
                           log.PositionDilutionOfPrecision = rd["PositionDilutionOfPrecision"] as double?;
                           log.HorizontalDilutionOfPrecision = rd["HorizontalDilutionOfPrecision"] as double?;
                           log.VerticalDilutionOfPrecision = rd["VerticalDilutionOfPrecision"] as double?;
                           log.FixQuality = (FixQuality)((byte)rd["FixQuality"]);
                           log.FixType = (FixType)((byte)rd["FixType"]);
                           log.FixSelection = (FixSelection)((byte)rd["FixSelection"]);
                        }

                        if(log.SessionCounter - lastSessionCounter != 1 && points.Count > 0)
                        {
                           List<GpsLog> ret = new List<GpsLog>(points);
                           points.Clear();
                           {
                              yield return ret;
                           }
                        }

                        points.Add(log);
                        lastSessionCounter = log.SessionCounter;
                     }

                     if(points.Count > 0)
                     {
                        List<GpsLog> ret = new List<GpsLog>(points);
                        points.Clear();
                        {
                           yield return ret;
                        }
                     }

                     points.Clear();
                     points = null;

                     rd.Close();
                  }
               }
            }
            cn.Close();
         }
#else
         return null;
#endif
      }

      static readonly Random r = new Random();

      /// <summary>
      /// gets realtime data from public transport in city vilnius of lithuania
      /// </summary>
      /// <param name="type">type of transport</param>
      /// <param name="line">linenum or null to get all</param>
      /// <param name="ret"></param>
      public static void GetVilniusTransportData(TransportType type, string line, List<VehicleData> ret)
      {
         ret.Clear();

         //http://stops.lt/vilnius/gps.txt?1318577178193
         //http://www.marsrutai.lt/vilnius/Vehicle_Map.aspx?trackID=34006&t=1318577231295
         // http://www.troleibusai.lt/eismas/get_gps.php?allowed=true&more=1&bus=1&rand=0.5487859781558404

         string url = string.Format(CultureInfo.InvariantCulture, "http://www.troleibusai.lt/eismas/get_gps.php?allowed=true&more=1&bus={0}&rand={1}", type == TransportType.Bus ? 2 : 1, r.NextDouble());

         if(!string.IsNullOrEmpty(line))
         {
            url += "&nr=" + line;
         }

#if !PocketPC
         url += "&app=GMap.NET.Desktop";
#else
         url += "&app=GMap.NET.WindowsMobile";
#endif

         string xml = string.Empty;
         {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);

            request.UserAgent = GMapProvider.UserAgent;
            request.Timeout = GMapProvider.TimeoutMs;
            request.ReadWriteTimeout = GMapProvider.TimeoutMs * 6;
            request.Accept = "*/*";
            request.KeepAlive = true;

            using(HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
               using(Stream responseStream = response.GetResponseStream())
               {
                  using(StreamReader read = new StreamReader(responseStream, Encoding.UTF8))
                  {
                     xml = read.ReadToEnd();
                  }
               }
#if PocketPC
               request.Abort();
#endif
               response.Close();
            }
         }

         // 54.690688; 25.2116; 1263522; 1; 48.152; 2011-10-14 14:41:29

         if(!string.IsNullOrEmpty(xml))
         {
            var items = xml.Split('&');

            foreach(var it in items)
            {
               var sit = it.Split(';');
               if(sit.Length == 8)
               {
                  VehicleData d = new VehicleData();
                  {
                     d.Id = int.Parse(sit[2]);
                     d.Lat = double.Parse(sit[0], CultureInfo.InvariantCulture);
                     d.Lng = double.Parse(sit[1], CultureInfo.InvariantCulture);
                     d.Line = sit[3];
                     if(!string.IsNullOrEmpty(sit[4]))
                     {
                        d.Bearing = double.Parse(sit[4], CultureInfo.InvariantCulture);
                     }

                     if(!string.IsNullOrEmpty(sit[5]))
                     {
                        d.Time = sit[5];

                        var t = DateTime.Parse(d.Time);
                        if(DateTime.Now - t > TimeSpan.FromMinutes(5))
                        {
                           continue;
                        }

                        d.Time = t.ToLongTimeString();
                     }

                     d.TrackType = sit[6];
                  }

                  //if(d.Id == 1262760)
                  //if(d.Line == "13")
                  {
                     ret.Add(d);
                  }
               }
            }
         }
         #region -- old --
         //XmlDocument doc = new XmlDocument();
         //{
         //   doc.LoadXml(xml);

         //   XmlNodeList devices = doc.GetElementsByTagName("Device");
         //   foreach(XmlNode dev in devices)
         //   {
         //      VehicleData d = new VehicleData();
         //      d.Id = int.Parse(dev.Attributes["ID"].InnerText);

         //      foreach(XmlElement elem in dev.ChildNodes)
         //      {
         //         // Debug.WriteLine(d.Id + "->" + elem.Name + ": " + elem.InnerText);

         //         switch(elem.Name)
         //         {
         //            case "Lat":
         //            {
         //               d.Lat = double.Parse(elem.InnerText, CultureInfo.InvariantCulture);
         //            }
         //            break;

         //            case "Lng":
         //            {
         //               d.Lng = double.Parse(elem.InnerText, CultureInfo.InvariantCulture);
         //            }
         //            break;

         //            case "Bearing":
         //            {
         //               if(!string.IsNullOrEmpty(elem.InnerText))
         //               {
         //                  d.Bearing = double.Parse(elem.InnerText, CultureInfo.InvariantCulture);
         //               }
         //            }
         //            break;

         //            case "LineNum":
         //            {
         //               d.Line = elem.InnerText;
         //            }
         //            break;

         //            case "AreaName":
         //            {
         //               d.AreaName = elem.InnerText;
         //            }
         //            break;

         //            case "StreetName":
         //            {
         //               d.StreetName = elem.InnerText;
         //            }
         //            break;

         //            case "TrackType":
         //            {
         //               d.TrackType = elem.InnerText;
         //            }
         //            break;

         //            case "LastStop":
         //            {
         //               d.LastStop = elem.InnerText;
         //            }
         //            break;

         //            case "Time":
         //            {
         //               d.Time = elem.InnerText;
         //            }
         //            break;
         //         }
         //      }
         //      ret.Add(d);
         //   }
         //} 
         #endregion
      }

      static string sessionId = string.Empty;

#if !PocketPC
      public static void GetFlightRadarData(List<FlightRadarData> ret, RectLatLng bounds)
      {
         ret.Clear();

         //if(resetSession || string.IsNullOrEmpty(sessionId))
         //{
         //   sessionId = GetFlightRadarContentUsingHttp("http://www.flightradar24.com/", location, zoom, string.Empty);
         //}

         // get track for one object
         //var tm = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
         //var r = GetContentUsingHttp("http://www.flightradar24.com/FlightDataService.php?callsign=WZZ1MF&hex=47340F&date=" + tm, p1, 6, id);
         //Debug.WriteLine(r);

         //if(!string.IsNullOrEmpty(sessionId))
         {
            //var response = GetFlightRadarContentUsingHttp("http://arn.data.fr24.com/zones/fcgi/feed.js?bounds=63.056845879294244,55.95299968262111,5.99853515625,28.54248046875&faa=1&mlat=1&flarm=1&adsb=1&gnd=1&air=1&vehicles=1&estimated=1&maxage=900&gliders=1&stats=1&", location, zoom, sessionId);
            var response = GetFlightRadarContentUsingHttp(string.Format(CultureInfo.InvariantCulture, "http://arn.data.fr24.com/zones/fcgi/feed.js?bounds={0},{1},{2},{3}&faa=1&mlat=1&flarm=1&adsb=1&gnd=1&air=1&vehicles=1&estimated=1&maxage=900&gliders=1&stats=1&", bounds.Top, bounds.Bottom, bounds.Left, bounds.Right));

            var items = response.Split(new string[] { "\n," }, StringSplitOptions.RemoveEmptyEntries);
                        
            //int i = 0;
            foreach(var it in items)
            {
               if(it.Length > 11 && !it.Contains("full_count") && !it.Contains("stats"))
               {
                  var d = it.TrimEnd(']').Replace(":[", ",").Replace("\"", string.Empty);

                  //Debug.WriteLine(++i + " -> " + d);

                  // BAW576":["400803",48.9923,1.8083,"144","36950","462","0512","LFPO","A319","G-EUPC"
                  var par = d.Split(',');
                  if(par.Length >= 9)
                  {
                     var id = Convert.ToInt32(par[0], 16);
                     var name = par[8] + "|" + par[9] + "|" + par[10];
                     var lat = par[2];
                     var lng = par[3];
                     var bearing = par[4];
                     var altitude = (int) (int.Parse(par[5]) * 0.3048) + "m";
                     var speed = (int) (int.Parse(par[6]) * 1.852) + "km/h";

                     FlightRadarData fd = new FlightRadarData();
                     fd.name = name;
                     fd.bearing = int.Parse(bearing);
                     fd.altitude = altitude;
                     fd.speed = speed;
                     fd.point = new PointLatLng(double.Parse(lat, CultureInfo.InvariantCulture), double.Parse(lng, CultureInfo.InvariantCulture));
                     fd.Id = id;

                     ret.Add(fd);

                     //Debug.WriteLine("name: " + name);
                     //Debug.WriteLine("hex: " + hex);
                     //Debug.WriteLine("point: " + fd.point);
                     //Debug.WriteLine("bearing: " + bearing);
                     //Debug.WriteLine("altitude: " + altitude);
                     //Debug.WriteLine("speed: " + speed);
                  }
                  else
                  {
#if DEBUG
                     if(Debugger.IsAttached)
                     {
                        Debugger.Break();
                     }
#endif
                  }
                  //Debug.WriteLine("--------------");
               }
            }
         }
      }

      static string GetFlightRadarContentUsingHttp(string url)
      {
         string ret = string.Empty;

         HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);

         request.UserAgent = GMapProvider.UserAgent;
         request.Timeout = GMapProvider.TimeoutMs;
         request.ReadWriteTimeout = GMapProvider.TimeoutMs * 6;
         request.Accept = "*/*";
         request.Referer = "http://www.flightradar24.com/";
         request.KeepAlive = true;
         //request.Headers.Add("Cookie", string.Format(System.Globalization.CultureInfo.InvariantCulture, "map_lat={0}; map_lon={1}; map_zoom={2}; " + (!string.IsNullOrEmpty(sid) ? "PHPSESSID=" + sid + ";" : string.Empty) + "__utma=109878426.303091014.1316587318.1316587318.1316587318.1; __utmb=109878426.2.10.1316587318; __utmz=109878426.1316587318.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)", p.Lat, p.Lng, zoom));

         using(HttpWebResponse response = request.GetResponse() as HttpWebResponse)
         {
            //if(string.IsNullOrEmpty(sid))
            //{
            //   var c = response.Headers["Set-Cookie"];
            //   //Debug.WriteLine(c);
            //   if(c.Contains("PHPSESSID"))
            //   {
            //      c = c.Split('=')[1].Split(';')[0];
            //      ret = c;
            //   }
            //}

            using(Stream responseStream = response.GetResponseStream())
            {
               using(StreamReader read = new StreamReader(responseStream, Encoding.UTF8))
               {
                  var tmp = read.ReadToEnd();
                  //if(!string.IsNullOrEmpty(sid))
                  {
                     ret = tmp;
                  }
               }
            }

#if PocketPC
            request.Abort();
#endif
            response.Close();
         }

         return ret;
      }
#endif
   }
}
