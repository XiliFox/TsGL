using System.Collections.Generic;
using UnityEngine;

namespace HDYH
{
    public class BoilerApartPart : MonoBehaviour
    {
        public Dictionary<string, bool> m_partStateDic = new Dictionary<string, bool>();
        private List<string> m_dicKeysList;

        // ���������״̬
        public bool ApartPartState { get; set; } = false;

        private void Awake()
        {
            Collider[] partStates = GetComponentsInChildren<Collider>();
            foreach(var state in partStates)
            {
                m_partStateDic.Add(state.transform.name, false);
            }

            m_dicKeysList = new List<string>(m_partStateDic.Keys);
        }

        // �ı��������������״̬
        public void ChangeSubState3(bool state)
        {
            for(int i = 0; i < m_dicKeysList.Count; i++)
            {
                m_partStateDic[m_dicKeysList[i]] = state;
            }
            ApartPartState = GetSubState3().GetValueOrDefault(ApartPartState);
        }

        // �ı��������������״̬
        public void ChangeSubState3(string partName)
        {
            m_partStateDic[partName] = !m_partStateDic[partName];
            ApartPartState = GetSubState3().GetValueOrDefault(ApartPartState);
        }

        // �������������״̬ȷ�϶��������״̬�Ƿ�ı�
        public bool? GetSubState3()
        {
            bool allTrue = true;
            bool allFalse = true;

            foreach (var data in m_partStateDic.Values)
            {
                if (data)
                    allFalse = false;
                else
                    allTrue = false;

                if (!allTrue && !allFalse)
                    return null;
            }

            return allTrue;
        }

        // �ж��ֵ����Ƿ����true
        public bool Dic3ValueState()
        {
            foreach (var state in m_partStateDic.Values)
            {
                if (state)
                    return true;
            }
            return false;
        }
    }
}