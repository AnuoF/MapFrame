
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MgisTilesImportTool
{
    public class Tile
    {
        public byte[] tile;

        public int type;

        public int x;

        public int y;

        public int zoom;

        public Tile(byte[] _tile, int _type, int _x, int _y, int _zoom)
        {
            tile = _tile;
            type = _type;
            x = _x;
            y = _y;
            zoom = _zoom;
        }

    }
}
