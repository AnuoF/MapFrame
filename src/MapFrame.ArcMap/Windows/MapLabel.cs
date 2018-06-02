/**************************************************************************
 * 类名：ToolTipEx.cs
 * 描述：标牌控件
 * 作者：CJ
 * 日期：2016年8月8日
 * 
 * ************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using MapFrame.Core.Model;

namespace MapFrame.ArcMap.Windows
{
    /// <summary>
    ///  标牌
    /// </summary>
    public partial class MapLabel : UserControl
    {   
        /// <summary>
        /// 标牌内容（主体）
        /// </summary>
        private string context;
        /// <summary>
        /// 目标信息（头）
        /// </summary>
        private string targetInfo;
        /// <summary>
        /// 标牌内容（全部）
        /// </summary>
        private string labelText;
        /// <summary>
        /// 坐标X偏移量
        /// </summary>
        private int offset_x = 0;
        /// <summary>
        /// 坐标Y偏移量
        /// </summary>
        private int offset_y = 0;
        /// <summary>
        /// 鼠标在标牌中的位置
        /// </summary>
        private Point mouseOff;
        /// <summary>
        /// 鼠标左键是否按下
        /// </summary>
        private bool bLeftBtnDown = false;
        /// <summary>
        /// 目标点相对于屏幕坐标的位置
        /// </summary>
        private Point parentPoint;
        /// <summary>
        /// 标牌的Xposition
        /// </summary>
        public volatile int LocalPositionX;
        /// <summary>
        /// 标牌的Xposition
        /// </summary>
        public volatile int LocalPositionY;
        /// <summary>
        /// 移动标牌之间记录差值
        /// </summary>
        private Point offSetPoint;
        /// <summary>
        /// 标牌被关闭事件
        /// </summary>
        public event EventHandler ClosedLabelEvent;
        /// <summary>
        /// 标牌位置
        /// </summary>
        public Point LabelLocation;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_parentPoint">目标点位置</param>
        public MapLabel(Point _parentPoint)
        {
            InitializeComponent();
            parentPoint = _parentPoint;
            offset_x = offset_y = 30;   // 初始化时，偏移量=30
            Point location = new Point(_parentPoint.X+30,_parentPoint.Y+30);
            this.LabelLocation = location;
            //this.Location = location;
        }

        /// <summary>
        /// 释放该标牌
        /// </summary>
        public void DisposeLabel()
        {
            if (IsDisposed) return;

            if (ClosedLabelEvent != null)
                ClosedLabelEvent(this, null);

            Control parent = this.Parent;
            if (parent != null)
            {
                parent.Controls.Remove(this);
            }
        }

        #region 标牌移动


        private void ToolTipEx_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y);
                bLeftBtnDown = true;
                offSetPoint = e.Location;
            }

        }

        private void ToolTipEx_MouseMove(object sender, MouseEventArgs e)
        {
            if (bLeftBtnDown)
            {
                // 更改标牌位置
                Control parent = this.Parent;
                Point point = parent.PointToClient(Cursor.Position);
                point.Offset(mouseOff);
                this.Location = point;

                offset_x = this.Location.X - (int)parentPoint.X;
                offset_y = this.Location.Y - (int)parentPoint.Y;

                int x = e.Location.X - offSetPoint.X;
                int y = e.Location.Y - offSetPoint.Y;
                System.Diagnostics.Debug.WriteLine(string.Format("X:{0}Y:{1}", x, y));
                this.LocalPositionX += x;
                this.LocalPositionY += y;
                parent.Refresh();
            }
        }

        private void ToolTipEx_MouseUp(object sender, MouseEventArgs e)
        {
            bLeftBtnDown = false;
        }

        /// <summary>
        /// 更新标牌的位置
        /// </summary>
        /// <param name="targetPoint">目标位置</param>
        /// <param name="position">目标点位置</param>
        public void ShowLable(Point targetPoint, Point position) 
        {
            Point tipPoint = new Point(targetPoint.X + offset_x, targetPoint.Y + offset_y);
            this.Location = tipPoint;
            this.LocalPositionX = position.X + offset_x;
            this.LocalPositionY = position.Y + offset_y;
            parentPoint = targetPoint;
            this.Visible = true;
        }


        #endregion

        /// <summary>
        /// 点移动记录偏移量
        /// </summary>
        /// <param name="targetPoint">目标位置</param>
        /// <param name="position">目标点位置</param>
        public void UpdateLabelLocationAndPosition(Point targetPoint, Point position)
        {
            if (parentPoint == targetPoint) return;
            this.offset_x = this.LocalPositionX - position.X;
            this.offset_y = this.LocalPositionY - position.Y;
            parentPoint = targetPoint;
        }

        /// <summary>
        /// 更新标牌位置
        /// </summary>
        /// <param name="targetPoint"></param>
        public void UpdateLabelLocation(Point targetPoint) 
        {
            Point location = new Point(targetPoint.X+offset_x,targetPoint.Y+offset_y);
            this.Location = location;
        }

        /// <summary>
        /// 设置目标信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        public void SetTargetInfo(string name, double lng, double lat)
        {
            targetInfo = string.Format("目标编号:{0}\r经度:{1}\r纬度:{2}\r", name, lng, lat);
            SetLabelText(context);
        }

        /// <summary>
        /// 设置标牌大小
        /// </summary>
        /// <param name="size">大小</param>
        public void SetSize(Size size)
        {
            this.Size = size;
        }

        /// <summary>
        /// 设置标牌内容
        /// </summary>
        /// <param name="txt">内容</param>
        public void SetLabelText(string txt)
        {
            context = txt;
            labelText = targetInfo + txt;
            string[] txtA = labelText.Split('\r');
            if (this.InvokeRequired)//多线程操作
            {
                this.Invoke((Action)delegate()
                {
                    foreach (var item in txtA)
                    {
                        labF.Text = item;
                        if (labF.Width + 10 >= this.Width - 14)
                        {
                            this.Width = labF.Width + 24;
                        }
                    }
                    labContext.Text = labelText;
                });
            }
            else//主线程调用
            {
                foreach (var item in txtA)
                {
                    labF.Text = item;
                    if (labF.Width + 10 >= this.Width - 14)
                    {
                        this.Width = labF.Width + 24;
                    }
                }
                labContext.Text = labelText;
            }
        }

        /// <summary>
        /// 设置标牌内容
        /// </summary>
        /// <param name="txt">内容</param>
        /// <param name="fontColor">字体颜色</param>
        /// <param name="backColor">背景色</param>
        public void SetLabelText(string txt, Color fontColor, Color backColor)
        {
            if (string.IsNullOrEmpty(txt)) return;
            this.ForeColor = fontColor;
            this.BackColor = backColor;
            string[] txtA = txt.Split('\r');
            foreach (var item in txtA)
            {
                labF.Text = item;
                if (labF.Width + 10 >= this.Width - 14)
                {
                    this.Width = labF.Width + 24;
                }
            }
            labContext.Text = txt;
        }

        /// <summary>
        /// 设置标牌是否显示
        /// </summary>
        /// <param name="visible">是否显示</param>
        public void SetVisisble(bool visible)
        {
            Thread thread = new Thread(o =>
            {
                if (visible)
                    Thread.Sleep(1000);
                if (this.InvokeRequired)
                {
                    this.Invoke((Action)delegate()
                    {
                        this.Visible = visible;
                    });
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 设置标牌背景色
        /// </summary>
        /// <param name="color">颜色  透明在50至255之间</param>
        public void SetBackgroundColor(Color color)
        {
            this.BackColor = color;
            tootipTool1.BackColor = Color.AliceBlue;
            if (color.A > 50)
                tootipTool1.Transparency = color.A;
        }

        /// <summary>
        /// 设置字体颜色
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetFontColor(Color color)
        {
            labContext.ForeColor = color;
        }

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="opacity">透明度  50至255之间</param>
        public void SetTransparency(int opacity)
        {
            this.BackColor = Color.FromArgb((byte)opacity, this.BackColor);
        }

        /// <summary>
        /// 重置焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolTipEx_MouseLeave(object sender, EventArgs e)
        {
            labContext.Focus();
        }

    }
}
