using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ColorWheel;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using GlobleSituation.Business;
using GlobleSituation.Common;
using GlobleSituation.Interface;
using GlobleSituation.Model;
using MapFrame.Core.Interface;

namespace GlobleSituation.UI
{
    public partial class frmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private NotifyIcon notifyIcon = null;
        private MainControl MainCtrl = null;
        internal static DateTime StartTime = DateTime.Now;
        private IBookmarkManager _BookmarkManager;
        private frmAreaManager areaManagerFrm;
        private frmWarnInfo warnInfoFrm;
        private List<string> graphicNames = new List<string>();


        public frmMain()
        {
            InitializeComponent();

            DateTime dt = DateTime.Now;
            barStaticRunTime.Caption = dt.ToLongDateString() + " " + dt.ToLongTimeString();

            EventPublisher.ShowBookmarkEvent += EventPublisher_ShowBookmarkEvent;
            EventPublisher.WarnDataEvent += new EventHandler<TSDataEventArgs>(EventPublisher_WarnDataEvent);

            InitialTray();               // 托盘图标
            ShowMainCtrl();              // 显示地球界面
            //SQLiteHelper.CreateDB();     // 检查、创建数据库
            GXStroreClient storeClient = new GXStroreClient();

            warnInfoFrm = new frmWarnInfo(MainCtrl.GlobeMapContainer.globeCtrl.globeBusiness, MainCtrl.GlobeMapContainer.gmapCtrl.mapBusiness);
            warnInfoFrm.Visible = false;

            _BookmarkManager = new BookmarkManager(MainCtrl.GlobeMapContainer.globeCtrl.axGlobeControl1);

            barButtonItem16.ItemClick += BarButtonItem16_ItemClick;
            barButtonItem17.ItemClick += BarButtonItem16_ItemClick;
            barButtonItem18.ItemClick += BarButtonItem16_ItemClick;
            barButtonItem48.ItemClick += BarButtonItem16_ItemClick;
            barButtonItem49.ItemClick += BarButtonItem16_ItemClick;
            barButtonItem50.ItemClick += BarButtonItem16_ItemClick;

            EventPublisher.ChangedToHistoryEvent += new EventHandler<EventArgs>(EventPublisher_ChangedToHistoryEvent);
        }

        void EventPublisher_ChangedToHistoryEvent(object sender, EventArgs e)
        {
            MainCtrl.LoadTSData(false);            // 停止接收实时态势数据
            cbRealData.ValueChecked = false;

            Utils.Mode = Model.ShowMode.HISTORY;

            MainCtrl.GlobeMapContainer.globeCtrl.globeBusiness.ClearRealData();
            MainCtrl.GlobeMapContainer.gmapCtrl.mapBusiness.ClearRealData();
        }

        /// <summary>
        /// 加载皮肤与图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            SkinHelper.InitSkinGallery(rgbiSkins);
            ribbonControl1.Toolbar.ItemLinks.Add(rgbiSkins);

            Log4Allen.InitLog();     // 初始化日志
            //Thread.Sleep(1000);
            defaultLookAndFeel1.LookAndFeel.SkinName = AppConfigFacade.DefaultTheme;
            this.Icon = notifyIcon.Icon = global::GlobleSituation.Properties.Resources.App;
        }

        // 显示桌面
        private void btnDesktop_ItemClick(object sender, ItemClickEventArgs e)
        {
            Type shellType = Type.GetTypeFromProgID("Shell.Application");
            object shellObject = System.Activator.CreateInstance(shellType);
            shellType.InvokeMember("ToggleDesktop", System.Reflection.BindingFlags.InvokeMethod, null, shellObject, null);
        }

        // 打开程序所在目录
        private void barButtonItem14_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", Application.StartupPath);
            }
            catch
            { }
        }

        /// <summary>
        /// 重启软件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRestart_ItemClick(object sender, ItemClickEventArgs e)
        {
            Close();
            Application.Exit();
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbout_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }

        /// <summary>
        /// 初始化托盘图标
        /// </summary>
        private void InitialTray()
        {
            this.Hide();
            notifyIcon = new NotifyIcon();
            notifyIcon.BalloonTipText = "系统正在运行中...";
            notifyIcon.Text = "三维态势展示系统";
            notifyIcon.Icon = global::GlobleSituation.Properties.Resources.App;
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(1000);
            notifyIcon.MouseClick += new MouseEventHandler(notifyIcon_MouseClick);

            MenuItem exit = new MenuItem("退出");
            exit.Click += new EventHandler(exit_Click);
            MenuItem[] childen = new MenuItem[] { exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);
            this.FormClosing += new FormClosingEventHandler(frmMain_FormClosing);
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (XtraMessageBox.Show("是否退出系统？", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                e.Cancel = false;
                notifyIcon.Visible = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 鼠标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visible == true)
                {
                    this.Visible = false;
                }
                else
                {
                    this.Visible = true;
                    this.Activate();
                }
            }
        }

        // 三维地球
        private void barButtonItem13_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowMainCtrl();
        }

        // 显示三维地球
        public void ShowMainCtrl()
        {
            panelMain.Controls.Clear();

            if (MainCtrl == null)
                MainCtrl = new MainControl() { Dock = DockStyle.Fill };
            panelMain.Controls.Add(MainCtrl);
        }

        // 书签列表
        private void ribbonPageGroup6_CaptionButtonClick(object sender, DevExpress.XtraBars.Ribbon.RibbonPageGroupEventArgs e)
        {
            frmBookmarkMgr frm = new frmBookmarkMgr();
            frm.TopMost = true;
            frm.Show();
        }

        // 实时数据
        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmRealData FrmRealData = new frmRealData();
            using (FrmRealData)
            {
                FrmRealData.ShowDialog();
            }
        }

        // 经纬度计算
        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmLngLatCalculate frm = new frmLngLatCalculate(MainCtrl);
            frm.Show();
        }

        // 经纬度转换
        private void barButtonItem15_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmLngLatConvert frm = new frmLngLatConvert();
            frm.Show();
        }

        // 鹰眼
        private void cbEye_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit ck = sender as CheckEdit;
            if (ck != null)
            {
                string vis = ck.Checked == true ? "visible" : "hide";
                MainCtrl.GlobeMapContainer.globeCtrl.ShowEagleEye(vis);
            }
        }

        // 地图工具栏
        private void riceToolBar_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit ck = sender as CheckEdit;
            if (ck != null)
            {
                MainCtrl.GlobeMapContainer.globeCtrl.ShowToolBar(ck.Checked);
            }
        }

        // 自定义皮肤
        private void bsiNavigation_ItemClick(object sender, ItemClickEventArgs e)
        {
            ColorWheelForm form = new ColorWheelForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.SkinMaskColor = UserLookAndFeel.Default.SkinMaskColor;
            form.SkinMaskColor2 = UserLookAndFeel.Default.SkinMaskColor2;
            form.Text = "颜色混合器";
            form.ShowDialog(this);
        }

        // 实时数据
        private void cbRealData_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit ck = sender as CheckEdit;
            if (ck != null)
            {
                MainCtrl.LoadTSData(ck.Checked);
                if (ck.Checked)
                    EventPublisher.PublishChangedToRealTimeEvent(this, null);
            }
        }

        // 波束数据
        private void cbBeamData_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit ck = sender as CheckEdit;
            if (ck != null)
            {
                MainCtrl.LoadBeamData(ck.Checked);
            }
        }

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainCtrl.LoadTitleFromServer();

            return;

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "3dd地图文件(*.3dd)|*.3dd|lyr矢量文件(*.lyr)|*.lyr|shp地图文件(*.shp)|*.shp";
            string path = "";

            if (open.ShowDialog() == DialogResult.OK)
            {
                path = open.FileName;
            }
            if (path == "")
            {
                return;
            }

            MainCtrl.Load3dFile(path);
        }

        /// <summary>
        /// 全图显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainCtrl.GlobeMapContainer.globeCtrl.axGlobeControl1.CurrentTool = null;
            ICommand Cmd = new ControlsGlobeFullExtentCommandClass();
            Cmd.OnCreate(MainCtrl.GlobeMapContainer.globeCtrl.axGlobeControl1.Object);
            Cmd.OnClick();
        }

        // 移动到书签
        private void BarIetm_ItemClick(object sender, ItemClickEventArgs e)
        {
            string name = e.Item.Caption;
            _BookmarkManager.Move2Bookmark(name);
        }

        // 创建书签
        private void barButtonItem19_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (frmBookmarkAdd frm = new frmBookmarkAdd())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // 创建书签
                    ISceneBookmarks pBookmarks = MainCtrl.GlobeMapContainer.globeCtrl.axGlobeControl1.Globe.GlobeDisplay.Scene as ISceneBookmarks;
                    ICamera camera = MainCtrl.GlobeMapContainer.globeCtrl.axGlobeControl1.Globe.GlobeDisplay.ActiveViewer.Camera;
                    string name = frm.GetName();
                    string remark = frm.GetRemark();
                    IBookmark3D pBookmark3D = new Bookmark3DClass();
                    pBookmark3D.Name = name;
                    pBookmark3D.Capture(camera);
                    pBookmarks.AddBookmark(pBookmark3D);

                    // 保存书签
                    Bookmark bm = new Bookmark();
                    bm.Altitude = camera.Observer.Z;
                    bm.Azimuth = camera.Azimuth;
                    bm.Inclination = camera.Inclination;
                    bm.Latitude = camera.Observer.Y;
                    bm.Longitude = camera.Observer.X;
                    bm.Name = name;
                    bm.Remark = remark;
                    bm.RollAngle = camera.RollAngle;
                    bm.Show = "true";
                    bm.ViewFieldAngle = camera.ViewFieldAngle;
                    bm.ViewingDistance = camera.ViewingDistance;

                    string xmlFile = AppDomain.CurrentDomain.BaseDirectory + "Config\\Bookmark.xml";
                    BookmarkList bookmarkList = null;
                    if (File.Exists(xmlFile))
                    {
                        bookmarkList = XmlHelper.XmlDeserializeFromFile<BookmarkList>(xmlFile, Encoding.UTF8);
                    }

                    if (bookmarkList == null)
                    {
                        bookmarkList = new BookmarkList();
                        bookmarkList.BookmarkArr.Add(bm);
                    }
                    else
                    {
                        bookmarkList.BookmarkArr.Add(bm);
                    }
                    // 发布事件，通知主窗体添加按钮
                    EventPublisher.PublishShowBookmarkEvent(this, new ShowBookmarkEventArgs() { NameList = new System.Collections.Generic.List<string> { name }, Append = true });
                    // 将书签信息保存到xml配置文件
                    XmlHelper.XmlSerializeToFile(bookmarkList, xmlFile, Encoding.UTF8);
                }
            }
        }

        // 显示控制
        private void BarButtonItem16_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainCtrl.ShowPanel(e.Item.Caption);
        }

        // 告警策略
        private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (frmWarnRule frm = new frmWarnRule())
            {
                frm.ShowDialog();
            }
        }

        // 告警管理
        private void barButtonItem11_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (frmWarnSet frm = new frmWarnSet())
            {
                frm.ShowDialog();
            }
        }

        // 告警信息
        private void barButtonItem12_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (warnInfoFrm == null || warnInfoFrm.IsDisposed)
            {
                warnInfoFrm = new frmWarnInfo(MainCtrl.GlobeMapContainer.globeCtrl.globeBusiness, MainCtrl.GlobeMapContainer.gmapCtrl.mapBusiness);
            }
            warnInfoFrm.Show();
        }

        // 预警区域
        private void barButtonItem45_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (areaManagerFrm == null || areaManagerFrm.IsDisposed)
            {
                areaManagerFrm = new frmAreaManager(MainCtrl.GlobeMapContainer.warnMgr);
            }
            areaManagerFrm.Show();
        }


        // 圆形
        private void barButtonItem32_ItemClick(object sender, ItemClickEventArgs e)
        {
            //var toolBox = MainCtrl.GlobeMapContainer.globeCtrl.mapLogic.GetToolBox();

            //if (toolBox != null)
            //{
            //    toolBox = MainCtrl.GlobeMapContainer.globeCtrl.mapLogic.GetToolBox();
            //    if (toolBox != null)
            //    {
            //        toolBox.CommondExecutedEvent += new EventHandler<MapFrame.Core.Model.MessageEventArgs>(toolBox_CommondExecutedEvent);
            //        toolBox.DrawGraphic(MapFrame.Core.Model.ElementTypeEnum.Circle);
            //    }
            //}
        }

        // 多边形
        private void barButtonItem33_ItemClick(object sender, ItemClickEventArgs e)
        {
            var toolBox = MainCtrl.GlobeMapContainer.globeCtrl.mapLogic.GetToolBox();

            if (toolBox != null)
            {
                toolBox = MainCtrl.GlobeMapContainer.globeCtrl.mapLogic.GetToolBox();
                if (toolBox != null)
                {
                    toolBox.CommondExecutedEvent += new EventHandler<MapFrame.Core.Model.MessageEventArgs>(toolBox_CommondExecutedEvent);
                    toolBox.DrawGraphic(MapFrame.Core.Model.ElementTypeEnum.Polygon);
                }
            }
        }

        // 绘制图形完成事件
        void toolBox_CommondExecutedEvent(object sender, MapFrame.Core.Model.MessageEventArgs e)
        {
            try
            {
                var tool = MainCtrl.GlobeMapContainer.globeCtrl.mapLogic.GetToolBox();
                if (tool == null) return;

                tool.CommondExecutedEvent -= toolBox_CommondExecutedEvent;
                tool.ReleaseTool();

                var element = e.Data as IMFElement;
                if (element == null) return;

                graphicNames.Add(element.ElementName);
            }
            catch (Exception)
            {
            }
        }

        // 清除
        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (MainCtrl.GlobeMapContainer.globeCtrl.mapLogic != null)
            {
                foreach (string name in graphicNames)
                {
                    var ele = MainCtrl.GlobeMapContainer.globeCtrl.mapLogic.GetElement(name);
                    if (ele != null)
                    {
                        var layer = ele.BelongLayer;
                        layer.RemoveElement(ele);
                    }
                }

                graphicNames.Clear();
            }
        }

        // 设置
        private void barButtonItem41_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (frmSet setFrm = new frmSet())
            {
                setFrm.TopMost = true;
                setFrm.ShowDialog();
            }
        }

        // 区域管理
        private void barButtonItem42_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (areaManagerFrm == null || areaManagerFrm.IsDisposed)
            {
                areaManagerFrm = new frmAreaManager(MainCtrl.GlobeMapContainer.warnMgr);
            }
            areaManagerFrm.Show();
        }

        // 显示书签列表
        private void EventPublisher_ShowBookmarkEvent(object sender, ShowBookmarkEventArgs e)
        {
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();

            if (e.Append == false)
                barSubItem3.LinksPersistInfo.Clear();

            if (e.NameList == null)
            {
                ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
                return;
            }

            foreach (string name in e.NameList)
            {
                BarButtonItem barIetm = new BarButtonItem();
                barIetm.Name = name;
                barIetm.Caption = name;
                barIetm.ItemClick += BarIetm_ItemClick;
                barSubItem3.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(barIetm));
            }

            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
        }

        // 预警数据
        void EventPublisher_WarnDataEvent(object sender, TSDataEventArgs e)
        {
            if (Utils.bStartTip)
            {
                string title = "态势告警";
                string text = string.Format("目标编号：{0}\n经度：{1}\n纬度：{1}\n高度：{2}\n告警区域：{3}", e.Data.TargetNum, e.Data.Longitude, e.Data.Latitude, e.Data.Altitude, e.AreaName);

                notifyIcon.BalloonTipTitle = title;
                notifyIcon.BalloonTipText = text;
                notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000);
            }
        }

        // 实时信息
        private void barButtonItem43_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmRealDataCount frm = new frmRealDataCount();
            frm.TopMost = true;
            frm.Show();
        }

        // 区域统计
        private void barButtonItem44_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmAreaChart frm = new frmAreaChart(MainCtrl.GlobeMapContainer.gmapCtrl.mapLogic.GetToolBox());
            frm.TopMost = true;
            frm.Show();
        }


    }
}
