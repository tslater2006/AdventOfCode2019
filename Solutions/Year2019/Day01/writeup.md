# Day 1 - The Tyranny of the Rocket Equation

This year we start off with a relatively simple problem. We have a series of masses and we are concerned with the amount of fuel required by the rocket to lift the masses.

[Full Problem Description](https://adventofcode.com/2019/day/1)

## Part 1

The first part tells us that in order to calculate the required fuel for a given mass, we are to divide the mass by 3, round down, and then subtract 2.

First thing we need to do is parse the input file, the file contents are made available via the base class as the "Input" property, and it is an array of strings.

We can define an array of integers to hold the masses at the class level, and then convert from the strings to integers in the constructor (this is because the numbers will be required in both parts).

```c#
    class Day01 : ASolution {
        int[] masses = null;
        public Day01() : base(1, 2019, "") {
            masses = Input.Select(s => int.Parse(s)).ToArray();
        }
```

Next we can make a method that takes in a mass and returns the fuel requirement:

```c#
        private int GetFuelForMass(int m)
        {
            return (int)(Math.Floor(m / 3.0) - 2);
        }
```

The answer to the problem is the sum of the fuel requirements for each mass. This could be achieve with a for loop or a foreach loop, but again I prefer LINQ:

```c#
return masses.Select(m => GetFuelForMass(m)).Sum().ToString();
```

## Part 2

This part is similar to Part 1, however this is where the problems name comes into play. All of that fuel we need is also mass the rocket needs to lift, and thus requires fuel to lift the fuel to lift the mass.

There are a variety of ways to solve this and for the actual submission of my answer I went with a foreach/while loop and did the math inline.

I've since gone back through and made a more "proper" solution to the problem. This solution will piggy back on `GetFuelForMass` method from Part 1.

What we need to solve for here is the **recursive** fuel requirement for each mass. Lets make a method that does that:

```c#
        private int GetRecursiveFuelForMass(int m)
        {
            /* determine the main fuel requirement */
            var initialFuel = GetFuelForMass(m);

            /* if we need more fuel, we need to return that fuel + the fuel required to lift the fuel */
            if (initialFuel > 0)
            {
                return initialFuel + GetRecursiveFuelForMass(initialFuel);
            } else
            {
                /* this is the recursion escape clause, 
                 * if we're called with an initial fuel requirement of 0, just return 0 
                 */
                return 0;
            }
        }
```

Once we have that method, we can make the same LINQ call chain as we did in Part1, just with our recursive method instead

```c#
return masses.Select(m => GetRecursiveFuelForMass(m)).Sum().ToString(); 
```

------
[Write-up Table of Contents](../../../README.md)