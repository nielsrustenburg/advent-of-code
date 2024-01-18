using System.Text;

namespace Shared
{
    public static class GridHelper
    {
        public static char[,] CreateGrid(string[] lines)
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

        public static int[,] CreateNumericalGrid(string[] lines)
        {
            var grid = new int[lines[0].Length, lines.Count()];
            for (int y = 0; y < lines.Count(); y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    grid[x, y] = int.Parse(lines[y].Substring(x,1));
                }
            }
            return grid;
        }

        public static void DrawGrid(char[,] grid)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                var s = new StringBuilder();
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    s.Append(grid[x, y]);
                }
                Console.WriteLine(s.ToString());
            }
        }

        public static List<(int x, int y)> FindNeighbours<T>(T[,] grid, int x, int y)
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

        public static Dictionary<Direction, (int x, int y)> FindNeighboursWithDirections<T>(T[,] grid, int x, int y)
        {
            var neighbours = new Dictionary<Direction, (int x, int y)>();
            Direction[] candidateDirections = {Direction.Up, Direction.Right, Direction.Down, Direction.Left };
            foreach (var candidateDirection in candidateDirections)
            {
                try
                {
                    var candidate = candidateDirection.StepInDirection(x, y);
                    var _ = grid[candidate.X, candidate.Y];
                    neighbours.Add(candidateDirection, candidate);
                }
                catch { }
            }
            return neighbours;
        }

        public static bool IsWithinGrid<T>(T[,] grid, int x, int y) {
            return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
        }
    }
}
