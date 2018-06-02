namespace MapFrame.ArcMap.Windows
{
    partial class MapLabel
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
            this.labContext = new System.Windows.Forms.Label();
            this.labF = new System.Windows.Forms.Label();
            this.tootipTool1 = new MapFrame.ArcMap.Windows.TootipTool();
            this.SuspendLayout();
            // 
            // labContext
            // 
            this.labContext.AutoSize = true;
            this.labContext.ForeColor = System.Drawing.Color.White;
            this.labContext.Location = new System.Drawing.Point(10, 10);
            this.labContext.Name = "labContext";
            this.labContext.Size = new System.Drawing.Size(29, 12);
            this.labContext.TabIndex = 3;
            this.labContext.Text = "标牌";
            // 
            // labF
            // 
            this.labF.AutoSize = true;
            this.labF.Location = new System.Drawing.Point(10, 10);
            this.labF.Name = "labF";
            this.labF.Size = new System.Drawing.Size(29, 12);
            this.labF.TabIndex = 1;
            this.labF.Text = "labF";
            this.labF.Visible = false;
            // 
            // tootipTool1
            // 
            this.tootipTool1.BackColor = System.Drawing.Color.LightGray;
            this.tootipTool1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tootipTool1.ForeColor = System.Drawing.Color.Black;
            this.tootipTool1.Location = new System.Drawing.Point(166, 0);
            this.tootipTool1.Name = "tootipTool1";
            this.tootipTool1.Size = new System.Drawing.Size(14, 246);
            this.tootipTool1.TabIndex = 2;
            this.tootipTool1.Transparency = 180;
            // 
            // ToolTipEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.Controls.Add(this.tootipTool1);
            this.Controls.Add(this.labF);
            this.Controls.Add(this.labContext);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "ToolTipEx";
            this.Size = new System.Drawing.Size(180, 246);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ToolTipEx_MouseDown);
            this.MouseLeave += new System.EventHandler(this.ToolTipEx_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ToolTipEx_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ToolTipEx_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labContext;
        private System.Windows.Forms.Label labF;
        private MapFrame.ArcMap.Windows.TootipTool tootipTool1;

    }
}
