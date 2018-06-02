namespace GlobleSituation.UI
{
    partial class LayersControl
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.闪烁ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.颜色ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.treeList1 = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.cbCover = new DevExpress.XtraEditors.CheckEdit();
            this.cbBeam = new DevExpress.XtraEditors.CheckEdit();
            this.cbSatellite = new DevExpress.XtraEditors.CheckEdit();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbCover.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbBeam.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbSatellite.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.闪烁ToolStripMenuItem,
            this.颜色ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 48);
            // 
            // 闪烁ToolStripMenuItem
            // 
            this.闪烁ToolStripMenuItem.Name = "闪烁ToolStripMenuItem";
            this.闪烁ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.闪烁ToolStripMenuItem.Text = "闪烁";
            this.闪烁ToolStripMenuItem.Click += new System.EventHandler(this.闪烁ToolStripMenuItem_Click);
            // 
            // 颜色ToolStripMenuItem
            // 
            this.颜色ToolStripMenuItem.Name = "颜色ToolStripMenuItem";
            this.颜色ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.颜色ToolStripMenuItem.Text = "颜色";
            this.颜色ToolStripMenuItem.Click += new System.EventHandler(this.颜色ToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // treeList1
            // 
            this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1});
            this.treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList1.Location = new System.Drawing.Point(2, 21);
            this.treeList1.Name = "treeList1";
            this.treeList1.OptionsClipboard.AllowCopy = DevExpress.Utils.DefaultBoolean.True;
            this.treeList1.OptionsClipboard.CopyNodeHierarchy = DevExpress.Utils.DefaultBoolean.True;
            this.treeList1.OptionsView.ShowCheckBoxes = true;
            this.treeList1.Size = new System.Drawing.Size(295, 338);
            this.treeList1.TabIndex = 1;
            this.treeList1.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.treeList1_AfterCheckNode);
            this.treeList1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeList1_MouseDown);
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "图元";
            this.treeListColumn1.FieldName = "colLayerName";
            this.treeListColumn1.MinWidth = 32;
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.OptionsColumn.AllowEdit = false;
            this.treeListColumn1.OptionsColumn.AllowMove = false;
            this.treeListColumn1.OptionsColumn.AllowMoveToCustomizationForm = false;
            this.treeListColumn1.OptionsColumn.AllowSort = false;
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            this.treeListColumn1.Width = 91;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.cbCover);
            this.groupControl1.Controls.Add(this.cbBeam);
            this.groupControl1.Controls.Add(this.cbSatellite);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(299, 103);
            this.groupControl1.TabIndex = 2;
            this.groupControl1.Text = "图层显示控制";
            // 
            // cbCover
            // 
            this.cbCover.EditValue = true;
            this.cbCover.Location = new System.Drawing.Point(20, 77);
            this.cbCover.Name = "cbCover";
            this.cbCover.Properties.Caption = "地面覆盖图层";
            this.cbCover.Size = new System.Drawing.Size(101, 19);
            this.cbCover.TabIndex = 2;
            this.cbCover.CheckedChanged += new System.EventHandler(this.cbCover_CheckedChanged);
            // 
            // cbBeam
            // 
            this.cbBeam.EditValue = true;
            this.cbBeam.Location = new System.Drawing.Point(20, 51);
            this.cbBeam.Name = "cbBeam";
            this.cbBeam.Properties.Caption = "波束图层";
            this.cbBeam.Size = new System.Drawing.Size(75, 19);
            this.cbBeam.TabIndex = 1;
            this.cbBeam.CheckedChanged += new System.EventHandler(this.cbBeam_CheckedChanged);
            // 
            // cbSatellite
            // 
            this.cbSatellite.EditValue = true;
            this.cbSatellite.Location = new System.Drawing.Point(20, 25);
            this.cbSatellite.Name = "cbSatellite";
            this.cbSatellite.Properties.Caption = "卫星图层";
            this.cbSatellite.Size = new System.Drawing.Size(75, 19);
            this.cbSatellite.TabIndex = 0;
            this.cbSatellite.CheckedChanged += new System.EventHandler(this.cbSatellite_CheckedChanged);
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.treeList1);
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl2.Location = new System.Drawing.Point(0, 103);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(299, 361);
            this.groupControl2.TabIndex = 3;
            this.groupControl2.Text = "图元显示控制";
            // 
            // LayersControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupControl2);
            this.Controls.Add(this.groupControl1);
            this.Name = "LayersControl";
            this.Size = new System.Drawing.Size(299, 464);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbCover.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbBeam.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbSatellite.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private DevExpress.XtraTreeList.TreeList treeList1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private System.Windows.Forms.ToolStripMenuItem 闪烁ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 颜色ToolStripMenuItem;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.CheckEdit cbCover;
        private DevExpress.XtraEditors.CheckEdit cbBeam;
        private DevExpress.XtraEditors.CheckEdit cbSatellite;
        private DevExpress.XtraEditors.GroupControl groupControl2;
    }
}
