/**************************************************************************
 * 类名：Picture_ArcMap.cs
 * 描述：带图标点图元
 * 作者：Allen
 * 日期：Aug 30,2016
 * 
 * ************************************************************************/


using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System;
using System.Drawing;
using System.IO;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using System.Collections.Generic;
using ESRI.ArcGIS.SystemUI;
using MapFrame.Core.Interface;
using MapFrame.ArcMap.Factory;

namespace MapFrame.ArcMap.Element
{
    /// <summary>
    /// 带图标点图元
    /// </summary>
    class PointIco_ArcMap : MarkerElementClass, IMFPicture
    {
        /// <summary>
        /// 图元名称
        /// </summary>
        private string name;
        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHightLight = false;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 闪烁
        /// </summary>
        private bool isTimer = false;
        /// <summary>
        /// 记录Size
        /// </summary>
        private float bSize;
        /// <summary>
        /// 是否可见true显示,false隐藏
        /// </summary>
        private bool isVisible = true;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private System.Timers.Timer flashTimer = null;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 图片类型
        /// </summary>
        private esriIPictureType pictureType;
        /// <summary>
        /// 图片符号
        /// </summary>
        private PictureMarkerSymbolClass pictureMarkerSymbol;
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 地图工厂
        /// </summary>
        private FactoryArcMap mapFactory = null;
        private IMFLayer belongLayer = null;
        /// <summary>
        /// 地图标牌
        /// </summary>
        MapFrame.ArcMap.Windows.MapLabel mapLabel;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl"></param>
        /// <param name="pointKml"></param>
        public PointIco_ArcMap(AxMapControl _mapControl, MapFrame.Core.Model.KmlPoint pointKml, FactoryArcMap _mapFactory)
        {
            this.mapControl = _mapControl;
            this.mapFactory = _mapFactory;

            Dosomething((Action)(delegate
            {
                pictureMarkerSymbol = new PictureMarkerSymbolClass();
                SetPictureType(pointKml.IcoUrl);
                if (pointKml.Size == null)
                {
                    pictureMarkerSymbol.Size = 5;
                    bSize = 5;
                }
                else
                {
                    pictureMarkerSymbol.Size = pointKml.Size.Width;
                    bSize = pointKml.Size.Width;
                }
                IPoint point = new PointClass();
                point.PutCoords(pointKml.Position.Lng, pointKml.Position.Lat);
                base.Geometry = point;

                base.Symbol = pictureMarkerSymbol;
            }), true);

            this.Description = pointKml.Description;
            flashTimer = new System.Timers.Timer();
            flashTimer.Elapsed += new System.Timers.ElapsedEventHandler(flashTimer_Elapsed);
            isFlash = false;
        }

        /// <summary>
        /// 设置图片类型
        /// </summary>
        /// <param name="url"></param>
        private void SetPictureType(string url)
        {
            int index = url.LastIndexOf('.');
            int count = url.Length - 1 - index;
            string picType = url.Substring(index + 1, count).ToLower();

            switch (picType)
            {
                case "bmp":
                    pictureMarkerSymbol.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureBitmap, url);
                    pictureType = esriIPictureType.esriIPictureBitmap;
                    break;
                case "emf":
                    pictureMarkerSymbol.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureEMF, url);
                    pictureType = esriIPictureType.esriIPictureEMF;
                    break;
                case "gif":
                    pictureMarkerSymbol.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, url);
                    pictureType = esriIPictureType.esriIPictureGIF;
                    break;
                case "jpg":
                    pictureMarkerSymbol.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureJPG, url);
                    pictureType = esriIPictureType.esriIPictureJPG;
                    break;
                case "jpeg":
                    pictureMarkerSymbol.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureJPG, url);
                    pictureType = esriIPictureType.esriIPictureJPG;
                    break;
                case "png":
                    pictureMarkerSymbol.CreateMarkerSymbolFromFile(esriIPictureType.esriIPicturePNG, url);
                    pictureType = esriIPictureType.esriIPicturePNG;
                    break;
            }
        }

        #region MapFrame.Core.Interface.IPictureArcMap
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 图元所属图层
        /// </summary>
        public Core.Interface.IMFLayer BelongLayer
        {
            get { return belongLayer; }
            set { belongLayer = value; }
        }

        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return ""; }
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        public void UpdatePosition(Core.Model.MapLngLat lngLat)
        {
            IPoint point = new PointClass();
            point.PutCoords(lngLat.Lng, lngLat.Lat);
            base.Geometry = point;
            Update();
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="alt">纬度</param>
        public void UpdatePosition(double lng, double lat, double alt = 0)
        {
            IPoint point = new PointClass();
            point.PutCoords(lng, lat);
            base.Geometry = point;
            Update();
        }

        /// <summary>
        /// 不实现
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {

        }

        /// <summary>
        /// 不实现
        /// </summary>
        /// <param name="argb"></param>
        public void SetColor(int argb)
        {

        }

        /// <summary>
        /// 不实现
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetColor(int r, int g, int b)
        {

        }

        /// <summary>
        /// 不实现
        /// </summary>
        /// <param name="a"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetColor(int a, int r, int g, int b)
        {

        }

        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="bitmap"></param>
        public void SetIcon(string bitmap)
        {
            if (File.Exists(bitmap))
            {
                SetPictureType(bitmap);
                pictureMarkerSymbol.CreateMarkerSymbolFromFile(pictureType, bitmap);
                Update();
            }
        }

        /// <summary>
        /// 设置角度
        /// </summary>
        /// <param name="angle"></param>
        public void SetAngle(float angle)
        {
            pictureMarkerSymbol.Angle = angle;
            Update();
        }

        /// <summary>
        /// 设置比例大小
        /// </summary>
        /// <param name="scale"></param>
        public void SetScale(float scale)
        {
            pictureMarkerSymbol.Size = scale;
            bSize = scale;
            Update();
        }

        /// <summary>
        /// 获取当前位置
        /// </summary>
        /// <returns></returns>
        public Core.Model.MapLngLat GetLngLat()
        {
            IPoint point = base.Geometry as IPoint;
            Core.Model.MapLngLat lnglat = new Core.Model.MapLngLat(point.X, point.Y);
            return lnglat;
        }

        /// <summary>
        /// 设置Tip文字
        /// </summary>
        /// <param name="tipText"></param>
        public void SetTipText(string tipText)
        {

        }

        /// <summary>
        /// 设置Tip显示
        /// </summary>
        /// <param name="showType">显示方式</param>
        public void SetTipShow(Core.Model.ShowTypeEnum showType)
        {
            switch (showType)
            {
                case Core.Model.ShowTypeEnum.Always:
                    break;
                case Core.Model.ShowTypeEnum.MouseHover:
                    break;
                case Core.Model.ShowTypeEnum.No:
                    break;
            }
        }

        /// <summary>
        /// 设置标牌内容
        /// </summary>
        /// <param name="labelText">文字内容</param>
        public void SetLabelText(string labelText)
        {
            mapLabel.SetLabelText(labelText);
        }

        /// <summary>
        /// 设置标牌显示
        /// </summary>
        /// <param name="showType">显示方式</param>
        public void SetLableShow(Core.Model.ShowTypeEnum showType)
        {
            switch (showType)
            {
                case Core.Model.ShowTypeEnum.Always:
                    InitLable();
                    break;
                case Core.Model.ShowTypeEnum.MouseHover:
                    break;
                case Core.Model.ShowTypeEnum.No:
                    CloseLable();
                    break;
            }
        }

        /// <summary>
        /// 初始化标牌
        /// </summary>
        private void InitLable()
        {
            IPoint point = base.Geometry as IPoint;
            mapControl = BelongLayer.MapControl as AxMapControl;
            mapControl.Update();
            mapControl.OnMouseDown += mapControl_OnMouseDown;
            int x = -1;
            int y = -1;
            mapControl.FromMapPoint(point, ref x, ref y);
            System.Drawing.Point targetPoint = new System.Drawing.Point(x, y);
            mapLabel = new Windows.MapLabel(targetPoint);
            mapLabel.Move += new EventHandler(mapLabel_Move);
            mapLabel.Location = mapLabel.LabelLocation;
            IPoint labelLocation = mapControl.ToMapPoint(mapLabel.Location.X, mapLabel.Location.Y);

            Dosomething((Action)(delegate
            {
                mapControl.Controls.Add(mapLabel);
            }), true);

            //画线
            MapFrame.Core.Model.Kml kml = new MapFrame.Core.Model.Kml();
            kml.Placemark.Name = "标牌线";
            MapFrame.Core.Model.KmlLineString line = new MapFrame.Core.Model.KmlLineString();
            List<MapFrame.Core.Model.MapLngLat> pointList = new List<MapFrame.Core.Model.MapLngLat>();
            pointList.Add(new MapFrame.Core.Model.MapLngLat(point.X, point.Y));
            pointList.Add(new MapFrame.Core.Model.MapLngLat(labelLocation.X, labelLocation.Y));
            line.PositionList = pointList;

            line.Color = Color.Black;
            line.Width = 2;
            kml.Placemark.Graph = line;
            BelongLayer.AddElement(kml);
        }

        #region  地图移动

        private int eventInited = 0;
        private void mapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (eventInited == 1)
            {
                ICommand command = mapControl.CurrentTool as ICommand;
                if (command != null && command.Name == "ControlToolsMapNavigation_Pan")
                {
                    //更新标牌
                    IPoint targetPoint = base.Geometry as IPoint;
                    Core.Model.MapLngLat startPoint = new Core.Model.MapLngLat(targetPoint.X, targetPoint.Y);
                    int x = -1;
                    int y = -1;
                    mapControl.FromMapPoint(targetPoint, ref x, ref y);
                    mapLabel.UpdateLabelLocation(new System.Drawing.Point(x, y));

                    //更新线
                    IPoint labelPoint = mapControl.ToMapPoint(mapLabel.Location.X, mapLabel.Location.Y);
                    Core.Model.MapLngLat endPoint = new Core.Model.MapLngLat(labelPoint.X, labelPoint.Y);
                    MapFrame.Core.Interface.IMFElement element = BelongLayer.GetElement("标牌线");
                    if (element == null) return;
                    IMFLine lineElement = element as IMFLine;
                    List<Core.Model.MapLngLat> lnglatList = new List<Core.Model.MapLngLat>();
                    lnglatList.Add(startPoint);
                    lnglatList.Add(endPoint);
                    lineElement.UpdatePosition(lnglatList);
                }
            }
        }

        private void mapControl_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            eventInited = 0;
            mapControl.OnMouseMove -= mapControl_OnMouseMove;
            mapControl.OnMouseUp -= mapControl_OnMouseUp;
        }

        private void mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (eventInited == 0)
            {
                mapControl.OnMouseMove += mapControl_OnMouseMove;
                mapControl.OnMouseUp += mapControl_OnMouseUp;
                eventInited = 1;
            }
        }

        #endregion

        /// <summary>
        /// 标牌移动移动线的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapLabel_Move(object sender, EventArgs e)
        {
            IPoint targetPoint = base.Geometry as IPoint;
            Core.Model.MapLngLat startPoint = new Core.Model.MapLngLat(targetPoint.X, targetPoint.Y);//目标点

            MapFrame.ArcMap.Windows.MapLabel label = sender as MapFrame.ArcMap.Windows.MapLabel;
            IPoint labelLocation = mapControl.ToMapPoint(label.Location.X, label.Location.Y);
            Core.Model.MapLngLat endPoint = new Core.Model.MapLngLat(labelLocation.X, labelLocation.Y);//标牌点

            MapFrame.Core.Interface.IMFElement element = BelongLayer.GetElement("标牌线");
            if (element == null) return;
            IMFLine lineElement = element as IMFLine;
            List<Core.Model.MapLngLat> lnglatList = new List<Core.Model.MapLngLat>();
            lnglatList.Add(startPoint);
            lnglatList.Add(endPoint);
            lineElement.UpdatePosition(lnglatList);//更新位置
        }

        /// <summary>
        /// 释放标牌
        /// </summary>
        private void CloseLable()
        {
            if (mapLabel != null && !mapLabel.IsDisposed)
            {
                mapControl.OnMouseDown -= mapControl_OnMouseDown;
                mapControl.OnMouseMove -= mapControl_OnMouseMove;
                mapControl.OnMouseUp -= mapControl_OnMouseUp;
                mapLabel.Dispose();
                mapLabel = null;
            }
        }

        /// <summary>
        /// 图元描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 图元名称
        /// </summary>
        public string ElementName
        {
            get { return name; }
            set
            {
                name = value;
                base.Name = value;
            }
        }

        /// <summary>
        /// 图元类型
        /// </summary>
        public Core.Model.ElementTypeEnum ElementType
        {
            get;
            set;
        }

        /// <summary>
        /// 是否高亮
        /// </summary>
        public bool IsHightLight
        {
            get { return isHightLight; }
        }

        /// <summary>
        /// 是否闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return isFlash; }
        }

        /// <summary>
        /// 是否可见true显示,false隐藏
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight"></param>
        public void HightLight(bool isHightLight)
        {
            lock (lockObj)
            {
                if (isHightLight)
                {
                    pictureMarkerSymbol.Size = bSize + 5;
                }
                else
                {
                    pictureMarkerSymbol.Size = bSize;
                }
            }
            Update();
            this.isHightLight = isHightLight;
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="interval">闪烁用时</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (this.isFlash == isFlash) return;
            this.isFlash = isFlash;
            if (isFlash)
            {
                flashTimer.Interval = interval;
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                pictureMarkerSymbol.Size = bSize;
                Update();
            }
        }

        /// <summary>
        /// 事件间隔事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flashTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!isTimer)
            {
                Dosomething((Action)(delegate
                {
                    pictureMarkerSymbol.Size = bSize + 5;
                }), true);
            }
            else
            {
                Dosomething((Action)(delegate
                {
                    pictureMarkerSymbol.Size = bSize;
                }), true);
            }
            Update();
            isTimer = !isTimer;
        }

        /// <summary>
        /// 显示隐藏
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            if (this.isVisible == isVisible) return;
            ILayer layer = mapFactory.GetLayerByName(belongLayer.LayerName);
            CompositeGraphicsLayerClass graphLayer = layer as CompositeGraphicsLayerClass;
            this.isVisible = isVisible;
            if (isVisible)//显示
            {
                graphLayer.AddElement(this, 1);
            }
            else
            {
                graphLayer.DeleteElement(this);
            }
            Update();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            base.Symbol = pictureMarkerSymbol;
            if (this.BelongLayer != null)
            {
                this.BelongLayer.Refresh();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (flashTimer != null)
            {
                flashTimer.Stop();
                flashTimer.Dispose();
            }
            this.isVisible = true;
            this.isFlash = false;
            this.isHightLight = false;
            pictureMarkerSymbol = null;
            mapFactory = null;
            mapControl = null;
            this.CloseLable();
        }
        #endregion

        /// <summary>
        /// 主线程做事
        /// </summary>
        /// <param name="action">要做的内容</param>
        /// <param name="synchronization">是否同步执行</param>
        private void Dosomething(Action action, bool synchronization)
        {
            if (mapControl == null) return;
            if (synchronization)
            {
                if (mapControl.InvokeRequired)
                    mapControl.Invoke(action);
                else
                    action();
            }
            else
            {
                if (mapControl.InvokeRequired)
                    mapControl.BeginInvoke(action);
                else
                    action();
            }
        }

        public bool IsLableVisible
        {
            get { throw new NotImplementedException(); }
        }

        public Core.Model.ShowTypeEnum LabelShowType
        {
            get { throw new NotImplementedException(); }
        }

        public void SetSize(double size)
        {
            throw new NotImplementedException();
        }
    }
}
