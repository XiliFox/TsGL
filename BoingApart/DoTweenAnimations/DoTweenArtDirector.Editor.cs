using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEditor;
using System;
using UnityEngine.Rendering;

namespace DoTweenAnimationUtility 
{
    /// <summary>
    /// 编辑器工具
    /// </summary>
    public partial class DoTweenArtDirector : MonoBehaviour
    {
        public List<KeyValueStartCallBack<string, List<AnimationClip>>> m_AnimationCollection
            = new List<KeyValueStartCallBack<string, List<AnimationClip>>>();

        public Dictionary<string, KeyValuePair<Sequence, List<DoTweenAnimationBase>>> m_DotweenDic = new Dictionary<string, KeyValuePair<Sequence, List<DoTweenAnimationBase>>>();
        [System.Serializable]
        public class AnimationClip
        {

            public string anmiationClipName;

            public float animationClipDelayTime = 0.0f;

            public AnimationType animationType = AnimationType.Material;//动画类型

            /// <summary>
            /// Material类动画参数
            /// </summary>
            [ShowIf("@this.animationType == AnimationType.Material")]
            public List<KeyValue<Renderer, List<MaterialParams>, DoTweenAnimationBase>> materialParams
                = new List<KeyValue<Renderer, List<MaterialParams>, DoTweenAnimationBase>>();
            /// <summary>
            /// Transform类动画参数
            /// </summary>
            [ShowIf("@this.animationType == AnimationType.Transform")]
            public List<KeyValue<Transform, List<TransformParams>, DoTweenAnimationBase>> transformParams
                = new List<KeyValue<Transform, List<TransformParams>, DoTweenAnimationBase>>();
            /// <summary>
            /// UI类动画参数
            /// </summary>
            [ShowIf("@this.animationType == AnimationType.UI")]
            public List<KeyValue<RectTransform, List<UIParams>, DoTweenAnimationBase>> uiParams
                = new List<KeyValue<RectTransform, List<UIParams>, DoTweenAnimationBase>>();
            /// <summary>
            /// Callback类动画参数
            /// </summary>
            [ShowIf("@this.animationType == AnimationType.CallBack")]
            public List<KeyValue<List<CallBackParams>, DoTweenAnimationBase>> callBackParams
                = new List<KeyValue<List<CallBackParams>, DoTweenAnimationBase>>();
            /// <summary>
            /// Cinemachine类动画参数
            /// </summary>
            [ShowIf("@this.animationType == AnimationType.Cinemachine")]
            public List<KeyValue<List<CinemachineParams>, DoTweenAnimationBase>> cinemachineParams
                = new List<KeyValue<List<CinemachineParams>, DoTweenAnimationBase>>();
            /// <summary>
            /// Volume类动画参数
            /// </summary>
            [ShowIf("@this.animationType == AnimationType.Volume")]
            [Tooltip("Volume组件为空时,工具会自动匹配为场景中第一个Volume组件")]
            public List<KeyValue<Volume, List<VolumeParams>, DoTweenAnimationBase>> volumeParams
                = new List<KeyValue<Volume, List<VolumeParams>, DoTweenAnimationBase>>();

            /// <summary>
            /// ShareMaterial类动画参数
            /// </summary>
            [ShowIf("@this.animationType == AnimationType.ShareMaterial")]
            public List<KeyValue<Material, List<MaterialParams>, DoTweenAnimationBase>> shareMaterialParams
                = new List<KeyValue<Material, List<MaterialParams>, DoTweenAnimationBase>>();

            public void Clear() 
            {
                materialParams.ForEach((x)=>x.value2?.Clear());
                transformParams.ForEach((x) => x.value2?.Clear());
                uiParams.ForEach((x) => x.value2?.Clear());
                callBackParams.ForEach((x) => x.value?.Clear());
                cinemachineParams.ForEach((x) => x.value?.Clear());
                volumeParams.ForEach((x) => x.value2?.Clear());
                shareMaterialParams.ForEach((x) => x.value2?.Clear());
            }
        }

#if UNITY_EDITOR

        [ShowIf("@this.editor_IsUseEditorLine == false")]
        [Button(ButtonSizes.Large)]
        [ButtonGroup("Control")]
        private void EditorPlay() 
        {
            if (!Application.isPlaying)
                return;
            if (editor_IsUseEditorLine)
                return;
            AnimationControl(DoTweenArtType.Play, editor_AnimationKey);
        }
        [ShowIf("@this.editor_IsUseEditorLine == false")]
        [Button(ButtonSizes.Large)]
        [ButtonGroup("Control")]
        private void EditorStop()
        {
            if (!Application.isPlaying)
                return;
            if (editor_IsUseEditorLine)
                return;

            AnimationControl(DoTweenArtType.Stop, editor_AnimationKey);
        }
        [ShowIf("@this.editor_IsUseEditorLine == false")]
        [Button(ButtonSizes.Large)]
        [ButtonGroup("Control")]
        private void EditorRestart()
        {
            if (!Application.isPlaying)
                return;
            if (editor_IsUseEditorLine)
                return;

            AnimationControl(DoTweenArtType.Restart, editor_AnimationKey);
        }
        [ShowIf("@this.editor_IsUseEditorLine == false")]
        [Button(ButtonSizes.Large)]
        [ButtonGroup("Control")]
        private void EditorBackPlay()
        {
            if (!Application.isPlaying)
                return;
            if (editor_IsUseEditorLine)
                return;

            AnimationControl(DoTweenArtType.BackPlay, editor_AnimationKey);
        }
        [ShowIf("@this.editor_IsUseEditorLine == false")]
        [Button(ButtonSizes.Large)]
        [ButtonGroup("Control")]
        private void EditorSmoothlyRewind()
        {
            if (!Application.isPlaying)
                return;
            if (editor_IsUseEditorLine)
                return;

            AnimationControl(DoTweenArtType.SmoothlyRewind, editor_AnimationKey);
        }

        [ShowIf("@this.editor_IsUseEditorLine == false")]
        [Button(ButtonSizes.Large)]
        [ButtonGroup("GoToTargetTime")]
        private void EditorGoToTargetTime()
        {
            if (!Application.isPlaying)
                return;
            if (editor_IsUseEditorLine)
                return;
            GoToTargetTime(editor_AnimationKey, editor_GoToTargetTime, editor_GoToTargetbool);
        }

        [SerializeField]
        private string editor_AnimationKey;

        [SerializeField]
        private bool editor_IsUseEditorLine = false;

        [ShowIf("@this.editor_IsUseEditorLine == false")]
        [HorizontalGroup("GoToTargetParams")]
        [SerializeField]
        private float editor_GoToTargetTime = 1.0f;

        [ShowIf("@this.editor_IsUseEditorLine == false")]
        [HorizontalGroup("GoToTargetParams")]
        [SerializeField]
        private bool editor_GoToTargetbool = false;

        [ShowIf("@this.editor_IsUseEditorLine")]
        [SerializeField]
        private float editor_StartTime = 0.0f;

        [ShowIf("@this.editor_IsUseEditorLine")]
        [SerializeField]
        private float editor_EndTime = 1.0f;

        //[ShowIf("@this.editor_IsUseEditorLine")]
        //[CustomValueDrawer("CustomValueDrawerChanged")]
        //[SerializeField]
        //private float editor_AnimationLine = 0;

        private float CustomValueDrawerChanged(float value, GUIContent label)
        {
            float result = EditorGUILayout.Slider(label, value, this.editor_StartTime, this.editor_EndTime);
            if (editor_IsUseEditorLine && Application.isPlaying)
                GoToTargetTime(editor_AnimationKey, result, editor_GoToTargetbool);
            return result;
        }
#endif
    }
}
