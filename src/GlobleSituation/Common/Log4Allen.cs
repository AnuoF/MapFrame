/**************************************************************
 * 类名：Log4AE.cs
 * 描述：日志类，用于记录错误、异常、调试日志
 * 创建者：Allen
 * 时间：Jan 9,2016
 * 
 * ***********************************************************/

using log4net.Config;
using System;
//[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace GlobleSituation.Common
{
    /// <summary>
    /// 日志类
    /// </summary>
    public class Log4Allen
    {
        /// <summary>
        /// 初始化日志
        /// </summary>
        public static void InitLog()
        {
            XmlConfigurator.ConfigureAndWatch(
                new System.IO.FileInfo("Config\\log4net.config"));
        }

        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        #region static void WriteLog(Type t, Exception ex)

        public static void WriteLog(Type t, Exception ex)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error("Error", ex);
        }

        #endregion

        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        #region static void WriteLog(Type t, string msg)

        public static void WriteLog(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error(msg);
        }

        #endregion

        /// <summary>
        /// 输出日志到界面显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        public static void WriteLog2UI(object sender,string msg)
        {
            //EventPublisher.PublishWriteLogEvent(sender, msg);
        }
    }
}


