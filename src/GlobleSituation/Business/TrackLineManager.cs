
using System.Collections.Generic;
using GlobleSituation.Model;
using GlobleSituation.Common;
using MapFrame.Core.Model;
using System;
using System.Xml;

namespace GlobleSituation.Business
{
    /// <summary>
    /// 航迹管理
    /// </summary>
    public class TrackLineManager
    {
        /// <summary>
        /// 需要显示航迹的目标集合
        /// </summary>
        private List<Track> tracks = new List<Track>();
        /// <summary>
        /// 目标对应的航迹点
        /// </summary>
        private Dictionary<string, TrackLine> modelDic = new Dictionary<string, TrackLine>();
        /// <summary>
        /// 目标对应已绘制的航迹点
        /// </summary>
        private Dictionary<string, List<string>> drawPointDic = new Dictionary<string, List<string>>();
        /// <summary>
        /// 航迹点索引
        /// </summary>
        private uint index = 0;


        public delegate void RemoveCurrTrackLineDeleget(Track track);
        public delegate void RemoveAllTrackLineDeleget(List<Track> ts);

        public RemoveCurrTrackLineDeleget RemoevTrackLine;
        public RemoveAllTrackLineDeleget RemoveAllTrackLine;


        public TrackLineManager()
        {
            ReadConfig();
        }

        /// <summary>
        /// 添加航迹点
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="point"></param>
        public void AddTrackPoint(string modelName, byte type, MapLngLat point)
        {
            try
            {
                if (modelDic.ContainsKey(modelName))
                {
                    TrackPoint tp = new TrackPoint();
                    tp.Index = index;
                    tp.PointName = modelName + "point_" + index;
                    tp.Position = point;

                    modelDic[modelName].Points.Add(tp);

                    if (modelDic[modelName].Points.Count > Utils.TrackPointNum)
                    {
                        modelDic[modelName].Points.RemoveAt(0);
                    }
                }
                else
                {
                    TrackPoint tp = new TrackPoint();
                    tp.Index = index;
                    tp.PointName = modelName + "point_" + index;
                    tp.Position = point;

                    TrackLine line = new TrackLine();
                    line.LineName = modelName + "line";
                    line.ModelName = modelName;
                    line.Points = new List<TrackPoint>();
                    line.Points.Add(tp);
                    line.TargetType = type;

                    modelDic.Add(modelName, line);
                }

                index++;
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(TrackLineManager), ex.Message);
            }
        }

        /// <summary>
        /// 获取航迹集合
        /// </summary>
        /// <param name="modelName"></param>
        public List<TrackPoint> GetTrackPoints(string modelName)
        {
            if (!modelDic.ContainsKey(modelName)) return null;
            return modelDic[modelName].Points;
        }

        /// <summary>
        /// 检查模型是否需要显示航迹
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public bool IsShowTrack(string modelName)
        {
            var t = tracks.Find(obj => obj.ElementName == modelName);
            if (t != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 添加需要显示航迹的目标
        /// </summary>
        /// <param name="modelName">目标名称</param>
        public void AddShowTrackModel(string modelName, byte type)
        {
            if (!IsShowTrack(modelName))
            {
                Track t = new Track(modelName, type);
                tracks.Add(t);
            }
        }

        /// <summary>
        /// 移除需要显示航迹的目标
        /// </summary>
        /// <param name="modelName">目标名称</param>
        public void RemoveShowTrackModel(string modelName)
        {
            var t = tracks.Find(obj => obj.ElementName == modelName);
            if (t != null)
            {
                tracks.Remove(t);
            }
            RemoevTrackLine(t);
        }

        /// <summary>
        /// 显示所有航迹
        /// </summary>
        public void ShowAllTrackLine()
        {
            tracks.Clear();

            foreach (var v in modelDic.Values)
            {
                Track t = new Track(v.ModelName, v.TargetType);
                tracks.Add(t);
            }
        }

        /// <summary>
        /// 关闭所有航迹
        /// </summary>
        /// <returns></returns>
        public List<Track> CloseAllTrackLine()
        {
            List<Track> ts = new List<Track>();
            foreach (Track t in tracks)
            {
                ts.Add(t);
            }
            tracks.Clear();

            RemoveAllTrackLine(ts);
            return ts;
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void ClearData()
        {
            try
            {
                tracks.Clear();
                modelDic.Clear();
                drawPointDic.Clear();
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(TrackLineManager), ex.Message);
            }
        }

        // 读取配置
        private void ReadConfig()
        {
            try
            {
                string xmlConfig = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config\\GlobeConfig.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlConfig);

                XmlNode node;
                node = doc.SelectSingleNode("Globe/Config/TrackPointLimit");
                Utils.TrackPointNum = Convert.ToInt32(node.InnerXml);
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(ArcGlobeBusiness), ex.Message);
            }
        }


    }

    /// <summary>
    /// 需要显示航迹的对象
    /// </summary>
    public class Track
    {
        public Track(string elementName, byte type)
        {
            ElementName = elementName;
            TargetType = type;
        }

        /// <summary>
        /// 图元名称
        /// </summary>
        public string ElementName;

        /// <summary>
        /// 图元类型
        /// </summary>
        public byte TargetType;
    }
}
