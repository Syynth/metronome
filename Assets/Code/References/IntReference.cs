using System;

using UnityEngine;

namespace Assets.Code.References
{

    [Serializable]
    public class IntReference
    {
       
        [SerializeField]
        private bool literal = false;

        [SerializeField]
        private int m_Value;

        [SerializeField]
        private IntVariable m_Variable;

        public int Value
        {
            get
            {
                return literal ? m_Value : m_Variable?.Value ?? 0;
            }
            set
            {
                if (literal)
                {
                    m_Value = value;
                }
                else
                {
                    if (m_Variable != null) m_Variable.Value = value;
                }
            }
        }

    }
}
