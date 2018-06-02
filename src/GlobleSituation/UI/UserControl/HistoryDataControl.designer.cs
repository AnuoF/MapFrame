namespace GlobleSituation.UI
{
    partial class uc_HistoryDataCtrl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lblPageInfo = new System.Windows.Forms.Label();
            this.btnNext = new DevExpress.XtraEditors.SimpleButton();
            this.btnPre = new DevExpress.XtraEditors.SimpleButton();
            this.gcRealData = new DevExpress.XtraGrid.GridControl();
            this.cmsRealData = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemJump = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItemClear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItemExport = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.gvRealData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnLon = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnLat = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnHeight = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnSource = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnCountry = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnTargetProperty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnEquipModel = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnFOV = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnFOA = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.dateEnd = new DevExpress.XtraEditors.DateEdit();
            this.dateStart = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.errSearch = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.toolTipDetail = new DevExpress.Utils.ToolTipController(this.components);
            this.saveRealDialog = new System.Windows.Forms.SaveFileDialog();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcRealData)).BeginInit();
            this.cmsRealData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvRealData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEnd.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEnd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateStart.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateStart.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblPageInfo);
            this.layoutControl1.Controls.Add(this.btnNext);
            this.layoutControl1.Controls.Add(this.btnPre);
            this.layoutControl1.Controls.Add(this.gcRealData);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.dateEnd);
            this.layoutControl1.Controls.Add(this.dateStart);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(306, 144, 648, 544);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(387, 573);
            this.layoutControl1.TabIndex = 3;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblPageInfo
            // 
            this.lblPageInfo.Location = new System.Drawing.Point(5, 518);
            this.lblPageInfo.Name = "lblPageInfo";
            this.lblPageInfo.Size = new System.Drawing.Size(377, 24);
            this.lblPageInfo.TabIndex = 22;
            this.lblPageInfo.Text = "第0页，共0页";
            this.lblPageInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnNext
            // 
            this.btnNext.Enabled = false;
            this.btnNext.Location = new System.Drawing.Point(198, 546);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(184, 22);
            this.btnNext.StyleController = this.layoutControl1;
            this.btnNext.TabIndex = 21;
            this.btnNext.Text = "下一页";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPre
            // 
            this.btnPre.Enabled = false;
            this.btnPre.Location = new System.Drawing.Point(5, 546);
            this.btnPre.Name = "btnPre";
            this.btnPre.Size = new System.Drawing.Size(189, 22);
            this.btnPre.StyleController = this.layoutControl1;
            this.btnPre.TabIndex = 20;
            this.btnPre.Text = "前一页";
            this.btnPre.Click += new System.EventHandler(this.btnPre_Click);
            // 
            // gcRealData
            // 
            this.gcRealData.ContextMenuStrip = this.cmsRealData;
            this.gcRealData.Location = new System.Drawing.Point(5, 79);
            this.gcRealData.MainView = this.gvRealData;
            this.gcRealData.Name = "gcRealData";
            this.gcRealData.Size = new System.Drawing.Size(377, 435);
            this.gcRealData.TabIndex = 19;
            this.gcRealData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvRealData});
            // 
            // cmsRealData
            // 
            this.cmsRealData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemJump,
            this.toolStripSeparator1,
            this.ToolStripMenuItemClear,
            this.toolStripSeparator3,
            this.ToolStripMenuItemExport,
            this.ToolStripMenuItemRefresh});
            this.cmsRealData.Name = "cmsRealData";
            this.cmsRealData.Size = new System.Drawing.Size(153, 126);
            // 
            // ToolStripMenuItemJump
            // 
            this.ToolStripMenuItemJump.Name = "ToolStripMenuItemJump";
            this.ToolStripMenuItemJump.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.J)));
            this.ToolStripMenuItemJump.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItemJump.Text = "跳转";
            this.ToolStripMenuItemJump.Click += new System.EventHandler(this.ToolStripMenuItemJump_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // ToolStripMenuItemClear
            // 
            this.ToolStripMenuItemClear.Name = "ToolStripMenuItemClear";
            this.ToolStripMenuItemClear.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.ToolStripMenuItemClear.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItemClear.Text = "清空";
            this.ToolStripMenuItemClear.Click += new System.EventHandler(this.ToolStripMenuItemClear_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // ToolStripMenuItemExport
            // 
            this.ToolStripMenuItemExport.Name = "ToolStripMenuItemExport";
            this.ToolStripMenuItemExport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this.ToolStripMenuItemExport.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItemExport.Text = "导出";
            this.ToolStripMenuItemExport.Click += new System.EventHandler(this.ToolStripMenuItemExport_Click);
            // 
            // ToolStripMenuItemRefresh
            // 
            this.ToolStripMenuItemRefresh.Checked = true;
            this.ToolStripMenuItemRefresh.CheckOnClick = true;
            this.ToolStripMenuItemRefresh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ToolStripMenuItemRefresh.Name = "ToolStripMenuItemRefresh";
            this.ToolStripMenuItemRefresh.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.ToolStripMenuItemRefresh.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItemRefresh.Text = "刷新";
            this.ToolStripMenuItemRefresh.Visible = false;
            // 
            // gvRealData
            // 
            this.gvRealData.Appearance.Row.Options.UseTextOptions = true;
            this.gvRealData.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvRealData.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gvRealData.Appearance.ViewCaption.Options.UseTextOptions = true;
            this.gvRealData.Appearance.ViewCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvRealData.Appearance.ViewCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gvRealData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnID,
            this.gridColumnLon,
            this.gridColumnLat,
            this.gridColumnHeight,
            this.gridColumnTime,
            this.gridColumnSource,
            this.gridColumnCountry,
            this.gridColumnTargetProperty,
            this.gridColumnEquipModel,
            this.gridColumnFOV,
            this.gridColumnFOA});
            this.gvRealData.GridControl = this.gcRealData;
            this.gvRealData.IndicatorWidth = 70;
            this.gvRealData.Name = "gvRealData";
            this.gvRealData.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gvRealData.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gvRealData.OptionsBehavior.Editable = false;
            this.gvRealData.OptionsBehavior.ReadOnly = true;
            this.gvRealData.OptionsDetail.AllowZoomDetail = false;
            this.gvRealData.OptionsView.ColumnAutoWidth = false;
            this.gvRealData.OptionsView.ShowGroupPanel = false;
            this.gvRealData.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvRealData_CustomDrawRowIndicator_1);
            this.gvRealData.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gvRealData_CustomDrawCell);
            this.gvRealData.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gvRealData_MouseMove);
            this.gvRealData.MouseLeave += new System.EventHandler(this.gvRealData_MouseLeave);
            // 
            // gridColumnID
            // 
            this.gridColumnID.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnID.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnID.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnID.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnID.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnID.Caption = "目标编号";
            this.gridColumnID.FieldName = "TargetNum";
            this.gridColumnID.Name = "gridColumnID";
            this.gridColumnID.Visible = true;
            this.gridColumnID.VisibleIndex = 0;
            this.gridColumnID.Width = 150;
            // 
            // gridColumnLon
            // 
            this.gridColumnLon.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnLon.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnLon.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnLon.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnLon.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnLon.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnLon.Caption = "经度";
            this.gridColumnLon.DisplayFormat.FormatString = "N4";
            this.gridColumnLon.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnLon.FieldName = "Longitude";
            this.gridColumnLon.Name = "gridColumnLon";
            this.gridColumnLon.Visible = true;
            this.gridColumnLon.VisibleIndex = 1;
            this.gridColumnLon.Width = 100;
            // 
            // gridColumnLat
            // 
            this.gridColumnLat.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnLat.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnLat.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnLat.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnLat.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnLat.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnLat.Caption = "纬度";
            this.gridColumnLat.DisplayFormat.FormatString = "N4";
            this.gridColumnLat.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnLat.FieldName = "Latitude";
            this.gridColumnLat.Name = "gridColumnLat";
            this.gridColumnLat.Visible = true;
            this.gridColumnLat.VisibleIndex = 2;
            this.gridColumnLat.Width = 100;
            // 
            // gridColumnHeight
            // 
            this.gridColumnHeight.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnHeight.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnHeight.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnHeight.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnHeight.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnHeight.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnHeight.Caption = "高度";
            this.gridColumnHeight.DisplayFormat.FormatString = "N4";
            this.gridColumnHeight.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnHeight.FieldName = "Altitude";
            this.gridColumnHeight.Name = "gridColumnHeight";
            this.gridColumnHeight.Visible = true;
            this.gridColumnHeight.VisibleIndex = 3;
            this.gridColumnHeight.Width = 100;
            // 
            // gridColumnTime
            // 
            this.gridColumnTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnTime.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnTime.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnTime.Caption = "时间";
            this.gridColumnTime.FieldName = "PositionDate";
            this.gridColumnTime.Name = "gridColumnTime";
            this.gridColumnTime.Visible = true;
            this.gridColumnTime.VisibleIndex = 6;
            this.gridColumnTime.Width = 150;
            // 
            // gridColumnSource
            // 
            this.gridColumnSource.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnSource.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnSource.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnSource.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnSource.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnSource.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnSource.Caption = "来源";
            this.gridColumnSource.FieldName = "InformationSource";
            this.gridColumnSource.Name = "gridColumnSource";
            this.gridColumnSource.Visible = true;
            this.gridColumnSource.VisibleIndex = 7;
            this.gridColumnSource.Width = 100;
            // 
            // gridColumnCountry
            // 
            this.gridColumnCountry.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnCountry.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnCountry.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnCountry.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnCountry.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnCountry.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnCountry.Caption = "国家";
            this.gridColumnCountry.FieldName = "Country";
            this.gridColumnCountry.Name = "gridColumnCountry";
            this.gridColumnCountry.Visible = true;
            this.gridColumnCountry.VisibleIndex = 4;
            this.gridColumnCountry.Width = 100;
            // 
            // gridColumnTargetProperty
            // 
            this.gridColumnTargetProperty.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnTargetProperty.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnTargetProperty.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnTargetProperty.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnTargetProperty.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnTargetProperty.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnTargetProperty.Caption = "性质";
            this.gridColumnTargetProperty.FieldName = "TargetProperty";
            this.gridColumnTargetProperty.Name = "gridColumnTargetProperty";
            this.gridColumnTargetProperty.Visible = true;
            this.gridColumnTargetProperty.VisibleIndex = 8;
            this.gridColumnTargetProperty.Width = 100;
            // 
            // gridColumnEquipModel
            // 
            this.gridColumnEquipModel.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnEquipModel.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnEquipModel.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnEquipModel.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnEquipModel.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnEquipModel.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnEquipModel.Caption = "型号";
            this.gridColumnEquipModel.FieldName = "EquipModelNumber";
            this.gridColumnEquipModel.Name = "gridColumnEquipModel";
            this.gridColumnEquipModel.Visible = true;
            this.gridColumnEquipModel.VisibleIndex = 5;
            this.gridColumnEquipModel.Width = 100;
            // 
            // gridColumnFOV
            // 
            this.gridColumnFOV.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnFOV.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnFOV.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnFOV.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnFOV.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnFOV.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnFOV.Caption = "视野范围";
            this.gridColumnFOV.DisplayFormat.FormatString = "N4";
            this.gridColumnFOV.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnFOV.FieldName = "ScanRange";
            this.gridColumnFOV.Name = "gridColumnFOV";
            this.gridColumnFOV.Visible = true;
            this.gridColumnFOV.VisibleIndex = 9;
            this.gridColumnFOV.Width = 100;
            // 
            // gridColumnFOA
            // 
            this.gridColumnFOA.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnFOA.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnFOA.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnFOA.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnFOA.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnFOA.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnFOA.Caption = "行动范围";
            this.gridColumnFOA.DisplayFormat.FormatString = "N4";
            this.gridColumnFOA.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnFOA.FieldName = "ActionRange";
            this.gridColumnFOA.Name = "gridColumnFOA";
            this.gridColumnFOA.Visible = true;
            this.gridColumnFOA.VisibleIndex = 10;
            this.gridColumnFOA.Width = 100;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(5, 53);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(377, 22);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 16;
            this.btnSearch.Text = "查询(&C)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dateEnd
            // 
            this.dateEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateEnd.EditValue = null;
            this.dateEnd.Location = new System.Drawing.Point(69, 29);
            this.dateEnd.Name = "dateEnd";
            this.dateEnd.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dateEnd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEnd.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
            this.dateEnd.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dateEnd.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Vista;
            this.dateEnd.Properties.DisplayFormat.FormatString = "G";
            this.dateEnd.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dateEnd.Properties.EditFormat.FormatString = "G";
            this.dateEnd.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dateEnd.Properties.Mask.EditMask = "G";
            this.dateEnd.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
            this.dateEnd.Size = new System.Drawing.Size(313, 20);
            this.dateEnd.StyleController = this.layoutControl1;
            this.dateEnd.TabIndex = 15;
            // 
            // dateStart
            // 
            this.dateStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dateStart.EditValue = null;
            this.dateStart.Location = new System.Drawing.Point(69, 5);
            this.dateStart.Name = "dateStart";
            this.dateStart.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dateStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateStart.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
            this.dateStart.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dateStart.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Vista;
            this.dateStart.Properties.DisplayFormat.FormatString = "G";
            this.dateStart.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dateStart.Properties.EditFormat.FormatString = "G";
            this.dateStart.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dateStart.Properties.Mask.EditMask = "G";
            this.dateStart.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
            this.dateStart.Size = new System.Drawing.Size(313, 20);
            this.dateStart.StyleController = this.layoutControl1;
            this.dateStart.TabIndex = 14;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem8,
            this.layoutControlItem9,
            this.layoutControlItem10,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
            this.layoutControlGroup1.Size = new System.Drawing.Size(387, 573);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.dateStart;
            this.layoutControlItem8.CustomizationFormText = "开始时间：";
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(381, 24);
            this.layoutControlItem8.Text = "开始时间：";
            this.layoutControlItem8.TextSize = new System.Drawing.Size(60, 14);
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.dateEnd;
            this.layoutControlItem9.CustomizationFormText = "结束时间：";
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(381, 24);
            this.layoutControlItem9.Text = "结束时间：";
            this.layoutControlItem9.TextSize = new System.Drawing.Size(60, 14);
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.btnSearch;
            this.layoutControlItem10.CustomizationFormText = "layoutControlItem10";
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(381, 26);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcRealData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 74);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(381, 439);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnPre;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 541);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(193, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnNext;
            this.layoutControlItem3.Location = new System.Drawing.Point(193, 541);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(188, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.lblPageInfo;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 513);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(381, 28);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // errSearch
            // 
            this.errSearch.ContainerControl = this;
            // 
            // toolTipDetail
            // 
            this.toolTipDetail.Appearance.BackColor = System.Drawing.Color.White;
            this.toolTipDetail.Appearance.Options.UseBackColor = true;
            this.toolTipDetail.Rounded = true;
            this.toolTipDetail.ShowBeak = true;
            this.toolTipDetail.ToolTipLocation = DevExpress.Utils.ToolTipLocation.LeftCenter;
            this.toolTipDetail.ToolTipType = DevExpress.Utils.ToolTipType.Standard;
            // 
            // saveRealDialog
            // 
            this.saveRealDialog.Filter = "CSV|*.csv|PDF|*.pdf|HTML|*.html|RTF|*.RTF|Excel 2003|*.xls|Excel 2007|*.xlsx";
            this.saveRealDialog.Title = "导出实时数据列表";
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "IMSI";
            this.gridColumn3.FieldName = "IMSI";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 0;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "IMEI";
            this.gridColumn4.FieldName = "IMEI";
            this.gridColumn4.Name = "gridColumn4";
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "TMSI";
            this.gridColumn5.FieldName = "TMSI";
            this.gridColumn5.Name = "gridColumn5";
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "经度";
            this.gridColumn6.DisplayFormat.FormatString = "n6";
            this.gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn6.FieldName = "LongitudeForShow";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 1;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "纬度";
            this.gridColumn7.DisplayFormat.FormatString = "n6";
            this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn7.FieldName = "LatitudeForShow";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 2;
            // 
            // gridColumn11
            // 
            this.gridColumn11.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn11.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn11.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn11.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn11.Caption = "载波类型";
            this.gridColumn11.FieldName = "BearerTypeForShow";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 3;
            this.gridColumn11.Width = 145;
            // 
            // gridColumn12
            // 
            this.gridColumn12.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn12.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn12.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn12.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn12.Caption = "子载波类型";
            this.gridColumn12.FieldName = "SubBearerTypeForShow";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 4;
            this.gridColumn12.Width = 145;
            // 
            // uc_HistoryDataCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.layoutControl1);
            this.DoubleBuffered = true;
            this.Name = "uc_HistoryDataCtrl";
            this.Size = new System.Drawing.Size(387, 573);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcRealData)).EndInit();
            this.cmsRealData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvRealData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEnd.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEnd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateStart.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateStart.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errSearch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.DateEdit dateEnd;
        private DevExpress.XtraEditors.DateEdit dateStart;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider errSearch;
        private DevExpress.Utils.ToolTipController toolTipDetail;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private System.Windows.Forms.ContextMenuStrip cmsRealData;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemJump;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemClear;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemExport;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemRefresh;
        private System.Windows.Forms.SaveFileDialog saveRealDialog;
        private DevExpress.XtraGrid.GridControl gcRealData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvRealData;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnID;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLon;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLat;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnHeight;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSource;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnCountry;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnTargetProperty;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnEquipModel;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnFOV;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnFOA;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraEditors.SimpleButton btnNext;
        private DevExpress.XtraEditors.SimpleButton btnPre;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private System.Windows.Forms.Label lblPageInfo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
    }
}
