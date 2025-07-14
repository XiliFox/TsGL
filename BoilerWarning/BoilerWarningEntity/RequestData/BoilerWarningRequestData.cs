using EffortFramework;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace HDYH
{
    public class BoilerWarningRequestData : MonoBehaviour
    {
        private CancellationTokenSource m_tokenSource = new();
        private Dictionary<string, BoilerWarningData> m_BoilerWarningDataDic = new Dictionary<string, BoilerWarningData>();

        // 模块初始化
        public void Initialize()
        {
            MessageManager.Register<string, BoilerWarningRecord, object>(MessageConst.RequestBoilerWarningInfoData, OnRequestBoilerWarningInfoData);
        }

        // 模块关闭
        public void Shutdown()
        {
            MessageManager.Unregister<string, BoilerWarningRecord, object>(MessageConst.RequestBoilerWarningInfoData, OnRequestBoilerWarningInfoData);

            m_tokenSource.Cancel();
            m_tokenSource.Dispose();
            m_tokenSource = null;
        }

        // ------------------------------------------------------------------------------

        public void RequestWarningData()
        {
            string url = string.Format(UrlHelper.BoilerWarningURL, 1);
            WebRequestDataWrapper.Create<BoilerWarningRecord>(WebRequestType.Get,
                url, MessageConst.RequestBoilerWarningInfoData, m_tokenSource.Token);
        }

        public void RequestWarningAloneData()
        {
            string unit = WebUtility.GetUnitFromHTML();
            string url = string.Format(UrlHelper.BoilerWarningURL, unit);
            WebRequestDataWrapper.Create<BoilerWarningRecord>(WebRequestType.Get,
                url, MessageConst.RequestBoilerWarningInfoData, m_tokenSource.Token);
        }

        // ------------------------------------------------------------------------------

        private void OnRequestBoilerWarningInfoData(string msgName, BoilerWarningRecord eventArgs, object userData)
        {
            if (msgName != MessageConst.RequestBoilerWarningInfoData)
                return;

            m_BoilerWarningDataDic.Clear();
            if (eventArgs != null && eventArgs.data != null)
            {
                foreach(var item in eventArgs.data)
                {
                    m_BoilerWarningDataDic.Add(item.positionCode, item);
                }
            }
            MessageManager.SendMessage(MessageConst.RequestBoilerWarningDataSuccess, m_BoilerWarningDataDic);
        }
    }
}