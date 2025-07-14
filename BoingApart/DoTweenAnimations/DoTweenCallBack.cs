using DG.Tweening;
using DoTweenAnimationUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoTweenAnimationUtility
{
    public class DoTweenCallBack : DoTweenAnimationBase
    {

        private IEnumerable<CallBackParams> m_callBackParams;
        public DoTweenCallBack(Sequence sequence, IEnumerable<DoTweenParamsBase> callBackParams, float clipdelaytime) : base(sequence, clipdelaytime)
        {
            m_callBackParams = callBackParams as IEnumerable<CallBackParams>;
            Init();
        }

        protected override void Init()
        {
            foreach (var item in m_callBackParams)
                DoAnimation(item);
        }
        private void DoAnimation(CallBackParams callBackParams)
        {
            m_sq.InsertCallback(callBackParams.delayTime + m_delayTime, () => { callBackParams.callback.Invoke(m_sq); });
        }
        public override void Clear()
        {
            base.Clear();
        }

        public override bool CheckState() => true;
    }
}