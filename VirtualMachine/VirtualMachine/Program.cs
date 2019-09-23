using System;
using System.IO;
using System.Collections.Generic;

namespace VirtualMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new Exception("exprcted file name");
            }

            string ext = Path.GetExtension(args[0]);
            if (ext != ".bpsc")
            {
                throw new Exception("wrong file extension");
            }

            try
            {
                using (FileStream fstream = new FileStream(args[0], FileMode.Open))
                {
                    byte[] code = new byte[fstream.Length];
                    fstream.Read(code, 0, code.Length);

                    Decoder decoder = new Decoder();
                    decoder.LoadCode(code);
                    Command[] program = decoder.Decode();

                    VirtualMachine vm = new VirtualMachine();
                    vm.LoadProgram(program);
                    vm.Run();

                    Variant[] stack = vm.GetStack().ToArray();
                    for (int i = stack.Length - 1; i >= 0; --i)
                    {
                        Console.WriteLine(stack[i]);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
