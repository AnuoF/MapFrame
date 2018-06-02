/**************************************************************************
 * 类名：TextFactory.cs
 * 描述：文字图元工厂
 * 作者：CJ
 * 日期：Aug 26,2016
 * 
 * ************************************************************************/

using System;
using MapFrame.Core.Interface;
using ESRI.ArcGIS.Controls;
using MapFrame.Core.Model;
using System.Drawing;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Analyst3D;

namespace MapFrame.ArcGlobe.Tool
{
    /// <summary>
    /// 手动画圆
    /// </summary>
    class DrawCircle : IMFTool, IMFDraw
    {
        /// <summary>
        /// 图元
        /// </summary>
        private IMFElement circleElement = null;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private AxGlobeControl mapControl = null;

        /// <summary>
        /// 圆的kml
        /// </summary>
        private KmlCircle circleKml = null;
        /// <summary>
        /// kml
        /// </summary>
        private Kml kml = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 添加图元是否成功
        /// </summary>
        private bool drawn = false;
        /// <summary>
        /// 图层管理
        /// </summary>
        public IMapLogic MapLogic
        {
            set { mapLogic = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件对象</param>
        public DrawCircle(AxGlobeControl _mapControl)
        {
            this.mapControl = _mapControl;
        }

        /// <summary>
        /// 获取绘制后的图元
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return circleElement;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            mapControl.CurrentTool = null;
            layer = mapLogic.AddLayer("draw_layer");
            mapControl.OnMouseDown += mapControl_OnMouseDown;
            mapControl.OnKeyDown += mapControl_OnKeyDown;
            mapControl.OnDoubleClick += mapControl_OnDoubleClick;
        }

        /// <summary>
        /// 鼠标双击完成绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnDoubleClick(object sender, IGlobeControlEvents_OnDoubleClickEvent e)
        {
            RegistCommondExecutedEvent();
            kml = null;
        }

        /// <summary>
        /// 键盘监听事件Esc取消绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnKeyDown(object sender, IGlobeControlEvents_OnKeyDownEvent e)
        {
            if (e.keyCode == (int)ConsoleKey.Escape)
            {
                ReleaseCommond();
                if (drawn) layer.RemoveElement(circleElement);
            }
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseDown(object sender, IGlobeControlEvents_OnMouseDownEvent e)
        {
            if (e.button != 1) return;
            MapLngLat lnglat = this.SceneToGeographyPoint(e.x, e.y);
            if (kml == null)
            {
                kml = new Kml();
                kml.Placemark.Name = "绘制圆" + Utils.Index;
                circleKml = new KmlCircle();
                circleKml.Description = "手动绘制的圆";
                circleKml.FillColor = Color.FromArgb(70, Color.Orange);
                circleKml.StrokeColor = Color.FromArgb(70, Color.Red);
                circleKml.StrokeWidth = 2;
                circleKml.Rasterize = true;
                circleKml.Position = lnglat;
            }
            else
            {
                circleKml.RandomPosition = lnglat;
                drawn = layer.AddElement(kml, out circleElement);
            }
        }


        /// <summary>
        /// 屏幕坐标转地理坐标
        /// </summary>
        /// <param name="x">屏幕X</param>
        /// <param name="y">屏幕Y</param>
        /// <returns></returns>
        public MapLngLat SceneToGeographyPoint(int x, int y)
        {
            double dLat, dLon, dAlt;
            IGlobeDisplay pGlobeDisplay = mapControl.GlobeDisplay;
            ISceneViewer pViewer = mapControl.GlobeDisplay.ActiveViewer;
            IGlobeViewUtil pGlobeViewUtil = mapControl.GlobeCamera as IGlobeViewUtil;

            pGlobeViewUtil.WindowToGeographic(pGlobeDisplay, pViewer, x, y, true, out dLon, out dLat, out dAlt);
            Core.Model.MapLngLat lnglat = new MapLngLat(dLon, dLat, dAlt);
            return lnglat;
        }

        /// <summary>
        /// 地理坐标转化为屏幕坐标
        /// </summary>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="alt">高度</param>
        /// <returns></returns>
        public Point GeographyToScenePoint(double lon, double lat, double alt = 0)
        {
            int x, y;
            IGlobeViewUtil globeViewUtil = mapControl.GlobeCamera as IGlobeViewUtil;
            IGlobeDisplay pGlobeDisplay = mapControl.GlobeDisplay;
            globeViewUtil.GeocentricToWindow(lon, lat, alt, out x, out y);
            Point point = new Point(x, y);
            return point;
        }

        /// <summary>
        /// 终止命令
        /// </summary>
        public void ReleaseCommond()
        {
            mapControl.Navigate = true;
            mapControl.OnMouseDown -= mapControl_OnMouseDown;
            mapControl.OnKeyDown -= mapControl_OnKeyDown;
            mapControl.OnDoubleClick -= mapControl_OnDoubleClick;
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommondExecutedEvent()
        {
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "手动绘制圆，返回数据为圆对象",
                    Data = circleElement,
                    ToolType = ToolTypeEnum.Draw
                };

                CommondExecutedEvent(this, msg);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            circleElement = null;
            mapControl = null;
            circleKml = null;
            kml = null;
            layer = null;
            CommondExecutedEvent = null;
            mapLogic = null;
            drawn = false;
        }

    }
}
