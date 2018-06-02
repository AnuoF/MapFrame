/**************************************************************************
 * ������DrawPolygon.cs
 * ���������ƶ����
 * ���ߣ�CJ
 * ���ڣ�2016��9��8��
 * 
 * ************************************************************************/

using System;
using ESRI.ArcGIS.Controls;
using System.Collections.Generic;
using ESRI.ArcGIS.SystemUI;
using MapFrame.Core.Interface;
using MapFrame.Core.Model;
using MapFrame.ArcMap.Common;
using System.Drawing;

namespace MapFrame.ArcMap.Tool
{
    /// <summary>
    /// ���ƶ����
    /// </summary>
    class DrawPolygon : IMFTool, IMFDraw
    {
        /// <summary>
        /// ����ִ������¼�
        /// </summary>
        public event EventHandler<MessageEventArgs> CommondExecutedEvent;
        /// <summary>
        /// arcgis�ĵ�ͼ�ؼ�
        /// </summary>
        private AxMapControl mapControl = null;
        /// <summary>
        /// �Ƿ����
        /// </summary>
        private bool isFinish = false;
        /// <summary>
        ///ͼ�����
        /// </summary>
        private IMapLogic mapLogic = null;
        /// <summary>
        /// �����ͼԪ
        /// </summary>
        private IMFPolygon polygonElement = null;
        /// <summary>
        /// ͼ��
        /// </summary>
        private IMFLayer layer = null;
        /// <summary>
        /// ����㼯��
        /// </summary>
        private List<MapLngLat> listMapPoints = null;
        /// <summary>
        /// �Ƿ���Control��
        /// </summary>
        private bool isControl = false;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="_mapControl"></param>
        public DrawPolygon(AxMapControl _mapControl)
        {
            mapControl = _mapControl;
            listMapPoints = new List<MapLngLat>();
        }

        /// <summary>
        /// ͼ�����
        /// </summary>
        public IMapLogic MapLogic
        {
            set { mapLogic = value; }
        }

        /// <summary>
        /// ��ȡͼԪ
        /// </summary>
        /// <returns></returns>
        public IMFElement GetDrawElement()
        {
            return polygonElement;
        }

        /// <summary>
        /// ִ�й�������
        /// </summary>
        public void RunCommond()
        {
            mapControl.CurrentTool = null;
            layer = mapLogic.AddLayer("draw_arcLayer");
            RegistEvent();
        }

        /// <summary>
        /// �ͷŹ���
        /// </summary>
        public void ReleaseCommond()
        {
            if (mapControl != null)
            {
                LogoutEvent();
                ICommand tool = new ControlsMapPanToolClass();
                tool.OnCreate(mapControl.Object);
                mapControl.CurrentTool = tool as ITool;
            }
        }

        /// <summary>
        /// ע���¼�
        /// </summary>
        private void RegistEvent()
        {
            mapControl.OnMouseDown += mapControl_OnMouseDown;
            mapControl.OnDoubleClick += mapControl_OnDoubleClick;
            mapControl.OnMouseMove += mapControl_OnMouseMove;
            mapControl.OnKeyDown += mapControl_OnKeyDown;
            mapControl.OnKeyUp += mapControl_OnKeyUp;
        }

        /// <summary>
        /// ע���¼�
        /// </summary>
        private void LogoutEvent()
        {
            mapControl.OnMouseDown -= mapControl_OnMouseDown;
            mapControl.OnMouseMove -= mapControl_OnMouseMove;
            mapControl.OnDoubleClick -= mapControl_OnDoubleClick;
            mapControl.OnKeyDown -= mapControl_OnKeyDown;
            mapControl.OnKeyUp -= mapControl_OnKeyUp;
        }

        /// <summary>
        /// ע���������¼�
        /// </summary>
        private void RegistCommondExecutedEvent()
        {
            if (this.CommondExecutedEvent != null)
            {
                MessageEventArgs msg = new MessageEventArgs()
                {
                    Describe = "�ֶ����ƶ����",
                    Data = polygonElement,
                    ToolType = ToolTypeEnum.Draw
                };
                this.CommondExecutedEvent(this, msg);
            }
        }

        #region  �¼�

        /// <summary>
        /// ������ΰ���escȡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            if (e.keyCode == 27)
            {
                if (!isFinish)
                {
                    layer.RemoveElement(polygonElement);
                    listMapPoints.Clear();
                    isFinish = true;
                }
                else
                {
                    ReleaseCommond();
                }
            }
            if (e.keyCode == 17)//�ո�
            {
                isControl = true;
                ICommand command = new ControlsMapPanToolClass();
                command.OnCreate(mapControl.Object);
                if (command.Enabled)
                {
                    mapControl.CurrentTool = command as ITool;
                }
            }
        }

        /// <summary>
        /// �ո�������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnKeyUp(object sender, IMapControlEvents2_OnKeyUpEvent e)
        {
            if (e.keyCode == 17)
            {
                isControl = false;//���̵���
                mapControl.CurrentTool = null;//����ͼ�Ĺ�����Ϊ��
            }
        }

        /// <summary>
        /// ���˫����ɻ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            if (e.button == 1 && !isControl)
            {
                if (listMapPoints.Count == 1)
                {
                    if (polygonElement != null)
                    {
                        layer.RemoveElement(polygonElement);
                    }
                }
                else
                {
                    isFinish = true;
                    listMapPoints.Clear();//���꼯�����
                    RegistCommondExecutedEvent();
                }

                ReleaseCommond();//�޸�  �¾�
            }
        }

        /// <summary>
        /// ����ƶ�ʵʱ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (listMapPoints.Count != 0 && !isControl)
            {
                MapLngLat moveLngLat = new MapLngLat() { Lng = e.mapX, Lat = e.mapY };
                listMapPoints.Add(moveLngLat);
                polygonElement.UpdatePosition(listMapPoints);
                listMapPoints.Remove(moveLngLat);
            }
        }

        /// <summary>
        /// ��갴�¿�ʼ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 1 && !isControl)
            {
                MapLngLat lngLat = new MapLngLat(e.mapX, e.mapY);
                if (listMapPoints.Count == 0)
                {
                    listMapPoints.Add(lngLat);
                    Kml kml = new Kml();
                    kml.Placemark.Name = "arc_Polygon" + Utils.ElementIndex;
                    Color outlineColor = Color.Blue;
                    Color fillColor = Color.Black;
                    kml.Placemark.Graph = new KmlPolygon() { FillColor = fillColor, OutLineColor = outlineColor, OutLineSize = 1, PositionList = listMapPoints };
                    IMFElement element = null;
                    layer.AddElement(kml, out element);
                    polygonElement = element as IMFPolygon;
                    isFinish = false;//Ϊ�����״̬
                }
                else if (listMapPoints.Find(p => p.Lng == e.mapX && p.Lat == e.mapY) == null)
                {
                    listMapPoints.Add(lngLat);
                }
            }
        }

        #endregion

        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        public void Dispose()
        {
            ReleaseCommond();
            CommondExecutedEvent = null;
            mapControl = null;
            mapLogic = null;
            listMapPoints = null;
            isControl = false;
            isFinish = false;
            layer = null;
        }

    }
}