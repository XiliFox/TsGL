using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DoTweenAnimationUtility
{
    #region 后处理参数
    [System.Serializable]
    public sealed class VignetteParams
    {
        public bool ColorParams = false;
        [ShowIf("ColorParams")]
        public ColorParameter color = new ColorParameter(Color.black, false, false, true);

        public bool CenterParams = false;
        [ShowIf("CenterParams")]
        public Vector2Parameter center = new Vector2Parameter(new Vector2(0.5f, 0.5f));

        public bool IntensityParams = false;
        [ShowIf("IntensityParams")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        public bool SmoothnessParams = false;
        [ShowIf("SmoothnessParams")]
        public ClampedFloatParameter smoothness = new ClampedFloatParameter(0.2f, 0.01f, 1f);
    }

    [System.Serializable]
    public sealed class DepthOfFieldParams
    {
        public DepthOfFieldMode Mode = DepthOfFieldMode.Off;
        //Gaussian
        [ShowIf("@this.Mode == DepthOfFieldMode.Gaussian")]
        public bool UseGaussianStart = false;
        [ShowIf("@this.Mode == DepthOfFieldMode.Gaussian && UseGaussianStart")]
        public MinFloatParameter gaussianStart = new MinFloatParameter(10f, 0f);
        [ShowIf("@this.Mode == DepthOfFieldMode.Gaussian")]
        public bool UseGaussianEnd = false;
        [ShowIf("@this.Mode == DepthOfFieldMode.Gaussian && UseGaussianEnd")]
        public MinFloatParameter gaussianEnd = new MinFloatParameter(30f, 0f);
        [ShowIf("@this.Mode == DepthOfFieldMode.Gaussian")]
        public bool UseGaussianMaxRadius = false;
        [ShowIf("@this.Mode == DepthOfFieldMode.Gaussian && UseGaussianMaxRadius")]
        public ClampedFloatParameter gaussianMaxRadius = new ClampedFloatParameter(1f, 0.5f, 1.5f);
        //Bokeh
        [ShowIf("@this.Mode == DepthOfFieldMode.Bokeh")]
        public bool UseFocusDistance = false;
        [ShowIf("@this.Mode == DepthOfFieldMode.Bokeh && UseFocusDistance")]
        public MinFloatParameter focusDistance = new MinFloatParameter(10f, 0.1f);
        [ShowIf("@this.Mode == DepthOfFieldMode.Bokeh")]
        public bool UseAperture = false;
        [ShowIf("@this.Mode == DepthOfFieldMode.Bokeh && UseAperture")]
        public ClampedFloatParameter aperture = new ClampedFloatParameter(5.6f, 1f, 32f);
        [ShowIf("@this.Mode == DepthOfFieldMode.Bokeh")]
        public bool UseFocalLength = false;
        [ShowIf("@this.Mode == DepthOfFieldMode.Bokeh && UseFocalLength")]
        public ClampedFloatParameter focalLength = new ClampedFloatParameter(50f, 1f, 300f);
        [ShowIf("@this.Mode == DepthOfFieldMode.Bokeh")]
        public bool UseBladeCount = false;
        [ShowIf("@this.Mode == DepthOfFieldMode.Bokeh && UseBladeCount")]
        public ClampedIntParameter bladeCount = new ClampedIntParameter(5, 3, 9);
        [ShowIf("@this.Mode == DepthOfFieldMode.Bokeh")]
        public bool UseBladeCurvature = false;
        [ShowIf("@this.Mode == DepthOfFieldMode.Bokeh && UseBladeCurvature")]
        public ClampedFloatParameter bladeCurvature = new ClampedFloatParameter(1f, 0f, 1f);
        [ShowIf("@this.Mode == DepthOfFieldMode.Bokeh")]
        public bool UseBladeRotation = false;
        [ShowIf("@this.Mode == DepthOfFieldMode.Bokeh && UseBladeRotation")]
        public ClampedFloatParameter bladeRotation = new ClampedFloatParameter(0f, -180f, 180f);
    }
    #endregion

    /// <summary>
    /// 后处理动画
    /// </summary>
    public partial class DoTweenVolume : DoTweenAnimationBase
    {
        public float delayTime;//动画延时时间
        public float animationTime;//动画时间
        public Ease EaseType = Ease.Linear;

        private abstract class VolumeContext
        {
            protected Sequence m_Sequence;
            protected Volume m_Volume;
            protected float m_Delaytime;
            protected float m_Animationtime;
            protected Ease m_EaseType;
            public VolumeContext(Sequence sq, Volume vm, float delaytime, float animationtime, Ease easetype)
            {
                m_Sequence = sq;
                m_Volume = vm;
                m_Delaytime = delaytime;
                m_Animationtime = animationtime;
                m_EaseType = easetype;
            }

            /// <summary>
            /// 执行
            /// </summary>
            public abstract void Excute();

            /// <summary>
            /// 设置Color类型变量动画参数
            /// </summary>
            public void SetDotweenColorParams(Sequence sq, float delaytime, float animationtime, Ease easetype, DOGetter<Color> startcolor, Color endcolor, DOSetter<Color> callback)
            {
                sq.Insert(delaytime, DOTween.To(startcolor, callback, endcolor, animationtime)).SetEase(easetype);
            }

            /// <summary>
            /// 设置Vector2类型变量动画参数
            /// </summary>
            public void SetDotweenVector2Params(Sequence sq, float delaytime, float animationtime, Ease easetype, DOGetter<Vector2> startvector2, Vector2 endvector2, DOSetter<Vector2> callback)
            {
                sq.Insert(delaytime, DOTween.To(startvector2, callback, endvector2, animationtime)).SetEase(easetype);
            }

            /// <summary>
            /// 设置float类型变量动画参数
            /// </summary>
            public void SetDotweenFloatParams(Sequence sq, float delaytime, float animationtime, Ease easetype, DOGetter<float> startfloat, float endfloat, DOSetter<float> callback)
            {
                sq.Insert(delaytime, DOTween.To(startfloat, callback, endfloat, animationtime)).SetEase(easetype);
            }

            /// <summary>
            /// 设置int类型变量动画参数
            /// </summary>
            public void SetDotweenIntParams(Sequence sq, float delaytime, float animationtime, Ease easetype, DOGetter<int> startint, int endint, DOSetter<int> callback)
            {
                sq.Insert(delaytime, DOTween.To(startint, callback, endint, animationtime)).SetEase(easetype);
            }
        }

        /// <summary>
        /// Vignette后处理动画执行类
        /// </summary>
        private class VignetteVolumeContext : VolumeContext
        {
            private VignetteParams m_VignetteParams;
            public VignetteVolumeContext(Sequence sequence, Volume vm, float delaytime, float animationtime, Ease easetype, VignetteParams vignetteparams)
                : base(sequence, vm, delaytime, animationtime, easetype) => m_VignetteParams = vignetteparams;

            public override void Excute()
            {
                if (m_Volume == null)
                {
                    Debug.LogError("Append Sequence Volume->Vignette is Failed, Because Volume Component is null!");
                    return;
                }
                bool profile = m_Volume.profile.TryGet<Vignette>(out var vignette);
                if (profile)
                {
                    if (m_VignetteParams.ColorParams)
                    {
                        vignette.color.overrideState = m_VignetteParams.color.overrideState;
                        SetDotweenColorParams(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType, () => vignette.color.value, m_VignetteParams.color.value, x => vignette.color.value = x);
                    }

                    if (m_VignetteParams.CenterParams)
                    {
                        vignette.center.overrideState = m_VignetteParams.center.overrideState;
                        SetDotweenVector2Params(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType, () => vignette.center.value, m_VignetteParams.center.value, x => vignette.center.value = x);
                    }
                    if (m_VignetteParams.IntensityParams)
                    {
                        vignette.intensity.overrideState = m_VignetteParams.intensity.overrideState;
                        SetDotweenFloatParams(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType, () => vignette.intensity.value, m_VignetteParams.intensity.value, x => vignette.intensity.value = x);
                    }

                    if (m_VignetteParams.SmoothnessParams)
                    {
                        vignette.smoothness.overrideState = m_VignetteParams.smoothness.overrideState;
                        SetDotweenFloatParams(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType, () => vignette.smoothness.value, m_VignetteParams.smoothness.value, x => vignette.smoothness.value = x);
                    }
                }
            }
        }
        /// <summary>
        /// DepthOfField后处理动画执行类
        /// </summary>
        private class DepthOfFieldVolumeContext : VolumeContext
        {
            private DepthOfFieldParams m_DepthOfFieldParams;
            public DepthOfFieldVolumeContext(Sequence sequence, Volume vm, float delaytime, float animationtime, Ease easetype, DepthOfFieldParams depthoffieldparams)
                : base(sequence, vm, delaytime, animationtime, easetype) => m_DepthOfFieldParams = depthoffieldparams;

            public override void Excute()
            {
                if (m_Volume == null)
                {
                    Debug.LogError("Append Sequence Volume->DepthOfField is Failed, Because Volume Component is null!");
                    return;
                }
                bool profile = m_Volume.profile.TryGet<DepthOfField>(out var depthoffield);
                if (profile)
                {
                    switch (m_DepthOfFieldParams.Mode)
                    {
                        case DepthOfFieldMode.Off:
                            break;
                        case DepthOfFieldMode.Gaussian:
                            if (m_DepthOfFieldParams.UseGaussianStart)
                            {
                                depthoffield.gaussianStart.overrideState = m_DepthOfFieldParams.gaussianStart.overrideState;
                                SetDotweenFloatParams(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType,
                                    () => depthoffield.gaussianStart.value, m_DepthOfFieldParams.gaussianStart.value, x => depthoffield.gaussianStart.value = x);
                            }
                            if (m_DepthOfFieldParams.UseGaussianEnd)
                            {
                                depthoffield.gaussianEnd.overrideState = m_DepthOfFieldParams.gaussianEnd.overrideState;
                                SetDotweenFloatParams(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType,
                                    () => depthoffield.gaussianEnd.value, m_DepthOfFieldParams.gaussianEnd.value, x => depthoffield.gaussianEnd.value = x);
                            }
                            if (m_DepthOfFieldParams.UseGaussianMaxRadius)
                            {
                                depthoffield.gaussianMaxRadius.overrideState = m_DepthOfFieldParams.gaussianMaxRadius.overrideState;
                                SetDotweenFloatParams(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType,
                                    () => depthoffield.gaussianMaxRadius.value, m_DepthOfFieldParams.gaussianMaxRadius.value, x => depthoffield.gaussianMaxRadius.value = x);
                            }
                            break;
                        case DepthOfFieldMode.Bokeh:
                            if (m_DepthOfFieldParams.UseFocusDistance)
                            {
                                depthoffield.focusDistance.overrideState = m_DepthOfFieldParams.focusDistance.overrideState;
                                SetDotweenFloatParams(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType,
                                    () => depthoffield.focusDistance.value, m_DepthOfFieldParams.focusDistance.value, x => depthoffield.focusDistance.value = x);
                            }
                            if (m_DepthOfFieldParams.UseFocalLength)
                            {
                                depthoffield.focalLength.overrideState = m_DepthOfFieldParams.focalLength.overrideState;
                                SetDotweenFloatParams(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType,
                                    () => depthoffield.focalLength.value, m_DepthOfFieldParams.focalLength.value, x => depthoffield.focalLength.value = x);
                            }
                            if (m_DepthOfFieldParams.UseAperture)
                            {
                                depthoffield.aperture.overrideState = m_DepthOfFieldParams.aperture.overrideState;
                                SetDotweenFloatParams(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType,
                                    () => depthoffield.aperture.value, m_DepthOfFieldParams.aperture.value, x => depthoffield.aperture.value = x);
                            }
                            if (m_DepthOfFieldParams.UseBladeCount)
                            {
                                depthoffield.bladeCount.overrideState = m_DepthOfFieldParams.bladeCount.overrideState;
                                SetDotweenIntParams(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType,
                                    () => depthoffield.bladeCount.value, m_DepthOfFieldParams.bladeCount.value, x => depthoffield.bladeCount.value = x);
                            }

                            if (m_DepthOfFieldParams.UseBladeCurvature)
                            {
                                depthoffield.bladeCurvature.overrideState = m_DepthOfFieldParams.bladeCurvature.overrideState;
                                SetDotweenFloatParams(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType,
                                    () => depthoffield.bladeCurvature.value, m_DepthOfFieldParams.bladeCurvature.value, x => depthoffield.bladeCurvature.value = x);
                            }
                            if (m_DepthOfFieldParams.UseBladeRotation)
                            {
                                depthoffield.bladeRotation.overrideState = m_DepthOfFieldParams.bladeRotation.overrideState;
                                SetDotweenFloatParams(m_Sequence, m_Delaytime, m_Animationtime, m_EaseType,
                                    () => depthoffield.bladeRotation.value, m_DepthOfFieldParams.bladeRotation.value, x => depthoffield.bladeRotation.value = x);
                            }
                            break;
                    }
                }
            }
        }
    }
}