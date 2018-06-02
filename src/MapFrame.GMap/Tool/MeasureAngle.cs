/**************************************************************************
 * 类名：MeasureAngle.cs
 * 描述：测量方位角工具
 * 作者：CJ
 * 日期：July 14,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using GMap.NET.WindowsForms;
using MapFrame.Core.Model;
using MapFrame.GMap.Model;
using System.Windows.Forms;
using MapFrame.GMap.Element;
using System.Drawing;
using MapFrame.Core.Interface;
using MapFrame.GMap.Common;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 测量方位角工具
    /// </summary>
    class MeasureAngle : IMFTool
    {
        /// <summary>
        /// 测量的距离
        /// </summary>
        private double angle;
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent = null;
        /// <summary>
        /// 线的点集合
        /// </summary>
        private List<MapLngLat> pointList = null;
        /// <summary>
        /// GMap地图控件
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 地图图层
        /// </summary>
        private GMapOverlay mapOverlay = null;
        /// <summary>
        /// 编辑点
        /// </summary>
        private EditMarker marker = null;
        /// <summary>
        /// 线图元
        /// </summary>
        private Line_GMap lineRoute = null;
        /// <summary>
        /// 鼠标点击计数
        /// </summary>
        private int index;
        /// <summary>
        /// 刷新间隔timer
        /// </summary>
        private System.Timers.Timer refreshTimer = null;
        /// <summary>
        /// Refresh()方法是否可以被调用
        /// </summary>
        private bool isRefreshCall = true;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl">地图控件</param>
        public MeasureAngle(GMapControl _gmapControl)
        {
            gmapControl = _gmapControl;
            pointList = new List<MapLngLat>();
            pointList.Add(new MapLngLat(0, 0));
            pointList.Add(new MapLngLat(0, 0));

            refreshTimer = new System.Timers.Timer();
            refreshTimer.Interval = 100;
            refreshTimer.Elapsed += refreshTimer_Elapsed;
        }

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            mapOverlay = new GMapOverlay("measure_layer");
            gmapControl.Overlays.Add(mapOverlay);
            gmapControl.KeyDown += gmapControl_KeyDown;
            InitEvent();
        }

        /// <summary>
        /// 注册完成事件
        /// </summary>
        private void RegistCommandExcuteEvent()
        {

            if (CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "测量方位角，返回方位角",
                    Data = angle,
                    ToolType = ToolTypeEnum.Measure
                };
                CommondExecutedEvent(this, msg);
            }
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
        }

        /// <summary>
        /// 初始化事件
        /// </summary>
        private void InitEvent()
        {
            Utils.bPublishEvent = false;
            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.MouseDoubleClick += gmapControl_MouseDoubleClick;
        }

        /// <summary>
        /// 鼠标移动，实时更新测量线和方位角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseMove(object sender, MouseEventArgs e)
        {
            var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
            MapLngLat point = new MapLngLat(lngLat.Lng, lngLat.Lat);

            //计算角度
            double zqz = Math.Abs((pointList[0].Lng - point.Lng)) / Math.Abs((pointList[0].Lat - point.Lat));
            double zqzjd = Math.Atan(zqz);
            angle = Math.Round(180 / Math.PI * zqzjd, 2);
            if ((pointList[0].Lng - point.Lng) < 0 && (pointList[0].Lat - point.Lat) < 0)//第一象限
            {

            }
            else if ((pointList[0].Lng - point.Lng) < 0 && (pointList[0].Lat - point.Lat) > 0) //第四象限
            {
                angle = 180 - angle;
            }
            else if ((pointList[0].Lng - point.Lng) > 0 && (pointList[0].Lat - point.Lat) > 0) //第三象限
            {
                angle += 180;
            }
            else if ((pointList[0].Lng - point.Lng) > 0 && (pointList[0].Lat - point.Lat) < 0) //第二象限
            {
                angle = 360 - angle;
            }

            pointList[1] = point;
            lineRoute.Points[1] = lngLat;

            marker.ToolTipText = string.Format("起点\n经度：{0}\n纬度：{1}\n角度：{2}°", lngLat.Lng, lngLat.Lat, 0);
            string txt = marker.ToolTipText;
            string jd = txt.Split('\n')[txt.Split('\n').Length - 1];
            txt = txt.Replace(jd, string.Format("角度：{0}°", angle));
            //更新Tip信息
            marker.ToolTipText = txt;

            this.Refresh();   //刷新
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            // 地图框架不主动刷新，由调用者自行刷新
            //return;

            // 需要判断是否可以被调用
            lock (lockObj)
            {
                if (isRefreshCall)
                {
                    RefreshEx();
                }
            }
        }

        private void RefreshEx()
        {
            isRefreshCall = false;
            refreshTimer.Start();
        }

        private void refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (lockObj)
            {
                isRefreshCall = true;
                refreshTimer.Stop();
            }

            // 单位时间内，只执行一次，且是在单位时间结束时执行
            if (gmapControl.InvokeRequired)
            {
                gmapControl.Invoke(new Action(delegate
                {
                    gmapControl.Refresh();
                }));
            }
            else
            {
                gmapControl.Refresh();
            }
        }

        /// <summary>
        /// 鼠标双击，完成测量，注销事件，移除图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ReleaseCommond();
        }

        /// <summary>
        /// 鼠标按下，绘制第一个点，并注册鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
                if (index == 0)
                {
                    //添加点
                    MapLngLat point = new MapLngLat(lngLat.Lng, lngLat.Lat);
                    marker = new EditMarker(lngLat);
                    marker.Tag = "点";
                    marker.ToolTipText = string.Format("起点\n经度：{0}\n纬度：{1}\n角度：{2}°", lngLat.Lng, lngLat.Lat, 0);
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    mapOverlay.Markers.Add(marker);
                    //添加线
                    KmlLineString linekml = new KmlLineString();
                    linekml.Color = Color.Green;
                    linekml.Width = 3;
                    List<MapLngLat> pList = new List<MapLngLat>();
                    pList.Add(new MapLngLat(lngLat.Lng, lngLat.Lat));
                    pList.Add(new MapLngLat(lngLat.Lng, lngLat.Lat));
                    linekml.PositionList = pList;
                    lineRoute = new Line_GMap("measure_Angle", linekml);
                    mapOverlay.Routes.Add(lineRoute);
                    pointList[0] = point;
                    //注册移动事件
                    gmapControl.MouseMove += new System.Windows.Forms.MouseEventHandler(gmapControl_MouseMove);
                    index++;
                }
                else if (index == 1)
                {
                    index++;
                    gmapControl.MouseMove -= gmapControl_MouseMove;
                    RegistCommandExcuteEvent();
                    ReleaseCommond();//修改  陈静
                }
                else if (index == 2)
                {
                    index = 0;
                    mapOverlay.Markers.Remove(marker);
                    mapOverlay.Routes.Remove(lineRoute);
                }
            }
        }

        /// <summary>
        /// 取消操作
        /// </summary>
        public void ReleaseCommond()
        {
            if (gmapControl != null)
            {
                gmapControl.Cursor = Cursors.Default;
                gmapControl.CanDragMap = true;
                gmapControl.MouseMove -= gmapControl_MouseMove;
                gmapControl.MouseDown -= gmapControl_MouseDown;
                gmapControl.KeyDown -= gmapControl_KeyDown;
                gmapControl.MouseDoubleClick -= gmapControl_MouseDoubleClick;
                mapOverlay.Markers.Clear();
                mapOverlay.Routes.Clear();
                gmapControl.Overlays.Remove(mapOverlay);
                mapOverlay.Clear();
            }

            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
                refreshTimer = null;
            }

            Utils.bPublishEvent = true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            gmapControl = null;
            mapOverlay = null;
            marker = null;
            lineRoute = null;
        }
    }
}
