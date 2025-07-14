using DG.Tweening;
using EffortFramework;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HDYH
{
    public class HomeViewForm : UGuiForm
    {
        [FoldoutGroup("数据链接"), SerializeField, LabelText("链接文件")]
        private HomeRequestData m_homeRequestData;
        [FoldoutGroup("数据链接"), SerializeField, LabelText("请求间隔")]
        private float interval = 10f;

        private float timer = 0f;
        public static string BoilerUnit = "1";//机组号

        private void Awake()
        {
            InitSetting();// 初始化
            OnDataRegister();// 数据监听
            JumpToFront_AddListener();// 链接前端
        }

        private void OnDestroy()
        {
            JumpToFront_RemoveListener();
            OnDataUnregister();
        }

        // ===请求数据===-----------------------------------------------------------------------------

        private void OnEnable()
        {
            m_homeRequestData.RequestHomeViewData();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                m_homeRequestData.RequestHomeViewData();
                timer = 0f;
            }
        }

        // ===初始化===-----------------------------------------------------------------------------

        private void InitSetting()
        {
            this.gameObject.SetActive(true);
            transform.localScale = new Vector3(0f, 1f, 1f);
            transform.DOScale(Vector3.one, 1f);
        }

        // ===链接前端部分===------------------------------------------------------------------------

        [FoldoutGroup("链接前端Btn"), SerializeField, LabelText("运行指导Btn")]
        private Button m_runGuidanceBtn;
        [FoldoutGroup("链接前端Btn"), SerializeField, LabelText("泄漏报警Btn")]
        private Button m_leakageAlarmBtn;
        [FoldoutGroup("链接前端Btn"), SerializeField, LabelText("超温报警Btn")]
        private Button m_overTempAlarmBtn;
        [FoldoutGroup("链接前端Btn"), SerializeField, LabelText("锅炉预警Btn")]
        private Button m_warningAlarmBtn;
        [FoldoutGroup("链接前端Btn"), SerializeField, LabelText("煤质参数Btn")]
        private Button m_coalDataBtn;
        [FoldoutGroup("链接前端Btn"), SerializeField, LabelText("水质参数Btn")]
        private Button m_waterDataBtn;
        [FoldoutGroup("链接前端Btn"), SerializeField, LabelText("运行曲线Btn")]
        private Button m_startCurveBtn;

        private void JumpToFront_AddListener()
        {
            m_runGuidanceBtn.onClick.AddListener(OnClickRunGuidanceBtn);
            m_leakageAlarmBtn.onClick.AddListener(OnClickLeakageAlarmBtn);
            m_overTempAlarmBtn.onClick.AddListener(OnClickOverTempAlarmBtn);
            m_warningAlarmBtn.onClick.AddListener(OnClickWarningAlarmBtn);
            m_coalDataBtn.onClick.AddListener(OnClickCoalDataBtn);
            m_waterDataBtn.onClick.AddListener(OnClickWaterDataBtn);
            m_startCurveBtn.onClick.AddListener(OnClickStartCurveBtn);
        }

        private void JumpToFront_RemoveListener()
        {
            m_runGuidanceBtn.onClick.RemoveListener(OnClickRunGuidanceBtn);
            m_leakageAlarmBtn.onClick.RemoveListener(OnClickLeakageAlarmBtn);
            m_overTempAlarmBtn.onClick.RemoveListener(OnClickOverTempAlarmBtn);
            m_warningAlarmBtn.onClick.RemoveListener(OnClickWarningAlarmBtn);
            m_coalDataBtn.onClick.RemoveListener(OnClickCoalDataBtn);
            m_waterDataBtn.onClick.RemoveListener(OnClickWaterDataBtn);
            m_startCurveBtn.onClick.RemoveListener(OnClickStartCurveBtn);
        }

        private void OnClickRunGuidanceBtn()
        { WebUtility.RequestOpenUrl((int)UrlID.运行指导页面, ""); }
        private void OnClickLeakageAlarmBtn()
        { WebUtility.RequestOpenUrl((int)UrlID.泄漏报警, ""); }
        private void OnClickOverTempAlarmBtn()
        { WebUtility.RequestOpenUrl((int)UrlID.超温报警, ""); }
        private void OnClickWarningAlarmBtn()
        { WebUtility.RequestOpenUrl((int)UrlID.锅炉预警, ""); }
        private void OnClickCoalDataBtn()
        { WebUtility.RequestOpenUrl((int)UrlID.煤质参数, ""); }
        private void OnClickWaterDataBtn()
        { WebUtility.RequestOpenUrl((int)UrlID.水质参数, ""); }
        private void OnClickStartCurveBtn()
        { WebUtility.RequestOpenWebUI((int)WebUIID.运行曲线, BoilerUnit); }

        // ===数据监听===---------------------------------------------------------------------------

        [FoldoutGroup("数据链接/锅炉报警"), SerializeField, LabelText("泄漏报警")]
        private TextMeshProUGUI m_leakageAlarmText;
        [FoldoutGroup("数据链接/锅炉报警"), SerializeField, LabelText("超温报警")]
        private TextMeshProUGUI m_overTempAlarmText;
        [FoldoutGroup("数据链接/锅炉报警"), SerializeField, LabelText("锅炉报警")]
        private TextMeshProUGUI m_BoilerAlarmText;

        [FoldoutGroup("数据链接/锅炉预警"), SerializeField, LabelText("危险预警")]
        private TextMeshProUGUI m_dangerousAlarmText;
        [FoldoutGroup("数据链接/锅炉预警"), SerializeField, LabelText("严重预警")]
        private TextMeshProUGUI m_seriousAlarmText;
        [FoldoutGroup("数据链接/锅炉预警"), SerializeField, LabelText("一般预警")]
        private TextMeshProUGUI m_normalAlarmText;
        [FoldoutGroup("数据链接/锅炉预警"), SerializeField, LabelText("锅炉报警")]
        private TextMeshProUGUI m_AlarmText;

        [FoldoutGroup("数据链接/锅炉预警"), SerializeField, LabelText("无预警扇形图")]
        private GameObject m_zeroImage;
        [FoldoutGroup("数据链接/锅炉预警"), SerializeField, LabelText("严重预警扇形图")]
        private Image m_seriousImage;
        [FoldoutGroup("数据链接/锅炉预警"), SerializeField, LabelText("黄蓝分界线")]
        private Image m_seriousImageDivid;
        [FoldoutGroup("数据链接/锅炉预警"), SerializeField, LabelText("危险预警扇形图")]
        private Image m_dangerousImage;
        [FoldoutGroup("数据链接/锅炉预警"), SerializeField, LabelText("红黄分界线")]
        private Image m_dangerousImageDivid;

        [FoldoutGroup("数据链接/煤质参数"), SerializeField, LabelText("水分Mad")]
        private TextMeshProUGUI m_waterMadText;
        [FoldoutGroup("数据链接/煤质参数"), SerializeField, LabelText("全水分Mt")]
        private TextMeshProUGUI m_allWaterMtText;
        [FoldoutGroup("数据链接/煤质参数"), SerializeField, LabelText("灰分Aad")]
        private TextMeshProUGUI m_ashAadText;
        [FoldoutGroup("数据链接/煤质参数"), SerializeField, LabelText("挥发分Vad")]
        private TextMeshProUGUI m_volatileVadText;
        [FoldoutGroup("数据链接/煤质参数"), SerializeField, LabelText("硫Sad")]
        private TextMeshProUGUI m_sulfurSadText;
        [FoldoutGroup("数据链接/煤质参数"), SerializeField, LabelText("飞灰可燃物")]
        private TextMeshProUGUI m_flyAshText;

        [FoldoutGroup("数据链接/水质参数"), SerializeField, LabelText("PH值")]
        private TextMeshProUGUI m_PHText;
        [FoldoutGroup("数据链接/水质参数"), SerializeField, LabelText("浊度")]
        private TextMeshProUGUI m_turbidityText;
        [FoldoutGroup("数据链接/水质参数"), SerializeField, LabelText("氯离子")]
        private TextMeshProUGUI m_chlorineText;
        [FoldoutGroup("数据链接/水质参数"), SerializeField, LabelText("硬度")]
        private TextMeshProUGUI m_hardnessText;
        [FoldoutGroup("数据链接/水质参数"), SerializeField, LabelText("溶解氧")]
        private TextMeshProUGUI m_oxygenText;
        [FoldoutGroup("数据链接/水质参数"), SerializeField, LabelText("亚硫酸根")]
        private TextMeshProUGUI m_SO3Text;

        private Tween m_Tween1;

        private void OnDataRegister()
        {
            m_homeRequestData.Initialize();
            MessageManager.Register<bool>(MessageConst.SetHomeView, OnSetHomeView); // 开关本页面            
            MessageManager.Register<HomeViewData>(MessageConst.RequestHomeDataSuccess, OnRequestHomeDataSuccess);// 数据更新反馈
        }

        private void OnDataUnregister()
        {
            m_homeRequestData.Shutdown();
            MessageManager.Unregister<bool>(MessageConst.SetHomeView, OnSetHomeView);
            MessageManager.Unregister<HomeViewData>(MessageConst.RequestHomeDataSuccess, OnRequestHomeDataSuccess);
        }

        private void OnSetHomeView(bool state)
        {
            if (state)
            {
                this.gameObject.SetActive(true);
                m_Tween1 = transform.DOScale(Vector3.one, 0.4f).SetRecyclable(true);
            }
            else
            {
                transform.localScale = new Vector3(0f, 1f, 1f);
                this.gameObject.SetActive(false);
            }
        }

        private void OnRequestHomeDataSuccess(HomeViewData data)
        {
            // 锅炉报警
            m_leakageAlarmText.text = data.BoilerAlarmNum.leak.ToString();          // 泄漏报警数量
            m_overTempAlarmText.text = data.BoilerAlarmNum.overTemp.ToString();     // 超温报警数量
            m_BoilerAlarmText.text = (data.BoilerAlarmNum.leak + data.BoilerAlarmNum.overTemp).ToString();//总分

            // 锅炉预警
            m_dangerousAlarmText.text = data.BoilerWarningNum.dangerous.ToString();    // 危险报警数量
            m_seriousAlarmText.text = data.BoilerWarningNum.serious.ToString();        // 严重报警数量
            m_normalAlarmText.text = data.BoilerWarningNum.generally.ToString();       // 一般报警数量

            // 预警扇形图
            int warningNum = data.BoilerWarningNum.dangerous + data.BoilerWarningNum.serious + data.BoilerWarningNum.generally;//总报警数
            m_AlarmText.text = warningNum.ToString();
            if (warningNum == 0)
            {
                m_zeroImage.SetActive(true);
            }
            else
            {
                m_zeroImage.SetActive(false);
                m_dangerousImage.fillAmount = data.BoilerWarningNum.dangerous / (warningNum * 1.0f);                                                    //红区
                m_dangerousImageDivid.fillAmount = data.BoilerWarningNum.dangerous / (warningNum * 1.0f) + 0.005f;                                      //红黄分界线
                m_seriousImage.fillAmount = (data.BoilerWarningNum.dangerous + data.BoilerWarningNum.serious) / (warningNum * 1.0f);                    //红区
                m_seriousImageDivid.fillAmount = (data.BoilerWarningNum.dangerous + data.BoilerWarningNum.serious) / (warningNum * 1.0f) + 0.005f;      //黄蓝分界线
            }

            // 煤质参数数据
            m_waterMadText.text = StringToStandard(data.CoalQuality.mad);        //水分Mad
            m_allWaterMtText.text = StringToStandard(data.CoalQuality.mt);       //全水分Mt
            m_ashAadText.text = StringToStandard(data.CoalQuality.aad);          //灰分Aad
            m_volatileVadText.text = StringToStandard(data.CoalQuality.vad);     //挥发分Vad
            m_sulfurSadText.text = StringToStandard(data.CoalQuality.sad);       //硫Sad
            m_flyAshText.text = StringToStandard(data.CoalQuality.combustibles); //飞灰可燃物

            // 水质参数数据
            m_PHText.text = StringToStandard(data.WaterQuality.ph);                 //PH值
            m_turbidityText.text = StringToStandard(data.WaterQuality.turbidity);   //浊度
            m_chlorineText.text = StringToStandard(data.WaterQuality.chloride);     //氯离子
            m_hardnessText.text = StringToStandard(data.WaterQuality.hardness);     //硬度
            m_oxygenText.text = StringToStandard(data.WaterQuality.oxygen);         //溶解氧
            m_SO3Text.text = StringToStandard(data.WaterQuality.sulfiteRadical);    //亚硫酸根
        }

        public static string StringToStandard(string value, string accuracy = "F2")
        {
            float result;
            if (float.TryParse(value, out result))
            {
                return result.ToString(accuracy);
            }
            return value;
        }
    }
}