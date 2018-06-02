/**************************************************************************
 * 类名：MeasureDistance.cs
 * 描述：测量距离
 * 作者：Allen
 * 日期：July 14,2016
 * 
 * ************************************************************************/

using GMap.NET.WindowsForms;
using MapFrame.Core.Model;
using MapFrame.GMap.Element;
using MapFrame.GMap.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MapFrame.Core.Interface;
using MapFrame.GMap.Common;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 测量距离
    /// </summary>
    class MeasureDistance : IMFTool
    {
        /// <summary>
        /// 测量的距离
        /// </summary>
        private double distance;
        /// <summary>
        /// 执行明明完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// GMap地图控件
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 是否已完成测量
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        /// 点的索引
        /// </summary>
        private int pointIndex = 0;
        /// <summary>
        /// 图层
        /// </summary>
        private GMapOverlay mapOverlay;
        /// <summary>
        /// 线图元
        /// </summary>
        private Line_GMap lineRoute = null;
        /// <summary>
        /// 点图元
        /// </summary>
        private EditMarker marker = null;
        /// <summary>
        /// 
        /// </summary>
        private List<EditMarker> markerList = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl">地图控件</param>
        public MeasureDistance(GMapControl _gmapControl)
        {
            gmapControl = _gmapControl;
            markerList = new List<EditMarker>();
        }

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            gmapControl.CanDragMap = false;
            InitEvent();
        }

        /// <summary>
        /// 释放工具
        /// </summary>
        public void ReleaseCommond()
        {
            if (gmapControl != null)
            {
                Utils.bPublishEvent = false;
                gmapControl.Cursor = Cursors.Default;
                gmapControl.CanDragMap = true;
                gmapControl.MouseClick -= gmapControl_MouseClick;
                gmapControl.MouseDoubleClick -= gmapControl_MouseDoubleClick;
                gmapControl.KeyUp -= gmapControl_KeyUp;
                gmapControl.KeyDown -= gmapControl_KeyDown;
                mapOverlay.Markers.Clear();
                mapOverlay.Routes.Clear();
                gmapControl.Overlays.Remove(mapOverlay);//删除图层
            }

            if (lineRoute != null)
                lineRoute.Dispose();
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommondExcuteEvent()
        {
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "测量距离，返回距离",
                    Data = distance,
                    ToolType = ToolTypeEnum.Measure
                };
                CommondExecutedEvent(this, msg);
            }
        }

        /// <summary>
        /// 初始化事件
        /// </summary>
        private void InitEvent()
        {
            //添加图层
            mapOverlay = new GMapOverlay("measure_layer");
            gmapControl.Overlays.Add(mapOverlay);
            gmapControl.MouseClick += gmapControl_MouseClick;
            gmapControl.MouseDoubleClick += gmapControl_MouseDoubleClick;
            gmapControl.KeyDown += gmapControl_KeyDown;
            gmapControl.KeyUp += gmapControl_KeyUp;
            Utils.bPublishEvent = true;
        }


        /// <summary>
        /// 按下esc取消测量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ReleaseCommond();
            }
            else if (e.KeyCode == (Keys.LButton | Keys.ShiftKey))
            {
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
                gmapControl.CanDragMap = false;
            }
        }

        /// <summary>
        /// 鼠标单击，开始测量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && gmapControl.CanDragMap == false)
            {
                if (isFinish == true)
                {
                    foreach (var item in markerList)
                    {
                        mapOverlay.Markers.Remove(item);
                    }

                    mapOverlay.Routes.Remove(lineRoute);
                    isFinish = false;
                    //ReleaseCommond();
                    //return;
                }

                string name = string.Format("point_{0}", pointIndex);
                var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);

                if (pointIndex == 0)   // 第一个点
                {
                    // 加点
                    marker = new EditMarker(lngLat);
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    marker.ToolTipText = string.Format("起点\n经度：{0}\n纬度：{1}\n", Math.Round(lngLat.Lng, 6), Math.Round(lngLat.Lat, 6));
                    mapOverlay.Markers.Add(marker);
                    marker.ToolTip.Format.Alignment = StringAlignment.Near;

                    // 加线
                    KmlLineString linekml = new KmlLineString();
                    linekml.Color = Color.Green;
                    linekml.Width = 3;
                    List<MapLngLat> pList = new List<MapLngLat>();
                    pList.Add(new MapLngLat(lngLat.Lng, lngLat.Lat));
                    linekml.PositionList = pList;
                    lineRoute = new Line_GMap("measure_line", linekml);
                    mapOverlay.Routes.Add(lineRoute);

                    pointIndex++;
                }
                else
                {
                    pointIndex++;

                    // 添加点
                    lineRoute.Points.Add(lngLat);
                    lineRoute.AddPoint(new MapLngLat(lngLat.Lng, lngLat.Lat));
                    gmapControl.Refresh();

                    // 添加Marker
                    marker = new EditMarker(lngLat);
                    mapOverlay.Markers.Add(marker);
                    distance = lineRoute.Distance;
                    distance = Math.Round(distance, 3);
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    marker.ToolTipText = string.Format("点{0}\n经度：{1}\n纬度：{2}\n距离：{3}（公里）", pointIndex, Math.Round(lngLat.Lng, 6), Math.Round(lngLat.Lat, 6), distance);
                    marker.ToolTip.Format.Alignment = StringAlignment.Near;
                }
                markerList.Add(marker);
            }
        }

        /// <summary>
        /// 鼠标双击，完成测量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
                marker = new EditMarker(lngLat);
                mapOverlay.Markers.Add(marker);
                double distance = lineRoute.Distance;
                distance = Math.Round(distance, 3);
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTipText = string.Format("终点\n经度：{0}\n纬度：{1}\n距离：{2}（公里）", lngLat.Lng, lngLat.Lat, distance);

                markerList.Add(marker);
                // 完成测量
                isFinish = true;
                pointIndex = 0;
                RegistCommondExcuteEvent();
                ReleaseCommond();//修改  陈静
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
            mapOverlay = null;
            lineRoute = null;
            marker = null;
            markerList = null;
        }
    }
}
