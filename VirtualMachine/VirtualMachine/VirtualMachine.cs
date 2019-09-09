using System;
using System.Collections.Generic;
using PSCompiler;

namespace VirtualMachine
{
    public enum ByteCommand : byte
    {
        FETCH,
        STORE,
        PUSH,
        POP,
        ADD,
        SUB,
        LT,
        JZ,
        JNZ,
        JMP,
        HALT,

        NONE
    }

    class VirtualMachine
    {
        private Stack<Variant> stack = new Stack<Variant>();

        public void Run(byte[] program)
        {

        }
    }
}
