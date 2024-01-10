using Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoCSharp2023
{
    internal class Day16
    {
        public Day16()
        {
            string[] lines = File.ReadAllLines("./inputs/16.txt");
            var grid = GridHelper.CreateGrid(lines);
            Console.WriteLine(Part1(grid));
            Console.WriteLine(Part2(grid));
        }

        public int Part1(char[,] grid)
        {
            return FindEnergizedSquares(grid, (0, 0, Direction.Right));
        }

        public int Part2(char[,] grid)
        {
            int bestSoFar = 0;
            for(int x = 0; x < grid.GetLength(0); x++)
            {
                var upperCandidate = FindEnergizedSquares(grid, (x, 0, Direction.Down));
                var lowerCandidate = FindEnergizedSquares(grid, (x, grid.GetLength(1)-1, Direction.Down));
                if(upperCandidate > bestSoFar)
                {
                    bestSoFar = upperCandidate;
                }

                if(lowerCandidate > bestSoFar)
                {
                    bestSoFar = lowerCandidate;
                }
            }

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                var leftCandidate = FindEnergizedSquares(grid, (0, y, Direction.Right));
                var rightCandidate = FindEnergizedSquares(grid, (grid.GetLength(0)-1, y, Direction.Left));
                if (leftCandidate > bestSoFar)
                {
                    bestSoFar = leftCandidate;
                }

                if (rightCandidate > bestSoFar)
                {
                    bestSoFar = rightCandidate;
                }
            }

            return bestSoFar;
        }

        public int FindEnergizedSquares(char[,] grid, (int x, int y, Direction dir) initialBeam)
        {
            var visited = new HashSet<(int x, int y, Direction dir)>();
            var queue = new List<(int x, int y, Direction dir)>
            {
                (initialBeam.x, initialBeam.y, initialBeam.dir)
            };

            while (queue.Any())
            {
                var (x, y, dir) = queue.First();
                queue.RemoveAt(0);
                if (GridHelper.IsWithinGrid(grid, x, y) && !visited.Contains((x, y, dir)))
                {
                    visited.Add((x, y, dir));
                    queue.AddRange(NextCoordinates(x, y, dir, grid[x, y]));
                }
            }

            return visited.DistinctBy(visit => (visit.x, visit.y)).Count();
        }

        List<(int X, int Y, Direction Dir)> NextCoordinates(int x, int y, Direction dir, char tile)
        {
            return (dir, tile) switch
            {
                (Direction.Up, '.') => new List<(int X, int Y, Direction Dir)> { (x, y - 1, Direction.Up) },
                (Direction.Right, '.') => new List<(int X, int Y, Direction Dir)> { (x + 1, y, Direction.Right) },
                (Direction.Down, '.') => new List<(int X, int Y, Direction Dir)> { (x, y + 1, Direction.Down) },
                (Direction.Left, '.') => new List<(int X, int Y, Direction Dir)> { (x - 1, y, Direction.Left) },
                (Direction.Up, '|') => new List<(int X, int Y, Direction Dir)> { (x, y - 1, Direction.Up) },
                (Direction.Right, '-') => new List<(int X, int Y, Direction Dir)> { (x + 1, y, Direction.Right) },
                (Direction.Down, '|') => new List<(int X, int Y, Direction Dir)> { (x, y + 1, Direction.Down) },
                (Direction.Left, '-') => new List<(int X, int Y, Direction Dir)> { (x - 1, y, Direction.Left) },
                (Direction.Up, '-') => new List<(int X, int Y, Direction Dir)> { (x - 1, y, Direction.Left), (x + 1, y, Direction.Right) },
                (Direction.Right, '|') => new List<(int X, int Y, Direction Dir)> { (x, y - 1, Direction.Up), (x, y + 1, Direction.Down) },
                (Direction.Down, '-') => new List<(int X, int Y, Direction Dir)> { (x - 1, y, Direction.Left), (x + 1, y, Direction.Right) },
                (Direction.Left, '|') => new List<(int X, int Y, Direction Dir)> { (x, y - 1, Direction.Up), (x, y + 1, Direction.Down) },
                (Direction.Up, '/') => new List<(int X, int Y, Direction Dir)> { (x + 1, y, Direction.Right) },
                (Direction.Right, '/') => new List<(int X, int Y, Direction Dir)> { (x, y - 1, Direction.Up) },
                (Direction.Down, '/') => new List<(int X, int Y, Direction Dir)> { (x - 1, y, Direction.Left) },
                (Direction.Left, '/') => new List<(int X, int Y, Direction Dir)> { (x, y + 1, Direction.Down) },
                (Direction.Up, '\\') => new List<(int X, int Y, Direction Dir)> { (x - 1, y, Direction.Left) },
                (Direction.Right, '\\') => new List<(int X, int Y, Direction Dir)> { (x, y + 1, Direction.Down) },
                (Direction.Down, '\\') => new List<(int X, int Y, Direction Dir)> { (x + 1, y, Direction.Right) },
                (Direction.Left, '\\') => new List<(int X, int Y, Direction Dir)> { (x, y - 1, Direction.Up) },
                (_, _) => throw new Exception("shouldn't be here")
            };
        }
    }

    enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
}
