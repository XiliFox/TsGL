using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using EffortFramework;
using System.Linq;

namespace HDYH 
{
    public partial class BoilerDetailedLeftBottomPanel : MonoBehaviour
    {
        [SerializeField]
        private Button m_CloseButton = null;
        [SerializeField]
        private ScrollRect m_ScrollRect = null;
        [SerializeField]
        private GameObject m_BoilerDetailedLeftBottomItemPrefab = null;
        [SerializeField]
        private Transform m_ObjectPoolTransform = null;
        [SerializeField]
        private Transform m_Content = null;
        [SerializeField]
        private float m_DoMoveY = 100.0f;
        private float m_StartY;
        private BoilerSystemGameObjectPool<BoilerDetailedLeftBottomItem> m_BoilerDetailedLeftBottomItemObjectPool = null;
        private List<BoilerDetailedLeftBottomItem> m_BoilerDetailedLeftBottomItems = new List<BoilerDetailedLeftBottomItem>();
        private Tween m_Tween;
        private bool m_IsShow = false;
        public bool IsShow => m_IsShow;
        private BoilerDetailedPartNetwork m_BoilerDetailedPartNetwork;
        private BoilerDetailedPartNetwork.DetailedPartPostData m_DetailedPartPostData = new BoilerDetailedPartNetwork.DetailedPartPostData();

        private void Awake()
        {
            m_CloseButton.onClick.AddListener(Close);
            m_BoilerDetailedLeftBottomItemObjectPool = new BoilerSystemGameObjectPool<BoilerDetailedLeftBottomItem>(m_ObjectPoolTransform);
            m_StartY = transform.position.y;
            m_BoilerDetailedPartNetwork = GetComponentInChildren<BoilerDetailedPartNetwork>();
            MessageManager.Register<BoilerDetailedPartNetwork.DetailedPartData[]>(MessageConst.更新设备详情UI, UpdateUI);
        }
        private void Start()
        {
            m_BoilerDetailedLeftBottomItemPrefab.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            m_CloseButton.onClick.RemoveListener(Close);
            MessageManager.Unregister<BoilerDetailedPartNetwork.DetailedPartData[]>(MessageConst.更新设备详情UI, UpdateUI);
            m_BoilerDetailedLeftBottomItemObjectPool.ClearPool();
            m_BoilerDetailedLeftBottomItemObjectPool = null;
        }

        public void ShowLeftBottomPanel(List<DREquipment> drequipments)
        {
            m_Tween = transform.DOMoveY(m_StartY, 0.1f).SetRecyclable(true);
            //foreach (var item in m_BoilerDetailedLeftBottomItems)
            //    m_BoilerDetailedLeftBottomItemObjectPool.ReturnToItemPool(item.gameObject);
            m_BoilerDetailedLeftBottomItemObjectPool.ReturnToItemPool(m_BoilerDetailedLeftBottomItems);
            m_BoilerDetailedLeftBottomItems = new List<BoilerDetailedLeftBottomItem>();
            List<string> arr = new List<string>();
            foreach (var item in drequipments)
            {
                var data = m_BoilerDetailedLeftBottomItemObjectPool.GetItemInstance(m_BoilerDetailedLeftBottomItemPrefab);
                data.transform.SetParent(m_Content);
                data.SetActive(true);
                BoilerDetailedLeftBottomItem bdlbi = data.GetComponent<BoilerDetailedLeftBottomItem>();
                bdlbi.OnShow(item);
                m_BoilerDetailedLeftBottomItems.Add(bdlbi);
                arr.Add(item.Positioncode);
                data.transform.localScale = Vector3.one;
            }
            //--------------------------请求数据---------------------------
            m_DetailedPartPostData.positionCodes = arr.ToArray();
            m_DetailedPartPostData.unit = HomeViewForm.BoilerUnit;
            m_BoilerDetailedPartNetwork.SetData(m_DetailedPartPostData);
            m_BoilerDetailedPartNetwork.RequestData();
            //------------------------------------------------------------
            m_ScrollRect.verticalNormalizedPosition = 1;
            m_IsShow = true;
        }

        public void Close()
        {
            m_Tween = transform.DOMoveY(m_StartY - m_DoMoveY, 0.1f).SetRecyclable(true);

            //foreach (var item in m_BoilerDetailedLeftBottomItems)
            //{
            //    m_BoilerDetailedLeftBottomItemObjectPool.ReturnToItemPool(item.gameObject);
            //}
            m_BoilerDetailedLeftBottomItemObjectPool.ReturnToItemPool(m_BoilerDetailedLeftBottomItems);
            m_BoilerDetailedLeftBottomItems = new List<BoilerDetailedLeftBottomItem>();
            //this.gameObject.SetActive(false); 
            m_IsShow = false;
            m_BoilerDetailedPartNetwork.StopAutoRequest();
        }

        private void UpdateUI(BoilerDetailedPartNetwork.DetailedPartData[] datas) 
        {
            var datalist = datas.ToList();
            foreach (var item in m_BoilerDetailedLeftBottomItems)
            {
                var result = datalist.Find(x => x.positionCode == item.DREquipment.Positioncode);
                if (result != null)
                {
                    item.OnShow(result);
                }
                //else
                //{
                //    Debug.LogError("未有此项，显示默认值。");
                //    item.OnShow();//显示默认值
                //}
            }
        }
    }
}
