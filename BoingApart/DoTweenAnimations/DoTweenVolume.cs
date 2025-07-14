using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

namespace DoTweenAnimationUtility
{
    public enum VolumeType
    {
        Vignette,
        DepthOfField,
    }

    /// <summary>
    /// 后处理动画
    /// </summary>
    public partial class DoTweenVolume : DoTweenAnimationBase
    {
        private IEnumerable<VolumeParams> m_VolumeParams;
        private Volume m_Volume = null;
        public DoTweenVolume(Sequence sequence, Volume volume, IEnumerable<DoTweenParamsBase> volumeParams, float clipdelaytime) : base(sequence, clipdelaytime)
        {
            m_Volume = volume == null ? UnityEngine.Object.FindFirstObjectByType<Volume>() : volume;
            m_VolumeParams = volumeParams as IEnumerable<VolumeParams>;
            Init();
        }

        protected override void Init()
        {
            if (m_VolumeParams == null || m_VolumeParams.Count() <= 0)
                return;

            if (m_Volume == null)
                return;

            foreach (var item in m_VolumeParams)
                DoAnimaiton(item);
        }

        private void DoAnimaiton(VolumeParams volumeParams) 
        {
            VolumeContext volumecontext = volumeParams.volumeType switch
            {
                VolumeType.Vignette => new VignetteVolumeContext(m_sq, m_Volume, volumeParams.delayTime + m_delayTime, volumeParams.animationTime, volumeParams.EaseType, volumeParams.vignetteParams),
                VolumeType.DepthOfField => new DepthOfFieldVolumeContext(m_sq, m_Volume, volumeParams.delayTime + m_delayTime, volumeParams.animationTime, volumeParams.EaseType, volumeParams.depthOfFieldParams),
                _ => throw new NotImplementedException(),
            };
            volumecontext.Excute();
        }
        public override void Clear()
        {
            base.Clear();
        }
        public override bool CheckState() => true;
    }
}