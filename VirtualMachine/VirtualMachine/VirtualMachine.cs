﻿using System;
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
        //private Dictionary<uint, Variant> vars = new Dictionary<uint, Variant>();

        public VirtualMachine(byte[] _program, uint _size)
        {
            program = _program;
            size = _size;
        }

        private Variant GetVar(ref uint pos)
        {
            if (pos + Variant.size < size)
            {
                Variant arg = new Variant(BitConverter.ToDouble(program, (int)pos));
                pos += Variant.size;
                return arg;
            }
            else
            {
                throw new Exception("stack overflow");
            }
        }

        private void JmpToMark(ref uint pos)
        {
            uint mark = BitConverter.ToUInt32(program, (int)pos);
            if (mark < size)
            {
                pos = mark;
            }
        }

        public void Run()
        {
            for (uint i = 0; i < size; ++i)
            {
                switch((ByteCommand)program[i])
                {
                    case ByteCommand.FETCH:
                        {
                            Variant var = GetVar(ref i);
                            stack.Push(var);
                            //vars.Add(var);
                            break;
                        }
                    case ByteCommand.STORE:
                        { 
                            //vars.Add(stack.Pop());
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
                            Variant op1 = stack.Pop();
                            Variant op2 = stack.Pop();
                            stack.Push(op1 + op2);
                            break;
                        }
                    case ByteCommand.SUB:
                        {
                            Variant op1 = stack.Pop();
                            Variant op2 = stack.Pop();
                            stack.Push(op1 - op2);
                            break;
                        }
                    case ByteCommand.LT:
                        {
                            Variant op1 = stack.Pop();
                            Variant op2 = stack.Pop();
                            Variant res = new Variant(op1 < op2 ? 1.0 : 0.0);
                            stack.Push(res);
                            break;
                        }
                    case ByteCommand.GT:
                        {
                            Variant op1 = stack.Pop();
                            Variant op2 = stack.Pop();
                            Variant res = new Variant(op1 > op2 ? 1.0 : 0.0);
                            stack.Push(res);
                            break;
                        }
                    case ByteCommand.JZ:
                        {
                            if (stack.Pop() == (Variant)(0.0))
                            {
                                JmpToMark(ref i);
                            }
                            break;
                        }
                    case ByteCommand.JNZ:
                        {
                            if (stack.Pop() != (Variant)(0.0))
                            {
                                JmpToMark(ref i);
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