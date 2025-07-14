using EffortFramework;
using System.Text;
using UnityEngine;

namespace HDYH
{
    /// <summary>
    /// 锅炉故障点
    /// </summary>
    public class BoilerFaultPoints : MonoBehaviour
    {
        public string PositionCode;
        public string EquipmentName;
        public MeshRenderer m_MeshRenderer;//故障点绑定的mesh
        private PCode m_Data = null;
        private StringBuilder m_StringBuilder = new StringBuilder();
        [SerializeField]
        private BoilerFaultPointsScriptableObject m_BoilerFaultPointsScriptableObject;
        /// <summary>
        /// 更新故障点数据
        /// </summary>
        /// <param name="max"></param>
        /// <param name="value"></param>
        public void UpdateValue(float max, PCode value)
        {
            m_Data = value;

            if (max == 0)
                return;
            transform.localScale = Vector3.Lerp(AuxiliaryClass.PointMinValue, AuxiliaryClass.PointMaxValue, value.num / max);
        }
        private void OnMouseEnter()
        {
            m_StringBuilder.Clear();
            Vector3 screenPos = Input.mousePosition;
            m_StringBuilder.Append(EquipmentName);
            m_StringBuilder.Append(":");
            m_StringBuilder.Append(m_Data.value);
            MessageManager.SendMessage(MessageConst.ShowEqipmentInfoPanel, m_StringBuilder.ToString(), screenPos);
            SetMaterialValue(m_MeshRenderer, m_BoilerFaultPointsScriptableObject.m_EnterColor);
        }

        private void OnMouseExit()
        {
            MessageManager.SendMessage(MessageConst.HideEqipmentInfoPanel);
            SetMaterialValue(m_MeshRenderer, m_BoilerFaultPointsScriptableObject.m_BaseColor);
        }
        private void SetMaterialValue(MeshRenderer targetmesh, Color targetcolor) => targetmesh.material.SetColor("_EmissionColor", targetcolor);
    }
}
