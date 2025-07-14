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
            Ԥ�����ϵ�ֲ�,
            ���޹��ϵ�ֲ�,
        }

        private PointType m_PointType = PointType.Ԥ�����ϵ�ֲ�;

        public PointType SetPointType
        {
            get { return m_PointType; }
            set { m_PointType = value; }
        }

        // --------------------------------------------------------------------

        private string m_StartTime = "";//��ʼʱ��
        private string m_EndTime = "";//����ʱ��

        // Ԥ������
        private string m_StartTimeAlarm = "";
        private string m_EndTimeAlarm = "";
        private DistributionOfFaultPointsData m_alarmData;

        // ���޹���ʱ�����
        private string m_StartTimeFix = "";
        private string m_EndTimeFix = "";
        private DistributionOfFaultPointsData m_fixData;

        private CancellationTokenSource m_tokenSource = new();

        // ģ���ʼ��
        public void Initialize()
        {
            MessageManager.Register<string, BoilerDistributionOfFaultPointsRecord, object>(MessageConst.����Ԥ�����ϵ�����, OnRequestFaultPointsAlarmInfoData);
            MessageManager.Register<string, BoilerDistributionOfFaultPointsRecord, object>(MessageConst.������޹��ϵ�����, OnRequestFaultPointsFixInfoData);
            MessageManager.Register<string>(MessageConst.Web��ѡ�����ڲ�ѯ, WebSearchData);
        }

        // ģ��ر�
        public void Shutdown()
        {
            MessageManager.Unregister<string, BoilerDistributionOfFaultPointsRecord, object>(MessageConst.����Ԥ�����ϵ�����, OnRequestFaultPointsAlarmInfoData); 
            MessageManager.Unregister<string, BoilerDistributionOfFaultPointsRecord, object>(MessageConst.������޹��ϵ�����, OnRequestFaultPointsFixInfoData);
            MessageManager.Unregister<string>(MessageConst.Web��ѡ�����ڲ�ѯ, WebSearchData);

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
                Debug.LogError("��������ʧ�ܣ�δ��ȡ����ʼʱ������ʱ�䡣");
                return;
            }

            if (m_PointType == PointType.Ԥ�����ϵ�ֲ�)
            {
                if (m_StartTime != m_StartTimeAlarm || m_EndTime != m_EndTimeAlarm)
                {
                    m_StartTimeAlarm = m_StartTime;
                    m_EndTimeAlarm = m_EndTime;

                    string Url = string.Format(UrlHelper.WarningDistributionOfFaultPoints, m_EndTime, m_StartTime, "1");
                    WebRequestDataWrapper.Create<BoilerDistributionOfFaultPointsRecord>(WebRequestType.Get,
                        Url, MessageConst.����Ԥ�����ϵ�����, m_tokenSource.Token);
                }
                else
                {
                    MessageManager.SendMessage(MessageConst.���ϵ�ֲ�����UI����, m_alarmData);
                    MessageManager.SendMessage(MessageConst.���ϵ�ֲ�����ʵ������, m_alarmData.pCode.ToList());
                }
            }

            if (m_PointType == PointType.���޹��ϵ�ֲ�)
            {
                if (m_StartTime != m_StartTimeFix || m_EndTime != m_EndTimeFix)
                {
                    m_StartTimeFix = m_StartTime;
                    m_EndTimeFix = m_EndTime;

                    string Url = string.Format(UrlHelper.CheckDistributionOfFaultPoints, m_EndTime, m_StartTime, "1");
                    WebRequestDataWrapper.Create<BoilerDistributionOfFaultPointsRecord>(WebRequestType.Get,
                        Url, MessageConst.������޹��ϵ�����, m_tokenSource.Token);
                }
                else
                {
                    MessageManager.SendMessage(MessageConst.���ϵ�ֲ�����UI����, m_fixData);
                    MessageManager.SendMessage(MessageConst.���ϵ�ֲ�����ʵ������, m_fixData.pCode.ToList());
                }
            }
        }

        private void OnRequestFaultPointsAlarmInfoData(string msgName, BoilerDistributionOfFaultPointsRecord result, object userData)
        {
            if (msgName != MessageConst.����Ԥ�����ϵ�����)
                return;

            if (result.success)
            {
                m_alarmData = result.data;
                MessageManager.SendMessage(MessageConst.���ϵ�ֲ�����UI����, m_alarmData);
                MessageManager.SendMessage(MessageConst.���ϵ�ֲ�����ʵ������, m_alarmData.pCode.ToList());

            }
        }

        private void OnRequestFaultPointsFixInfoData(string msgName, BoilerDistributionOfFaultPointsRecord result, object userData)
        {
            if (msgName != MessageConst.������޹��ϵ�����)
                return;

            m_fixData = result.data;
            MessageManager.SendMessage(MessageConst.���ϵ�ֲ�����UI����, m_fixData);
            MessageManager.SendMessage(MessageConst.���ϵ�ֲ�����ʵ������, m_fixData.pCode.ToList());
        }

        // --------------------------------------------------------------------

        /// <summary>
        /// ��ȡWeb��ѡ�������
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
        /// Web��ѡ���ѯ����
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
                    Debug.LogError("���ڸ�ʽ����ȷ:" + data);
            }
            else
                Debug.LogError("����Ϊ���ַ���");
        }
    }
}