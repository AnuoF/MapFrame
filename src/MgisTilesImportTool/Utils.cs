

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MgisTilesImportTool
{
    class Utils
    {
        private static List<Tile> tileList = new List<Tile>();

        private static object lockObj = new object();

        public static void AddTile(byte[] _tile, int _type, int _x, int _y, int _zoom)
        {
           
            lock (lockObj)
            {
            }
        }

        public static List<Tile> GetTileList()
        {
            lock (lockObj)
            {
                List<Tile> tmp = tileList;
                tileList.Clear();
                return tmp;
            }
        }

    }
}
