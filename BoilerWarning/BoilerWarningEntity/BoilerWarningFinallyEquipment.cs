using EffortFramework;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HDYH
{
    public class BoilerWarningFinallyEquipment : MonoBehaviour
    {
        [SerializeField]
        private BoilerMaterialEntityScriptableObject m_boilerMaterialEntityScriptableObject;

        public string Code; //记录零件code，不要改名

        private MeshCollider m_meshCollider;
        private Material m_material;
        private BoilerWarningData m_equipData;
        private Color m_baseColor;
        private Color m_HDRColor;
        private StringBuilder m_stringBuilder = new StringBuilder();

        private void Awake()
        {
            m_meshCollider = GetComponent<MeshCollider>();
            m_material = GetComponent<MeshRenderer>().material;
            MessageManager.Register<Dictionary<string, BoilerWarningData>>(MessageConst.RequestBoilerWarningDataSuccess, OnRequestBoilerWarningDataSuccess);
        }

        private void OnDestroy()
        {
            MessageManager.Unregister<Dictionary<string, BoilerWarningData>>(MessageConst.RequestBoilerWarningDataSuccess, OnRequestBoilerWarningDataSuccess);
        }

        private void OnRequestBoilerWarningDataSuccess(Dictionary<string, BoilerWarningData> warningDic)
        {
            m_meshCollider.enabled = false; // 关闭碰撞盒
            (Color baseColor, Color HDRColor) = m_boilerMaterialEntityScriptableObject.GetColor("Normal");

            if (warningDic.ContainsKey(Code))
            {
                m_equipData = warningDic[Code];

                if (m_equipData.alarmLevel == "1")
                {
                    m_meshCollider.enabled = true;
                    (baseColor, HDRColor) = m_boilerMaterialEntityScriptableObject.GetColor("General");
                }

                if (m_equipData.alarmLevel == "2")
                {
                    m_meshCollider.enabled = true;
                    (baseColor, HDRColor) = m_boilerMaterialEntityScriptableObject.GetColor("Serious");
                }

                if (m_equipData.alarmLevel == "3")
                {
                    m_meshCollider.enabled = true;
                    (baseColor, HDRColor) = m_boilerMaterialEntityScriptableObject.GetColor("Dangerous");
                }

                m_baseColor = baseColor;
                m_HDRColor = HDRColor;
            }

            BoilerWarningData.SetValue(m_material, baseColor, HDRColor);
        }

        // -----------------------------------------------------------------------------------------

        protected void OnMouseEnter()
        {
            if (m_equipData == null) return;

            (Color selectColor, Color selectHDRColor) = m_boilerMaterialEntityScriptableObject.GetColor("Select");
            BoilerWarningData.SetValue(m_material, selectColor, selectHDRColor);

            m_stringBuilder.Clear();
            m_stringBuilder.Append(transform.name);
            m_stringBuilder.Append(" ");
            m_stringBuilder.Append(BoilerWarningData.GetDamageType(m_equipData.damageType));
            m_stringBuilder.Append(" ");
            m_stringBuilder.Append(HomeViewForm.StringToStandard(m_equipData.value));
            m_stringBuilder.Append("/");
            m_stringBuilder.Append(m_equipData.thickness);
            m_stringBuilder.Append("mm");

            Vector3 screenpos = Input.mousePosition;
            MessageManager.SendMessage(MessageConst.ShowWarningEqipInfoPanel, m_stringBuilder.ToString(), screenpos, m_equipData.alarmLevel);
        }

        protected void OnMouseExit()
        {
            BoilerWarningData.SetValue(m_material, m_baseColor, m_HDRColor);
            MessageManager.SendMessage(MessageConst.HideEqipmentInfoPanel);
        }

        protected virtual void OnMouseUpAsButton()
        {
            if (m_equipData == null) return;
            WebUtility.RequestOpenUrl((int)UrlID.受热面预警页面, $"{HomeViewForm.BoilerUnit}&{Code}");
        }
    }
}