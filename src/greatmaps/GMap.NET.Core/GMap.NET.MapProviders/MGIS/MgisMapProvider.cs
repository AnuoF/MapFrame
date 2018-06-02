/**************************************************************************
 * 类名：MgisMapProvider.cs
 * 描述：MGIS地图提供类
 * 作者：Allen
 * 日期：Nov 18,2016
 * 
 * ************************************************************************/

using GMap.NET.MapProviders;
using GMap.NET.Projections;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMap.NET.MapProviders.MGIS
{

    public abstract class MgisProviderBase : GMapProvider
    {
        public MgisProviderBase()
        {
            MaxZoom = null;
            RefererUrl = "http://wwww.mgis.com";
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


    public class MgisMapProvider : MgisProviderBase
    {
        public static readonly MgisMapProvider Instance;
        readonly Guid id = new Guid("EF3DD303-3F74-4938-BF40-232D0595EE77");

        public override Guid Id
        {
            get
            {
                return id;
            }
        }

        readonly string name = "Mgis";
        public override string Name
        {
            get
            {
                return name;
            }
        }

        static MgisMapProvider()
        {
            Instance = new MgisMapProvider();
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

        static readonly string UrlFormat = "http://webrd01.is.mgis.com/appmaptile?lang=zh_cn&size=1&scale=1&style=7&x={0}&y={1}&z={2}";

    }
}
