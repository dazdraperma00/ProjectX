using System;
using System.Collections.Generic;
using VirtualMachine;

namespace PSCompiler
{
    class Compiler
    {
        private List<byte> byteCode = new List<byte>();
        
        private void Gen(byte command)
        {
            byteCode.Add(command);
        }

        private void Compile(Node node)
        {
            switch(node.GetNodeType())
            {

            }
        }
    }
}
