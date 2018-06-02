/**************************************************************************
 * 类名：MeasureArea.cs
 * 描述：测量面积
 * 作者：陈静
 * 日期：July 14,2016
 * 
 * ************************************************************************/

using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using GMap.NET;
using MapFrame.GMap.Model;
using System.Windows.Forms;
using MapFrame.Core.Model;
using MapFrame.Core.Interface;
using MapFrame.GMap.Common;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 测量面积
    /// </summary>
    class MeasureArea : IMFTool
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
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
        /// 点集合
        /// </summary>
        private List<PointLatLng> pointList = null;
        /// <summary>
        /// 多边形图元名称
        /// </summary>
        private string PolygonName = "measure_polygon";
        /// <summary>
        /// 图层
        /// </summary>
        private GMapOverlay gmapOverlay = null;
        /// <summary>
        /// 多边形图元
        /// </summary>
        private GMapPolygon gmapPolygon = null;
        /// <summary>
        /// 点图元
        /// </summary>
        private EditMarker marker = null;
        /// <summary>
        /// 图层名称
        /// </summary>
        private string layerName = "measure_layer";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl">gmap地图控件</param>
        public MeasureArea(GMapControl _gmapControl)
        {
            gmapControl = _gmapControl;
            pointList = new List<PointLatLng>();
        }

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            // 添加图层
            gmapOverlay = new GMapOverlay(layerName);
            gmapControl.Overlays.Add(gmapOverlay);
            pointList.Clear();
            InitEvent();
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
        /// 鼠标单击，开始测量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (isFinish == true)
                {
                    ReleaseCommond();
                    return;
                }

                string name = string.Format("point_{0}", pointIndex);
                var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);

                if (pointIndex == 0)   // 第一个点生成面
                {
                    //加点
                    marker = new EditMarker(lngLat);
                    gmapOverlay.Markers.Add(marker);
                    //多边形
                    pointList.Add(lngLat);
                    gmapPolygon = new GMapPolygon(pointList, PolygonName);
                    gmapOverlay.Polygons.Add(gmapPolygon);
                    gmapControl.UpdatePolygonLocalPosition(gmapPolygon);
                    pointIndex++;
                }
                else//面对象生成以后添加面的点
                {
                    pointIndex++;

                    marker = new EditMarker(lngLat);
                    gmapOverlay.Markers.Add(marker);
                    pointList.Add(lngLat);
                    gmapPolygon.Points.Add(lngLat);
                    gmapControl.UpdatePolygonLocalPosition(gmapPolygon);
                }
            }
        }

        /// <summary>
        /// 鼠标双击，完成测量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // 加点
                var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);

                //加点
                marker = new EditMarker(lngLat);
                gmapOverlay.Markers.Add(marker);
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTipText = string.Format("终点\n经度：{0}\n纬度：{1}\n", lngLat.Lng, lngLat.Lat);

                // 完成测量
                isFinish = true;
                if (CommondExecutedEvent != null)
                {
                    MessageEventArgs msg = new MessageEventArgs()
                    {
                        ToolType = ToolTypeEnum.Measure
                    };
                    CommondExecutedEvent(this, msg);
                }
            }
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        public void ReleaseCommond()
        {
            //isFinish = false;
            //pointList.Clear();
            //pointIndex = 0;
            if (gmapControl != null)
            {
                gmapControl.Cursor = Cursors.Default;
                gmapControl.Overlays.Remove(gmapOverlay);//删除图层
                gmapControl.MouseDown -= gmapControl_MouseDown;
                gmapControl.MouseDoubleClick -= gmapControl_MouseDoubleClick;
            }

            Utils.bPublishEvent = true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            pointList = null;
            PolygonName = string.Empty;
            gmapOverlay = null;
            gmapPolygon = null;
            marker = null;
            layerName = string.Empty;
        }
    }
}
