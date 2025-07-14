using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace DoTweenAnimationUtility 
{
    public enum MaterialAnimationType
    {
        Color,
        Float,
        Fade,
    }

    /// <summary>
    /// 负责处理材质改变的动画
    /// </summary>
    public class DOTweenMaterial : DoTweenAnimationBase
    {
        public Renderer m_renderer;
        public IEnumerable<MaterialParams> m_MaterialParams;

        public DOTweenMaterial(Sequence sequence, Renderer renderer, IEnumerable<DoTweenParamsBase> materialParams, float clipdelaytime) : base(sequence, clipdelaytime)
        {
            m_renderer = renderer;
            m_MaterialParams = materialParams as IEnumerable<MaterialParams>;
            Init();
        }
        protected override void Init()
        {
            if (m_MaterialParams == null || m_MaterialParams.Count() <= 0)
                return;

            if (m_renderer == null)
            {
                Debug.LogError($"RendererComponent is null!");
                return;
            }

            foreach (var item in m_MaterialParams)
            {
                switch (item.animationType)
                {
                    case MaterialAnimationType.Color:
                        DoColor(item);
                        break;

                    case MaterialAnimationType.Float:
                        DoFloat(item);
                        break;

                    case MaterialAnimationType.Fade:
                        DoFade(item);
                        break;
                }
            }
            //正播、重播、倒播回调
            //m_sq.OnComplete(() => m_callback?.Invoke());
                //.OnRewind(() => m_callback?.Invoke());
        }
        /// <summary>
        /// 改变颜色
        /// </summary>
        /// <param name="materialParams"></param>
        private void DoColor(MaterialParams materialParams)
        {
            if (materialParams.isHDRColor)
            {
                foreach (var item in m_renderer.materials)
                {
                    m_sq.Insert(materialParams.delayTime + m_delayTime, item.DOColor
                                (materialParams.endHDRColor, materialParams.keyWords, materialParams.animationTime).SetEase(materialParams.EaseType));

                }
            }
            else
            {
                foreach (var item in m_renderer.materials)
                {
                    m_sq.Insert(materialParams.delayTime + m_delayTime, item.DOColor
                        (materialParams.endColor, materialParams.keyWords, materialParams.animationTime).SetEase(materialParams.EaseType));
                }
            }
        }
        /// <summary>
        /// 改变数值
        /// </summary>
        /// <param name="materialParams"></param>
        private void DoFloat(MaterialParams materialParams) 
        {
            if (m_renderer == null)
            {
                Debug.LogError($"Add Animation fail,{m_renderer.name} is Null!");
                return;
            }
            foreach (var item in m_renderer.materials)
            {
                m_sq.Insert(materialParams.delayTime + m_delayTime, item.DOFloat(materialParams.endValue, materialParams.keyWords, materialParams.animationTime).SetEase(materialParams.EaseType));
            }
        }

        /// <summary>
        /// 改变数值
        /// </summary>
        /// <param name="materialParams"></param>
        private void DoFade(MaterialParams materialParams)
        {
            foreach (var item in m_renderer.materials)
            {
                m_sq.Insert(materialParams.delayTime + m_delayTime, item.DOFade(materialParams.endValue, materialParams.keyWords, materialParams.animationTime).SetEase(materialParams.EaseType));
            }
        }
        public override void Clear()
        {
            base.Clear();
        }
        public override bool CheckState() => m_renderer != null;
    }
}