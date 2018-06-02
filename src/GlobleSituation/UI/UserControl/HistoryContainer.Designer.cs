namespace GlobleSituation.UI
{
    partial class HistoryContainer
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataPlayControl1
            // 
            this.dataPlayControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataPlayControl1.Location = new System.Drawing.Point(0, 365);
            this.dataPlayControl1.Name = "dataPlayControl1";
            this.dataPlayControl1.Size = new System.Drawing.Size(298, 58);
            this.dataPlayControl1.TabIndex = 0;
            // 
            // panelControl1
            // 
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(298, 365);
            this.panelControl1.TabIndex = 1;
            // 
            // HistoryContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.dataPlayControl1);
            this.Name = "HistoryContainer";
            this.Size = new System.Drawing.Size(298, 423);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private HistoryDataPlayControl dataPlayControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
    }
}
