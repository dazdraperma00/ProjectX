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
        VAX,
        HALT,

        NONE
    }

    public enum State : byte
    {
        RUNNING,
        HALTED,
        BROKEN,
    }

    class VirtualMachine
    {
        private byte[] m_baProgram;
        private int m_npc = 0;

        private Stack m_stack = new Stack();
        private int m_nbp = -1;
        private Variant m_vax = null;

        private State m_state = State.HALTED;

        public void LoadProgram(byte[] program)
        {
            m_baProgram = program;
        }

        public State GetState()
        {
            return m_state;
        }

        public Stack GetStack()
        {
            return m_stack;
        }

        private Variant GetVar()
        {
            Variant arg = Variant.FromBytes(m_baProgram, m_npc);
            m_npc += Variant.s_size;
            return arg;
        }

        private int GetInt()
        {
            int arg = BitConverter.ToInt32(m_baProgram, m_npc);
            m_npc += sizeof(int);
            return arg;
        }

        public void Run()
        {
            m_state = State.RUNNING;

            while (true)
            {
                switch((ByteCommand)m_baProgram[m_npc++])
                {
                    case ByteCommand.CALL:
                        {
                            int mark = GetInt();
                            m_stack.Push(m_npc);
                            m_npc = mark;
                            m_stack.Push(m_nbp);
                            m_nbp = m_stack.GetStackPointer();
                            break;
                        }
                    case ByteCommand.RETURN:
                        {
                            m_vax = m_stack.GetStackPointer() != m_nbp ? m_stack.Pop() : null;
                            m_nbp = (int)m_stack.Pop();
                            m_npc = (int)m_stack.Pop();
                            break;
                        }
                    case ByteCommand.VAX:
                        {
                            m_stack.Push(m_vax);
                            break;
                        }
                    case ByteCommand.FETCH:
                        {
                            m_stack.Pick(m_nbp + GetInt());
                            break;
                        }
                    case ByteCommand.STORE:
                        {
                            m_stack.Set(m_nbp + GetInt(), m_stack.Pop());
                            break;
                        }
                    case ByteCommand.PUSH:
                        {
                            m_stack.Push(GetVar());
                            break;
                        }
                    case ByteCommand.POP:
                        {
                            m_stack.Pop();
                            break;
                        }
                    case ByteCommand.ADD:
                        {
                            Variant op2 = m_stack.Pop();
                            Variant op1 = m_stack.Pop();
                            m_stack.Push(op1 + op2);
                            break;
                        }
                    case ByteCommand.SUB:
                        {
                            Variant op2 = m_stack.Pop();
                            Variant op1 = m_stack.Pop();
                            m_stack.Push(op1 - op2);
                            break;
                        }
                    case ByteCommand.LT:
                        {
                            Variant op2 = m_stack.Pop();
                            Variant op1 = m_stack.Pop();
                            Variant res = new Variant(op1 < op2 ? 1.0 : 0.0);
                            m_stack.Push(res);
                            break;
                        }
                    case ByteCommand.GT:
                        {
                            Variant op2 = m_stack.Pop();
                            Variant op1 = m_stack.Pop();
                            Variant res = new Variant(op1 > op2 ? 1.0 : 0.0);
                            m_stack.Push(res);
                            break;
                        }
                    case ByteCommand.LET:
                        {
                            Variant op2 = m_stack.Pop();
                            Variant op1 = m_stack.Pop();
                            Variant res = new Variant(op1 <= op2 ? 1.0 : 0.0);
                            m_stack.Push(res);
                            break;
                        }
                    case ByteCommand.GET:
                        {
                            Variant op2 = m_stack.Pop();
                            Variant op1 = m_stack.Pop();
                            Variant res = new Variant(op1 >= op2 ? 1.0 : 0.0);
                            m_stack.Push(res);
                            break;
                        }
                    case ByteCommand.EQ:
                        {
                            Variant op2 = m_stack.Pop();
                            Variant op1 = m_stack.Pop();
                            Variant res = new Variant(op1 == op2 ? 1.0 : 0.0);
                            m_stack.Push(res);
                            break;
                        }
                    case ByteCommand.NEQ:
                        {
                            Variant op2 = m_stack.Pop();
                            Variant op1 = m_stack.Pop();
                            Variant res = new Variant(op1 != op2 ? 1.0 : 0.0);
                            m_stack.Push(res);
                            break;
                        }
                    case ByteCommand.JZ:
                        {
                            if (m_stack.Pop() == 0.0)
                            {
                                m_npc += GetInt();
                            }
                            else
                            {
                                m_npc += sizeof(int);
                            }
                            break;
                        }
                    case ByteCommand.JNZ:
                        {
                            if (m_stack.Pop() != 0.0)
                            {
                                m_npc += GetInt();
                            }
                            else
                            {
                                m_npc += sizeof(int);
                            }
                            break;
                        }
                    case ByteCommand.JMP:
                        {
                            m_npc += GetInt();
                            break;
                        }
                    case ByteCommand.NONE:
                        {
                            break;
                        }
                    case ByteCommand.HALT:
                        {
                            m_state = State.HALTED;
                            return;
                        }
                    default:
                        {
                            m_state = State.BROKEN;
                            return;
                        }
                }
            }
        }
    }
}
