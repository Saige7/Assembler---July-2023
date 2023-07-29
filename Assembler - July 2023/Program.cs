using System;
using System.Collections.Generic;

namespace Assembler___July_2023
{
    class Program
    {
        enum OpCode : byte
        {
            ADD = 0x10,
        };
        static Dictionary<string, byte> OpCodes = new Dictionary<string, byte>()
        {
            ["NOP"] = 0x00,

            ["ADD"] = 0x10,
            ["SUB"] = 0x11,
            ["MUL"] = 0x12,
            ["DIV"] = 0x13,
            ["MOD"] = 0x14,

            ["NOT"] = 0x20,
            ["AND"] = 0x21,
            ["OR"] = 0x22,
            ["XOR"] = 0x23,
            ["EQ"] = 0x24,
            ["NEQ"] = 0x25,
            ["GTE"] = 0x26,
            ["LTE"] = 0x27,
            ["GT"] = 0x28,
            ["LT"] = 0x29,

            ["JMP"] = 0x30,
            ["JMPi"] = 0x31,
            ["JMPT"] = 0x32,
            ["JMPTi"] = 0x33,

            ["SET"] = 0x40,
            ["COPY"] = 0x41,
            ["LOAD"] = 0x42,
            ["LOADi"] = 0x43,
            ["STR"] = 0x44,
            ["STRi"] = 0x45,
            ["PUSH"] = 0x46,
            ["POP"] = 0x47
        };
        static Dictionary<string, byte> Registers = new Dictionary<string, byte>()
        {
            ["R0"] = 0x00,
            ["R1"] = 0x01,
            ["R2"] = 0x02,
            ["R3"] = 0x03,
            ["R4"] = 0x04,
            ["R5"] = 0x05,
            ["R6"] = 0x06,
            ["R7"] = 0x07
        };

        static byte[] AssemblyToByte(string line, Dictionary<string, int> labelsLines)
        {
            string[] partsOfLine = line.Split(" ");
            byte pad = 0x00;

            byte opCode = OpCodes[partsOfLine[0]];
            switch (opCode)
            {
                case 0x00:
                    return new byte[] { opCode, pad, pad, pad };

                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                case 0x14:
                case 0x21:
                case 0x22:
                case 0x23:
                case 0x24:
                case 0x25:
                case 0x26:
                case 0x27:
                case 0x28:
                case 0x29:
                    return new byte[] { opCode, Registers[partsOfLine[1]], Registers[partsOfLine[2]], Registers[partsOfLine[3]] };

                case 0x30:
                    if(labelsLines.ContainsKey(partsOfLine[1]))
                    {
                        ushort lineNum = (ushort)labelsLines[partsOfLine[1]];
                        return new byte[] { opCode, (byte)(lineNum >> 8), (byte)lineNum, pad};
                    }
                    var addr = ushort.Parse(partsOfLine[1]);
                    byte addrLowByte = (byte)addr;
                    byte addrHighByte = (byte)(addr >> 8);
                    return new byte[] { opCode, addrHighByte, addrLowByte, pad };
                case 0x32:
                case 0x44:
                    if(labelsLines.ContainsKey(partsOfLine[1]))
                    {
                        ushort lineNum = (ushort)labelsLines[partsOfLine[1]];
                        return new byte[] { opCode, (byte)(lineNum >> 8), (byte)lineNum, Registers[partsOfLine[2]]};
                    }

                    var address = ushort.Parse(partsOfLine[1]);
                    byte addressLowByte = (byte)address;
                    byte addressHighByte = (byte)(address >> 8);
                    return new byte[] { opCode, addressHighByte, addressLowByte, Registers[partsOfLine[3]] };

                case 0x20:
                case 0x33:
                case 0x41:
                case 0x43:
                case 0x45:
                    return new byte[] { opCode, Registers[partsOfLine[1]], Registers[partsOfLine[2]], pad };
                
                case 0x40:
                case 0X42:
                    var value = ushort.Parse(partsOfLine[2]);
                    byte valueLowByte = (byte)value;
                    byte valueHighByte = (byte)(value >> 8);
                    return new byte[] { opCode, Registers[partsOfLine[1]], valueHighByte, valueLowByte };

                case 0x31:
                case 0x46:
                case 0x47:
                    return new byte[] { opCode, Registers[partsOfLine[1]], pad, pad };

                default: throw new Exception("Invalid assembly :(");
            }
        }

        static void Main(string[] args)
        {
            string[] assemblyLines = System.IO.File.ReadAllLines(args[0]);
            List<byte> assembledBytes = new List<byte>();

            List<string> assembly = new List<string>();
            Dictionary<string, int> labelsLines = new Dictionary<string, int>();

            for (int i = 0; i < assemblyLines.Length; i++)
            {
                if (assemblyLines[i].Contains(':'))
                {
                    string label = assemblyLines[i];
                    label = label.Remove(label.Length - 1);
                    labelsLines.Add(label, i - labelsLines.Count);
                    continue;
                }
                assembly.Add(assemblyLines[i]);
            }

            foreach (var line in assembly)
            {
                Console.WriteLine(line);
                var bytes = AssemblyToByte(line, labelsLines);
                foreach (var aByte in bytes)
                {
                    Console.Write($"   0x{aByte:X2}");
                }

                assembledBytes.AddRange(bytes);
                Console.WriteLine();
            }

            System.IO.File.WriteAllBytes(@"C:\Users\saige.kumar\Downloads\BinaryTest.bin", assembledBytes.ToArray());
        }
    }
}