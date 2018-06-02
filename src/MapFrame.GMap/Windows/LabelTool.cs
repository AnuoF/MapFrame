/**************************************************************************
 * 类名：TootipTool.cs
 * 描述：标牌的工具控件
 * 作者：Aline
 * 日期：2016年8月8日
 * 
 * ************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;
using MapFrame.GMap.Windows;

namespace MapFrame.GMap
{
    /// <summary>
    /// 标牌工具
    /// </summary>
    public partial class TootipTool : UserControl
    {
        private int transparency = 220;
        /// <summary>
        /// 标牌透明度
        /// </summary>
        public int Transparency 
        {
            get { return transparency; }
            set 
            {
                transparency = value;
                this.trackBar1.Value = value;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public TootipTool()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 透明度改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            transparency = trackBar1.Value;
            this.Parent.BackColor = Color.FromArgb(transparency, this.Parent.BackColor);
        }

        /// <summary>
        /// 关闭标牌
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labClose_Click(object sender, EventArgs e)
        {
            MapLabel tool = this.Parent as MapLabel;
            tool.DisposeLabel();
        }

        /// <summary>
        /// 字体颜色设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labFColor_Click(object sender, EventArgs e)
        {
            ColorDialog dia = new ColorDialog();
            MapLabel tool = this.Parent as MapLabel;
            if (dia.ShowDialog() == DialogResult.OK)
            {
                tool.SetFontColor(dia.Color);
            }
        }

        /// <summary>
        /// 背景色设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labBColor_Click(object sender, EventArgs e)
        {
            ColorDialog dia = new ColorDialog();
            if (dia.ShowDialog() == DialogResult.OK)
            {
                this.Parent.BackColor = Color.FromArgb(transparency, dia.Color);
            }
        }

        /// <summary>
        /// 鼠标移上之后显示效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labFColor_MouseHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
    }
}
