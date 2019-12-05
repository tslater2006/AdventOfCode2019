# Day 3 - Crossed Wires

Today's problem is the first 2D graph problem of the year. The premise of the problem is that we have 2 wires in our ship that are intersecting. We don't know the exact locations of the wires but we have a list of the directions the wires take. Both parts deal with specific intersections between the 2 wires.

Both wires begin at the same origin, wire self-intersections are not considered for this problem, nor is the intersection at the origin point.

[Full Problem Description](https://adventofcode.com/2019/day/3)

## Part 1

For this part, we want to know which intersection is closest to the origin point, calculated by the [Manhattan Distance](https://en.wikipedia.org/wiki/Taxicab_geometry) from the intersection point to the origin.

First up we need to parse the input, the input is given to us as 2 lines (each line is a wire) and the line consists of directions like `R10,U40,L2,D4` where U/D/L/R indicate the direction of movement, and the number is the units to move in the specified direction.

There are a variety of ways you can model this information in order to solve the problem. For this solution we are going to build out a list of every coordinate the wire passes through. So assuming that the origin is `(0,0)` and the first instruction is R4, we will add to our list of points:

- `(1,0)`
- `(2,0)`
- `(3,0)`
- `(4,0)`

Since we want to do this for both wires, lets build out a `ParseWire` method:

```c#
internal List<Point> ParseWire (string path) {
    /* break the string into the individual instructions */
    var directions = path.Split(',');
    
    /* prep return object */
    List<Point> newWire = new List<Point>();

    /* wires start at the origin 0,0. Note: the origin is chosen as 0,0 for ease of Manhattan distance calcs */
    Point currentPoint = new Point(0, 0);

    /* process each direction, and expand to the correct series of points */
    foreach (var direction in directions)
    {
        var amount = int.Parse(direction.Substring(1));
        
        /* default the offsets each time */
        int xOffset = 0;
        int yOffset = 0;

        /* based on the direction starting letter, set the offsets */
        switch (direction[0])
        {
            case 'U':
                yOffset = -1;
                break;
            case 'D':
                yOffset = 1;
                break;
            case 'L':
                xOffset = -1;
                break;
            case 'R':
                xOffset = 1;
                break;
        }

        /* add the right number of points in the proper direction */
        for (var x = 0; x < amount; x++)
        {
            currentPoint = new Point(currentPoint.X + xOffset, currentPoint.Y + yOffset);
            newWire.Add(currentPoint);
        }
    }

    return newWire;
}
```

Once we have this method, we can parse both of our wires. Since the wires will likely be used in both parts, we'll parse them in the constructor:

```c#
class Day03 : ASolution {
    List<Point> Wire1 = null;
    List<Point> Wire2 = null;
    public Day03() : base(3, 2019, "Crossed Wires")
    {
        Wire1 = ParseWire(Input[0]);
        Wire2 = ParseWire(Input[1]);
    }
```

Now recall that we want to find intersection points, luckily LINQ provides a handy mechanism for this:

```c#
var intersects = Wire1.Intersect(Wire2);
```

Now that we have our list of points that intersect, we just need to find the one with the smallest Manhattan distance. Again, LINQ has stuff to make this easy.

```c#
var closest = intersects.Select(p => Math.Abs(p.X) + Math.Abs(p.Y)).Min();
```

This line does a couple of things, we leverage Select() to return a list of the Manhattan distances, based on the list of points, and then we get the minimum value.

Note: The Manhattan distance is just the sum of the absolute values of X and Y of the point because we chose an origin of `(0,0)`. Had we chosen any other point as the origin we'd need to offset that in the formula like this:

```c#
var closest = intersects.Select(p => Math.Abs(p.X - originX) + Math.Abs(p.Y - originY)).Min();
```

We now have the closest distance, and have completed Part 1.

## Part 2

This part is very similar to Part 1, the only difference is that instead of the minimum in terms of the Manhattan distance, we now want the point that has the minimum **combined** walking distance. That is, we need to take the distance you would have to walk on Wire #1 and the distance you would walk on Wire #2, combine them, and then choose the intersection that minimizes this value.

Because of how we generate the list of points in `ParseWire()`, we happen to already know the distance for a given point. Because the points are added to the list **in order** their index in our list happens to equal the walking distance.

Note: because we don't add the origin point to our lists, we need to account for the first point being at index `0` but having a step distance of `1`.

We will attack this the same way as Part 1, get intersections, calculate a value, and grab the smallest.

We start off getting the intersection points again:

```c#
var intersects = Wire1.Intersect(Wire2);
```
But this time our formula is a little different:

```c#
/* the + 1's are to offset the fact that the 0-index list starts with the first step length of 1 */
var closestWalk = intersects.Select(p => (Wire1.IndexOf(p) + 1) + (Wire2.IndexOf(p) + 1)).Min();
```

This is the answer to Part 2 and we're done for the day. Time for some hot chocolate!

## Adendum 

I've since revisited this solution and switch from using a List<Point> to a Dictionary<Point,int> to store the walk distance for Part 2, this has much better lookup performance and is purely for improving the execution speed. Everything above still applies in terms of the algorithms employed.

------
[Write-up Table of Contents](../../../README.md)
