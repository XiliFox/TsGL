using EffortFramework;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace  HDYH
{
    public partial class BoilerDistributionOfFaultPointsRequestData : MonoBehaviour
    {
        public enum PointType
        {
            预警故障点分布,
            检修故障点分布,
        }

        private PointType m_PointType = PointType.预警故障点分布;

        public PointType SetPointType
        {
            get { return m_PointType; }
            set { m_PointType = value; }
        }

        // --------------------------------------------------------------------

        private string m_StartTime = "";//开始时间
        private string m_EndTime = "";//结束时间

        // 预警故障
        private string m_StartTimeAlarm = "";
        private string m_EndTimeAlarm = "";
        private DistributionOfFaultPointsData m_alarmData;

        // 检修故障时间对照
        private string m_StartTimeFix = "";
        private string m_EndTimeFix = "";
        private DistributionOfFaultPointsData m_fixData;

        private CancellationTokenSource m_tokenSource = new();

        // 模块初始化
        public void Initialize()
        {
            MessageManager.Register<string, BoilerDistributionOfFaultPointsRecord, object>(MessageConst.申请预警故障点数据, OnRequestFaultPointsAlarmInfoData);
            MessageManager.Register<string, BoilerDistributionOfFaultPointsRecord, object>(MessageConst.申请检修故障点数据, OnRequestFaultPointsFixInfoData);
            MessageManager.Register<string>(MessageConst.Web端选择日期查询, WebSearchData);
        }

        // 模块关闭
        public void Shutdown()
        {
            MessageManager.Unregister<string, BoilerDistributionOfFaultPointsRecord, object>(MessageConst.申请预警故障点数据, OnRequestFaultPointsAlarmInfoData); 
            MessageManager.Unregister<string, BoilerDistributionOfFaultPointsRecord, object>(MessageConst.申请检修故障点数据, OnRequestFaultPointsFixInfoData);
            MessageManager.Unregister<string>(MessageConst.Web端选择日期查询, WebSearchData);

            m_tokenSource.Cancel();
            m_tokenSource.Dispose();
            m_tokenSource = null;
        }

        public void RequestData()
        {
#if UNITY_EDITOR
            m_StartTime = "2021-01-01 00:00:00";
            m_EndTime = "2025-01-01 00:00:00";
#else
            (bool result, string starTime, string endTime) = GetSelectDataTime();
            if (result)
            {
                m_StartTime = starTime;
                m_EndTime = endTime;
            }
#endif
            if (string.IsNullOrEmpty(m_EndTime) || string.IsNullOrEmpty(m_StartTime))
            {
                Debug.LogError("请求数据失败，未获取到起始时间或结束时间。");
                return;
            }

            if (m_PointType == PointType.预警故障点分布)
            {
                if (m_StartTime != m_StartTimeAlarm || m_EndTime != m_EndTimeAlarm)
                {
                    m_StartTimeAlarm = m_StartTime;
                    m_EndTimeAlarm = m_EndTime;

                    string Url = string.Format(UrlHelper.WarningDistributionOfFaultPoints, m_EndTime, m_StartTime, "1");
                    WebRequestDataWrapper.Create<BoilerDistributionOfFaultPointsRecord>(WebRequestType.Get,
                        Url, MessageConst.申请预警故障点数据, m_tokenSource.Token);
                }
                else
                {
                    MessageManager.SendMessage(MessageConst.故障点分布更新UI数据, m_alarmData);
                    MessageManager.SendMessage(MessageConst.故障点分布更新实体数据, m_alarmData.pCode.ToList());
                }
            }

            if (m_PointType == PointType.检修故障点分布)
            {
                if (m_StartTime != m_StartTimeFix || m_EndTime != m_EndTimeFix)
                {
                    m_StartTimeFix = m_StartTime;
                    m_EndTimeFix = m_EndTime;

                    string Url = string.Format(UrlHelper.CheckDistributionOfFaultPoints, m_EndTime, m_StartTime, "1");
                    WebRequestDataWrapper.Create<BoilerDistributionOfFaultPointsRecord>(WebRequestType.Get,
                        Url, MessageConst.申请检修故障点数据, m_tokenSource.Token);
                }
                else
                {
                    MessageManager.SendMessage(MessageConst.故障点分布更新UI数据, m_fixData);
                    MessageManager.SendMessage(MessageConst.故障点分布更新实体数据, m_fixData.pCode.ToList());
                }
            }
        }

        private void OnRequestFaultPointsAlarmInfoData(string msgName, BoilerDistributionOfFaultPointsRecord result, object userData)
        {
            if (msgName != MessageConst.申请预警故障点数据)
                return;

            if (result.success)
            {
                m_alarmData = result.data;
                MessageManager.SendMessage(MessageConst.故障点分布更新UI数据, m_alarmData);
                MessageManager.SendMessage(MessageConst.故障点分布更新实体数据, m_alarmData.pCode.ToList());

            }
        }

        private void OnRequestFaultPointsFixInfoData(string msgName, BoilerDistributionOfFaultPointsRecord result, object userData)
        {
            if (msgName != MessageConst.申请检修故障点数据)
                return;

            m_fixData = result.data;
            MessageManager.SendMessage(MessageConst.故障点分布更新UI数据, m_fixData);
            MessageManager.SendMessage(MessageConst.故障点分布更新实体数据, m_fixData.pCode.ToList());
        }

        // --------------------------------------------------------------------

        /// <summary>
        /// 获取Web端选择的日期
        /// </summary>
        /// <returns></returns>
        private (bool, string, string) GetSelectDataTime()
        {
            var result = WebUtility.GetDataTimeFromHTML();
            string startTime = "";
            string endTime = "";

            if (!string.IsNullOrEmpty(result))
            {
                var data = result.Split("&");
                if (data.Length == 2)
                {
                    startTime = data[0];
                    endTime = data[1];
                    return (true, startTime, endTime);
                }
                return (false, "", "");
            }
            return (false, "", "");
        }

        /// <summary>
        /// Web端选择查询日期
        /// </summary>
        /// <param name="data"></param>
        private void WebSearchData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var result = data.Split("&");
                if (result.Length == 2)
                {
                    m_StartTime = result[0];
                    m_EndTime = result[1];
                    RequestData();
                }
                else
                    Debug.LogError("日期格式不正确:" + data);
            }
            else
                Debug.LogError("日期为空字符串");
        }
    }
}