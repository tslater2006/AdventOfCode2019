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

------
[Write-up Table of Contents](../../../README.md)