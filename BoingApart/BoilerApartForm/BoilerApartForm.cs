using EffortFramework;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HDYH
{
    public class BoilerApartForm : UGuiForm
    {
        [BoxGroup("全部拆解"), SerializeField, LabelText("拆解")]
        private Button m_AllApart;
        [BoxGroup("全部拆解"), SerializeField, LabelText("选中")]
        private GameObject m_AllSelected;
        [BoxGroup("全部拆解"), SerializeField, LabelText("名称")]
        private TextMeshProUGUI m_ApartName;

        [BoxGroup("过热器"), SerializeField, LabelText("拆解")]
        private Button m_SuperheaterApart;
        [BoxGroup("过热器"), SerializeField, LabelText("选中")]
        private GameObject m_SuperheaterSelected;
        [BoxGroup("过热器"), SerializeField, LabelText("名称")]
        private TextMeshProUGUI m_SuperheaterName;

        [BoxGroup("再热器"), SerializeField, LabelText("拆解")]
        private Button m_ReheaterApart;
        [BoxGroup("再热器"), SerializeField, LabelText("选中")]
        private GameObject m_ReheaterSelected;
        [BoxGroup("再热器"), SerializeField, LabelText("名称")]
        private TextMeshProUGUI m_ReheaterName;

        [BoxGroup("水冷壁"), SerializeField, LabelText("拆解")]
        private Button m_WaterWallApart;
        [BoxGroup("水冷壁"), SerializeField, LabelText("选中")]
        private GameObject m_WaterWallSelected;
        [BoxGroup("水冷壁"), SerializeField, LabelText("名称")]
        private TextMeshProUGUI m_WaterWallName;

        [BoxGroup("省煤器"), SerializeField, LabelText("拆解")]
        private Button m_EconomizerApart;
        [BoxGroup("省煤器"), SerializeField, LabelText("选中")]
        private GameObject m_EconomizerSelected;
        [BoxGroup("省煤器"), SerializeField, LabelText("名称")]
        private TextMeshProUGUI m_EconomizerName;

        [SerializeField, LabelText("返回")] private Button m_back;

        private List<Button> m_ToggleList = new List<Button>();

        // -----------------------------------------------------------------------------------------

        private void Awake()
        {
            m_ToggleList = new List<Button> { m_AllApart, m_SuperheaterApart, m_ReheaterApart, m_WaterWallApart, m_EconomizerApart };

            m_SuperheaterApart.onClick.AddListener(OnClickSuperheater);
            m_ReheaterApart.onClick.AddListener(OnClickReheater);
            m_WaterWallApart.onClick.AddListener(OnClickWaterWall);
            m_EconomizerApart.onClick.AddListener(OnClickEconomizer);
            m_AllApart.onClick.AddListener(OnClickApart);

            m_back.onClick.AddListener(OnClickBack);
            MessageManager.Register<Dictionary<string, BoilerApartPart>, bool>(MessageConst.ChangeUIState, OnChangeUIstate);
        }

        private void OnDestroy()
        {
            m_SuperheaterApart.onClick.RemoveListener(OnClickSuperheater);
            m_ReheaterApart.onClick.RemoveListener(OnClickReheater);
            m_WaterWallApart.onClick.RemoveListener(OnClickWaterWall);
            m_EconomizerApart.onClick.RemoveListener(OnClickEconomizer);
            m_AllApart.onClick.RemoveListener(OnClickApart);

            m_back.onClick.RemoveListener(OnClickBack);
            MessageManager.Register<Dictionary<string, BoilerApartPart>, bool>(MessageConst.ChangeUIState, OnChangeUIstate);
        }

        private void OnEnable()
        {
            ChangeToggleInteractable(true);
        }

        //-----------------------------------------------------------------------------------------

        private void OnClickSuperheater() => OnClickBtn("过热器");
        private void OnClickReheater() => OnClickBtn("再热器");
        private void OnClickWaterWall() => OnClickBtn("水冷壁");
        private void OnClickEconomizer() => OnClickBtn("省煤器");
        private void OnClickApart() => OnClickOverallBtn();

        private void OnClickBack()
        {
            MessageManager.SendMessage(MessageConst.OnClickBackButton);
            MessageManager.SendMessage(MessageConst.SetHomeView, true);
            MessageManager.SendMessage(MessageConst.SetCameraOffsetPos, new Vector3(-35, 0, 0));
            MessageManager.SendMessage(MessageConst.ChangeModel, EntityType.BoilerWarningEntity, EntityType.None, "", "");
        }

        //-----------------------------------------------------------------------------------------

        private void OnClickBtn(string partName)
        {
            MessageManager.SendMessage(MessageConst.OnClickButton, partName);
            ChangeToggleInteractable(false);
        }
        private void OnClickOverallBtn()
        {
            MessageManager.SendMessage(MessageConst.OnClickOverallButton);
            ChangeToggleInteractable(false);
        }

        private void OnChangeUIstate(Dictionary<string, BoilerApartPart> partDic,bool overallState)
        {
            foreach (var part in partDic.Values)
            {
                switch (part.name)
                {
                    case "过热器": ChangeTogglePartLable(m_SuperheaterSelected, part.ApartPartState, m_SuperheaterName); break;
                    case "再热器": ChangeTogglePartLable(m_ReheaterSelected, part.ApartPartState, m_ReheaterName); break;
                    case "水冷壁": ChangeTogglePartLable(m_WaterWallSelected, part.ApartPartState, m_WaterWallName); break;
                    case "省煤器": ChangeTogglePartLable(m_EconomizerSelected, part.ApartPartState, m_EconomizerName); break;
                }
            }

            ChangeTogglePartLable(m_AllSelected, overallState, m_ApartName);
            StartCoroutine(OpenAllBtnInteraction());
        }

        private void ChangeTogglePartLable(GameObject selected, bool state,TextMeshProUGUI togName)
        {
            selected.SetActive(state);
            string togState = state ? "组装" : "拆解";
            togName.text = string.Format("{1}{0}", togState, togName.transform.parent.name);
        }

        private IEnumerator OpenAllBtnInteraction()
        {
            yield return new WaitForSeconds(1.25f);
            ChangeToggleInteractable(true);
        }

        private void ChangeToggleInteractable(bool state)
        {
            foreach (var child in m_ToggleList)
            {
                child.interactable = state;
            }
        }
    }

    public static partial class MessageConst
    {
        public const string OnClickButton = "OnClickButton"; // 点击UI切换按钮
        public const string ChangeUIState = "ChangeUIState";
        public const string OnClickOverallButton = "OnClickOverallButton";
        public const string OnClickBackButton = "OnClickBackButton";
    }
}