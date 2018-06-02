/**************************************************************************
 * 类名：AMapProvider.cs
 * 描述：高德地图提供类
 * 作者：Allen
 * 日期：Nov 8,2016
 * 
 * ************************************************************************/

using System;
using GMap.NET.Projections;

namespace GMap.NET.MapProviders
{

    public abstract class AMapProviderBase : GMapProvider
    {
        public AMapProviderBase()
        {
            MaxZoom = null;
            RefererUrl = "http://wwww.amap.com";
            //Copyright = string.Format("©{0}高德Corporation,©{0}NAVTEQ,©{0}Image courtesy of NASA", DateTime.Today.Year);
        }

        public override PureProjection Projection
        {
            get { return MercatorProjection.Instance; }
        }

        GMapProvider[] overlays;

        public override GMapProvider[] Overlays
        {
            get
            {
                if (overlays == null)
                {
                    overlays = new GMapProvider[] { this };
                }
                return overlays;
            }
        }
    }

    public class AMapProvider : AMapProviderBase
    {
        public static readonly AMapProvider Instance;
        readonly Guid id = new Guid("EF3DD303-3F74-4938-BF40-232D0595EE88");

        public override Guid Id
        {
            get
            {
                return id;
            }
        }

        readonly string name = "AMap";
        public override string Name
        {
            get
            {
                return name;
            }
        }

        static AMapProvider()
        {
            Instance = new AMapProvider();
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            try
            {
                string url = MakeTileImageUrl(pos, zoom, LanguageStr);
                return GetTileImageUsingHttp(url);
            }
            catch
            {
                return null;
            }
        }

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            var num = (pos.X + pos.Y) % 4 + 1;
            string url = string.Format(UrlFormat, pos.X, pos.Y);
            return url;
        }

        static readonly string UrlFormat = "http://webrd01.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=7&x={0}&y={1}&z={2}";
    }
}
