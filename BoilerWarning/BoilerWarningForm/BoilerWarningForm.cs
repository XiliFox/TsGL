using DG.Tweening;
using EffortFramework;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HDYH
{
    // 锅炉预警页面
    public class BoilerWarningForm : UGuiForm
    {
        private Tween m_Tween1;
        private Tween m_Tween2;

        private void Awake()
        {
            JumpToFront_AddListener(); //页面跳转
        }

        private void OnDestroy()
        {
            JumpToFront_RemoveListener();
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_Tween1 = m_InfoView.transform.DOScale(Vector3.one, 0.4f).SetRecyclable(true);
            m_Tween2 = m_BtnGroup.transform.DOScale(Vector3.one, 0.4f).SetRecyclable(true);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            m_InfoView.transform.localScale = new Vector3(1f, 0f, 1f);
            m_BtnGroup.transform.localScale = new Vector3(0f, 1f, 1f);
        }

        // ===页面跳转===---------------------------------------------------------------------------

        [BoxGroup("按钮组"), SerializeField, LabelText("流场")]
        private Button m_velocityFieldBtn;
        [BoxGroup("按钮组"), SerializeField, LabelText("焊缝")]
        private Button m_weldFieldBtn;
        [BoxGroup("按钮组"), SerializeField, LabelText("拆解")]
        private Button m_apartBtn;
        [BoxGroup("按钮组"), SerializeField, LabelText("材质")]
        private Button m_materialBtn;
        [BoxGroup("按钮组"), SerializeField, LabelText("按钮组")]
        private GameObject m_BtnGroup = null;

        private void JumpToFront_AddListener()
        {
            m_velocityFieldBtn.onClick.AddListener(OnClickVelocityFieldBtn);
            m_weldFieldBtn.onClick.AddListener(OnClickWeldFieldBtn);
            m_apartBtn.onClick.AddListener(OnClickApartBtn);
            m_materialBtn.onClick.AddListener(OnClickMaterialBtn);
        }

        private void JumpToFront_RemoveListener()
        {
            m_velocityFieldBtn.onClick.RemoveListener(OnClickVelocityFieldBtn);
            m_weldFieldBtn.onClick.RemoveListener(OnClickWeldFieldBtn);
            m_apartBtn.onClick.RemoveListener(OnClickApartBtn);
            m_materialBtn.onClick.RemoveListener(OnClickMaterialBtn);
        }

        private void OnClickVelocityFieldBtn()
        {
            WebUtility.RequestOpenUrl((int)UrlID.流场, "");
        }

        private void OnClickWeldFieldBtn()
        {
            MessageManager.SendMessage(MessageConst.SetHomeView, false);
            MessageManager.SendMessage(MessageConst.ChangeModel, EntityType.BoilerWeldJunctionEntity, EntityType.None, "", "");
        }

        private void OnClickApartBtn()
        {
            MessageManager.SendMessage(MessageConst.SetHomeView, false);
            MessageManager.SendMessage(MessageConst.ChangeModel, EntityType.BoilerApartEntity, EntityType.None, "", "");
        }

        private void OnClickMaterialBtn()
        {
            MessageManager.SendMessage(MessageConst.SetHomeView, false);
            MessageManager.SendMessage(MessageConst.ChangeModel, EntityType.BoilerMaterialEntity, EntityType.None, "", "");
        }

        // ===展示测点工具面板===------------------------------------------------------------------------

        [BoxGroup("测点"), SerializeField, LabelText("测量点工具面板")]
        private MeasurePointToolsPanel m_MeasurePointToolsPanel = null;

        // 在 【BoilerWarningForm - 测点数据】 的单个数据下调用方法
        public void OnShowMeasurePointToolsPanel(BaseEventData data)
        {
            var result = data as PointerEventData;
            if (result != null)
            {
                var pointName = result.pointerEnter.name;
                var infodata = m_ParameterInfolist.Find(x => x.TextComponent.name == pointName);

                if (infodata != null)
                {
                    string tagvalue = HomeViewForm.BoilerUnit == "1" ? infodata.TagName1 : infodata.TagName2;
                    string value = $"#{HomeViewForm.BoilerUnit}{pointName}&{tagvalue}";
                    m_MeasurePointToolsPanel.OnOpen(value, Input.mousePosition);
                }
            }
        }

        // ===更新测点信息===------------------------------------------------------------------------

        [System.Serializable]
        public class ParameterInfo
        {
            [LabelText("text")] public TextMeshProUGUI TextComponent;
            [LabelText("#1机组测点")] public string TagName1;//测点名称
            [LabelText("#2机组测点")] public string TagName2;//测点名称
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        [BoxGroup("测点"), SerializeField, LabelText("数据链接文件")]
        private WebRequestObservationPointAgent m_WebRequestObservationPointAgent = null;

        [BoxGroup("测点"), SerializeField, LabelText("数据刷新时间")]
        private float m_refreshTime = 5.0f;

        [BoxGroup("测点"), SerializeField, LabelText("测点数据")]
        private GameObject m_InfoView = null;

        [BoxGroup("测点"), SerializeField, LabelText("参数信息列表")]
        private List<ParameterInfo> m_ParameterInfolist = new List<ParameterInfo>();

        private float timer = 0.0f;

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            timer += Time.deltaTime;
            if (timer >= m_refreshTime)
            {
                UpDateTagInfo();
                timer = 0f;
            }
        }

        // 更新测点信息
        private void UpDateTagInfo()
        {
            if (m_WebRequestObservationPointAgent == null) return;

            for (int i = 0; i < m_ParameterInfolist.Count; ++i)
            {
                string tagvalue = HomeViewForm.BoilerUnit == "1" ? m_ParameterInfolist[i].TagName1 : m_ParameterInfolist[i].TagName2;
                var (isValid, floatValue, stringValue) = m_WebRequestObservationPointAgent.GetValue(tagvalue);
                m_ParameterInfolist[i].TextComponent.text = floatValue.ToString("F2");
            }
        }
    }
}