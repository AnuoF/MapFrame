using System;
using MapFrame.Core.Model;
using AxHOSOFTMapControlLib;
using System.Drawing;
using MapFrame.Core.Interface;

namespace MapFrame.Mgis.Element
{
    class Picture_Mgis : IMFPicture
    {
        /// <summary>
        /// 刷新用的时间
        /// </summary>
        private System.Timers.Timer timer;
        /// <summary>
        /// 地图控件
        /// </summary>
        private AxHOSOFTMapControl mapControl;
        /// <summary>
        /// 图层管理
        /// </summary>
        private IMFLayer _belongLayer;
        /// <summary>
        /// 动目标指针
        /// </summary>
        private ulong moveObj;
        /// <summary>
        /// 图元是否闪烁true显示,false隐藏
        /// </summary>
        private bool isFlash;
        /// <summary>
        /// 图元是否高亮
        /// </summary>
        private bool isHightLight;

        public Picture_Mgis()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Update();
        }

        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 图层管理
        /// </summary>
        public Core.Interface.IMFLayer BelongLayer
        {
            get
            {
                return _belongLayer;
            }
            set
            {
                mapControl = value.MapControl as AxHOSOFTMapControl;
                _belongLayer = value;
            }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 图元名称
        /// </summary>
        public string ElementName
        {
            get;
            set;
        }

        /// <summary>
        /// 图元类型
        /// </summary>
        public ElementTypeEnum ElementType
        {
            get;
            set;
        }

        /// <summary>
        /// 是否高亮
        /// </summary>
        public bool IsHightLight
        {
            get { return isFlash; }
        }

        /// <summary>
        /// 是否闪烁
        /// </summary>
        public bool IsFlash
        {
            get
            {
                return isFlash;
            }
        }

        /// <summary>
        /// 是否可见true显示,false隐藏
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return mapControl.getMoveObjectVisible(moveObj) == 1 ? true : false;
            }
        }

        private bool isFollowing;
        /// <summary>
        /// 目标是否处于被跟踪状态
        /// </summary>
        public bool IsFollowing
        {
            get { return isFollowing; }
        }

        /// <summary>
        /// 传递动目标到该类
        /// </summary>
        /// <param name="_moveObj">动目标指针</param>
        public void SetMoveObj(ulong _moveObj)
        {
            this.moveObj = _moveObj;
        }

        /// <summary>
        /// 图元指针
        /// </summary>
        public string ElementPtr
        {
            get { return moveObj + ""; }
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="alt">高度</param>
        public void UpdatePosition(double lng, double lat, double alt = 0)
        {
            mapControl.setMoveObjectPositon(moveObj, lng, lat, 1);
            Update();
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lngLat">经纬度</param>
        public void UpdatePosition(Core.Model.MapLngLat lngLat)
        {
            mapControl.setMoveObjectPositon(moveObj, lngLat.Lng, lngLat.Lat, 1);
            Update();
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetColor(System.Drawing.Color color)
        {
            mapControl.setMoveObjectColor(moveObj, color.R, color.G, color.B, color.A);
            Update();
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="argb">argb值</param>
        public void SetColor(int argb)
        {
            Color color = Color.FromArgb(argb);
            this.SetColor(color);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="r">R值</param>
        /// <param name="g">G值</param>
        /// <param name="b">B值</param>
        public void SetColor(int r, int g, int b)
        {
            Color color = Color.FromArgb(r, g, b);
            this.SetColor(color);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="a">A值</param>
        /// <param name="r">R值</param>
        /// <param name="g">G值</param>
        /// <param name="b">B值</param>
        public void SetColor(int a, int r, int g, int b)
        {
            Color color = Color.FromArgb(a, r, g, b);
            this.SetColor(color);
        }

        /// <summary>
        /// 获取经纬度
        /// </summary>
        /// <returns></returns>
        public Core.Model.MapLngLat GetLngLat()
        {
            double lng = 0;
            double lat = 0;
            mapControl.getMoveObjectPosition(moveObj, ref lng, ref lat);
            Core.Model.MapLngLat lnglat = new MapLngLat(lng, lat);
            return lnglat;
        }

        /// <summary>
        /// 设置方位角
        /// </summary>
        /// <param name="angle"></param>
        public void SetAngle(double angle)
        {
            mapControl.setMoveObjectRotate(moveObj, angle, 0);
            Update();
        }

        /// <summary>
        /// 重新设置图片
        /// </summary>
        /// <param name="icon">图片路径</param>
        public void SetIcon(string icon)
        {
            if (!string.IsNullOrEmpty(icon) && System.IO.File.Exists(icon))
            {
                mapControl.setMoveObjectImage(moveObj, icon);
                Update();
            }
        }

        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(MapSize size)
        {
            mapControl.setMoveObjectScale(moveObj, size.Width, size.Height);
            Update();
        }

        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Size size)
        {
            mapControl.setMoveObjectScale(moveObj, size.Width, size.Height);
            Update();
        }

        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void SetSize(int width, int height)
        {
            mapControl.setMoveObjectScale(moveObj, width, width);
            Update();
        }

        /// <summary>
        /// 设置大小
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(double size)
        {
            mapControl.setMoveObjectScale(moveObj, size, size);
            Update();
        }

        /// <summary>
        /// 设置tip内容
        /// </summary>
        /// <param name="tipText"></param>
        public void SetTipText(string tipText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 设置tip显示方式
        /// </summary>
        /// <param name="showType"></param>
        public void SetTipShow(Core.Model.ShowTypeEnum showType)
        {
            switch (showType)
            {
                case ShowTypeEnum.Always:
                    mapControl.setTipsVisible(1);
                    break;
                case ShowTypeEnum.MouseHover:
                    break;
                case ShowTypeEnum.No:
                    mapControl.setTipsVisible(0);
                    break;
            }
        }

        /// <summary>
        /// 跟踪
        /// </summary>
        /// <param name="flag">true跟踪,false不跟踪</param>
        public void Follow(bool flag)
        {
            isFollowing = flag;
            mapControl.moveObjectFollowing(moveObj, flag == true ? 1 : 0);
            Update();
        }

        /// <summary>
        /// 高亮
        /// </summary>
        /// <param name="isHightLight"></param>
        public void HightLight(bool isHightLight)
        {
            this.isHightLight = IsHightLight;
            mapControl.moveObjectFollowing(moveObj, isHightLight == true ? 1 : 0);
            Update();
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="isFlash">是否闪烁</param>        
        /// <param name="interval">闪烁间隔</param>
        public void Flash(bool isFlash, int interval = 500)
        {
            if (this.isFlash == isFlash) return;//防止被多次调用
            this.timer.Interval = interval;
            double x = 0;
            double y = 0;
            mapControl.getMoveObjectScale(moveObj, ref x, ref y);
            if (isFlash)
            {
                this.timer.Start();
                mapControl.setMoveObjectFlash(moveObj, interval, (int)x * 2, 1);
            }
            else
            {
                this.timer.Stop();
                mapControl.setMoveObjectFlash(moveObj, 0, (int)x, 0);
            }
            this.isFlash = isFlash;
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="isVisible">是否显示</param>
        public void SetVisible(bool isVisible)
        {
            mapControl.setMoveObjectProperty(moveObj, "目标类型", "军用");
            mapControl.setMoveObjectVisible(moveObj, isVisible ? 1 : 0);
            Update();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            _belongLayer.Refresh();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {

        }

        #region 标牌

        /// <summary>
        /// 标牌是否显示
        /// </summary>
        public bool BiaoPaiVisible
        {
            get { return mapControl.getMoveObjectBiaoPaiVisible(moveObj) == 1 ? true : false; }
        }

        /// <summary>
        /// 设置标牌是否可见
        /// </summary>
        /// <param name="visible">是否可见true显示false隐藏</param>
        public void SetBiaoPaiVis(bool visible)
        {
            mapControl.setMoveObjectBiaoPaiVisible(moveObj, visible == true ? 1 : 0);
            Update();
        }

        /// <summary>
        /// 设置标牌背景颜色
        /// </summary>
        /// <param name="bgColor"></param>
        public void SetBiaoPaiBkCo(Color bgColor)
        {
            mapControl.setBiaoPaiBkColor(moveObj, bgColor.R, bgColor.G, bgColor.B, bgColor.A);
        }

        /// <summary>
        /// 设置标牌文字颜色
        /// </summary>
        /// <param name="fontColor"></param>
        public void SetBiaoPaiTxtCo(Color fontColor)
        {
            mapControl.setBiaoPaiTextColor(moveObj, fontColor.R, fontColor.G, fontColor.B, fontColor.A);
        }

        /// <summary>
        /// 设置标牌线颜色
        /// </summary>
        /// <param name="lineColor"></param>
        public void SetBiaoPaiLineCo(Color lineColor)
        {
            mapControl.setBiaoPaiLineColor(moveObj, lineColor.R, lineColor.G, lineColor.B, lineColor.A);
        }

        /// <summary>
        /// 设置标牌内容
        /// </summary>
        /// <param name="attributeName">属性名称</param>
        /// <param name="value">值</param>
        /// <param name="index">属性顺序索引</param>
        public void SetBiaoPaiContent(string attributeName, string value, int index = -1)
        {
            try
            {
                if (string.IsNullOrEmpty(attributeName)) { throw new Exception("属性不能为空"); }
                mapControl.SetLayerMoveObjPropVisInBp(_belongLayer.LayerName, attributeName, index, 1);
                mapControl.setMoveObjectProperty(moveObj, attributeName, value);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 设置标牌附加内容
        /// </summary>
        /// <param name="attributeName">属性名称</param>
        /// <param name="value">值</param>
        /// <param name="attachedIndex">附加属性索引</param>
        public void SetBiaoPaiAttContent(string attributeName, string value, int attachedIndex = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(attributeName)) { throw new Exception("属性不能为空"); }
                mapControl.SetLayerTrackPointPropVisInBp(_belongLayer.LayerName, attributeName, attachedIndex, 1);
                mapControl.setMoveObjectTrackProperty(moveObj, 0, attributeName, value);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region 航迹
        /// <summary>
        /// 设置轨迹线是否可见
        /// </summary>
        /// <param name="visible">true:可见 false:隐藏</param>
        public void SetTrackVisible(bool visible)
        {
            SetTrackMaxPoint(10);
            SetTrackLineStrip(2);
            SetTrackLineColor(Color.Green);
            SetTrackPointSize(10);
            mapControl.setMoveObjectTrackVisible(moveObj, visible == true ? 1 : 0);
        }

        /// <summary>
        /// 设置航迹点容量
        /// </summary>
        /// <param name="trackPointNum"></param>
        public void SetTrackMaxPoint(int trackPointNum = 65000)
        {
            mapControl.setMoveObjectTrackMaxPoint(moveObj, trackPointNum);
        }

        /// <summary>
        /// 设置航迹点类型
        /// </summary>
        /// <param name="strip"></param>
        public void SetTrackLineStrip(int strip = -1)
        {
            mapControl.setMoveObjectTrackLineStrip(moveObj, strip);
        }

        /// <summary>
        /// 设置航迹线颜色
        /// </summary>
        /// <param name="lineColor"></param>
        public void SetTrackLineColor(Color lineColor)
        {
            mapControl.setMoveObjectTrackLineColor(moveObj, lineColor.R, lineColor.G, lineColor.B, lineColor.A);
        }

        /// <summary>
        /// 设置航迹点大小
        /// </summary>
        /// <param name="size"></param>
        public void SetTrackPointSize(int size = 5)
        {
            mapControl.setMoveObjectTrackPointSize(moveObj, size);
        }

        /// <summary>
        /// 设置航迹线宽度
        /// </summary>
        /// <param name="width"></param>
        public void SetTrackPointWidth(int width)
        {
            mapControl.setMoveObjectTrackLineWidth(moveObj, width);
        }
        #endregion

        #region Tip


        /// <summary>
        /// tip是否可见
        /// </summary>
        public bool TipsVisible
        {
            get { return mapControl.getTipsVisible() == 1 ? true : false; }
        }

        public bool IsLableVisible
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ShowTypeEnum LabelShowType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 设置Tip是否显示
        /// </summary>
        /// <param name="visible">true可见,false隐藏</param>
        public void SetTipVisible(bool visible)
        {
            mapControl.setTipsVisible(visible == true ? 1 : 0);
        }

        /// <summary>
        /// 设置Tip背景颜色
        /// </summary>
        /// <param name="bgColor"></param>
        public void SetTipBgColor(Color bgColor)
        {
            mapControl.setTooltipBkColor(bgColor.R, bgColor.G, bgColor.B, bgColor.A);
        }

        /// <summary>
        /// 设置Tip文字颜色
        /// </summary>
        /// <param name="fontColor"></param>
        public void SetTipTextColor(Color fontColor)
        {
            mapControl.setTooltipTextColor(fontColor.R, fontColor.G, fontColor.B, fontColor.A);
        }

        /// <summary>
        /// 设置Tip内容
        /// </summary>
        /// <param name="attributeName">属性名称</param>
        /// <param name="value">值</param>
        /// <param name="index">属性索引</param>
        public void SetTipContent(string attributeName, string value, int index = -1)
        {
            try
            {
                if (string.IsNullOrEmpty(attributeName)) { throw new Exception("属性不能为空"); }
                mapControl.SetLayerMoveObjPropVisInTip(_belongLayer.LayerName, attributeName, index, 1);
                mapControl.setMoveObjectProperty(moveObj, attributeName, value);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 设置Tip附加内容
        /// </summary>
        /// <param name="attributeName">属性名称</param>
        /// <param name="value">值</param>
        /// <param name="attachIndex">附加属性索引</param>
        public void SetTipAttachedContent(string attributeName, string value, int attachIndex = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(attributeName)) { throw new Exception("属性不能为空"); }
                mapControl.SetLayerTrackPointPropVisInTip(_belongLayer.LayerName, attributeName, attachIndex, 1);
                mapControl.setMoveObjectTrackProperty(moveObj, 0, attributeName, value);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetAngle(float angle)
        {
            throw new NotImplementedException();
        }

        public void SetScale(float scale)
        {
            throw new NotImplementedException();
        }

        public void SetLabelText(string labelText)
        {
            throw new NotImplementedException();
        }

        public void SetLableShow(ShowTypeEnum showType)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
