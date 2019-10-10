using System;

namespace VirtualMachine
{
    public enum ByteCommand : byte
    {
        CALL,
        RETURN,
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

        private bool Run(byte[] program)
        {
            byte* ppc;

            fixed (byte* p = program)
            {
                ppc = p;

                while (true)
                {
                    switch ((ByteCommand)(*(ppc++)))
                    {
                        case ByteCommand.CALL:
                            {
                                int mark = *((int*)ppc++);
                                m_stack.PushUp(*ppc);
                                ppc = p + mark;
                                m_stack.PushUp(m_stack.m_nbp);
                                m_stack.m_nbp = m_stack.m_nsp;
                                break;
                            }
                        case ByteCommand.RETURN:
                            {
                                m_stack.PushDown(m_stack.m_nsp != m_stack.m_nbp ? m_stack.PopUp() : null);
                                m_stack.m_nbp = (int)m_stack.PopDown();
                                byte b = (byte)m_stack.PopDown();
                                ppc = &b;
                                break;
                            }
                        case ByteCommand.FETCH:
                            {
                                m_stack.Pick(m_stack.m_nbp + (*((int*)ppc++)));
                                break;
                            }
                        case ByteCommand.STORE:
                            {
                                m_stack.Set(m_stack.m_nbp + *((int*)ppc++), m_stack.PopUp());
                                break;
                            }
                        case ByteCommand.PUSH:
                            {
                                m_stack.PushDown(*((Variant*)ppc++));
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
                                    ppc += *((int*)ppc++);
                                }
                                else
                                {
                                    ppc += 4;
                                }
                                break;
                            }
                        case ByteCommand.JNZ:
                            {
                                if (m_stack.PopUp() != 0.0)
                                {
                                    ppc += *((int*)ppc++);
                                }
                                else
                                {
                                    ppc += 4;
                                }
                                break;
                            }
                        case ByteCommand.JMP:
                            {
                                ppc += *((int*)ppc++);
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
