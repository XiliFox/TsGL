using UnityEngine;
using TMPro;
using EffortFramework;
using UnityEngine.UI;

namespace HDYH 
{
    public class EquipmentInfoPanel : MonoBehaviour
    {
        [SerializeField]
        private Transform m_Root = null;

        [SerializeField]
        private TextMeshProUGUI m_TMP_Text = null;

        [SerializeField]
        private Image m_ImageRoot;
        [SerializeField]
        private Sprite m_ImageGeneral;
        [SerializeField]
        private Sprite m_ImageSerious;
        [SerializeField]
        private Sprite m_ImageDangerous;

        private RectTransform m_RectTransform = null;
        private void Awake()
        {
            m_RectTransform = transform.parent.GetComponent<RectTransform>();
            MessageManager.Register<string, Vector3, string>(MessageConst.ShowWarningEqipInfoPanel, ShowWithInfo);
            MessageManager.Register(MessageConst.HideEqipmentInfoPanel, HidePanel);

        }
        private void OnDestroy()
        {
            MessageManager.Unregister<string, Vector3, string>(MessageConst.ShowWarningEqipInfoPanel, ShowWithInfo);
            MessageManager.Unregister(MessageConst.HideEqipmentInfoPanel, HidePanel);
        }

        private void OnEnable()
        {
            HidePanel();
        }

        public void ShowWithInfo(string value, Vector3 targetPosition, string alarmLevel)
        {
            m_Root.gameObject.SetActive(true);

            if (alarmLevel == "1") m_ImageRoot.sprite = m_ImageGeneral;
            if (alarmLevel == "2") m_ImageRoot.sprite = m_ImageSerious;
            if (alarmLevel == "3") m_ImageRoot.sprite = m_ImageDangerous;

            m_TMP_Text.text = value;
            transform.position = targetPosition;
        }
        public void HidePanel() => m_Root.gameObject.SetActive(false);
    }
}