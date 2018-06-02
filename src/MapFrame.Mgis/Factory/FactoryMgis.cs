

using MapFrame.Core.Interface;
using System;
using AxHOSOFTMapControlLib;
using MapFrame.Core.Model;
using System.Windows.Forms;
using MapFrame.Mgis.Common;
using System.IO;
using System.Drawing;

namespace MapFrame.Mgis.Factory
{
    class FactoryMgis : IMapFactory, IMFMap
    {
        /// <summary>
        /// 地图对象
        /// </summary>
        private AxHOSOFTMapControl axMapControl;
        /// <summary>
        /// 图层管理
        /// </summary>
        private LayerManager lyMgr = null;
        /// <summary>
        /// 点工厂
        /// </summary>
        private PointFactory pointFac = null;
        /// <summary>
        /// 图片工厂
        /// </summary>
        private PictureFactory pictureFac = null;
        /// <summary>
        /// 线工厂
        /// </summary>
        private LineFactory lineFac = null;
        /// <summary>
        /// 面工厂
        /// </summary>
        private PolygonFactory polygonFac = null;
        /// <summary>
        /// 圆工厂
        /// </summary>
        private CircleFactory circleFac = null;
        /// <summary>
        /// 文字工厂
        /// </summary>
        private TextFactory textFac = null;
        /// <summary>
        /// 刷新间隔timer
        /// </summary>
        private System.Timers.Timer refreshTimer = null;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// Refresh()方法是否可以被调用
        /// </summary>
        private bool isRefreshCall = true;
        /// <summary>
        /// 鼠标移动用的经纬度
        /// </summary>
        private MapLngLat moveLnglat = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapObject">地图对象</param>
        public FactoryMgis(object _mapObject)
        {
            moveLnglat = new MapLngLat();
            if (_mapObject != null)
            {
                this.axMapControl = _mapObject as AxHOSOFTMapControl;
            }
            else
            {
                InitMapControl();
            }

            axMapControl.eventInitFinish += new _DHOSOFTMapControlEvents_eventInitFinishEventHandler(axMapControl_eventInitFinish);
            axMapControl.eventKeyDown += new _DHOSOFTMapControlEvents_eventKeyDownEventHandler(axMapControl_eventKeyDown);
            axMapControl.eventKeyUp += new _DHOSOFTMapControlEvents_eventKeyUpEventHandler(axMapControl_eventKeyUp);
            axMapControl.eventLButtonDbClick += new _DHOSOFTMapControlEvents_eventLButtonDbClickEventHandler(axMapControl_eventLButtonDbClick);
            axMapControl.eventLButtonDown += new _DHOSOFTMapControlEvents_eventLButtonDownEventHandler(axMapControl_eventLButtonDown);
            axMapControl.eventLButtonUp += new _DHOSOFTMapControlEvents_eventLButtonUpEventHandler(axMapControl_eventLButtonUp);
            axMapControl.eventMouseMove += new _DHOSOFTMapControlEvents_eventMouseMoveEventHandler(axMapControl_eventMouseMove);
            axMapControl.eventRButtonDbClick += new _DHOSOFTMapControlEvents_eventRButtonDbClickEventHandler(axMapControl_eventRButtonDbClick);
            axMapControl.eventRButtonDown += new _DHOSOFTMapControlEvents_eventRButtonDownEventHandler(axMapControl_eventRButtonDown);
            axMapControl.eventRButtonUp += new _DHOSOFTMapControlEvents_eventRButtonUpEventHandler(axMapControl_eventRButtonUp);

            pointFac = new PointFactory(axMapControl);
            pictureFac = new PictureFactory(axMapControl);
            circleFac = new CircleFactory(axMapControl);
            lineFac = new LineFactory(axMapControl);
            polygonFac = new PolygonFactory(axMapControl);
            textFac = new TextFactory(axMapControl);
            lyMgr = new LayerManager(axMapControl);
            lyMgr.RefreshMapDelegate += Refresh;

            refreshTimer = new System.Timers.Timer();
            refreshTimer.Interval = 100;
            refreshTimer.Elapsed += refreshTimer_Elapsed;
        }

        private void refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (lockObj)
            {
                isRefreshCall = true;
                refreshTimer.Stop();
            }

            // 单位时间内，只执行一次，且是在单位时间结束时执行
            if (axMapControl.InvokeRequired)
            {
                axMapControl.Invoke(new Action(delegate
                {
                    axMapControl.update();
                }));
            }
            else
            {
                axMapControl.update();
            }
        }

        private void RefreshEx()
        {
            isRefreshCall = false;
            refreshTimer.Start();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            lock (lockObj)
            {
                if (isRefreshCall)
                {
                    RefreshEx();
                }
            }
        }


        #region 图层操作
        /// <summary>
        /// 移除图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public bool RemoveLayer(string layerName)
        {
            var ret = lyMgr.RemoverLayer(layerName);
            this.Refresh();
            return ret;
        }

        /// <summary>
        /// 移除所有图层
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllLayer()
        {
            var ret = lyMgr.RemoveAllLayer();
            this.Refresh();
            return ret;
        }

        /// <summary>
        /// 移除图层上的图元，不删除图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        public void ClearLayer(string layerName)
        {
            lyMgr.ClearLayer(layerName);
            this.Refresh();
        }

        /// <summary>
        /// 清除所有图层上的图元，不删除图层
        /// </summary>
        public void ClearLayer()
        {
            lyMgr.ClearLayer();
            this.Refresh();
        }

        /// <summary>
        /// 设置图层显示
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="visible">是否显示</param>
        public void ShowLayer(string layerName, bool visible)
        {
            lyMgr.SetLayerVisable(layerName, visible);
            this.Refresh();
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="layerName">图层名</param>
        /// <returns></returns>
        bool IMapFactory.AddLayer(string layerName)
        {
            return lyMgr.AddLayer(layerName);
        }

        /// <summary>
        /// 设置图层显示或者隐藏
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="visible"></param>
        public void SetLayerVisiable(string layerName, bool visible)
        {
            lyMgr.SetLayerVisable(layerName, visible);
        }

        /// <summary>
        /// 保留：刷新
        /// </summary>
        /// <param name="layer"></param>
        public void Refresh(IMFLayer layer)
        {
            axMapControl.update();
        }
        #endregion

        #region 图元操作
        /// <summary>
        /// 添加图元
        /// </summary>
        /// <param name="layerName">图元所在的图层名称</param>
        /// <param name="kml">kml对象</param>
        /// <returns></returns>
        public IMFElement AddElement(string layerName, Core.Model.Kml kml)
        {
            if (kml == null) return null;
            if (kml.Placemark == null) return null;
            if (kml.Placemark.Graph == null) return null;
            if (string.IsNullOrEmpty(kml.Placemark.Name)) return null;

            var layer = lyMgr.GetLayer(layerName);
            if (layer == 0) return null;

            IElementFactory elementFactory = null;
            Type type = kml.Placemark.Graph.GetType();

            // 点
            if (type == typeof(KmlPoint))
            {
                elementFactory = pointFac;
            }
            // 线
            else if (type == typeof(KmlLineString))
            {
                elementFactory = lineFac;
            }
            // 面
            else if (type == typeof(KmlPolygon))
            {
                elementFactory = polygonFac;
            }
            // 文字
            else if (type == typeof(KmlText))
            {
                elementFactory = textFac;
            }
            // 圆
            else if (type == typeof(KmlCircle))
            {
                elementFactory = circleFac;
            }
            else if (type == typeof(KmlPicture))
            {
                elementFactory = pictureFac;
            }

            if (elementFactory == null) return null;

            IMFElement element = elementFactory.CreateElement(kml, layerName);
            if (element != null)
            {
                element.ElementName = kml.Placemark.Name;
            }
            return element;
        }

        /// <summary>
        /// 移除图元
        /// </summary>
        /// <param name="layerName">图元所在的图层名称</param>
        /// <param name="element">图元对象</param>
        /// <returns></returns>
        public bool RemoveElement(string layerName, IMFElement element)
        {
            if (element == null) return true;
            var layer = lyMgr.GetLayer(layerName);
            if (layer == 0) return true;
            IElementFactory elementFactory = null;

            switch (element.ElementType)
            {
                case ElementTypeEnum.Point:
                    elementFactory = pointFac;
                    break;
                case ElementTypeEnum.Picture:
                    elementFactory = pictureFac;
                    break;
                case ElementTypeEnum.Line:
                    elementFactory = lineFac;
                    break;

                case ElementTypeEnum.Polygon:
                    elementFactory = polygonFac;
                    break;

                case ElementTypeEnum.Text:
                    elementFactory = textFac;
                    break;

                case ElementTypeEnum.Circle:
                    elementFactory = circleFac;
                    break;
            }

            if (elementFactory == null) return false;

            bool ret = elementFactory.RemoveElement(element, layerName);
            element.Dispose();
            return ret;
        }
        #endregion

        #region IMFMap
        public event EventHandler<Core.Model.MFMouseEventArgs> MouseClickEvent;

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseMoveEvent;

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseDownEvent;

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseUpEvent;

        public event EventHandler<Core.Model.MFMouseEventArgs> MouseDbClickEvent;

        public event EventHandler<Core.Model.MFElementEnterEventArgs> ElementEnterEvent;

        public event EventHandler<MFElementLeaveEventArgs> ElementLeaveEvent;

        public event EventHandler<Core.Model.MFElementClickEventArgs> ElementClickEvent;

        public event EventHandler<Core.Model.MFKeyEventArgs> KeyDownEvent;

        public event EventHandler<Core.Model.MFKeyEventArgs> KeyUpEvent;

        public event EventHandler<MFKeyPressEventArgs> KeyPressEvent;

        public event EventHandler<EventArgs> InitFinishEvent;

        public object MapControl
        {
            get
            {
                return axMapControl;
            }
        }

        /// <summary>
        /// 获取地图对象
        /// </summary>
        /// <returns></returns>
        public object GetMapControl()
        {
            return axMapControl;
        }

        /// <summary>
        /// 获取地图接口对象
        /// </summary>
        /// <returns></returns>
        public IMFMap GetIMFMap()
        {
            return this;
        }

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="mapFile">地图文件</param>
        public void LoadMap(string mapFile)
        {
            if (File.Exists(mapFile))
            {
                axMapControl.openMap(mapFile);
            }
        }

        /// <summary>
        /// 屏幕坐标转化为经纬度
        /// </summary>
        /// <param name="x">屏幕X</param>
        /// <param name="y">屏幕Y</param>
        /// <returns></returns>
        public MapLngLat FromLocalToLngLat(int x, int y)
        {
            float b = 0, l = 0;
            axMapControl.MgsAppXYtoBL(0, x, y, ref b, ref l);
            MapLngLat lnglat = new MapLngLat(b,l);
            return lnglat;
        }

        /// <summary>
        /// 经纬度转化为屏幕坐标
        /// </summary>
        /// <param name="position">经纬度</param>
        /// <returns></returns>
        public Point FromLngLatToLocal(MapLngLat position)
        {
            float x = 0, y = 0;
            axMapControl.MgsAppBLtoXY(0, (float)position.Lng, (float)position.Lat, ref x, ref y);
            Point point = new Point((int)x, (int)y);
            return point;
        }

        /// <summary>
        /// 地图快照
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Image Snapshot()
        {
            axMapControl.MgsSaveAsBmp("D:\\a.bmp", 0, 0);
            return null;
        }

        #endregion


        #region Private Function

        /// <summary>
        /// 初始化地图控件
        /// </summary>
        private void InitMapControl()
        {
            this.axMapControl = new AxHOSOFTMapControlLib.AxHOSOFTMapControl();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl)).BeginInit();

            this.axMapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axMapControl.Enabled = true;
            this.axMapControl.Location = new System.Drawing.Point(0, 25);
            this.axMapControl.Name = "axHOSOFTMapControl1";
            //this.axHOSOFTMapControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axHOSOFTMapControl1.OcxState")));
            this.axMapControl.Size = new System.Drawing.Size(638, 463);
            this.axMapControl.TabIndex = 1;

            ((System.ComponentModel.ISupportInitialize)(this.axMapControl)).EndInit();
        }

        /// <summary>
        /// 地图鼠标右键弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void axMapControl_eventRButtonUp(object sender, _DHOSOFTMapControlEvents_eventRButtonUpEvent e)
        {
            if (Utils.bPublishEvent == false) return;
            MapLngLat lnglat = new MapLngLat(e.dLong, e.dLat);
            MFMouseEventArgs args = new MFMouseEventArgs(lnglat, MouseButtons.Right, e.x, e.y);
            if (MouseUpEvent != null)
            {
                MouseUpEvent.Invoke(this, args);
            }
        }

        /// <summary>
        /// 地图鼠标右键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl_eventRButtonDown(object sender, _DHOSOFTMapControlEvents_eventRButtonDownEvent e)
        {
            if (Utils.bPublishEvent == false) return;
            MapLngLat lnglat = new MapLngLat(e.dLong, e.dLat);
            MFMouseEventArgs args = new MFMouseEventArgs(lnglat, MouseButtons.Right, e.x, e.y);
            if (MouseDownEvent != null)
            {
                MouseDownEvent.Invoke(this, args);
            }
        }

        //地图控件鼠标右键双击事件
        private void axMapControl_eventRButtonDbClick(object sender, _DHOSOFTMapControlEvents_eventRButtonDbClickEvent e)
        {
            if (Utils.bPublishEvent == false) return;
            MapLngLat lnglat = new MapLngLat(e.dLong, e.dLat);
            MFMouseEventArgs args = new MFMouseEventArgs(lnglat, MouseButtons.Right, e.x, e.y);
            if (MouseDbClickEvent != null)
            {
                MouseDbClickEvent.Invoke(this, args);
            }
        }

        //地图控件鼠标移动事件
        private void axMapControl_eventMouseMove(object sender, _DHOSOFTMapControlEvents_eventMouseMoveEvent e)
        {
            if (Utils.bPublishEvent == false) return;
            moveLnglat.Lng = e.dLong;
            moveLnglat.Lat = e.dLat;
            MFMouseEventArgs args = new MFMouseEventArgs(moveLnglat, MouseButtons.None, e.x, e.y);
            if (MouseMoveEvent != null)
            {
                MouseMoveEvent.Invoke(this, args);
            }
        }

        //地图控件鼠标左键弹起事件
        private void axMapControl_eventLButtonUp(object sender, _DHOSOFTMapControlEvents_eventLButtonUpEvent e)
        {
            if (Utils.bPublishEvent == false) return;
            MapLngLat lnglat = new MapLngLat(e.dLong, e.dLat);
            MFMouseEventArgs args = new MFMouseEventArgs(lnglat, MouseButtons.Left, e.x, e.y);
            if (MouseUpEvent != null)
            {
                MouseUpEvent.Invoke(this, args);
            }
        }

        //地图控件鼠标左键按下事件
        private void axMapControl_eventLButtonDown(object sender, _DHOSOFTMapControlEvents_eventLButtonDownEvent e)
        {
            if (Utils.bPublishEvent == false) return;
            MapLngLat lnglat = new MapLngLat(e.dLong, e.dLat);
            MFMouseEventArgs args = new MFMouseEventArgs(lnglat, MouseButtons.Left, e.x, e.y);
            if (MouseDownEvent != null)
            {
                MouseDownEvent.Invoke(this, args);
            }
        }

        //地图控件鼠标左键双击事件
        private void axMapControl_eventLButtonDbClick(object sender, _DHOSOFTMapControlEvents_eventLButtonDbClickEvent e)
        {
            if (Utils.bPublishEvent == false) return;
            MapLngLat lnglat = new MapLngLat(e.dLong, e.dLat);
            MFMouseEventArgs args = new MFMouseEventArgs(lnglat, MouseButtons.Left, e.x, e.y);
            if (MouseDbClickEvent != null)
            {
                MouseDownEvent.Invoke(this, args);
            }
        }

        //地图控件键盘弹起事件
        private void axMapControl_eventKeyUp(object sender, _DHOSOFTMapControlEvents_eventKeyUpEvent e)
        {
            if (Utils.bPublishEvent == false) return;
            MFKeyEventArgs args = new MFKeyEventArgs();
            args.KeyEventArgs = new KeyEventArgs((Keys)e.nChar);
            if (KeyUpEvent != null)
            {
                KeyUpEvent.Invoke(this, args);
            }
        }

        //地图控件键盘按下事件
        private void axMapControl_eventKeyDown(object sender, _DHOSOFTMapControlEvents_eventKeyDownEvent e)
        {
            if (Utils.bPublishEvent == false) return;
            MFKeyEventArgs args = new MFKeyEventArgs();
            args.KeyEventArgs = new KeyEventArgs((Keys)e.nChar);
            if (KeyDownEvent != null)
            {
                KeyDownEvent.Invoke(this, args);
            }
        }

        //地图控件加载完成事件
        private void axMapControl_eventInitFinish(object sender, _DHOSOFTMapControlEvents_eventInitFinishEvent e)
        {
            if (!Utils.bPublishEvent) return;
            MFMapInitFinishEventArgs args = new MFMapInitFinishEventArgs();
            if (InitFinishEvent != null)
            {
                InitFinishEvent.Invoke(this, args);
            }
        }


        #endregion


        public event EventHandler<MapZoomChangedEventArgs> MapZoomChangedEvent;


        public void SetRasterize(string layerName, bool bRasterize)
        {
            throw new NotImplementedException();
        }

        public void GetRasterize(string layerName)
        {
            throw new NotImplementedException();
        }


        public void SetTransparency(string layerName, short transparency)
        {
            throw new NotImplementedException();
        }

    }
}
