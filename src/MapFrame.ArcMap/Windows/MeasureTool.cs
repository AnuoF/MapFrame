using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapFrame.ArcMap.Tool;

namespace MapFrame.ArcMap.Windows
{
    partial class MeasureTool : UserControl
    {
        private Measure measure;
        public MeasureTool(Measure _measure)
        {
            InitializeComponent();
            measure = _measure;
            measure.ResultEventArgs += new Measure.ResultEvent(measure_ResultEventArgs);
        }

        void measure_ResultEventArgs(string result)
        {
            if (lbResult.InvokeRequired)
            {
                lbResult.Invoke((Action)delegate() { lbResult.Text = result; });
            }
            else
            {
                lbResult.Text = result;
            }
        }

        private void tool_Copy_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(lbResult.Text);
        }
    }
}
