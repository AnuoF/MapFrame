
using System;
using GlobleSituation.Model;

namespace GlobleSituation.Common
{
    public class EventPublisher
    {

        /// <summary>
        /// 接收波束数据事件
        /// </summary>
        public static event EventHandler<BeamData> BeamDataComeEvent;
        public static void PublishBeamDataComeEvent(object sender, BeamData e)
        {
            if (BeamDataComeEvent != null)
            {
                BeamDataComeEvent.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 接收波束数据事件
        /// </summary>
        public static event EventHandler<BeamData> MapDealBeamDataEvent;
        public static void PublishMapDealBeamDataEvent(object sender, BeamData e)
        {
            if (MapDealBeamDataEvent != null)
            {
                MapDealBeamDataEvent.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 态势数据
        /// </summary>
        public static event EventHandler<TSDataEventArgs> TSDataEvent;
        //态势数据
        public static void PublishTSDataEvent(object sender, TSDataEventArgs e)
        {
            if (TSDataEvent != null)
            {
                TSDataEvent.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 添加图元后触发事件
        /// </summary>
        public static event EventHandler<ElementAddEventArgs> ElementAddEvent;
        public static void PublishElementAddEvent(object sender, ElementAddEventArgs e)
        {
            if(ElementAddEvent!=null)
            {
                ElementAddEvent.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 预警数据事件
        /// </summary>
        public static event EventHandler<TSDataEventArgs> WarnDataEvent;
        public static void PublishWarnDataEvent(object sender, TSDataEventArgs e)
        {
            if (WarnDataEvent != null)
            {
                WarnDataEvent.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 跳转到三维视图事件
        /// </summary>
        public static event EventHandler<JumpToGlobeViewEventArgs> JumpToGlobeViewEvent;
        public static void PublishJumpToGlobeViewEvent(object sender, JumpToGlobeViewEventArgs e)
        {
            if (JumpToGlobeViewEvent != null)
            {
                JumpToGlobeViewEvent.Invoke(sender, e);
            }
        }
     
        /// <summary>
        /// 态势展示方式改变历史态势事件
        /// </summary>
        public static event EventHandler<EventArgs> ChangedToHistoryEvent;
        public static void PublishChangedToHistoryEvent(object sender, ShowModeChangedEventArgs e)
        {
            if (ChangedToHistoryEvent != null)
            {
                ChangedToHistoryEvent.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 态势展示方式改变实时态势事件
        /// </summary>
        public static event EventHandler<EventArgs> ChangedToRealTimeEvent;
        public static void PublishChangedToRealTimeEvent(object sender, ShowModeChangedEventArgs e)
        {
            if (ChangedToRealTimeEvent != null)
            {
                ChangedToRealTimeEvent.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 入库实时数据事件
        /// </summary>
        public static event EventHandler<SendInsertDataToStoreEventArgs> SendInsertDataToStoreEvent;
        public static void PublishSendInsertDataToStoreEvent(object sender, SendInsertDataToStoreEventArgs e)
        {
            if (SendInsertDataToStoreEvent != null)
            {
                SendInsertDataToStoreEvent.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 查询数据请求事件
        /// </summary>
        public static event EventHandler<SendSearchDataToStoreEventArgs> SendSearchDataToStoreEvent;
        public static void PublishSendSearchDataToStoreEvent(object sender, SendSearchDataToStoreEventArgs e)
        {
            if (SendSearchDataToStoreEvent != null)
            {
                SendSearchDataToStoreEvent.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 接收查询数据事件
        /// </summary>
        public static event EventHandler<RecvSearchDataEventArgs> RecvSearchDataEvent;
        public static void PublishRecvSearchDataEvent(object sender, RecvSearchDataEventArgs e)
        {
            if (RecvSearchDataEvent != null)
            {
                RecvSearchDataEvent.Invoke(sender, e);
            }
        }

        //------------------------分割线--------------------------//


        /// <summary>
        /// 显示书签列表事件
        /// </summary>
        public static event EventHandler<ShowBookmarkEventArgs> ShowBookmarkEvent;
        public static void PublishShowBookmarkEvent(object sender, ShowBookmarkEventArgs e)
        {
            if (ShowBookmarkEvent != null)
            {
                ShowBookmarkEvent.Invoke(sender, e);
            }
        }

        ///// <summary>
        ///// 绘制区域事件
        ///// </summary>
        //public static event EventHandler<DrawAreaEventArgs> DrawAreaEvent;
        //public static void PublishDrawAreaEvent(object sender, DrawAreaEventArgs e)
        //{
        //    if (DrawAreaEvent != null)
        //    {
        //        DrawAreaEvent.BeginInvoke(sender, e, null, null);
        //    }
        //}

        ///// <summary>
        ///// 绘制完成事件
        ///// </summary>
        //public static event EventHandler<FinshAreaEventArgs> FinishAreahEvent;
        //public static void PublishFinishAreahEvent(object sender, FinshAreaEventArgs e)
        //{
        //    if (FinishAreahEvent != null)
        //    {
        //        FinishAreahEvent.Invoke(sender, e);
        //    }
        //}

        ///// <summary>
        ///// 修改区域事件
        ///// </summary>
        //public static event EventHandler<ChangeAreaEventArgs> ChangeAreaEvent;

        //public static void PublishChangeAreaEvent(object sender, ChangeAreaEventArgs e)
        //{
        //    if (ChangeAreaEvent != null)
        //    {
        //        ChangeAreaEvent.Invoke(sender, e);
        //    }
        //}

        ///// <summary>
        ///// 删除区域事件
        ///// </summary>
        //public static event EventHandler<DeleteAreaEventArgs> DeleteAreaEvent;

        //public static void PublishDeleteAreaEvent(object sender, DeleteAreaEventArgs e)
        //{
        //    if (DeleteAreaEvent != null)
        //    {
        //        DeleteAreaEvent.Invoke(sender, e);
        //    }
        //}

        /// <summary>
        /// 重新加载地图事件
        /// </summary>
        public static event EventHandler<EventArgs> Reload3dDocumentEvent;
        public static void PublishReload3dDocumentEvent(object sender, EventArgs e)
        {
            if (Reload3dDocumentEvent != null)
            {
                Reload3dDocumentEvent.Invoke(sender, e);
            }
        }



    }
}
