using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.ArcMap.Tool
{
    class SelectElementEx : MapFrame.Core.Interface.ITool, MapFrame.Core.Interface.ISelect
    {
        public List<Core.Interface.IElement> GetSelectElements()
        {
            throw new NotImplementedException();
        }

        public event EventHandler CommondExecutedEvent;

        public void RunCommond()
        {
            throw new NotImplementedException();
        }

        public void ReleaseCommond()
        {
            throw new NotImplementedException();
        }
    }
}
