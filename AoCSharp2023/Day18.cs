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
        }

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
