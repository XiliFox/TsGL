using UnityEngine;
using EffortFramework;

namespace HDYH
{
    /// <summary>
    /// 锅炉预警单独页面实体
    /// </summary>
    public sealed class BoilerWarningAloneEntity : BoilerEntityBase
    {
        private float timer = 0f;
        private float interval = 10f; // 间隔时间
        private BoilerWarningRequestData m_BoilerWarningRequestData;

        protected override void Awake()
        {
            base.Awake();
            m_BoilerWarningRequestData = GetComponentInChildren<BoilerWarningRequestData>();
            m_BoilerWarningRequestData.Initialize();
            m_BoilerWarningRequestData.RequestWarningAloneData();

            MessageManager.Register(MessageConst.WebHideEquipmentInfoPanel, OnWebHideEquipmentInfoPanel);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            m_BoilerWarningRequestData.Shutdown();

            MessageManager.Unregister(MessageConst.WebHideEquipmentInfoPanel, OnWebHideEquipmentInfoPanel);
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                m_BoilerWarningRequestData.RequestWarningAloneData();
                timer = 0f;
            }
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            string PositionCode = string.Empty;

#if !UNITY_EDITOR && UNITY_WEBGL
            var param = WebUtility.GetHtmlUrlParam();
            if (!string.IsNullOrWhiteSpace(param))
            {
                PositionCode = WebUtility.GetURLParam(param, "PositionCode");//场景编号
                Debug.Log($"PositionCode is {PositionCode}");
            }
#endif
            if (string.IsNullOrEmpty(PositionCode))
            {
                OnWebHideEquipmentInfoPanel(); // 此时为通过进入首页面预警三维首页的方式进入。镜头拉到最大视角
            }
            else
            {
                //此时为用户通过在线监测页面点击设备进入，PositionCode就是用户点击的设备号,对应Equipment.csv表里的Positioncode
                //镜头靠近选中的设备，并通知前端弹出设备详情弹窗。
                MessageManager.SendMessage(MessageConst.BoilerWarningAloneChangeCamera, PositionCode);
            }

            m_BoilerWarningRequestData.RequestWarningAloneData();
        }

        //------------------------------------------------------------------------------------------

        /// <summary>
        /// web端点击设备信息弹窗关闭按钮触发=>此方法由Web端来触发
        /// </summary>
        private void OnWebHideEquipmentInfoPanel()
        {
            // 镜头拉到最大视角
            MessageManager.SendMessage(MessageConst.SetCameraOffsetPos, new Vector3(-30, 0, 0));
            MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, new Vector3(0, 50, 0), new Vector3(10, 20, 0), 130f, 0.5f);
        }
    }

    public partial class MessageConst
    {
        // 摄像机跳转到对应视角
        public const string BoilerWarningAloneChangeCamera = "BoilerWarningAloneChangeCamera";
        // Web端主动关闭设备详情弹窗
        public const string WebHideEquipmentInfoPanel = "WebHideEquipmentInfoPanel";
    }
}
