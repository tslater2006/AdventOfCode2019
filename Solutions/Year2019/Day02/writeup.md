# Day 2 - 

In this problem we get introduced to what I'm sure will be a reoccuring item in the coming days, the Intcode computer. 

In past years the "VM" portion has shown up a bit later, with a lot more features up front. This time around it comes earlier but with less pieces, which I'm sure we will add to over the coming days.

Please be sure to read the problem description for a full overview of the Intcode machine.

[Full Problem Description](https://adventofcode.com/2019/day/2)

## Part 1

Because I assume we'll be seeing Intcode again, we will be implementing it as a VM, and then for Day 2 we will simply leverage the VM to get the answers.

A pretty basic VM contains the following:

- Something to hold "instructions" (opcodes)
- Memory to operate on
- Instruction Pointer
- Some sort of run loop to execute the instructions

We'll start off by creating an IntCodeVM class in the Utilities package, since this is sure to be reused later. And we'll setup a basic skeleton:

```c#
class IntCodeVM
{
    int[] memory;
    int IP;
    public IntCodeVM (string prog)
    {
        /* Intcode programs are comma separated numbers, parse these into memory array */
        memory = prog.Split(',').Select(s => int.Parse(s)).ToArray();

        /* VM always starts with instruction pointer at 0 */
        IP = 0;
    }
    public void RunProgram()
    {
        /* TODO */
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
```

At this point we have the ability to load an Intcode program into an array, and basic read/write operations into that array (the VMs memory).

Per the Intcode specification for Day2, there are 3 opcodes we need to handle, `1` (Addition) `2` (Multiplication) and `99` (Halt)

Once the VM starts execution it continues until it sees a Halt operation. To simplify the implementation we can assume that we will only be given well forms Intcode programs, so we don't need to handle invalid opcodes. 

We are also told that to go to the next instruction, we simply add 4 to the Instruction Pointer (this is because both opcodes `1` and `2` are 4 numbers long), `99` is 1 number long but it halts the VM so no need to increment after that.

Lets start with a while loop to continue the execution and a switch statement to handle the other 2 opcodes:

```c#
public void RunProgram()
{
    while (memory[IP] != 99)
    {
        switch(memory[IP])
        {
            case 1:
                break;
            case 2:
                break;
        }
        IP += 4;
    }
}
```

Now we have the skeleton for handling each opcode we need to look at what the pieces of an opcode are. According to the problem the opcode format is `operation, operand1, operand2, destination`

The key part to notice is that the `operand1` `operand2` and `destination` are **indexes** into memory where we should read or write from.

For the time being we will not be parsing the intcode into instruction objects, so we will just reference the parts via relative offsets from the current Instruction Pointer, so `operation` would be `memory[IP]` and `operand1` would be `memory[IP + 1]` etc.

With this information the implementation of Opcode `1` would be something like the following psuedocode:

```c#
newValue = ReadMemory(operand1) + ReadMemory(operand2);
WriteMemory(operand3, newValue);
```

However since we are inside the VM class, accessing the memory array directly is more efficient than additional method calls, so we convert the above into:

```c#
memory[memory[IP + 3]] = memory[memory[IP + 1]] + memory[memory[IP + 2]];
```

Operand `2` is similar, just with multiplication

```c#
memory[memory[IP + 3]] = memory[memory[IP + 1]] * memory[memory[IP + 2]];
```

So now our "RunProgram" method looks like this:

```c#
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
```

And with that we have a fully functioning VM! Now its time to leverage it for solving Part 1 :)

The problem states that before running the Intcode program, we should first put it in the `1202` state, this can be achieved by writing to memory address 1 and 2. 

First lets create a VM instance and load our intcode program. Since this will likely be used for Part 2 as well, we'll make it a class instance variable:

```c#
class Day02 : ASolution {
    IntCodeVM vm = null;
    public Day02() : base(2, 2019, "") {
        vm = new IntCodeVM(Input[0]);
    }
```

Next up is to set the `1202` error state, we can do this with our WriteMemory method:

```c#
/* set the IntCode computer to error state 1202 */
vm.SetMemory(1, 12);
vm.SetMemory(2, 02);
```

Then we run the program with 
```c#
vm.RunProgram()
```

 After it halts we read out the value at memory address 0 with:

```c#
/* read out memory 0 */
int result = vm.ReadMemory(0);
```

And there we have our result!

Full Part 1 solution:

```c#
protected override string SolvePartOne() {

    /* VM is already loaded with program via the constructor */

    /* set the IntCode computer to error state 1202 */
    vm.SetMemory(1, 12);
    vm.SetMemory(2, 02);

    /* run the program */
    vm.RunProgram();

    /* read out memory 0 */
    int result = vm.ReadMemory(0);

    return result.ToString(); 
}
```

## Part 2

The second part of the challenge is much like the first half, just the question is inverted. 

For Part 1 it was "given this input, 1202, what is the output" and now we have the question of "what input gives you the output `19690720`"

Recall that we set an input value by writing to memory address 1 and 2, the problem goes on to explain that the value you write to memory address 1 is the "noun" and memory address 2 is the "verb", and it also assures us that the values can be between 0 and 99 (inclusive)

The easiest way to solve this problem is just to run the VM with the different combinations until one of them yields us the correct final answer, at that point we know the noun and verb values, and the answer is `100 * noun + verb`

First things first, we need to correct some things in our VM, and add a way to reset the machine state. The [current state of the VM](https://github.com/tslater2006/AdventOfCode2019/blob/19840a9c7ff81999f9795d3264bd0005e68a0f00/Utilities/IntCodeVM.cs) conflates 2 things, the program that is running, and the memory. 

Because the programs can modify the memory, resetting isn't as simple as just setting the Instruction Pointer back to 0, some spots in the program that got written to are now different.

To solve this we will separate out the program (opcodes) and the memory (used during the running of the VM)

We need to:

- Store the program in a separate array
- Adjust constructor to parse the program into this new array
- Make the memory array the same size as the program
- Copy the new arrays values into "memory"

```c#
class IntCodeVM
{
    int[] program;
    int[] memory;
    int IP;
    public IntCodeVM (string prog)
    {
        program = prog.Split(',').Select(s => int.Parse(s)).ToArray();
        memory = new int[program.Length];
        program.CopyTo(memory, 0);
        IP = 0;
    }
```

At this point the VM will run exactly as before, but also retain a copy of the original program, this allows us to create a "Reset" method. All this reset method needs to do is re-copy the program into memory and reset the Instruction Pointer

```c#
public void Reset()
{
    program.CopyTo(memory, 0);
    IP = 0;
}
```

We now have everyting we need implemented to solve Part 2. 

The easiest way to solve this is to just go through all of the combinations via nested for loops, we try each Noun-Verb combination until we find the correct output, at that point we break out of the loops and return the result:
```c#
protected override string SolvePartTwo() {

    bool answerFound = false;
    int noun = 0;
    int verb = 0;

    for (var x = 0; x <= 99; x++)
    {
        for (var y = 0; y <= 99; y++)
        {
            vm.Reset();
            vm.SetMemory(1, x);
            vm.SetMemory(2, y);

            vm.RunProgram();

            if (vm.ReadMemory(0) == 19690720)
            {
                answerFound = true;
            }
            if (answerFound)
            {
                verb = y;
                break;
            }
        }
        if (answerFound) {
            noun = x;
            break;
        }
    }

    return (100 * noun + verb).ToString(); 
}
```

Here is the final [Intcode VM For Day 2](https://github.com/tslater2006/AdventOfCode2019/blob/d1f6a37b391c9b970f70856d6a396e515046c8b3/Utilities/IntCodeVM.cs).

------
[Write-up Table of Contents](../../../README.md)