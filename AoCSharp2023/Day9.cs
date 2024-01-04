namespace AoCSharp2023
{
    internal class Day9
    {
        public Day9()
        {
            string[] lines = File.ReadAllLines("./inputs/9.txt");
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        public int Part1(string[] lines)
        {
            var total = 0;
            foreach (var line in lines)
            {
                var seq = Parse(line);
                total += seq.FindNext();
            }
            return total;
        }

        public int Part2(string[] lines)
        {
            var total = 0;
            foreach (var line in lines)
            {
                var seq = Parse2(line);
                total += seq.FindNext();
            }
            return total;
        }

        public PatternedSequence Parse(string line)
        {
            return new PatternedSequence(line.Split(' ').Select(i => int.Parse(i)));
        }

        public PatternedSequence Parse2(string line)
        {
            return new PatternedSequence(line.Split(' ').Select(i => int.Parse(i)).Reverse());
        }
    }

    internal class PatternedSequence
    {
        private List<int> _sequence;
        public List<int> Sequence { get { return _sequence.ToList(); } }
        public PatternedSequence(IEnumerable<int> numbers)
        {
            _sequence = numbers.ToList();
        }

        public int FindNext()
        {
            if (_sequence.All(i => i == 0))
            {
                return 0;
            }

            var differences = new List<int>();
            for (int i = 1; i < _sequence.Count; i++)
            {
                differences.Add(_sequence[i] - _sequence[i - 1]);
            }
            var subSequence = new PatternedSequence(differences);
            var add = subSequence.FindNext();
            return _sequence.Last() + add;
        }
    }
}
