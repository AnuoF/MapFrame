using System;
using DevExpress.XtraEditors;
using System.IO;

namespace GlobleSituation.UI
{
    public partial class MapControl : XtraUserControl
    {
        public MapControl()
        {
            InitializeComponent();

            LoadMap();
        }

        // 加载地图
        private void LoadMap()
        {
            string arcMapFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps\\world\\World Map.mxd");
            if (axMapControl1.CheckMxFile(arcMapFile))
            {
                axMapControl1.LoadMxFile(arcMapFile);
            }
        }
    }
}
