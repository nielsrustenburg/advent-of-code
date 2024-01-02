using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoCSharp2023
{
    internal class Day6
    {
        public Day6()
        {
            string[] lines = File.ReadAllLines("./inputs/6.txt");
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        int Part1(IList<string> lines)
        {
            var races = ParseInput(lines[0], lines[1]);
            return races.Select(race => NumberOfRecordBreakingStrategies(race.totalTime, race.recordDistance)).Aggregate(1, (a, b) => a * b);
        }

        long Part2(IList<string> lines)
        {
            var race = ParseInput2(lines[0], lines[1]);
            return NumberOfRecordBreakingStrategiesLong(race.totalTime, race.recordDistance);
        }


        List<(int totalTime, int recordDistance)> ParseInput(string timeLine, string distLine)
        {
            var times = timeLine.Split(':')[1].Split(' ').Where(x => x != string.Empty).Select(x => int.Parse(x)).ToList();
            var distances = distLine.Split(':')[1].Split(' ').Where(x => x != string.Empty).Select(x => int.Parse(x)).ToList();
            var result = new List<(int totalTime, int recordDistance)>();
            for (int i = 0; i < times.Count; i++)
            {
                result.Add((times[i], distances[i]));
            }
            return result;
        }

        (long totalTime, long recordDistance) ParseInput2(string timeLine, string distLine)
        {
            var time = long.Parse(timeLine.Split(':')[1].Replace(" ", ""));
            var distance = long.Parse(distLine.Split(':')[1].Replace(" ", ""));
            return (time, distance);
        }

        long NumberOfRecordBreakingStrategiesLong(long totalTime, long distanceToBeat)
        {
            // Find the first winning strategy, anything between that and the inverse strategy should be winning
            var currentBestChargeTime = totalTime / 2;
            bool beatsRecord = (totalTime - currentBestChargeTime) * currentBestChargeTime > distanceToBeat;
            if (!beatsRecord)
            {
                return 0;
            }

            var currentChargeTime = currentBestChargeTime;
            long stepSize = currentChargeTime / 2;
            while(stepSize > 1)
            {
                if (beatsRecord)
                {
                    currentChargeTime -= stepSize;
                } else
                {
                    currentChargeTime += stepSize;
                }

                beatsRecord = (totalTime - currentChargeTime) * currentChargeTime > distanceToBeat;
                stepSize = stepSize / 2;
            }

            // I think its likely that the above strategy can already reach the number with some proper reasoning, but dividing by 2 on integers might mess me up so to be safe, after approaching the edge going to find it iteratively
            while (beatsRecord)
            {
                currentChargeTime--;
                beatsRecord = (totalTime - currentChargeTime) * currentChargeTime > distanceToBeat;
            }

            while (!beatsRecord)
            {
                currentChargeTime++;
                beatsRecord = (totalTime - currentChargeTime) * currentChargeTime > distanceToBeat;
            }

            return (totalTime - currentChargeTime) - currentChargeTime + 1; // All between currentChargeTime and inverse 
        }

        int NumberOfRecordBreakingStrategies(int totalTime, int distanceToBeat)
        {
            List<int> winningStrategies = new List<int>();
            // TODO: can consider doing half work because dist is chargeTime * remaining time, so if 3 chargetime 4 remaining is winning then so is 4 charge 3 remaining
            for (int i = 0; i < totalTime; i++)
            {
                var distance = DistanceTravelled(i, totalTime);
                if (distance > distanceToBeat)
                {
                    winningStrategies.Add(i);
                }
            }
            return winningStrategies.Count();
        }

        int DistanceTravelled(int secondsCharged, int totalTime)
        {
            return (totalTime - secondsCharged) * secondsCharged;
        }
    }
}
