using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapFrame.Mgis.Windows
{
    /// <summary>
    /// 输入文字控件
    /// </summary>
    public partial class TextInput : UserControl
    {
        /// <summary>
        /// 文字输入完成委托
        /// </summary>
        /// <param name="context">文字内容</param>
        public delegate void InputFinishedDelegate(string context,Color color,bool esc);
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

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)   // 完成编辑
            {
                string context = txtInput.Text.Trim();
                this.Dispose();
                InputFinished(context,color,true);
            }
            else if (e.KeyCode == Keys.Escape)    // 取消编辑
            {
                this.Dispose();
                InputFinished(null,color, false);
            }
        }

        private void TextInput_Load(object sender, EventArgs e)
        {
            if (txtInput.InvokeRequired)
            {
                txtInput.Invoke(new Action(delegate
                {
                    txtInput.Focus();

                }));
            }
            else
            {
                txtInput.Focus();
            }
        }

        /// <summary>
        /// 设置输入文字焦点
        /// </summary>
        public void SetTextFocucs()
        {
            txtInput.Focus();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDia = new ColorDialog();
            if (colorDia.ShowDialog() == DialogResult.OK)
            {
                color = colorDia.Color;
            }
        }
    }
}
