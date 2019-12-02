using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Utilities
{
    class IntCodeVM
    {
        int[] memory;
        int IP;
        public IntCodeVM (string prog)
        {
            memory = prog.Split(',').Select(s => int.Parse(s)).ToArray();
            IP = 0;
        }

        public void RunProgram()
        {
            while (memory[IP] != 99)
            {
                switch(memory[IP])
                {
                    case 1:
                        memory[memory[IP + 3]] = memory[memory[IP + 1]] + memory[memory[IP + 2]];
                        break;
                    case 2:
                        memory[memory[IP + 3]] = memory[memory[IP + 1]] * memory[memory[IP + 2]];
                        break;
                }
                IP += 4;
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
