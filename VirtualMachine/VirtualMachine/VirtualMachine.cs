using System;

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
        public const int c_capacity = 16;

        private Variant[] m_stack = new Variant[c_capacity];

        private int m_nsp = c_capacity;
        private int m_nbp = -1;
        private Variant m_vax = Variant.s_null;

        private void Resize()
        {
            int inc = m_stack.Length / 2;
            Variant[] vStack = new Variant[m_stack.Length + inc];

            Buffer.BlockCopy(m_stack, 0, vStack, 0, m_nbp);
            Buffer.BlockCopy(m_stack, m_nsp, vStack, m_nsp + inc, m_stack.Length - 1 - m_nsp);

            m_stack = vStack;
        }

        public Variant[] GetStack()
        {
            return m_stack;
        }

        public bool Run(byte[] program)
        {
            fixed (byte* p = program)
            {
                byte* ppc = p;

                m_stack[++m_nbp] = new Variant(m_nsp);

                while (true)
                {
                    switch ((ByteCommand)(*(ppc++)))
                    {
                        case ByteCommand.CALL:
                            {
                                //Ask about it
                                int mark = *((int*)ppc);
                                ppc += sizeof(int);
                                if (m_nsp + 2 == m_nbp)
                                {
                                    Resize();
                                }
                                m_stack[++m_nbp] = new Variant(ppc - p);
                                m_stack[++m_nbp] = new Variant(m_nsp);
                                ppc = p + mark;
                                break;
                            }
                        case ByteCommand.RET:
                            {
                                m_vax = m_nsp != (int)m_stack[m_nbp].m_dValue ? m_stack[m_nsp++] : Variant.s_null;
                                m_nsp = (int)m_stack[m_nbp--].m_dValue;
                                ppc = p + (int)m_stack[m_nbp--].m_dValue;
                                break;
                            }
                        case ByteCommand.VAX:
                            {
                                if (m_nsp - 1 == m_nbp)
                                {
                                    Resize();
                                }
                                m_stack[--m_nsp] = m_vax;
                                break;
                            }
                        case ByteCommand.FETCH:
                            {
                                if (m_nsp - 1 == m_nbp)
                                {
                                    Resize();
                                }
                                m_stack[--m_nsp] = m_stack[(int)m_stack[m_nbp].m_dValue - *((int*)ppc)];
                                ppc += sizeof(int);
                                break;
                            }
                        case ByteCommand.STORE:
                            {
                                m_stack[m_stack.Length - 1 - *((int*)ppc)] = m_stack[m_nsp++];
                                ppc += sizeof(int);
                                break;
                            }
                        case ByteCommand.PUSH:
                            {
                                if (m_nsp + 1 == m_nbp)
                                {
                                    Resize();
                                }
                                m_stack[--m_nsp] = Variant.FromBytes(ref ppc);
                                break;
                            }
                        case ByteCommand.POP:
                            {
                                m_nsp++;
                                break;
                            }
                        case ByteCommand.ADD:
                            {
                                Variant op2 = m_stack[m_nsp++];
                                Variant op1 = m_stack[m_nsp];
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

                                m_stack[m_nsp] = res;
                                break;
                            }
                        case ByteCommand.SUB:
                            {
                                Variant op2 = m_stack[m_nsp++];
                                Variant op1 = m_stack[m_nsp];
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant(op1.m_dValue - op2.m_dValue);
                                }
                                else
                                {
                                    res = Variant.s_null;
                                }

                                m_stack[m_nsp] = res;
                                break;
                            }
                        case ByteCommand.INC:
                            {
                                if (m_stack[m_nsp].m_usValue0 != Variant.c_null)
                                {
                                    ++m_stack[m_nsp].m_dValue;
                                }
                                else
                                {
                                    m_stack[m_nsp] = Variant.s_null;
                                }

                                break;
                            }
                        case ByteCommand.DEC:
                            {
                                if (m_stack[m_nsp].m_usValue0 != Variant.c_null)
                                {
                                    --m_stack[m_nsp].m_dValue;
                                }
                                else
                                {
                                    m_stack[m_nsp] = Variant.s_null;
                                }

                                break;
                            }
                        case ByteCommand.MULT:
                            {
                                Variant op2 = m_stack[m_nsp++];
                                Variant op1 = m_stack[m_nsp];
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant(op1.m_dValue * op2.m_dValue);
                                }
                                else
                                {
                                    res = Variant.s_null;
                                }

                                m_stack[m_nsp] = res;
                                break;
                            }
                        case ByteCommand.DIV:
                            {
                                Variant op2 = m_stack[m_nsp++];
                                Variant op1 = m_stack[m_nsp];
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null && op2.m_dValue != 0.0)
                                {
                                    res = new Variant(op1.m_dValue / op2.m_dValue);
                                }
                                else
                                {
                                    res = Variant.s_null;
                                }

                                m_stack[m_nsp] = res;
                                break;
                            }
                        case ByteCommand.AND:
                            {
                                Variant op2 = m_stack[m_nsp++];
                                Variant op1 = m_stack[m_nsp];
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant((op1.m_dValue != 0 && op2.m_dValue != 0) ? 1.0 : 0.0);
                                }
                                else
                                {
                                    res = new Variant(0.0);
                                }

                                m_stack[m_nsp] = res;
                                break;
                            }
                        case ByteCommand.OR:
                            {
                                Variant op2 = m_stack[m_nsp++];
                                Variant op1 = m_stack[m_nsp];
                                Variant res;

                                if (op1.m_usValue0 != Variant.c_null && op2.m_usValue0 != Variant.c_null)
                                {
                                    res = new Variant((op1.m_dValue != 0 || op2.m_dValue != 0) ? 1.0 : 0.0);
                                }
                                else
                                {
                                    res = new Variant(0.0);
                                }

                                m_stack[m_nsp] = res;
                                break;
                            }
                        case ByteCommand.NOT:
                            {
                                if (m_stack[m_nsp].m_usValue0 != Variant.c_null)
                                {
                                    m_stack[m_nsp].m_dValue = m_stack[m_nsp].m_dValue == 0.0 ? 1.0 : 0.0;
                                }
                                else
                                {
                                    m_stack[m_nsp] = Variant.s_null;
                                }
                                break;
                            }
                        case ByteCommand.LT:
                            {
                                Variant op2 = m_stack[m_nsp++];
                                Variant op1 = m_stack[m_nsp];
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

                                m_stack[m_nsp] = res;
                                break;
                            }
                        case ByteCommand.GT:
                            {
                                Variant op2 = m_stack[m_nsp++];
                                Variant op1 = m_stack[m_nsp];
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

                                m_stack[m_nsp] = res;
                                break;
                            }
                        case ByteCommand.LET:
                            {
                                Variant op2 = m_stack[m_nsp++];
                                Variant op1 = m_stack[m_nsp];
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

                                m_stack[m_nsp] = res;
                                break;
                            }
                        case ByteCommand.GET:
                            {
                                Variant op2 = m_stack[m_nsp++];
                                Variant op1 = m_stack[m_nsp];
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

                                m_stack[m_nsp] = res;
                                break;
                            }
                        case ByteCommand.EQ:
                            {
                                Variant op2 = m_stack[m_nsp++];
                                Variant op1 = m_stack[m_nsp];
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

                                m_stack[m_nsp] = res;
                                break;
                            }
                        case ByteCommand.NEQ:
                            {
                                Variant op2 = m_stack[m_nsp++];
                                Variant op1 = m_stack[m_nsp];
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

                                m_stack[m_nsp] = res;
                                break;
                            }
                        case ByteCommand.JZ:
                            {
                                if (m_stack[m_nsp++].m_dValue == 0.0)
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
                                if (m_stack[m_nsp++].m_dValue != 0.0)
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
                                if (m_nbp + 2 == m_nsp)
                                {
                                    Resize();
                                }
                                m_stack[++m_nbp] = new Variant(ppc - p);
                                ppc = p + mark;
                                m_stack[++m_nbp] = new Variant(ppc + length - p);
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

                    if (ppc - p == (int)m_stack[m_nbp].m_dValue)
                    {
                        --m_nbp;
                        ppc = p + (int)m_stack[m_nbp--].m_dValue;
                    }
                }
            }
        }
    }
}
