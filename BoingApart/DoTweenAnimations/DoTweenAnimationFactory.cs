using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DoTweenAnimationUtility
{
    public class DoTweenAnimationFactory
    {
        public static void CreateDoTweenStrategy(DoTweenArtDirector.AnimationClip animationClip, KeyValuePair<Sequence, List<DoTweenAnimationBase>> keyValuePair) 
        {
            switch (animationClip.animationType)
            {
                case AnimationType.Material:
                    foreach (var item2 in animationClip.materialParams)
                    {
                        if (item2.value2 == null)
                        {
                            item2.value2 = new DOTweenMaterial(keyValuePair.Key, item2.key, item2.value1, animationClip.animationClipDelayTime);
                            keyValuePair.Value.Add(item2.value2);
                        } 
                    }
                    break;
                case AnimationType.Transform:
                    foreach (var item2 in animationClip.transformParams)
                    {
                        if (item2.value2 == null)
                        {
                            item2.value2 = new DOTweenTransform(keyValuePair.Key, item2.key, item2.value1, animationClip.animationClipDelayTime);
                            keyValuePair.Value.Add(item2.value2);
                        }
                    }
                    break;
                case AnimationType.UI:
                    foreach (var item2 in animationClip.uiParams)
                    {
                        if (item2.value2 == null)
                        {
                            item2.value2 = new DoTweenUI(keyValuePair.Key, item2.key, item2.value1, animationClip.animationClipDelayTime);
                            keyValuePair.Value.Add(item2.value2);
                        }
                    }
                    break;
                case AnimationType.CallBack:
                    foreach (var item2 in animationClip.callBackParams)
                    {
                        if (item2.value == null)
                        {
                            item2.value = new DoTweenCallBack(keyValuePair.Key, item2.key, animationClip.animationClipDelayTime);
                            keyValuePair.Value.Add(item2.value);
                        }
                    }
                    break;
                case AnimationType.Cinemachine:
                    foreach (var item2 in animationClip.cinemachineParams)
                    {
                        if (item2.value == null)
                        {
                            item2.value = new DoTweenCinemachine(keyValuePair.Key, item2.key, animationClip.animationClipDelayTime);
                            keyValuePair.Value.Add(item2.value);
                        }
                    }
                    break;
                case AnimationType.Volume:
                    foreach (var item2 in animationClip.volumeParams)
                    {
                        if (item2.value2 == null)
                        {
                            item2.value2 = new DoTweenVolume(keyValuePair.Key, item2.key, item2.value1, animationClip.animationClipDelayTime);
                            keyValuePair.Value.Add(item2.value2);
                        }
                    }
                    break;
                case AnimationType.ShareMaterial:
                    foreach (var item2 in animationClip.shareMaterialParams)
                    {
                        if (item2.value2 == null)
                        {
                            item2.value2 = new DoTweenShareMaterial(keyValuePair.Key, item2.key, item2.value1, animationClip.animationClipDelayTime);
                            keyValuePair.Value.Add(item2.value2);
                        }
                    }
                    break;
            }
        }
    }
}