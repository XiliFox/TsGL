using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DoTweenAnimationUtility
{
    public class DoTweenShareMaterial : DoTweenAnimationBase
    {
        public IEnumerable<MaterialParams> m_MaterialParams;
        public Material m_Material;//sharematerial
        private Dictionary<string, object> m_MaterialCache = new Dictionary<string, object>();
        public DoTweenShareMaterial(Sequence sequence, Material material, IEnumerable<DoTweenParamsBase> materialParams, float clipdelaytime) : base(sequence, clipdelaytime)
        {
            m_Material = material;
            m_MaterialParams = materialParams as IEnumerable<MaterialParams>;
            Init();
        }
        protected override void Init()
        {
            if (m_MaterialParams == null || m_MaterialParams.Count() <= 0)
                return;

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
        }
        /// <summary>
        /// 改变颜色
        /// </summary>
        /// <param name="materialParams"></param>
        private void DoColor(MaterialParams materialParams)
        {
            if (!m_MaterialCache.ContainsKey(materialParams.keyWords))
                m_MaterialCache.TryAdd(materialParams.keyWords, m_Material.GetColor(materialParams.keyWords));
            if (materialParams.isHDRColor)
            {
                m_sq.Insert(materialParams.delayTime + m_delayTime, m_Material.DOColor
                            (materialParams.endHDRColor, materialParams.keyWords, materialParams.animationTime).SetEase(materialParams.EaseType));

            }
            else
            {
                m_sq.Insert(materialParams.delayTime + m_delayTime, m_Material.DOColor
                    (materialParams.endColor, materialParams.keyWords, materialParams.animationTime).SetEase(materialParams.EaseType));
            }
        }
        /// <summary>
        /// 改变数值
        /// </summary>
        /// <param name="materialParams"></param>
        private void DoFloat(MaterialParams materialParams)
        {
            if (!m_MaterialCache.ContainsKey(materialParams.keyWords))
                m_MaterialCache.TryAdd(materialParams.keyWords, m_Material.GetFloat(materialParams.keyWords));
            m_sq.Insert(materialParams.delayTime + m_delayTime, m_Material.DOFloat(materialParams.endValue, materialParams.keyWords, materialParams.animationTime).SetEase(materialParams.EaseType));
        }

        /// <summary>
        /// 改变数值
        /// </summary>
        /// <param name="materialParams"></param>
        private void DoFade(MaterialParams materialParams)
        {
            if (!m_MaterialCache.ContainsKey(materialParams.keyWords))
                m_MaterialCache.TryAdd(materialParams.keyWords, m_Material.GetFloat(materialParams.keyWords));

            m_sq.Insert(materialParams.delayTime + m_delayTime, m_Material.DOFade(materialParams.endValue, materialParams.keyWords, materialParams.animationTime).SetEase(materialParams.EaseType));
        }
        public override void Clear()
        {
            base.Clear();
            foreach (var item in m_MaterialCache)
            {
                if (item.Value is Color)
                    m_Material.SetColor(item.Key, (Color)item.Value);
                else
                    m_Material.SetFloat(item.Key, (float)item.Value);
            }
            m_MaterialCache.Clear();
        }

        public override bool CheckState() => m_Material != null;
    }
}