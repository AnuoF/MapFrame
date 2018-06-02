namespace GlobleSituation.UI
{
    partial class RealDataListControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            dbWriteThread.Abort();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gcRealData = new DevExpress.XtraGrid.GridControl();
            this.cmsRealData = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.设置颜色ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.跳转ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.闪烁ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.设置颜色ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
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
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnEquipModel = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnFOV = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnFOA = new DevExpress.XtraGrid.Columns.GridColumn();
            this.toolTipDetail = new DevExpress.Utils.ToolTipController(this.components);
            this.saveRealDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.gcRealData)).BeginInit();
            this.cmsRealData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvRealData)).BeginInit();
            this.SuspendLayout();
            // 
            // gcRealData
            // 
            this.gcRealData.ContextMenuStrip = this.cmsRealData;
            this.gcRealData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcRealData.Location = new System.Drawing.Point(0, 0);
            this.gcRealData.MainView = this.gvRealData;
            this.gcRealData.Name = "gcRealData";
            this.gcRealData.Size = new System.Drawing.Size(807, 219);
            this.gcRealData.TabIndex = 0;
            this.gcRealData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvRealData});
            // 
            // cmsRealData
            // 
            this.cmsRealData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设置颜色ToolStripMenuItem,
            this.toolStripSeparator1,
            this.ToolStripMenuItemDelete,
            this.ToolStripMenuItemClear,
            this.toolStripSeparator3,
            this.ToolStripMenuItemExport,
            this.ToolStripMenuItemRefresh});
            this.cmsRealData.Name = "cmsRealData";
            this.cmsRealData.Size = new System.Drawing.Size(153, 148);
            // 
            // 设置颜色ToolStripMenuItem
            // 
            this.设置颜色ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.跳转ToolStripMenuItem,
            this.闪烁ToolStripMenuItem1,
            this.设置颜色ToolStripMenuItem1});
            this.设置颜色ToolStripMenuItem.Name = "设置颜色ToolStripMenuItem";
            this.设置颜色ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.设置颜色ToolStripMenuItem.Text = "目标操作";
            // 
            // 跳转ToolStripMenuItem
            // 
            this.跳转ToolStripMenuItem.Name = "跳转ToolStripMenuItem";
            this.跳转ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.J)));
            this.跳转ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.跳转ToolStripMenuItem.Text = "跳转";
            this.跳转ToolStripMenuItem.Click += new System.EventHandler(this.跳转ToolStripMenuItem_Click);
            // 
            // 闪烁ToolStripMenuItem1
            // 
            this.闪烁ToolStripMenuItem1.Name = "闪烁ToolStripMenuItem1";
            this.闪烁ToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.闪烁ToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.闪烁ToolStripMenuItem1.Text = "闪烁";
            this.闪烁ToolStripMenuItem1.Click += new System.EventHandler(this.闪烁ToolStripMenuItem1_Click);
            // 
            // 设置颜色ToolStripMenuItem1
            // 
            this.设置颜色ToolStripMenuItem1.Name = "设置颜色ToolStripMenuItem1";
            this.设置颜色ToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.设置颜色ToolStripMenuItem1.Text = "设置颜色";
            this.设置颜色ToolStripMenuItem1.Visible = false;
            this.设置颜色ToolStripMenuItem1.Click += new System.EventHandler(this.设置颜色ToolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // ToolStripMenuItemDelete
            // 
            this.ToolStripMenuItemDelete.Name = "ToolStripMenuItemDelete";
            this.ToolStripMenuItemDelete.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D)));
            this.ToolStripMenuItemDelete.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItemDelete.Text = "删除";
            this.ToolStripMenuItemDelete.Click += new System.EventHandler(this.ToolStripMenuItemDelete_Click);
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
            this.gridColumn3,
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
            this.gvRealData.OptionsView.ColumnAutoWidth = false;
            this.gvRealData.OptionsView.ShowGroupPanel = false;
            this.gvRealData.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvRealData_CustomDrawRowIndicator);
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
            // gridColumn3
            // 
            this.gridColumn3.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumn3.Caption = "性质";
            this.gridColumn3.FieldName = "TargetProperty";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 8;
            this.gridColumn3.Width = 100;
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
            // saveRealDialog
            // 
            this.saveRealDialog.Filter = "CSV|*.csv|PDF|*.pdf|HTML|*.html|RTF|*.RTF|Excel 2003|*.xls|Excel 2007|*.xlsx";
            this.saveRealDialog.Title = "导出实时数据列表";
            // 
            // RealDataListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gcRealData);
            this.DoubleBuffered = true;
            this.Name = "RealDataListControl";
            this.Size = new System.Drawing.Size(807, 219);
            ((System.ComponentModel.ISupportInitialize)(this.gcRealData)).EndInit();
            this.cmsRealData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvRealData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gcRealData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvRealData;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnID;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLon;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLat;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnHeight;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnCountry;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnEquipModel;
        private System.Windows.Forms.ContextMenuStrip cmsRealData;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemDelete;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemClear;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemExport;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemRefresh;
        private DevExpress.Utils.ToolTipController toolTipDetail;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.SaveFileDialog saveRealDialog;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSource;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnFOV;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnFOA;
        private System.Windows.Forms.ToolStripMenuItem 设置颜色ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 跳转ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 闪烁ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 设置颜色ToolStripMenuItem1;
    }
}
