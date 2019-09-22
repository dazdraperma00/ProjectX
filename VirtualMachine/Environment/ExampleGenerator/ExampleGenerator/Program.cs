﻿using System;
using System.IO;
using System.Collections.Generic;
using VirtualMachine;

namespace ExampleGenerator
{
    class Program
    {
        private static List<byte> byteCode = new List<byte>();

        private static void AddOperand(string op)
        {
            Variant var = Variant.Parse(op);
            byte[] bytes = var.ToBytes();

            for (int i = 0; i < bytes.Length; ++i)
            {
                byteCode.Add(bytes[i]);
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new Exception("expected file name");
            }

            string[] separators = { " ", "\n", "\r", "\t" };
            string[] words = File.ReadAllText(args[0]).Split(separators, StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < words.Length;)
            {
                switch(words[i++])
                {
                    case "FETCH":
                        byteCode.Add((byte)ByteCommand.FETCH);
                        AddOperand(words[i++]);
                        break;
                    case "STORE":
                        byteCode.Add((byte)ByteCommand.STORE);
                        AddOperand(words[i++]);
                        break;
                    case "PUSH":
                        byteCode.Add((byte)ByteCommand.PUSH);
                        AddOperand(words[i++]);
                        break;
                    case "POP":
                        byteCode.Add((byte)ByteCommand.POP);
                        break;
                    case "ADD":
                        byteCode.Add((byte)ByteCommand.ADD);
                        break;
                    case "SUB":
                        byteCode.Add((byte)ByteCommand.SUB);
                        break;
                    case "LT":
                        byteCode.Add((byte)ByteCommand.LT);
                        break;
                    case "GT":
                        byteCode.Add((byte)ByteCommand.GT);
                        break;
                    case "LET":
                        byteCode.Add((byte)ByteCommand.LET);
                        break;
                    case "GET":
                        byteCode.Add((byte)ByteCommand.GET);
                        break;
                    case "EQ":
                        byteCode.Add((byte)ByteCommand.EQ);
                        break;
                    case "NEQ":
                        byteCode.Add((byte)ByteCommand.NEQ);
                        break;
                    case "JZ":
                        byteCode.Add((byte)ByteCommand.JZ);
                        AddOperand(words[i++]);
                        break;
                    case "JNZ":
                        byteCode.Add((byte)ByteCommand.JNZ);
                        AddOperand(words[i++]);
                        break;
                    case "JMP":
                        byteCode.Add((byte)ByteCommand.JMP);
                        AddOperand(words[i++]);
                        break;
                    case "HALT":
                        byteCode.Add((byte)ByteCommand.HALT);
                        break;
                    default:
                        throw new Exception("unknown command");
                }
            }

            using (FileStream fstream = new FileStream("../../../../Examples/" +
                   Path.GetFileNameWithoutExtension(args[0]) + ".bpsc", FileMode.Create))
            {
                fstream.Write(byteCode.ToArray(), 0, byteCode.Count);
            }
        }
    }
}
