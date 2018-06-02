
using System;
using System.Drawing;
using System.Windows.Forms;
using MapFrame.Core.Interface;
using MapFrame.Logic;
using MapFrame.Core.Model;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Generic;
using GlobleSituation.Business;
using GlobleSituation.Common;
using GlobleSituation.Model;

namespace GlobleSituation.UI
{
    public partial class GMapControlEx : UserControl
    {
        /// <summary>
        /// 地图逻辑类
        /// </summary>
        public IMapLogic mapLogic = null;
        private IMFToolBox toolBox = null;

        private List<IMFElement> drawElements = new List<IMFElement>();        // 地图上已绘制的图元
        private IMFMap map = null;                                             // 地图对象
        private bool bEdit = false;                                            // 是否编辑图元
        private IMFPicture prevPicture = null;                                 // 当前图元
        public GMapControlBusiness mapBusiness = null;                         // 业务类


        public GMapControlEx(TrackLineManager trackMgr, ArcGlobeBusiness _globeBusiness)
        {
            InitializeComponent();

            InitMapFrame mapFrame = new InitMapFrame(MapEngineType.GMap, null);
            mapLogic = mapFrame.GetMapLogic();
            toolBox = mapLogic.GetToolBox();
            map = mapLogic.GetIMFMap();
            map.ElementClickEvent += Map_ElementClickEvent;
            map.MouseMoveEvent += new EventHandler<MFMouseEventArgs>(map_MouseMoveEvent);

            Control mapControl = (Control)mapLogic.GetMapControl();
            mapControl.Dock = DockStyle.Fill;
            this.mapPanel.Controls.Add(mapControl);

            mapBusiness = new GMapControlBusiness(mapLogic, trackMgr, _globeBusiness);
        }


        // 处理实时数据
        public void DealRealData(RealData data)
        {
            mapBusiness.DealRealData(data);
        }

        // 处理预警结果数据
        public void DealWarnData(RealData data, bool isWarn, List<string> warnNames)
        {
            mapBusiness.DealWarnData(data, isWarn, warnNames);
        }

        // 工具完成事件
        private void ToolBox_CommondExecutedEvent(object sender, MessageEventArgs e)
        {
            toolBox.CommondExecutedEvent -= ToolBox_CommondExecutedEvent;

            var element = e.Data as IMFElement;
            if (element != null)
            {
                drawElements.Add(element);
            }
        }

        // 漫游
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.Roam();
        }

        // 放大
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.ZoomIn();
        }

        // 缩小
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.ZoomOut();
        }

        // 全图显示
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.FullView();
        }

        // 归心
        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.ZoomToPosition(new MapLngLat(110, 23), 9);
        }

        // 地图快照
        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Image image = mapLogic.GetIMFMap().Snapshot();
            if (image != null)
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.ShowNewFolderButton = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string path = Path.Combine(dlg.SelectedPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp");
                    image.Save(path, ImageFormat.Bmp);
                }
            }
        }

        // 测量距离
        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.Measure(MeasureTypeEnum.Distance);
        }

        // 测量面积
        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.Measure(MeasureTypeEnum.Area);
        }

        // 测量方位角
        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.Measure(MeasureTypeEnum.Angle);
        }

        // 线
        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.CommondExecutedEvent -= ToolBox_CommondExecutedEvent;
            toolBox.CommondExecutedEvent += ToolBox_CommondExecutedEvent;
            toolBox.DrawGraphic(ElementTypeEnum.Line);
        }

        // 多边形
        private void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.CommondExecutedEvent -= ToolBox_CommondExecutedEvent;
            toolBox.CommondExecutedEvent += ToolBox_CommondExecutedEvent;
            toolBox.DrawGraphic(ElementTypeEnum.Polygon);
        }

        // 圆形
        private void barButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.CommondExecutedEvent -= ToolBox_CommondExecutedEvent;
            toolBox.CommondExecutedEvent += ToolBox_CommondExecutedEvent;
            toolBox.DrawGraphic(ElementTypeEnum.Circle);
        }

        // 矩形
        private void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.CommondExecutedEvent -= ToolBox_CommondExecutedEvent;
            toolBox.CommondExecutedEvent += ToolBox_CommondExecutedEvent;
            toolBox.DrawGraphic(ElementTypeEnum.Rectangle);
        }

        // 文字
        private void barButtonItem17_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            toolBox.CommondExecutedEvent -= ToolBox_CommondExecutedEvent;
            toolBox.CommondExecutedEvent += ToolBox_CommondExecutedEvent;
            toolBox.DrawGraphic(ElementTypeEnum.Text);
        }

        // 清除
        private void barButtonItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (IMFElement element in drawElements)
            {
                if (element.BelongLayer == null) continue;

                string layerName = element.BelongLayer.LayerName;
                var layer = mapLogic.GetLayer(layerName);
                layer.RemoveElement(element);
            }

            drawElements.Clear();
        }

        // 地图移动事件
        private void map_MouseMoveEvent(object sender, MFMouseEventArgs e)
        {
            string info = string.Format("经度：{0}，纬度：{1}", e.Position.Lng, e.Position.Lat);
            labLngLat.Text = info;
        }

        // 地图点击事件
        private void Map_ElementClickEvent(object sender, MFElementClickEventArgs e)
        {
            if (e.Element == null) return;

            switch (e.Element.ElementType)
            {
                case ElementTypeEnum.Picture:
                    IMFPicture picture = e.Element as IMFPicture;
                    if (picture == null) return;

                    if (prevPicture != null)
                        prevPicture.HightLight(false);

                    picture.HightLight(true);
                    prevPicture = picture;

                    if (e.MouseEventArgs.Button == MouseButtons.Right)
                    {
                        Point screenLocation = new Point(e.MouseEventArgs.X + this.Parent.Location.X + this.Parent.Parent.Location.X + this.Parent.Parent.Parent.Location.X + this.Parent.Parent.Parent.Parent.Location.X, e.MouseEventArgs.Y + this.Parent.Location.Y + this.Parent.Parent.Location.Y + this.Parent.Parent.Parent.Location.Y + this.Parent.Parent.Parent.Parent.Location.Y);
                        contextMenuStrip1.Show(screenLocation);
                    }
                    break;
                default:
                    if (bEdit == true && e.Element != null)
                    {
                        // 只能编辑已绘制的图元
                        var editElement = drawElements.Find(o => o.ElementName == e.Element.ElementName);
                        if (editElement != null)
                            toolBox.EditElement(editElement);
                    }
                    break;
            }
        }

        // 显示航迹
        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (prevPicture != null)
                mapBusiness.DoShowTrackLine(prevPicture.BelongLayer.LayerName, prevPicture.ElementName, true);
        }

        // 取消航迹
        private void 取消显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (prevPicture != null)
                mapBusiness.DoShowTrackLine(prevPicture.BelongLayer.LayerName, prevPicture.ElementName, false);
        }

        private void 全部显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapBusiness.ShowAllTrackLine(true);
        }

        private void 全部取消显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapBusiness.ShowAllTrackLine(false);
        }

        // 显示标牌
        private void 显示ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (prevPicture != null)
                prevPicture.SetLableShow(ShowTypeEnum.Always);
        }

        // 隐藏标牌
        private void 取消显示ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (prevPicture != null)
                prevPicture.SetLableShow(ShowTypeEnum.No);
        }

        // 目标显示
        private void 显示ToolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void 隐藏ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 目标详情ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // 跳转到三维
        private void 跳转到三维ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (prevPicture != null)
            {
                EventPublisher.PublishJumpToGlobeViewEvent(this, new Model.JumpToGlobeViewEventArgs(prevPicture.ElementName, prevPicture.GetLngLat()));
            }
        }

        // 删除目标
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (prevPicture != null)
                {
                    mapBusiness.DeletePlane(prevPicture.BelongLayer.LayerName, prevPicture.ElementName);
                    prevPicture = null;
                }
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(GMapControlEx), ex.Message);
            }
        }

        // 开始编辑
        private void barButtonItem20_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bEdit = true;
        }

        // 取消编辑
        private void barButtonItem21_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bEdit = false;
        }

        // 跟踪目标 
        private void 跟踪当前ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapBusiness.SetTrackElement(prevPicture, true);
        }

        // 取消跟踪
        private void 取消跟踪ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapBusiness.SetTrackElement(null, false);
        }


    }
}
