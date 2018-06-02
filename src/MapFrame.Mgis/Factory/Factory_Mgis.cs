

using MapFrame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapFrame.Mgis.Factory
{
    class Factory_Mgis : IMapFactory
    {
        public object GetMapControl()
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public void AddLayer(string layerName)
        {
            throw new NotImplementedException();
        }

        public void RemoveLayer(string layerName)
        {
            throw new NotImplementedException();
        }

        public void ClearLayer(string layerName)
        {
            throw new NotImplementedException();
        }

        public void ClearLayer()
        {
            throw new NotImplementedException();
        }

        public void ShowLayer(string layerName, bool visible)
        {
            throw new NotImplementedException();
        }

        public IElement AddElement(string layerName, Core.Model.Kml kml)
        {
            throw new NotImplementedException();
        }

        public bool RemoveElement(string layerName, IElement element)
        {
            throw new NotImplementedException();
        }
    }
}
