using DevExpress.XtraEditors;
using System;
using System.Threading;
using System.Windows.Forms;

namespace GlobleSituation.UI
{
    public partial class frmSplash : XtraForm
    {
        public frmSplash()
        {
            InitializeComponent();
        }

        private void KillMe(object sender,EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 加载显示主窗体
        /// </summary>
        /// <param name="_frmMain">主窗体</param>
        public static void LoadAndRun(frmMain _frmMain)
        {
            // 订阅主窗体的句柄创建事件
            _frmMain.HandleCreated += delegate
              {
                  // 启动新线程来显示Splash窗体
                  new Thread(new ThreadStart(delegate
                  {
                      frmSplash splash = new frmSplash();
                      
                      // 订阅主窗体的Shown事件
                      _frmMain.Shown += delegate
                        {
                        // 通知Splash窗体关闭自身
                        splash.Invoke(new EventHandler(splash.KillMe));
                            splash.Dispose();
                        };
                      // 显示splash窗体
                      Application.Run(splash);
                  })).Start();
              };
            // 显示主窗体
            Application.Run(_frmMain);
        }
    }
}
