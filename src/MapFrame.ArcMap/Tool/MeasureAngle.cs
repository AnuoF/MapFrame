/**************************************************************************
 * 类名：MeasureAngle.cs
 * 描述：测量方位角工具
 * 作者：CJ
 * 日期：July 14,2016
 * 
 * ************************************************************************/

using System;
using System.Drawing;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.SystemUI;
using MapFrame.Core.Model;
using MapFrame.Core.Interface;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 测量方位角工具
    /// </summary>
    class MeasureAngle : IMFTool
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// 图层名称
        /// </summary>
        private string layerName = "measure_layer";
        /// <summary>
        /// 测量文字图元
        /// </summary>
        private TextElementClass textElement = null;
        /// <summary>
        /// 图层
        /// </summary>
        private ILayer layer = null;
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool isMouseDown = false;
        /// <summary>
        /// 方位线图元
        /// </summary>
        private LineElementClass lineElement = null;
        /// <summary>
        /// 第一个点的位置
        /// </summary>
        private IPoint firstPoint = null;
        /// <summary>
        /// 是否点击Control键
        /// </summary>
        private bool isControl = false;
        /// <summary>
        /// 是否完成
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        /// 测量角度
        /// </summary>
        private double angle = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl">地图控件</param>
        public MeasureAngle(AxMapControl _mapControl)
        {
            mapControl = _mapControl;
        }

        #region 命令
        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            layer = new CompositeGraphicsLayerClass();
            mapControl.CurrentTool = null;
            layer.Name = layerName;
            mapControl.AddLayer(layer);
            InitEvent();
        }

        /// <summary>
        /// 初始化事件
        /// </summary>
        private void InitEvent()
        {
            mapControl.OnKeyDown += mapControl_OnKeyDown;
            mapControl.OnMouseDown += mapControl_OnMouseDown;
            mapControl.OnKeyUp += mapControl_OnKeyUp;
            mapControl.OnMouseMove += mapControl_OnMouseMove;
            mapControl.OnMouseUp += mapControl_OnMouseUp;
        }
        
        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.OnKeyDown -= mapControl_OnKeyDown;
            mapControl.OnMouseDown -= mapControl_OnMouseDown;
            mapControl.OnKeyUp -= mapControl_OnKeyUp;
            mapControl.OnMouseMove -= mapControl_OnMouseMove;
        }

        /// <summary>
        /// 取消操作
        /// </summary>
        public void ReleaseCommond()
        {
            ICommand tool = new ControlsMapPanToolClass();
            tool.OnCreate(mapControl.Object);
            mapControl.CurrentTool = tool as ESRI.ArcGIS.SystemUI.ITool;

            LogoutEvent();
            if(layer!=null)
                mapControl.Map.DeleteLayer(layer);
            mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        /// <summary>
        /// 注册绘制完成事件
        /// </summary>
        private void RegistCommondExecutedEvent()
        {
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs args = new MessageEventArgs()
                {
                    Describe ="测量方位角",
                    ToolType = ToolTypeEnum.Measure,
                    Data = angle
                };
                CommondExecutedEvent("测量方位角", args);
            }
        }
        #endregion

        #region  事件

        /// <summary>
        /// 按下esc取消测量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == 27)
            {
                if (!isFinish)
                {
                    if (lineElement != null && textElement != null)
                    {
                        (layer as CompositeGraphicsLayerClass).DeleteElement(textElement);

                        (layer as CompositeGraphicsLayerClass).DeleteElement(lineElement);
                        mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, null, null);
                    }
                    isFinish = true;
                    isMouseDown = false;
                }
                else
                {
                    ReleaseCommond();
                }
            }
            else if (e.keyCode == 17)
            {
                isControl = true;
                ICommand command = new ControlsMapPanToolClass();
                command.OnCreate(mapControl.Object);
                if (command.Enabled)
                {
                    mapControl.CurrentTool = command as ITool;
                }
            }
        }

        /// <summary>
        /// 键盘弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnKeyUp(object sender, IMapControlEvents2_OnKeyUpEvent e)
        {
            if (e.keyCode == 17)
            {
                isControl = false;
                mapControl.CurrentTool = null;
            }
        }

        /// <summary>
        /// 鼠标左键弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            if (e.button == 1 && !isControl)
            {
                isFinish = true;
                isMouseDown = false;
                RegistCommondExecutedEvent();
                ReleaseCommond();//修改  陈静
            }
        }

        /// <summary>
        /// 鼠标按下生成方位线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            
            if (!isMouseDown && !isControl && e.button==1)
            {
                if (lineElement != null)
                {
                    (layer as CompositeGraphicsLayerClass).DeleteElement(textElement);
                    (layer as CompositeGraphicsLayerClass).DeleteElement(lineElement);
                    mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, null, null);
                }
                lineElement = new LineElementClass();
                ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
                IColor color = new RgbColorClass()
                {
                    Transparency = Color.Red.A,
                    Red = Color.Red.R,
                    Green = Color.Red.G,
                    Blue = Color.Red.B
                };
                lineElement.Symbol = pLineSymbol;

                IPolyline polyLine = new PolylineClass();
                IPointCollection pointCollection = polyLine as IPointCollection;
                IPoint p = new PointClass();
                p.PutCoords(e.mapX, e.mapY);
                firstPoint = p;
                pointCollection.AddPoint(p);
                pointCollection.AddPoint(p);
                lineElement.Geometry = pointCollection as IGeometry;

                (layer as CompositeGraphicsLayerClass).AddElement(lineElement, 0);
                isMouseDown = true;

                textElement = new TextElementClass();
                textElement.Size = 9;
                textElement.Text = "方位0°";
                textElement.Geometry = p;
                (layer as CompositeGraphicsLayerClass).AddElement(textElement, 0);

                isMouseDown = true;
                isFinish = false;
            }
        }

        /// <summary>
        /// 鼠标移动实时更新测量线和方位角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (isMouseDown && !isControl && e.button == 1)
            {
                IPointCollection pointCollection = lineElement.Geometry as IPointCollection;
                IPoint point = new PointClass();
                point.PutCoords(e.mapX, e.mapY);
                pointCollection.UpdatePoint(1, point);
                lineElement.Geometry = pointCollection as IGeometry;
                angle = GetAngle(e.mapX, e.mapY);
                textElement.Text = string.Format("角度{0}°", angle);
                mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
        }

        #endregion

        /// <summary>
        /// 获取角度
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns></returns>
        private double GetAngle(double lng, double lat)
        {
            MapFrame.Core.Model.MapLngLat point = new MapFrame.Core.Model.MapLngLat(lng, lat);

            //计算角度
            double zqz = Math.Abs((firstPoint.X - point.Lng)) / Math.Abs((firstPoint.Y - point.Lat));
            double zqzjd = Math.Atan(zqz);
            double jd = Math.Round(180 / Math.PI * zqzjd, 2);
            if ((firstPoint.X - point.Lng) < 0 && (firstPoint.Y - point.Lat) < 0)//第一象限
            {

            }
            else if ((firstPoint.X - point.Lng) < 0 && (firstPoint.Y - point.Lat) > 0) //第四象限
            {
                jd = 180 - jd;
            }
            else if ((firstPoint.X - point.Lng) > 0 && (firstPoint.Y - point.Lat) > 0) //第三象限
            {
                jd += 180;
            }
            else if ((firstPoint.X - point.Lng) > 0 && (firstPoint.Y - point.Lat) < 0) //第二象限
            {
                jd = 360 - jd;
            }
            return jd;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            layerName = string.Empty;
            textElement = null;
            layer = null;
            mapControl = null;
            lineElement = null;
            firstPoint = null;
            angle = 0;
            isFinish = false;
            isMouseDown = false;
            isControl = false;
        }
    }
}
