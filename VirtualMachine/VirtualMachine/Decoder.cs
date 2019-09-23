using System;
using System.Collections.Generic;

namespace VirtualMachine
{
    public class Command
    {
        public ByteCommand command;
    };

    public class VarCommand : Command
    {
        public Variant var;
    };

    public class OffsetCommand : Command
    {
        public int offset;
    }

    class Decoder
    {
        public static readonly Exception bcc = new Exception("byte code corrupted");

        private byte[] code;
        private int i = 0;

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

        private int GetOffset()
        {
            if (i + sizeof(int) < code.Length)
            {
                int arg = BitConverter.ToInt32(code, i);
                i += sizeof(int);
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
                Command command;
                ByteCommand byteCommand = (ByteCommand)code[i++];

                switch(byteCommand)
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
                        command = new Command();
                        break;
                    case ByteCommand.FETCH:
                    case ByteCommand.JMP:
                    case ByteCommand.JNZ:
                    case ByteCommand.JZ:
                    case ByteCommand.STORE:
                        command = new OffsetCommand();
                        ((OffsetCommand)command).offset = GetOffset();
                        break;
                    case ByteCommand.PUSH:
                        command = new VarCommand();
                        ((VarCommand)command).var = GetVar();
                        break;
                    default:
                        throw bcc;
                }

                command.command = byteCommand;
                program.Add(command);
            }

            return program.ToArray();
        }
    }
}
