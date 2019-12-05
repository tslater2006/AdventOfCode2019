using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AdventOfCode.Utilities
{
    class IntCodeInstruction
    {
        private IntCodeVM VM;
        public IntCodeOp OpCode;
        public int[] Parameters;
        public IntCodeMode[] Modes;
        public int Length;

        public IntCodeInstruction(IntCodeVM vmInst)
        {
            VM = vmInst;
        }

        public int GetParam(int paramNum)
        {
            if (Modes[paramNum - 1] == IntCodeMode.POSITION)
            {
                return VM.memory[Parameters[paramNum - 1]];
            } else if (Modes[paramNum - 1] == IntCodeMode.IMMEDIATE)
            {
                return Parameters[paramNum - 1];
            }

            throw new FormatException();
        }

    }

    enum IntCodeOp : int
    {
        ADD = 1, MULT = 2, INPUT = 3, OUTPUT = 4, JUMP_TRUE = 5, JUMP_FALSE = 6, LT = 7, EQUALS = 8, HALT = 99
    }

    enum IntCodeMode : int
    {
        POSITION = 0, IMMEDIATE = 1
    }


    class IntCodeVM
    {
        int[] program;
        public int[] memory;
        Queue<int> Inputs = new Queue<int>();
        List<int> Outputs = new List<int>();

        int IP;
        public IntCodeVM(string prog)
        {
            program = prog.Split(',').Select(s => int.Parse(s)).ToArray();
            memory = new int[program.Length];
            program.CopyTo(memory, 0);
            IP = 0;
        }
        public void Reset()
        {
            program.CopyTo(memory, 0);
            IP = 0;
            Inputs.Clear();
            Outputs.Clear();
        }
        public void WriteInput(int input)
        {
            Inputs.Enqueue(input);
        }

        public List<int> ReadOutputs()
        {
            return Outputs;
        }

        private IntCodeInstruction ParseOpCode()
        {
            IntCodeInstruction instr = new IntCodeInstruction(this);
            int tempOpCode = memory[IP];
            instr.OpCode = (IntCodeOp)(tempOpCode % 100);

            int paramCount = 0;
            switch (instr.OpCode)
            {
                case IntCodeOp.ADD:
                    paramCount = 3;
                    break;
                case IntCodeOp.MULT:
                    paramCount = 3;
                    break;
                case IntCodeOp.INPUT:
                    paramCount = 1;
                    break;
                case IntCodeOp.OUTPUT:
                    paramCount = 1;
                    break;
                case IntCodeOp.JUMP_TRUE:
                    paramCount = 2;
                    break;
                case IntCodeOp.JUMP_FALSE:
                    paramCount = 2;
                    break;
                case IntCodeOp.LT:
                    paramCount = 3;
                    break;
                case IntCodeOp.EQUALS:
                    paramCount = 3;
                    break;
            }

            instr.Length = paramCount + 1;

            instr.Parameters = new int[paramCount];
            instr.Modes = new IntCodeMode[paramCount];

            for (var x = 0; x < paramCount; x++)
            {
                instr.Parameters[x] = memory[IP + (x + 1)];
            }

            /* parse the bitmask for addressing types */
            var accessMask = tempOpCode / 100;

            for (var x = 0; x < paramCount; x++)
            {
                switch (accessMask % 10)
                {
                    case 0:
                        instr.Modes[x] = IntCodeMode.POSITION;
                        break;
                    case 1:
                        instr.Modes[x] = IntCodeMode.IMMEDIATE;
                        break;
                }
                accessMask /= 10;
            }

            return instr;
        }

        public void RunProgram()
        {
            while (true)
            {
                IntCodeInstruction instr = ParseOpCode();
                bool IPModified = false;
                switch (instr.OpCode)
                {
                    case IntCodeOp.HALT:
                        return;
                    case IntCodeOp.ADD:
                        WriteMemory(instr.Parameters[2], instr.GetParam(1) + instr.GetParam(2));
                        break;
                    case IntCodeOp.MULT:
                        WriteMemory(instr.Parameters[2], instr.GetParam(1) * instr.GetParam(2));
                        break;
                    case IntCodeOp.INPUT:
                        WriteMemory(instr.Parameters[0], Inputs.Dequeue());
                        break;
                    case IntCodeOp.OUTPUT:
                        Outputs.Add(instr.GetParam(1));
                        break;
                    case IntCodeOp.JUMP_TRUE:
                        if (instr.GetParam(1) != 0)
                        {
                            IP = instr.GetParam(2);
                            IPModified = true;
                        }
                        break;
                    case IntCodeOp.JUMP_FALSE:
                        if (instr.GetParam(1) == 0)
                        {
                            IP = instr.GetParam(2);
                            IPModified = true;
                        }
                        break;
                    case IntCodeOp.LT:
                        if (instr.GetParam(1) < instr.GetParam(2))
                        {
                            WriteMemory(instr.Parameters[2], 1);
                        } else
                        {
                            WriteMemory(instr.Parameters[2], 0);
                        }
                        break;
                    case IntCodeOp.EQUALS:
                        if (instr.GetParam(1) == instr.GetParam(2))
                        {
                            WriteMemory(instr.Parameters[2], 1);
                        } else
                        {
                            WriteMemory(instr.Parameters[2], 0);
                        }
                        break;
                }
                if (IPModified == false)
                {
                    IP += instr.Length;
                }
                IPModified = false;
            }
        }
    
        public int ReadMemory(int x)
        {
            return memory[x];
        }

        public void WriteMemory(int x, int value)
        {
            memory[x] = value;
        }
    }
}
