/*************************************************************
 * 类名：SelectElement
 * 描述:框选目标
 * 创建人：Aline
 * 时间：2016年7月19日15:48:48
 * 修改时间：2016年7月19日15:49:02
 * **********************************************************/

using MapFrame.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using GMap.NET.WindowsForms;
using MapFrame.Core.Model;
using MapFrame.GMap.Common;

namespace MapFrame.GMap.Tool
{
    /// <summary>
    /// 框选目标
    /// </summary>
    class SelectElement : IMFTool, IMFSelect
    {
        /// <summary>
        /// 命令执行完成事件
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// 地图控件对象
        /// </summary>
        private GMapControl gmapControl = null;
        /// <summary>
        /// 目标对象集合
        /// </summary>
        private List<GMapMarker> markerList = null;
        /// <summary>
        /// 被选择图元集合
        /// </summary>
        private List<IMFElement> elementList = null;
        /// <summary>
        /// 地图逻辑
        /// </summary>
        private IMapLogic mapLogic;
        /// <summary>
        /// 解决左键点击问题
        /// </summary>
        private volatile bool flag;
        /// <summary>
        /// 被选中的图元
        /// </summary>
        private IMFElement selectedElment;
        /// <summary>
        /// 是否按下ctrl
        /// </summary>
        private bool ctrl;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="control"></param>
        /// <param name="_mapLogic">地图逻辑接口</param>
        public SelectElement(GMapControl control, IMapLogic _mapLogic)
        {
            mapLogic = _mapLogic;
            gmapControl = control;
            elementList = new List<IMFElement>();
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        public void RunCommond()
        {
            gmapControl.DisableAltForSelection = true;
            Utils.bPublishEvent = false;
            gmapControl.OnSelectionChange += new SelectionChange(gmapControl_OnSelectionChange);
            gmapControl.DoubleClick += new EventHandler(gmapControl_DoubleClick);
            gmapControl.MouseDown += new System.Windows.Forms.MouseEventHandler(gmapControl_MouseDown);
            gmapControl.OnMarkerClick += new MarkerClick(gmapControl_OnMarkerClick);
            gmapControl.OnPolygonClick += new PolygonClick(gmapControl_OnPolygonClick);
            gmapControl.OnRouteClick += new RouteClick(gmapControl_OnRouteClick);
            gmapControl.KeyDown += new System.Windows.Forms.KeyEventHandler(gmapControl_KeyDown);
            gmapControl.KeyUp += new System.Windows.Forms.KeyEventHandler(gmapControl_KeyUp);
        }

        /// <summary>
        /// 松开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gmapControl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == 17)
                ctrl = false;
        }

        /// <summary>
        /// 按下Ctrl可以多选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gmapControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == 17)
            {
                ctrl = true;
            }
        }

        /// <summary>
        /// 线点击事件
        /// </summary>
        /// <param name="item"></param>
        /// <param name="e"></param>
        void gmapControl_OnRouteClick(GMapRoute item, System.Windows.Forms.MouseEventArgs e)
        {
            selectedElment = mapLogic.GetElement(item.Tag.ToString());
            if (selectedElment == null) return;
            selectedElment.HightLight(!selectedElment.IsHightLight);
        }

        /// <summary>
        /// 面点击事件
        /// </summary>
        /// <param name="item"></param>
        /// <param name="e"></param>
        void gmapControl_OnPolygonClick(GMapPolygon item, System.Windows.Forms.MouseEventArgs e)
        {
            selectedElment = mapLogic.GetElement(item.Tag.ToString());
            if (selectedElment == null) return;
            selectedElment.HightLight(!selectedElment.IsHightLight);
        }

        /// <summary>
        /// 点点击事件
        /// </summary>
        /// <param name="item"></param>
        /// <param name="e"></param>
        void gmapControl_OnMarkerClick(GMapMarker item, System.Windows.Forms.MouseEventArgs e)
        {
            IMFElement element = mapLogic.GetElement(item.Tag.ToString());
            if (element == null) return;
            IMFElement el = elementList.Find(o => o.ElementName == element.ElementName);
            if (element.IsHightLight && el == null)
            {
                //他人高亮的
                return;
            }
            element.HightLight(!element.IsHightLight);

            if (selectedElment != null)
            {

                if (selectedElment != null && element.ElementName != selectedElment.ElementName)
                {
                    ClearNotSelecteMark();
                }
            }
            else
            {
                ClearNotSelecteMark();
            }

            if (element.IsHightLight) //已经高亮了
                elementList.Add(element);
            else//没有高亮
                elementList.Remove(element);

            selectedElment = element;
            if (CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    ToolType = ToolTypeEnum.Select
                };
                CommondExecutedEvent(this, msg);
            }
            //gmapControl.Focus();
        }

        /// <summary>
        /// 清除没有被选中的状态
        /// </summary>
        private void ClearNotSelecteMark()
        {
            if (!ctrl)//没有按下ctrl
            {
                //将之前选中的所有都取消选中状态
                int cout = elementList.Count;
                for (int i = 0; i < cout; i++)
                {
                    elementList[i].HightLight(false);
                }
                elementList.Clear();
            }
        }

        /// <summary>
        /// 解决鼠标左键点击也会触发区域选择完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                flag = true;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                flag = false;
            }
        }

        /// <summary>
        /// 鼠标双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControl_DoubleClick(object sender, EventArgs e)
        {
            ReleaseCommond();
            selectedElment.HightLight(false);
            foreach (var item in elementList)
            {
                item.HightLight(false);
            }
        }

        //区域选择完成事件
        private void gmapControl_OnSelectionChange(global::GMap.NET.RectLatLng Selection, bool ZoomToFit)
        {
            if (flag) return;
            if (ZoomToFit) return;
            foreach (var item in elementList)
            {
                item.HightLight(false);
            }
            elementList.Clear();
            markerList = gmapControl.Overlays[0].Markers.ToList();
            foreach (var item in markerList)
            {
                if (Selection.Contains(item.Position) && gmapControl.DisableAltForSelection) //包含在矩形内
                {
                    IMFElement element = item as IMFElement;
                    IMFElement el = elementList.Find(o => o.ElementName == element.ElementName);
                    if (elementList.Count > -1 && element.IsHightLight && el == null)
                        continue;
                    else
                    {
                        element.HightLight(true);//让选中的目标高亮
                        if (el == null)
                        {
                            elementList.Add(element);
                        }
                    }
                }
            }

            if (CommondExecutedEvent != null)
                CommondExecutedEvent(this, null);
            mapLogic.Refresh();
            flag = false;
        }

        /// <summary>
        /// 取消操作
        /// </summary>
        public void ReleaseCommond()
        {
            CommondExecutedEvent = null;
            gmapControl.DisableAltForSelection = false;
            gmapControl.OnSelectionChange -= gmapControl_OnSelectionChange;
            gmapControl.MouseDown -= gmapControl_MouseDown;
            gmapControl.MouseDoubleClick -= gmapControl_DoubleClick;
            gmapControl.OnRouteClick -= gmapControl_OnRouteClick;
            gmapControl.OnMarkerClick -= gmapControl_OnMarkerClick;
            gmapControl.OnPolygonClick -= gmapControl_OnPolygonClick;
            gmapControl.KeyDown -= gmapControl_KeyDown;
            gmapControl.KeyUp -= gmapControl_KeyUp;
            Utils.bPublishEvent = true;
        }

        /// <summary>
        /// 获取框选后的图元
        /// </summary>
        /// <returns></returns>
        public List<IMFElement> GetSelectElements()
        {
            return elementList;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            markerList = null;
            elementList = null;
            mapLogic = null;
            selectedElment = null;
        }
    }
}
