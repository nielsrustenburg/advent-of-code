namespace AoCSharp2023
{
    internal class Day10
    {
        public Day10()
        {
            string[] lines = File.ReadAllLines("./inputs/10.txt");
            Console.WriteLine(Part1(lines));
            //Console.WriteLine(Part2(lines));
        }

        int Part1(string[] lines)
        {
            var grid = CreateGrid(lines);
            return (FindLoopLength(grid) + 1) / 2;
        }

        char[,] CreateGrid(string[] lines)
        {
            var grid = new char[lines[0].Length, lines.Count()];
            for (int y = 0; y < lines.Count(); y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    grid[x, y] = lines[y][x];
                }
            }
            return grid;
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

        int FindLoopLength(char[,] grid)
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
                if (grid[x, y] == '-')
                {
                    if (directionOfPrevious == Direction.Right)
                    {
                        x--;
                    }
                    else if (directionOfPrevious == Direction.Left)
                    {
                        x++;
                    }
                    else throw new Exception("Unexpected direction");
                }
                else if (grid[x, y] == '|')
                {
                    if (directionOfPrevious == Direction.Up)
                    {
                        y++;
                    }
                    else if (directionOfPrevious == Direction.Down)
                    {
                        y--;
                    }
                    else throw new Exception("Unexpected direction");
                }
                else if (grid[x, y] == 'F')
                {
                    if (directionOfPrevious == Direction.Right)
                    {
                        y++;
                        directionOfPrevious = Direction.Up;
                    }
                    else if (directionOfPrevious == Direction.Down)
                    {
                        x++;
                        directionOfPrevious = Direction.Left;
                    }
                    else throw new Exception("Unexpected direction");
                }
                else if (grid[x, y] == '7')
                {
                    if (directionOfPrevious == Direction.Left)
                    {
                        y++;
                        directionOfPrevious = Direction.Up;
                    }
                    else if (directionOfPrevious == Direction.Down)
                    {
                        x--;
                        directionOfPrevious = Direction.Right;
                    }
                    else throw new Exception("Unexpected direction");
                }
                else if (grid[x, y] == 'J')
                {
                    if (directionOfPrevious == Direction.Left)
                    {
                        y--;
                        directionOfPrevious = Direction.Down;
                    }
                    else if (directionOfPrevious == Direction.Up)
                    {
                        x--;
                        directionOfPrevious = Direction.Right;
                    }
                    else throw new Exception("Unexpected direction");
                }
                else if (grid[x, y] == 'L')
                {
                    if (directionOfPrevious == Direction.Right)
                    {
                        y--;
                        directionOfPrevious = Direction.Down;
                    }
                    else if (directionOfPrevious == Direction.Up)
                    {
                        x++;
                        directionOfPrevious = Direction.Left;
                    }
                    else throw new Exception("Unexpected direction");
                }

                steps++;
            }
            return steps;
        }
    }
}
