/**************************************************************************
 * 类名：InitMapFrame.cs
 * 描述：初始化地图框架类,使用前先初始化框架
 * 作者：Allen
 * 日期：June 30,2016
 * 
 * ************************************************************************/

using MapFrame.Core.Model;
using MapFrame.Core.Interface;

namespace MapFrame.Logic
{
    /// <summary>
    /// 初始化地图框架类
    /// </summary>
    public class InitMapFrame
    {
        /// <summary>
        /// 地图逻辑类
        /// </summary>
        private IMapLogic mapLogic = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="engineType">引擎类型</param>
        /// <param name="mapObject">地图控件对象，默认为空。如果为空，则用框架的地图控件；不为空，则使用外部地图控件</param>
        public InitMapFrame( MapEngineType engineType,object mapObject = null)
        {
            mapLogic = new MapLogic(engineType, mapObject);
        }

        /// <summary>
        /// 获取地图逻辑控制对象
        /// </summary>
        /// <returns>IMapLogic</returns>
        public IMapLogic GetMapLogic()
        {
            return mapLogic;
        }


    }
}
