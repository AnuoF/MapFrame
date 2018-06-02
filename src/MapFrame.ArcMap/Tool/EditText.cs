/**************************************************************************
 * 类名：EditText.cs
 * 描述：编辑文字类
 * 作者：LX
 * 日期：2016年9月8日
 * 
 * ************************************************************************/

using System;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.ArcMap.Windows;
using MapFrame.ArcMap.Element;
using System.Drawing;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// 编辑文字类
    /// </summary>
    class EditText : IMFTool, IMFDraw
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
        /// 当前工具
        /// </summary>
        private ITool currTool = null;
        /// <summary>
        /// 文字输入控件
        /// </summary>
        private TextInput textCtr = null;
        /// <summary>
        /// 编辑图元
        /// </summary>
        private Text_ArcMap editElement = null;
        /// <summary>
        /// 图元所属图层
        /// </summary>
        private CompositeGraphicsLayerClass graphicsContainer = null;
        /// <summary>
        /// 是否左键
        /// </summary>
        private bool isLeftBtnDown = false;
        /// <summary>
        /// 是否移动
        /// </summary>
        private bool isMove = false;
        /// <summary>
        /// 地图控件
        /// </summary>
        private IMapLogic mapLogic = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_mapControl">arcgis地图控件</param>
        /// <param name="_element">要编辑的文字图元</param>
        public EditText(AxMapControl _mapControl, IMFElement _element)
        {
            mapControl = _mapControl;
            currTool = _mapControl.CurrentTool;

            editElement = _element as Text_ArcMap;
            if (editElement != null)
                graphicsContainer = GetLayerByName(editElement.BelongLayer.LayerName) as CompositeGraphicsLayerClass;
        }

        #region 命令
        /// <summary>
        /// 执行命令
        /// </summary>
        public void RunCommond()
        {
            if (editElement == null) return;
            RegistEvent();
        }

        /// <summary>
        /// 终止命令
        /// </summary>
        public void ReleaseCommond()
        {
            if (mapControl != null)
            {
                LogoutEvent();
                mapControl.CurrentTool = currTool;
                mapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }
            if (textCtr != null) textCtr.Dispose();
            textCtr = null;
            isMove = false;
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommondExcutedEvent()
        {
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs args = new MessageEventArgs()
                {
                    Describe = "编辑文字，返回文字对象",
                    Data = editElement,
                    ToolType = ToolTypeEnum.Edit
                };
                CommondExecutedEvent(this, args);
            }
        }

        #endregion

        #region 事件
        /// <summary>
        /// 鼠标双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            if (e.button != 1) return;
            isMove = false;
            IPoint point = new PointClass();
            point.PutCoords(e.mapX, e.mapY);
            var elementenum = graphicsContainer.LocateElements(point, 0);
            if (elementenum != null)
            {
                isLeftBtnDown = true;
                mapControl.CurrentTool = null;
                mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                textCtr = new MapFrame.ArcMap.Windows.TextInput();
                textCtr.SetText(editElement.Text);
                textCtr.SetFont(new System.Drawing.Font(editElement.FontName, (float)editElement.Size));

                //将arc的颜色转换为.net颜色
                System.Drawing.Color color = System.Drawing.ColorTranslator.FromOle(editElement.Color.RGB);
                textCtr.SetColor(color);
                textCtr.Location = new System.Drawing.Point(e.x, e.y);
                mapControl.CreateControl();
                mapControl.Controls.Add(textCtr);
                textCtr.InputFinished += InEditFinshen;
            }
        }

        /// <summary>
        /// 修改完成
        /// </summary>
        /// <param name="context"></param>
        /// <param name="font"></param>
        /// <param name="color"></param>
        /// <param name="esc"></param>
        private void InEditFinshen(string context, System.Drawing.Font font, Color color, bool esc)
        {
            if (!esc)
            {
                if (!string.IsNullOrEmpty(context))
                {
                    editElement.Text = context;

                    System.Drawing.Color rgbcolor = color;
                    editElement.Color = new RgbColorClass() { Red = rgbcolor.R, Green = rgbcolor.G, Blue = rgbcolor.B };
                    editElement.FontName = font.Name;
                    editElement.Size = font.Size;
                    mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                }
                textCtr.Dispose();
            }
            ReleaseCommond();
            RegistCommondExcutedEvent();
        }

        /// <summary>
        /// 键盘输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == 27)
            {
                ReleaseCommond();
            }
        }

        /// <summary>
        /// 鼠标弹起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            isLeftBtnDown = false;
            mapControl.CurrentTool = currTool;
            mapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (!isMove) return;
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

            editElement.Geometry = point;          // 重新给位置
            editElement.BelongLayer.Refresh();     // 刷新
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button != 1) return;
            IPoint point = new PointClass();
            point.PutCoords(e.mapX, e.mapY);
            var elementenum = graphicsContainer.LocateElements(point, 0);
            if (elementenum != null)
            {
                isLeftBtnDown = true;
                isMove = true;
                mapControl.CurrentTool = null;
                mapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            }
        }
        #endregion

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegistEvent()
        {
            mapControl.OnMouseDown += mapControl_OnMouseDown;
            mapControl.OnMouseUp += mapControl_OnMouseUp;
            mapControl.OnDoubleClick += mapControl_OnDoubleClick;
            mapControl.OnMouseMove += mapControl_OnMouseMove;
            mapControl.OnKeyDown += mapControl_OnKeyDown;
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.OnMouseDown -= mapControl_OnMouseDown;
            mapControl.OnDoubleClick -= mapControl_OnDoubleClick;
            mapControl.OnKeyDown -= mapControl_OnKeyDown;
            mapControl.OnMouseUp -= mapControl_OnMouseUp;
            mapControl.OnMouseMove -= mapControl_OnMouseMove;
        }

        /// <summary>
        /// 释放该类
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            currTool = null;
            mapControl = null;
            textCtr = null;
            editElement = null;
            editElement = null;
            graphicsContainer = null;
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

        /// <summary>
        /// 获取图层
        /// </summary>
        /// <param name="globe"></param>
        /// <param name="layerName">图层名</param>
        /// <returns></returns>
        private ILayer GetLayerByName(string layerName)
        {
            IEnumLayer pEnumLayer = mapControl.Map.get_Layers(null, true);
            if (pEnumLayer == null)
                return null;

            ILayer pLayer = null;
            while ((pLayer = pEnumLayer.Next()) != null)
            {
                ILayer retLayer = GetLayerByName(pLayer, layerName);
                if (retLayer != null) return retLayer;
            }

            return pLayer;
        }

        /// <summary>
        /// 获?取?图?层?
        /// </summary>
        /// <param name="pLayer">图?层?</param>
        /// <param name="layerName">图?层?名?</param>
        /// <returns></returns>
        private ILayer GetLayerByName(ILayer pLayer, string layerName)
        {
            if (pLayer.Name == layerName)
            {
                return pLayer;
            }
            else
            {
                if (pLayer is GroupLayer)
                {
                    ICompositeLayer pCompositeLayer = null;
                    pCompositeLayer = pLayer as ICompositeLayer;
                    for (int i = 0; i < pCompositeLayer.Count; i++)
                    {
                        ILayer tmpLayer = pCompositeLayer.get_Layer(i);
                        ILayer retLayer = GetLayerByName(tmpLayer, layerName);
                        if (retLayer != null) return retLayer;
                    }
                }
            }

            return null;
        }

    }
}
