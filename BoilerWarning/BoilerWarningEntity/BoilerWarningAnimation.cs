using Sirenix.OdinInspector;
using UnityEngine;

namespace HDYH
{
    public class BoilerWarningAnimation : MonoBehaviour
    {
        [SerializeField, LabelText("空预器1")]
        private Transform m_AirPreheater1;

        [SerializeField, LabelText("空预器2")]
        private Transform m_AirPreheater2;

        [SerializeField, LabelText("旋转速度")]
        private float BgRotateSpeed = 90f;

        private void Update()
        {
            m_AirPreheater1.Rotate(new Vector3(0, 0, BgRotateSpeed * Time.deltaTime));
            m_AirPreheater2.Rotate(new Vector3(0, 0, BgRotateSpeed * Time.deltaTime));
        }
    }
}
