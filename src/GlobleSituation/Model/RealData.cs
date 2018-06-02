using System;
using System.Text;
using System.Data;

namespace GlobleSituation.Model
{
    public class RealData
    {
        long targetNum;

        /// <summary>
        /// 目标编号
        /// </summary>
        public long TargetNum
        {
            get { return targetNum; }
            set { targetNum = value; }
        }
        byte informationSource;

        /// <summary>
        /// 信息来源
        /// </summary>
        public byte InformationSource
        {
            get { return informationSource; }
            set { informationSource = value; }
        }
        short country;

        /// <summary>
        /// 国家
        /// </summary>
        public short Country
        {
            get { return country; }
            set { country = value; }
        }
        byte targetProperty;

        /// <summary>
        /// 目标性质
        /// </summary>
        public byte TargetProperty
        {
            get { return targetProperty; }
            set { targetProperty = value; }
        }
        byte targetType;

        /// <summary>
        /// 目标类别 
        /// 0-空中目标；1-陆地目标；2-海洋目标；3-未知目标
        /// </summary>
        public byte TargetType
        {
            get { return targetType; }
            set { targetType = value; }
        }

        string equipModelNumber;
        /// <summary>
        /// 装备型号
        /// </summary>
        public string EquipModelNumber
        {
            get { return equipModelNumber; }
            set { equipModelNumber = value; }
        }

        long positionDate;
        /// <summary>
        /// 位置时间
        /// </summary>
        public long PositionDate
        {
            get { return positionDate; }
            set { positionDate = value; }
        }
        double longitude;
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }
        double latitude;
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
        double altitude;

        /// <summary>
        /// 高深度
        /// </summary>
        public double Altitude
        {
            get { return altitude; }
            set { altitude = value; }
        }

        double scanRange;
        /// <summary>
        /// 视野范围
        /// </summary>
        public double ScanRange
        {
            get { return scanRange; }
            set { scanRange = value; }
        }

        double actionRange;
        /// <summary>
        /// 行动范围 = 改成航向
        /// </summary>
        public double ActionRange
        {
            get { return actionRange; }
            set { actionRange = value; }
        }

        public byte[] ToDataBytes()
        {
            byte[] data = new byte[69];
            Buffer.BlockCopy(BitConverter.GetBytes(targetNum), 0, data, 0, BitConverter.GetBytes(targetNum).Length);
            Buffer.BlockCopy(BitConverter.GetBytes(informationSource), 0, data, 8, BitConverter.GetBytes(informationSource).Length);
            Buffer.BlockCopy(BitConverter.GetBytes(country), 0, data, 9, BitConverter.GetBytes(country).Length);
            Buffer.BlockCopy(BitConverter.GetBytes(targetProperty), 0, data, 11, BitConverter.GetBytes(targetProperty).Length);
            Buffer.BlockCopy(BitConverter.GetBytes(targetType), 0, data, 12, BitConverter.GetBytes(targetType).Length);
            byte[] d = Encoding.UTF8.GetBytes(equipModelNumber);
            Buffer.BlockCopy(d, 0, data, 13, d.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(positionDate), 0, data, 21, BitConverter.GetBytes(positionDate).Length);
            Buffer.BlockCopy(BitConverter.GetBytes(longitude), 0, data, 29, BitConverter.GetBytes(longitude).Length);
            Buffer.BlockCopy(BitConverter.GetBytes(latitude), 0, data, 37, BitConverter.GetBytes(latitude).Length);//
            Buffer.BlockCopy(BitConverter.GetBytes(altitude), 0, data, 45, BitConverter.GetBytes(altitude).Length);
            Buffer.BlockCopy(BitConverter.GetBytes(scanRange), 0, data, 53, BitConverter.GetBytes(scanRange).Length);
            Buffer.BlockCopy(BitConverter.GetBytes(actionRange), 0, data, 61, BitConverter.GetBytes(actionRange).Length);
            return data;
        }

        public static RealData ToRealData(byte[] data)
        {
            RealData realData = new RealData()
            {
                targetNum = BitConverter.ToInt64(data, 0),
                informationSource = data[8],
                country = BitConverter.ToInt16(data, 9),
                targetProperty = data[11],
                targetType = data[12],
                equipModelNumber = System.Text.Encoding.UTF8.GetString(data, 13, 8),
                positionDate = BitConverter.ToInt64(data, 21),
                longitude = BitConverter.ToDouble(data, 29),
                latitude = BitConverter.ToDouble(data, 37),
                altitude = BitConverter.ToDouble(data, 45),
                scanRange = BitConverter.ToDouble(data, 53),
                actionRange = BitConverter.ToDouble(data, 61),
            };
            return realData;
        }

        public static RealData ToRealData(DataRow row)
        {
            try
            {
                RealData realData = new RealData()
                {
                    targetNum = Convert.ToInt64(row["TargetNum"]),
                    informationSource = Convert.ToByte(row["InformationSource"]),
                    country = Convert.ToInt16(row["Country"]),
                    targetProperty = Convert.ToByte(row["TargetProperty"]),
                    targetType = Convert.ToByte(row["TargetType"]),
                    equipModelNumber = row["EquipModelNumber"].ToString(),
                    positionDate = Convert.ToInt64(row["PositionDate"]),
                    longitude = Convert.ToDouble(row["Longitude"]),
                    latitude = Convert.ToDouble(row["Latitude"]),
                    altitude = Convert.ToDouble(row["Altitude"]),
                    scanRange = Convert.ToDouble(row["ScanRange"]),
                    actionRange = Convert.ToDouble(row["ActionRange"])
                };
                return realData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override string ToString()
        {
            try
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.AppendFormat("目标编号：{0}\r\n", this.TargetNum);
                strBuilder.AppendFormat("经度：{0:F4}\r\n", this.Longitude);
                strBuilder.AppendFormat("纬度：{0:F4}\r\n", this.Latitude);
                strBuilder.AppendFormat("高度：{0:F4}\r\n", this.Altitude);
                strBuilder.AppendFormat("时间：{0}\r\n", DateTime.FromFileTime(this.PositionDate).ToString("yyyy-MM-dd HH:mm:ss"));
                strBuilder.AppendFormat("来源：{0}\r\n", this.InformationSource);
                strBuilder.AppendFormat("国家：{0}\r\n", this.Country);
                strBuilder.AppendFormat("性质：{0}\r\n", this.TargetProperty);
                strBuilder.AppendFormat("型号：{0}\r\n", this.EquipModelNumber);
                strBuilder.AppendFormat("视野范围：{0:F4}\r\n", this.ScanRange);
                strBuilder.AppendFormat("行动范围：{0:F4}\r\n", this.ActionRange);

                return strBuilder.ToString();
            }
            catch (Exception ex)
            {
                return "未能获取详细信息\r\n异常：" + ex.Message;
            }
        }
    }
}