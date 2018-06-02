using System.Windows.Forms;
using MapFrame.Logic;
using MapFrame.Core.Interface;
using System;
using MapFrame.Core.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace MapFrame.Test
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 地图逻辑类
        /// </summary>
        private IMapLogic mapLogic = null;

        private IMFMap map = null;


        private string drawLayerName = "draw_layer";
        private IMFElement element = null;
        private IMFPoint pointElement = null;
        private IMFPicture picElement = null;
        private IMFLine lineElement = null;
        private IMFPolygon polygonElement = null;
        private IMFText textElement = null;
        private IMFCircle circleElement = null;
        private IMFPolygon rectangleElement = null;

        public Form1()
        {
            InitializeComponent();

            InitMapFrame();
        }

        /// <summary>
        /// 初始化地图框架
        /// </summary>
        private void InitMapFrame()
        {
            InitMapFrame mapFrame = new InitMapFrame(MapEngineType.GMap, null);
            mapLogic = mapFrame.GetMapLogic();
            map = mapLogic.GetIMFMap();

            map.ElementClickEvent += new System.EventHandler<Core.Model.MFElementClickEventArgs>(map_ElementClickEvent);
            map.MouseMoveEvent += new EventHandler<MFMouseEventArgs>(map_MouseMoveEvent);
            Control mapControl = (Control)mapLogic.GetMapControl();
            mapControl.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(mapControl);

            mapLogic.GetToolBox().CommondExecutedEvent += new EventHandler<MessageEventArgs>(Form1_CommondExecutedEvent);
            System.Diagnostics.Debug.WriteLine("初始化程序线程ID" + Thread.CurrentThread.ManagedThreadId);
        }

        void map_MouseMoveEvent(object sender, MFMouseEventArgs e)
        {
            toolStripStatusLabel1.Text = string.Format("经度：{0}\t纬度：{1}\t高度：{2}", e.Position.Lng, e.Position.Lat, e.Position.Alt);
        }

        void Form1_CommondExecutedEvent(object sender, MessageEventArgs e)
        {
            switch (e.ToolType)
            {
                case ToolTypeEnum.Draw:
                    element = e.Data as IMFElement;
                    switch (element.ElementType)
                    {
                        case ElementTypeEnum.Circle:
                            circleElement = element as IMFCircle;
                            break;
                        case ElementTypeEnum.Point:
                            pointElement = element as IMFPoint;
                            break;
                        case ElementTypeEnum.Line:
                            lineElement = element as IMFLine;
                            double length = lineElement.GetDistance();
                            break;
                        case ElementTypeEnum.Polygon:
                            polygonElement = element as IMFPolygon;
                            double area = polygonElement.GetArea();
                            break;
                        case ElementTypeEnum.Text:
                            textElement = element as IMFText;
                            break;
                        case ElementTypeEnum.Rectangle:
                            rectangleElement = element as IMFPolygon;
                            break;
                    }
                    break;
                case ToolTypeEnum.Measure:
                    break;
                case ToolTypeEnum.Select:
                    List<IMFElement> elements = (sender as IMFSelect).GetSelectElements();
                    break;
                case ToolTypeEnum.Edit:
                    switch (element.ElementType)
                    {
                        case ElementTypeEnum.Point:
                            pointElement = element as IMFPoint;
                            break;
                        case ElementTypeEnum.Line:
                            lineElement = element as IMFLine;
                            double length = lineElement.GetDistance();
                            break;
                        case ElementTypeEnum.Polygon:
                            polygonElement = element as IMFPolygon;
                            double area = polygonElement.GetArea();
                            break;
                        case ElementTypeEnum.Text:
                            textElement = element as IMFText;
                            break;
                    }
                    break;
            }
        }

        void map_ElementClickEvent(object sender, Core.Model.MFElementClickEventArgs e)
        {
            Debug.WriteLine("ElementClickEvent___" + e.Element.ElementType.ToString());
        }

        private void 加载地图ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            string mapFile = @"D:\workspace\Src\MapFrame.Test\bin\Debug\GMapCache\";
            mapLogic.GetIMFMap().LoadMap(mapFile);
        }

        private void 添加图层ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            mapLogic.AddLayer(drawLayerName);
        }

        private void 删除图层ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            mapLogic.RemoveLayer(drawLayerName);
        }

        #region 点

        List<IMFPicture> picList = new List<IMFPicture>();
        private void 添加ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("添加点程序线程ID" + Thread.CurrentThread.ManagedThreadId);
            for (int i = 0; i < 5000; i++)
            {
                Kml kml = new Kml();
                string iconUrl = AppDomain.CurrentDomain.BaseDirectory + @"Image\hoverplane.png";
                kml.Placemark.Name = "point"+i;
                MapLngLat lnglat = new MapLngLat(random.Next(-18000, 18000) / 100, random.Next(-9000, 9000) / 100);
                kml.Placemark.Graph = new KmlPicture() { Position = lnglat, IconUrl = iconUrl, Scale = 1, TipText = "标牌测试...." };
                mapLogic.GetLayer(drawLayerName).AddElement(kml, out element);
                picElement = element as IMFPicture;
                picList.Add(picElement);
            }
        }

        private void 手动添加ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            //mapLogic.GetToolBox().DrawGraphic(ElementTypeEnum.Point);
        }

        private void 设置大小ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (picElement == null) return;
            picElement.SetScale(2);
        }

        private void 更新位置ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (picElement == null) return;
            picElement.UpdatePosition(new MapLngLat(101, 33));
        }

        private void 设置图标ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (picElement == null) return;
            picElement.SetIcon(AppDomain.CurrentDomain.BaseDirectory + @"Image\hoverplane.png");
        }

        Random random = new Random();
        private void 设置方位角ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Thread thread = new Thread(SetAngle);
            if (setedAngle)
            {
                thread.Abort();
                setedAngle = false;
            }
            else 
            {
                thread.Start();
            }
        }

        bool setedAngle = false;
        private void SetAngle(object obj) 
        {
            setedAngle = true;
            while (obj == null)
            {
                foreach (var item in picList)
                {
                    if (item == null) break;
                    item.SetAngle(random.Next(0, 360));
                }
            }
        }

        private void 设置标牌内容ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (picElement == null) return;
            picElement.SetTipText("这是我设置的tip内容");
        }

        private void 编辑图元ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (picElement == null) return;
            mapLogic.GetToolBox().EditElement(picElement);
        }

        private void 闪烁ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            foreach (var item in picList) 
            {
                if (item == null) break;
                item.Flash(!item.IsFlash,500);
            }
        }

        private void 高亮ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (picElement == null) return;
            picElement.HightLight(!picElement.IsHightLight);
        }

        private void 显示隐藏ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (picElement == null) return;
            picElement.SetVisible(!picElement.IsVisible);
        }

        private void 删除图元ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (picElement == null) return;
            mapLogic.GetLayer(drawLayerName).RemoveElement(picElement);
        }
        #endregion

        #region 线

        private void 添加ToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            Kml kml = new Kml();
            kml.Placemark.Name = "line";

            KmlLineString linekml = new KmlLineString();
            List<MapLngLat> pList = new List<MapLngLat>();
            pList.Add(new MapLngLat(101, 23));
            pList.Add(new MapLngLat(15, 50));

            linekml.PositionList = pList;
            kml.Placemark.Graph = linekml;
            mapLogic.GetLayer(drawLayerName).AddElement(kml, out element);
            lineElement = element as IMFLine;
        }

        private void 手动添加ToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            mapLogic.GetToolBox().DrawGraphic(ElementTypeEnum.Line);
        }

        private void 设置线宽ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (lineElement == null) return;
            lineElement.SetWidth(20);
        }

        private void 设置颜色ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (lineElement == null) return;
            lineElement.SetColor(Color.Red);
        }

        private void 更新位置ToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            if (lineElement == null) return;
            MapLngLat map1 = new MapLngLat(56, 48);
            MapLngLat map2 = new MapLngLat(101, 69);
            List<MapLngLat> listMap = new List<MapLngLat>();
            listMap.Add(map1);
            listMap.Add(map2);
            lineElement.UpdatePosition(listMap);
        }

        private void 编辑图元ToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            if (lineElement == null) return;
            mapLogic.GetToolBox().EditElement(lineElement);
        }

        private void 闪烁ToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            if (lineElement == null) return;
            lineElement.Flash(!lineElement.IsFlash);
        }

        private void 高亮ToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            if (lineElement == null) return;
            lineElement.HightLight(!lineElement.IsHightLight);
        }

        private void 显示隐藏ToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            if (lineElement == null) return;
            lineElement.SetVisible(!lineElement.IsVisible);
        }

        private void 添加一个点ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (lineElement == null) return;
            lineElement.AddPoint(new MapLngLat(0, 0));
        }

        private void 移除一个点ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (lineElement == null) return;
            lineElement.RemovePoint(new MapLngLat(0, 0));
        }

        private void 获取长度ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (lineElement == null) return;
            lineElement.GetDistance();
        }

        private void 获取点集合ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (lineElement == null) return;
            List<MapLngLat> pointList = lineElement.GetLngLat();
        }

        private void 删除图元ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (lineElement == null) return;
            mapLogic.GetLayer(drawLayerName).RemoveElement(lineElement);
        }
        #endregion

        #region 面

        private void 添加ToolStripMenuItem2_Click(object sender, System.EventArgs e)
        {
            Kml polygonKml = new Kml();
            polygonKml.Placemark.Name = "polygon";
            List<MapLngLat> pList = new List<MapLngLat>();
            pList.Add(new MapLngLat(102, 45));
            pList.Add(new MapLngLat(97, 16));
            pList.Add(new MapLngLat(89, 62));
            pList.Add(new MapLngLat(79, 63));

            polygonKml.Placemark.Graph = new KmlPolygon() { Description = "", PositionList = pList, };    //position 
            mapLogic.GetLayer(drawLayerName).AddElement(polygonKml);//添加多边形到地图中
            polygonElement = mapLogic.GetLayer(drawLayerName).GetElement("polygon") as IMFPolygon;
        }

        private void 手动添加ToolStripMenuItem2_Click(object sender, System.EventArgs e)
        {
            mapLogic.GetToolBox().DrawGraphic(ElementTypeEnum.Polygon);
        }

        private void 设置轮廓颜色ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (polygonElement == null) return;
            polygonElement.SetOutLineColor(Color.Red);
        }

        private void 设置填充色ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (polygonElement == null) return;
            polygonElement.SetFillColor(Color.Yellow);
        }

        private void 设置轮廓粗细ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (polygonElement == null) return;
            polygonElement.SetOutLineSize(20);
        }

        private void 更新位置ToolStripMenuItem2_Click(object sender, System.EventArgs e)
        {
            if (polygonElement == null) return;
            MapLngLat m1 = new MapLngLat(111, 56);
            MapLngLat m2 = new MapLngLat(89, 76);
            MapLngLat m3 = new MapLngLat(92, 66);
            MapLngLat m4 = new MapLngLat(100, 86);
            List<MapLngLat> listMap = new List<MapLngLat>();
            listMap.Add(m1);
            listMap.Add(m2);
            listMap.Add(m3);
            listMap.Add(m4);
            polygonElement.UpdatePosition(listMap);
        }

        private void 编辑图元ToolStripMenuItem2_Click(object sender, System.EventArgs e)
        {
            if (polygonElement == null) return;
            mapLogic.GetToolBox().EditElement(polygonElement);
        }

        private void 闪烁ToolStripMenuItem2_Click(object sender, System.EventArgs e)
        {
            if (polygonElement == null) return;
            polygonElement.Flash(!polygonElement.IsFlash);
        }

        private void 显示隐藏ToolStripMenuItem2_Click(object sender, System.EventArgs e)
        {
            if (polygonElement == null) return;
            polygonElement.SetVisible(!polygonElement.IsVisible);
        }

        private void 高亮ToolStripMenuItem2_Click(object sender, System.EventArgs e)
        {
            if (polygonElement == null) return;
            polygonElement.HightLight(!polygonElement.IsHightLight);
        }

        private void 添加一个点ToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            if (polygonElement == null) return;
            polygonElement.AddPoint(new MapLngLat(0, 0));
        }

        private void 移除一个点ToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            if (polygonElement == null) return;
            polygonElement.RemovePoint(new MapLngLat(0, 0));
        }


        private void 删除图元ToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            if (polygonElement == null) return;
            mapLogic.GetLayer(drawLayerName).RemoveElement(polygonElement);
        }
        #endregion

        #region 文字

        private void 添加ToolStripMenuItem3_Click(object sender, System.EventArgs e)
        {
            Kml kml = new Kml();
            kml.Placemark.Name = "text";
            kml.Placemark.Graph = new KmlText() { Position = new MapLngLat(102, 45), Content = "Hello World", Color = Color.Red, Font = "宋体", Size = 20 };
            mapLogic.GetLayer(drawLayerName).AddElement(kml);//添加多边形到地图中
            textElement = mapLogic.GetLayer(drawLayerName).GetElement("text") as IMFText;
        }

        private void 手动添加ToolStripMenuItem3_Click(object sender, System.EventArgs e)
        {
            mapLogic.GetToolBox().DrawGraphic(ElementTypeEnum.Text);
        }

        private void 设置文本内容ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (textElement == null) return;
            textElement.SetContext("地图");
        }

        private void 设置文本颜色ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (textElement == null) return;
            textElement.SetColor(Color.Black.ToArgb());
        }

        private void 设置字体ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (textElement == null) return;
            textElement.SetFont("隶书");
        }

        private void 设置大小ToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
            if (textElement == null) return;
            textElement.SetSize(50);
        }

        private void 编辑图元ToolStripMenuItem3_Click(object sender, System.EventArgs e)
        {
            if (textElement == null) return;
            mapLogic.GetToolBox().EditElement(textElement);
        }

        private void 删除图元ToolStripMenuItem3_Click(object sender, System.EventArgs e)
        {
            if (textElement == null) return;
            mapLogic.GetLayer(drawLayerName).RemoveElement(textElement);
        }
        #endregion

        #region 圆
        private void 添加圆ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kml kml = new Kml();
            kml.Placemark.Name = "my_circle";

            KmlCircle circle = new KmlCircle();
            circle.Position = new MapLngLat(110, 30);
            circle.FillColor = Color.Red;
            circle.Radius = 500000;
            circle.StrokeColor = Color.Gray;
            circle.StrokeWidth = 3;

            kml.Placemark.Graph = circle;

            // 画点
            mapLogic.GetLayer(drawLayerName).AddElement(kml, out element);
            circleElement = element as IMFCircle;
        }

        private void 手动添加圆ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapLogic.GetToolBox().DrawGraphic(ElementTypeEnum.Circle);
        }

        private void 更换填充色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (circleElement == null) return;
            circleElement.SetFillColor(Color.FromArgb(70,Color.Yellow));
        }

        private void 更改轮廓色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (circleElement == null) return;
            circleElement.SetStroke(Color.Blue, 2);
        }

        private void 更改轮廓宽度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (circleElement == null) return;
            circleElement.SetStroke(Color.Blue, 5);
        }

        private void 高亮ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (circleElement == null) return;
            circleElement.HightLight(!circleElement.IsHightLight);
        }

        private void 闪烁ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (circleElement == null) return;
            circleElement.Flash(!circleElement.IsFlash);
        }

        private void 显示隐藏ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (circleElement == null) return;
            circleElement.SetVisible(!circleElement.IsVisible);
        }

        private void 更改半径ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (circleElement == null) return;
            circleElement.UpdatePosition(300000);
        }

        private void 更改圆心坐标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (circleElement == null) return;
            circleElement.UpdatePosition(new MapLngLat(90, 90));
        }

        private void 编辑圆ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (circleElement == null) return;
            mapLogic.GetToolBox().EditElement(circleElement);
        }

        private void 删除圆ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (circleElement == null) return;
            mapLogic.GetLayer(drawLayerName).RemoveElement(circleElement);
        }
        #endregion

        #region 工具

        private void 放大ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            mapLogic.GetToolBox().ZoomIn();
        }

        private void 缩小ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            mapLogic.GetToolBox().ZoomOut();
        }

        private void 漫游ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            mapLogic.GetToolBox().Roam();
        }

        private void 定位到某点ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            mapLogic.GetToolBox().ZoomToPosition(new MapLngLat(110, 30));
        }

        private void 全图显示ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            mapLogic.GetToolBox().FullView();
        }


        private void 框选ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            mapLogic.GetToolBox().Select();
        }

        private void 测量距离ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapLogic.GetToolBox().Measure(MeasureTypeEnum.Distance);
        }

        private void 测量面积ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapLogic.GetToolBox().Measure(MeasureTypeEnum.Area);
        }

        private void 测量方位角ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapLogic.GetToolBox().Measure(MeasureTypeEnum.Angle);
        }
        #endregion

    }
}
