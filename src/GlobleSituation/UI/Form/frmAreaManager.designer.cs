namespace GlobleSituation.UI
{
    partial class frmAreaManager
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
            DevExpress.XtraGrid.StyleFormatCondition styleFormatCondition1 = new DevExpress.XtraGrid.StyleFormatCondition();
            DevExpress.XtraGrid.StyleFormatCondition styleFormatCondition2 = new DevExpress.XtraGrid.StyleFormatCondition();
            this.gridColumnLongitude = new DevExpress.XtraGrid.Columns.GridColumn();
            this.SpinEditLongitude = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.gridColumnLatitude = new DevExpress.XtraGrid.Columns.GridColumn();
            this.SpinEditLatitude = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.contextMenuStripDelete = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除预警区域DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddArea = new DevExpress.XtraEditors.SimpleButton();
            this.gcPoints = new DevExpress.XtraGrid.GridControl();
            this.gvPointList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnAltitude = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.dxErrorProviderMap = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.toolTipControllerVisible = new DevExpress.Utils.ToolTipController(this.components);
            this.gpAddArea = new DevExpress.XtraEditors.GroupControl();
            this.cmbImportantArea = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.cmbIsWarningArea = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblIsWarningArea = new DevExpress.XtraEditors.LabelControl();
            this.cmbAreaType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblAreaType = new DevExpress.XtraEditors.LabelControl();
            this.txtAreaName = new DevExpress.XtraEditors.TextEdit();
            this.lblAreaName = new DevExpress.XtraEditors.LabelControl();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.gcAreaMangaer = new DevExpress.XtraGrid.GridControl();
            this.gvAreaList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnAreaName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnAreaType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnIsWarningArea = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbAreaIsWarningChanged = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.gridColumnIsVisible = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbAreaVisibleChanged = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.gridColumnColor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemColorEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.btnSavePointChange = new DevExpress.XtraEditors.SimpleButton();
            this.查看统计分析ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.SpinEditLongitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpinEditLatitude)).BeginInit();
            this.contextMenuStripDelete.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPointList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderMap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gpAddArea)).BeginInit();
            this.gpAddArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbImportantArea.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbIsWarningArea.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAreaType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAreaName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcAreaMangaer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAreaList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAreaIsWarningChanged)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAreaVisibleChanged)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridColumnLongitude
            // 
            this.gridColumnLongitude.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnLongitude.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnLongitude.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnLongitude.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnLongitude.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnLongitude.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnLongitude.Caption = "经度";
            this.gridColumnLongitude.ColumnEdit = this.SpinEditLongitude;
            this.gridColumnLongitude.DisplayFormat.FormatString = "n6";
            this.gridColumnLongitude.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnLongitude.FieldName = "Longitude";
            this.gridColumnLongitude.Name = "gridColumnLongitude";
            this.gridColumnLongitude.OptionsColumn.AllowMove = false;
            this.gridColumnLongitude.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnLongitude.Visible = true;
            this.gridColumnLongitude.VisibleIndex = 0;
            // 
            // SpinEditLongitude
            // 
            this.SpinEditLongitude.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.SpinEditLongitude.AutoHeight = false;
            this.SpinEditLongitude.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.SpinEditLongitude.DisplayFormat.FormatString = "n6";
            this.SpinEditLongitude.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.SpinEditLongitude.EditFormat.FormatString = "n6";
            this.SpinEditLongitude.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.SpinEditLongitude.MaxValue = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.SpinEditLongitude.MinValue = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.SpinEditLongitude.Name = "SpinEditLongitude";
            // 
            // gridColumnLatitude
            // 
            this.gridColumnLatitude.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnLatitude.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnLatitude.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnLatitude.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnLatitude.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnLatitude.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnLatitude.Caption = "纬度";
            this.gridColumnLatitude.ColumnEdit = this.SpinEditLatitude;
            this.gridColumnLatitude.DisplayFormat.FormatString = "n6";
            this.gridColumnLatitude.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnLatitude.FieldName = "Latgitude";
            this.gridColumnLatitude.Name = "gridColumnLatitude";
            this.gridColumnLatitude.OptionsColumn.AllowMove = false;
            this.gridColumnLatitude.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnLatitude.Visible = true;
            this.gridColumnLatitude.VisibleIndex = 1;
            // 
            // SpinEditLatitude
            // 
            this.SpinEditLatitude.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.SpinEditLatitude.AutoHeight = false;
            this.SpinEditLatitude.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.SpinEditLatitude.DisplayFormat.FormatString = "n6";
            this.SpinEditLatitude.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.SpinEditLatitude.EditFormat.FormatString = "n6";
            this.SpinEditLatitude.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.SpinEditLatitude.MaxValue = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.SpinEditLatitude.MinValue = new decimal(new int[] {
            90,
            0,
            0,
            -2147483648});
            this.SpinEditLatitude.Name = "SpinEditLatitude";
            // 
            // contextMenuStripDelete
            // 
            this.contextMenuStripDelete.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.查看统计分析ToolStripMenuItem,
            this.toolStripSeparator1,
            this.删除预警区域DToolStripMenuItem});
            this.contextMenuStripDelete.Name = "contextMenuStripDelete";
            this.contextMenuStripDelete.Size = new System.Drawing.Size(153, 76);
            // 
            // 删除预警区域DToolStripMenuItem
            // 
            this.删除预警区域DToolStripMenuItem.Name = "删除预警区域DToolStripMenuItem";
            this.删除预警区域DToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.删除预警区域DToolStripMenuItem.Text = "删除区域(&D)";
            this.删除预警区域DToolStripMenuItem.Click += new System.EventHandler(this.删除预警区域DToolStripMenuItem_Click);
            // 
            // btnAddArea
            // 
            this.btnAddArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddArea.Location = new System.Drawing.Point(428, 27);
            this.btnAddArea.Name = "btnAddArea";
            this.btnAddArea.Size = new System.Drawing.Size(126, 52);
            this.btnAddArea.TabIndex = 6;
            this.btnAddArea.Text = "开始绘制(&A)";
            this.btnAddArea.Click += new System.EventHandler(this.btnAddArea_Click);
            // 
            // gcPoints
            // 
            this.gcPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcPoints.Location = new System.Drawing.Point(2, 21);
            this.gcPoints.MainView = this.gvPointList;
            this.gcPoints.Name = "gcPoints";
            this.gcPoints.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.SpinEditLatitude,
            this.SpinEditLongitude,
            this.repositoryItemSpinEdit1});
            this.gcPoints.Size = new System.Drawing.Size(244, 243);
            this.gcPoints.TabIndex = 8;
            this.gcPoints.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvPointList});
            // 
            // gvPointList
            // 
            this.gvPointList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnLongitude,
            this.gridColumnLatitude,
            this.gridColumnAltitude});
            styleFormatCondition1.ApplyToRow = true;
            styleFormatCondition1.Column = this.gridColumnLongitude;
            styleFormatCondition1.Condition = DevExpress.XtraGrid.FormatConditionEnum.NotBetween;
            styleFormatCondition1.Value1 = "-180";
            styleFormatCondition1.Value2 = "180";
            styleFormatCondition2.ApplyToRow = true;
            styleFormatCondition2.Column = this.gridColumnLatitude;
            styleFormatCondition2.Condition = DevExpress.XtraGrid.FormatConditionEnum.NotBetween;
            styleFormatCondition2.Value1 = "-90";
            styleFormatCondition2.Value2 = "90";
            this.gvPointList.FormatConditions.AddRange(new DevExpress.XtraGrid.StyleFormatCondition[] {
            styleFormatCondition1,
            styleFormatCondition2});
            this.gvPointList.GridControl = this.gcPoints;
            this.gvPointList.IndicatorWidth = 30;
            this.gvPointList.Name = "gvPointList";
            this.gvPointList.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gvPointList.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gvPointList.OptionsCustomization.AllowColumnMoving = false;
            this.gvPointList.OptionsCustomization.AllowColumnResizing = false;
            this.gvPointList.OptionsCustomization.AllowFilter = false;
            this.gvPointList.OptionsCustomization.AllowGroup = false;
            this.gvPointList.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvPointList.OptionsCustomization.AllowSort = false;
            this.gvPointList.OptionsDetail.AllowZoomDetail = false;
            this.gvPointList.OptionsDetail.ShowDetailTabs = false;
            this.gvPointList.OptionsDetail.SmartDetailExpand = false;
            this.gvPointList.OptionsFilter.AllowColumnMRUFilterList = false;
            this.gvPointList.OptionsFilter.AllowFilterEditor = false;
            this.gvPointList.OptionsFind.AllowFindPanel = false;
            this.gvPointList.OptionsMenu.EnableColumnMenu = false;
            this.gvPointList.OptionsMenu.EnableFooterMenu = false;
            this.gvPointList.OptionsMenu.EnableGroupPanelMenu = false;
            this.gvPointList.OptionsMenu.ShowAddNewSummaryItem = DevExpress.Utils.DefaultBoolean.False;
            this.gvPointList.OptionsMenu.ShowAutoFilterRowItem = false;
            this.gvPointList.OptionsMenu.ShowGroupSortSummaryItems = false;
            this.gvPointList.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
            this.gvPointList.OptionsView.ShowGroupPanel = false;
            this.gvPointList.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvPointList_CustomDrawRowIndicator);
            // 
            // gridColumnAltitude
            // 
            this.gridColumnAltitude.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnAltitude.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnAltitude.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnAltitude.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnAltitude.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnAltitude.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnAltitude.Caption = "高度";
            this.gridColumnAltitude.ColumnEdit = this.repositoryItemSpinEdit1;
            this.gridColumnAltitude.DisplayFormat.FormatString = "n6";
            this.gridColumnAltitude.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumnAltitude.FieldName = "Altitude";
            this.gridColumnAltitude.Name = "gridColumnAltitude";
            this.gridColumnAltitude.OptionsColumn.AllowEdit = false;
            this.gridColumnAltitude.Visible = true;
            this.gridColumnAltitude.VisibleIndex = 2;
            // 
            // repositoryItemSpinEdit1
            // 
            this.repositoryItemSpinEdit1.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.repositoryItemSpinEdit1.AutoHeight = false;
            this.repositoryItemSpinEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinEdit1.DisplayFormat.FormatString = "n6";
            this.repositoryItemSpinEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repositoryItemSpinEdit1.EditFormat.FormatString = "n6";
            this.repositoryItemSpinEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repositoryItemSpinEdit1.MinValue = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
            // 
            // dxErrorProviderMap
            // 
            this.dxErrorProviderMap.ContainerControl = this;
            // 
            // gpAddArea
            // 
            this.gpAddArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gpAddArea.Controls.Add(this.cmbImportantArea);
            this.gpAddArea.Controls.Add(this.labelControl1);
            this.gpAddArea.Controls.Add(this.simpleButton1);
            this.gpAddArea.Controls.Add(this.cmbIsWarningArea);
            this.gpAddArea.Controls.Add(this.lblIsWarningArea);
            this.gpAddArea.Controls.Add(this.btnAddArea);
            this.gpAddArea.Controls.Add(this.cmbAreaType);
            this.gpAddArea.Controls.Add(this.lblAreaType);
            this.gpAddArea.Controls.Add(this.txtAreaName);
            this.gpAddArea.Controls.Add(this.lblAreaName);
            this.gpAddArea.Location = new System.Drawing.Point(12, 8);
            this.gpAddArea.Name = "gpAddArea";
            this.gpAddArea.Size = new System.Drawing.Size(676, 84);
            this.gpAddArea.TabIndex = 13;
            this.gpAddArea.Text = "新增区域";
            // 
            // cmbImportantArea
            // 
            this.cmbImportantArea.EditValue = "否";
            this.cmbImportantArea.Location = new System.Drawing.Point(294, 28);
            this.cmbImportantArea.Name = "cmbImportantArea";
            this.cmbImportantArea.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbImportantArea.Properties.Items.AddRange(new object[] {
            "是",
            "否"});
            this.cmbImportantArea.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbImportantArea.Size = new System.Drawing.Size(118, 20);
            this.cmbImportantArea.TabIndex = 23;
            this.cmbImportantArea.ToolTip = "指示目标进入该区域时是否会触发警报\r\n注意：如果该区域为预警区域但不可见，目标进入该区域时不会发出警报！";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(228, 31);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(60, 14);
            this.labelControl1.TabIndex = 22;
            this.labelControl1.Text = "重要区域：";
            // 
            // simpleButton1
            // 
            this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton1.Location = new System.Drawing.Point(560, 27);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(111, 52);
            this.simpleButton1.TabIndex = 21;
            this.simpleButton1.Text = "关    闭";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // cmbIsWarningArea
            // 
            this.cmbIsWarningArea.EditValue = "否";
            this.cmbIsWarningArea.Location = new System.Drawing.Point(71, 58);
            this.cmbIsWarningArea.Name = "cmbIsWarningArea";
            this.cmbIsWarningArea.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbIsWarningArea.Properties.Items.AddRange(new object[] {
            "是",
            "否"});
            this.cmbIsWarningArea.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbIsWarningArea.Size = new System.Drawing.Size(118, 20);
            this.cmbIsWarningArea.TabIndex = 20;
            this.cmbIsWarningArea.ToolTip = "指示目标进入该区域时是否会触发警报\r\n注意：如果该区域为预警区域但不可见，目标进入该区域时不会发出警报！";
            // 
            // lblIsWarningArea
            // 
            this.lblIsWarningArea.Location = new System.Drawing.Point(5, 61);
            this.lblIsWarningArea.Name = "lblIsWarningArea";
            this.lblIsWarningArea.Size = new System.Drawing.Size(60, 14);
            this.lblIsWarningArea.TabIndex = 19;
            this.lblIsWarningArea.Text = "区域预警：";
            // 
            // cmbAreaType
            // 
            this.cmbAreaType.EditValue = "多边形";
            this.cmbAreaType.Location = new System.Drawing.Point(294, 58);
            this.cmbAreaType.Name = "cmbAreaType";
            this.cmbAreaType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbAreaType.Properties.Items.AddRange(new object[] {
            "多边形",
            "矩形"});
            this.cmbAreaType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbAreaType.Size = new System.Drawing.Size(118, 20);
            this.cmbAreaType.TabIndex = 16;
            // 
            // lblAreaType
            // 
            this.lblAreaType.Location = new System.Drawing.Point(228, 61);
            this.lblAreaType.Name = "lblAreaType";
            this.lblAreaType.Size = new System.Drawing.Size(60, 14);
            this.lblAreaType.TabIndex = 15;
            this.lblAreaType.Text = "区域类型：";
            // 
            // txtAreaName
            // 
            this.txtAreaName.Location = new System.Drawing.Point(71, 28);
            this.txtAreaName.Name = "txtAreaName";
            this.txtAreaName.Size = new System.Drawing.Size(118, 20);
            this.txtAreaName.TabIndex = 14;
            this.txtAreaName.ToolTip = "注意：区域名称必须保证唯一！";
            this.txtAreaName.EditValueChanged += new System.EventHandler(this.txtAreaName_EditValueChanged);
            // 
            // lblAreaName
            // 
            this.lblAreaName.Location = new System.Drawing.Point(5, 31);
            this.lblAreaName.Name = "lblAreaName";
            this.lblAreaName.Size = new System.Drawing.Size(60, 14);
            this.lblAreaName.TabIndex = 13;
            this.lblAreaName.Text = "区域名称：";
            // 
            // groupControl1
            // 
            this.groupControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupControl1.Controls.Add(this.gcAreaMangaer);
            this.groupControl1.Location = new System.Drawing.Point(12, 98);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(422, 305);
            this.groupControl1.TabIndex = 21;
            this.groupControl1.Text = "区域列表    【提示：选中后点击鼠标右键可删除】";
            // 
            // gcAreaMangaer
            // 
            this.gcAreaMangaer.ContextMenuStrip = this.contextMenuStripDelete;
            this.gcAreaMangaer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcAreaMangaer.Location = new System.Drawing.Point(2, 21);
            this.gcAreaMangaer.MainView = this.gvAreaList;
            this.gcAreaMangaer.Name = "gcAreaMangaer";
            this.gcAreaMangaer.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.cmbAreaVisibleChanged,
            this.cmbAreaIsWarningChanged,
            this.repositoryItemColorEdit1,
            this.repositoryItemComboBox1});
            this.gcAreaMangaer.Size = new System.Drawing.Size(418, 282);
            this.gcAreaMangaer.TabIndex = 1;
            this.gcAreaMangaer.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvAreaList});
            // 
            // gvAreaList
            // 
            this.gvAreaList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnAreaName,
            this.gridColumnAreaType,
            this.gridColumnIsWarningArea,
            this.gridColumn1,
            this.gridColumnIsVisible,
            this.gridColumnColor});
            this.gvAreaList.GridControl = this.gcAreaMangaer;
            this.gvAreaList.IndicatorWidth = 30;
            this.gvAreaList.Name = "gvAreaList";
            this.gvAreaList.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gvAreaList.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
            this.gvAreaList.OptionsCustomization.AllowColumnMoving = false;
            this.gvAreaList.OptionsCustomization.AllowColumnResizing = false;
            this.gvAreaList.OptionsCustomization.AllowFilter = false;
            this.gvAreaList.OptionsCustomization.AllowGroup = false;
            this.gvAreaList.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvAreaList.OptionsCustomization.AllowSort = false;
            this.gvAreaList.OptionsDetail.ShowDetailTabs = false;
            this.gvAreaList.OptionsDetail.SmartDetailExpand = false;
            this.gvAreaList.OptionsFilter.AllowColumnMRUFilterList = false;
            this.gvAreaList.OptionsFilter.AllowFilterEditor = false;
            this.gvAreaList.OptionsFind.AllowFindPanel = false;
            this.gvAreaList.OptionsMenu.EnableColumnMenu = false;
            this.gvAreaList.OptionsMenu.EnableFooterMenu = false;
            this.gvAreaList.OptionsMenu.EnableGroupPanelMenu = false;
            this.gvAreaList.OptionsMenu.ShowAddNewSummaryItem = DevExpress.Utils.DefaultBoolean.False;
            this.gvAreaList.OptionsMenu.ShowAutoFilterRowItem = false;
            this.gvAreaList.OptionsMenu.ShowGroupSortSummaryItems = false;
            this.gvAreaList.OptionsNavigation.AutoFocusNewRow = true;
            this.gvAreaList.OptionsView.ShowDetailButtons = false;
            this.gvAreaList.OptionsView.ShowGroupPanel = false;
            this.gvAreaList.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvAreaList_CustomDrawRowIndicator);
            this.gvAreaList.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvAreaList_FocusedRowChanged);
            this.gvAreaList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gvAreaList_KeyDown);
            // 
            // gridColumnAreaName
            // 
            this.gridColumnAreaName.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnAreaName.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnAreaName.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnAreaName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnAreaName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnAreaName.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnAreaName.Caption = "名称";
            this.gridColumnAreaName.FieldName = "areaName";
            this.gridColumnAreaName.Name = "gridColumnAreaName";
            this.gridColumnAreaName.OptionsColumn.AllowEdit = false;
            this.gridColumnAreaName.OptionsColumn.AllowMove = false;
            this.gridColumnAreaName.Visible = true;
            this.gridColumnAreaName.VisibleIndex = 0;
            this.gridColumnAreaName.Width = 85;
            // 
            // gridColumnAreaType
            // 
            this.gridColumnAreaType.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnAreaType.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnAreaType.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnAreaType.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnAreaType.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnAreaType.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnAreaType.Caption = "类型";
            this.gridColumnAreaType.FieldName = "Type";
            this.gridColumnAreaType.MaxWidth = 80;
            this.gridColumnAreaType.MinWidth = 80;
            this.gridColumnAreaType.Name = "gridColumnAreaType";
            this.gridColumnAreaType.OptionsColumn.AllowEdit = false;
            this.gridColumnAreaType.OptionsColumn.AllowMove = false;
            this.gridColumnAreaType.Visible = true;
            this.gridColumnAreaType.VisibleIndex = 1;
            this.gridColumnAreaType.Width = 80;
            // 
            // gridColumnIsWarningArea
            // 
            this.gridColumnIsWarningArea.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnIsWarningArea.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnIsWarningArea.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnIsWarningArea.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnIsWarningArea.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnIsWarningArea.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnIsWarningArea.Caption = "预警";
            this.gridColumnIsWarningArea.ColumnEdit = this.cmbAreaIsWarningChanged;
            this.gridColumnIsWarningArea.FieldName = "IsWarningArea";
            this.gridColumnIsWarningArea.MaxWidth = 50;
            this.gridColumnIsWarningArea.MinWidth = 50;
            this.gridColumnIsWarningArea.Name = "gridColumnIsWarningArea";
            this.gridColumnIsWarningArea.Visible = true;
            this.gridColumnIsWarningArea.VisibleIndex = 2;
            this.gridColumnIsWarningArea.Width = 50;
            // 
            // cmbAreaIsWarningChanged
            // 
            this.cmbAreaIsWarningChanged.AutoHeight = false;
            this.cmbAreaIsWarningChanged.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbAreaIsWarningChanged.Items.AddRange(new object[] {
            "是",
            "否"});
            this.cmbAreaIsWarningChanged.Name = "cmbAreaIsWarningChanged";
            this.cmbAreaIsWarningChanged.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbAreaIsWarningChanged.SelectedIndexChanged += new System.EventHandler(this.cmbAreaIsWarningChanged_SelectedIndexChanged);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumn1.Caption = "重点区域";
            this.gridColumn1.ColumnEdit = this.repositoryItemComboBox1;
            this.gridColumn1.FieldName = "IsImportant";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 5;
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Items.AddRange(new object[] {
            "是",
            "否"});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            this.repositoryItemComboBox1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.repositoryItemComboBox1.SelectedIndexChanged += new System.EventHandler(this.repositoryItemComboBox1_SelectedIndexChanged);
            // 
            // gridColumnIsVisible
            // 
            this.gridColumnIsVisible.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnIsVisible.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnIsVisible.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnIsVisible.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnIsVisible.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnIsVisible.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridColumnIsVisible.Caption = "显示";
            this.gridColumnIsVisible.ColumnEdit = this.cmbAreaVisibleChanged;
            this.gridColumnIsVisible.FieldName = "IsVisible";
            this.gridColumnIsVisible.MaxWidth = 50;
            this.gridColumnIsVisible.MinWidth = 50;
            this.gridColumnIsVisible.Name = "gridColumnIsVisible";
            this.gridColumnIsVisible.Visible = true;
            this.gridColumnIsVisible.VisibleIndex = 3;
            this.gridColumnIsVisible.Width = 50;
            // 
            // cmbAreaVisibleChanged
            // 
            this.cmbAreaVisibleChanged.AutoHeight = false;
            this.cmbAreaVisibleChanged.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbAreaVisibleChanged.Items.AddRange(new object[] {
            "是",
            "否"});
            this.cmbAreaVisibleChanged.Name = "cmbAreaVisibleChanged";
            this.cmbAreaVisibleChanged.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbAreaVisibleChanged.SelectedIndexChanged += new System.EventHandler(this.cmbAreaVisibleChanged_SelectedIndexChanged);
            // 
            // gridColumnColor
            // 
            this.gridColumnColor.Caption = "颜色";
            this.gridColumnColor.ColumnEdit = this.repositoryItemColorEdit1;
            this.gridColumnColor.FieldName = "Color";
            this.gridColumnColor.MaxWidth = 50;
            this.gridColumnColor.MinWidth = 50;
            this.gridColumnColor.Name = "gridColumnColor";
            this.gridColumnColor.Visible = true;
            this.gridColumnColor.VisibleIndex = 4;
            this.gridColumnColor.Width = 50;
            // 
            // repositoryItemColorEdit1
            // 
            this.repositoryItemColorEdit1.AutoHeight = false;
            this.repositoryItemColorEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemColorEdit1.CustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.Blue,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty,
        System.Drawing.Color.Empty};
            this.repositoryItemColorEdit1.Name = "repositoryItemColorEdit1";
            this.repositoryItemColorEdit1.EditValueChanged += new System.EventHandler(this.repositoryItemColorEdit1_EditValueChanged);
            // 
            // groupControl2
            // 
            this.groupControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupControl2.Controls.Add(this.gcPoints);
            this.groupControl2.Controls.Add(this.btnSavePointChange);
            this.groupControl2.Location = new System.Drawing.Point(440, 98);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(248, 305);
            this.groupControl2.TabIndex = 22;
            this.groupControl2.Text = "区域端点列表";
            // 
            // btnSavePointChange
            // 
            this.btnSavePointChange.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSavePointChange.Location = new System.Drawing.Point(2, 264);
            this.btnSavePointChange.Name = "btnSavePointChange";
            this.btnSavePointChange.Size = new System.Drawing.Size(244, 39);
            this.btnSavePointChange.TabIndex = 9;
            this.btnSavePointChange.Text = "保存所选区域端点修改(&P)";
            this.btnSavePointChange.Click += new System.EventHandler(this.btnSaveChange_Click);
            // 
            // 查看统计分析ToolStripMenuItem
            // 
            this.查看统计分析ToolStripMenuItem.Name = "查看统计分析ToolStripMenuItem";
            this.查看统计分析ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.查看统计分析ToolStripMenuItem.Text = "查看统计分析";
            this.查看统计分析ToolStripMenuItem.Click += new System.EventHandler(this.查看统计分析ToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // frmAreaManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 407);
            this.Controls.Add(this.gpAddArea);
            this.Controls.Add(this.groupControl2);
            this.Controls.Add(this.groupControl1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(700, 445);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(634, 407);
            this.Name = "frmAreaManager";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "区域管理";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAreaManager_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.SpinEditLongitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpinEditLatitude)).EndInit();
            this.contextMenuStripDelete.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPointList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderMap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gpAddArea)).EndInit();
            this.gpAddArea.ResumeLayout(false);
            this.gpAddArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbImportantArea.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbIsWarningArea.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAreaType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAreaName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcAreaMangaer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAreaList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAreaIsWarningChanged)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAreaVisibleChanged)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnAddArea;
        private DevExpress.XtraGrid.GridControl gcPoints;
        private DevExpress.XtraGrid.Views.Grid.GridView gvPointList;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLongitude;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit SpinEditLongitude;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLatitude;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit SpinEditLatitude;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProviderMap;
        private DevExpress.Utils.ToolTipController toolTipControllerVisible;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripDelete;
        private System.Windows.Forms.ToolStripMenuItem 删除预警区域DToolStripMenuItem;
        private DevExpress.XtraEditors.GroupControl gpAddArea;
        private DevExpress.XtraEditors.ComboBoxEdit cmbIsWarningArea;
        private DevExpress.XtraEditors.LabelControl lblIsWarningArea;
        private DevExpress.XtraEditors.ComboBoxEdit cmbAreaType;
        private DevExpress.XtraEditors.LabelControl lblAreaType;
        private DevExpress.XtraEditors.TextEdit txtAreaName;
        private DevExpress.XtraEditors.LabelControl lblAreaName;
        private DevExpress.XtraEditors.SimpleButton btnSavePointChange;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraGrid.GridControl gcAreaMangaer;
        private DevExpress.XtraGrid.Views.Grid.GridView gvAreaList;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnAreaName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnAreaType;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbAreaVisibleChanged;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnIsWarningArea;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbAreaIsWarningChanged;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnAltitude;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnIsVisible;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnColor;
        private DevExpress.XtraEditors.Repository.RepositoryItemColorEdit repositoryItemColorEdit1;
        private DevExpress.XtraEditors.ComboBoxEdit cmbImportantArea;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
        private System.Windows.Forms.ToolStripMenuItem 查看统计分析ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}