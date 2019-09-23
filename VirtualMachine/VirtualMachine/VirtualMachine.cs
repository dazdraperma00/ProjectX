using System;

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
    public enum State : byte
    {
        RUNNING,
        HALTED,
        BROKEN,
    }

    class VirtualMachine
    {
        private Command[] program;
        private int pc = 0;

        private Stack stack = new Stack();

        private State state = State.HALTED;

        public void LoadProgram(Command[] _program)
        {
            program = _program;
        }

        public State GetState()
        {
            return state;
        }

        public Stack GetStack()
        {
            return stack;
        }

        public void Run()
        {
            state = State.RUNNING;

            while (true)
            {
                switch((ByteCommand)program[pc].command)
                {
                    case ByteCommand.FETCH:
                        {
                            stack.Pick(((OffsetCommand)program[pc]).offset);
                            break;
                        }
                    case ByteCommand.STORE:
                        {
                            stack.Set(((OffsetCommand)program[pc]).offset, stack.Pop());
                            break;
                        }
                    case ByteCommand.PUSH:
                        {
                            stack.Push(((VarCommand)program[pc]).var);
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
                                pc += ((OffsetCommand)program[pc]).offset - 1;
                            }
                            break;
                        }
                    case ByteCommand.JNZ:
                        {
                            if (stack.Pop() != 0.0)
                            {
                                pc += ((OffsetCommand)program[pc]).offset - 1;
                            }
                            break;
                        }
                    case ByteCommand.JMP:
                        {
                            pc += ((OffsetCommand)program[pc]).offset - 1;
                            break;
                        }
                    case ByteCommand.NONE:
                        {
                            break;
                        }
                    case ByteCommand.HALT:
                        {
                            state = State.HALTED;
                            return;
                        }
                    default:
                        {
                            state = State.BROKEN;
                            return;
                        }
                }

                ++pc;
            }
        }
    }
}
