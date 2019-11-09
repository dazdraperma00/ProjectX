using System;

namespace VirtualMachine
{
    public enum ByteCommand : byte
    {
        CALL,
        RET,
        FETCH,
        STORE,
        LFETCH,
        LSTORE,
        LALLOC,
        LFREE,
        ALLOC,
        FREE,
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

        private Variant* m_sp;
        private Variant* m_bp;

        public VirtualMachine()
        {
            fixed (Variant* pStack = m_stack)
            {
                m_bp = pStack - 1;
                m_sp = pStack + m_stack.Length;
            }
        }

        private void Resize(Variant* pStack)
        {
            int inc = m_stack.Length >> 1;
            Variant[] vStack = new Variant[m_stack.Length + inc];

            fixed (Variant* p = vStack)
            {
                int size = (int)((byte*)(m_bp + 1) - (byte*)pStack);
                Buffer.MemoryCopy(pStack, p, size, size);
                Variant* sp = p + (m_sp - pStack + inc);
                size = (int)((byte*)(pStack + m_stack.Length) - (byte*)m_sp);
                Buffer.MemoryCopy(m_sp, sp, size, size);
                m_sp = sp;
                m_bp = p + (m_bp - pStack);
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
                byte* pc = p;

                while (true)
                {
                    fixed (Variant* vp = m_stack)
                    {
                        switch ((ByteCommand)(*pc))
                        {
                            case ByteCommand.CALL:
                                {
                                    int mark = *((int*)pc);
                                    pc += sizeof(int);
                                    if (m_bp + 2 >= m_sp)
                                    {
                                        Resize(vp);
                                        continue;
                                    }
                                    (++m_bp)->dValue = pc - p;
                                    pc = p + mark;
                                    break;
                                }
                            case ByteCommand.RET:
                                {
                                    pc = p + (int)((m_bp--)->dValue);
                                    break;
                                }
                            case ByteCommand.FETCH:
                                {
                                    if (m_sp - 1 == m_bp)
                                    {
                                        Resize(vp);
                                        continue;
                                    }
                                    *(--m_sp) = *(vp + m_stack.Length - *((int*)pc));
                                    pc += sizeof(int);
                                    break;
                                }
                            case ByteCommand.STORE:
                                {
                                    *(vp + m_stack.Length - *((int*)pc)) = *(m_sp++);
                                    pc += sizeof(int);
                                    break;
                                }
                            case ByteCommand.LFETCH:
                                {
                                    if (m_sp - 1 == m_bp)
                                    {
                                        Resize(vp);
                                        continue;
                                    }
                                    *(--m_sp) = *(m_bp - *((int*)pc));
                                    pc += sizeof(int);
                                    break;
                                }
                            case ByteCommand.LSTORE:
                                {
                                    *(m_bp - *((int*)pc)) = *(m_sp++);
                                    pc += sizeof(int);
                                    break;
                                }
                            case ByteCommand.LALLOC:
                                {
                                    int size = *((int*)pc);
                                    pc += sizeof(int);
                                    if (m_bp + size + 1 >= m_sp)
                                    {
                                        Resize(vp);
                                        continue;
                                    }
                                    m_bp += size + 1;
                                    m_bp->dValue = size;
                                    break;
                                }
                            case ByteCommand.LFREE:
                                {
                                    Variant* bp = m_bp - (int)(m_bp--)->dValue;
                                    while (m_bp > bp)
                                    {
                                        (m_bp--)->pValue = null;
                                    }
                                    break;
                                }
                            case ByteCommand.PUSH:
                                {
                                    if (m_sp - 1 == m_bp)
                                    {
                                        Resize(vp);
                                        continue;
                                    }
                                    *(--m_sp) = Variant.FromBytes(&pc);
                                    break;
                                }
                            case ByteCommand.POP:
                                {
                                    (m_sp++)->pValue = null;
                                    break;
                                }
                            case ByteCommand.ADD:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    op1->dValue += op2->dValue;
                                    op1->pValue = null;
                                    op2->pValue = null;
                                    break;
                                }
                            case ByteCommand.SUB:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    op1->dValue -= op2->dValue;
                                    op1->pValue = null;
                                    op2->pValue = null;
                                    break;
                                }
                            case ByteCommand.INC:
                                {
                                    ++m_sp->dValue;
                                    m_sp->pValue = null;
                                    break;
                                }
                            case ByteCommand.DEC:
                                {
                                    --m_sp->dValue;
                                    m_sp->pValue = null;
                                    break;
                                }
                            case ByteCommand.MULT:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    op1->dValue *= op2->dValue;
                                    op1->pValue = null;
                                    op2->pValue = null;
                                    break;
                                }
                            case ByteCommand.DIV:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    if (op2->dValue != 0.0)
                                    {
                                        op1->dValue /= op2->dValue;
                                    }
                                    else
                                    {
                                        op1->usValue0 = Variant.c_null;
                                    }
                                    op1->pValue = null;
                                    op2->pValue = null;
                                    break;
                                }
                            case ByteCommand.MOD:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    if (op2->dValue != 0.0)
                                    {
                                        op1->dValue = (int)op1->dValue % (int)op2->dValue;
                                    }
                                    else
                                    {
                                        op1->usValue0 = Variant.c_null;
                                    }
                                    op1->pValue = null;
                                    op2->pValue = null;
                                    break;
                                }
                            case ByteCommand.AND:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    op1->dValue = (op1->dValue != 0.0 && op2->dValue != 0.0
                                        && op1->usValue0 != Variant.c_null && op2->usValue0 != Variant.c_null) ? 1.0 : 0.0;
                                    op1->pValue = null;
                                    op2->pValue = null;
                                    break;
                                }
                            case ByteCommand.OR:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    op1->dValue = (op1->dValue != 0.0 && op1->usValue0 != Variant.c_null
                                        || op2->dValue != 0.0 && op2->usValue0 != Variant.c_null) ? 1.0 : 0.0;
                                    op1->pValue = null;
                                    op2->pValue = null;
                                    break;
                                }
                            case ByteCommand.NOT:
                                {
                                    m_sp->dValue = (m_sp->dValue == 0.0 || m_sp->usValue0 == Variant.c_null) ? 1.0 : 0.0;
                                    m_sp->pValue = null;
                                    break;
                                }
                            case ByteCommand.LT:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    op1->dValue = op1->dValue < op2->dValue ? 1.0 : 0.0;
                                    op1->pValue = null;
                                    op2->pValue = null;
                                    break;
                                }
                            case ByteCommand.GT:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    op1->dValue = op1->dValue > op2->dValue ? 1.0 : 0.0;
                                    op1->pValue = null;
                                    op2->pValue = null;
                                    break;
                                }
                            case ByteCommand.LET:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    op1->dValue = op1->dValue <= op2->dValue ? 1.0 : 0.0;
                                    op1->pValue = null;
                                    op2->pValue = null;
                                    break;
                                }
                            case ByteCommand.GET:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    op1->dValue = op1->dValue >= op2->dValue ? 1.0 : 0.0;
                                    op1->pValue = null;
                                    op2->pValue = null;
                                    break;
                                }
                            case ByteCommand.EQ:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    op1->dValue = Variant.Equal(op1, op2) ? 1.0 : 0.0;
                                    op1->pValue = null;
                                    op2->pValue = null;
                                    break;
                                }
                            case ByteCommand.NEQ:
                                {
                                    Variant* op2 = m_sp++;
                                    Variant* op1 = m_sp;

                                    op1->dValue = Variant.Equal(op1, op2) ? 0.0 : 1.0;
                                    op1->pValue = null;
                                    op1->pValue = null;
                                    break;
                                }
                            case ByteCommand.JZ:
                                {
                                    pc = m_sp++->dValue == 0.0 ? p + *((int*)pc) : pc + sizeof(int);
                                    break;
                                }
                            case ByteCommand.JNZ:
                                {
                                    pc = m_sp++->dValue != 0.0 ? p + *((int*)pc) : pc + sizeof(int);
                                    break;
                                }
                            case ByteCommand.JMP:
                                {
                                    pc = p + *((int*)pc);
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

                        ++pc;
                    }
                }
            }
        }
    }
}
