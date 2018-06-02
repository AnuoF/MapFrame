
using System;
using DevExpress.XtraEditors;
using System.IO;
using GlobleSituation.Business;

namespace GlobleSituation.UI
{
    public partial class AxMapControlEx : XtraUserControl
    {

        private MapFrame.Logic.InitMapFrame mapFrame = null;
        private MapFrame.Core.Interface.IMapLogic mapLogic = null;
        private ArcMapBusiness arcMapBusiness = null;


        public AxMapControlEx()
        {
            InitializeComponent();
            this.Load += new EventHandler(AxMapControlEx_Load);

            ////初始化
            //mapFrame = new MapFrame.Logic.InitMapFrame(axMapControl1, "ArcMap");
            //mapLogic = mapFrame.GetMapLogic();

            //arcMapBusiness = new ArcMapBusiness(mapLogic, axMapControl1);
        }

        void AxMapControlEx_Load(object sender, EventArgs e)
        {
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
