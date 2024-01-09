using System;
using System.ComponentModel;
using System.Text;
using Shared;

namespace AoCSharp2023
{
    internal class Day10
    {
        public Day10()
        {
            string[] lines = File.ReadAllLines("./inputs/10.txt");
            var grid = GridHelper.CreateGrid(lines);
            var results = Solve(grid);
            Console.WriteLine((results.LoopLength + 1) / 2);
            Console.WriteLine(new int[] { results.SideOne, results.SideTwo }.Min()); // Assuming inside is smaller than outside
        }


        (int X, int Y) FindAnimalCoordinates(char[,] grid)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y] == 'S') return (x, y);
                }
            }
            throw new Exception("Can't find the animal");
        }

        enum Direction
        {
            Left,
            Right,
            Down,
            Up,
        }

        List<(int x, int y)> FindNeighbours(char[,] grid, int x, int y)
        {
            var neighbours = new List<(int x, int y)>();
            (int x, int y)[] candidates = { (x, y + 1), (x, y - 1), (x + 1, y), (x - 1, y) };
            foreach (var candidate in candidates)
            {
                try
                {
                    var _ = grid[candidate.x, candidate.y];
                    neighbours.Add(candidate);
                }
                catch { }
            }
            return neighbours;
        }

        void DrawGrid(char[,] grid)
        {
            char[] markedSymbols = { 'S', '1', '2' };
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                var s = new StringBuilder();
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    if (markedSymbols.Contains(grid[x, y]))
                    {
                        s.Append(grid[x, y]);
                    }
                    else
                    {
                        s.Append('.');
                    }
                }
                Console.WriteLine(s.ToString());
            }
        }

        void MarkOnGrid(char[,] grid, int x, int y, char mark)
        {
            char[] pipeSymbols = { 'S' };
            if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1) && !pipeSymbols.Contains(grid[x,y]))
            {
                grid[x, y] = mark;
            }
        }

        (int Side1, int Side2) FloodInOutSides(char[,] grid)
        {
            var queue = new List<(int x, int y)>();
            for(int x = 0; x < grid.GetLength(0); x++)
            {
                for(int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y] == '1' || grid[x, y] == '2')
                    {
                        queue.Add((x, y));
                    }
                }
            }

            char[] markedSymbols = { 'S', '1', '2' };
            while (queue.Any())
            {
                var newQueue = new List<(int x, int y)>();
                foreach(var(x, y) in queue)
                {
                    var nb = FindNeighbours(grid, x, y);
                    foreach(var(nx,ny) in nb)
                    {
                        if (!markedSymbols.Contains(grid[nx, ny]))
                        {
                            grid[nx, ny] = grid[x, y];
                            newQueue.Add((nx,ny));
                        }
                    }
                }
                queue = newQueue;
            }

            var sideOne = 0;
            var sideTwo = 0;
            foreach(var c in grid)
            {
                if(c == '1')
                {
                    sideOne++;
                }

                if(c == '2')
                {
                    sideTwo++;
                }
            }

            //DrawGrid(grid);
            return (sideOne, sideTwo);
        }

        (int LoopLength,int SideOne,int SideTwo) Solve(char[,] grid)
        {
            var (x, y) = FindAnimalCoordinates(grid);
            // Find a connected neighbour
            char[] leftConnectors = { 'F', 'L', '-' };
            char[] rightConnectors = { '7', 'J', '-' };
            char[] upConnectors = { 'F', '7', '|' };
            char[] downConnectors = { 'L', 'J', '|' };
            var neighbours = FindNeighbours(grid, x, y); // In case animal is on edge, as is case in examples..
            bool left = neighbours.Contains((x - 1, y)) && leftConnectors.Contains(grid[x - 1, y]);
            bool right = neighbours.Contains((x + 1, y)) && rightConnectors.Contains(grid[x + 1, y]);
            bool up = neighbours.Contains((x, y - 1)) && upConnectors.Contains(grid[x, y - 1]);
            bool down = neighbours.Contains((x, y + 1)) && downConnectors.Contains(grid[x, y + 1]);

            var inOutGrid = new char[grid.GetLength(0), grid.GetLength(1)];
            inOutGrid[x, y] = 'S';
            Direction directionOfPrevious = Direction.Right; // Just to avoid some unassigned complaints..
            if (left)
            {
                x--;
                directionOfPrevious = Direction.Right;
            }
            else if (right)
            {
                x++;
                directionOfPrevious = Direction.Left;
            }
            else if (up)
            {
                y--;
                directionOfPrevious = Direction.Down;
            }
            else if (down)
            {
                y++;
                directionOfPrevious = Direction.Up;
            }

            // Start traversing the loop
            int steps = 1;
            while (grid[x, y] != 'S')
            {
                inOutGrid[x, y] = 'S';
                if (grid[x, y] == '-')
                {
                    if (directionOfPrevious == Direction.Right)
                    {
                        MarkOnGrid(inOutGrid, x, y - 1, '2');
                        MarkOnGrid(inOutGrid, x, y + 1, '1');
                        x--;
                    }
                    else if (directionOfPrevious == Direction.Left)
                    {
                        MarkOnGrid(inOutGrid, x, y - 1, '1');
                        MarkOnGrid(inOutGrid, x, y + 1, '2');
                        x++;
                    }
                    else throw new Exception("Unexpected direction");
                }
                else if (grid[x, y] == '|')
                {
                    if (directionOfPrevious == Direction.Up)
                    {
                        MarkOnGrid(inOutGrid, x - 1, y, '2');
                        MarkOnGrid(inOutGrid, x + 1, y, '1');
                        y++;
                    }
                    else if (directionOfPrevious == Direction.Down)
                    {
                        MarkOnGrid(inOutGrid, x - 1, y, '1');
                        MarkOnGrid(inOutGrid, x + 1, y, '2');
                        y--;
                    }
                    else throw new Exception("Unexpected direction");
                }
                else if (grid[x, y] == 'F')
                {
                    if (directionOfPrevious == Direction.Right)
                    {
                        MarkOnGrid(inOutGrid, x, y - 1, '2');
                        MarkOnGrid(inOutGrid, x - 1, y, '2');
                        y++;
                        directionOfPrevious = Direction.Up;
                    }
                    else if (directionOfPrevious == Direction.Down)
                    {
                        MarkOnGrid(inOutGrid, x, y - 1, '1');
                        MarkOnGrid(inOutGrid, x - 1, y, '1');
                        x++;
                        directionOfPrevious = Direction.Left;
                    }
                    else throw new Exception("Unexpected direction");
                }
                else if (grid[x, y] == '7')
                {
                    if (directionOfPrevious == Direction.Left)
                    {
                        MarkOnGrid(inOutGrid, x, y - 1, '1');
                        MarkOnGrid(inOutGrid, x + 1, y, '1');
                        y++;
                        directionOfPrevious = Direction.Up;
                    }
                    else if (directionOfPrevious == Direction.Down)
                    {
                        MarkOnGrid(inOutGrid, x, y - 1, '2');
                        MarkOnGrid(inOutGrid, x + 1, y, '2');
                        x--;
                        directionOfPrevious = Direction.Right;
                    }
                    else throw new Exception("Unexpected direction");
                }
                else if (grid[x, y] == 'J')
                {
                    if (directionOfPrevious == Direction.Left)
                    {
                        MarkOnGrid(inOutGrid, x, y + 1, '2');
                        MarkOnGrid(inOutGrid, x + 1, y, '2');
                        y--;
                        directionOfPrevious = Direction.Down;
                    }
                    else if (directionOfPrevious == Direction.Up)
                    {
                        MarkOnGrid(inOutGrid, x, y + 1, '1');
                        MarkOnGrid(inOutGrid, x + 1, y, '1');
                        x--;
                        directionOfPrevious = Direction.Right;
                    }
                    else throw new Exception("Unexpected direction");
                }
                else if (grid[x, y] == 'L')
                {
                    if (directionOfPrevious == Direction.Right)
                    {
                        MarkOnGrid(inOutGrid, x, y + 1, '1');
                        MarkOnGrid(inOutGrid, x - 1, y, '1');
                        y--;
                        directionOfPrevious = Direction.Down;
                    }
                    else if (directionOfPrevious == Direction.Up)
                    {
                        MarkOnGrid(inOutGrid, x, y + 1, '2');
                        MarkOnGrid(inOutGrid, x - 1, y, '2');
                        x++;
                        directionOfPrevious = Direction.Left;
                    }
                    else throw new Exception("Unexpected direction");
                }

                steps++;
            }

            var (sideOne, sideTwo) = FloodInOutSides(inOutGrid);

            return (steps, sideOne, sideTwo);
        }
    }
}
