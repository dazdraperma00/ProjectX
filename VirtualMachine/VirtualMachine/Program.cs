using System;
using System.IO;
using PSCompiler;

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

                    VirtualMachine vm = new VirtualMachine(code, (uint)code.Length);
                    vm.Run();

                    Variant[] stackCont = vm.GetStackContent();
                    for (int i = 0; i < stackCont.Length; ++i)
                    {
                        Console.Write(stackCont[i].ToString() + " ");
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
