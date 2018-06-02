using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobleSituation.Model
{
    public class SendInsertDataToStoreEventArgs : EventArgs
    {
        public List<RealData> DataList { get; set; }
    }
}
