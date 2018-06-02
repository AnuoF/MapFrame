/**************************************************************************
 * 类名：TextInput.cs
 * 描述：文字输入工具
 * 作者：CJ
 * 日期：2016年9月8日
 * 
 * ************************************************************************/
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MapFrame.ArcMap.Windows
{
    /// <summary>
    /// 文字输入工具
    /// </summary>
    public partial class TextInput : UserControl
    {
        /// <summary>
        /// 文字输入控件
        /// </summary>
        /// <param name="context">文字内容</param>
        /// <param name="font">文字字体</param>
        /// <param name="color">字体颜色</param>
        /// <param name="esc">是否取消编辑</param>
        public delegate void InputFinishedDelegate(string context, Font font, Color color, bool esc);
        /// <summary>
        /// 文字输入委托方法
        /// </summary>
        public InputFinishedDelegate InputFinished;
        /// <summary>
        /// 字体默认颜色
        /// </summary>
        private Color color = System.Drawing.Color.Black;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TextInput()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 控件加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextInput_Load(object sender, EventArgs e)
        {
            txtInput.Focus();
        }

        /// <summary>
        /// 字体设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = viewLable.Font;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                viewLable.Font = fontDialog1.Font;
                txtInput.Focus();
            }
        }

        /// <summary>
        /// 键盘按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string context = txtInput.Text;
                this.Dispose();
                InputFinished(context, viewLable.Font, color, false);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Dispose();
                InputFinished(null, viewLable.Font, color, true);
            }
        }

        /// <summary>
        /// 设置控件内容
        /// </summary>
        /// <param name="context"></param>
        public void SetText(string context)
        {
            txtInput.Text = context;
            viewLable.Text = context;
        }

        /// <summary>
        /// 设置控件内容的颜色
        /// </summary>
        /// <param name="c"></param>
        public void SetColor(Color c)
        {
            viewLable.ForeColor = c;
            color = c;
        }

        /// <summary>
        /// 设置字体颜色
        /// </summary>
        /// <param name="font"></param>
        public void SetFont(Font font)
        {
            viewLable.Font = font;
        }

        /// <summary>
        /// 设置字体颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pboxColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDia = new ColorDialog();
            if (colorDia.ShowDialog() == DialogResult.OK)
            {
                SetColor(colorDia.Color);
            }
        }

        /// <summary>
        /// 写字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            viewLable.Text = txtInput.Text.Trim();
        }
    }
}
