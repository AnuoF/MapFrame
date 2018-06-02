
using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;

namespace MapFrame.ArcMap.Tool
{
    public class SelectElements : IMFTool, IMFSelect
    {
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// 图元集合
        /// </summary>
        private IEnumElement elementEnums = null;
        /// <summary>
        /// 图元集合
        /// </summary>
        private List<IMFElement> listElements = null;
        /// <summary>
        /// 矩形几何
        /// </summary>
        private INewEnvelopeFeedback rectangleFeedback = null;
        /// <summary>
        /// 几何
        /// </summary>
        private IEnvelope envelope = null;
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_mapControl"></param>
        public SelectElements(AxMapControl _mapControl)
        {
            mapControl = _mapControl;
            mapControl.CurrentTool = null;
        }

        /// <summary>
        /// 开始操作
        /// </summary>
        public void RunCommond()
        {
            mapControl.OnMouseDown += mapControl_OnMouseDown;
            mapControl.OnMouseUp += mapControl_OnMouseUp;
            mapControl.OnKeyDown += mapControl_OnKeyDown;
        }

        #region 事件
        /// <summary>
        /// 按下ESC退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == 27) ReleaseCommond();
        }

        /// <summary>
        /// 鼠标移动绘制矩形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            IPoint point = new PointClass();
            point.PutCoords(e.mapX, e.mapY);
            rectangleFeedback.MoveTo(point);
        }

        /// <summary>
        /// 鼠标弹起结束绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            IPoint point = new PointClass() { X = e.mapX, Y = e.mapY };
            envelope = rectangleFeedback.Stop() as IEnvelope;

            int layerCount = mapControl.LayerCount;
            for (int i = 0; i < layerCount; i++)
            {
                ILayer layer = mapControl.get_Layer(i);
                CompositeGraphicsLayerClass comp = layer as CompositeGraphicsLayerClass;
                if (comp == null) continue;
                if (envelope.IsEmpty == true)
                {
                    elementEnums = comp.LocateElements(point, 0);
                }
                else
                {
                    elementEnums = comp.LocateElementsByEnvelope(envelope);
                }
                List<IElement> list = new List<IElement>();
                if (elementEnums == null) return;
                IElement el = null;

                do
                {
                    el = elementEnums.Next();
                    if (el != null)
                    {
                        var element = el as IMFElement;
                        listElements.Add(element);
                    }
                }
                while (el != null);
            }
            ReleaseCommond();
        }

        /// <summary>
        /// 鼠标按下开始绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            IPoint point = new PointClass() { X = e.mapX, Y = e.mapY };

            if (rectangleFeedback == null)
            {
                rectangleFeedback = new NewEnvelopeFeedbackClass();
                rectangleFeedback.Display = (mapControl.Map as IActiveView).ScreenDisplay;
                rectangleFeedback.Start(point);
                listElements = new List<IMFElement>();
                mapControl.OnMouseMove += mapControl_OnMouseMove;
            }
        }

        #endregion

        /// <summary>
        /// 结束操作
        /// </summary>
        public void ReleaseCommond()
        {
            mapControl.OnMouseDown -= mapControl_OnMouseDown;
            mapControl.OnMouseMove -= mapControl_OnMouseMove;
            mapControl.OnMouseUp -= mapControl_OnMouseUp;
            mapControl.OnKeyDown -= mapControl_OnKeyDown;

            if (CommondExecutedEvent != null)
            {
                MessageEventArgs message = new MessageEventArgs()
                {
                    Describe = "选择图元",
                    Data = listElements,
                    ToolType = ToolTypeEnum.Select
                };

                CommondExecutedEvent(this, message);
            }
            ICommand tool = new ControlsMapPanToolClass();
            tool.OnCreate(mapControl.Object);
            mapControl.CurrentTool = tool as ITool;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (elementEnums != null) elementEnums.Reset();
            envelope = null;
            rectangleFeedback = null;
            if (listElements != null)
            {
                listElements.Clear();
            }
            listElements = null;
        }

        /// <summary>
        /// 获取框选的图元集合
        /// </summary>
        /// <returns></returns>
        public List<IMFElement> GetSelectElements()
        {
            return listElements;
        }
    }
}
