



using GlobleSituation.Common;
using GlobleSituation.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using MapFrame.Core.Model;
using MapFrame.Core.Interface;

namespace GlobleSituation.Business
{
    class ArcMapBusiness
    {
        /// <summary>
        /// 地图框架
        /// </summary>
        private MapFrame.Core.Interface.IMapLogic mapLogic = null;
        /// <summary>
        /// 图元管理类
        /// </summary>
        private ArcMapElementMgr elementMgr = null;

        private TrackLineManager trackMgr = null;            // 航迹管理

        /// <summary>
        /// 图层名称
        /// </summary>
        private string objLayer = "obj_layer";
        /// <summary>
        /// 航迹线图层
        /// </summary>
        private string trackLineLayer = "trackline_layer";


        private ESRI.ArcGIS.Controls.AxMapControl axMapControl1;


        public ArcMapBusiness(MapFrame.Core.Interface.IMapLogic _mapLogic, ESRI.ArcGIS.Controls.AxMapControl _axMapControl1)
        {
            mapLogic = _mapLogic;
            axMapControl1 = _axMapControl1;

            elementMgr = new ArcMapElementMgr();
            trackMgr = new TrackLineManager();

            mapLogic.AddLayer(objLayer);
            mapLogic.AddLayer(trackLineLayer);

            EventPublisher.TSDataEvent += EventPublisher_TSDataEvent;

        }

        // 接收态势数据
        private void EventPublisher_TSDataEvent(object sender, Model.TSDataEventArgs e)
        {
            if (e.Data == null) return;

            //if (axMapControl1.InvokeRequired)
            //{
            //    axMapControl1.Invoke(new Action(delegate
            //    {
            //        DealRealData(e.Data);
            //    }));
            //}
            //else
            DealRealData(e.Data);
        }

        // 处理实时态势数据
        private void DealRealData(RealData data)
        {
            var point = new MapLngLat(data.Longitude, data.Latitude, data.Altitude);
            string name = data.TargetNum.ToString();
            bool isHaveDraw = elementMgr.IsHaveElement(name);
            trackMgr.AddTrackPoint(name, data.TargetType, point);           // 不管目标是否存在，都要添加航迹

            if (isHaveDraw == false)
            {
                if (AddElement(data))
                {
                    trackMgr.AddShowTrackModel(name, data.TargetType);   // ceshi................
                }
            }
            else
            {
                elementMgr.UpdateElementPosition(name, point);
                UpdateElement(data);
            }
        }

        // 添加目标
        private bool AddElement(RealData data)
        {
            var layer = mapLogic.AddLayer(objLayer);
            if (layer == null) return false;

            string iconUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image\\plane.png");
            string name = data.TargetNum.ToString();
            MapFrame.Core.Model.Kml kml = new MapFrame.Core.Model.Kml();
            kml.Placemark.Name = name;
            kml.Placemark.Graph = new MapFrame.Core.Model.KmlPoint() { Position = new MapFrame.Core.Model.MapLngLat(data.Longitude, data.Latitude), Size = new MapFrame.Core.Model.MapSize(10, 10), Description = name, IcoUrl = iconUrl, TipText = "标牌测试...." };

            bool flag = layer.AddElement(kml);
            if (flag)
            {
                var info = new ElementInfo();
                var point = new MapLngLat(data.Longitude, data.Latitude, data.Altitude);
                info.ElementName = name;
                info.ElementType = ElementType.Picture;
                info.CreateTime = DateTime.Now;
                info.LayerName = objLayer;
                info.Position = point;

                elementMgr.AddElement(name, info);
            }

            return flag;
        }

        // 更新目标
        private bool UpdateElement(RealData data)
        {
            var layer = mapLogic.GetLayer(objLayer);
            if (layer == null) return false;

            string name = data.TargetNum.ToString();
            if (!elementMgr.IsHaveElement(name)) return false;

            var element = mapLogic.GetLayer(objLayer).GetElement(name);
            if (element == null) return false;
            IMFPicture picElement = element as IMFPicture;
            if (picElement == null) return false;
            // 更新目标位置
            picElement.UpdatePosition(data.Longitude, data.Latitude);

            // 更新目标航迹
            if (trackMgr.IsShowTrack(name))
            {
                UpdateElementTrackLine(name);
            }

            return true;
        }

        // 更新目标航迹
        private void UpdateElementTrackLine(string name)
        {
            //List<TrackPoint> trackPoints = trackMgr.GetTrackPoints(name);
            //if (trackPoints == null || trackPoints.Count <= 1) return;    // 如果没有点，或者最多只有一个点，则返回不进行航迹绘制

            #region MyRegion

            //var layer = mapLogic.GetLayerCollection().AddLayer(trackLineLayer);    // 添加航迹点图层
            //if (layer == null) return;

            //if (!elementMgr.IsHaveElement(name)) return;

            //ElementInfo elementInfo = elementMgr.GetElementInfo(name);
            //if (elementInfo == null) return;

            //int count = elementInfo.HistoryPoint.Count;

            //if (count <= 10)
            //{
            //    MapPoint[] points = elementInfo.HistoryPoint.ToArray();
            //    List<MapFrame.Core.Model.MapLngLat> pointList = new List<MapFrame.Core.Model.MapLngLat>();

            //    if (count == 2)  // 第一次创建航迹线
            //    {
            //        // 添加2个点，一条线

            //        for (int i = 0; i < points.Length; i++)
            //        {
            //            MapFrame.Core.Model.Kml kmlPoint = new MapFrame.Core.Model.Kml();
            //            kmlPoint.Placemark.Name = string.Format("{0}_trackpoint_{1}", name, i);
            //            kmlPoint.Placemark.Graph = new MapFrame.Core.Model.KmlPoint() { Position = new MapFrame.Core.Model.MapLngLat(points[i].Longitude, points[i].Latgitude), Color = Color.Gray, Size = new MapFrame.Core.Model.MapSize(20, 20), TipText = "标牌测试...." };
            //            layer.AddElement(kmlPoint);

            //            pointList.Add(new MapFrame.Core.Model.MapLngLat(points[i].Longitude, points[i].Latgitude, 0));
            //        }

            //        MapFrame.Core.Model.Kml kmlLine = new MapFrame.Core.Model.Kml();
            //        kmlLine.Placemark.Name = string.Format("{0}_trackline", name);
            //        MapFrame.Core.Model.KmlLineString line = new MapFrame.Core.Model.KmlLineString();
            //        line.PositionList = pointList;
            //        line.Color = Color.Black;
            //        line.Width = 2;
            //        kmlLine.Placemark.Graph = line;
            //        layer.AddElement(kmlLine);
            //    }
            //    else
            //    {
            //        var point = points[points.Length - 1];
            //        MapFrame.Core.Model.Kml kmlPoint = new MapFrame.Core.Model.Kml();
            //        kmlPoint.Placemark.Name = string.Format("{0}_trackpoint_{1}", name, points.Length - 1);
            //        kmlPoint.Placemark.Graph = new MapFrame.Core.Model.KmlPoint() { Position = new MapFrame.Core.Model.MapLngLat(point.Longitude, point.Latgitude), Color = Color.Gray, Size = new MapFrame.Core.Model.MapSize(20, 20), TipText = "标牌测试...." };
            //        layer.AddElement(kmlPoint);

            //        var e = layer.GetElement(string.Format("{0}_trackline", name));
            //        if (e == null) return;
            //        var line = e as MapFrame.ArcMap.Interface.ILineArcMap;
            //        if (line == null) return;

            //        line.AddPoint(new MapFrame.Core.Model.MapLngLat(point.Longitude, point.Latgitude));
            //    }
            //}
            //else
            //{
            //    // 有11个点，需要删除第一个点
            //    var point = elementInfo.RemoveTrackPoint();
            //    var addPoint = new MapFrame.Core.Model.MapLngLat(point.Longitude, point.Latgitude);
            //    string deletePointName = string.Format("{0}_trackpoint_{1}", name, elementInfo.DeleteIndex);
            //    layer.RemoveElement(deletePointName);
            //    elementInfo.SetDeleteIndex(trackPointCount);

            //    // 绘制一个新点
            //    MapFrame.Core.Model.Kml kmlPoint = new MapFrame.Core.Model.Kml();
            //    kmlPoint.Placemark.Name = deletePointName;
            //    kmlPoint.Placemark.Graph = new MapFrame.Core.Model.KmlPoint() { Position = addPoint, Color = Color.Gray, Size = new MapFrame.Core.Model.MapSize(20, 20), TipText = "标牌测试...." };
            //    layer.AddElement(kmlPoint);

            //    MapFrame.Core.Model.MapLngLat delPoint = new MapFrame.Core.Model.MapLngLat(point.Longitude, point.Latgitude, 0);
            //    var e = layer.GetElement(string.Format("{0}_trackline", name));
            //    if (e == null) return;
            //    var line = e as MapFrame.ArcMap.Interface.ILineArcMap;
            //    if (line == null) return;
            //    line.RemovePoint(delPoint);    // 移除旧点
            //    line.AddPoint(addPoint);       // 添加新点
            //}

            #endregion
        }


    }
}
