using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EffortFramework;
using System;

namespace HDYH 
{
    public class BoilerDetailedLeftBottomItem : BoilerSystemObject, IPointerClickHandler
    {
        [SerializeField]
        private TextMeshProUGUI m_EquipmentNameText;
        [SerializeField]
        private TextMeshProUGUI m_TypeText;
        [SerializeField]
        private TextMeshProUGUI m_ValueText;
        [SerializeField]
        private TextMeshProUGUI m_CheckRepairText;//建议检修
        [SerializeField]
        private TextMeshProUGUI m_CheckChangeText;//建议更换
        [SerializeField]
        private TextMeshProUGUI m_WarnningTimeText;//预警时间
        [SerializeField]
        private Slider m_Slider;//滑动条

        private DREquipment m_DREquipment;

        public void OnPointerClick(PointerEventData eventData)
        {
            //Debug.Log("web开启UI：" + m_DREquipment.Positioncode);
            //WebUtility.RequestOpenWebUI((int)WebUIID.拆解设备详情弹窗, m_DREquipment.Positioncode);
            MessageManager.SendMessage(MessageConst.SelectViewVariantItem, m_DREquipment);
        }

        public void OnShow(DREquipment drequipment) 
        {
            m_DREquipment = drequipment;
            m_EquipmentNameText.text = drequipment.EquipmentName;
            m_TypeText.text = "";
            m_ValueText.text = "";
            m_CheckRepairText.text = $"建议检修:-";
            m_CheckChangeText.text = $"建议更换:-";
            m_WarnningTimeText.text = $"预警:-";
            m_Slider.value = 0;
        }
        /// <summary>
        /// 接到数据后的展示
        /// </summary>
        /// <param name="data"></param>
        public void OnShow(BoilerDetailedPartNetwork.DetailedPartData data)
        {
            m_TypeText.text = data.typeName;
            //float losslength = float.Parse(data.lossLength);
            //float originLength = float.Parse(data.originLength);
            m_ValueText.text = $"({data.lossLength}/{data.originLength}mm)";
            //m_ValueText.text = $"({data.lossLength}/<#A6A6A6>{data.originLength}mm</color>)";

            DateTime OverhaulDate;
            if(DateTime.TryParse(data.sgtOverhaulTime,out OverhaulDate))
            {
                string result = OverhaulDate.ToString("yyyy-MM-dd");
                m_CheckRepairText.text = string.IsNullOrEmpty(data.sgtOverhaulTime) ? $"建议检修:-" : $"建议检修:{result}";
            }

            DateTime ExchangeDate;
            if (DateTime.TryParse(data.sgtExchangeTime, out ExchangeDate))
            {
                string result = ExchangeDate.ToString("yyyy-MM-dd");
                m_CheckChangeText.text = string.IsNullOrEmpty(data.sgtExchangeTime) ? $"建议更换:-" : $"建议更换:{result}";
            }

            m_WarnningTimeText.text = string.IsNullOrEmpty(data.warnTime)? $"预警:-": $"预警:{data.warnTime}";
            m_Slider.value = float.Parse(data.percent);//这个需要改成后端加的新字段
        }
        public void OnShow()
        {
            m_EquipmentNameText.text = m_DREquipment.EquipmentName;
            m_TypeText.text = "正常";
            m_ValueText.text = "";
            m_CheckRepairText.text = $"建议检修:-";
            m_CheckChangeText.text = $"建议更换:-";
            m_WarnningTimeText.text = $"预警:-";
            m_Slider.value = 0;
        }
        public DREquipment DREquipment => m_DREquipment;
    }
}