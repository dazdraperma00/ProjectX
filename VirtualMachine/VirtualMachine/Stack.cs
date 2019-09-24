using System;
using System.Collections.Generic;

namespace VirtualMachine
{
    class Stack
    {
        private static readonly Exception sof = new Exception("stack overflow");

        private List<Variant> m_stack = new List<Variant>();

        public int GetStackPointer()
        {
            return m_stack.Count - 1;
        }

        public Variant[] ToArray()
        {
            return m_stack.ToArray();
        }

        public void Push(Variant var)
        {
            m_stack.Add(var);
        }

        public Variant Pop()
        {
            Variant var = m_stack[m_stack.Count - 1];
            m_stack.RemoveAt(m_stack.Count - 1);
            return var;
        }

        public void Pick(int offset)
        {
            m_stack.Add(m_stack[offset]);
        }

        public void Set(int offset, Variant var)
        {
            m_stack[offset] = var;
        }
    }
}
