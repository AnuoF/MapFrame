using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace GlobleSituation.Model
{
    [XmlType(TypeName = "Bookmark")]
    public class Bookmark
    {
        public string Show { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Altitude { get; set; }
        //public string Target { get; set; }
        public double Azimuth { get; set; }
        public double Inclination { get; set; }
        public double ViewingDistance { get; set; }
        public double ViewFieldAngle { get; set; }
        public double RollAngle { get; set; }
    }
}
