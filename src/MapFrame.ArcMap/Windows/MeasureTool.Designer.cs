namespace MapFrame.ArcMap.Windows
{
    partial class MeasureTool
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
            this.lbResult = new System.Windows.Forms.Label();
            this.contextMeasure = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tool_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMeasure.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbResult
            // 
            this.lbResult.AutoSize = true;
            this.lbResult.Location = new System.Drawing.Point(9, 25);
            this.lbResult.Name = "lbResult";
            this.lbResult.Size = new System.Drawing.Size(0, 12);
            this.lbResult.TabIndex = 0;
            // 
            // contextMeasure
            // 
            this.contextMeasure.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tool_Copy});
            this.contextMeasure.Name = "contextMenuStrip1";
            this.contextMeasure.Size = new System.Drawing.Size(153, 48);
            // 
            // tool_Copy
            // 
            this.tool_Copy.Name = "tool_Copy";
            this.tool_Copy.Size = new System.Drawing.Size(152, 22);
            this.tool_Copy.Text = "复制";
            this.tool_Copy.Click += new System.EventHandler(this.tool_Copy_Click);
            // 
            // MeasureTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ContextMenuStrip = this.contextMeasure;
            this.Controls.Add(this.lbResult);
            this.Name = "MeasureTool";
            this.Size = new System.Drawing.Size(229, 80);
            this.contextMeasure.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbResult;
        private System.Windows.Forms.ContextMenuStrip contextMeasure;
        private System.Windows.Forms.ToolStripMenuItem tool_Copy;
    }
}
