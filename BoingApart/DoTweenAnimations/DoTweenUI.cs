using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace DoTweenAnimationUtility
{
    public enum UIAnimationType
    {
        LocalMove,
        LocalRotate,
        Scale,
        Color,
        MaterialFloat,
        ImageFillAmount,
        CanvasFade,
    }
    /// <summary>
    /// 用来处理位移、旋转、缩放类型的动画 
    /// </summary>
    public class DoTweenUI : DoTweenAnimationBase
    {
        private RectTransform m_rectTransform = null;//被控制的UI
        private IEnumerable<UIParams> m_uiParams;
        private Image m_image = null;
        private CanvasGroup m_CanvasGroup;
        public DoTweenUI(Sequence sequence, RectTransform rectTransform, IEnumerable<DoTweenParamsBase> uiParams, float clipdelaytime) : base(sequence, clipdelaytime)
        {
            m_rectTransform = rectTransform;
            m_uiParams = uiParams as IEnumerable<UIParams>;
            Init();
        }

        public override bool CheckState() => m_rectTransform != null;

        public override void Clear()
        {
            base.Clear();
        }
        protected override void Init()
        {
            if (m_uiParams == null || m_uiParams.Count() <= 0)
                return;

            if (m_rectTransform == null)
            {
                Debug.LogError($"Image is null!");
                return;
            }
            m_image = m_rectTransform.GetComponent<Image>();
            if (m_image != null) 
                m_image.material = new Material(m_image.material);

            m_CanvasGroup = m_rectTransform.GetComponent<CanvasGroup>();
            foreach (var item in m_uiParams)
                DoAnimation(item);
        }

        private void DoAnimation(UIParams uiParams)
        {
            switch (uiParams.animationType)
            {
                case UIAnimationType.LocalMove:
                    m_sq.Insert(uiParams.delayTime + m_delayTime, m_rectTransform.DOAnchorPos(uiParams.endV2Value, uiParams.animationTime).SetEase(uiParams.EaseType));
                    break;
                case UIAnimationType.LocalRotate:
                    m_sq.Insert(uiParams.delayTime + m_delayTime, m_rectTransform.DOLocalRotate(uiParams.endV3Value, uiParams.animationTime).SetEase(uiParams.EaseType));
                    break;
                case UIAnimationType.Scale:
                    m_sq.Insert(uiParams.delayTime + m_delayTime, m_rectTransform.DOScale(uiParams.endV3Value, uiParams.animationTime).SetEase(uiParams.EaseType));
                    break;
                case UIAnimationType.Color:
                    if (m_image == null)
                        return;
                    m_sq.Insert(uiParams.delayTime + m_delayTime, m_image.DOColor(uiParams.endColor, uiParams.animationTime).SetEase(uiParams.EaseType));
                    break;
                case UIAnimationType.MaterialFloat:
                    if (m_image == null)
                        return;
                    m_sq.Insert(uiParams.delayTime + m_delayTime, m_image.material.DOFloat(uiParams.endFValue, uiParams.keyWords, uiParams.animationTime).SetEase(uiParams.EaseType));
                    break;

                case UIAnimationType.ImageFillAmount:
                    if (m_image == null)
                        return;
                    m_sq.Insert(uiParams.delayTime + m_delayTime, m_image.DOFillAmount(uiParams.endFValue, uiParams.animationTime).SetEase(uiParams.EaseType));
                    break;
                case UIAnimationType.CanvasFade:
                    if (m_CanvasGroup == null)
                        return;             
                    m_sq.Insert(uiParams.delayTime + m_delayTime, DOTween.To(() => m_CanvasGroup.alpha, x => m_CanvasGroup.alpha = x, uiParams.endFValue, uiParams.animationTime).SetEase(uiParams.EaseType));
                    break;
            }
        }
    }
}