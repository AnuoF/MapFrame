namespace MapFrame.GMap
{
    partial class TootipTool
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
            this.labBColor = new System.Windows.Forms.Label();
            this.labFColor = new System.Windows.Forms.Label();
            this.labClose = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // labBColor
            // 
            this.labBColor.AutoSize = true;
            this.labBColor.Font = new System.Drawing.Font("宋体", 8F);
            this.labBColor.Location = new System.Drawing.Point(0, 36);
            this.labBColor.Name = "labBColor";
            this.labBColor.Size = new System.Drawing.Size(16, 11);
            this.labBColor.TabIndex = 2;
            this.labBColor.Text = "背";
            this.labBColor.Click += new System.EventHandler(this.labBColor_Click);
            this.labBColor.MouseHover += new System.EventHandler(this.labFColor_MouseHover);
            // 
            // labFColor
            // 
            this.labFColor.AutoSize = true;
            this.labFColor.BackColor = System.Drawing.Color.Transparent;
            this.labFColor.Font = new System.Drawing.Font("宋体", 8F);
            this.labFColor.Location = new System.Drawing.Point(0, 20);
            this.labFColor.Name = "labFColor";
            this.labFColor.Size = new System.Drawing.Size(16, 11);
            this.labFColor.TabIndex = 1;
            this.labFColor.Text = "字";
            this.labFColor.Click += new System.EventHandler(this.labFColor_Click);
            this.labFColor.MouseHover += new System.EventHandler(this.labFColor_MouseHover);
            // 
            // labClose
            // 
            this.labClose.AutoSize = true;
            this.labClose.Font = new System.Drawing.Font("宋体", 12F);
            this.labClose.Location = new System.Drawing.Point(-3, 1);
            this.labClose.Name = "labClose";
            this.labClose.Size = new System.Drawing.Size(24, 16);
            this.labClose.TabIndex = 5;
            this.labClose.Text = "×";
            this.labClose.Click += new System.EventHandler(this.labClose_Click);
            this.labClose.MouseHover += new System.EventHandler(this.labFColor_MouseHover);
            // 
            // trackBar1
            // 
            this.trackBar1.AutoSize = false;
            this.trackBar1.Location = new System.Drawing.Point(-1, 44);
            this.trackBar1.Maximum = 255;
            this.trackBar1.Minimum = 50;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(19, 192);
            this.trackBar1.TabIndex = 4;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Value = 250;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 8F);
            this.label1.Location = new System.Drawing.Point(-1, 231);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 11);
            this.label1.TabIndex = 3;
            this.label1.Text = "扩";
            this.label1.MouseHover += new System.EventHandler(this.labFColor_MouseHover);
            // 
            // TootipTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labBColor);
            this.Controls.Add(this.labFColor);
            this.Controls.Add(this.labClose);
            this.Controls.Add(this.trackBar1);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "TootipTool";
            this.Size = new System.Drawing.Size(14, 242);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labBColor;
        private System.Windows.Forms.Label labFColor;
        private System.Windows.Forms.Label labClose;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label1;
    }
}
