using System.Windows.Forms;
using System.Drawing;

namespace GlobleSituation.Common
{
    class ColorDialogEx : ColorDialog
    {
    
        public Color GetColor()
        {
            Color c = this.Color;
            Color color = Color.FromArgb(100, c);

            return color;
        }
    }
}
