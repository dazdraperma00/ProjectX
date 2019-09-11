using System;
using System.Collections.Generic;
using VirtualMachine;

namespace PSCompiler
{
    class Compiler
    {
        private List<byte> byteCode = new List<byte>();
        private uint pc = 0;
        
        public List<byte> GetByteCode()
        {
            return byteCode;
        }

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

        private void Gen(uint mark, uint pos = 0)
        {
            byte[] code = BitConverter.GetBytes(mark);

            if (pos == 0)
            {
                for (ushort i = 0; i < sizeof(uint); ++i)
                {
                    byteCode.Add(code[i]);
                }

                pc += sizeof(uint);
            }
            else
            {
                for (ushort i = 0; i < sizeof(uint); ++i)
                {
                    byteCode[(int)pos + i] = code[i];
                }
            }
        }

        private void Compile(Node node)
        {
            switch(node.GetNodeType())
            {
                case NodeType.VAR:
                    {
                        Gen(ByteCommand.FETCH);
                        Gen(((VarNode)node).GetName());
                        break;
                    }
                case NodeType.CONST:
                    {
                        Gen(ByteCommand.PUSH);
                        Gen(((VarNode)node).GetValue());
                        break;
                    }
                case NodeType.DECL:
                    {
                        Gen(ByteCommand.PUSH);
                        Gen(((VarNode)node).GetValue());
                        Gen(ByteCommand.STORE);
                        Gen(((VarNode)node).GetName());
                        break;
                    }
                case NodeType.ADD:
                    {
                        Compile(node.GetOperand(0));
                        Compile(node.GetOperand(1));
                        Gen(ByteCommand.ADD);
                        break;
                    }
                case NodeType.SUB:
                    {
                        Compile(node.GetOperand(0));
                        Compile(node.GetOperand(1));
                        Gen(ByteCommand.SUB);
                        break;
                    }
                case NodeType.DO:
                    {
                        uint mark = pc;
                        Compile(node.GetOperand(0));
                        Compile(node.GetOperand(1));
                        Gen(ByteCommand.JNZ);
                        Gen(mark);
                        break;
                    }
                case NodeType.WHILE:
                    {
                        uint mark1 = pc;
                        Compile(node.GetOperand(0));
                        Gen(ByteCommand.JZ);
                        uint mark2 = 0;
                        uint pos = pc;
                        Gen(mark2);
                        Compile(node.GetOperand(1));
                        Gen(ByteCommand.JMP);
                        Gen(mark1);
                        mark2 = pc;
                        Gen(mark2, pos);
                        break;
                    }
                case NodeType.IF:
                    {
                        Compile(node.GetOperand(0));
                        Gen(ByteCommand.JZ);
                        uint pos = pc;
                        uint mark = 0;
                        Gen(mark);
                        Compile(node.GetOperand(1));
                        mark = pc;
                        Gen(mark, pos);
                        break;
                    }
                case NodeType.IFELSE:
                    {
                        Compile(node.GetOperand(0));
                        Gen(ByteCommand.JZ);
                        uint pos = pc;
                        uint mark1 = 0;
                        Gen(mark1);
                        Compile(node.GetOperand(1));
                        Gen(ByteCommand.JMP);
                        mark1 = pc;
                        Gen(mark1, pos);
                        uint mark2 = 0;
                        pos = pc;
                        Gen(mark2);
                        Compile(node.GetOperand(2));
                        mark2 = pc;
                        Gen(mark2, pos);
                        break;
                    }
                case NodeType.EQ:
                    {
                        Compile(node.GetOperand(0));
                        Compile(node.GetOperand(1));
                        Gen(ByteCommand.EQ);
                        break;
                    }
                case NodeType.LT:
                    {
                        Compile(node.GetOperand(0));
                        Compile(node.GetOperand(1));
                        Gen(ByteCommand.LT);
                        break;
                    }
                case NodeType.GT:
                    {
                        Compile(node.GetOperand(0));
                        Compile(node.GetOperand(1));
                        Gen(ByteCommand.GT);
                        break;
                    }
                case NodeType.SET:
                    {
                        Compile(node.GetOperand(0));
                        Gen(ByteCommand.STORE);
                        Gen(((VarNode)node).GetName());
                        break;
                    }
                case NodeType.SEQ:
                    {
                        Compile(node.GetOperand(0));
                        Compile(node.GetOperand(1));
                        break;
                    }
                case NodeType.PROGRAM:
                    {
                        Compile(node.GetOperand(0));
                        Gen(ByteCommand.HALT);
                        break;
                    }
                default:
                    throw new Exception("compile error");
            }
        }
    }
}
