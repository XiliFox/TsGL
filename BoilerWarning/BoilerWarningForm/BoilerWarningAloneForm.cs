using EffortFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HDYH
{
    // 锅炉预警单独页面
    public class BoilerWarningAloneForm : UGuiForm
    {
        [SerializeField, LabelText("设备信息")]
        private EquipmentInfoPanel m_EquipmentInfoPanel = null;

        private void Awake()
        {
            MessageManager.Register<string, Vector3, string>(MessageConst.ShowEqipmentInfoPanel, ShowEquipmentInfoPanel);
            MessageManager.Register(MessageConst.HideEqipmentInfoPanel, HideEquipmentInfoPanel);
        }

        private void OnDestroy()
        {
            MessageManager.Unregister<string, Vector3, string>(MessageConst.ShowEqipmentInfoPanel, ShowEquipmentInfoPanel);
            MessageManager.Unregister(MessageConst.HideEqipmentInfoPanel, HideEquipmentInfoPanel);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            HideEquipmentInfoPanel();
            WebUtility.RequestOpenWebUI((int)WebUIID.受热面左侧弹窗,"");
        }

        private void ShowEquipmentInfoPanel(string value, Vector3 targetPosition, string alarmLevel)
        {
            if (m_EquipmentInfoPanel != null)
                m_EquipmentInfoPanel.ShowWithInfo(value, targetPosition, alarmLevel);
        }

        private void HideEquipmentInfoPanel()
        {
            if (m_EquipmentInfoPanel != null)
                m_EquipmentInfoPanel.HidePanel();
        }
    }
}