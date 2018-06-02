namespace GlobleSituation.UI
{
    partial class DisplayMgrControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayMgrControl));
            this.axTOCControl1 = new ESRI.ArcGIS.Controls.AxTOCControl();
            this.tpObject = new DevExpress.XtraEditors.PanelControl();
            this.sbObject = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.tpMap = new DevExpress.XtraEditors.PanelControl();
            this.sbMap = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.tpArea = new DevExpress.XtraEditors.PanelControl();
            this.sbArea = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
            this.panelMap = new DevExpress.XtraEditors.PanelControl();
            this.panelArea = new DevExpress.XtraEditors.PanelControl();
            this.panelObject = new DevExpress.XtraEditors.PanelControl();
            this.cbSea = new DevExpress.XtraEditors.CheckEdit();
            this.cbLand = new DevExpress.XtraEditors.CheckEdit();
            this.cbSky = new DevExpress.XtraEditors.CheckEdit();
            this.cbSatelliteBeam = new DevExpress.XtraEditors.CheckEdit();
            this.checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpObject)).BeginInit();
            this.tpObject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tpMap)).BeginInit();
            this.tpMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tpArea)).BeginInit();
            this.tpArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
            this.panelControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelMap)).BeginInit();
            this.panelMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelObject)).BeginInit();
            this.panelObject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbSea.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbLand.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbSky.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbSatelliteBeam.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // axTOCControl1
            // 
            this.axTOCControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axTOCControl1.Location = new System.Drawing.Point(2, 2);
            this.axTOCControl1.Name = "axTOCControl1";
            this.axTOCControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTOCControl1.OcxState")));
            this.axTOCControl1.Size = new System.Drawing.Size(243, 141);
            this.axTOCControl1.TabIndex = 1;
            // 
            // tpObject
            // 
            this.tpObject.Controls.Add(this.sbObject);
            this.tpObject.Controls.Add(this.labelControl1);
            this.tpObject.Dock = System.Windows.Forms.DockStyle.Top;
            this.tpObject.Location = new System.Drawing.Point(0, 0);
            this.tpObject.Name = "tpObject";
            this.tpObject.Size = new System.Drawing.Size(251, 30);
            this.tpObject.TabIndex = 0;
            // 
            // sbObject
            // 
            this.sbObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sbObject.Location = new System.Drawing.Point(219, 1);
            this.sbObject.Name = "sbObject";
            this.sbObject.Size = new System.Drawing.Size(26, 27);
            this.sbObject.TabIndex = 1;
            this.sbObject.Tag = "true";
            this.sbObject.Text = "";
            this.sbObject.Click += new System.EventHandler(this.sbObject_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(7, 6);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 14);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "目标显示";
            // 
            // tpMap
            // 
            this.tpMap.Controls.Add(this.sbMap);
            this.tpMap.Controls.Add(this.labelControl2);
            this.tpMap.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tpMap.Location = new System.Drawing.Point(0, 551);
            this.tpMap.Name = "tpMap";
            this.tpMap.Size = new System.Drawing.Size(251, 30);
            this.tpMap.TabIndex = 1;
            // 
            // sbMap
            // 
            this.sbMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sbMap.Location = new System.Drawing.Point(219, 1);
            this.sbMap.Name = "sbMap";
            this.sbMap.Size = new System.Drawing.Size(26, 27);
            this.sbMap.TabIndex = 1;
            this.sbMap.Tag = "false";
            this.sbMap.Text = "";
            this.sbMap.Click += new System.EventHandler(this.sbMap_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(7, 6);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(48, 14);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "地图显控";
            // 
            // tpArea
            // 
            this.tpArea.Controls.Add(this.sbArea);
            this.tpArea.Controls.Add(this.labelControl3);
            this.tpArea.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tpArea.Location = new System.Drawing.Point(0, 521);
            this.tpArea.Name = "tpArea";
            this.tpArea.Size = new System.Drawing.Size(251, 30);
            this.tpArea.TabIndex = 5;
            // 
            // sbArea
            // 
            this.sbArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sbArea.Location = new System.Drawing.Point(219, 1);
            this.sbArea.Name = "sbArea";
            this.sbArea.Size = new System.Drawing.Size(26, 27);
            this.sbArea.TabIndex = 1;
            this.sbArea.Tag = "false";
            this.sbArea.Text = "";
            this.sbArea.Click += new System.EventHandler(this.sbArea_Click);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(7, 6);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(48, 14);
            this.labelControl3.TabIndex = 0;
            this.labelControl3.Text = "区域显控";
            // 
            // panelControl4
            // 
            this.panelControl4.Controls.Add(this.panelMap);
            this.panelControl4.Controls.Add(this.panelArea);
            this.panelControl4.Controls.Add(this.panelObject);
            this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl4.Location = new System.Drawing.Point(0, 30);
            this.panelControl4.Name = "panelControl4";
            this.panelControl4.Size = new System.Drawing.Size(251, 491);
            this.panelControl4.TabIndex = 6;
            // 
            // panelMap
            // 
            this.panelMap.Appearance.BackColor = System.Drawing.Color.Blue;
            this.panelMap.Appearance.Options.UseBackColor = true;
            this.panelMap.Controls.Add(this.axTOCControl1);
            this.panelMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMap.Location = new System.Drawing.Point(2, 344);
            this.panelMap.Name = "panelMap";
            this.panelMap.Size = new System.Drawing.Size(247, 145);
            this.panelMap.TabIndex = 2;
            // 
            // panelArea
            // 
            this.panelArea.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.panelArea.Appearance.Options.UseBackColor = true;
            this.panelArea.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelArea.Location = new System.Drawing.Point(2, 141);
            this.panelArea.Name = "panelArea";
            this.panelArea.Size = new System.Drawing.Size(247, 203);
            this.panelArea.TabIndex = 1;
            // 
            // panelObject
            // 
            this.panelObject.Controls.Add(this.checkEdit1);
            this.panelObject.Controls.Add(this.cbSea);
            this.panelObject.Controls.Add(this.cbLand);
            this.panelObject.Controls.Add(this.cbSky);
            this.panelObject.Controls.Add(this.cbSatelliteBeam);
            this.panelObject.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelObject.Location = new System.Drawing.Point(2, 2);
            this.panelObject.Name = "panelObject";
            this.panelObject.Size = new System.Drawing.Size(247, 139);
            this.panelObject.TabIndex = 0;
            // 
            // cbSea
            // 
            this.cbSea.EditValue = true;
            this.cbSea.Location = new System.Drawing.Point(17, 84);
            this.cbSea.Name = "cbSea";
            this.cbSea.Properties.Caption = "海洋态势图层";
            this.cbSea.Size = new System.Drawing.Size(107, 19);
            this.cbSea.TabIndex = 3;
            this.cbSea.Tag = "海洋态势图层";
            this.cbSea.CheckedChanged += new System.EventHandler(this.cbSatelliteBeam_CheckedChanged);
            // 
            // cbLand
            // 
            this.cbLand.EditValue = true;
            this.cbLand.Location = new System.Drawing.Point(17, 58);
            this.cbLand.Name = "cbLand";
            this.cbLand.Properties.Caption = "陆地态势图层";
            this.cbLand.Size = new System.Drawing.Size(107, 19);
            this.cbLand.TabIndex = 2;
            this.cbLand.Tag = "陆地态势图层";
            this.cbLand.CheckedChanged += new System.EventHandler(this.cbSatelliteBeam_CheckedChanged);
            // 
            // cbSky
            // 
            this.cbSky.EditValue = true;
            this.cbSky.Location = new System.Drawing.Point(17, 32);
            this.cbSky.Name = "cbSky";
            this.cbSky.Properties.Caption = "天空态势图层";
            this.cbSky.Size = new System.Drawing.Size(107, 19);
            this.cbSky.TabIndex = 1;
            this.cbSky.Tag = "天空态势图层";
            this.cbSky.CheckedChanged += new System.EventHandler(this.cbSatelliteBeam_CheckedChanged);
            // 
            // cbSatelliteBeam
            // 
            this.cbSatelliteBeam.EditValue = true;
            this.cbSatelliteBeam.Location = new System.Drawing.Point(17, 6);
            this.cbSatelliteBeam.Name = "cbSatelliteBeam";
            this.cbSatelliteBeam.Properties.Caption = "卫星波束图层";
            this.cbSatelliteBeam.Size = new System.Drawing.Size(107, 19);
            this.cbSatelliteBeam.TabIndex = 0;
            this.cbSatelliteBeam.Tag = "卫星波束图层";
            this.cbSatelliteBeam.CheckedChanged += new System.EventHandler(this.cbSatelliteBeam_CheckedChanged);
            // 
            // checkEdit1
            // 
            this.checkEdit1.EditValue = true;
            this.checkEdit1.Location = new System.Drawing.Point(17, 109);
            this.checkEdit1.Name = "checkEdit1";
            this.checkEdit1.Properties.Caption = "预警区域图层";
            this.checkEdit1.Size = new System.Drawing.Size(107, 19);
            this.checkEdit1.TabIndex = 4;
            this.checkEdit1.Tag = "warnLayer";
            this.checkEdit1.CheckedChanged += new System.EventHandler(this.cbSatelliteBeam_CheckedChanged);
            // 
            // DisplayMgrControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl4);
            this.Controls.Add(this.tpArea);
            this.Controls.Add(this.tpMap);
            this.Controls.Add(this.tpObject);
            this.Name = "DisplayMgrControl";
            this.Size = new System.Drawing.Size(251, 581);
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpObject)).EndInit();
            this.tpObject.ResumeLayout(false);
            this.tpObject.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tpMap)).EndInit();
            this.tpMap.ResumeLayout(false);
            this.tpMap.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tpArea)).EndInit();
            this.tpArea.ResumeLayout(false);
            this.tpArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
            this.panelControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelMap)).EndInit();
            this.panelMap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelObject)).EndInit();
            this.panelObject.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbSea.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbLand.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbSky.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbSatelliteBeam.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl tpObject;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton sbObject;
        private DevExpress.XtraEditors.PanelControl tpMap;
        private DevExpress.XtraEditors.SimpleButton sbMap;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.PanelControl tpArea;
        private DevExpress.XtraEditors.SimpleButton sbArea;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.PanelControl panelControl4;
        private DevExpress.XtraEditors.PanelControl panelMap;
        private DevExpress.XtraEditors.PanelControl panelArea;
        private DevExpress.XtraEditors.PanelControl panelObject;
        private ESRI.ArcGIS.Controls.AxTOCControl axTOCControl1;
        private DevExpress.XtraEditors.CheckEdit cbSatelliteBeam;
        private DevExpress.XtraEditors.CheckEdit cbSea;
        private DevExpress.XtraEditors.CheckEdit cbLand;
        private DevExpress.XtraEditors.CheckEdit cbSky;
        private DevExpress.XtraEditors.CheckEdit checkEdit1;
    }
}
