using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using DG.Tweening;
using System.Linq;

namespace DoTweenAnimationUtility 
{
    public enum CinemachineAnimationType
    {
        CinemachineFollow,
        CinemachineHardLookAt,
        Target,
    }

    public enum CinemachineFollowType 
    {
        FollowOffset,
        RotationDamping,
        PositionDamping,
    }
    /// <summary>
    /// 基于Cinemachine的DoTween拓展
    /// </summary>
    public class DoTweenCinemachine : DoTweenAnimationBase
    {
        private IEnumerable<CinemachineParams> m_CinemachineParams;
        private CinemachineFollow m_CinemachineFollow = null;
        private CinemachineHardLookAt m_CinemachineHardLookAt = null;
        private CinemachineCamera m_CinemachineCamera = null;

        public DoTweenCinemachine(Sequence sequence, IEnumerable<DoTweenParamsBase> cinemachineParams, float clipdelaytime) : base(sequence, clipdelaytime)
        {
            m_CinemachineParams = cinemachineParams as IEnumerable<CinemachineParams>;
            Init();
        }

        public override bool CheckState()
        {
            return m_CinemachineCamera != null && m_CinemachineFollow != null && m_CinemachineHardLookAt != null;
        }

        public override void Clear()
        {
            base.Clear();
        }
        protected override void Init()
        {
            if (m_CinemachineParams == null || m_CinemachineParams.Count() <= 0)
                return;

            m_CinemachineCamera = Object.FindFirstObjectByType<CinemachineCamera>();
            m_CinemachineFollow = Object.FindFirstObjectByType<CinemachineFollow>();
            m_CinemachineHardLookAt = Object.FindFirstObjectByType<CinemachineHardLookAt>();

            foreach (var item in m_CinemachineParams)
                DoAnimation(item);
        }

        private void DoAnimation(CinemachineParams cinemachineParams)
        {
            switch (cinemachineParams.cinemachineAnimationType)
            {
                case CinemachineAnimationType.CinemachineFollow:
                    if (cinemachineParams.cinemachineFollow == null)
                    {
                        if (m_CinemachineFollow != null)
                        {
                            cinemachineParams.cinemachineFollow = m_CinemachineFollow;
                        }
                        else
                        {
                            Debug.LogError("Can not find suitable CinemachineFollow in scene");
                            break;
                        }
                    } 

                    switch (cinemachineParams.cinemachineFollowType)//CinemachineFollow Component
                    {
                        case CinemachineFollowType.FollowOffset:
                            m_sq.Insert(cinemachineParams.delayTime + m_delayTime,
                                DOTween.To(() => cinemachineParams.useStartParams ? cinemachineParams.vectorStartParams : cinemachineParams.cinemachineFollow.FollowOffset, x => cinemachineParams.cinemachineFollow.FollowOffset = x,
                                cinemachineParams.vectorEndParams, cinemachineParams.animationTime))
                                .SetEase(cinemachineParams.EaseType);
                            break;
                        case CinemachineFollowType.RotationDamping:
                            m_sq.Insert(cinemachineParams.delayTime + m_delayTime,
                                DOTween.To(() => cinemachineParams.useStartParams ? cinemachineParams.vectorStartParams : cinemachineParams.cinemachineFollow.TrackerSettings.RotationDamping, x => cinemachineParams.cinemachineFollow.TrackerSettings.RotationDamping = x,
                                cinemachineParams.vectorEndParams, cinemachineParams.animationTime))
                                .SetEase(cinemachineParams.EaseType);
                            break;
                        case CinemachineFollowType.PositionDamping:
                            m_sq.Insert(cinemachineParams.delayTime + m_delayTime,
                                DOTween.To(() => cinemachineParams.useStartParams ? cinemachineParams.vectorStartParams : cinemachineParams.cinemachineFollow.TrackerSettings.PositionDamping, x => cinemachineParams.cinemachineFollow.TrackerSettings.PositionDamping = x,
                                cinemachineParams.vectorEndParams, cinemachineParams.animationTime))
                                .SetEase(cinemachineParams.EaseType);
                            break;
                    }
                    break;
                case CinemachineAnimationType.CinemachineHardLookAt://CinemachineHardLookAt Component
                    if (cinemachineParams.cinemachineHardLookAt == null)
                        if (m_CinemachineHardLookAt != null)
                        {
                            cinemachineParams.cinemachineHardLookAt = m_CinemachineHardLookAt;
                        }
                        else
                        {
                            Debug.LogError("Can not find suitable CinemachineHardLookAt in scene");
                            break;
                        }

                    m_sq.Insert(cinemachineParams.delayTime + m_delayTime,
                        DOTween.To(() => cinemachineParams.useStartParams ? cinemachineParams.vectorStartParams : cinemachineParams.cinemachineHardLookAt.LookAtOffset, x => cinemachineParams.cinemachineHardLookAt.LookAtOffset = x,
                        cinemachineParams.vectorEndParams, cinemachineParams.animationTime))
                        .SetEase(cinemachineParams.EaseType);
                    break;

                case CinemachineAnimationType.Target:
                    if (m_CinemachineCamera.Target.TrackingTarget == null)
                        return;
                    if (cinemachineParams.transformParams == null || cinemachineParams.transformParams.Count == 0) 
                        return;

                    DOTweenTransform doTweenTransform = new DOTweenTransform(m_sq, m_CinemachineCamera.Target.TrackingTarget, cinemachineParams.transformParams, m_delayTime);
                    break;
            }
        }
    }
}