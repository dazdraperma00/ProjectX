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

                    VirtualMachine vm = new VirtualMachine();

                    DateTime tStart = DateTime.Now;

                    bool bSuccess = vm.Run(code);

                    DateTime tEnd = DateTime.Now;

                    Console.WriteLine((tEnd - tStart).Milliseconds);

                    if (bSuccess)
                    {
                        Console.WriteLine("succesfull");
                    }
                    else
                    {
                        Console.WriteLine("broken");
                    }

                    Variant[] stack = vm.GetStack();
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
