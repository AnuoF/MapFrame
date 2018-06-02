/**************************************************************************
 * 类名：DrawPolygon.cs
 * 描述：绘制折线图元
 * 作者：lx
 * 日期：July 27,2016
 * 
 * ************************************************************************/

using System.Collections.Generic;
using MapFrame.Core.Model;
using System.Drawing;
using System.Windows.Forms;
using MapFrame.Core.Interface;
using GMap.NET.WindowsForms;
using MapFrame.GMap.Common;
using System;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 绘制折线
    /// </summary>
    class DrawLine : IMFTool, IMFDraw
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 线图元
        /// </summary>
        private IMFLine lineElement = null;
        /// <summary>
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        private List<MapLngLat> listMapPoints = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 添加图元是否成功
        /// </summary>
        private bool drawn = false;
        /// <summary>
        /// ctrl键是否按下
        /// </summary>
        private bool mouseDown = false;
        /// <summary>
        /// 记录第一次点击的时间
        /// </summary>
        private DateTime fistTimer;

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
        /// <param name="_gmapControl"></param>
        public DrawLine(GMapControl _gmapControl)
        {
            gmapControl = _gmapControl;
            listMapPoints = new List<MapLngLat>();
            fistTimer = DateTime.Now;
        }

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            //添加图层
            layer = mapLogic.AddLayer("draw_layer");
            gmapControl.CanDragMap = false;
            gmapControl.SetCursor(Cursors.Cross);
            Utils.bPublishEvent = false;
            gmapControl.MouseDoubleClick += gmapControl_MouseDoubleClick;
            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.KeyDown += gmapControl_KeyDown;
            gmapControl.KeyUp += gmapControl_KeyUp;
            Utils.bPublishEvent = false;
        }

        /// <summary>
        /// 键盘按下后弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == (Keys.LButton | Keys.ShiftKey))
            {
                mouseDown = false;
                gmapControl.CanDragMap = false;
            }
        }

        /// <summary>
        /// 按下esc取消画图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (lineElement != null && drawn)
                {
                    layer.RemoveElement(lineElement);
                    drawn = false;
                }
                else
                    ReleaseCommond();
                listMapPoints.Clear();
            }
            else if (e.KeyCode == (Keys.ShiftKey | Keys.LButton))
            {
                mouseDown = true;
                gmapControl.CanDragMap = true;
            }
        }

        /// <summary>
        /// 释放工具
        /// </summary>
        public void ReleaseCommond()
        {
            if (gmapControl != null)
            {
                gmapControl.CanDragMap = true;
                gmapControl.SetCursor(Cursors.Default);
                LogoutEvent();
            }
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommondExcuteEvent()
        {
            if (this.CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "手动绘制线，返回线对象",
                    Data = lineElement,
                    ToolType = ToolTypeEnum.Draw
                };
                this.CommondExecutedEvent(this, msg);
            }
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private void LogoutEvent()
        {
            gmapControl.MouseDown -= gmapControl_MouseDown;
            gmapControl.MouseMove -= gmapControl_MouseMove;
            gmapControl.MouseDoubleClick -= gmapControl_MouseDoubleClick;
            gmapControl.KeyDown -= gmapControl_KeyDown;
            gmapControl.KeyUp -= gmapControl_KeyUp;
            Utils.bPublishEvent = true;
        }

        /// <summary>
        /// 获取绘制的图元
        /// </summary>
        /// <returns>图元</returns>
        public IMFElement GetDrawElement()
        {
            return lineElement;
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDown(object sender, MouseEventArgs e)
        {

            TimeSpan span = DateTime.Now - fistTimer;
            if (span.TotalMilliseconds < 170) return;
            fistTimer = DateTime.Now;
            if (e.Clicks == 2 || e.Button != MouseButtons.Left || mouseDown) return;
            var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
            var maplngLat = new MapLngLat(lngLat.Lng, lngLat.Lat);
            if (listMapPoints.Count == 0)
            {
                //加线
                Kml kmlLine = new Kml();
                kmlLine.Placemark.Name = "draw_line" + Utils.ElementIndex;

                KmlLineString line = new KmlLineString();
                line.Color = Color.Green;
                line.Width = 2;
                List<MapLngLat> pList = new List<MapLngLat>();
                pList.Add(new MapLngLat(lngLat.Lng, lngLat.Lat));
                line.PositionList = pList;                          //string.Format("{0},{1}", lngLat.Lng, lngLat.Lat);
                kmlLine.Placemark.Graph = line;
                IMFElement element = null;
                drawn = layer.AddElement(kmlLine, out element);
                lineElement = element as IMFLine;
                listMapPoints.Add(maplngLat);
                gmapControl.MouseMove += gmapControl_MouseMove;
            }
            else
            {
                if (!listMapPoints.Contains(maplngLat))
                {
                    listMapPoints.Add(maplngLat);
                    lineElement.AddPoint(maplngLat);
                }
            }
        }

        /// <summary>
        /// 双击鼠标，停止绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lineElement != null)
            {
                lineElement.UpdatePosition(listMapPoints);// 更新
                layer.Refresh();   // 刷新
                gmapControl.MouseMove -= gmapControl_MouseMove;
                drawn = false;
                listMapPoints.Clear();
                RegistCommondExcuteEvent();
                ReleaseCommond();//修改  陈静
            }
        }

        /// <summary>
        /// 移动鼠标实时绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!drawn) return;
            var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
            var maplngLat = new MapLngLat(lngLat.Lng, lngLat.Lat);
            listMapPoints.Add(maplngLat);    // 添加点
            lineElement.UpdatePosition(listMapPoints);     // 更新
            listMapPoints.Remove(maplngLat);   // 移除点
            layer.Refresh();  // 刷新
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            gmapControl = null;
            lineElement = null;
            mapLogic = null;
            listMapPoints = null;
            layer = null;
            drawn = false;
            mouseDown = false;
        }
    }
}
