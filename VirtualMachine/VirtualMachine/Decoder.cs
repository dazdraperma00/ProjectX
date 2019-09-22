using System;
using System.Collections.Generic;

namespace VirtualMachine
{
    public struct Command
    {
        public ByteCommand command;
        public Variant oper;
    };

    class Decoder
    {
        public static readonly Exception bcc = new Exception("byte code corrupted");

        private byte[] code;
        private uint i = 0;

        private List<Command> program = new List<Command>();

        private Variant GetVar()
        {
            if (i + Variant.size < code.Length)
            {
                Variant arg = Variant.FromBytes(code, i);
                i += Variant.size;
                return arg;
            }
            else
            {
                throw bcc;
            }
        }

        public void LoadCode(byte[] _code)
        {
            code = _code;
        }

        public Command[] Decode()
        {
            while (i < code.Length)
            {
                Command command = new Command();
                command.command = (ByteCommand)(code[i++]);

                switch(command.command)
                {
                    case ByteCommand.ADD:
                    case ByteCommand.EQ:
                    case ByteCommand.GET:
                    case ByteCommand.GT:
                    case ByteCommand.HALT:
                    case ByteCommand.LET:
                    case ByteCommand.LT:
                    case ByteCommand.NEQ:
                    case ByteCommand.NONE:
                    case ByteCommand.POP:
                    case ByteCommand.SUB:
                        break;
                    case ByteCommand.FETCH:
                    case ByteCommand.JMP:
                    case ByteCommand.JNZ:
                    case ByteCommand.JZ:
                    case ByteCommand.PUSH:
                    case ByteCommand.STORE:
                        command.oper = GetVar();
                        break;
                    default:
                        throw bcc;
                }

                program.Add(command);
            }

            return program.ToArray();
        }
    }
}
