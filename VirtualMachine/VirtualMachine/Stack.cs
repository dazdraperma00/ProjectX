using System;
using System.Collections.Generic;

namespace VirtualMachine
{
    class Stack
    {
        private static readonly Exception sof = new Exception("stack overflow");

        private List<Variant> stack = new List<Variant>();

        public void Push(Variant var)
        {
            stack.Add(var);
        }

        public Variant Pop()
        {
            Variant var = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            return var;
        }

        public void Pick(uint offset)
        {
            if (offset < stack.Count)
            {
                stack.Add(stack[(int)offset]);
            }
            else
            {
                throw sof;
            }
        }

        public void Set(uint offset, Variant var)
        {
            if (offset < stack.Count)
            {
                stack[(int)offset] = var;
            }
            else
            {
                throw sof;
            }
        }

        public int GetStackPointer()
        {
            return stack.Count - 1;
        }
    }
}
