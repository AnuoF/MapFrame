
using System.Collections.Generic;
using System.Windows.Forms;
using GlobleSituation.Common;
using GlobleSituation.Model;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System.Drawing;
using System.Diagnostics;
using System.Data;

namespace GlobleSituation.Business
{
    public class GMapControlBusiness
    {
        public IMapLogic mapLogic = null;                                     // 地图框架接口
        private PlaneManager planeMgr = null;                                 // 飞机对象管理类
        private TrackLineManager trackMgr = null;                             // 航迹管理类
        private ArcGlobeBusiness globeBusiness = null;                        // 三维地图业务处理类
        private IMFPicture trackPicture = null;                               // 跟踪目标图元

        private Dictionary<int, List<int>> beamDic = new Dictionary<int, List<int>>();
        private string coverLayerName = "波束图层";
        private double zoom = 1;          // 当前缩放层级
        private double visibleZoom = 5;   // 波束达到多少层级是显示


        public GMapControlBusiness(IMapLogic _mapLogic, TrackLineManager _trackMgr, ArcGlobeBusiness _globeBusiness)
        {
            mapLogic = _mapLogic;
            globeBusiness = _globeBusiness;
            trackMgr = _trackMgr;

            var map = mapLogic.GetIMFMap();
            if (map != null)
            {
                map.MapZoomChangedEvent += new System.EventHandler<MapZoomChangedEventArgs>(map_MapZoomChangedEvent);
            }

            planeMgr = new PlaneManager();
            EventPublisher.MapDealBeamDataEvent += new System.EventHandler<BeamData>(EventPublisher_MapDealBeamDataEvent);
        }

        /// <summary>
        /// 清理态势数据
        /// </summary>
        public void ClearRealData()
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    mapLogic.RemoveLayer(GetModelLayerName((byte)i));   // 移除图层
                }

                planeMgr.ClearData();
                trackMgr.ClearData();
                trackPicture = null;
            }
            catch (System.Exception ex)
            {
                Log4Allen.WriteLog(typeof(ArcGlobeBusiness), ex.Message);
            }
        }

        #region 态势展示

        // 处理实时数据
        public void DealRealData(Model.RealData data)
        {
            var position = new MapLngLat(data.Longitude, data.Latitude, data.Altitude);
            Plane plane = CreateStateModel(data, position);
            trackMgr.AddTrackPoint(plane.Name, data.TargetType, position);

            if (!planeMgr.HasModel(plane))
            {
                if (CreatePlane(plane))
                {
                    planeMgr.AddModel(plane);
                }
            }
            else
            {
                planeMgr.UpdataModel(plane);
                UpdatePlanePosition(plane);

                if (trackMgr.IsShowTrack(plane.Name))
                    UpdateTrackPoint(plane);
            }

            // 目标跟踪
            if (trackPicture != null)
            {
                MapLngLat pos = trackPicture.GetLngLat();
                mapLogic.GetToolBox().ZoomToPosition(pos);
                EventPublisher.PublishJumpToGlobeViewEvent(this, new Model.JumpToGlobeViewEventArgs(trackPicture.ElementName, pos));
            }
        }

        // 处理预警结果
        public void DealWarnData(RealData data, bool isWarn, List<string> warnNames)
        {
            IMFLayer layer = mapLogic.GetLayer(GetModelLayerName(data.TargetType));
            if (layer == null) return;

            IMFElement element = layer.GetElement(data.TargetNum.ToString());
            if (element == null) return;

            if (isWarn == false)
            {
                element.Flash(false);
            }
            else
            {
                if (element.IsFlash == false)
                {
                    element.Flash(true, 500);
                }
            }
        }

        // 绘制动目标
        private bool CreatePlane(Plane plane)
        {
            var layer = mapLogic.AddLayer(plane.LayerName);
            if (layer == null)
            {
                Log4Allen.WriteLog(typeof(GMapControlBusiness), "创建目标失败：添加目标图层失败。");
                return false;
            }

            string tipText = string.Format("目标：{0}", plane.Name);
            Kml kml = new Kml();
            kml.Placemark.Name = plane.Name;
            kml.Placemark.Graph = new KmlPicture() { Position = plane.Coordinate, IconUrl = plane.ImageUrl, Scale = 1, TipText = tipText, LabelText = plane.LabelText, IconColor = plane.PlaneColor };

            return layer.AddElement(kml);
        }

        // 更新目标位置
        private bool UpdatePlanePosition(Plane plane)
        {
            var layer = mapLogic.GetLayer(plane.LayerName);
            if (layer == null) return false;

            var element = layer.GetElement(plane.Name);
            if (element == null) return false;

            IMFPicture picture = element as IMFPicture;
            if (picture == null) return false;

            picture.UpdatePosition(plane.Coordinate);
            if (plane.TargetType == 0)    // 只有飞机才设置方位角
                picture.SetAngle((float)plane.Azimuth);

            layer.Refresh();
            return true;
        }

        // 更新航迹
        private void UpdateTrackPoint(Plane plane)
        {
            List<TrackPoint> trackPoints = trackMgr.GetTrackPoints(plane.Name);
            if (trackPoints == null || trackPoints.Count <= 1) return;

            var layer = mapLogic.AddLayer(plane.LayerName);
            if (layer == null) return;

            string lineName = plane.Name + "line";
            var line = layer.GetElement(lineName);
            if (line == null)
            {
                Kml kmlLine = new Kml();
                kmlLine.Placemark.Name = lineName;
                KmlLineString lineKml = new KmlLineString();
                lineKml.Color = System.Drawing.Color.Blue;
                lineKml.Width = 1;
                List<MapLngLat> plist = new List<MapLngLat>();

                for (int i = 0; i < trackPoints.Count; i++)
                {
                    var point = new MapLngLat(trackPoints[i].Position.Lng, trackPoints[i].Position.Lat, trackPoints[i].Position.Alt);
                    plist.Add(point);
                }

                lineKml.PositionList = plist;
                kmlLine.Placemark.Graph = lineKml;
                var ret = layer.AddElement(kmlLine);
            }
            else
            {
                IMFLine lineElement = line as IMFLine;
                if (line == null) return;

                List<MapLngLat> plist = new List<MapLngLat>();
                for (int i = 0; i < trackPoints.Count; i++)
                {
                    var point = new MapLngLat(trackPoints[i].Position.Lng, trackPoints[i].Position.Lat, trackPoints[i].Position.Alt);
                    plist.Add(point);
                }

                lineElement.UpdatePosition(plist);
            }
        }

        // 创建目标对象
        private Plane CreateStateModel(RealData data, MapLngLat postion)
        {
            Plane p = new Plane();
            p.Name = data.TargetNum.ToString();
            p.Azimuth = data.ActionRange;
            p.Coordinate = postion;
            p.TrackLayerName = GetModelLayerName(data.TargetType);
            p.TargetType = data.TargetType;
            p.PlaneColor = data.TargetProperty == 0 ? Color.Red : Color.Blue;

            string text = string.Format("目标：{0}\n经度：{1}\n纬度：{2}\n高度：{3}\n信息来源：{4}\n国家：{5}\n目标性质：{6}\n目标类别：{7}\n装备型号：{8}\n位置时间：{9}",
                data.TargetNum, data.Longitude, data.Latitude, data.Altitude, data.InformationSource, data.Country, data.TargetProperty, data.TargetType, data.EquipModelNumber, data.PositionDate);
            p.LabelText = text;

            string path = "";
            string layerName = "";

            switch (data.TargetType)
            {
                case 0:    // 空中目标
                    path = string.Format("{0}|{1}|{2}", Application.StartupPath + "\\Image\\TTMITC.TTF", 88, 18);
                    layerName = "天空态势图层";
                    break;
                case 1:    // 陆地目标
                    path = string.Format("{0}|{1}|{2}", Application.StartupPath + "\\Image\\TTMITC.TTF", 92, 18);
                    layerName = "陆地态势图层";
                    break;
                case 2:    // 海洋目标
                    path = string.Format("{0}|{1}|{2}", Application.StartupPath + "\\Image\\TTMITC.TTF", 97, 18);
                    layerName = "海洋态势图层";
                    break;
                case 3:    // 未知目标
                    path = string.Format("{0}|{1}|{2}", Application.StartupPath + "\\Image\\TTMITC.TTF", 86, 18);
                    layerName = "未知目标图层";
                    break;
                default:
                    path = string.Format("{0}|{1}|{2}", Application.StartupPath + "\\Image\\TTMITC.TTF", 86, 18);
                    layerName = "未知目标图层";
                    break;
            }

            p.LayerName = layerName;
            p.ImageUrl = path;
            return p;
        }

        // 显示航迹
        public void DoShowTrackLine(string layerName, string elementName, bool bShow)
        {
            if (bShow)
            {
                trackMgr.AddShowTrackModel(elementName, GetTypeByLayerName(layerName));
            }
            else
            {
                trackMgr.RemoveShowTrackModel(elementName);
                var layer = mapLogic.GetLayer(layerName);
                if (layer == null) return;
                layer.RemoveElement(elementName + "line");
            }
        }

        // 显示所有航迹
        public void ShowAllTrackLine(bool bShow)
        {
            if (bShow)
            {
                trackMgr.ShowAllTrackLine();
            }
            else
            {
                List<Track> lines = trackMgr.CloseAllTrackLine();
                if (lines == null) return;

                foreach (var track in lines)
                {
                    var layer = mapLogic.GetLayer(GetModelLayerName(track.TargetType));
                    if (layer == null) continue;

                    layer.RemoveElement(track.ElementName + "line");
                }
            }
        }

        // 删除目标
        public void DeletePlane(string layerName, string elementName)
        {
            var layer = mapLogic.GetLayer(layerName);
            if (layer == null) return;

            layer.RemoveElement(elementName);             // 删除图元
            layer.RemoveElement(elementName + "line");    // 删除航迹
            planeMgr.DeleteModel(elementName);            // 从管理中删除

            globeBusiness.DeletePlane(layerName, elementName);       // 删除三维地图中对应的模型
        }

        // 设置要跟踪的目标
        public void SetTrackElement(IMFPicture element, bool bTrack)
        {
            trackPicture = element;
            if (bTrack == false)
            {
                trackPicture = null;
            }
        }

        /// <summary>
        /// 跳转到某目标
        /// </summary>
        /// <param name="name"></param>
        public void JumpToPlane(byte type, string elementName)
        {
            if (mapLogic == null) return;
            IMFLayer layer = mapLogic.GetLayer(GetModelLayerName(type));
            if (layer == null) return;

            IMFElement element = layer.GetElement(elementName);
            if (element == null) return;

            IMFPicture model = element as IMFPicture;
            if (model == null) return;

            MapLngLat position = model.GetLngLat(); ;
            IMFToolBox toolBox = mapLogic.GetToolBox();
            if (toolBox == null) return;

            toolBox.ZoomToPosition(position);
        }
        #endregion

        #region 波束覆盖


        private void map_MapZoomChangedEvent(object sender, MapZoomChangedEventArgs e)
        {
            zoom = e.Zoom;
            SetTextVisible();
        }

        // 接收波束数据
        private void EventPublisher_MapDealBeamDataEvent(object sender, BeamData e)
        {
            DealBeamCover(e);
        }

        // 处理波束数据
        private void DealBeamCover(BeamData e)
        {
            if (e.Point.Alt > 0) return;    // 卫星数据，不做处理
            if (mapLogic == null) return;
            // 添加圆图元（波束覆盖）
            IMFLayer layer = mapLogic.AddLayer(coverLayerName);
            if (layer == null) return;

            string circleName = string.Format("卫星{0}-波束{1}", e.SatelliteId, e.BeamId);
            string textName = string.Format("卫星{0}-波束{1}_描述", e.SatelliteId, e.BeamId, e.BeamId);

            if (!beamDic.ContainsKey(e.SatelliteId))   // 新的波束
            {
                Kml kmlCircle = new Kml();
                kmlCircle.Placemark.Name = circleName;
                KmlCircle circle = new KmlCircle();
                circle.Position = e.Point;
                circle.FillColor = Color.FromArgb(50, Color.Green);
                circle.Radius = 500000;
                circle.StrokeColor = Color.Blue;
                circle.StrokeWidth = 1;
                kmlCircle.Placemark.Graph = circle;
                layer.AddElement(kmlCircle);

                // 添加文字图元
                Kml kmlText = new Kml();
                kmlText.Placemark.Name = textName;
                string context = string.Format("卫星{0}-波束{1}", e.SatelliteId, e.BeamId);
                kmlText.Placemark.Graph = new KmlText() { Position = e.Point, Content = context, Color = Color.Blue, Font = "宋体", Size = 10 };

                IMFElement elementText;
                if (layer.AddElement(kmlText, out elementText))
                {
                    bool visible = zoom >= visibleZoom ? true : false;
                    elementText.SetVisible(visible);
                }


                // 添加到字典进行维护
                List<int> beamIdList = new List<int>();
                beamIdList.Add(e.BeamId);
                lock (beamDic)
                {
                    beamDic.Add(e.SatelliteId, beamIdList);
                }
            }
            else
            {
                if (!beamDic[e.SatelliteId].Contains(e.BeamId))   // 新的波束
                {
                    Kml kmlCircle = new Kml();
                    kmlCircle.Placemark.Name = circleName;
                    KmlCircle circle = new KmlCircle();
                    circle.Position = e.Point;
                    circle.FillColor = Color.FromArgb(50, Color.Green);
                    circle.Radius = 500000;
                    circle.StrokeColor = Color.Blue;
                    circle.StrokeWidth = 1;
                    kmlCircle.Placemark.Graph = circle;
                    layer.AddElement(kmlCircle);


                    // 添加文字图元
                    string context = string.Format("卫星{0}-波束{1}", e.SatelliteId, e.BeamId);
                    Kml kmlText = new Kml();
                    kmlText.Placemark.Name = textName;
                    kmlText.Placemark.Graph = new KmlText() { Position = e.Point, Content = context, Color = Color.Blue, Font = "宋体", Size = 10 };

                    IMFElement elementText;
                    if (layer.AddElement(kmlText, out elementText))
                    {
                        bool visible = zoom >= visibleZoom ? true : false;
                        elementText.SetVisible(visible);
                    }

                    lock (beamDic)
                    {
                        // 添加到字典进行维护
                        beamDic[e.SatelliteId].Add(e.BeamId);
                    }
                }
                else
                {
                    // 更新圆图元（波束覆盖）位置
                    IMFElement elementCircle = layer.GetElement(circleName);
                    if (elementCircle != null)
                    {
                        IMFCircle circle = elementCircle as IMFCircle;
                        if (circle != null)
                        {
                            circle.UpdatePosition(e.Point);
                        }
                    }

                    // 更新文字图元（描述信息）位置
                    IMFElement elementText = layer.GetElement(textName);
                    if (elementText != null)
                    {
                        IMFText text = elementText as IMFText;
                        if (text != null)
                        {
                            text.UpdatePosition(e.Point);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 设置文字是否可见
        /// </summary>
        public void SetTextVisible(bool show = true, string name = "")
        {
            if (mapLogic == null) return;
            IMFLayer layer = mapLogic.GetLayer(coverLayerName);
            if (layer == null) return;

            bool visible = zoom >= visibleZoom ? true : false;
            if (visible)
            {
                visible = show;
            }

            if (string.IsNullOrEmpty(name))
            {
                lock (beamDic)
                {
                    foreach (int satelliteId in beamDic.Keys)
                    {
                        foreach (int beamId in beamDic[satelliteId])
                        {
                            string textName = string.Format("卫星{0}-波束{1}_描述", satelliteId, beamId);
                            IMFElement element = layer.GetElement(textName);
                            if (element != null)
                            {
                                element.SetVisible(visible);
                            }
                        }
                    }
                }
            }
            else
            {
                string textName = name + "_描述";
                IMFElement element = layer.GetElement(textName);
                if (element != null)
                {
                    element.SetVisible(visible);
                }
            }
        }

        // 设置波束覆盖是否可见
        public void SetCoverLayerVisible(bool visible)
        {
            IMFLayer layer = mapLogic.GetLayer(coverLayerName);
            if (layer == null) return;

            layer.SetLayerVisible(visible);
        }

        #endregion

        #region 设备服务

        // 绘制设备服务
        public void DrawDeviceRanage(DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0) return;

            foreach (DataRow row in dt.Rows)
            {
                DeviceData device = new DeviceData(row);

                IMFLayer layer = mapLogic.AddLayer("设备服务图层");
                if (layer == null) continue;

                // 绘制设备（基站）
                string url = string.Format("{0}|{1}|{2}", Application.StartupPath + "\\Image\\TTMITC.TTF", 113, 20);
                Kml kml = new Kml();
                kml.Placemark.Name = device.DeviceNumber;
                kml.Placemark.Graph = new KmlPicture() { Position = new MapLngLat(device.Lng, device.Lat), IconUrl = url, Scale = 1, TipText = device.DeviceNumber, LabelText = device.DeviceName, IconColor = Color.Red };
                layer.AddElement(kml);

                // 绘制服务范围（面）
                Kml kmlCircle = new Kml();
                kmlCircle.Placemark.Name = device.DeviceNumber + "polygon";
                KmlCircle circle = new KmlCircle();
                circle.Position = new MapLngLat(device.Lng, device.Lat);
                circle.FillColor = Color.FromArgb(50, Color.Green);
                circle.Radius = device.RangeRadius * 1000;
                circle.StrokeColor = Color.Blue;
                circle.StrokeWidth = 1;
                kmlCircle.Placemark.Graph = circle;
                layer.AddElement(kmlCircle);
            }
        }

        // 清除设备
        public void ClearDevice()
        {
            mapLogic.RemoveLayer("设备服务图层");
        }

        // 设置设备服务图层显示、隐藏
        public void SetDeviceRangeLayerVisible(bool visible)
        {
            IMFLayer layer = mapLogic.GetLayer("设备服务图层");
            if (layer == null) return;

            layer.SetLayerVisible(visible);
        }

        // 设置设备显示、隐藏
        public void SetDeviceVisible(string deviceName, bool visible)
        {
            IMFLayer layer = mapLogic.GetLayer("设备服务图层");
            if (layer == null) return;

            IMFElement element = layer.GetElement(deviceName);
            if (element != null)
            {
                IMFPicture picture = element as IMFPicture;
                if (picture != null)
                {
                    picture.SetVisible(visible);
                }
            }

            IMFElement ele = layer.GetElement(deviceName + "polygon");
            if (ele != null)
            {
                IMFCircle circle = ele as IMFCircle;
                if (circle != null)
                {
                    circle.SetVisible(visible);
                }
            }
        }
        #endregion

        // 获取目标对应的图层
        private string GetModelLayerName(byte targetType)
        {
            string layerName;

            switch (targetType)
            {
                case 0:    // 空中目标
                    layerName = "天空态势图层";
                    break;
                case 1:    // 陆地目标
                    layerName = "陆地态势图层";
                    break;
                case 2:    // 海洋目标
                    layerName = "海洋态势图层";
                    break;
                case 3:    // 未知目标
                    layerName = "未知目标图层";
                    break;
                default:
                    layerName = "未知目标图层";
                    break;
            }

            return layerName;
        }

        private byte GetTypeByLayerName(string layerName)
        {
            byte type = 0;

            switch (layerName)
            {
                case "天空态势图层":
                    type = 0;
                    break;
                case "陆地态势图层":
                    type = 1;
                    break;
                case "海洋态势图层":
                    type = 2;
                    break;
                case "未知目标图层":
                    type = 3;
                    break;
                default:
                    type = 3;
                    break;
            }

            return type;
        }
    }
}
