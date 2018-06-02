
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlobleSituation.Model
{
    public class RecvSearchDataEventArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }
}
