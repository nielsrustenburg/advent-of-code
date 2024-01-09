using Shared;

namespace AoCSharp2023
{
    internal class Day14
    {
        Dictionary<string, string> _cache;

        Dictionary<long, List<(string, int)>> _statusCache;

        public Day14()
        {
            _cache = new Dictionary<string, string>();
            _statusCache = new Dictionary<long, List<(string, int)>>();
            string[] lines = File.ReadAllLines("./inputs/14.txt");
            var grid = GridHelper.CreateGrid(lines);
            Console.WriteLine(Part1(grid));
            Console.WriteLine(Part2(grid));
        }

        long Part1(char[,] grid)
        {
            var tiltedGrid = Tilt(grid, 0);
            return TotalLoad(tiltedGrid);
        }

        long Part2(char[,] grid)
        {
            var newGrid = grid;
            var repeatingCyclePeriod = -1;
            var totalCycles = 1000000000;
            for (int i = 0; i < totalCycles; i++)
            {
                newGrid = Cycle(newGrid);
                if(i == 10000000)
                {
                    System.Diagnostics.Debugger.Break();
                }
                var load = TotalLoad(newGrid);
                var gridKey = GridToString(newGrid);
                if(!_statusCache.TryGetValue(load, out var resultsList))
                {
                    resultsList = new List<(string, int)>();
                    _statusCache[load] = resultsList;
                } else
                {
                    var match = resultsList.FirstOrDefault(a => a.Item1 == gridKey);
                    if (match.Item1 != null)
                    {
                        repeatingCyclePeriod = i - match.Item2;
                        var remainingCycles = totalCycles - i;
                        totalCycles = i + (remainingCycles % repeatingCyclePeriod);
                    }
                }
                resultsList.Add((gridKey,i));
            }
            return TotalLoad(newGrid);
        }

        public long TotalLoad(char[,] grid)
        {
            long total = 0;
            for(int y = 0; y < grid.GetLength(1); y++)
            {
                var loadPerRock = grid.GetLength(1) - y;
                for(int x = 0;  x < grid.GetLength(0); x++)
                {
                    if (grid[x,y] == 'O')
                    {
                        total += loadPerRock;
                    }
                }
            }
            return total;
        }

        public string GridToString(char[,] grid)
        {
            var s = "";
            for(int x = 0; x < grid.GetLength(0); x++)
            {
                for(int y = 0;y < grid.GetLength(1); y++)
                {
                    s = s + grid[x,y];
                }
            }
            return s;
        }

        public char[,] Tilt(char[,] grid, int direction)
        {
            var newGrid = new char[grid.GetLength(0), grid.GetLength(1)];
            bool horizontalAxisTilt = direction % 2 == 0;
            var iLimit = horizontalAxisTilt ? grid.GetLength(0) : grid.GetLength(1);
            var jLimit = horizontalAxisTilt ? grid.GetLength(1) : grid.GetLength(0);
            for (int i = 0; i < iLimit; i++)
            {
                var roundRocksPerSection = new List<int>();
                var sectionBoundaries = new List<int>();
                var roundRocksCount = 0;
                for (int j = 0; j < jLimit; j++)
                {
                    var (x, y) = XyHelper(i, j, jLimit, direction);
                    if (grid[x, y] == 'O')
                    {
                        roundRocksCount++;
                    }
                    else if (grid[x, y] == '.')
                    {

                    }
                    else if (grid[x, y] == '#')
                    {
                        sectionBoundaries.Add(j);
                        roundRocksPerSection.Add(roundRocksCount);
                        roundRocksCount = 0;
                    }
                }
                roundRocksPerSection.Add(roundRocksCount);

                int sectionIndex = 0;
                for (int j = 0; j < jLimit; j++)
                {
                    var (x, y) = XyHelper(i, j, jLimit, direction);
                    if (sectionIndex < sectionBoundaries.Count && sectionBoundaries[sectionIndex] == j)
                    {
                        newGrid[x, y] = '#';
                        sectionIndex++;
                    }
                    else if (sectionIndex < roundRocksPerSection.Count && roundRocksPerSection[sectionIndex] > 0)
                    {
                        newGrid[x, y] = 'O';
                        roundRocksPerSection[sectionIndex]--;
                    } else
                    {
                        newGrid[x, y] = '.';
                    }
                }
            }

            return newGrid;
        }

        public char[,] Tilt2(char[,] grid, int direction)
        {
            var iLimit = direction % 2 == 0 ? grid.GetLength(1) : grid.GetLength(0);
            var jLimit = direction % 2 == 0 ? grid.GetLength(0) : grid.GetLength(1);
            var newGrid = new char[grid.GetLength(0), grid.GetLength(1)];
            for(int i = 0; i < iLimit; i++)
            {
                var rockString = "";
                for(int j = 0; j < jLimit; j++)
                {
                    var (x, y) = XyHelper(i, j, jLimit, direction);
                    rockString = rockString + grid[x, y];
                }

                var newRocks = MoveRocksLeft(rockString);
                for(int j = 0; j < jLimit; j++)
                {
                    var (x, y) = XyHelper(i, j, jLimit, direction);
                    newGrid[x, y] = newRocks[j];
                }
            }

            return newGrid;
        }

        public string MoveRocksLeft(string line)
        {
            if(_cache.TryGetValue(line, out var result))
            {
                return result;
            }

            var roundRocksPerSection = new List<int>();
            var sectionBoundaries = new List<int>();
            var roundRocksCount = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == 'O')
                {
                    roundRocksCount++;
                }
                else if (line[i] == '#')
                {
                    sectionBoundaries.Add(i);
                    roundRocksPerSection.Add(roundRocksCount);
                    roundRocksCount = 0;
                }
            }
            roundRocksPerSection.Add(roundRocksCount);
            int sectionIndex = 0;
            var withRocksMoved = "";
            for (int i = 0; i < line.Length; i++)
            {
                if (sectionIndex < sectionBoundaries.Count && sectionBoundaries[sectionIndex] == i)
                {
                    withRocksMoved = withRocksMoved + '#';
                    sectionIndex++;
                }
                else if (sectionIndex < roundRocksPerSection.Count && roundRocksPerSection[sectionIndex] > 0)
                {
                    withRocksMoved = withRocksMoved + 'O';
                    roundRocksPerSection[sectionIndex]--;
                }
                else
                {
                    withRocksMoved = withRocksMoved + '.';
                }
            }

            _cache[line] = withRocksMoved;
            return withRocksMoved;
        }

        public char[,] Cycle(char[,] grid)
        {
            var tiltNorth = Tilt2(grid, 0);
            var tiltWest = Tilt2(tiltNorth, 1);
            var tiltSouth = Tilt2(tiltWest, 2);
            return Tilt2(tiltSouth, 3);
        }

        private (int x, int y) XyHelper(int i, int j, int jLimit, int direction)
        {
            if (direction == 0)
            {
                return (i, j);
            }
            if (direction == 1)
            {
                return (j, i);
            }
            if (direction == 2)
            {
                return (i, jLimit - (j + 1));
            }
            if (direction == 3)
            {
                return (jLimit - (j + 1), i);
            }
            throw new Exception("Should not get here");
        }
    }
}
