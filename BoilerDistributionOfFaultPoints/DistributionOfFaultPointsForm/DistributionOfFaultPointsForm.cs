using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EffortFramework;
using TMPro;
using System.Linq;
using DoTweenAnimationUtility;
using Sirenix.OdinInspector;

namespace HDYH
{
    public class DistributionOfFaultPointsForm : UGuiForm
    {
        [SerializeField, BoxGroup("入场动画"), LabelText("入场动画")]
        private DoTweenArtDirector m_DoTweenArtDirector;

        [SerializeField, BoxGroup("请求数据"),LabelText("请求数据")] 
        private BoilerDistributionOfFaultPointsRequestData m_RequestData;
        [SerializeField, BoxGroup("请求数据"), LabelText("数据请求间隔")]
        private float m_RequestDataInterval = 10f;

        [SerializeField, BoxGroup("故障点"),LabelText("预警故障点分布")] 
        private Toggle m_WarningPointPart;
        [SerializeField, BoxGroup("故障点"),LabelText("检修故障点分布")]
        private Toggle m_OverhaulPointPart;

        [SerializeField, BoxGroup("部件切换"), LabelText("总览")]
        private Toggle m_Overview = null;
        [SerializeField, BoxGroup("部件切换"), LabelText("过热器")]
        private Toggle m_Superheater = null;
        [SerializeField, BoxGroup("部件切换"), LabelText("再热器")]
        private Toggle m_Reheater = null;
        [SerializeField, BoxGroup("部件切换"), LabelText("水冷壁")]
        private Toggle m_WaterWall = null;
        [SerializeField, BoxGroup("部件切换"), LabelText("省煤器")]
        private Toggle m_Economizer = null;

        [SerializeField]
        private List<DistributionStruct> m_DistributionStruct = new List<DistributionStruct>();

        private float timer = 0.0f;

        //-------------------------------------------------------------------------------

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_RequestData.Initialize();
            MessageManager.Register<DistributionOfFaultPointsData>(MessageConst.故障点分布更新UI数据, UpDateUI);


            m_RequestData.RequestData();

            m_WarningPointPart.onValueChanged.AddListener(OnClickToggleWarningPointPart);
            m_OverhaulPointPart.onValueChanged.AddListener(OnClickTogglemOverhaulPointPart);

            m_Overview.onValueChanged.AddListener(OnClickTogglem);
            m_Superheater.onValueChanged.AddListener(OnClickTogglem);
            m_Reheater.onValueChanged.AddListener(OnClickTogglem);
            m_WaterWall.onValueChanged.AddListener(OnClickTogglem);
            m_Economizer.onValueChanged.AddListener(OnClickTogglem);
        }

        private void OnDestroy()
        {
            m_RequestData.Shutdown();
            MessageManager.Unregister<DistributionOfFaultPointsData>(MessageConst.故障点分布更新UI数据, UpDateUI);

            m_WarningPointPart.onValueChanged.RemoveListener(OnClickToggleWarningPointPart);
            m_OverhaulPointPart.onValueChanged.RemoveListener(OnClickTogglemOverhaulPointPart);

            m_Overview.onValueChanged.RemoveListener(OnClickTogglem);
            m_Superheater.onValueChanged.RemoveListener(OnClickTogglem);
            m_Reheater.onValueChanged.RemoveListener(OnClickTogglem);
            m_WaterWall.onValueChanged.RemoveListener(OnClickTogglem);
            m_Economizer.onValueChanged.RemoveListener(OnClickTogglem);
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= m_RequestDataInterval)
            {
                m_RequestData.RequestData();
            }
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            MessageManager.SendMessage(MessageConst.HideEqipmentInfoPanel);
            WebUtility.RequestOpenWebUI((int)WebUIID.日期选择弹窗, "");
            m_DoTweenArtDirector.AnimationControl(DoTweenArtType.Play);
        }


        //-------------------------------------------------------------------------------

        // 预警故障点分布
        private void OnClickToggleWarningPointPart(bool state)
        {
            if (state)
            {
                m_RequestData.SetPointType = BoilerDistributionOfFaultPointsRequestData.PointType.预警故障点分布;
                m_RequestData.RequestData();
            }
        }

        // 检修故障点分布
        private void OnClickTogglemOverhaulPointPart(bool state)
        {
            if (state)
            {
                m_RequestData.SetPointType = BoilerDistributionOfFaultPointsRequestData.PointType.检修故障点分布;
                m_RequestData.RequestData();
            }
        }

        // 切换按钮
        private void OnClickTogglem(bool state)
        {
            if (state)
                MessageManager.SendMessage(MessageConst.更新故障点视角, GetToggleIsOnName());
        }

        private string GetToggleIsOnName()
        {
            string equipmentName = "";

            foreach (var item in m_DistributionStruct)
            {
                if (item.Toggle.isOn)
                    equipmentName = item.ToggleName;
            }
            return equipmentName;
        }

        //-------------------------------------------------------------------------------

        private void UpDateUI(DistributionOfFaultPointsData data)
        {
            if (data.oneLevel == null) return;
            var result = data.oneLevel.ToList();
            int value = 0;
            for (int i = 0; i < m_DistributionStruct.Count; i++)
            {
                var resultData = result.Find(x => x.onelevel == m_DistributionStruct[i].ToggleName);
                if (resultData != null)
                {
                    m_DistributionStruct[i].UGUIText.text = resultData.num.ToString();
                    value += resultData.num;
                }
                else
                {
                    m_DistributionStruct[i].UGUIText.text = "0";
                }
            }
            var overView = m_DistributionStruct.Find(x => x.ToggleName == "总览");
            overView.UGUIText.text = value.ToString();
        }

        //-------------------------------------------------------------------------------

        [System.Serializable]
        public struct DistributionStruct
        {
            public string ToggleName;
            public TextMeshProUGUI UGUIText;
            public Toggle Toggle;
        }
    }
}