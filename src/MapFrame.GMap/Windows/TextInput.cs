/**************************************************************************
 * 类名：TextInput.cs
 * 描述：标牌的工具控件
 * 作者：LX
 * 日期：2016年8月8日
 * 
 * ************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace MapFrame.GMap.Windows
{
    /// <summary>
    /// 文字输入控件
    /// </summary>
    public partial class TextInput : UserControl
    {
        /// <summary>
        /// 文字输入完成委托
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
        private Color color = Color.Black;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TextInput()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 控件记载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextInput_Load(object sender, EventArgs e)
        {
            txtInput.Focus();
        }

        /// <summary>
        /// 设置输入文字焦点
        /// </summary>
        public void SetTextFocucs()
        {
            txtInput.Focus();
        }

        // 字体设置
        private void btnFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            fd.Font = label1.Font;//字体选择框的字体格式
            if (fd.ShowDialog() == DialogResult.OK)
            {
                label1.Font = fd.Font;
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
            if (e.KeyCode == Keys.Enter)   // 完成编辑
            {
                string context = txtInput.Text;
                txtInput.Text = "";
                this.Hide();
                InputFinished(context, label1.Font, color, false);
            }
            else if (e.KeyCode == Keys.Escape)    // 取消编辑
            {
                this.Hide();
                InputFinished(null, label1.Font, color, true);
            }
        }

        /// <summary>
        /// 设置控件内容
        /// </summary>
        /// <param name="context"></param>
        public void SetText(string context)
        {
            txtInput.Text = context;
            label1.Text = context;
        }

        /// <summary>
        /// 设置控件内容的颜色
        /// </summary>
        /// <param name="c"></param>
        public void SetColor(Color c)
        {
            label1.ForeColor = c;
            color = c;
        }

        /// <summary>
        /// 设置控件的文本格式
        /// </summary>
        /// <param name="font"></param>
        public void SetFont(Font font)
        {
            label1.Font = font;
        }

        /// <summary>
        /// 设置字体颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pboxColor_Click(object sender, EventArgs e)
        {
            ColorDialog dia = new ColorDialog();
            if (dia.ShowDialog() == DialogResult.OK)
            {
                SetColor(dia.Color);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// 写字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            label1.Text = txtInput.Text;
        }
    }
}
