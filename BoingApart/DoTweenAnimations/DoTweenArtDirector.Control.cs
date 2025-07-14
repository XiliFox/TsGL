using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace DoTweenAnimationUtility
{
    public partial class DoTweenArtDirector : MonoBehaviour
    {
        /// <summary>
        /// 初始化动画数据
        /// </summary>
        private void Init()
        {
            DOTween.defaultAutoKill = false;//先把DOTween自动kill功能关掉才能使用回放、倒播等功能

            foreach (var item in m_AnimationCollection)
            {
                if (!m_DotweenDic.ContainsKey(item.key))
                {
                    m_DotweenDic.Add(item.key, new KeyValuePair<Sequence, List<DoTweenAnimationBase>>(DOTween.Sequence(), new List<DoTweenAnimationBase>()));
                }
                m_DotweenDic[item.key].Key.AppendCallback(() => item.startcallback?.Invoke());
                m_DotweenDic[item.key].Key.OnComplete(() => item.playendcallback?.Invoke());
                m_DotweenDic[item.key].Key.OnRewind(() => item.backplayendcallback?.Invoke());
                m_DotweenDic[item.key].Key.timeScale = item.animatioSpeed;
                foreach (var item1 in item.value)
                    DoTweenAnimationFactory.CreateDoTweenStrategy(item1, m_DotweenDic[item.key]);
            }
        }

        /// <summary>
        /// 获取指定DoTween序列当前距离开始时间(当前正播已播放的时间)
        /// </summary>
        /// <returns></returns>
        public (bool, float, Sequence) GetAnimationResidueTime(string animationName) 
        {
            KeyValuePair<Sequence, List<DoTweenAnimationBase>> keyValuePair;
            m_DotweenDic.TryGetValue(animationName, out keyValuePair);
            if (keyValuePair.Key != null)
            {
                return (true, keyValuePair.Key.fullPosition, keyValuePair.Key);
            }
            else return (false, 0.0f, null);
        }

        private void OnDestroy()
        {
            foreach (var item in m_AnimationCollection)
            {
                foreach (var value in item.value)
                {
                    value.Clear();
                }
            }
            m_DotweenDic.Clear();
        }

        /// <summary>
        /// 到达指定动画时间点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="time"></param>
        /// <param name="andplay"></param>
        public void GoToTargetTime(string key, float time, bool andplay = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                foreach (var item in m_DotweenDic)
                    item.Value.Key.Goto(time, andplay);
            }
            else
            {
                foreach (var item in m_DotweenDic)
                {
                    if (item.Key == key)
                        item.Value.Key.Goto(time, andplay);
                }
            }
        }

        public void AnimationControl(DoTweenArtType doTweenArtType, string animationKey = "")
        {
            if (string.IsNullOrEmpty(animationKey))//播放所有
            {
                foreach (var item in m_DotweenDic)
                    AnimationBehavior(item.Value.Key, doTweenArtType);
            }
            else
            {
                if (m_DotweenDic.ContainsKey(animationKey))//
                    AnimationBehavior(m_DotweenDic[animationKey].Key, doTweenArtType);
                else
                    Debug.LogError($"Animaiton:{animationKey} 不存在。");

            }
        }
        private void AnimationBehavior(Sequence sequence, DoTweenArtType doTweenArtType) 
        {
            
            if (sequence == null || !sequence.IsActive())
                return;

            switch (doTweenArtType)
            {
                case DoTweenArtType.Play:
                        sequence.PlayForward();
                    break;
                case DoTweenArtType.Stop:
                    bool state = true;
                    //先检测DoTween序列的合法性，可能会出现DoTween序列中某些带有DoTween动画的UnityObject被销毁了，但此时还会主动调用Goto方法。
                    foreach (var item in m_DotweenDic)
                    {
                        foreach (var value in item.Value.Value)
                        {
                            state = state && value.CheckState();
                            if (!value.CheckState())
                                Debug.LogWarning($"{value} CheckState Fail!");
                        } 
                    }
                    if (state)
                        sequence.Goto(0, false);
                    break;
                case DoTweenArtType.Restart:
                        sequence.Restart();
                    break;
                case DoTweenArtType.BackPlay:
                        sequence.PlayBackwards();
                    break;
                case DoTweenArtType.SmoothlyRewind:
                        sequence.SmoothRewind();
                    break;
                default:
                    break;
            }
        }
    }

    public enum DoTweenArtType 
    {
        Play,
        Stop,
        Restart,
        BackPlay,
        SmoothlyRewind,
    }
}