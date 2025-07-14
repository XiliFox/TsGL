using DoTweenAnimationUtility;
using EffortFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HDYH
{
    public class BoilerWeldJunctionForm : UGuiForm
    {
        [SerializeField]
        private Button m_back;

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
                    MessageManager.SendMessage(MessageConst.BoilerWeldJunctionEntityShowWeld, toggle.name);
            });
        }
    }
}
