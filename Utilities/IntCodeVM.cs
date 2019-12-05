using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Utilities
{
    class IntCodeInstruction
    {
        public IntCodeOp OpCode;
        public int[] Parameters;
        public IntCodeMode[] ParameterModes;
        public int Length;
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
        int[] memory;
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
            IntCodeInstruction instr = new IntCodeInstruction();
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
            instr.ParameterModes = new IntCodeMode[paramCount];

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
                        instr.ParameterModes[x] = IntCodeMode.POSITION;
                        break;
                    case 1:
                        instr.ParameterModes[x] = IntCodeMode.IMMEDIATE;
                        break;
                }
                accessMask /= 10;
            }

            return instr;
        }

        public int ResolveParam(IntCodeInstruction instr, int paramNum)
        {
            switch (instr.ParameterModes[paramNum - 1])
            {
                case IntCodeMode.IMMEDIATE:
                    return instr.Parameters[paramNum - 1];
                case IntCodeMode.POSITION:
                    return memory[instr.Parameters[paramNum - 1]];
            }
            return 0;
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
                        SetMemory(instr.Parameters[2], ResolveParam(instr, 1) + ResolveParam(instr, 2));
                        break;
                    case IntCodeOp.MULT:
                        SetMemory(instr.Parameters[2], ResolveParam(instr, 1) * ResolveParam(instr, 2));
                        break;
                    case IntCodeOp.INPUT:
                        SetMemory(instr.Parameters[0], Inputs.Dequeue());
                        break;
                    case IntCodeOp.OUTPUT:
                        Outputs.Add(ResolveParam(instr, 1));
                        break;
                    case IntCodeOp.JUMP_TRUE:
                        if (ResolveParam(instr,1) != 0)
                        {
                            IP = ResolveParam(instr, 2);
                            IPModified = true;
                        }
                        break;
                    case IntCodeOp.JUMP_FALSE:
                        if (ResolveParam(instr, 1) == 0)
                        {
                            IP = ResolveParam(instr, 2);
                            IPModified = true;
                        }
                        break;
                    case IntCodeOp.LT:
                        if (ResolveParam(instr,1) < ResolveParam(instr,2))
                        {
                            SetMemory(instr.Parameters[2], 1);
                        } else
                        {
                            SetMemory(instr.Parameters[2], 0);
                        }
                        break;
                    case IntCodeOp.EQUALS:
                        if (ResolveParam(instr, 1) == ResolveParam(instr, 2))
                        {
                            SetMemory(instr.Parameters[2], 1);
                        } else
                        {
                            SetMemory(instr.Parameters[2], 0);
                        }
                        break;
                }
                if (!IPModified)
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

        public void SetMemory(int x, int value)
        {
            memory[x] = value;
        }
    }
}
