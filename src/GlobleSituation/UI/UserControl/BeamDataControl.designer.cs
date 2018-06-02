namespace GlobleSituation.UI
{
    partial class BeamDataControl
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.数据滚动ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colStatelliteId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBeamId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colLng = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colLat = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAlt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.toolTipDetail = new DevExpress.Utils.ToolTipController(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(537, 348);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.数据滚动ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 26);
            // 
            // 数据滚动ToolStripMenuItem
            // 
            this.数据滚动ToolStripMenuItem.Name = "数据滚动ToolStripMenuItem";
            this.数据滚动ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.数据滚动ToolStripMenuItem.Text = "数据滚动";
            this.数据滚动ToolStripMenuItem.Click += new System.EventHandler(this.数据滚动ToolStripMenuItem_Click);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colStatelliteId,
            this.colBeamId,
            this.colLng,
            this.colLat,
            this.colAlt,
            this.colType});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.IndicatorWidth = 50;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridView1_MouseMove);
            // 
            // colStatelliteId
            // 
            this.colStatelliteId.Caption = "卫星ID";
            this.colStatelliteId.FieldName = "StateId";
            this.colStatelliteId.Name = "colStatelliteId";
            this.colStatelliteId.OptionsColumn.AllowEdit = false;
            this.colStatelliteId.OptionsColumn.AllowMove = false;
            this.colStatelliteId.OptionsFilter.AllowFilter = false;
            this.colStatelliteId.Visible = true;
            this.colStatelliteId.VisibleIndex = 0;
            // 
            // colBeamId
            // 
            this.colBeamId.Caption = "波束ID";
            this.colBeamId.FieldName = "BeamId";
            this.colBeamId.Name = "colBeamId";
            this.colBeamId.OptionsColumn.AllowEdit = false;
            this.colBeamId.OptionsColumn.AllowMove = false;
            this.colBeamId.OptionsFilter.AllowFilter = false;
            this.colBeamId.Visible = true;
            this.colBeamId.VisibleIndex = 1;
            // 
            // colLng
            // 
            this.colLng.Caption = "经度";
            this.colLng.FieldName = "Lng";
            this.colLng.Name = "colLng";
            this.colLng.OptionsColumn.AllowEdit = false;
            this.colLng.OptionsColumn.AllowMove = false;
            this.colLng.OptionsFilter.AllowFilter = false;
            this.colLng.Visible = true;
            this.colLng.VisibleIndex = 2;
            // 
            // colLat
            // 
            this.colLat.Caption = "纬度";
            this.colLat.FieldName = "Lat";
            this.colLat.Name = "colLat";
            this.colLat.OptionsColumn.AllowEdit = false;
            this.colLat.OptionsColumn.AllowMove = false;
            this.colLat.OptionsFilter.AllowFilter = false;
            this.colLat.Visible = true;
            this.colLat.VisibleIndex = 3;
            // 
            // colAlt
            // 
            this.colAlt.Caption = "高度";
            this.colAlt.FieldName = "Alt";
            this.colAlt.Name = "colAlt";
            this.colAlt.OptionsColumn.AllowEdit = false;
            this.colAlt.OptionsColumn.AllowMove = false;
            this.colAlt.OptionsFilter.AllowFilter = false;
            this.colAlt.Visible = true;
            this.colAlt.VisibleIndex = 4;
            // 
            // colType
            // 
            this.colType.Caption = "数据类型";
            this.colType.FieldName = "BType";
            this.colType.Name = "colType";
            this.colType.OptionsColumn.AllowEdit = false;
            this.colType.OptionsColumn.AllowMove = false;
            this.colType.OptionsFilter.AllowFilter = false;
            this.colType.Visible = true;
            this.colType.VisibleIndex = 5;
            // 
            // BeamDataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridControl1);
            this.Name = "BeamDataControl";
            this.Size = new System.Drawing.Size(537, 348);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colStatelliteId;
        private DevExpress.XtraGrid.Columns.GridColumn colBeamId;
        private DevExpress.XtraGrid.Columns.GridColumn colLng;
        private DevExpress.XtraGrid.Columns.GridColumn colLat;
        private DevExpress.XtraGrid.Columns.GridColumn colAlt;
        private DevExpress.XtraGrid.Columns.GridColumn colType;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 数据滚动ToolStripMenuItem;
        private DevExpress.Utils.ToolTipController toolTipDetail;
    }
}
