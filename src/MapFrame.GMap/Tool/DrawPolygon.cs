/**************************************************************************
 * 类名：DrawPolygon.cs
 * 描述：绘制多边形图元
 * 作者：lx
 * 日期：July 20,2016
 * 
 * ************************************************************************/

using System.Collections.Generic;
using MapFrame.Core.Interface;
using GMap.NET.WindowsForms;
using MapFrame.GMap.Common;
using MapFrame.Core.Model;
using System;
using System.Windows.Forms;
using System.Drawing;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 绘制多边形
    /// </summary>
    class DrawPolygon : IMFTool, IMFDraw
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
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// 多边形对象
        /// </summary>
        private IMFPolygon polygonElement = null;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        private List<MapLngLat> listMapPoints = null;
        /// <summary>
        /// 图层
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool isMouseDown = false;
        /// <summary>
        /// 图元是否添加成功
        /// </summary>
        private bool drawn = false;
        /// <summary>
        /// 记录第一次点击的时间
        /// </summary>
        private DateTime fistTimer;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl">地图控件</param>
        public DrawPolygon(GMapControl _gmapControl)
        {
            gmapControl = _gmapControl;
            listMapPoints = new List<MapLngLat>();
        }

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            //添加图层
            layer = mapLogic.AddLayer("draw_layer");
            gmapControl.SetCursor(Cursors.Cross);
            Utils.bPublishEvent = false;
            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.MouseDoubleClick += gmapControl_MouseDoubleClick;
            gmapControl.KeyDown += gmapControl_KeyDown;
            gmapControl.KeyUp += gmapControl_KeyUp;
        }

        /// <summary>
        /// 释放工具
        /// </summary>
        public void ReleaseCommond()
        {
            if (gmapControl != null)
            {
                gmapControl.SetCursor(Cursors.Default);
                gmapControl.MouseDown -= gmapControl_MouseDown;
                gmapControl.MouseMove -= gmapControl_MouseMove;
                gmapControl.MouseDoubleClick -= gmapControl_MouseDoubleClick;
                gmapControl.KeyDown -= gmapControl_KeyDown;
                gmapControl.KeyUp -= gmapControl_KeyUp;
                Utils.bPublishEvent = true;
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
                    Describe = "手动绘制多边形，返回多边形对象",
                    Data = polygonElement,
                    ToolType = ToolTypeEnum.Draw,
                };
                this.CommondExecutedEvent(this, msg);
            }
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
        /// <returns>返回图元</returns>
        public IMFElement GetDrawElement()
        {
            return polygonElement;
        }

        /// <summary>
        /// 画多边形按下esc取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (polygonElement != null && drawn)
                    layer.RemoveElement(polygonElement);
                else
                    ReleaseCommond();
                listMapPoints.Clear();
            }
            else if (e.KeyCode == (Keys.ShiftKey | Keys.LButton))
            {
                isMouseDown = true;
                gmapControl.CanDragMap = true;
            }
        }

        /// <summary>
        /// 按键弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == (Keys.LButton | Keys.ShiftKey))
            {
                isMouseDown = false;
                gmapControl.CanDragMap = false;
            }
        }

        /// <summary>
        /// 鼠标双击停止绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && polygonElement != null)
            {
                polygonElement.UpdatePosition(listMapPoints);//更新一次
                layer.Refresh();
                gmapControl.MouseMove -= gmapControl_MouseMove;
                drawn = false;
                listMapPoints.Clear();
                RegistCommondExcuteEvent();
                ReleaseCommond();//修改  陈静
            }
        }

        /// <summary>
        /// 鼠标移动，实时绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!drawn) return;
            var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
            var maplngLat = new MapLngLat(lngLat.Lng, lngLat.Lat);
            listMapPoints.Add(maplngLat);//添加点
            polygonElement.UpdatePosition(listMapPoints);    //更新
            listMapPoints.Remove(maplngLat);//移除点
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDown(object sender, MouseEventArgs e)
        {
            TimeSpan span = DateTime.Now - fistTimer;
            if (span.TotalMilliseconds < 170) return;
            fistTimer = DateTime.Now;
            if (e.Clicks == 2 || e.Button != MouseButtons.Left || isMouseDown) return;

            var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
            var maplanLat = new MapLngLat(lngLat.Lng, lngLat.Lat);
            if (listMapPoints.Count == 0)//第一个点  生成多边形
            {
                Kml polygonKml = new Kml();
                polygonKml.Placemark.Name = "draw_polygon" + Utils.ElementIndex;

                listMapPoints.Add(maplanLat);     //添加到集合中
                polygonKml.Placemark.Graph = new KmlPolygon()
                {
                    Description = "",
                    PositionList = listMapPoints,
                    FillColor = Color.FromArgb(155, Color.AliceBlue),
                    OutLineColor = Color.FromArgb(155, Color.MidnightBlue)
                };
                IMFElement element = null;
                drawn = layer.AddElement(polygonKml, out element);     // 添加多边形到地图中
                polygonElement = element as IMFPolygon;
                gmapControl.MouseMove += gmapControl_MouseMove;
            }
            else
            {
                if (!listMapPoints.Contains(maplanLat))
                {
                    listMapPoints.Add(maplanLat);
                    polygonElement.AddPoint(maplanLat);
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            gmapControl = null;
            mapLogic = null;
            polygonElement = null;
            listMapPoints = null;
            layer = null;
            isMouseDown = false;
            drawn = false;
        }
    }
}
