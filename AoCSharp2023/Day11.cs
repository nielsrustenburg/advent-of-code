using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoCSharp2023
{
    internal class Day11
    {
        public Day11()
        {
            string[] lines = File.ReadAllLines("./inputs/11.txt");
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        long Part1(string[] lines)
        {
            var galaxies = FindGalaxies(lines);
            var emptyRows = FindEmptyRows(lines);
            var emptyCols = FindEmptyCols(lines);
            var updatedGalaxies = AdjustGalaxyCoordinates(galaxies, emptyRows, emptyCols);
            return SumAllPairwiseDistances(updatedGalaxies);
        }

        long Part2(string[] lines)
        {
            var galaxies = FindGalaxies(lines);
            var emptyRows = FindEmptyRows(lines);
            var emptyCols = FindEmptyCols(lines);
            var updatedGalaxies = AdjustGalaxyCoordinates(galaxies, emptyRows, emptyCols, true);
            return SumAllPairwiseDistances(updatedGalaxies);
        }

        List<(int x, int y)> FindGalaxies(string[] lines)
        {
            var galaxies = new List<(int x, int y)>();
            for (int y = 0; y < lines.Count(); y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == '#')
                    {
                        galaxies.Add((x, y));
                    }
                }
            }
            return galaxies;
        }

        List<int> FindEmptyRows(string[] lines)
        {
            var rows = new List<int>();
            for (int y = 0; y < lines.Count(); y++)
            {
                bool empty = true;
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == '#')
                    {
                        empty = false;
                    }
                }
                if (empty)
                {
                    rows.Add(y);
                }
            }
            return rows;
        }

        List<int> FindEmptyCols(string[] lines) {
            var cols = new List<int>();
            for(int x = 0; x < lines[0].Length; x++)
            {
                bool empty = true;
                for(int y = 0; y < lines.Count(); y++)
                {
                    if (lines[y][x] == '#')
                    {
                        empty = false;
                    }
                }
                if (empty)
                {
                    cols.Add(x);
                }
            }
            return cols;
        }

        List<(int x, int y)> AdjustGalaxyCoordinates(List<(int x, int y)> galaxies, List<int> emptyRows, List<int> emptyCols, bool part2 = false)
        {
            List<(int x, int y)> updatedGalaxies = new List<(int x, int y)> ();

            foreach(var galaxy in galaxies)
            {
                var yOffset = emptyRows.Count(rowId => galaxy.y > rowId);
                var xOffset = emptyCols.Count(colId => galaxy.x > colId);

                if (part2)
                {
                    yOffset = (yOffset * 999999);
                    xOffset = (xOffset * 999999);
                }

                updatedGalaxies.Add((galaxy.x + xOffset, galaxy.y + yOffset));
            }

            return updatedGalaxies;
        }

        long SumAllPairwiseDistances(List<(int x, int y)> galaxies)
        {
            long sum = 0;

            var index = 0;
            while (index < galaxies.Count-1)
            {
                var currentGalaxy = galaxies[index];
                for(int i = index+1; i < galaxies.Count; i++)
                {
                    sum += AoCMath.ManhattanDistance(currentGalaxy, galaxies[i]);
                }
                index++;
            }

            return sum;
        }
    }
}
