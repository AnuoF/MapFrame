
using MapFrame.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobleSituation.Model
{
    public class JumpToGlobeViewEventArgs : EventArgs
    {
        public string ElementName;
        public MapLngLat Position;

        public JumpToGlobeViewEventArgs(string _elementName,MapLngLat _position)
        {
            ElementName = _elementName;
            Position = _position;
        }
    }
}
