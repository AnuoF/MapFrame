/*****************************************************
** 文件名：SessionIdConstructor.cs
** 版 本：1.0
** 内容简述：会话ID生成器
** 创建日期：Mar 10,2014
** 创建人：王喜进 
** 修改记录：
*****************************************************/

using System;
using System.Linq;
using System.Xml.XPath;
using System.Xml.Linq;
using System.IO;

namespace MgisTilesImportTool
{
    /// <summary>
    /// 会话ID生成器
    /// </summary>
    class SessionIdConstructor
    {
        /// <summary>
        /// 会话编号互斥锁
        /// </summary>
        private static object synCmdNo = new object();
        /// <summary>
        /// 会话编号
        /// </summary>
        private static uint sessionNo = 0;
        /// <summary>
        /// Xml文件路径
        /// </summary>
        private static string exeConfigFile = string.Empty;
        /// <summary>
        /// 服务实例，注意该变量只能在所有其它变量之后调用
        /// </summary>
        //private static readonly SessionIdConstructor instance = new SessionIdConstructor();

        /// <summary>
        /// @function:
        /// @author：王喜进
        /// </summary>
        /// <remarks>
        /// 主要思路：
        ///  获取CommandID,读取配置文件
        /// 调用方法：
        ///  
        /// 起始日期：2012/01/10 9:40--2012/1/10 10:45 
        /// 修改日期：        修改内容：      修改人：
        private SessionIdConstructor()
        {
            //try
            //{
            //    exeConfigFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //    exeConfigFile += @"\MapPlugin\Config\SessionID.xml";
            //    if (!File.Exists(exeConfigFile))
            //        return;

            //    XElement xmlRoot = XElement.Load(exeConfigFile);
            //    var query = from item in xmlRoot.XPathSelectElements(@"./Configuration/add")
            //                where item.Attribute("key").Value == "SessionID"
            //                select item;

            //    sessionNo = uint.Parse(query.First().Attribute("value").Value);
            //}
            //catch (Exception ex)
            //{
            //    string strErr = string.Format("读取指令编号配置出错，错误信息：{0}", ex.Message);
            //    sessionNo = 0;
            //}
        }

        /// <summary>
        /// 产生指令号
        /// </summary>
        /// <returns>大于0表示成功</returns>
        public static uint GetCommandID()
        {
            uint nRet = 0;
            lock (synCmdNo)
            {
                nRet = (++sessionNo) % uint.MaxValue;
            }
            return nRet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool InitCmdID()
        {
            return true;
        }

        /// <summary>
        /// @function:
        /// @author：王喜进
        /// </summary>
        /// <remarks>
        /// 主要思路：
        ///  获取CommandID,保存配置文件
        /// 调用方法：
        ///  
        /// 起始日期：2012/01/10 9:40--2012/1/10 10:45 
        /// 修改日期：        修改内容：      修改人：
        ~SessionIdConstructor()
        {
            //lock (synCmdNo)
            //{
            //    try
            //    {
            //        if (File.Exists(exeConfigFile))
            //        {
            //            File.Delete(exeConfigFile);
            //        }

            //        XDocument xdoc = new XDocument(new XDeclaration("1.0", "uft-8", ""),
            //            new XElement("Configurations",
            //                new XElement("Configuration",
            //        new XElement("add", new XAttribute("key", "SessionID"),
            //            new XAttribute("value", sessionNo.ToString())))));
            //        xdoc.Save(exeConfigFile);
            //    }
            //    catch
            //    {

            //    }
            //}
        }
    }
}
