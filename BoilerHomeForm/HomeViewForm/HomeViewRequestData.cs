using EffortFramework;
using System.Threading;
using UnityEngine;

namespace HDYH
{
    public class HomeRequestData : MonoBehaviour
    {
        private CancellationTokenSource m_tokenSource = new();
        private HomeViewData homeViewData = null;

        // 模块初始化
        public void Initialize()
        {
            MessageManager.Register<string, HomeViewRecord, object>(MessageConst.RequestHomeInfoData, OnRequestHomeInfoData);
        }

        // 模块关闭
        public void Shutdown()
        {
            MessageManager.Unregister<string, HomeViewRecord, object>(MessageConst.RequestHomeInfoData, OnRequestHomeInfoData);

            m_tokenSource.Cancel();
            m_tokenSource.Dispose();
            m_tokenSource = null;
        }

        // ------------------------------------------------------------------------------

        public void RequestHomeViewData()
        {
            string url = string.Format(UrlHelper.HomeViewURL, HomeViewForm.BoilerUnit);
            WebRequestDataWrapper.Create<HomeViewRecord>(WebRequestType.Get,
                url, MessageConst.RequestHomeInfoData, m_tokenSource.Token);
        }

        // ------------------------------------------------------------------------------

        private void OnRequestHomeInfoData(string msgName, HomeViewRecord eventArgs, object userData)
        {
            if (msgName != MessageConst.RequestHomeInfoData)
                return;

            if (eventArgs != null && eventArgs.data != null)
            {
                homeViewData = eventArgs.data;
            }

            MessageManager.SendMessage(MessageConst.RequestHomeDataSuccess, homeViewData);
        }
    }
}