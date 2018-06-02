
using System.Collections.Generic;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System.Drawing;
using GlobleSituation.Model;

namespace GlobleSituation.Business
{
    /// <summary>
    /// 预警管理
    /// </summary>
    public class WarnManager
    {
        public IMapLogic globeMapLogic = null;
        public IMapLogic mapMapLogic = null;

        private string layerName = "warnLayer";
        private Dictionary<string, WarnArea> warnAresDic = new Dictionary<string, WarnArea>();
        public double Atilute = 500000;

        public WarnManager(IMapLogic _globeMapLogic, IMapLogic _mapMapLogic)
        {
            globeMapLogic = _globeMapLogic;
            mapMapLogic = _mapMapLogic;

            mapMapLogic.AddLayer(layerName);
        }

        #region Public Function

        /// <summary>
        /// 判断点是否在多边形内（预警）
        /// </summary>
        /// <param name="point"></param>
        /// <param name="warnNames"></param>
        /// <returns></returns>
        public bool Warn(RealData dataIn, out RealData dataOut, out bool isWarn, out List<string> warnNames)
        {
            warnNames = new List<string>();
            isWarn = false;
            dataOut = dataIn;

            if (dataIn.Altitude > Atilute) return false;    // 如果高度超过预警高度，则直接返回false

            foreach (var item in warnAresDic.Values)
            {
                if (item.IsWarn == false) continue;

                if (MapFrame.Core.Common.Utils.IsInsidePolygon(new MapLngLat(dataIn.Longitude, dataIn.Latitude, dataIn.Altitude), item.Points))
                {
                    isWarn = true;
                    warnNames.Add(item.Name);
                }
            }

            return isWarn;
        }

        /// <summary>
        /// 绘制预警区域
        /// </summary>
        /// <param name="name">区域名称</param>
        /// <param name="points">点集合</param>
        /// <param name="isWarn">是否预警</param>
        /// <param name="isImportant">是否是重点区域</param>
        public void DrawArea(string name, List<MapLngLat> points, bool isWarn, bool isImportant)
        {
            if (points.Count < 3)
                return;

            DrawAreaGlobe(name, points, isWarn, isImportant);
            DrawAreaArcMap(name, points, isWarn);
        }

        /// 重绘区域
        /// </summary>
        /// <param name="name"></param>
        /// <param name="points"></param>
        public void ReDrawArea(string name, List<MapLngLat> points)
        {
            // 先删除之前的面，再添加新的面
            if (!warnAresDic.ContainsKey(name)) return;

            WarnArea area = warnAresDic[name];
            foreach (string polygonName in area.Polygons)
            {
                IMFLayer layer = globeMapLogic.GetLayer(layerName);
                if (layer == null) return;

                layer.RemoveElement(polygonName);
            }

            bool isWarn = area.IsWarn;
            bool isImportant = area.IsImportant;
            warnAresDic.Remove(name);

            DrawAreaGlobe(name, points, isWarn, isImportant);

            // arcmap
            IMFLayer layerMap = mapMapLogic.AddLayer(layerName);
            if (layerMap == null) return;

            IMFElement ele = layerMap.GetElement(name);
            if (ele == null) return;
            IMFPolygon polygon = ele as IMFPolygon;
            if (polygon == null) return;
            polygon.UpdatePosition(points);
        }

        /// <summary>
        /// 获取区域信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WarnArea GetAreaByName(string name)
        {
            if (!warnAresDic.ContainsKey(name)) return null;
            return warnAresDic[name];
        }

        /// <summary>
        /// 删除预警区域
        /// </summary>
        /// <param name="name">区域名称</param>
        public void DeleteArea(string name)
        {
            if (!warnAresDic.ContainsKey(name)) return;

            DeleteAreaGlobe(name);
            DeleteAreaArcMap(name);

            warnAresDic.Remove(name);
        }

        /// <summary>
        /// 设置区域预警
        /// </summary>
        /// <param name="name">区域名称</param>
        /// <param name="isWarn">是否预警</param>
        public void SetAreaWarn(string name, bool isWarn)
        {
            if (!warnAresDic.ContainsKey(name)) return;
            warnAresDic[name].IsWarn = isWarn;
        }

        /// <summary>
        /// 设置重点区域
        /// </summary>
        /// <param name="name">区域名称</param>
        /// <param name="isImportant">是否重点</param>
        public void SetAreaImportant(string name, bool isImportant)
        {
            if (!warnAresDic.ContainsKey(name)) return;
            warnAresDic[name].IsImportant = isImportant;
        }

        /// <summary>
        /// 设置区域显示或隐藏
        /// </summary>
        /// <param name="name">区域名称</param>
        /// <param name="visible">显示、隐藏</param>
        public void SetAreaVisible(string name, bool visible)
        {
            if (!warnAresDic.ContainsKey(name)) return;

            warnAresDic[name].IsVisible = visible;

            SetAreaVisibleGlobe(name, visible);
            SetAreaVisibleArcMap(name, visible);
        }

        /// <summary>
        /// 设置区域颜色
        /// </summary>
        /// <param name="name">区域名称</param>
        /// <param name="color">颜色</param>
        public void SetAreaColor(string name, Color color)
        {
            if (!warnAresDic.ContainsKey(name)) return;

            SetAreaColorGlobe(name, color);
            SetAreaColorArcMap(name, color);
        }
        #endregion

        #region Private Function

        private void SetAreaVisibleGlobe(string name, bool visible)
        {
            IMFLayer layer = globeMapLogic.GetLayer(layerName);
            if (layer == null) return;


            foreach (string polygonName in warnAresDic[name].Polygons)
            {
                IMFElement element = layer.GetElement(polygonName);
                element.SetVisible(visible);
            }

            layer.Refresh();
        }

        private void SetAreaVisibleArcMap(string name, bool visible)
        {
            var layer = mapMapLogic.GetLayer(layerName);
            if (layer == null) return;

            var element = layer.GetElement(name);
            if (element == null) return;

            element.SetVisible(visible);
        }

        private void DeleteAreaGlobe(string name)
        {
            IMFLayer layer = globeMapLogic.GetLayer(layerName);
            if (layer == null) return;

            WarnArea area = warnAresDic[name];

            foreach (string eleName in area.Polygons)
            {
                layer.RemoveElement(eleName);
            }

            layer.Refresh();
        }

        private void DeleteAreaArcMap(string name)
        {
            IMFLayer layerMap = mapMapLogic.GetLayer(layerName);
            if (layerMap == null) return;

            layerMap.RemoveElement(name);
            layerMap.Refresh();
        }

        private void DrawAreaGlobe(string name, List<MapLngLat> points, bool isWarn,bool isImportant)
        {
            IMFLayer layerGlobe = globeMapLogic.AddLayer(layerName);
            WarnArea area = new WarnArea();
            area.Name = name;
            area.Points = points;
            area.IsWarn = isWarn;
            area.IsVisible = true;
            area.IsImportant = isImportant;

            // 最后一个面
            List<MapLngLat> pLast = new List<MapLngLat>();
            pLast.Add(new MapLngLat(points[0].Lng, points[0].Lat, 0));
            pLast.Add(new MapLngLat(points[0].Lng, points[0].Lat, Atilute));
            pLast.Add(new MapLngLat(points[points.Count - 1].Lng, points[points.Count - 1].Lat, Atilute));
            pLast.Add(new MapLngLat(points[points.Count - 1].Lng, points[points.Count - 1].Lat, 0));

            Kml polygonLast = new Kml();
            polygonLast.Placemark.Name = name + (points.Count - 1);
            polygonLast.Placemark.Graph = new KmlPolygon() { Description = name + (points.Count - 1), PositionList = pLast, OutLineColor = Color.Blue, FillColor = Color.FromArgb(80, Color.Blue), OutLineSize = 3 };    //position 

            layerGlobe.AddElement(polygonLast);

            area.Polygons.Add(name + (points.Count - 1));

            // 最上面的面
            List<MapLngLat> pUp = new List<MapLngLat>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                string eleName = name + i;
                area.Polygons.Add(eleName);

                List<MapLngLat> pArr = new List<MapLngLat>();
                pArr.Add(new MapLngLat(points[i].Lng, points[i].Lat, 0));
                pArr.Add(new MapLngLat(points[i].Lng, points[i].Lat, Atilute));
                pArr.Add(new MapLngLat(points[i + 1].Lng, points[i + 1].Lat, Atilute));
                pArr.Add(new MapLngLat(points[i + 1].Lng, points[i + 1].Lat, 0));

                Kml polygonKml = new Kml();
                polygonKml.Placemark.Name = eleName;
                polygonKml.Placemark.Graph = new KmlPolygon() { Description = eleName, PositionList = pArr, OutLineColor = Color.Blue, FillColor = Color.FromArgb(80, Color.Blue), OutLineSize = 3 };    //position 
                // 创建波束图元
                layerGlobe.AddElement(polygonKml);

                pUp.Add(new MapLngLat(points[i].Lng, points[i].Lat, Atilute));
            }

            pUp.Add(new MapLngLat(points[points.Count - 1].Lng, points[points.Count - 1].Lat, Atilute));
            Kml polygonUp = new Kml();
            polygonUp.Placemark.Name = name + points.Count;
            polygonUp.Placemark.Graph = new KmlPolygon() { Description = name + points.Count, PositionList = pUp, OutLineColor = Color.Blue, FillColor = Color.FromArgb(80, Color.Blue), OutLineSize = 3 };    //position 
            layerGlobe.AddElement(polygonUp);
            area.Polygons.Add(name + points.Count);

            layerGlobe.Refresh();
            warnAresDic.Add(name, area);
        }

        private void DrawAreaArcMap(string name, List<MapLngLat> points, bool isWarn)
        {
            IMFLayer layer = mapMapLogic.AddLayer(layerName);
            if (layer == null) return;

            Kml polygonKml = new Kml();
            polygonKml.Placemark.Name = name;
            polygonKml.Placemark.Graph = new KmlPolygon() { Description = name, PositionList = points, OutLineColor = Color.Blue, FillColor = Color.FromArgb(80, Color.Blue), OutLineSize = 1 };    //position 

            layer.AddElement(polygonKml);
        }

        private void SetAreaColorGlobe(string name, Color color)
        {
            IMFLayer layer = globeMapLogic.GetLayer(layerName);
            if (layer == null) return;

            foreach (string polygonName in warnAresDic[name].Polygons)
            {
                var element = layer.GetElement(polygonName);
                if (element == null) continue;

                IMFPolygon polygon = element as IMFPolygon;
                if (polygon == null) continue;
                Color c = Color.FromArgb(80, color.R, color.G, color.B);
                polygon.SetFillColor(c);
            }

            layer.Refresh();
        }

        private void SetAreaColorArcMap(string name, Color color)
        {
            IMFLayer layer = mapMapLogic.GetLayer(layerName);
            if (layer == null) return;

            IMFElement element = layer.GetElement(name);
            if (element == null) return;

            IMFPolygon polygon = element as IMFPolygon;
            if (polygon == null) return;

            polygon.SetFillColor(Color.FromArgb(80, color));
        }

        #endregion

    }

    /// <summary>
    /// 预警区域
    /// </summary>
    public class WarnArea
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 区域点集合
        /// </summary>
        public List<MapLngLat> Points;

        /// <summary>
        /// 多边形集合
        /// </summary>
        public List<string> Polygons;

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible;

        /// <summary>
        /// 是否是重点区域
        /// </summary>
        public bool IsImportant;

        /// <summary>
        /// 是否预警
        /// </summary>
        public bool IsWarn;

        public WarnArea()
        {
            Polygons = new List<string>();
        }

    }
}
