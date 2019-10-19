﻿using System;

namespace VirtualMachine
{
    public enum ByteCommand : byte
    {
        CALL,
        RET,
        VAX,
        FETCH,
        STORE,
        PUSH,
        POP,
        ADD,
        SUB,
        INC,
        DEC,
        MULT,
        DIV,
        AND,
        OR,
        NOT,
        LT,
        GT,
        LET,
        GET,
        EQ,
        NEQ,
        JZ,
        JNZ,
        JMP,
        LAMBDA,
        HALT,

        NONE
    }

    unsafe class VirtualMachine
    {
        private Stack m_stack = new Stack();

        public Stack GetStack()
        {
            return m_stack;
        }

        public bool Run(byte[] program)
        {
            fixed (byte* p = program)
            {
                byte* ppc = p;
                Variant vax = Variant.s_null;

                m_stack.PushUp(new Variant(m_stack.m_nsp));

                while (true)
                {
                    switch ((ByteCommand)(*(ppc++)))
                    {
                        case ByteCommand.CALL:
                            {
                                //Ask about it
                                int mark = *((int*)ppc);
                                ppc += sizeof(int);
                                m_stack.PushUp(new Variant(ppc - p));
                                m_stack.PushUp(new Variant(m_stack.m_nsp));
                                ppc = p + mark;
                                break;
                            }
                        case ByteCommand.RET:
                            {
                                vax = m_stack.m_nsp != m_stack.m_nbp ? m_stack.PopUp() : new Variant(0.0);
                                m_stack.m_nsp = (int)m_stack.PopDown().m_dValue;
                                ppc = p + (int)m_stack.PopDown().m_dValue;
                                break;
                            }
                        case ByteCommand.VAX:
                            {
                                m_stack.PushDown(vax);
                                break;
                            }
                        case ByteCommand.FETCH:
                            {
                                m_stack.Pick(*((int*)ppc));
                                ppc += sizeof(int);
                                break;
                            }
                        case ByteCommand.STORE:
                            {
                                m_stack.Set(*((int*)ppc), m_stack.PopUp());
                                ppc += sizeof(int);
                                break;
                            }
                        case ByteCommand.PUSH:
                            {
                                m_stack.PushDown(Variant.FromBytes(ref ppc));
                                break;
                            }
                        case ByteCommand.POP:
                            {
                                m_stack.PopUp();
                                break;
                            }
                        case ByteCommand.ADD:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant(op1.m_dValue + op2.m_dValue);
                                }
                                else if ((Variant.VarType)op1.m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2.m_usValue1 == Variant.VarType.STR)
                                {
                                    char[] resStr = new char[op1.m_nValue1 + op2.m_nValue1];

                                    fixed (char* pStr = resStr)
                                    {
                                        int size = sizeof(char) * op1.m_nValue1;
                                        Buffer.MemoryCopy(op1.m_pValue, pStr, size, size);
                                        size = sizeof(char) * op2.m_nValue1;
                                        Buffer.MemoryCopy(op2.m_pValue, (void*)(((char*)pStr) + op1.m_nValue1), size, size);
                                    }

                                    res = new Variant(resStr);
                                }
                                else
                                {
                                    res = Variant.s_null;
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.SUB:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant(op1.m_dValue - op2.m_dValue);
                                }
                                else
                                {
                                    res = Variant.s_null;
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.INC:
                            {
                                if (m_stack.m_stack[m_stack.m_nsp++].m_usValue0 != Variant.c_null)
                                {
                                    ++m_stack.m_stack[m_stack.m_nsp++].m_dValue;
                                }
                                else
                                {
                                    m_stack.m_stack[m_stack.m_nsp++] = Variant.s_null;
                                }

                                break;
                            }
                        case ByteCommand.DEC:
                            {
                                if (m_stack.m_stack[m_stack.m_nsp++].m_usValue0 != Variant.c_null)
                                {
                                    --m_stack.m_stack[m_stack.m_nsp++].m_dValue;
                                }
                                else
                                {
                                    m_stack.m_stack[m_stack.m_nsp++] = Variant.s_null;
                                }

                                break;
                            }
                        case ByteCommand.MULT:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant(op1.m_dValue * op2.m_dValue);
                                }
                                else
                                {
                                    res = Variant.s_null;
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.DIV:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null && op2.m_dValue != 0.0)
                                {
                                    res = new Variant(op1.m_dValue / op2.m_dValue);
                                }
                                else
                                {
                                    res = Variant.s_null;
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.AND:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant((op1.m_dValue != 0 && op2.m_dValue != 0) ? 1.0 : 0.0);
                                }
                                else
                                {
                                    res = new Variant(0.0);
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.OR:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant((op1.m_dValue != 0 || op2.m_dValue != 0) ? 1.0 : 0.0);
                                }
                                else
                                {
                                    res = new Variant(0.0);
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.NOT:
                            {
                                Variant var = m_stack.PopUp();
                                Variant res;

                                if (var.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant(var.m_dValue == 0 ? 1.0 : 0.0);
                                }
                                else
                                {
                                    res = Variant.s_null;
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.LT:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant(op1.m_dValue < op2.m_dValue ? 1.0 : 0.0);
                                }
                                else if ((Variant.VarType)op1.m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2.m_usValue1 == Variant.VarType.STR)
                                {
                                    res = new Variant(op1.m_uValue0 < op2.m_uValue0 ? 1.0 : 0.0);
                                }
                                else
                                {
                                    res = new Variant(0.0);
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.GT:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant(op1.m_dValue > op2.m_dValue ? 1.0 : 0.0);
                                }
                                else if ((Variant.VarType)op1.m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2.m_usValue1 == Variant.VarType.STR)
                                {
                                    res = new Variant(op1.m_uValue0 > op2.m_uValue0 ? 1.0 : 0.0);
                                }
                                else
                                {
                                    res = new Variant(0.0);
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.LET:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant(op1.m_dValue <= op2.m_dValue ? 1.0 : 0.0);
                                }
                                else if ((Variant.VarType)op1.m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2.m_usValue1 == Variant.VarType.STR)
                                {
                                    res = new Variant(op1.m_uValue0 <= op2.m_uValue0 ? 1.0 : 0.0);
                                }
                                else
                                {
                                    res = new Variant(0.0);
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.GET:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant(op1.m_dValue >= op2.m_dValue ? 1.0 : 0.0);
                                }
                                else if ((Variant.VarType)op1.m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2.m_usValue1 == Variant.VarType.STR)
                                {
                                    res = new Variant(op1.m_uValue0 >= op2.m_uValue0 ? 1.0 : 0.0);
                                }
                                else
                                {
                                    res = new Variant(0.0);
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.EQ:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant(op1.m_dValue == op2.m_dValue ? 1.0 : 0.0);
                                }
                                else if ((Variant.VarType)op1.m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2.m_usValue1 == Variant.VarType.STR)
                                {
                                    if (op1.m_nValue1 != op2.m_nValue1)
                                    {
                                        res = new Variant(0.0);
                                    }

                                    for (int i = 0; i < op1.m_nValue1; ++i)
                                    {
                                        if (((char*)op1.m_pValue)[i] != ((char*)op2.m_pValue)[i])
                                        {
                                            res = new Variant(0.0);
                                        }
                                    }

                                    res = new Variant(1.0);
                                }
                                else
                                {
                                    res = new Variant(0.0);
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.NEQ:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant(op1.m_dValue != op2.m_dValue ? 1.0 : 0.0);
                                }
                                else if ((Variant.VarType)op1.m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2.m_usValue1 == Variant.VarType.STR)
                                {
                                    if (op1.m_nValue1 != op2.m_nValue1)
                                    {
                                        res = new Variant(1.0);
                                    }

                                    for (int i = 0; i < op1.m_nValue1; ++i)
                                    {
                                        if (((char*)op1.m_pValue)[i] != ((char*)op2.m_pValue)[i])
                                        {
                                            res = new Variant(1.0);
                                        }
                                    }

                                    res = new Variant(0.0);
                                }
                                else
                                {
                                    res = new Variant(0.0);
                                }

                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.JZ:
                            {
                                if (m_stack.PopUp().m_dValue == 0.0)
                                {
                                    ppc += *((int*)ppc);
                                    ppc += sizeof(int);
                                }
                                else
                                {
                                    ppc += sizeof(int);
                                }
                                break;
                            }
                        case ByteCommand.JNZ:
                            {
                                if (m_stack.PopUp().m_dValue != 0.0)
                                {
                                    ppc += *((int*)ppc);
                                    ppc += sizeof(int);
                                }
                                else
                                {
                                    ppc += sizeof(int);
                                }
                                break;
                            }
                        case ByteCommand.JMP:
                            {
                                ppc += *((int*)ppc);
                                ppc += sizeof(int);
                                break;
                            }
                        case ByteCommand.LAMBDA:
                            {
                                //Ask about it
                                int mark = *((int*)ppc);
                                ppc += sizeof(int);
                                int length = *((int*)ppc);
                                ppc += sizeof(int);
                                m_stack.PushUp(new Variant(ppc - p));
                                ppc = p + mark;
                                m_stack.PushUp(new Variant(ppc + length - p));
                                break;
                            }
                        case ByteCommand.NONE:
                            {
                                break;
                            }
                        case ByteCommand.HALT:
                            {
                                return true;
                            }
                        default:
                            {
                                return false;
                            }
                    }

                    if (ppc - p == (int)m_stack.m_stack[m_stack.m_nbp].m_dValue)
                    {
                        m_stack.PopDown();
                        ppc = p + (int)m_stack.PopDown().m_dValue;
                    }
                }
            }
        }
    }
}
