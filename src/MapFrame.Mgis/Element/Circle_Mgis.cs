/**************************************************************************
 * 类名：Circle_GMap.cs
 * 描述：GMap地图圆图元
 * 作者：lx
 * 日期：Nov 1,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using AxHOSOFTMapControlLib;
using MapFrame.Core.Model;
using System.Runtime.InteropServices;
using System.Timers;
using MapFrame.Core.Interface;

namespace MapFrame.Mgis.Element
{
    /// <summary>
    /// 圆图元
    /// </summary>
    class Circle_Mgis : IMFCircle
    {
        /// <summary>
        /// Mgis地图控件
        /// </summary>
        private AxHOSOFTMapControl mapControl;
        /// <summary>
        /// 坐标集合
        /// </summary>
        private List<MapLngLat> listPoint;
        /// <summary>
        /// 图元名称
        /// </summary>
        private string symbolName = string.Empty;
        /// <summary>
        /// 是否隐藏
        /// </summary>
        private bool isVisible = false;
        /// <summary>
        /// 是否高亮
        /// </summary>
        private bool isHight = false;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        private bool isFlash = false;
        /// <summary>
        /// 闪烁
        /// </summary>
        private bool isTimer;
        /// <summary>
        /// 闪烁计时器
        /// </summary>
        private Timer flashTimer = null;
        /// <summary>
        /// 记录轮廓宽度
        /// </summary>
        private float width = 0;
        /// <summary>
        /// 资源互斥锁
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// 记录填充色
        /// </summary>
        private System.Drawing.Color bFillColor;
        /// <summary>
        /// 记录轮廓色
        /// </summary>
        private System.Drawing.Color bOutLineColor;
        /// <summary>
        /// 图元所属图层
        /// </summary>
        private IMFLayer belongLayer = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <param name="_mapControl">地图控件</param>
        public Circle_Mgis(Kml kml, AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
            KmlCircle kmlCircle = kml.Placemark.Graph as KmlCircle;
            if (kmlCircle.Position == null || kmlCircle.RandomPosition == null || kml.Placemark.Name == string.Empty) return;
            listPoint = new List<MapLngLat>();
            listPoint.Add(kmlCircle.Position);
            listPoint.Add(kmlCircle.RandomPosition);

            float[] vertex = new float[4];
            IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * 4);

            vertex[0] = (float)listPoint[0].Lng;
            vertex[1] = (float)listPoint[0].Lat;
            vertex[2] = (float)listPoint[1].Lng;
            vertex[3] = (float)listPoint[1].Lat;
            Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
            this.symbolName = kml.Placemark.Name;

            mapControl.MgsDrawLineSymByJBID(symbolName, 16, (ulong)(ptrVert.ToInt64()), 2);

            mapControl.MgsUpdateSymFillColor(symbolName, kmlCircle.FillColor.R, kmlCircle.FillColor.G, kmlCircle.FillColor.B, kmlCircle.FillColor.A);
            mapControl.MgsUpdateSymColor(symbolName, kmlCircle.StrokeColor.R, kmlCircle.StrokeColor.G, kmlCircle.StrokeColor.B, kmlCircle.StrokeColor.A);
            mapControl.MgsUpdateSymLineWidth(symbolName, kmlCircle.StrokeWidth);//轮廓大小
            mapControl.update();//刷新

            Marshal.FreeHGlobal(ptrVert);

            this.ElementType = ElementTypeEnum.Circle;
            bFillColor = kmlCircle.FillColor;//填充颜色
            bOutLineColor = kmlCircle.StrokeColor;//轮廓颜色
            this.width = kmlCircle.StrokeWidth;//轮廓大小

            flashTimer = new Timer();
            flashTimer.Elapsed += new ElapsedEventHandler(flashTimer_Elapsed);
            flashTimer.Interval = 500;
        }

        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 获取圆心坐标
        /// </summary>
        /// <returns></returns>
        public Core.Model.MapLngLat GetCenterDot()
        {
            return listPoint[0];
        }

        /// <summary>
        /// 获取半径 （公里）
        /// </summary>
        /// <returns></returns>
        public double GetRadius()
        {
            if (listPoint.Count != 2) return 0;
            return MapFrame.Core.Common.Utils.GetDistance(listPoint[0], listPoint[1]);
        }

        /// <summary>
        /// 更新圆心坐标
        /// </summary>
        /// <param name="centerDot"></param>
        public void UpdatePosition(Core.Model.MapLngLat centerDot)
        {
            this.listPoint[0] = centerDot;
            DrawNewCircle(listPoint);
        }

        /// <summary>
        /// 更新半径
        /// </summary>
        /// <param name="radius"></param>
        public void UpdatePosition(double radius)
        {
            MapLngLat point = MapFrame.Core.Common.Utils.GetPointByDistanceAndAngle((float)radius, listPoint[0], 180);
            listPoint[1] = point;

            DrawNewCircle(listPoint);
        }

        /// <summary>
        /// 更新圆心坐标和半径
        /// </summary>
        /// <param name="centerDot"></param>
        /// <param name="radius"></param>
        public void UpdatePosition(Core.Model.MapLngLat centerDot, double radius)
        {
            listPoint[0] = centerDot;
            //listPoint[1] = GetPointByDistanceAndAngle(radius, centerDot);
            listPoint[1] = MapFrame.Core.Common.Utils.GetPointByDistanceAndAngle((float)radius, listPoint[0], 180);
            DrawNewCircle(listPoint);
        }

        /// <summary>
        /// 绘制更新的图元
        /// </summary>
        /// <param name="listPoint"></param>
        private void DrawNewCircle(List<MapLngLat> listPoint)
        {
            float[] vertex = new float[4];
            IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * 4);

            vertex[0] = (float)listPoint[0].Lng;
            vertex[1] = (float)listPoint[0].Lat;

            vertex[2] = (float)listPoint[1].Lng;
            vertex[3] = (float)listPoint[1].Lat;
            Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
            mapControl.MgsDelObject(symbolName);//先删除掉原始线图元
            //再添加一个新的线图元
            mapControl.MgsDrawLineSymByJBID(symbolName, 16, (ulong)(ptrVert.ToInt64()), 2);
            mapControl.MgsUpdateSymColor(symbolName, bOutLineColor.R, bOutLineColor.G, bOutLineColor.B, bOutLineColor.A);
            mapControl.MgsUpdateSymFillColor(symbolName, bFillColor.R, bFillColor.G, bFillColor.B, bFillColor.A);
            mapControl.MgsUpdateSymLineWidth(symbolName, this.width);
            Update();//刷新
            Marshal.FreeHGlobal(ptrVert);
        }

        /// <summary>
        /// 设置填充色
        /// </summary>
        /// <param name="fillColor"></param>
        public void SetFillColor(System.Drawing.Color fillColor)
        {
            bFillColor = fillColor;
            mapControl.MgsUpdateSymFillColor(symbolName, fillColor.R, fillColor.G, fillColor.B, fillColor.A);
        }

        /// <summary>
        /// 设置轮廓颜色和宽度
        /// </summary>
        /// <param name="color"></param>
        /// <param name="width"></param>
        public void SetStroke(System.Drawing.Color color, float width)
        {
            bOutLineColor = color;
            this.width = width;
            mapControl.MgsUpdateSymColor(symbolName, color.R, color.G, color.B, color.A);
            mapControl.MgsUpdateSymLineWidth(symbolName, width);
        }

        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return symbolName; }
        }

        /// <summary>
        /// 所属图层
        /// </summary>
        public Core.Interface.IMFLayer BelongLayer
        {
            get { return belongLayer; }
            set { belongLayer = value; }
        }

        /// <summary>
        /// 图元描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 图元名称
        /// </summary>
        public string ElementName
        {
            get { return symbolName; }
            set { symbolName = value; }
        }

        /// <summary>
        /// 图元类型
        /// </summary>
        public Core.Model.ElementTypeEnum ElementType
        {
            get;
            set;
        }

        /// <summary>
        /// 是否高亮
        /// </summary>
        public bool IsHightLight
        {
            get { return this.isHight; }
        }

        /// <summary>
        /// 是否闪烁
        /// </summary>
        public bool IsFlash
        {
            get { return this.isFlash; }
        }

        /// <summary>
        /// 获取是否隐藏
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight"></param>
        public void HightLight(bool isHightLight)
        {
            lock (lockObj)
            {
                if (isHightLight)
                {
                    mapControl.MgsUpdateSymLineWidth(symbolName, width + 3);
                }
                else
                {
                    mapControl.MgsUpdateSymLineWidth(symbolName, width);
                }
            }
            this.isHight = isHightLight;
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="interval">闪烁间隔</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (this.isFlash == isFlash) return;//防止被多次调用
            this.isFlash = isFlash;
            if (isFlash)
            {
                flashTimer.Interval = interval;
                flashTimer.Start();
            }
            else
            {
                flashTimer.Stop();
                mapControl.MgsUpdateSymVisibility(symbolName, 0);
            }
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flashTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isTimer)
            {
                if (mapControl.InvokeRequired)
                {
                    mapControl.Invoke(new System.Action(delegate
                    {
                        mapControl.MgsUpdateSymVisibility(symbolName, 1);
                    }));
                }
                else
                {
                    mapControl.MgsUpdateSymVisibility(symbolName, 1);
                }
            }
            else
            {
                if (mapControl.InvokeRequired)
                {
                    mapControl.Invoke(new System.Action(delegate
                    {
                        mapControl.MgsUpdateSymVisibility(symbolName, 0);
                    }));
                }
                else
                {
                    mapControl.MgsUpdateSymVisibility(symbolName, 0);
                }
            }
            Update();
            isTimer = !isTimer;
        }

        /// <summary>
        /// 显示与否
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            if (this.isVisible == isVisible) return;
            this.isVisible = isVisible;
            if (isVisible)
            {
                mapControl.MgsUpdateSymVisibility(symbolName, 0);
            }
            else
            {
                mapControl.MgsUpdateSymVisibility(symbolName, 1);
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            if (belongLayer != null)
                belongLayer.Refresh();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (flashTimer != null)
            {
                flashTimer.Stop();
                flashTimer.Dispose();
            }
            isFlash = false;
            isHight = false;
            isVisible = false;
            belongLayer = null;
            symbolName = null;
        }

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="_opacity">透明度(0-255)</param>
        public void SetOpacity(int _opacity)
        {
            mapControl.MgsUpdateSymFillColor(symbolName, bFillColor.R, bFillColor.G, bFillColor.B, (byte)_opacity);
            bFillColor = System.Drawing.Color.FromArgb(_opacity, bFillColor.R, bFillColor.G, bFillColor.B);//填充色
        }
    }
}
