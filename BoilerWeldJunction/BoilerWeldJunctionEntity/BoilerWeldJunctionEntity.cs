using EffortFramework;
using UnityEngine;

namespace HDYH
{
    /// <summary>
    /// 锅炉焊点实体
    /// </summary>
    public sealed class BoilerWeldJunctionEntity : BoilerEntityBase
    {       
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            MessageManager.SendMessage(MessageConst.SetCameraOffsetPos, new Vector3(0, 0, 0));
            MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, new Vector3(0, 50, 0), new Vector3(0, 35, 0), 130f, 0.5f);
        }
    }
}