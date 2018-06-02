
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using ESRI.ArcGIS.Display;
using GlobleSituation.Common;
using GlobleSituation.Model;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using System.Data;

namespace GlobleSituation.Business
{
    public class ArcGlobeBusiness
    {
        public MapFrame.Core.Interface.IMapLogic mapLogic = null;

        private BeamManager beamMgr = null;                  // 波束管理
        private ModelManager modelMgr = null;                // 卫星波束模型管理
        private ModelManager tsModelMgr = null;              // 态势模型管理
        private System.Media.SoundPlayer soundPlayer = null; // 音频播放器
        private int satelliteScale = 80000;                  // 卫星模型比例
        private int planeScale = 20000;                      // 飞机模型比例
        private int beamAlpha = 50;                          // 波束透明度
        private int stepValue = 32;                          // 步进（绘制波束图形，步进越小越圆）
        private float beamRadius = 500;                      // 波束覆盖半径
        private TrackLineManager trackMgr = null;            // 航迹管理

        private string satelliteLayerName = "卫星图层";      // 卫星图层
        private string beamLayerName = "波束图层";           // 波束图层
        private string coverLayerName = "覆盖图层";          // 覆盖图层


        public ArcGlobeBusiness(MapFrame.Core.Interface.IMapLogic mapLogic, TrackLineManager _trackMgr)
        {
            beamMgr = new BeamManager();
            modelMgr = new ModelManager();
            tsModelMgr = new ModelManager();
            trackMgr = _trackMgr;

            trackMgr.RemoevTrackLine += RemoveTrackLineEx;
            trackMgr.RemoveAllTrackLine += RemoveAllTrackLineEx;

            this.mapLogic = mapLogic;

            soundPlayer = new System.Media.SoundPlayer();
            string wavFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Image\warn.wav";
            soundPlayer.SoundLocation = wavFile;

            EventPublisher.BeamDataComeEvent += new EventHandler<Model.BeamData>(EventPublisher_BeamDataComeEvent);    // 卫星波束数据

            ReadConfig();    // 读取配置文件，初始化
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
                    mapLogic.RemoveLayer(GetModelLayerName((byte)i));
                }

                tsModelMgr.ClearData();
                trackMgr.ClearData();
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(ArcGlobeBusiness), ex.Message);
            }
        }

        #region 卫星波束数据展示
        // 接收波束数据
        private void EventPublisher_BeamDataComeEvent(object sender, Model.BeamData e)
        {
            if (e == null) return;
            DealBeamData(e);
        }

        // 处理波束数据
        public void DealBeamData(Model.BeamData beamData)
        {
            if (beamData.PointType == 1)   //  波束
            {
                // 先获取卫星，如果没有，则不创建波束
                string satelliteId = string.Format("卫星{0}", beamData.SatelliteId);
                Model3D model = modelMgr.FindModel(satelliteId);
                if (model == null) return;

                EventPublisher.PublishMapDealBeamDataEvent(this, beamData);

                Beam beam = CreateBeam(beamData, model);
                beam.SatellitePoint = model.Coordinate;

                if (beamMgr.HasBeam(satelliteId, beam))   // 波束存在，移动波束
                {
                    UpdateBeamElement(beam, model);
                    beamMgr.UpdataBeamCenterPoint(satelliteId, beam);
                }
                else                         // 波束不存在，创建波束
                {
                    if (AddBeamElement(beam, model))
                        beamMgr.AddBeam(satelliteId, beam);
                }
            }
            else   // 卫星
            {
                string modelName = string.Format("卫星{0}", beamData.SatelliteId);
                Model3D model = CreateSatelliteModel(modelName, beamData.Point);

                if (modelMgr.HasModel(model))       // 卫星存在，则更新卫星信息，并移动波束的位置
                {
                    modelMgr.UpdataModel(model);    // 刷新卫星信息
                    UpdateModelPosition(model);     // 更新卫星位置

                    beamMgr.UpdataBeamCenterPoint(modelName, beamData.Point);    // 刷新波束信息

                    // 移动波束的位置
                    List<Beam> beamList = beamMgr.FindBeams(modelName, o => o.SatelliteID == beamData.SatelliteId);
                    if (beamList == null) return;
                    foreach (Beam b in beamList)
                    {
                        UpdateBeamElement(b, model);
                    }
                }
                else     // 卫星不存在，则创建卫星
                {
                    if (AddModel2Earth(model))
                    {
                        modelMgr.AddModel(model);
                    }
                }
            }
        }

        // 添加卫星模型图元
        private bool AddModel2Earth(Model3D model)
        {
            var layer = mapLogic.AddLayer(model.LayerName);
            if (layer == null)
            {
                Log4Allen.WriteLog(typeof(ArcGlobeBusiness), "创建卫星模型失败：添加图层失败。");
                return false;
            }

            Kml kml = new Kml();
            kml.Placemark.Name = model.ModelName;
            KmlModel3d modelKml = new KmlModel3d();

            modelKml.ModelFilePath = model.ModelPath;
            modelKml.Description = model.ModelName;
            modelKml.Position = model.Coordinate;
            modelKml.Scale = model.Scale;
            modelKml.Azimuth = model.Azimuth;
            kml.Placemark.Graph = modelKml;

            MapFrame.Core.Interface.IMFElement element = null;
            var ret = layer.AddElement(kml, out element);
            if (ret)
                EventPublisher.PublishElementAddEvent(this, new ElementAddEventArgs(model.LayerName, model.ModelName));

            return ret;
        }

        // 更新卫星模型图元位置
        private bool UpdateModelPosition(Model3D model)
        {
            // 更新卫星位置
            var layer = mapLogic.GetLayer(model.LayerName);
            if (layer == null) return false;

            var element = layer.GetElement(model.ModelName);
            if (element == null) return false;

            I3DModel modelElement = element as I3DModel;
            if (modelElement == null) return false;

            MapLngLat lnglat = new MapLngLat(model.Coordinate.Lng, model.Coordinate.Lat, model.Coordinate.Alt);
            modelElement.UpdateModel(lnglat, model.Azimuth + 90);    // 更新模型

            layer.Refresh();

            return true;
        }

        // 添加波束图元
        private bool AddBeamElement(Beam beam, Model3D model)
        {
            var coverLayer = mapLogic.AddLayer(coverLayerName);
            var beamLayer = mapLogic.AddLayer(beam.BeamLayerName);

            if (beamLayer == null) return false;

            string elementName = beam.BeamName;
            List<MapLngLat> pointListPolygon = new List<MapLngLat>();
            List<MapLngLat> pointListCover = new List<MapLngLat>();

            pointListPolygon.Add(model.Coordinate);
            //pointListCover.Add(new MapLngLat(beam.CenterPoint.Lng, beam.CenterPoint.Lat, 100000));

            for (double tempAngle = -180; tempAngle <= 180; tempAngle = tempAngle + beam.StepValue)
            {
                var p = Utils.GetPointByDistanceAndAngle(beam.Radius, beam.CenterPoint, tempAngle);
                if (p != null)
                {
                    pointListPolygon.Add(p);
                    pointListCover.Add(new MapLngLat(p.Lng, p.Lat, 100000));
                }
            }

            if (coverLayer != null)
            {
                // 创建覆盖图元
                Kml polygonCover = new Kml();
                polygonCover.Placemark.Name = elementName + "cover";
                polygonCover.Placemark.Graph = new KmlPolygon() { Description = elementName + "cover", PositionList = pointListCover, OutLineColor = Color.Red, FillColor = Color.FromArgb(20, Color.Blue), OutLineSize = 2 };
                coverLayer.AddElement(polygonCover);

                // 线
                Kml kmlLine = new Kml();
                kmlLine.Placemark.Name = elementName + "cover_line";
                KmlLineString linekml = new KmlLineString();
                linekml.Color = Color.Blue;
                linekml.Width = 2;
                linekml.PositionList = pointListCover;
                linekml.Rasterize = false;
                kmlLine.Placemark.Graph = linekml;
                coverLayer.AddElement(kmlLine);
            }

            // 创建波束图元
            Kml polygonKml = new Kml();
            polygonKml.Placemark.Name = elementName;
            polygonKml.Placemark.Graph = new KmlPolygon() { Description = elementName, PositionList = pointListPolygon, OutLineColor = Color.Red, FillColor = Color.FromArgb(beamAlpha, Color.YellowGreen), OutLineSize = 3 };
            if (beamLayer.AddElement(polygonKml))
                EventPublisher.PublishElementAddEvent(this, new ElementAddEventArgs(beam.BeamLayerName, elementName));

            beamLayer.Refresh();   // 波束整体刷新

            return true;
        }

        // 更新波束位置
        private void UpdateBeamElement(Beam beam, Model3D model)
        {
            var layer = mapLogic.GetLayer(beam.BeamLayerName);
            if (layer == null) return;

            string elementName = beam.BeamName;
            List<MapLngLat> pointListPolygon = new List<MapLngLat>();
            List<MapLngLat> pointListCover = new List<MapLngLat>();

            pointListPolygon.Add(model.Coordinate);
            //pointListCover.Add(new MapLngLat(beam.CenterPoint.Lng, beam.CenterPoint.Lat, 100000));

            for (double tempAngle = -180; tempAngle <= 180; tempAngle = tempAngle + beam.StepValue)
            {
                var p = Utils.GetPointByDistanceAndAngle(beam.Radius, beam.CenterPoint, tempAngle);
                if (p != null)
                {
                    pointListPolygon.Add(p);
                    pointListCover.Add(new MapLngLat(p.Lng, p.Lat, 100000));
                }
            }

            var coverLayer = mapLogic.AddLayer(coverLayerName);
            if (coverLayer != null)
            {
                // 线
                IMFElement eleLine = coverLayer.GetElement(elementName + "cover_line");
                if (eleLine != null)
                {
                    IMFLine line = eleLine as IMFLine;
                    if (line != null)
                    {
                        line.UpdatePosition(pointListCover);
                    }
                }

                // 更新覆盖图元
                IMFElement elePolygon = coverLayer.GetElement(elementName + "cover");
                if (elePolygon != null)
                {
                    IMFPolygon polygonEle = elePolygon as IMFPolygon;
                    if (polygonEle != null)
                    {
                        polygonEle.UpdatePosition(pointListCover);    // 更新覆盖图元位置
                    }
                }
            }

            // 更新波束图元
            IMFElement element = layer.GetElement(elementName);
            if (element == null) return;
            IMFPolygon polygonElement = element as IMFPolygon;
            if (polygonElement == null) return;
            polygonElement.UpdatePosition(pointListPolygon);    // 更新波束位置

            layer.Refresh();
        }

        /// <summary>
        /// 创建波束对象
        /// </summary>
        private Beam CreateBeam(BeamData beamData, Model3D model)
        {
            Beam beam = new Beam();
            beam.SatelliteID = beamData.SatelliteId;
            beam.BeamID = beamData.BeamId;
            beam.BeamLayerName = beamLayerName;
            beam.BeamName = string.Format("卫星{0}-波束{1}", beamData.SatelliteId, beamData.BeamId);
            beam.CenterPoint = beamData.Point;
            beam.Radius = beamRadius;
            beam.StepValue = stepValue;
            beam.FillColor = new RgbColorClass() { RGB = 261231 };
            return beam;
        }

        /// <summary>
        /// 创建模型对象
        /// </summary>
        private Model3D CreateSatelliteModel(string modelName, MapLngLat modelPoint, int azimuth = 0)
        {
            Model3D model = new Model3D();
            model.ModelName = modelName;
            //model.ModelType = ModelEnum.Satellite;
            model.Scale = satelliteScale;
            model.Azimuth = azimuth;
            model.LayerName = satelliteLayerName;
            model.ModelPath = Application.StartupPath + "\\ModelFile\\satellite.3DS";
            model.Coordinate = modelPoint;
            return model;
        }
        #endregion

        #region 态势数据展示

        // 处理实时数据
        public void DealRealData(RealData data)
        {
            var point = new MapLngLat(data.Longitude, data.Latitude, data.Altitude);
            Model3D model = CreateStateModel(data, point);

            if (!tsModelMgr.HasModel(model))  // 新的目标
            {
                if (AddModel2Earth(model))    // 创建目标
                {
                    tsModelMgr.AddModel(model);
                }
            }
            else   // 目标已存在
            {
                tsModelMgr.UpdataModel(model);
                UpdateModelPosition(model);          // 更新目标位置

                if (trackMgr.IsShowTrack(model.ModelName))
                    UpdateTrackPoint(model);         // 更新目标航迹
            }
        }

        /// <summary>
        /// 创建态势模型对象
        /// </summary>
        private Model3D CreateStateModel(RealData data, MapLngLat modelPoint, int azimuth = 0)
        {
            Model3D model = new Model3D();
            model.ModelName = data.TargetNum.ToString(); ;
            model.Scale = planeScale;
            model.Azimuth = data.ActionRange;
            model.Coordinate = modelPoint;

            string modelPath = "";

            switch (data.TargetType)
            {
                case 0:    // 空中目标
                    modelPath = Application.StartupPath + "\\ModelFile\\plane.3ds";
                    break;
                case 1:    // 陆地目标
                    modelPath = Application.StartupPath + "\\ModelFile\\car.3ds";
                    break;
                case 2:    // 海洋目标
                    modelPath = Application.StartupPath + "\\ModelFile\\ship.3ds";
                    break;
                case 3:    // 未知目标
                    modelPath = Application.StartupPath + "\\ModelFile\\plane.3ds";
                    break;
                default:
                    modelPath = Application.StartupPath + "\\ModelFile\\plane.3ds";
                    break;
            }

            model.LayerName = GetModelLayerName(data.TargetType);
            model.ModelPath = modelPath;

            return model;
        }

        // 更新目标航迹
        private void UpdateTrackPoint(Model3D model)
        {
            List<TrackPoint> trackPoints = trackMgr.GetTrackPoints(model.ModelName);
            if (trackPoints == null || trackPoints.Count <= 1) return;    // 如果没有点，或者最多只有一个点，则返回不进行航迹绘制

            var layer = mapLogic.AddLayer(model.LayerName);
            if (layer == null) return;

            string lineName = model.ModelName + "line";
            var line = layer.GetElement(lineName);

            if (line == null)
            {
                Kml kmlLine = new Kml();
                kmlLine.Placemark.Name = lineName;
                KmlLineString linekml = new KmlLineString();
                linekml.Color = Color.Red;
                linekml.Width = 1;
                List<MapLngLat> plist = new List<MapLngLat>();

                for (int i = 0; i < trackPoints.Count; i++)
                {
                    var point = new MapLngLat(trackPoints[i].Position.Lng, trackPoints[i].Position.Lat, trackPoints[i].Position.Alt);
                    plist.Add(point);
                }

                linekml.PositionList = plist;
                linekml.Rasterize = false;

                kmlLine.Placemark.Graph = linekml;
                var result = layer.AddElement(kmlLine);
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

        // 移除当前航迹
        private void RemoveTrackLineEx(Track track)
        {
            IMFLayer layer = mapLogic.GetLayer(GetModelLayerName(track.TargetType));
            if (layer == null) return;

            layer.RemoveElement(track.ElementName + "line");
            layer.Refresh();
        }

        // 移除所有航迹
        private void RemoveAllTrackLineEx(List<Track> tracks)
        {
            foreach (Track track in tracks)
            {
                IMFLayer layer = mapLogic.GetLayer(GetModelLayerName(track.TargetType));
                if (layer == null) continue;

                layer.RemoveElement(track.ElementName + "line");
            }
        }
        #endregion

        #region 预警

        // 处理预警数据
        public void DealWarnData(RealData data, bool isWarn, List<string> warnNames)
        {
            var layer = mapLogic.GetLayer(GetModelLayerName(data.TargetType));
            if (layer == null) return;
            var element = layer.GetElement(data.TargetNum.ToString());
            if (element == null) return;

            if (isWarn == false)   // 非预警数据，或停止预警
            {
                if (element.IsFlash)
                {
                    Utils.WarnCount--;
                }
                element.Flash(false);
            }
            else  // 预警数据
            {
                if (element.IsFlash == false)
                {
                    element.Flash(true, 500);
                    Utils.WarnCount++;

                    EventPublisher.PublishWarnDataEvent(this, new TSDataEventArgs() { Data = data, AreaName = warnNames[0] });
                }
            }

            PlaySound();
        }

        private bool isSoundPlay = false;

        /// <summary>
        /// 播放预警声音
        /// </summary>
        private void PlaySound()
        {
            if (Utils.bStartSound == false)
            {
                if (isSoundPlay)
                {
                    isSoundPlay = false;
                    soundPlayer.Stop();
                }
                return;
            }

            if (Utils.WarnCount > 0)
            {
                if (isSoundPlay == false)
                {
                    isSoundPlay = true;
                    soundPlayer.PlayLooping();
                }
            }
            else
            {
                if (isSoundPlay)
                {
                    isSoundPlay = false;
                    soundPlayer.Stop();
                }
            }
        }
        #endregion

        #region 设备覆盖
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
                Kml modelKml = new Kml();
                modelKml.Placemark.Name = device.DeviceNumber;

                KmlModel3d kml3d = new KmlModel3d();
                kml3d.ModelFilePath = Application.StartupPath + "\\ModelFile\\station.3DS";
                kml3d.Description = "";
                kml3d.Position = new MapLngLat(device.Lng, device.Lat, device.Alt);
                kml3d.Scale = 10000;
                kml3d.Azimuth = 0;
                modelKml.Placemark.Graph = kml3d;
                layer.AddElement(modelKml);

                // 绘制服务范围（面）
                List<MapLngLat> pointListCover = new List<MapLngLat>();

                for (double tempAngle = -180; tempAngle <= 180; tempAngle = tempAngle + 4)
                {
                    var p = Utils.GetPointByDistanceAndAngle((float)device.RangeRadius, new MapLngLat(device.Lng, device.Lat, device.Alt), tempAngle);
                    if (p != null)
                    {
                        pointListCover.Add(new MapLngLat(p.Lng, p.Lat, 100000));
                    }
                }

                Kml polygonCover = new Kml();
                polygonCover.Placemark.Name = device.DeviceNumber + "polygon";
                polygonCover.Placemark.Graph = new KmlPolygon() { Description = device.DeviceNumber + "polygon", PositionList = pointListCover, OutLineColor = Color.Red, FillColor = Color.FromArgb(20, Color.Blue), OutLineSize = 2 };
                layer.AddElement(polygonCover);
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
                I3DModel model = element as I3DModel;
                if (model != null)
                {
                    model.SetVisible(visible);
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

        public void AddLayerInit()
        {
            for (int i = 0; i < 4; i++)
            {
                mapLogic.AddLayer(GetModelLayerName((byte)i));
            }
        }

        /// <summary>
        /// 删除飞机图元
        /// </summary>
        /// <param name="planeName"></param>
        public void DeletePlane(string layerName, string planeName)
        {
            IMFLayer layer = mapLogic.GetLayer(layerName);
            if (layer == null) return;

            layer.RemoveElement(planeName);
            layer.RemoveElement(planeName + "line");

            tsModelMgr.DeleteModel(planeName);
        }

        /// <summary>
        /// 跳转到某目标
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="elementName">目标名称</param>
        public void JumpToPlane(byte targetType, string elementName)
        {
            if (mapLogic == null) return;
            IMFLayer layer = mapLogic.GetLayer(GetModelLayerName(targetType));
            if (layer == null) return;

            IMFElement element = layer.GetElement(elementName);
            if (element == null) return;

            I3DModel model = element as I3DModel;
            if (model == null) return;

            MapLngLat position = model.GetPosition();
            IMFToolBox toolBox = mapLogic.GetToolBox();
            if (toolBox == null) return;
            toolBox.ZoomToPosition(position);
        }

        // 设置卫星图层显示隐藏
        public void SetSatelliteLayerVisible(bool visible)
        {
            IMFLayer layer = mapLogic.GetLayer(satelliteLayerName);
            if (layer == null) return;

            layer.SetLayerVisible(visible);
        }

        // 设置波束图层显示隐藏
        public void SetBeamLayerVisible(bool visible)
        {
            IMFLayer layer = mapLogic.GetLayer(beamLayerName);
            if (layer == null) return;

            layer.SetLayerVisible(visible);
        }

        // 设置覆盖图层显示隐藏
        public void SetCoverLayerVisible(bool visible)
        {
            IMFLayer layer = mapLogic.GetLayer(coverLayerName);
            if (layer == null) return;

            layer.SetLayerVisible(visible);
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
                node = doc.SelectSingleNode("Globe/Config/PlaneModelScale");
                planeScale = Convert.ToInt32(node.InnerXml);
                node = doc.SelectSingleNode("Globe/Config/SallteModelScale");
                satelliteScale = Convert.ToInt32(node.InnerXml);
                node = doc.SelectSingleNode("Globe/Config/BeamAlpha");
                beamAlpha = Convert.ToInt32(node.InnerXml);
                node = doc.SelectSingleNode("Globe/Config/StepValue");
                stepValue = Convert.ToInt32(node.InnerXml);
                node = doc.SelectSingleNode("Globe/Config/BeamRadius");
                beamRadius = Convert.ToSingle(node.InnerXml);
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(ArcGlobeBusiness), ex.Message);
            }
        }

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
    }
}
