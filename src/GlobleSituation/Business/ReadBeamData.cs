
/********************************************************************************
** 文件名：ReadBeamData.cs
** 版 本：1.0
** 内容简述：从文件提取卫星波束数据
** 创建日期：Nov 7,2016
** 创建人：王喜进 
** 修改记录：
*********************************************************************************/

using DevExpress.XtraEditors;
using GlobleSituation.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MapFrame.Core.Model;
using GlobleSituation.Common;

namespace GlobleSituation.Business
{
    /// <summary>
    /// 从文件提取卫星波束数据
    /// </summary>
    public class ReadBeamData
    {
        #region MyRegion

        /// <summary>
        /// 委托
        /// </summary>
        /// <returns></returns>
        public delegate void PushDataDelete(BeamData beamData);
        public PushDataDelete PushData;

        private Thread readDataThd = null;


        /// <summary>
        /// 构造函数
        /// </summary>
        public ReadBeamData()
        {
            readDataThd = new Thread(new ThreadStart(ReadDataFormLog));
            readDataThd.IsBackground = true;
        }

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            try
            {
                if (readDataThd.ThreadState == (ThreadState.Background | ThreadState.Unstarted))
                {
                    readDataThd.Start();
                }
                else if (readDataThd.ThreadState == (ThreadState.Background | ThreadState.Suspended))
                {
                    readDataThd.Resume();
                }
                else if (readDataThd.ThreadState == (ThreadState.Background | ThreadState.Stopped))
                {
                    XtraMessageBox.Show("本地数据已经提取完成，没有新的波束数据可以继续使用。");
                }
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(ReadBeamData), ex.Message);
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            try
            {
                if (readDataThd.ThreadState == (ThreadState.Background))
                {
                    readDataThd.Suspend();
                }
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(ReadBeamData), ex.Message);
            }
        }

        /// <summary>
        /// 从日志文件读取数据
        /// </summary>
        private void ReadDataFormLog()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "波束运动情况";
            if (!Directory.Exists(path))
            {
                XtraMessageBox.Show("波束文件不存在！");
                return;
            }

            string[] fileArr = Directory.GetFiles(path);
            if (fileArr == null || fileArr.Count() <= 0) return;

            List<FileInfo> fileInfoList = new List<FileInfo>();
            foreach (string file in fileArr)
            {
                FileInfo fi = new FileInfo(file);
                fileInfoList.Add(fi);
            }

            // 排序
            fileInfoList.Sort(ListSort);

            for (int i = 0; i < fileInfoList.Count; i++)
            {
                FileInfo tmpFi = fileInfoList[i];
                string tmpFile = tmpFi.FullName;

                BeamData prevBeamData = null;
                long prevTime = 0;

                // 需注意文件与文件之前的数据并不连续
                using (StreamReader r = new StreamReader(tmpFile))
                {
                    string lineStr;
                    while ((lineStr = r.ReadLine()) != null)
                    {
                        if (lineStr == "") continue;
                        //51742533060: 卫星: 31 波束:37 位置:(  115.66,    34.40,   779.44)

                        string timeStr = lineStr.Substring(0, 13);
                        string idStr = lineStr.Substring(20, 3);
                        string beamStr = lineStr.Substring(29, 2);

                        long time = Convert.ToInt64(timeStr.Trim());
                        int id = Convert.ToInt32(idStr);
                        int beamId = Convert.ToInt32(beamStr);

                        string pointStr = lineStr.Substring(37, 28);
                        string[] pointArr = pointStr.Split(new char[] { ',' });
                        double longtitude = Convert.ToDouble(pointArr[0]);
                        double latitude = Convert.ToDouble(pointArr[1]);
                        double altitude = Convert.ToDouble(pointArr[2]) * 4000;

                        BeamData beamData = new BeamData();
                        beamData.SatelliteId = id;
                        beamData.BeamId = beamId;
                        beamData.Point = new MapLngLat() { Alt = altitude, Lng = longtitude, Lat = latitude };
                        beamData.PointType = altitude > 0 ? 0 : 1;

                        // 第n次取数，与第n-1次进行比较，设置Timer的Interval值，并推送上一包的数据
                        PushData(beamData);

                        if (prevBeamData != null)
                        {
                            TimeSpan ts = DateTime.FromFileTime(time) - DateTime.FromFileTime(prevTime);
                            if (ts.Ticks > 0)
                                Thread.Sleep(500);
                        }

                        prevBeamData = beamData;
                        prevTime = time;
                    }

                    // 推送最后一包数据
                    if (prevBeamData != null)
                        PushData(prevBeamData);
                }
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="fi1"></param>
        /// <param name="fi2"></param>
        /// <returns></returns>
        private int ListSort(FileInfo fi1, FileInfo fi2)
        {
            string name1 = fi1.Name;
            string name2 = fi2.Name;

            int iName1 = Convert.ToInt32(name1.Split(new char[] { '.' })[0]);
            int iName2 = Convert.ToInt32(name2.Split(new char[] { '.' })[0]);

            if (iName1 > iName2)
                return 1;
            else
                return -1;
        }

        #endregion

        #region 慢速

        ///// <summary>
        ///// 计时器
        ///// </summary>
        //private System.Timers.Timer readDataTimer = null;
        ///// <summary>
        ///// 信号量
        ///// </summary>
        //private Semaphore readSemaphore = new Semaphore(0, int.MaxValue);
        ///// <summary>
        ///// 委托
        ///// </summary>
        ///// <returns></returns>
        //public delegate void PushDataDelete(BeamData beamData);
        //public PushDataDelete PushData;


        ///// <summary>
        ///// 构造函数
        ///// </summary>
        //public ReadBeamData()
        //{
        //    readDataTimer = new System.Timers.Timer();
        //    readDataTimer.Interval = 1000;
        //    readDataTimer.Elapsed += new System.Timers.ElapsedEventHandler(readDataTimer_Elapsed);

        //    // 启动线程
        //    ThreadPool.QueueUserWorkItem(obj => { ReadDataFormLog(); });
        //}

        ///// <summary>
        ///// 开始
        ///// </summary>
        //public void Start()
        //{
        //    readDataTimer.Start();
        //}

        ///// <summary>
        ///// 停止
        ///// </summary>
        //public void Stop()
        //{
        //    readDataTimer.Stop();
        //}

        //void readDataTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    readSemaphore.Release();
        //}

        ///// <summary>
        ///// 计算器
        ///// </summary>
        //private long index = 0;

        ///// <summary>
        ///// 从日志文件读取数据
        ///// </summary>
        //private void ReadDataFormLog()
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory + "波束运动情况";
        //    if (!Directory.Exists(path))
        //    {
        //        XtraMessageBox.Show("波束文件不存在！");
        //        return;
        //    }

        //    string[] fileArr = Directory.GetFiles(path);
        //    if (fileArr == null || fileArr.Count() <= 0) return;

        //    List<FileInfo> fileInfoList = new List<FileInfo>();
        //    foreach (string file in fileArr)
        //    {
        //        FileInfo fi = new FileInfo(file);
        //        fileInfoList.Add(fi);
        //    }

        //    // 排序
        //    fileInfoList.Sort(ListSort);

        //    for (int i = 0; i < fileInfoList.Count; i++)
        //    {
        //        FileInfo tmpFi = fileInfoList[i];
        //        string tmpFile = tmpFi.FullName;

        //        BeamData prevBeamData = null;
        //        long prevTime = 0;

        //        // 需注意文件与文件之前的数据并不连续
        //        using (StreamReader r = new StreamReader(tmpFile))
        //        {
        //            string lineStr;
        //            while ((lineStr = r.ReadLine()) != null)
        //            {
        //                if (lineStr == "") continue;
        //                //51742533060: 卫星: 31 波束:37 位置:(  115.66,    34.40,   779.44)

        //                if (index > 0)
        //                    readSemaphore.WaitOne();

        //                string timeStr = lineStr.Substring(0, 13);
        //                string idStr = lineStr.Substring(20, 3);
        //                string beamStr = lineStr.Substring(29, 2);

        //                long time = Convert.ToInt64(timeStr.Trim());
        //                int id = Convert.ToInt32(idStr);
        //                int beamId = Convert.ToInt32(beamStr);

        //                string pointStr = lineStr.Substring(37, 28);
        //                string[] pointArr = pointStr.Split(new char[] { ',' });
        //                double longtitude = Convert.ToDouble(pointArr[0]);
        //                double latitude = Convert.ToDouble(pointArr[1]);
        //                double altitude = Convert.ToDouble(pointArr[2]) * 4000;

        //                BeamData beamData = new BeamData();
        //                beamData.SatelliteId = id;
        //                beamData.BeamId = beamId;
        //                beamData.Point = new MapLngLat() { Alt = altitude, Lng = longtitude, Lat = latitude };
        //                beamData.PointType = altitude > 0 ? 0 : 1;

        //                index++;

        //                if (index == 1)
        //                {
        //                    // 第1次取数，不用执行操作，继续第2次取数
        //                    prevBeamData = beamData;
        //                    prevTime = time;
        //                }
        //                else
        //                {
        //                    // 第n次取数，与第n-1次进行比较，设置Timer的Interval值，并推送上一包的数据
        //                    PushData(prevBeamData);

        //                    if (time - prevTime <= 0)
        //                    {
        //                        readSemaphore.Release();
        //                    }
        //                    else
        //                        readDataTimer.Interval = time - prevTime;

        //                    prevBeamData = beamData;
        //                    prevTime = time;
        //                }
        //            }

        //            // 推送最后一包数据
        //            if (prevBeamData != null)
        //                PushData(prevBeamData);

        //            index = 0;
        //        }
        //    }
        //}

        //private int ListSort(FileInfo fi1, FileInfo fi2)
        //{
        //    string name1 = fi1.Name;
        //    string name2 = fi2.Name;

        //    int iName1 = Convert.ToInt32(name1.Split(new char[] { '.' })[0]);
        //    int iName2 = Convert.ToInt32(name2.Split(new char[] { '.' })[0]);

        //    if (iName1 > iName2)
        //        return 1;
        //    else
        //        return -1;
        //}
        #endregion

    }
}


