using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HDYH 
{
    public class ToggleUtility : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI[] textMeshProUGUIs;

        private Toggle m_Toggle = null;

        [SerializeField]
        private Color m_SelectColor = Color.white;

        [SerializeField]
        private Color m_DisSelectColor = Color.white;

        void Start() 
        {
            m_Toggle = GetComponent<Toggle>();
            m_Toggle.onValueChanged.AddListener(OnValueChanged);
        } 

        private void OnValueChanged(bool state) 
        {
            if (state)
                foreach (var item in textMeshProUGUIs)
                    item.color = m_SelectColor;
            else
                foreach (var item in textMeshProUGUIs)
                    item.color = m_DisSelectColor;
        }
        private void OnDestroy() => m_Toggle.onValueChanged.RemoveListener(OnValueChanged);
    }
}
