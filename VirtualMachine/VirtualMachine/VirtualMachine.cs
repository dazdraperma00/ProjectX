using System;

namespace VirtualMachine
{
    public enum ByteCommand : byte
    {
        FETCH,
        PUSH,
        POP,
        ADD,
        SUB,
        LT,
        GT,
        LET,
        GET,
        EQ,
        NEQ,
        JZ,
        JNZ,
        JMP,
        HALT,

        NONE
    }

    class VirtualMachine
    {
        private readonly byte[] program;

        private Stack stack = new Stack();

        public VirtualMachine(byte[] _program)
        {
            program = _program;
        }

        private Variant GetVar(ref uint pos)
        {
            if (pos + Variant.size < program.Length)
            {
                Variant arg = Variant.FromBytes(program, pos);
                pos += Variant.size;
                return arg;
            }
            else
            {
                throw new Exception("variant operand corrapted");
            }
        }

        private uint GetVarId(ref uint pos)
        {
            if (pos + sizeof(uint) < program.Length)
            {
                uint id = BitConverter.ToUInt32(program, (int)pos);
                pos += sizeof(uint);
                return id;
            }
            else
            {
                throw new Exception("offset operand corrapted");
            }
        }

        private void JmpToMark(ref uint pos)
        {
            if (pos + sizeof(uint) < program.Length)
            {
                uint mark = BitConverter.ToUInt32(program, (int)pos);
                if (mark < program.Length)
                {
                    pos = mark;
                }
            }
            else
            {
                throw new Exception("nonexistent mark");
            }
        }

        public void Run()
        {
            for (uint i = 0; i < program.Length;)
            {
                switch((ByteCommand)program[i++])
                {
                    case ByteCommand.FETCH:
                        {
                            stack.Pick(GetVarId(ref i));
                            break;
                        }
                    case ByteCommand.PUSH:
                        {
                            stack.Push(GetVar(ref i));
                            break;
                        }
                    case ByteCommand.POP:
                        {
                            stack.Pop();
                            break;
                        }
                    case ByteCommand.ADD:
                        {
                            Variant op2 = stack.Pop();
                            Variant op1 = stack.Pop();
                            stack.Push(op1 + op2);
                            break;
                        }
                    case ByteCommand.SUB:
                        {
                            Variant op2 = stack.Pop();
                            Variant op1 = stack.Pop();
                            stack.Push(op1 - op2);
                            break;
                        }
                    case ByteCommand.LT:
                        {
                            Variant op2 = stack.Pop();
                            Variant op1 = stack.Pop();
                            Variant res = new Variant(op1 < op2 ? 1.0 : 0.0);
                            stack.Push(res);
                            break;
                        }
                    case ByteCommand.GT:
                        {
                            Variant op2 = stack.Pop();
                            Variant op1 = stack.Pop();
                            Variant res = new Variant(op1 > op2 ? 1.0 : 0.0);
                            stack.Push(res);
                            break;
                        }
                    case ByteCommand.LET:
                        {
                            Variant op2 = stack.Pop();
                            Variant op1 = stack.Pop();
                            Variant res = new Variant(op1 <= op2 ? 1.0 : 0.0);
                            stack.Push(res);
                            break;
                        }
                    case ByteCommand.GET:
                        {
                            Variant op2 = stack.Pop();
                            Variant op1 = stack.Pop();
                            Variant res = new Variant(op1 >= op2 ? 1.0 : 0.0);
                            stack.Push(res);
                            break;
                        }
                    case ByteCommand.EQ:
                        {
                            Variant op2 = stack.Pop();
                            Variant op1 = stack.Pop();
                            Variant res = new Variant(op1 == op2 ? 1.0 : 0.0);
                            stack.Push(res);
                            break;
                        }
                    case ByteCommand.NEQ:
                        {
                            Variant op2 = stack.Pop();
                            Variant op1 = stack.Pop();
                            Variant res = new Variant(op1 != op2 ? 1.0 : 0.0);
                            stack.Push(res);
                            break;
                        }
                    case ByteCommand.JZ:
                        {
                            if (stack.Pop() == 0.0)
                            {
                                JmpToMark(ref i);
                            }
                            else
                            {
                                i += sizeof(uint);
                            }
                            break;
                        }
                    case ByteCommand.JNZ:
                        {
                            if (stack.Pop() != 0.0)
                            {
                                JmpToMark(ref i);
                            }
                            else
                            {
                                i += sizeof(uint);
                            }
                            break;
                        }
                    case ByteCommand.JMP:
                        {
                            JmpToMark(ref i);
                            break;
                        }
                    case ByteCommand.HALT:
                        {
                            return;
                        }
                    default:
                        throw new Exception("byte error");
                }
            }
        }
    }
}
