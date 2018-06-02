
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobleSituation.Model
{
    public class ShowModeChangedEventArgs:EventArgs
    {
        /// <summary>
        /// 展示方式
        /// </summary>
        public ShowMode Mode { get; set; }
    }
}
