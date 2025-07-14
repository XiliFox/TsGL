using UnityEngine;
using UnityEngine.UI;
using EffortFramework;
using TMPro;
using System.Collections.Generic;
using DoTweenAnimationUtility;

namespace HDYH
{
    /// <summary>
    /// 锅炉材质页面
    /// </summary>
    public sealed class BoilerMaterialForm : UGuiForm
    {
        [SerializeField]
        private Button m_back;

        //[SerializeField]
        //private TMP_Dropdown m_Dropdown;
        [SerializeField]
        private Toggle m_StartIsOnToggle;

        [SerializeField]
        private List<Toggle> toggles = new List<Toggle>();
        private DoTweenArtDirector m_DoTweenArtDirector;
        private void Awake()
        {
            m_back.onClick.AddListener(OnClickBack);
            toggles.ForEach(x => OnAddListenerToggle(x));
        }
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_DoTweenArtDirector = GetComponentInChildren<DoTweenArtDirector>();
        }
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_StartIsOnToggle.isOn = true;
            if (m_DoTweenArtDirector != null) 
                m_DoTweenArtDirector.AnimationControl(DoTweenArtType.Play);
        }
        protected override void OnClose(bool isShutdown, object userData)
        {
            if (m_DoTweenArtDirector != null)
                m_DoTweenArtDirector.AnimationControl(DoTweenArtType.Stop);
            base.OnClose(isShutdown, userData);
        }
        private void OnDestroy()
        {
            m_back.onClick.RemoveListener(OnClickBack);
            toggles.ForEach(x => x.onValueChanged.RemoveAllListeners());
        }

        private void OnClickBack()
        {
            MessageManager.SendMessage(MessageConst.SetHomeView, true);
            MessageManager.SendMessage(MessageConst.ChangeModel, EntityType.BoilerWarningEntity, EntityType.None, "", "");
        }

        private void OnAddListenerToggle(Toggle toggle) 
        {
            toggle.onValueChanged.AddListener((state) => 
            {
                if (state)
                    MessageManager.SendMessage(MessageConst.BoilerMaterialEntityShowMaterial, toggle.name);
            });
        }
    }
}