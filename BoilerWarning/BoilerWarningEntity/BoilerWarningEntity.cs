using EffortFramework;
using UnityEngine;

namespace HDYH
{
    // 锅炉预警实体
    public sealed class BoilerWarningEntity : BoilerEntityBase
    {
        private float timer = 0f;
        private float interval = 10f; // 间隔时间
        private BoilerWarningRequestData m_BoilerWarningRequestData;

        protected override void Awake()
        {
            base.Awake();
            m_BoilerWarningRequestData = GetComponentInChildren<BoilerWarningRequestData>();
            m_BoilerWarningRequestData.Initialize();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            m_BoilerWarningRequestData.Shutdown();
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            MessageManager.SendMessage(MessageConst.SetCameraOffsetPos, new Vector3(-35, 0, 0));
            MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, new Vector3(0, 50, 0), new Vector3(0, 35, 0), 130f, 0.5f);
        }

        // ===请求数据===---------------------------------------------------------------------------

        private void OnEnable()
        {
            m_BoilerWarningRequestData.RequestWarningData();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                m_BoilerWarningRequestData.RequestWarningData();
                timer = 0f;
            }
        }
    }
}