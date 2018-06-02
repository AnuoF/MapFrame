/**************************************************************************
 * 类名：MapSize.cs
 * 描述：Size
 * 作者：Allen
 * 日期：Aug 18,2016
 * 
 * ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.Core.Model
{
    /// <summary>
    /// Size
    /// </summary>
    public class MapSize
    {
        private int width = 0;
        private int height = 0;

        /// <summary>
        /// 宽
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// 高
        /// </summary>
        public int Height 
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_width">宽</param>
        /// <param name="_height">高</param>
        public MapSize(int _width,int _height)
        {
            width = _width;
            height = _height;
        }
    }
}
