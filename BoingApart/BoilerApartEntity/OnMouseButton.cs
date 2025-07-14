using EffortFramework;
using UnityEngine;
using UnityEngine.UI;

namespace HDYH
{
    public class OnMouseButton : MonoBehaviour
    {
        [SerializeField]
        private Button JumpButton;

        string hitName;

        private void Awake()
        {
            hitName = transform.name;
            JumpButton.onClick.AddListener(OnClickJumpButton);
        }

        private void OnDestroy()
        {
            JumpButton.onClick.RemoveListener(OnClickJumpButton);
        }

        private void OnClickJumpButton()
        {
            MessageManager.SendMessage(MessageConst.SetMainSceneVolumeVignetteintensity, 1.0f);
            MessageManager.SendMessage(MessageConst.ChangeModel, EntityType.BoilerDetailedPartEntity, EntityType.None, hitName, hitName);
        }
    }
}
