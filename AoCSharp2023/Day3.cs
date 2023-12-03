using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoCSharp2023
{
    internal class Day3
    {
        public Day3()
        {
            string[] lines = File.ReadAllLines("./inputs/3.txt");

            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        int Part1(IList<string> input)
        {
            var result = new List<int>();
            for (int y = 0; y < input.Count; y++)
            {
                var number = new List<char>();
                bool adjacentToSymbol = false;
                for (int x = 0; x < input.Count; x++)
                {
                    if (char.IsNumber(input[y][x]))
                    {
                        number.Add(input[y][x]);

                        if (!adjacentToSymbol)
                        {
                            var neighbours = ValidNeighbours(x, y, input[0].Length, input.Count);
                            foreach (var (xx, yy) in neighbours)
                            {
                                if (input[yy][xx] != '.' && !char.IsNumber(input[yy][xx]))
                                {
                                    adjacentToSymbol = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!char.IsNumber(input[y][x]) || x == input.Count - 1)
                    {
                        if (adjacentToSymbol && number.Count > 0)
                        {
                            result.Add(Int32.Parse(new string(number.ToArray())));
                        }
                        number = new List<char>();
                        adjacentToSymbol = false;
                    }
                }
            }
            return result.Sum();
        }

        int Part2(IList<string> input)
        {
            Dictionary<(int x, int y), List<int>> gearMap = new Dictionary<(int x, int y), List<int>>();
            for (int y = 0; y < input.Count; y++)
            {
                var number = new List<char>();
                HashSet<(int x, int y)> adjacentGears = new HashSet<(int x, int y)>();
                for (int x = 0; x < input.Count; x++)
                {
                    if (char.IsNumber(input[y][x]))
                    {
                        number.Add(input[y][x]);

                        var neighbours = ValidNeighbours(x, y, input[0].Length, input.Count);
                        foreach (var (xx, yy) in neighbours)
                        {
                            if (input[yy][xx] == '*')
                            {
                                adjacentGears.Add((xx, yy));
                                break;
                            }
                        }
                    }
                    if (!char.IsNumber(input[y][x]) || x == input.Count - 1)
                    {
                        if (adjacentGears.Any() && number.Count > 0)
                        {
                            var numInt = Int32.Parse(new string(number.ToArray()));
                            foreach (var gearPos in adjacentGears)
                            {
                                if (!gearMap.TryGetValue(gearPos, out var gearNumberList))
                                {
                                    gearNumberList = new List<int>();
                                    gearMap.Add(gearPos, gearNumberList);
                                }
                                gearNumberList.Add(numInt);
                            }
                        }
                        number = new List<char>();
                        adjacentGears = new HashSet<(int x, int y)>();
                    }
                }
            }
            return gearMap.Values.Where(v => v.Count == 2).Select(v => v[0] * v[1]).Sum();
        }


        List<(int x, int y)> ValidNeighbours(int x, int y, int xMax, int yMax)
        {
            var result = new List<(int x, int y)>();
            for (int xd = -1; xd < 2; xd++)
            {
                for (int yd = -1; yd < 2; yd++)
                {
                    var xn = x + xd;
                    var yn = y + yd;
                    if (xn < xMax && xn >= 0 && yn < yMax && yn >= 0)
                    {
                        result.Add((xn, yn));
                    }
                }
            }
            return result;
        }
    }
}
