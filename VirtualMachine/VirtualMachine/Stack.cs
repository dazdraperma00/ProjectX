using System;
using System.Collections.Generic;

namespace VirtualMachine
{
    class Stack
    {
        private static readonly Exception sof = new Exception("stack overflow");

        private List<AVariant> m_stack = new List<AVariant>();

        public int GetStackPointer()
        {
            return m_stack.Count - 1;
        }

        public AVariant[] ToArray()
        {
            return m_stack.ToArray();
        }

        public void Push(AVariant var)
        {
            m_stack.Add(var);
        }

        public AVariant Pop()
        {
            AVariant var = m_stack[m_stack.Count - 1];
            m_stack.RemoveAt(m_stack.Count - 1);
            return var;
        }

        public void Pick(int offset)
        {
            m_stack.Add(m_stack[offset]);
        }

        public void Set(int offset, AVariant var)
        {
            m_stack[offset] = var;
        }
    }
}
