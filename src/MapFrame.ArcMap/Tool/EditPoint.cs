/**************************************************************************
 * 类名：EditPoint.cs
 * 描述：编辑点
 * 作者：Allen
 * 日期：Aug 30,2016
 * 
 * ************************************************************************/

using System;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 编辑点
    /// </summary>
    class EditPoint : IMFTool, IMFDraw
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// arcgis地图控件
        /// </summary>
        private AxMapControl mapControl;
        /// <summary>
        /// 地图当前工具
        /// </summary>
        private ITool currTool = null;
        /// <summary>
        /// 被编辑的点图元
        /// </summary>
        private IMFPoint editElement = null;
        /// <summary>
        /// 编辑点所在的图层
        /// </summary>
        private Core.Interface.IMFLayer layer = null;
        /// <summary>
        /// 鼠标左键是否按下
        /// </summary>
        private bool isLeftBtnDown = false;
        /// <summary>
        /// 图层
        /// </summary>
        private IGraphicsContainer graphicsContainer;
        /// <summary>
        /// 地图逻辑
        /// </summary>
        private IMapLogic mapLogic = null;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">地图控件</param>
        /// <param name="element">图元</param>
        public EditPoint(AxMapControl _mapControl, MapFrame.Core.Interface.IMFElement element)
        {
            mapControl = _mapControl;
            currTool = _mapControl.CurrentTool;

            editElement = element as IMFPoint;
            if (editElement != null)
                layer = editElement.BelongLayer;
            SetGraphicsLayer();
        }

        /// <summary>
        /// 获取图层
        /// </summary>
        /// <returns></returns>
        private void SetGraphicsLayer()
        {
            int count = mapControl.Map.LayerCount;
            ILayer l = null;
            for (int i = 0; i < count; i++)
            {
                l = mapControl.Map.get_Layer(i);
                if (l.Name == layer.LayerName)
                    break;
            }
            graphicsContainer = l as IGraphicsContainer;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            if (editElement == null) return;    // 如果当前编辑点为NULL，则不执行任何操作
            RegistEvent();
        }

        /// <summary>
        /// 终止命令
        /// </summary>
        public void ReleaseCommond()
        {
            if (mapControl != null)
            {
                ICommand tool = new ControlsMapPanToolClass();
                tool.OnCreate(mapControl.Object);
                mapControl.CurrentTool = tool as ITool;

                LogoutEvent();
            }
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommonExcutedEvent()
        {
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs args = new MessageEventArgs()
                {
                    Describe = "编辑点，返回编辑点对象",
                    Data = editElement,
                    ToolType = ToolTypeEnum.Edit
                };
                CommondExecutedEvent.Invoke(this, args);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegistEvent()
        {
            mapControl.OnMouseDown += new IMapControlEvents2_Ax_OnMouseDownEventHandler(mapControl_OnMouseDown);
            mapControl.OnMouseMove += new IMapControlEvents2_Ax_OnMouseMoveEventHandler(mapControl_OnMouseMove);
            mapControl.OnMouseUp += new IMapControlEvents2_Ax_OnMouseUpEventHandler(mapControl_OnMouseUp);
            mapControl.OnDoubleClick += new IMapControlEvents2_Ax_OnDoubleClickEventHandler(mapControl_OnDoubleClick);
            mapControl.OnKeyDown += mapControl_OnKeyDown;
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.OnMouseDown -= new IMapControlEvents2_Ax_OnMouseDownEventHandler(mapControl_OnMouseDown);
            mapControl.OnDoubleClick -= new IMapControlEvents2_Ax_OnDoubleClickEventHandler(mapControl_OnDoubleClick);
            mapControl.OnKeyDown -= mapControl_OnKeyDown;
            mapControl.OnMouseUp -= mapControl_OnMouseUp;
            mapControl.OnMouseMove -= mapControl_OnMouseMove;
        }

        #region  事件

        // 鼠标按下事件
        private void mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button != 1) return;

            IPoint point = new PointClass();
            point.PutCoords(e.mapX, e.mapY);
            var elementenum = graphicsContainer.LocateElements(point, 0);
            if (elementenum != null)   // 判断是否点击图元
            {
                isLeftBtnDown = true;
                mapControl.CurrentTool = null;
                mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            }
        }

        // 鼠标移动事件
        private void mapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            // 鼠标指针
            IPoint point = new PointClass();
            point.PutCoords(e.mapX, e.mapY);
            var elementenum = graphicsContainer.LocateElements(point, 0);
            if (elementenum != null)
            {
                mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                mapControl.CurrentTool = null;
            }
            else
            {
                if (isLeftBtnDown == false)
                    mapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }

            if (isLeftBtnDown == false) return;
            Core.Model.MapLngLat lnglat = new Core.Model.MapLngLat(e.mapX, e.mapY);
            editElement.UpdatePosition(lnglat);   // 重新给位置
        }

        // 鼠标起来事件
        private void mapControl_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            isLeftBtnDown = false;
            mapControl.CurrentTool = currTool;
            mapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
        }

        // 鼠标双击事件
        private void mapControl_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            ReleaseCommond();
            RegistCommonExcutedEvent();
        }

        // 键盘按下事件
        private void mapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == (int)System.Windows.Forms.Keys.Escape)
            {
                ReleaseCommond();
            }
        }

        #endregion

        /// <summary>
        /// 释放该类
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            layer = null;
            editElement = null;
            currTool = null;
            mapControl = null;
        }

        /// <summary>
        /// 图层管理
        /// </summary>
        public IMapLogic MapLogic
        {
            set { mapLogic = value; }
        }

        /// <summary>
        /// 获取图元
        /// </summary>
        /// <returns></returns>
        public Core.Interface.IMFElement GetDrawElement()
        {
            return editElement;
        }
    }
}
