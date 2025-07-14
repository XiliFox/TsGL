using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeViewComponent;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EffortFramework;
using UnityEngine.Events;
using TMPro;

namespace HDYH
{
    [System.Serializable]
    public class BoilerDetailedPartNodeEvent : UnityEvent<NodeBase> { };

    /// <summary>
    /// 拆解信息树形图
    /// </summary>
    public class BoilerDetailedPartNodeMonoBehavior : NodeMonoBehavior, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private Image m_backgroundbtnImg = null;

        [SerializeField]
        private TextMeshProUGUI m_text = null;

        [SerializeField]
        private TreeViewScriptableObject scriptableObject = null;

        public BoilerDetailedPartNodeEvent OnPointerExitEvent = null;
        public BoilerDetailedPartNodeEvent OnPointerEnterEvent = null;
        public BoilerDetailedPartNodeEvent OnPointerClickEvent = null;

        protected override void Awake()
        {
            base.Awake();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterEvent?.Invoke(m_nodeBase);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitEvent?.Invoke(m_nodeBase);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_nodeBase is BranchNode)
                return;

            OnPointerClickEvent?.Invoke(m_nodeBase);
        }

        public override void SetBackGroundState(TreeViewSelectState treeViewSelect)
        {
            if (m_SelectState == TreeViewSelectState.Point && treeViewSelect == TreeViewSelectState.Exit)
                return;

            if (m_SelectState == TreeViewSelectState.Point)
                WebUtility.RequestHideWebUI((int)WebUIID.拆解设备详情弹窗, "");//Web端关闭设备详情UI弹窗

            switch (treeViewSelect)
            {
                case TreeViewSelectState.Enter:
                    m_backgroundbtnImg.sprite = scriptableObject.mouseEnterSprite;
                    m_text.color = scriptableObject.mouseEnterColor;
                    break;
                case TreeViewSelectState.None:
                case TreeViewSelectState.Exit:
                    m_backgroundbtnImg.sprite = scriptableObject.mouseExitSprite;
                    m_text.color = scriptableObject.mouseExitColor;
                    break;
                case TreeViewSelectState.Point:
                    m_backgroundbtnImg.sprite = scriptableObject.mousePointSprite;
                    m_text.color = scriptableObject.mousePointColor;

                    break;
                default:
                    break;
            }


            m_SelectState = treeViewSelect;
        }
        public override void Clear()
        {
            base.Clear();
        }
        public override void Reset()
        {
            base.Reset();
            m_backgroundbtnImg.sprite = scriptableObject.mouseExitSprite;
            m_text.color = scriptableObject.mouseExitColor;
        }
    }
}