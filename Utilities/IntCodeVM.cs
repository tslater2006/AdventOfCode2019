﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AdventOfCode.Utilities
{
    class IntCodeInstruction
    {
        private IntCodeVM VM;
        public IntCodeOp OpCode;
        public long[] Parameters;
        public IntCodeMode[] Modes;
        public int Length;

        public IntCodeInstruction(IntCodeVM vmInst)
        {
            VM = vmInst;
        }

        public long GetParam(int paramNum)
        {
            if (Modes[paramNum - 1] == IntCodeMode.POSITION)
            {
                return VM.ReadMemory(Parameters[paramNum - 1]);
            } else if (Modes[paramNum - 1] == IntCodeMode.IMMEDIATE)
            {
                return Parameters[paramNum - 1];
            } else if (Modes[paramNum -1] == IntCodeMode.RELATIVE)
            {
                return VM.ReadMemory(VM.RelativeBase + Parameters[paramNum - 1]);
            }

            throw new FormatException();
        }

        public long GetWriteAddress(int paramNum)
        {
            if (Modes[paramNum -1] == IntCodeMode.POSITION)
            {
                return Parameters[paramNum - 1];
            } else if (Modes[paramNum -1] == IntCodeMode.RELATIVE)
            {
                return VM.RelativeBase + Parameters[paramNum - 1];
            }

            throw new FormatException();
        }

    }

    enum IntCodeOp : int
    {
        ADD = 1, MULT = 2, INPUT = 3, OUTPUT = 4, JUMP_TRUE = 5, JUMP_FALSE = 6, LT = 7, EQUALS = 8, ADJUST_REL_BASE = 9, HALT = 99
    }

    enum IntCodeMode : int
    {
        POSITION = 0, IMMEDIATE = 1, RELATIVE = 2
    }

    enum HaltType
    {
        HALT_TERMINATE, HALT_WAITING
    }

    class IntCodeVM
    {
        long[] program;
        public long[] memory = null;
        Queue<long> Inputs = new Queue<long>();
        Queue<long> Outputs = new Queue<long>();
        public long RelativeBase = 0;
        long IP;
        public IntCodeVM(string prog)
        {
            program = prog.Split(',').Select(s => long.Parse(s)).ToArray();
            memory = new long[program.Length];
            program.CopyTo(memory, 0);
            IP = 0;
        }
        public void Reset()
        {
            if (memory.Length == program.Length)
            {
                Array.Clear(memory, 0, memory.Length);
            } else
            {
                memory = new long[program.Length];
            }

            program.CopyTo(memory, 0);
            IP = 0;
            Inputs.Clear();
            Outputs.Clear();
            RelativeBase = 0;
        }
        public void WriteInput(long input)
        {
            Inputs.Enqueue(input);
        }

        public Queue<long> ReadOutputs()
        {
            return Outputs;
        }

        public long PopOutput()
        {
            return Outputs.Dequeue();
        }

        private IntCodeInstruction ParseOpCode()
        {
            IntCodeInstruction instr = new IntCodeInstruction(this);
            long tempOpCode = memory[IP];
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
                case IntCodeOp.ADJUST_REL_BASE:
                    paramCount = 1;
                    break;
            }

            instr.Length = paramCount + 1;

            instr.Parameters = new long[paramCount];
            instr.Modes = new IntCodeMode[paramCount];

            for (var x = 0; x < paramCount; x++)
            {
                instr.Parameters[x] = memory[IP + (x + 1)];
            }

            /* parse the bitmask for addressing types */
            var accessMask = tempOpCode / 100;

            for (var x = 0; x < paramCount; x++)
            {
                instr.Modes[x] = (IntCodeMode)(accessMask % 10);
                accessMask /= 10;
            }

            return instr;
        }

        public HaltType RunProgram()
        {
            while (true)
            {
                IntCodeInstruction instr = ParseOpCode();
                bool IPModified = false;
                switch (instr.OpCode)
                {
                    case IntCodeOp.HALT:
                        return HaltType.HALT_TERMINATE;
                    case IntCodeOp.ADD:
                        WriteMemory(instr.GetWriteAddress(3), instr.GetParam(1) + instr.GetParam(2));
                        break;
                    case IntCodeOp.MULT:
                        WriteMemory(instr.GetWriteAddress(3), instr.GetParam(1) * instr.GetParam(2));
                        break;
                    case IntCodeOp.INPUT:
                        if (Inputs.Count == 0)
                        {
                            /* no inputs, lets wait... */
                            return HaltType.HALT_WAITING;
                        }
                        
                        WriteMemory(instr.GetWriteAddress(1), Inputs.Dequeue());

                        break;
                    case IntCodeOp.OUTPUT:
                        Outputs.Enqueue(instr.GetParam(1));
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
                            WriteMemory(instr.GetWriteAddress(3), 1);
                        } else
                        {
                            WriteMemory(instr.GetWriteAddress(3), 0);
                        }
                        break;
                    case IntCodeOp.EQUALS:
                        if (instr.GetParam(1) == instr.GetParam(2))
                        {
                            WriteMemory(instr.GetWriteAddress(3), 1);
                        } else
                        {
                            WriteMemory(instr.GetWriteAddress(3), 0);
                        }
                        break;
                    case IntCodeOp.ADJUST_REL_BASE:
                        RelativeBase += instr.GetParam(1);
                        break;
                }
                if (IPModified == false)
                {
                    IP += instr.Length;
                }
                IPModified = false;
            }
        }
        private void ResizeMemory(long size)
        {
            var newMem = new long[size];
            memory.CopyTo(newMem, 0);
            memory = newMem;
        }
        public long ReadMemory(long x)
        {
            if (x > memory.Length-1)
            {
                ResizeMemory(x + 1);
            }
            return memory[x];
        }

        public void WriteMemory(long x, long value)
        {
            if (x > memory.Length -1)
            {
                ResizeMemory(x + 1);
            }
            if (x >= 0)
            {
                memory[x] = value;
            }
        }
    }
}
