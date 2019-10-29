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
        MOD,
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
        HALT,

        NONE
    }

    unsafe class VirtualMachine
    {
        public const int c_capacity = 16;

        private Variant[] m_stack = new Variant[c_capacity];

        private Variant* m_nsp;
        private Variant* m_nbp;
        private Variant m_vax = Variant.s_null;

        public VirtualMachine()
        {
            fixed (Variant* p = m_stack)
            {
                m_nsp = p + c_capacity;
                m_nbp = p - 1;
            }
        }

        private void Resize()
        {
            //shoud not working!
            int inc = m_stack.Length / 2;
            Variant[] vStack = new Variant[m_stack.Length + inc];

            fixed (Variant* p = m_stack)
            {
                Buffer.BlockCopy(m_stack, 0, vStack, 0, (int)(m_nbp - p));
                int nspSize = (int)(m_nsp - p);
                Buffer.BlockCopy(m_stack, nspSize, vStack, nspSize + inc, m_stack.Length - 1 - nspSize);
            }

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

                (++m_nbp)->m_ulValue = (ulong)m_nsp;

                while (true)
                {
                    switch ((ByteCommand)(*(ppc++)))
                    {
                        case ByteCommand.CALL:
                            {
                                int mark = *((int*)ppc);
                                ppc += sizeof(int);
                                if (m_nbp + 2 == m_nsp)
                                {
                                    Resize();
                                }
                                (++m_nbp)->m_ulValue = (ulong)ppc;
                                (++m_nbp)->m_ulValue = (ulong)m_nsp;
                                ppc = p + mark;
                                break;
                            }
                        case ByteCommand.RET:
                            {
                                m_vax = m_nsp != (Variant*)m_nbp->m_ulValue ? *(m_nsp++) : Variant.s_null;
                                if (m_nsp < (Variant*)m_nbp->m_pValue)
                                {
                                    m_nsp = (Variant*)(m_nbp--)->m_ulValue;
                                }
                                else
                                {
                                    m_nbp--;
                                }
                                ppc = (byte*)(m_nbp--)->m_ulValue;
                                break;
                            }
                        case ByteCommand.VAX:
                            {
                                if (m_nsp - 1 == m_nbp)
                                {
                                    Resize();
                                }
                                *(--m_nsp) = m_vax;
                                break;
                            }
                        case ByteCommand.FETCH:
                            {
                                if (m_nsp - 1 == m_nbp)
                                {
                                    Resize();
                                }
                                *(--m_nsp) = *((Variant*)m_nbp->m_ulValue - *((int*)ppc));
                                ppc += sizeof(int);
                                break;
                            }
                        case ByteCommand.STORE:
                            {
                                *((Variant*)m_nbp->m_ulValue - *((int*)ppc)) = *(m_nsp++);
                                ppc += sizeof(int);
                                break;
                            }
                        case ByteCommand.PUSH:
                            {
                                if (m_nsp - 1 == m_nbp)
                                {
                                    Resize();
                                }
                                *(--m_nsp) = Variant.FromBytes(ref ppc);
                                break;
                            }
                        case ByteCommand.POP:
                            {
                                (m_nsp++)->m_pValue = null;
                                break;
                            }
                        case ByteCommand.ADD:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null)
                                {
                                    op1->m_dValue = op1->m_dValue + op2->m_dValue;
                                }
                                else if ((Variant.VarType)op1->m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2->m_usValue1 == Variant.VarType.STR)
                                {
                                    char[] resStr = new char[op1->m_nValue1 + op2->m_nValue1];

                                    fixed (char* pStr = resStr)
                                    {
                                        int size = sizeof(char) * op1->m_nValue1;
                                        Buffer.MemoryCopy(op1->m_pValue, pStr, size, size);
                                        size = sizeof(char) * op2->m_nValue1;
                                        Buffer.MemoryCopy(op2->m_pValue, (void*)(((char*)pStr) + op1->m_nValue1), size, size);

                                        op1->m_nValue1 = resStr.Length;
                                        op1->m_pValue = pStr;
                                    }
                                }
                                else
                                {
                                    *op1 = Variant.s_null;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.SUB:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null)
                                {
                                    op1->m_dValue = op1->m_dValue - op2->m_dValue;
                                }
                                else
                                {
                                    *op1 = Variant.s_null;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.INC:
                            {
                                if (m_nsp->m_usValue0 != Variant.c_null)
                                {
                                    ++m_nsp->m_dValue;
                                }
                                else
                                {
                                    *m_nsp = Variant.s_null;
                                }

                                break;
                            }
                        case ByteCommand.DEC:
                            {
                                if (m_nsp->m_usValue0 != Variant.c_null)
                                {
                                    --m_nsp->m_dValue;
                                }
                                else
                                {
                                    *m_nsp = Variant.s_null;
                                }

                                break;
                            }
                        case ByteCommand.MULT:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null)
                                {
                                    op1->m_dValue = op1->m_dValue * op2->m_dValue;
                                }
                                else
                                {
                                    *op1 = Variant.s_null;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.DIV:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null && op2->m_dValue != 0.0)
                                {
                                    op1->m_dValue = op1->m_dValue / op2->m_dValue;
                                }
                                else
                                {
                                    *op1 = Variant.s_null;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.MOD:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null && op2->m_dValue != 0.0)
                                {
                                    op1->m_dValue = op1->m_dValue % op2->m_dValue;
                                }
                                else
                                {
                                    *op1 = Variant.s_null;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.AND:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null)
                                {
                                    op1->m_dValue = (op1->m_dValue != 0.0 && op2->m_dValue != 0.0) ? 1.0 : 0.0;
                                }
                                else
                                {
                                    op1->m_dValue = 0.0;
                                    op1->m_pValue = null;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.OR:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null)
                                {
                                    op1->m_dValue = (op1->m_dValue != 0.0 || op2->m_dValue != 0.0) ? 1.0 : 0.0;
                                }
                                else
                                {
                                    op1->m_dValue = 0.0;
                                    op1->m_pValue = null;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.NOT:
                            {
                                if (m_nsp->m_usValue0 != Variant.c_null)
                                {
                                    m_nsp->m_dValue = m_nsp->m_dValue == 0.0 ? 1.0 : 0.0;
                                }
                                else
                                {
                                    *m_nsp = Variant.s_null;
                                }
                                break;
                            }
                        case ByteCommand.LT:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null)
                                {
                                    op1->m_dValue = op1->m_dValue < op2->m_dValue ? 1.0 : 0.0;
                                }
                                else if ((Variant.VarType)op1->m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2->m_usValue1 == Variant.VarType.STR)
                                {
                                    op1->m_dValue = op1->m_uValue0 < op2->m_uValue0 ? 1.0 : 0.0;
                                    op1->m_pValue = null;
                                }
                                else
                                {
                                    op1->m_dValue = 0.0;
                                    op1->m_pValue = null;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.GT:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null)
                                {
                                    op1->m_dValue = op1->m_dValue > op2->m_dValue ? 1.0 : 0.0;
                                }
                                else if ((Variant.VarType)op1->m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2->m_usValue1 == Variant.VarType.STR)
                                {
                                    op1->m_dValue = op1->m_uValue0 > op2->m_uValue0 ? 1.0 : 0.0;
                                    op1->m_pValue = null;
                                }
                                else
                                {
                                    op1->m_dValue = 0.0;
                                    op1->m_pValue = null;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.LET:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null)
                                {
                                    op1->m_dValue = op1->m_dValue <= op2->m_dValue ? 1.0 : 0.0;
                                }
                                else if ((Variant.VarType)op1->m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2->m_usValue1 == Variant.VarType.STR)
                                {
                                    op1->m_dValue = op1->m_uValue0 <= op2->m_uValue0 ? 1.0 : 0.0;
                                    op1->m_pValue = null;
                                }
                                else
                                {
                                    op1->m_dValue = 0.0;
                                    op1->m_pValue = null;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.GET:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null)
                                {
                                    op1->m_dValue = op1->m_dValue >= op2->m_dValue ? 1.0 : 0.0;
                                }
                                else if ((Variant.VarType)op1->m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2->m_usValue1 == Variant.VarType.STR)
                                {
                                    op1->m_dValue = op1->m_uValue0 >= op2->m_uValue0 ? 1.0 : 0.0;
                                    op1->m_pValue = null;
                                }
                                else
                                {
                                    op1->m_dValue = 0.0;
                                    op1->m_pValue = null;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.EQ:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null)
                                {
                                    op1->m_dValue = op1->m_dValue == op2->m_dValue ? 1.0 : 0.0;
                                }
                                else if ((Variant.VarType)op1->m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2->m_usValue1 == Variant.VarType.STR)
                                {
                                    if (op1->m_nValue1 != op2->m_nValue1)
                                    {
                                        op1->m_dValue = 0.0;
                                        op1->m_pValue = null;
                                        break;
                                    }

                                    for (int i = 0; i < op1->m_nValue1; ++i)
                                    {
                                        if (((char*)op1->m_pValue)[i] != ((char*)op2->m_pValue)[i])
                                        {
                                            op1->m_dValue = 0.0;
                                            op1->m_pValue = null;
                                            break;
                                        }
                                    }

                                    op1->m_dValue = 1.0;
                                    op1->m_pValue = null;
                                }
                                else
                                {
                                    op1->m_dValue = 0.0;
                                    op1->m_pValue = null;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.NEQ:
                            {
                                Variant* op2 = m_nsp++;
                                Variant* op1 = m_nsp;

                                if (op1->m_usValue0 != Variant.c_null && op2->m_usValue0 != Variant.c_null)
                                {
                                    op1->m_dValue = op1->m_dValue != op2->m_dValue ? 1.0 : 0.0;
                                }
                                else if ((Variant.VarType)op1->m_usValue1 == Variant.VarType.STR && (Variant.VarType)op2->m_usValue1 == Variant.VarType.STR)
                                {
                                    if (op1->m_nValue1 != op2->m_nValue1)
                                    {
                                        op1->m_dValue = 1.0;
                                        op1->m_pValue = null;
                                        break;
                                    }

                                    for (int i = 0; i < op1->m_nValue1; ++i)
                                    {
                                        if (((char*)op1->m_pValue)[i] != ((char*)op2->m_pValue)[i])
                                        {
                                            op1->m_dValue = 1.0;
                                            op1->m_pValue = null;
                                            break;
                                        }
                                    }

                                    op1->m_dValue = 0.0;
                                    op1->m_pValue = null;
                                }
                                else
                                {
                                    op1->m_dValue = 0.0;
                                }
                                op2->m_pValue = null;
                                break;
                            }
                        case ByteCommand.JZ:
                            {
                                ppc = m_nsp++->m_dValue == 0.0 ? p + *((int*)ppc) : ppc + sizeof(int);
                                break;
                            }
                        case ByteCommand.JNZ:
                            {
                                ppc = m_nsp++->m_dValue != 0.0 ? p + *((int*)ppc) : ppc + sizeof(int);
                                break;
                            }
                        case ByteCommand.JMP:
                            {
                                ppc = p + *((int*)ppc);
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
                }
            }
        }
    }
}
