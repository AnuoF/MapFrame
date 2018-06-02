using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GlobleSituation.Model
{
    class DeviceData
    {
        public string DeviceName;

        public string DeviceNumber;

        public double RangeRadius;

        public double Lng;

        public double Lat;

        public double Alt;

        public string Belang;

        public string Remark;

        public string IsVisible = "显示";


        public DeviceData()
        {

        }

        public DeviceData(DataRow row)
        {
            DeviceName = row["DeviceName"].ToString();
            DeviceNumber = row["DeviceNumber"].ToString();
            RangeRadius = Convert.ToDouble(row["RangeRadius"]);
            Lng = Convert.ToDouble(row["Lng"]);
            Lat = Convert.ToDouble(row["Lat"]);
            Alt = Convert.ToDouble(row["Alt"]);
            Belang = row["Belang"].ToString();
            Remark = row["Remark"].ToString();
        }
    }
}
