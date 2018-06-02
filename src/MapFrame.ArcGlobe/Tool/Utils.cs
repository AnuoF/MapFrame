using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.ArcGlobe.Tool
{
    static class Utils
    {
        private static int index;
        /// <summary>
        /// 索引
        /// </summary>
        public static int Index
        {
            get
            {
                index++;
                return index; 
            }
        }
    }
}
