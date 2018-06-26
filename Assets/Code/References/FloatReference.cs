using System;

using UnityEngine;

namespace Assets.Code.References
{

    [Serializable]
    public class FloatReference
    {
       
        [SerializeField]
        private bool literal = false;

        [SerializeField]
        private float m_Value;

        [SerializeField]
        private FloatVariable m_Variable;

        public float Value
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
