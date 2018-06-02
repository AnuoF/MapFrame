/**************************************************************************
 * 类名：DrawStraightLine.cs
 * 描述：绘制直线图元
 * 作者：lx
 * 日期：July 27,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Interface;
using GMap.NET.WindowsForms;
using MapFrame.GMap.Common;
using System.Windows.Forms;
using MapFrame.Core.Model;
using System.Drawing;
using System;
using System.Collections.Generic;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 绘制折线
    /// </summary>
    class DrawStraightLine : IMFTool, IMFDraw
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 线操作
        /// </summary>
        private GMapRoute gmapRoute = null;
        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool isMouseDown = false;
        /// <summary>
        /// 点的索引
        /// </summary>
        private int pointIndex = 0;
        /// <summary>
        /// 是否完成绘制
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        /// 图元名称
        /// </summary>
        private string lineName = "draw_straightline";
        /// <summary>
        /// 图层管理
        /// </summary>
        private IMapLogic mapLogic;
        /// <summary>
        /// 画图的图层名称
        /// </summary>
        private string layerName = "draw_layer";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl"></param>
        public DrawStraightLine(GMapControl _gmapControl)
        {
            gmapControl = _gmapControl;
        }

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            if (mapLogic.GetLayer(layerName) == null)
                mapLogic.AddLayer(layerName);

            lineName += Utils.ElementIndex.ToString();//图元名称
            gmapControl.MouseDown += gmapControl_MouseDown;
            gmapControl.MouseMove += gmapControl_MouseMove;
            gmapControl.KeyDown += new KeyEventHandler(gmapControl_KeyDown);
        }

        /// <summary>
        /// 释放工具
        /// </summary>
        public void ReleaseCommond()
        {
            isFinish = false;
            pointIndex = 0;
            gmapControl.MouseDown -= gmapControl_MouseDown;
            gmapControl.MouseMove -= gmapControl_MouseMove;
            gmapControl.KeyDown -= gmapControl_KeyDown;
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
        public IMFElement GetDrawElement()
        {
            return mapLogic.GetLayer(layerName).GetElement(lineName);
        }

        /// <summary>
        /// 画折线时按下esc取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                mapLogic.GetLayer(layerName).RemoveElement(lineName);
                ReleaseCommond();
            }
        }

        /// <summary>
        /// 鼠标移动，实时绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown == false) return;
            if (isFinish) return;
            var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
            gmapRoute.Points[1] = lngLat;
            gmapControl.UpdateRouteLocalPosition(gmapRoute);
            gmapControl.Refresh();
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;

                if (isFinish == true)
                {
                    ReleaseCommond();
                    return;
                }
                if (pointIndex == 0)//第一个点
                {
                    var startPoint = gmapControl.FromLocalToLatLng(e.X, e.Y);
                    Kml kmlLine = new Kml();
                    kmlLine.Placemark.Name = lineName;

                    KmlLineString line = new KmlLineString();
                    line.Argb = Color.Green.ToArgb();
                    line.Width = 2;

                    List<MapLngLat> pList = new List<MapLngLat>();
                    pList.Add(new MapLngLat(startPoint.Lng, startPoint.Lat));
                    pList.Add(new MapLngLat(startPoint.Lng, startPoint.Lat));

                    line.PositionList = pList;
                    kmlLine.Placemark.Graph = line;
                    mapLogic.GetLayer(layerName).AddElement(kmlLine);
                    gmapRoute = mapLogic.GetLayer(layerName).GetElement(lineName) as GMapRoute;
                    pointIndex++;
                }
                else
                {
                    // 释放资源
                    isFinish = true;
                    ReleaseCommond();

                    // 发布完成事件
                    if (CommondExecutedEvent != null)
                    {
                        MessageEventArgs msg = new MessageEventArgs()
                        {
                            ToolType = ToolTypeEnum.Draw
                        };
                        CommondExecutedEvent(lineName, msg);
                    }
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            gmapRoute = null;
            lineName = string.Empty;
            mapLogic = null;
            layerName = string.Empty;
        }
    }
}
