namespace AoCSharp2023
{
    internal class Day13
    {
        public Day13()
        {
            string[] lines = File.ReadAllLines("./inputs/13.txt");
            var patterns = ExtractPatterns(lines);
            Console.WriteLine(Part1(patterns));
            Console.WriteLine(Part2(patterns));
        }

        long Part1(List<List<string>> patterns)
        {
            long total = 0;
            foreach(var pattern in patterns)
            {
                total += FindMirror(pattern);
            }
            return total;
        }

        long Part2(List<List<string>> patterns)
        {
            long total = 0;
            foreach (var pattern in patterns)
            {
                total += FindMirror(pattern, true);
            }
            return total;
        }

        List<List<string>> ExtractPatterns(string[] lines)
        {
            var currentPattern = new List<string>();
            var allPatterns = new List<List<string>>();
            for(int i = 0; i < lines.Length; i++) {
                if (lines[i] == string.Empty)
                {
                    allPatterns.Add(currentPattern);
                    currentPattern = new List<string>();
                } else
                {
                    currentPattern.Add(lines[i]);
                }
            }
            allPatterns.Add(currentPattern);
            return allPatterns;
        }

        string[] Rotate(IList<string> pattern)
        {
            string[] rotated = new string[pattern[0].Length];
            for(int i = 0; i < pattern[0].Length; i++)
            {
                rotated[i] = pattern.Aggregate("", (a, b) => a + b[i]); 
            }
            return rotated;
        }

        int FindMirror(IList<string> pattern, bool part2 = false)
        {
            var result = FindMirrorRow(pattern, part2 ? 1 : 0);
            if (result == 0)
            {
                return FindMirrorCol(pattern, part2 ? 1 : 0);
            } else
            {
                return result * 100;
            }
        }

        int FindMirrorRow(IList<string> pattern, int targetDistance = 0)
        {
            for(int i = 1; i < pattern.Count; i++) {
                var distance = 0;
                for (int j = 0; j < i && j < (pattern.Count - i); j++)
                {
                    distance += StringDistance(pattern[i - (1 + j)], pattern[i + j]);
                    if(distance > targetDistance)
                    {
                        break;
                    }
                }
                if (distance == targetDistance)
                {
                    return i;
                }
            }
            return 0;
        }

        int FindMirrorCol(IList<string> pattern, int targetDistance = 0)
        {
            return FindMirrorRow(Rotate(pattern), targetDistance);
        }

        // Assumes strings are equal length
        int StringDistance(string a, string b)
        {
            int dist = 0;
            for (int i = 0; i < a.Length; i++) {
                if (a[i] != b[i]) {  dist++; }
            }
            return dist;
        }
    }
}
