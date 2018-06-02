using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;

namespace GlobleSituation.Common
{
    public class AppConfigFacade
    {
        public static string DefaultTheme;


        private static XmlDocument exeConfig;
        private static string exeConfigPath;


        static AppConfigFacade()
        {
            try
            {
                IniExeConfig(); //允许配置文件不存在，供消费者服务使用

                DefaultTheme = GetExeConfig("DefaultTheme");
            }
            catch (Exception ex)
            {
                
            }
        }

        public static void IniExeConfig()
        {
            Configuration c = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            exeConfigPath = c.FilePath;
            exeConfig = new XmlDocument();
            exeConfig.Load(exeConfigPath);
        }

        public static void SaveExeConfig(string key, string value)
        {
            XmlNode node = exeConfig.SelectSingleNode("//appSettings/add[@key='" + key + "']");
            if (node != null)
            {
                ((XmlElement)node).SetAttribute("value", value);
            }
            else
            {
                throw new ArgumentException("在执行文件配置[" + exeConfigPath + "]中，未找到key节点[" + key + "]");
            }
            exeConfig.Save(exeConfigPath);
        }

        public static string GetExeConfig(string key)
        {
            string value = "";

            XmlNode node = exeConfig.SelectSingleNode("//appSettings/add[@key='" + key + "']");
            if (node != null)
            {
                value = node.Attributes["value"].Value.ToString();
            }
            else
            {
                throw new ArgumentException("在执行文件配置[" + exeConfigPath + "]中，未找到key节点[" + key + "]");
            }

            return value;
        }
    }
}
