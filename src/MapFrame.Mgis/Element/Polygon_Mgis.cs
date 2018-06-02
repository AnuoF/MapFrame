using System;
using System.Collections.Generic;
using AxHOSOFTMapControlLib;
using MapFrame.Core.Model;
using System.Timers;
using System.Drawing;
using System.Runtime.InteropServices;
using MapFrame.Core.Interface;

namespace MapFrame.Mgis.Element
{
    /// <summary>
    /// 多边形图元
    /// </summary>
    class Polygon_Mgis : IMFPolygon
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
        private string symbolName;
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
        /// 记录轮廓颜色
        /// </summary>
        private System.Drawing.Color bOutLineColor;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="kml">Kml对象</param>
        /// <param name="_mapControl">地图控件</param>
        public Polygon_Mgis(Kml kml, AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
            MapFrame.Core.Model.KmlPolygon kmlPolygon = kml.Placemark.Graph as MapFrame.Core.Model.KmlPolygon;
            if (kml.Placemark.Name == null || kmlPolygon.PositionList == null) return;
            this.symbolName = kml.Placemark.Name;
            this.listPoint = kmlPolygon.PositionList;

            int count = kmlPolygon.PositionList.Count;
            float[] vertex = new float[count * 2];
            IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * count * 2);
            for (int i = 0; i < count; i++)
            {
                vertex[2 * i] = (float)listPoint[i].Lng;
                vertex[2 * i + 1] = (float)listPoint[i].Lat;
            }
            Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
            mapControl.MgsDrawLineSymByJBID(symbolName, 11, (ulong)(ptrVert.ToInt64()), count);

            mapControl.MgsUpdateSymFillColor(symbolName, kmlPolygon.FillColor.R, kmlPolygon.FillColor.G, kmlPolygon.FillColor.B, kmlPolygon.FillColor.A);
            mapControl.MgsUpdateSymColor(symbolName, kmlPolygon.OutLineColor.R, kmlPolygon.OutLineColor.G, kmlPolygon.OutLineColor.B, kmlPolygon.OutLineColor.A);
            mapControl.update();//刷新
            Marshal.FreeHGlobal(ptrVert);

            this.ElementType = ElementTypeEnum.Polygon;
            this.bFillColor = kmlPolygon.FillColor;
            this.bOutLineColor = kmlPolygon.OutLineColor;
            this.width = kmlPolygon.OutLineSize;

            flashTimer = new Timer();
            flashTimer.Elapsed += new ElapsedEventHandler(flashTimer_Elapsed);
            flashTimer.Interval = 500;
        }


        /// <summary>
        /// 轮廓颜色RGB值
        /// </summary>
        public Color OutLineColor
        {
            get
            {
                //byte r = 0, g = 0, b = 0, a = 0;
                //mapControl.MgsGetSymColor(symbolName, ref r, ref g, ref b, ref a);
                //Color c = Color.FromArgb(a, r, g, b);
                return this.bOutLineColor;
            }
        }

        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 填充色RGB值
        /// </summary>
        public Color FillColor
        {
            get
            {
                //byte r = 0, g = 0, b = 0, a = 0;
                //mapControl.MgsGetSymFillColor(symbolName, ref r, ref g, ref b, ref a);
                //Color c = Color.FromArgb(a, r, g, b);
                return this.bFillColor;
            }
        }


        /// <summary>
        /// 设置轮廓色
        /// </summary>
        /// <param name="outlineColor"></param>
        /// <returns></returns>
        public bool SetOutLineColor(int outlineColor)
        {
            Color color = Color.FromArgb(outlineColor);
            int result = mapControl.MgsUpdateSymColor(symbolName, color.R, color.G, color.B, color.A);
            Update();
            bOutLineColor = color;
            return result == 1 ? true : false;
        }

        /// <summary>
        /// 设置轮廓色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool SetOutLineColor(System.Drawing.Color color)
        {
            int result = mapControl.MgsUpdateSymColor(symbolName, color.R, color.G, color.B, color.A);
            Update();
            bOutLineColor = color;
            return result == 1 ? true : false;
        }

        /// <summary>
        /// 设置填充色
        /// </summary>
        /// <param name="fillColor"></param>
        /// <returns></returns>
        public bool SetFillColor(int fillColor)
        {
            Color color = Color.FromArgb(fillColor);
            int result = mapControl.MgsUpdateSymFillColor(symbolName, color.R, color.G, color.B, color.A);
            Update();
            this.bFillColor = color;
            return result == 1 ? true : false;
        }

        /// <summary>
        /// 设置填充色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool SetFillColor(System.Drawing.Color color)
        {
            int result = mapControl.MgsUpdateSymFillColor(symbolName, color.R, color.G, color.B, color.A);
            Update();
            this.bFillColor = color;
            return result == 1 ? true : false;
        }

        /// <summary>
        /// 设置轮廓宽度
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool SetOutLineSize(float size)
        {
            int result = mapControl.MgsUpdateSymLineWidth(symbolName, size);
            this.width = size;
            Update();
            return result == 1 ? true : false;
        }

        /// <summary>
        /// 更新某个坐标点
        /// </summary>
        /// <param name="oldLngLat"></param>
        /// <param name="newLngLat"></param>
        /// <returns></returns>
        public bool UpdatePosition(Core.Model.MapLngLat oldLngLat, Core.Model.MapLngLat newLngLat)
        {
            var index = listPoint.FindIndex(o => o.Lng == oldLngLat.Lng && o.Lat == oldLngLat.Lat);
            if (index == -1) return false;
            listPoint[index] = newLngLat;

            return DrawNewPolygon(listPoint);
        }

        /// <summary>
        /// 更新坐标集合
        /// </summary>
        /// <param name="pList"></param>
        /// <returns></returns>
        public bool UpdatePosition(List<Core.Model.MapLngLat> pList)
        {
            if (pList.Count <= 2) return false;
            this.listPoint.Clear();
            foreach (var p in pList)
            {
                listPoint.Add(p);
            }
            return DrawNewPolygon(pList);
        }

        private bool DrawNewPolygon(List<MapLngLat> listPoints)
        {
            int count = listPoints.Count;
            if (count < 3) return false;//如果小于三个坐标点则返回
            float[] vertex = new float[count * 2];
            IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * count * 2);
            for (int i = 0; i < count; i++)
            {
                vertex[2 * i] = (float)listPoint[i].Lng;
                vertex[2 * i + 1] = (float)listPoint[i].Lat;
            }
            Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
            mapControl.MgsDelObject(symbolName);//先删除掉原始线图元
            int temp = mapControl.MgsDrawLineSymByJBID(symbolName, 11, (ulong)(ptrVert.ToInt64()), count);
            mapControl.MgsUpdateSymColor(symbolName, bOutLineColor.R, bOutLineColor.G, bOutLineColor.B, bOutLineColor.A);
            mapControl.MgsUpdateSymFillColor(symbolName, bFillColor.R, bFillColor.G, bFillColor.B, bFillColor.A);
            mapControl.MgsUpdateSymLineWidth(symbolName, width);
            Update();
            Marshal.FreeHGlobal(ptrVert);
            return temp == 0 ? true : false;
        }

        /// <summary>
        /// 获取坐标点集合
        /// </summary>
        /// <returns></returns>
        public List<Core.Model.MapLngLat> GetLngLat()
        {
            return this.listPoint;
        }

        /// <summary>
        /// 获取图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return this.symbolName; }
        }

        private IMFLayer belongLayer;
        /// <summary>
        /// 所属图层
        /// </summary>
        public Core.Interface.IMFLayer BelongLayer
        {
            get { return belongLayer; }
            set { belongLayer = value; }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 图元名
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
        /// 是否隐藏
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
            Update();
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash"></param>
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
        /// 显示隐藏
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
            Update();
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
        }

        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="lnglat"></param>
        public void AddPoint(MapLngLat lnglat)
        {
            var point = listPoint.Find(o => o.Lng == lnglat.Lng || o.Lat == lnglat.Lat);
            if (point == null)
            {
                listPoint.Add(lnglat);
                DrawNewPolygon(listPoint);
            }
        }

        /// <summary>
        /// 移除点
        /// </summary>
        /// <param name="lnglat"></param>
        public void RemovePoint(MapLngLat lnglat)
        {
            if (listPoint.Count <= 3) return;
            var point = listPoint.Find(o => o.Lng == lnglat.Lng || o.Lat == lnglat.Lat);
            if (point != null)
            {
                listPoint.Remove(point);
                DrawNewPolygon(listPoint);
            }
        }

        /// <summary>
        /// 设置透明度（0-255）
        /// </summary>
        /// <param name="_opacity"></param>
        public void SetOpacity(int _opacity)
        {
            mapControl.MgsUpdateSymFillColor(symbolName, bFillColor.R, bFillColor.G, bFillColor.B, (byte)_opacity);
            Update();
        }

        /// <summary>
        /// 求面积||没用
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            //int count = listPoint.Count;
            //if (count < 3) return 0;//如果小于三个坐标点则返回
            //float[] vertex = new float[count * 2];
            //IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * count * 2);
            //for (int i = 0; i < count; i++)
            //{
            //    vertex[2 * i] = (float)listPoint[i].Lng;
            //    vertex[2 * i + 1] = (float)listPoint[i].Lat;
            //}
            //Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
            //double area = mapControl.MgsCalculateArea(count, (ulong)ptrVert, 1);
            //Marshal.FreeHGlobal(ptrVert);
            //return area;
            return MapFrame.Core.Common.Utils.GetPolygonArea(listPoint);
        }
    }
}
