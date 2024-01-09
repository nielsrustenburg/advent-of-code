namespace AoCSharp2023
{
    internal class Day12
    {
        Dictionary<string, long> _cache;
        public Day12()
        {
            _cache = new Dictionary<string, long>();
            string[] lines = File.ReadAllLines("./inputs/12.txt");
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        public long Part1(string[] lines)
        {
            long total = 0;
            foreach(string line in lines)
            {
                var (template, blocks) = ReadLine(line);
                total += AmountOfArrangements(template, blocks);
            }
            return total;
        }

        public long Part2(string[] lines)
        {
            long total = 0;
            foreach (string line in lines)
            {
                var (template, blocks) = ReadLine(line);
                (template, blocks) = ExpandProblem(template, blocks);
                total += AmountOfArrangements(template, blocks);
            }
            return total;
        }

        public (string template, List<int> blocks) ReadLine(string line)
        {
            var initialSplit = line.Split(' ');
            var blocks = initialSplit[1].Split(',').Select(x => int.Parse(x)).ToList();
            return (initialSplit[0], blocks);
        }

        public (string template, List<int> blocks) ExpandProblem(string template,  List<int> blocks, int multiplyBy = 5)
        {
            var newTemplate = string.Join('?', Enumerable.Range(0, multiplyBy).Select(_ => template));
            var newBlocks = new List<int>();
            for(int i = 0; i < multiplyBy; i++) { 
                newBlocks.AddRange(blocks);
            }
            return(newTemplate, newBlocks);
        }

        public long AmountOfArrangements(string template, IList<int> blocks)
        {
            var cacheKey = blocks.Aggregate(template, (a, block) => a + $",{block}");
            if (_cache.TryGetValue(cacheKey, out var result))
            {
                return result;
            }

            if (!blocks.Any())
            {
                return template.All(c => c != '#') ? 1 : 0;
            }

            int freeBlocks = template.Length - (blocks.Sum() + blocks.Count - 2);

            long totalArrangements = 0;
            var blockSize = blocks.First();
            for (int i = 0; i < freeBlocks; i++)
            {
                var beforeBlock = template.Substring(0, i);
                if(beforeBlock.Any(c => c == '#'))
                {
                    break;
                }
                var block = template.Substring(i, blockSize);
                var templateHasCharsLeft = i + blockSize < template.Length;
                var nextChar = templateHasCharsLeft ? template[i + blockSize] : '?';
                if (block.All(c => c != '.') && nextChar != '#')
                {
                    totalArrangements += AmountOfArrangements(template.Substring(i + blockSize + (templateHasCharsLeft ? 1 : 0)), blocks.Skip(1).ToList());
                }
            }
            _cache[cacheKey] = totalArrangements;
            return totalArrangements;
        }
    }
}
