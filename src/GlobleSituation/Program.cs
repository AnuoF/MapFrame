using GlobleSituation.UI;
using System;
using System.Threading;
using System.Windows.Forms;

namespace GlobleSituation
{
    static class Program
    {
        private static Mutex mutex;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args != null && args.Length == 1)
            {
                if (args[0] == "Restart")
                {
                    Thread.Sleep(2000); //重启软件时，等待2秒，等上一个进程结束资源。
                }
                else if (args[0] == "StartUpdate")
                {
                    Thread.Sleep(1000);
                }
            }

            DevExpress.UserSkins.OfficeSkins.Register();
            DevExpress.Skins.SkinManager.EnableFormSkins();
            //进程同步
            bool isOwned = false;
            string name = "Global\\" + Application.StartupPath.Replace('\\', '-');  //Global\\表示全局使用Mutex，在多用户下也有效
            mutex = new Mutex(true, name, out isOwned); //名称不能使用文件路径
            if (!isOwned)
            {
                MessageBox.Show("应用程序【" + Application.ExecutablePath + "】已经启动，不能重复启动！", "错误");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new frmMain());
            frmSplash.LoadAndRun(new frmMain());
        }
    }
}
