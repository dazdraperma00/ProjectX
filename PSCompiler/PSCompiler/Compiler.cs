using System;
using System.Collections.Generic;
using VirtualMachine;

namespace PSCompiler
{
    class Compiler
    {
        private List<byte> byteCode = new List<byte>();
        private uint pc = 0;
        
        private void Gen(ByteCommand command)
        {
            byteCode.Add((byte)command);
            pc += sizeof(byte);
        }

        private void Gen(string varName)
        {
            byte[] code = BitConverter.GetBytes(varName.GetHashCode());

            for (ushort i = 0; i < sizeof(int); ++i)
            {
                byteCode.Add(code[i]);
            }

            pc += sizeof(int);
        }

        private void Gen(Variant var)
        {
            byte[] code = var.ToBytes();

            for (ushort i = 0; i < Variant.size; ++i)
            {
                byteCode.Add(code[i]);
            }

            pc += Variant.size;
        }

        private void Compile(Node node)
        {
            switch(node.GetNodeType())
            {
                case NodeType.VAR:
                    Gen(ByteCommand.FETCH);
                    Gen(((VarNode)node).GetName());
                    break;
                case NodeType.CONST:
                    Gen(ByteCommand.PUSH);
                    Gen(((VarNode)node).GetValue());
                    break;
                case NodeType.DECL:
                    Gen(ByteCommand.PUSH);
                    Gen(((VarNode)node).GetValue());
                    Gen(ByteCommand.STORE);
                    Gen(((VarNode)node).GetName());
                    break;
                case NodeType.ADD:
                    Compile(node.GetOperand(0));
                    Compile(node.GetOperand(1));
                    Gen(ByteCommand.ADD);
                    break;
                case NodeType.SUB:
                    Compile(node.GetOperand(0));
                    Compile(node.GetOperand(1));
                    Gen(ByteCommand.SUB);
                    break;
                case NodeType.DO:
                    uint mark = pc;
                    Compile(node.GetOperand(0));
                    Compile(node.GetOperand(1));
                    Gen(ByteCommand.JNZ);
                    Gen(mark);
                    break;
                case NodeType.WHILE:
                    uint mark1 = pc;
                    Compile(node.GetOperand(0));
                    Gen(ByteCommand.JZ);
                    Gen(mark1);
                    uint mark2 = pc;
                    Compile(node.GetOperand(1));
                    Gen(ByteCommand.JMP);
                    Gen(mark1);
                    break;
            }
        }
    }
}
