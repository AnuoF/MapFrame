
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using MapFrame.Core.Model;
using AxHOSOFTMapControlLib;
using System.Drawing;
using System.Runtime.InteropServices;
using MapFrame.Core.Interface;

namespace MapFrame.Mgis.Element
{
    class Line_Mgis : IMFLine
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
        /// 记录轮廓颜色
        /// </summary>
        private System.Drawing.Color bOutLineColor;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="kml">kml对象</param>
        /// <param name="_mapControl">地图控件</param>
        public Line_Mgis(Kml kml, AxHOSOFTMapControl _mapControl)
        {
            this.mapControl = _mapControl;
            KmlLineString kmlLine = kml.Placemark.Graph as KmlLineString;
            if (kml.Placemark.Name == null || kmlLine.PositionList == null) return;
            this.symbolName = kml.Placemark.Name;
            this.listPoint = kmlLine.PositionList;
            int count = kmlLine.PositionList.Count();
            float[] vertex = new float[count * 2];
            IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * count * 2);
            for (int i = 0; i < count; i++)
            {
                vertex[2 * i] = (float)kmlLine.PositionList[i].Lng;
                vertex[2 * i + 1] = (float)kmlLine.PositionList[i].Lat;
            }
            Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
            mapControl.MgsDrawLineSymByJBID(symbolName, 10, (ulong)(ptrVert.ToInt64()), count);
            Marshal.FreeHGlobal(ptrVert);
            mapControl.MgsUpdateSymColor(symbolName, kmlLine.Color.R, kmlLine.Color.G, kmlLine.Color.B, kmlLine.Color.A);
            mapControl.MgsUpdateSymLineWidth(symbolName, kmlLine.Width);
            Update();//刷新
            this.ElementType = ElementTypeEnum.Line;

            this.width = kmlLine.Width;
            this.bOutLineColor = kmlLine.Color;

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
        /// 设置线的宽度
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool SetWidth(float width)
        {
            this.width = width;
            int result = mapControl.MgsUpdateSymLineWidth(symbolName, width);
            Update();
            return result == 1 ? true : false;
        }

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(System.Drawing.Color color)
        {
            mapControl.MgsUpdateSymColor(symbolName, color.R, color.G, color.B, color.A);
            Update();
            bOutLineColor = color;
        }

        /// <summary>
        /// 设置线的颜色
        /// </summary>
        /// <param name="argb"></param>
        public void SetColor(int argb)
        {
            Color color = Color.FromArgb(argb);
            mapControl.MgsUpdateSymColor(symbolName, color.R, color.G, color.B, color.A);
            Update();
            this.bOutLineColor = color;
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetColor(int r, int g, int b)
        {
            mapControl.MgsUpdateSymColor(symbolName, (byte)r, (byte)g, (byte)b, 255);
            Update();
            this.bOutLineColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="a"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void SetColor(int a, int r, int g, int b)
        {
            mapControl.MgsUpdateSymColor(symbolName, (byte)r, (byte)g, (byte)b, (byte)a);
            this.bOutLineColor = Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// 更新坐标点集合
        /// </summary>
        /// <param name="pList"></param>
        /// <returns></returns>
        public bool UpdatePosition(List<Core.Model.MapLngLat> pList)
        {
            //listPoint.Clear();
            //listPoint.AddRange(pList);
            listPoint = pList;
            return DrawNewLine(listPoint);
        }

        /// <summary>
        /// 绘制新的线图元
        /// </summary>
        /// <param name="listPoints"></param>
        /// <returns></returns>
        private bool DrawNewLine(List<MapLngLat> listPoints)
        {
            int count = listPoint.Count();
            if (count < 2) return false;
            float[] vertex = new float[count * 2];
            IntPtr ptrVert = Marshal.AllocHGlobal(sizeof(float) * count * 2);
            for (int i = 0; i < count; i++)
            {
                vertex[2 * i] = (float)listPoint[i].Lng;
                vertex[2 * i + 1] = (float)listPoint[i].Lat;
            }
            Marshal.Copy(vertex, 0, ptrVert, vertex.Length);
            mapControl.MgsDelObject(symbolName);//先删除掉原始线图元
            int temp = mapControl.MgsDrawLineSymByJBID(symbolName, 10, (ulong)(ptrVert.ToInt64()), count);
            mapControl.MgsUpdateSymColor(symbolName, bOutLineColor.R, bOutLineColor.G, bOutLineColor.B, bOutLineColor.A);
            mapControl.MgsUpdateSymLineWidth(symbolName, this.width);
            mapControl.update();
            //再添加一个新的线图元
            //int result = mapControl.MgsDrawLineByJBID(symbolName, 10, (ulong)(ptrVert.ToInt64()), count);
            Marshal.FreeHGlobal(ptrVert);
            return temp == 0 ? true : false;
        }

        /// <summary>
        /// 获取线的坐标点集合
        /// </summary>
        /// <returns></returns>
        public List<Core.Model.MapLngLat> GetLngLat()
        {
            return this.listPoint;
        }

        /// <summary>
        /// 增加坐标点
        /// </summary>
        /// <param name="lngLat"></param>
        public void AddPoint(Core.Model.MapLngLat lngLat)
        {
            var index = listPoint.FindIndex(o => o.Lng == lngLat.Lng && o.Lat == lngLat.Lat);
            if (index == -1)
            {
                listPoint.Add(lngLat);
                DrawNewLine(listPoint);
            }
        }

        /// <summary>
        /// 移除某个坐标点
        /// </summary>
        /// <param name="lngLat"></param>
        public void RemovePoint(Core.Model.MapLngLat lngLat)
        {
            var index = listPoint.FindIndex(o => o.Lng == lngLat.Lng && o.Lat == lngLat.Lat);
            if (index == -1 || listPoint.Count <= 2) return;
            listPoint.RemoveAt(index);
            DrawNewLine(listPoint);
        }

        /// <summary>
        /// 获取图元名称
        /// </summary>
        public string ElementPtr
        {
            get { return symbolName; }
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
        /// 隐藏显示
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
        /// 获取长度(单位:公里)
        /// </summary>
        /// <returns></returns>
        public double GetDistance()
        {
            return MapFrame.Core.Common.Utils.CalculateLineLength(listPoint);
        }
    }
}
