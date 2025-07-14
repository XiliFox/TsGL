using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DoTweenAnimationUtility
{
    /// <summary>
    /// 用来声名DoTweenAnimation需要的数据类
    /// </summary>
    public enum AnimationType
    {
        Material,//材质动画
        Transform,//Transform动画
        UI,//UI动画
        CallBack,//动画回调
        Cinemachine,//Cinemachine动画
        Volume,//Volume动画
        ShareMaterial,//ShareMaterial动画
    }

    [System.Serializable]
    public class DoTweenParamsBase 
    {
        
    }

    #region 材质动画Data
    /// <summary>
    /// 材质动画参数
    /// </summary>
    [System.Serializable]
    public class MaterialParams: DoTweenParamsBase
    {
        public MaterialAnimationType animationType = MaterialAnimationType.Color;
        //-------------------------Color---------------------------------
        [ShowIf("@this.animationType == MaterialAnimationType.Color")]
        public bool isHDRColor = false;

        [ShowIf("@this.isHDRColor == false && this.animationType == MaterialAnimationType.Color")]
        public Color endColor = Color.white;

        [ShowIf("@this.isHDRColor && this.animationType == MaterialAnimationType.Color")]
        [ColorUsageAttribute(true, true)]
        public Color endHDRColor = Color.white;
        //---------------------------------------------------------------

        //-------------------------Float---------------------------------
        [ShowIf("@this.animationType == MaterialAnimationType.Float|| this.animationType == MaterialAnimationType.Fade")]
        public float endValue;//目标值

        //---------------------------------------------------------------
        public string keyWords;//shader关键字
        public float delayTime;//动画延时时间
        public float animationTime;//动画时间
        public Ease EaseType = Ease.Linear;
    }
    #endregion

    #region Transform动画Data
    [System.Serializable]
    public class TransformParams: DoTweenParamsBase
    {
        public TransformAnimationType transformAnimationType = TransformAnimationType.Move;
        //---------------------------------Move-----------------------------------
        [ShowIf("@this.transformAnimationType == TransformAnimationType.Move")]
        public Vector3 endPositionValue = Vector3.zero;

        [ShowIf("@this.transformAnimationType == TransformAnimationType.LocalMove")]
        public LocalMovePivotType localMovePivotType = LocalMovePivotType.X;

        [ShowIf("@this.transformAnimationType == TransformAnimationType.LocalMove")]
        public float endMovePivotValue = 0;
        //------------------------------------------------------------------------

        //--------------------------------Rotate----------------------------------
        [ShowIf("@this.transformAnimationType == TransformAnimationType.LocalRotate")]
        public Vector3 endLocalRotateValue = Vector3.zero;

        [ShowIf("@this.transformAnimationType == TransformAnimationType.LocalRotate")]
        public RotateMode rotateMode = RotateMode.FastBeyond360;
        //------------------------------------------------------------------------

        //--------------------------------Scale-----------------------------------
        [ShowIf("@this.transformAnimationType == TransformAnimationType.Scale")]
        public Vector3 endScaleValue = Vector3.zero;
        //------------------------------------------------------------------------
        public float delayTime;//动画延时时间
        public float animationTime;//动画时间
        public Ease EaseType = Ease.Linear;

    }
    #endregion

    #region UI动画Data
    /// <summary>
    /// 材质动画参数
    /// </summary>
    [System.Serializable]
    public class UIParams: DoTweenParamsBase
    {
        public UIAnimationType animationType = UIAnimationType.Color;
        //-------------------------Color---------------------------------

        [ShowIf("@this.animationType == UIAnimationType.Color")]
        public Color endColor = Color.white;

        [ShowIf("@this.animationType == UIAnimationType.LocalMove")]
        public Vector2 endV2Value = Vector2.zero;

        //---------------------------------------------------------------
        [ShowIf("@this.animationType == UIAnimationType.LocalRotate|| this.animationType == UIAnimationType.Scale")]
        public Vector3 endV3Value = Vector3.zero;

        //-------------------------Float---------------------------------
        [ShowIf("@this.animationType == UIAnimationType.MaterialFloat||this.animationType == UIAnimationType.ImageFillAmount||this.animationType == UIAnimationType.CanvasFade")]
        public float endFValue;//目标值

        //---------------------------------------------------------------
        [ShowIf("@this.animationType == UIAnimationType.MaterialFloat")]
        public string keyWords;//shader关键字
        public float delayTime;//动画延时时间
        public float animationTime;//动画时间
        public Ease EaseType = Ease.Linear;
    }
    #endregion

    #region CallBack动画Data
    /// <summary>
    /// 材质动画参数
    /// </summary>
    [System.Serializable]
    public class CallBackParams : DoTweenParamsBase
    {
        public string CallBackName;

        public UnityEvent<Sequence> callback;

        public float delayTime;//回调延时时间
    }
    #endregion

    #region Cinemachine动画Data

    [System.Serializable]
    public class CinemachineParams : DoTweenParamsBase
    {
        public CinemachineAnimationType cinemachineAnimationType = CinemachineAnimationType.CinemachineFollow;
        //CinemachineFollow
        [ShowIf("@this.cinemachineAnimationType == CinemachineAnimationType.CinemachineFollow")]
        [Tooltip("CinemachineFollow若为空,则会自动赋值为场景中的一个CinemachineFollow")]
        public CinemachineFollow cinemachineFollow = null;
        [ShowIf("@this.cinemachineAnimationType == CinemachineAnimationType.CinemachineFollow")]
        public CinemachineFollowType cinemachineFollowType = CinemachineFollowType.FollowOffset;
        //CinemachineHardLookAt
        [ShowIf("@this.cinemachineAnimationType == CinemachineAnimationType.CinemachineHardLookAt")]
        [Tooltip("CinemachineHardLookAt若为空,则会自动赋值为场景中的一个CinemachineHardLookAt")]
        public CinemachineHardLookAt cinemachineHardLookAt = null;
        [ShowIf("@this.cinemachineAnimationType == CinemachineAnimationType.CinemachineHardLookAt || this.cinemachineAnimationType == CinemachineAnimationType.CinemachineFollow")]
        public bool useStartParams;
        [ShowIf("@this.cinemachineAnimationType == CinemachineAnimationType.CinemachineHardLookAt || this.cinemachineAnimationType == CinemachineAnimationType.CinemachineFollow && useStartParams")]
        public Vector3 vectorStartParams = Vector3.zero;
        [ShowIf("@this.cinemachineAnimationType == CinemachineAnimationType.CinemachineHardLookAt || this.cinemachineAnimationType == CinemachineAnimationType.CinemachineFollow")]
        public Vector3 vectorEndParams = Vector3.zero;
        [ShowIf("@this.cinemachineAnimationType == CinemachineAnimationType.CinemachineHardLookAt || this.cinemachineAnimationType == CinemachineAnimationType.CinemachineFollow")]
        public float delayTime;//动画延时时间
        [ShowIf("@this.cinemachineAnimationType == CinemachineAnimationType.CinemachineHardLookAt || this.cinemachineAnimationType == CinemachineAnimationType.CinemachineFollow")]
        public float animationTime;//动画时间
        [ShowIf("@this.cinemachineAnimationType == CinemachineAnimationType.CinemachineHardLookAt || this.cinemachineAnimationType == CinemachineAnimationType.CinemachineFollow")]
        public Ease EaseType = Ease.Linear;

        [ShowIf("@this.cinemachineAnimationType == CinemachineAnimationType.Target")]
        public List<TransformParams> transformParams = null;
    }
    #endregion

    #region Volume动画Data
    [System.Serializable]
    public class VolumeParams : DoTweenParamsBase 
    {
        public VolumeType volumeType = VolumeType.Vignette;//后处理类型

        [ShowIf("@this.volumeType == VolumeType.Vignette")]
        public VignetteParams vignetteParams;

        [ShowIf("@this.volumeType == VolumeType.DepthOfField")]
        public DepthOfFieldParams depthOfFieldParams;

        public float delayTime;//动画延时时间
        public float animationTime;//动画时间
        public Ease EaseType = Ease.Linear;
    }

    #endregion

    [System.Serializable]
    public class KeyValue<T1, T2>
    {
        public T1 key;
        public T2 value;
    }
    [System.Serializable]
    public class KeyValueStartCallBack<T1, T2>
    {
        public T1 key;
        public T2 value;
        public float animatioSpeed = 1.0f;
        public bool UseSequenceCallBack = false;
        [ShowIf("@this.UseSequenceCallBack")]
        public UnityEvent startcallback;//开始播放回调
        [ShowIf("@this.UseSequenceCallBack")]
        public UnityEvent playendcallback;//正播结束回调
        [ShowIf("@this.UseSequenceCallBack")]
        public UnityEvent backplayendcallback;//倒播结束回调

    }
    [System.Serializable]
    public class KeyValue<T1, T2, T3>
    {
        public T1 key;
        public T2 value1;
        public T3 value2;
        //public UnityEvent callback;
    }
}
