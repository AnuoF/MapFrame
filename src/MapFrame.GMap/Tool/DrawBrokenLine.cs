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

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 绘制折线
    /// </summary>
    class DrawBrokenLine : ITool, IDraw
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event System.EventHandler CommondExecutedEvent;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private GMapControl gmapControl = null;
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
        private string lineName = "draw_brokenline";
        /// <summary>
        /// 图层管理
        /// </summary>
        private ILayerCollection _layerCollection;
        /// <summary>
        /// 坐标点集合
        /// </summary>
        private List<MapLngLat> listMapPoints = null;
        /// <summary>
        /// 画图的图层名称
        /// </summary>
        private string layerName = "draw_layer";

        /// <summary>
        /// 图层管理
        /// </summary>
        public ILayerCollection layerCollection
        {
            get { return _layerCollection; }
            set { _layerCollection = value; }
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_gmapControl"></param>
        public DrawBrokenLine(GMapControl _gmapControl)
        {
            gmapControl = _gmapControl;
        }

        /// <summary>
        /// 执行工具命令
        /// </summary>
        public void RunCommond()
        {
            //添加图层
            if (layerCollection.GetLayer(layerName) == null)
                layerCollection.AddLayer(layerName);

            lineName += Utils.ElementIndex.ToString();//图元名称
            gmapControl.MouseMove += new MouseEventHandler(gmapControl_MouseMove);
            gmapControl.MouseDoubleClick += new MouseEventHandler(gmapControl_MouseDoubleClick);
            gmapControl.MouseDown += new MouseEventHandler(gmapControl_MouseDown);
            gmapControl.KeyDown += new KeyEventHandler(gmapControl_KeyDown);
        }

        /// <summary>
        /// 释放工具
        /// </summary>
        public void ReleaseCommond()
        {
            isFinish = false;
            pointIndex = 0;
            gmapControl.MouseDown -= new MouseEventHandler(gmapControl_MouseDown);
            gmapControl.MouseMove -= new MouseEventHandler(gmapControl_MouseMove);
            gmapControl.MouseDoubleClick -= new MouseEventHandler(gmapControl_MouseDoubleClick);
            
            gmapControl.KeyDown -= gmapControl_KeyDown;
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
                layerCollection.GetLayer(layerName).RemoveElement(lineName);
                ReleaseCommond();
            }
        }

        /// <summary>
        /// 获取绘制的图元
        /// </summary>
        /// <returns>图元</returns>
        public IElement GetDrawElement()
        {
            return layerCollection.GetLayer(layerName).GetElement(lineName);
        }

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Clicks == 2) return;
            if (e.Button == MouseButtons.Left)
            {
                if (isFinish == true)
                {
                    ReleaseCommond();
                    return;
                }

                var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
                var maplngLat = new MapLngLat(lngLat.Lng, lngLat.Lat);
                if (pointIndex == 0)//第一个点
                {
                    listMapPoints = new List<MapLngLat>();

                    //加线
                    Kml kmlLine = new Kml();
                    kmlLine.Placemark.Name = lineName;

                    KmlLineString line = new KmlLineString();
                    line.Argb = Color.Green.ToArgb();
                    line.Width = 2;
                    List<MapLngLat> pList = new List<MapLngLat>();
                    pList.Add( new MapLngLat(lngLat.Lng,lngLat.Lat));
                    line.PositionList = pList;                          //string.Format("{0},{1}", lngLat.Lng, lngLat.Lat);
                    kmlLine.Placemark.Graph = line;
                    layerCollection.GetLayer(layerName).AddElement(kmlLine);

                    listMapPoints.Add(maplngLat);
                    pointIndex++;
                }
                else
                {
                    pointIndex++;
                    if (!listMapPoints.Contains(maplngLat))
                        listMapPoints.Add(maplngLat);
                }
                isMouseDown = true;
            }
        }

        /// <summary>
        /// 双击鼠标，停止绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isFinish = true;
                (layerCollection.GetLayer(layerName).GetElement(lineName) as ILineGMap).UpdatePosition(listMapPoints);//更新

                if (CommondExecutedEvent != null)
                    CommondExecutedEvent(this, null);

                ReleaseCommond();
            }
        }

        /// <summary>
        /// 移动鼠标实时绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isMouseDown == false) return;
            if (isFinish) return;
            var lngLat = gmapControl.FromLocalToLatLng(e.X, e.Y);
            var maplngLat = new MapLngLat(lngLat.Lng, lngLat.Lat);
            listMapPoints.Add(maplngLat);//添加点
            (layerCollection.GetLayer(layerName).GetElement(lineName) as ILineGMap).UpdatePosition(listMapPoints);//更新
            listMapPoints.Remove(maplngLat);//移除点
        }


    }

}
