using System;

namespace VirtualMachine
{
    class Stack
    {
        private const int c_capacity = 16;

        private Variant[] m_stack = new Variant[c_capacity]; // Do we can initialize array with nulls?

        public int m_nsp = c_capacity;
        public int m_nbp = -1;

        public void Resize()
        {
            int inc = m_stack.Length / 2;
            Variant[] vStack = new Variant[m_stack.Length + inc];

            for (int i = 0; i < m_nbp; ++i)
            {
                vStack[i] = m_stack[i];
            }

            for (int i = m_nsp; i < m_stack.Length - 1; ++i)
            {
                vStack[i + inc] = m_stack[i];
            }

            m_stack = vStack;
        }

        public void PushDown(Variant var)
        {
            if (m_nsp - 1 == m_nbp)
            {
                Resize();
            }

            m_stack[--m_nsp] = var;
        }

        public void PushUp(Variant var)
        {
            if (m_nbp + 1 == m_nsp)
            {
                Resize();
            }

            m_stack[++m_nbp] = var;
        }

        public Variant PopUp()
        {
            return m_stack[m_nsp++];
        }

        public Variant PopDown()
        {
            return m_stack[m_nbp--];
        }

        public void Pick(int offset)
        {
            PushDown(m_stack[(int)m_stack[m_nbp].m_dValue - offset]);
        }

        public void Set(int offset, Variant var)
        {
            m_stack[m_stack.Length - 1 - offset] = var;
        }

        public Variant[] ToArray()
        {
            return m_stack;
        }
    }
}
