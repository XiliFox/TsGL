using EffortFramework;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HDYH
{
    public class BoilerWarningAloneFinallyEquip : MonoBehaviour
    {
        [SerializeField]
        private BoilerMaterialEntityScriptableObject m_boilerMaterialEntityScriptableObject;

        public string code; //记录零件code，不要改名

        private MeshCollider m_meshCollider;
        private Material m_material;
        private BoilerWarningData m_equipData;
        private Color m_baseColor;
        private Color m_HDRColor;
        private StringBuilder m_stringBuilder = new StringBuilder();

        // -----------------------------------------------------------------------------------------

        private void Awake()
        {
            m_meshCollider = GetComponent<MeshCollider>();
            m_material = GetComponent<MeshRenderer>().material;
            MessageManager.Register<string>(MessageConst.BoilerWarningAloneChangeCamera, OnChangeCamera);
            MessageManager.Register<Dictionary<string, BoilerWarningData>>(MessageConst.RequestBoilerWarningDataSuccess, OnRequestBoilerWarningDataSuccess);
        }

        private void OnDestroy()
        {
            MessageManager.Unregister<string>(MessageConst.BoilerWarningAloneChangeCamera, OnChangeCamera);
            MessageManager.Unregister<Dictionary<string, BoilerWarningData>>(MessageConst.RequestBoilerWarningDataSuccess, OnRequestBoilerWarningDataSuccess);
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

            Vector3 screenpos = Input.mousePosition;// camera.main.worldtoscreenpoint(input.mouseposition);
            MessageManager.SendMessage(MessageConst.ShowEqipmentInfoPanel, m_stringBuilder.ToString(), screenpos);
        }

        protected void OnMouseExit()
        {
            BoilerWarningData.SetValue(m_material, m_baseColor, m_HDRColor);
            MessageManager.SendMessage(MessageConst.HideEqipmentInfoPanel);
        }

        protected virtual void OnMouseUpAsButton()
        {
            if (m_equipData == null) return;
            WebUtility.RequestOpenWebUI((int)WebUIID.设备详情弹窗, code);  //通知前端弹出设备详情弹窗
            OnChangeCamera(code);
        }

        // -----------------------------------------------------------------------------------------

        private void OnRequestBoilerWarningDataSuccess(Dictionary<string, BoilerWarningData> warningDic)
        {
            m_meshCollider.enabled = false; // 关闭碰撞盒
            (Color baseColor, Color HDRColor) = m_boilerMaterialEntityScriptableObject.GetColor("Normal");

            if (warningDic.ContainsKey(code))
            {
                m_equipData = warningDic[code];

                if (warningDic[code].alarmLevel == "2")
                {
                    m_meshCollider.enabled = true;
                    (baseColor, HDRColor) = m_boilerMaterialEntityScriptableObject.GetColor("Serious");

                }
                if (warningDic[code].alarmLevel == "3")
                {
                    m_meshCollider.enabled = true;
                    (baseColor, HDRColor) = m_boilerMaterialEntityScriptableObject.GetColor("Dangerous");
                }

                m_baseColor = baseColor;
                m_HDRColor = HDRColor;
            }

            BoilerWarningData.SetValue(m_material, baseColor, HDRColor);
        }

        private void OnChangeCamera(string positionCode)
        {
            if (string.IsNullOrEmpty(positionCode)) return;
            if (positionCode == code)
            {
                // 调整视角, 根据零件坐标判定旋转角度。锅炉中心X为10，Y为4
                Vector3 targetPosition = transform.position - new Vector3(0f, 0f, 0f);

                Vector3 targetRotation = new Vector3();
                float posX = transform.localPosition.x + transform.parent.localPosition.x + transform.parent.parent.localPosition.x;
                float posY = transform.localPosition.y + transform.parent.localPosition.y + transform.parent.parent.localPosition.y;
                if (posX >= 10f && posY <= 4f) targetRotation = new Vector3(10f, 40f, 0f);
                if (posX < 10f && posY <= 4f) targetRotation = new Vector3(10f, -50f, 0f);
                if (posX >= 10f && posY > 4f) targetRotation = new Vector3(10f, 140f, 0f);
                if (posX < 10f && posY > 4f) targetRotation = new Vector3(10f, -130f, 0f);

                float distance = RandomUtility.GetRandomValue(new RandomRange(60.0f, 80.0f));

                MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, targetPosition, targetRotation, distance, 0.5f);
            }
        }
    }
}