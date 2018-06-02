namespace GlobleSituation.UI
{
    partial class frmWarnInfo
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.跳转到二维地图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.暂停全部目标预警效果ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.清空预警列表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gvWarnInfo = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.sbHide = new DevExpress.XtraEditors.SimpleButton();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvWarnInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gvWarnInfo;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(656, 355);
            this.gridControl1.TabIndex = 1;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvWarnInfo});
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.跳转到二维地图ToolStripMenuItem,
            this.暂停全部目标预警效果ToolStripMenuItem,
            this.toolStripSeparator1,
            this.清空预警列表ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(173, 98);
            // 
            // 跳转到二维地图ToolStripMenuItem
            // 
            this.跳转到二维地图ToolStripMenuItem.Name = "跳转到二维地图ToolStripMenuItem";
            this.跳转到二维地图ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.跳转到二维地图ToolStripMenuItem.Text = "跳转到二维地图";
            this.跳转到二维地图ToolStripMenuItem.Click += new System.EventHandler(this.跳转到二维地图ToolStripMenuItem_Click);
            // 
            // 暂停全部目标预警效果ToolStripMenuItem
            // 
            this.暂停全部目标预警效果ToolStripMenuItem.Name = "暂停全部目标预警效果ToolStripMenuItem";
            this.暂停全部目标预警效果ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.暂停全部目标预警效果ToolStripMenuItem.Text = "跳转到三维地图";
            this.暂停全部目标预警效果ToolStripMenuItem.Click += new System.EventHandler(this.暂停全部目标预警效果ToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(169, 6);
            // 
            // 清空预警列表ToolStripMenuItem
            // 
            this.清空预警列表ToolStripMenuItem.Name = "清空预警列表ToolStripMenuItem";
            this.清空预警列表ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.清空预警列表ToolStripMenuItem.Text = "清空预警目标列表";
            this.清空预警列表ToolStripMenuItem.Click += new System.EventHandler(this.清空预警列表ToolStripMenuItem_Click);
            // 
            // gvWarnInfo
            // 
            this.gvWarnInfo.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5});
            this.gvWarnInfo.GridControl = this.gridControl1;
            this.gvWarnInfo.IndicatorWidth = 25;
            this.gvWarnInfo.Name = "gvWarnInfo";
            this.gvWarnInfo.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvWarnInfo_CustomDrawRowIndicator);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "控制";
            this.gridColumn1.FieldName = "KzCol";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.OptionsFilter.AllowFilter = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 58;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "目标编号";
            this.gridColumn2.FieldName = "TargetNumCol";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.OptionsFilter.AllowFilter = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 181;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "规则名";
            this.gridColumn3.FieldName = "RuleNameCol";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.OptionsFilter.AllowFilter = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 181;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "预警开始时间";
            this.gridColumn4.FieldName = "WarnStartCol";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.OptionsFilter.AllowFilter = false;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 185;
            // 
            // sbHide
            // 
            this.sbHide.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.sbHide.Location = new System.Drawing.Point(0, 355);
            this.sbHide.Name = "sbHide";
            this.sbHide.Size = new System.Drawing.Size(656, 23);
            this.sbHide.TabIndex = 2;
            this.sbHide.Text = "关    闭";
            this.sbHide.Click += new System.EventHandler(this.sbHide_Click);
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "目标图层";
            this.gridColumn5.FieldName = "TargetType";
            this.gridColumn5.Name = "gridColumn5";
            // 
            // frmWarnInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 378);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.sbHide);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmWarnInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "预警列表";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmWarnInfo_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvWarnInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gvWarnInfo;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 暂停全部目标预警效果ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 清空预警列表ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private DevExpress.XtraEditors.SimpleButton sbHide;
        private System.Windows.Forms.ToolStripMenuItem 跳转到二维地图ToolStripMenuItem;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
    }
}