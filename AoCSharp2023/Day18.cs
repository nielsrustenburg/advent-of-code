using Shared;
using System.Numerics;

namespace AoCSharp2023
{
    internal class Day18
    {
        public Day18()
        {
            string[] lines = File.ReadAllLines("./inputs/18.txt");
            Console.WriteLine(InstructionsToArea(lines));
            Console.WriteLine(InstructionsToArea(lines, true));
            //var grid = CreateGridFromInstructions(lines);
            //var grid = GridHelper.CreateGrid(lines);
            //GridHelper.DrawGrid(grid);
            //Console.WriteLine(Part1(grid));
            //Console.WriteLine(Part2(grid));
        }

        //public string Part1(char[,] grid)
        //{
        //    var aPlusOther = 0;
        //    var bPlusOther = 0;
        //    for (int y = 0; y < grid.GetLength(1); y++)
        //    {
        //        for (int x = 0; x < grid.GetLength(0); x++)
        //        {
        //            if (grid[x, y] == 'A')
        //            {
        //                aPlusOther++;
        //            }
        //            else if (grid[x, y] == 'B')
        //            {
        //                bPlusOther++;
        //            }
        //            else if (grid[x, y] != '.')
        //            {
        //                aPlusOther++;
        //                bPlusOther++;
        //            }
        //        }
        //    }

        //    return $"A: {aPlusOther} or B: {bPlusOther}";
        //}

        public BigInteger InstructionsToArea(string[] lines, bool part2 = false)
        {
            var instructions = lines.Select(line => new Instruction(line));
            (BigInteger X, BigInteger Y) current = (0, 0);
            var points = new List<(BigInteger X, BigInteger Y)>();
            foreach(var instruction in instructions)
            {
                if (!part2)
                {
                    current = instruction.Direction.StepInDirection(current.X, current.Y, instruction.Distance);
                } else
                {
                    current = instruction.HexDirection.StepInDirection(current.X, current.Y, instruction.HexDistance);
                }
                points.Add(current);
            }
            var shoelace = AoCMath.Shoelace(points);
            var boundaryPoints = part2 ? instructions.Sum(i => i.HexDistance) : instructions.Sum(i => i.Distance);
            var internalPoints = AoCMath.PicksFindI(shoelace, boundaryPoints);
            return internalPoints + boundaryPoints;
        }

        //public char[,] CreateGridFromInstructions(string[] lines)
        //{
        //    var marked = new Dictionary<(int X, int Y), Direction>(); // Add colour if necessary for p2
        //    var stepWisePath = new List<(int X, int Y)>();
        //    var instructions = lines.Select(line => new Instruction(line));
        //    var current = (X: 0, Y: 0);
        //    foreach (var instruction in instructions)
        //    {
        //        var steps = instruction.Distance;
        //        while (steps > 0)
        //        {
        //            marked.Add(current, instruction.Direction);
        //            stepWisePath.Add(current);
        //            current = instruction.Direction.StepInDirection(current.X, current.Y);
        //            steps--;
        //        }
        //    }
        //    //marked.Add(current, instructions.Last().Direction);
        //    //path.Add(current);

        //    // Shift to account for negative coordinate values
        //    var minX = marked.Keys.Min(c => c.X);
        //    var maxX = marked.Keys.Max(c => c.X) + 1;
        //    var minY = marked.Keys.Min(c => c.Y);
        //    var maxY = marked.Keys.Max(c => c.Y) + 1;
        //    var shiftX = 0;
        //    var shiftY = 0;
        //    if(minX < 0)
        //    {
        //        shiftX = -1 * minX;
        //    }
        //    if (minY < 0)
        //    {
        //        shiftY = -1 * minY;
        //    }

        //    marked = marked.ToDictionary(kvp => (X: kvp.Key.X + shiftX, Y: kvp.Key.Y + shiftY), kvp => kvp.Value);
        //    stepWisePath = stepWisePath.Select(c => (c.X + shiftX, c.Y + shiftY)).ToList();
        //    minX = 0;
        //    minY = 0;
        //    maxX = maxX + shiftX;
        //    maxY = maxY + shiftY;

        //    // Draw the grid
        //    var grid = new char[maxX, maxY];
        //    for (int x = 0; x < maxX; x++)
        //    {
        //        for (int y = 0; y < maxY; y++)
        //        {
        //            if (marked.TryGetValue((x, y), out var dir))
        //            {
        //                grid[x, y] = DirectionToArrow(dir);
        //            }
        //            else
        //            {
        //                grid[x, y] = '.';
        //            }
        //        }
        //    }

        //    GridHelper.DrawGrid(grid);

        //    var queue = new List<(int X, int Y)>();
        //    foreach (var step in stepWisePath)
        //    {
        //        (int X, int Y) sideA;
        //        (int X, int Y) sideB;

        //        sideA = ((Direction)(((int)marked[step] + 3) % 4)).StepInDirection(step.X, step.Y);
        //        sideB = ((Direction)(((int)marked[step] + 1) % 4)).StepInDirection(step.X, step.Y);

        //        if (GridHelper.IsWithinGrid(grid, sideA.X, sideA.Y) && grid[sideA.X, sideA.Y] == '.')
        //        {
        //            grid[sideA.X, sideA.Y] = 'A';
        //            queue.Add(sideA);
        //        }

        //        if (GridHelper.IsWithinGrid(grid, sideB.X, sideB.Y) && grid[sideB.X, sideB.Y] == '.')
        //        {
        //            grid[sideB.X, sideB.Y] = 'B';
        //            queue.Add(sideB);
        //        }

        //    }

        //    while (queue.Any())
        //    {
        //        var newQueue = new List<(int x, int y)>();
        //        foreach (var coords in queue)
        //        {
        //            var neighbours = GridHelper.FindNeighbours(grid, coords.X, coords.Y);
        //            foreach (var nb in neighbours)
        //            {
        //                if (grid[nb.x, nb.y] == '.')
        //                {
        //                    grid[nb.x, nb.y] = grid[coords.X, coords.Y];
        //                    newQueue.Add(nb);
        //                }
        //            }
        //        }
        //        queue = newQueue;
        //    }

        //    return grid;
        //}

        //private char DirectionToArrow(Direction direction)
        //{
        //    return direction switch
        //    {
        //        Direction.Up => '^',
        //        Direction.Right => '>',
        //        Direction.Down => 'v',
        //        Direction.Left => '<',
        //        _ => throw new NotImplementedException()
        //    };
        //}
    }

    internal class Instruction
    {
        public Instruction(string input)
        {
            var sections = input.Split(' ');
            Direction = CharToDirection(sections[0][0]);
            Distance = int.Parse(sections[1]);
            var hexCode = sections[2];
            HexDistance = int.Parse(hexCode.Substring(2,5), System.Globalization.NumberStyles.HexNumber);
            HexDirection = CharToDirection(hexCode[7]);
        }

        public Direction Direction { get; set; }
        public int Distance { get; set; }
        public int HexDistance { get; set; }
        public Direction HexDirection { get; set; }

        private Direction CharToDirection(char c)
        {
            return c switch
            {
                'U' => Direction.Up,
                'R' => Direction.Right,
                'D' => Direction.Down,
                'L' => Direction.Left,
                '0' => Direction.Right,
                '1' => Direction.Down,
                '2' => Direction.Left,
                '3' => Direction.Up,
                _ => throw new NotImplementedException("Should not use chars outside of URDL0123")
            };
        }
    }
}
