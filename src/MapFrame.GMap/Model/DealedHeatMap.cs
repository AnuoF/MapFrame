
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GMap.NET;

namespace MapFrame.GMap.Model
{
    class DealedHeatMap
    {
        public ColorRamp ColorRamp { get; set; }
        public Bitmap GrayMap { get;  set; }
        public Bitmap HeatMap { get; set; }
        public PointLatLng LatLng { get; set; }

    }
}
