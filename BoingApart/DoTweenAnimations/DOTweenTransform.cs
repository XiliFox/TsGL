using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoTweenAnimationUtility 
{
    public enum TransformAnimationType
    {
        Move,
        LocalRotate,
        Scale,
        LocalMove,//其实是本地坐标的平移量，既移动的offset
    }

    public enum LocalMovePivotType
    {
        X,
        Y,
        Z,
    }
    /// <summary>
    /// 用来处理位移、旋转、缩放类型的动画 
    /// </summary>
    public class DOTweenTransform : DoTweenAnimationBase
    {
        private Transform m_Transform = null;//被移动的对象
        private IEnumerable<TransformParams> m_transformParams = null;
        public DOTweenTransform(Sequence sequence, Transform transform, IEnumerable<DoTweenParamsBase> transformParams, float clipdelaytime) : base(sequence, clipdelaytime)
        {
            m_Transform = transform;
            m_transformParams = transformParams as IEnumerable<TransformParams>;
            Init();
        }

        public override bool CheckState() => m_Transform != null;

        public override void Clear()
        {
            base.Clear();
        }
        protected override void Init()
        {
            if (m_transformParams == null || m_transformParams.Count() <= 0)
                return;

            if (m_Transform == null)
            {
                Debug.LogError($"TransformComponent is null!");
                return;
            }
            foreach (var item in m_transformParams)
                DoAnimation(item);
        }

        private void DoAnimation(TransformParams transformParams) 
        {
            switch (transformParams.transformAnimationType)
            {
                case TransformAnimationType.Move:
                    m_sq.Insert(transformParams.delayTime + m_delayTime, m_Transform.DOMove(transformParams.endPositionValue, transformParams.animationTime)
                        .SetEase(transformParams.EaseType));
                    break;

                case TransformAnimationType.LocalMove:
                    switch (transformParams.localMovePivotType)
                    {
                        case LocalMovePivotType.X:
                            m_sq.Insert(transformParams.delayTime + m_delayTime, m_Transform.DOLocalMoveX(transformParams.endMovePivotValue + m_Transform.localPosition.x, transformParams.animationTime)
                                .SetEase(transformParams.EaseType));
                            break;
                        case LocalMovePivotType.Y:
                            m_sq.Insert(transformParams.delayTime + m_delayTime, m_Transform.DOLocalMoveY(transformParams.endMovePivotValue + m_Transform.localPosition.y, transformParams.animationTime)
                                .SetEase(transformParams.EaseType));
                            break;
                        case LocalMovePivotType.Z:
                            m_sq.Insert(transformParams.delayTime + m_delayTime, m_Transform.DOLocalMoveZ(transformParams.endMovePivotValue + m_Transform.localPosition.z, transformParams.animationTime)
                                .SetEase(transformParams.EaseType));
                            break;
                        default:
                            break;
                    }
                    break;

                case TransformAnimationType.LocalRotate:
                    m_sq.Insert(transformParams.delayTime + m_delayTime, m_Transform.DOLocalRotate(transformParams.endLocalRotateValue, transformParams.animationTime, transformParams.rotateMode)
                        .SetEase(transformParams.EaseType));
                    break;
                case TransformAnimationType.Scale:
                    m_sq.Insert(transformParams.delayTime + m_delayTime, m_Transform.DOScale(transformParams.endScaleValue, transformParams.animationTime)
                        .SetEase(transformParams.EaseType));
                    break;
            }
        }
    }
}