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
        GT,
        EQ,
        JZ,
        JNZ,
        JMP,
        HALT,

        NONE
    }

    class VirtualMachine
    {
        private readonly byte[] program;
        private readonly uint size;
        private Stack<Variant> stack = new Stack<Variant>();
        private Dictionary<int, Variant> vars = new Dictionary<int, Variant>();

        public VirtualMachine(byte[] _program, uint _size)
        {
            program = _program;
            size = _size;
        }

        public Variant[] GetStackContent()
        {
            int size = stack.Count;
            Variant[] cont = new Variant[size];

            for (int i = 0; i < size; ++i)
            {
                cont[i] = stack.Pop();
            }

            return cont;
        }

        public Dictionary<int, Variant> GetStackVars()
        {
            return vars;
        }

        private Variant GetVar(ref uint pos)
        {
            if (pos + Variant.size < size)
            {
                Variant arg = Variant.FromBytes(program, pos);
                pos += Variant.size;
                return arg;
            }
            else
            {
                throw new Exception("stack overflow");
            }
        }

        private int GetVarId(ref uint pos)
        {
            if (pos + sizeof(int) < size)
            {
                int id = BitConverter.ToInt32(program, (int)pos);
                pos += sizeof(int);
                return id;
            }
            else
            {
                throw new Exception("stack overflow");
            }
        }

        private void JmpToMark(ref uint pos)
        {
            if (pos + sizeof(uint) < size)
            {
                uint mark = BitConverter.ToUInt32(program, (int)pos);
                if (mark < size)
                {
                    pos = mark;
                }
            }
            else
            {
                throw new Exception("stack overflow");
            }
        }

        public void Run()
        {
            for (uint i = 0; i < size;)
            {
                switch((ByteCommand)program[i++])
                {
                    case ByteCommand.FETCH:
                        {
                            stack.Push(vars[GetVarId(ref i)]);
                            break;
                        }
                    case ByteCommand.STORE:
                        {
                            Variant var = stack.Pop();
                            vars[GetVarId(ref i)] = var;
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
                    case ByteCommand.EQ:
                        {
                            Variant op2 = stack.Pop();
                            Variant op1 = stack.Pop();
                            Variant res = new Variant(op1 == op2 ? 1.0 : 0.0);
                            stack.Push(res);
                            break;
                        }
                    case ByteCommand.JZ:
                        {
                            if (stack.Pop() == (Variant)(0.0))
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
                            if (stack.Pop() != (Variant)(0.0))
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
