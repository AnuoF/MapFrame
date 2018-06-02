namespace MapFrame.GMap.Windows
{
    partial class TextInput
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextInput));
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.pboxFont = new System.Windows.Forms.PictureBox();
            this.pboxColor = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pboxFont)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pboxColor)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtInput
            // 
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInput.Location = new System.Drawing.Point(0, 0);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(284, 22);
            this.txtInput.TabIndex = 0;
            this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            // 
            // pboxFont
            // 
            this.pboxFont.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pboxFont.BackgroundImage = global::MapFrame.GMap.Properties.Resources.font;
            this.pboxFont.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pboxFont.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pboxFont.Dock = System.Windows.Forms.DockStyle.Right;
            this.pboxFont.Location = new System.Drawing.Point(284, 0);
            this.pboxFont.Name = "pboxFont";
            this.pboxFont.Size = new System.Drawing.Size(22, 22);
            this.pboxFont.TabIndex = 2;
            this.pboxFont.TabStop = false;
            this.toolTip1.SetToolTip(this.pboxFont, "设置字体");
            this.pboxFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // pboxColor
            // 
            this.pboxColor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pboxColor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pboxColor.BackgroundImage")));
            this.pboxColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pboxColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pboxColor.Dock = System.Windows.Forms.DockStyle.Right;
            this.pboxColor.Location = new System.Drawing.Point(306, 0);
            this.pboxColor.Name = "pboxColor";
            this.pboxColor.Size = new System.Drawing.Size(22, 22);
            this.pboxColor.TabIndex = 3;
            this.pboxColor.TabStop = false;
            this.toolTip1.SetToolTip(this.pboxColor, "设置字颜色");
            this.pboxColor.Click += new System.EventHandler(this.pboxColor_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(328, 55);
            this.panel1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 0;
            // 
            // TextInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.pboxFont);
            this.Controls.Add(this.pboxColor);
            this.Controls.Add(this.panel1);
            this.Name = "TextInput";
            this.Size = new System.Drawing.Size(328, 77);
            this.Load += new System.EventHandler(this.TextInput_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pboxFont)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pboxColor)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.PictureBox pboxFont;
        private System.Windows.Forms.PictureBox pboxColor;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
