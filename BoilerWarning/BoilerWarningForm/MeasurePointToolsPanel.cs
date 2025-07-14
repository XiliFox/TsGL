using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace HDYH
{
    public class MeasurePointToolsPanel : MonoBehaviour
    {
        [SerializeField]
        private Transform m_Root = null;

        [SerializeField]
        private Transform m_PanelTips = null;

        [SerializeField]
        private Button m_AddBtn = null;

        [SerializeField]
        private Button m_CloseBtn = null;

        private string m_MeasurePoint = null;

        void Start()
        {
            m_Root.gameObject.SetActive(false);
            m_CloseBtn.onClick.AddListener(OnHide);
            m_AddBtn.onClick.AddListener(OnClickButtonAdd);
        }

        private void OnDestroy()
        {
            m_CloseBtn.onClick.RemoveListener(OnHide);
            m_AddBtn.onClick.RemoveListener(OnClickButtonAdd);
        }

        public void OnOpen(string value, Vector3 targetPosition)
        {
            m_Root.gameObject.SetActive(true);
            m_PanelTips.position = targetPosition;
            value = Regex.Replace(value, @"(<sub>|</sub>)", "");
            m_MeasurePoint = value;
        }

        public void OnHide()
        {
            m_MeasurePoint = "";
            m_Root.gameObject.SetActive(false);
        }

        private void OnClickButtonAdd()
        {
            if (!string.IsNullOrEmpty(m_MeasurePoint))
            {
                WebUtility.RequestOpenWebUI((int)WebUIID.添加测点值, m_MeasurePoint);
            }
            OnHide();
        }
    }
}