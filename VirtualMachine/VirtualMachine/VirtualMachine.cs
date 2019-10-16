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
                Variant vax = 0.0;

                m_stack.PushUp(m_stack.m_nsp);

                while (true)
                {
                    switch ((ByteCommand)(*(ppc++)))
                    {
                        case ByteCommand.CALL:
                            {
                                int mark = *((int*)ppc);
                                ppc += sizeof(int);
                                m_stack.PushUp(ppc - p);
                                m_stack.PushUp(m_stack.m_nsp);
                                ppc = p + mark;
                                break;
                            }
                        case ByteCommand.RET:
                            {
                                vax = m_stack.m_nsp != m_stack.m_nbp ? m_stack.PopUp() : new Variant(0.0);
                                m_stack.m_nsp = m_stack.PopDown();
                                ppc = p + m_stack.PopDown();
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
                                m_stack.PushDown(*((Variant*)ppc));
                                ppc += sizeof(Variant);
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
                                m_stack.PushDown(op1 + op2);
                                break;
                            }
                        case ByteCommand.SUB:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                m_stack.PushDown(op1 - op2);
                                break;
                            }
                        case ByteCommand.LT:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res = new Variant(op1 < op2 ? 1.0 : 0.0);
                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.GT:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res = new Variant(op1 > op2 ? 1.0 : 0.0);
                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.LET:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res = new Variant(op1 <= op2 ? 1.0 : 0.0);
                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.GET:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res = new Variant(op1 >= op2 ? 1.0 : 0.0);
                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.EQ:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res = new Variant(op1 == op2 ? 1.0 : 0.0);
                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.NEQ:
                            {
                                Variant op2 = m_stack.PopUp();
                                Variant op1 = m_stack.PopUp();
                                Variant res = new Variant(op1 != op2 ? 1.0 : 0.0);
                                m_stack.PushDown(res);
                                break;
                            }
                        case ByteCommand.JZ:
                            {
                                if (m_stack.PopUp() == 0.0)
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
                                if (m_stack.PopUp() != 0.0)
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
