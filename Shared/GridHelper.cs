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
    }
}
