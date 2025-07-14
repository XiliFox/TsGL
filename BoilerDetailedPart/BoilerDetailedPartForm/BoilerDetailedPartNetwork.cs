using EffortFramework;
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDYH 
{
    public partial class BoilerDetailedPartNetwork : BoilerSystemNetworkBase
    {
        private DetailedPartPostData m_DetailedPartPostData = null;
        protected override void Awake()
        {
            base.Awake();
            MessageManager.Register<string, DetailedPartWebData, object>(MessageConst.请求设备详情数据, this.OnReceiveData);
        }
        private void OnDestroy()
        {
            MessageManager.Unregister<string, DetailedPartWebData, object>(MessageConst.请求设备详情数据, this.OnReceiveData);
        }
        public override void RequestData()
        {
            if (!m_IsAutoRequest)
                m_IsAutoRequest = true;
            m_time = 0.0f;
            RequestLeftBottomData();
        }

        private void RequestLeftBottomData() 
        {
            if (m_DetailedPartPostData == null)
                return;

            string jsondata = Utility.Json.ToJson(m_DetailedPartPostData);
            RequestDataPost<DetailedPartWebData>(UrlHelper.WarnPostionDetail,MessageConst.请求设备详情数据, jsondata);
        }
        public void SetData(DetailedPartPostData detailedPartPostData) => m_DetailedPartPostData = detailedPartPostData;

        /// <summary>
        /// 接收后端发来的数据
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="result"></param>
        /// <param name="userData"></param>
        private void OnReceiveData(string msgName, DetailedPartWebData result, object userData)
        {
            if (result.success)
                MessageManager.SendMessage(MessageConst.更新设备详情UI, result.data);
        }
    }
    public static partial class MessageConst
    {
        public const string 请求设备详情数据 = "请求设备详情数据";
        public const string 更新设备详情UI = "更新设备详情UI";
    }
}
