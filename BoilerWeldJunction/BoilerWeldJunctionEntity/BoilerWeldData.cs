using EffortFramework;
using System.Collections.Generic;
using UnityEngine;

namespace HDYH
{
    public class BoilerWeldData : MonoBehaviour
    {
        [System.Serializable]
        public class WeldData
        {
            public string WeldName;
            public GameObject WeldPoint;
            public MeshRenderer[] WeldLine;
        }

        [SerializeField]
        private BoilerMaterialEntityScriptableObject m_boilerWeldEntityScriptableObject;

        [SerializeField]
        private Transform m_WeldJunction;

        [SerializeField]
        private Transform m_WeldLine;

        [SerializeField]
        private List<WeldData> WeldDataList = new List<WeldData>();


        private void Awake()
        {
            MessageManager.Register<string>(MessageConst.BoilerWeldJunctionEntityShowWeld, OnBoilerWeldJunctionEntityShowWeld);
        }

        private void OnDestroy()
        {
            MessageManager.Unregister<string>(MessageConst.BoilerWeldJunctionEntityShowWeld, OnBoilerWeldJunctionEntityShowWeld);
        }

        private void OnBoilerWeldJunctionEntityShowWeld(string toggleName)
        {
            if (null == toggleName) return;
            (Color baseColor, Color HDRColor) = m_boilerWeldEntityScriptableObject.GetColor("四管亮");
            (Color baseColor1, Color HDRColor1) = m_boilerWeldEntityScriptableObject.GetColor("四管暗");

            if (toggleName == "锅炉焊点")
            {

                foreach (var data in WeldDataList)
                {
                    data.WeldPoint.SetActive(true);
                    foreach(var info in data.WeldLine)
                    {
                        info.material.color = baseColor;
                    }
                }

                MessageManager.SendMessage(MessageConst.SetCameraOffsetPos, new Vector3(0, 0, 0));
                MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, new Vector3(0, 50, 0), new Vector3(0, 35, 0), 130f, 0.5f);
            }
            else
            {
                foreach (var data in WeldDataList)
                {
                    if (data.WeldName == toggleName)
                    {
                        data.WeldPoint.SetActive(true);
                        foreach (var info in data.WeldLine)
                        {
                            info.material.color = baseColor;
                        }
                        (float rotationY, float distance) = RandomUtility.GetRandomValue(new RandomRange(-45.0f, 45.0f), new RandomRange(60.0f, 120.0f));
                        MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, data.WeldPoint.transform.position, new Vector3(0.0f, rotationY, 0.0f), distance, 0.5f);
                    }
                    else
                    {
                        data.WeldPoint.SetActive(false);
                        foreach (var info in data.WeldLine)
                        {
                            info.material.color = baseColor1;
                        }
                    }
                }
            }
        }
    }
}