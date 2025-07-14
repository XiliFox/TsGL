using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DoTweenAnimationUtility 
{
    public abstract class DoTweenAnimationBase
    {
        protected Sequence m_sq = null;
        protected float m_delayTime = 0.0f;
        public DoTweenAnimationBase(Sequence sequence,float animationclipdelaytime)
        {
            m_sq = sequence;
            m_delayTime = animationclipdelaytime;
        }
        /// <summary>
        /// 初始化Sequence序列
        /// </summary>
        protected abstract void Init();

        public virtual void Clear() 
        {
            if (m_sq != null) 
            {
                DOTween.Kill(m_sq);
                m_sq = null;
            }
        }

        /// <summary>
        /// 核查有DoTween动画对象的合法状态
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckState();
    }
}
