namespace AoCSharp2023
{
    internal class Day2
    {
        public Day2()
        {
            string[] lines = File.ReadAllLines("./inputs/2.txt");

            var result = Part1(lines);
            Console.WriteLine(result);

            var result2 = Part2(lines);
            Console.WriteLine(result2);
        }

        public int Part1(IList<string> games)
        {
            var constraints = new Dictionary<string, int>() { { "red", 12 }, { "green", 13 }, { "blue", 14 } };

            var legalGames = new List<int>();
            for (int i = 0; i < games.Count; i++)
            {
                var relevantPart = games[i].Split(": ")[1];
                var rounds = relevantPart.Split("; ");

                bool legal = true;
                foreach (var round in rounds)
                {
                    var amountsAndColours = round.Split(", ").ToDictionary(ac => ac.Split(' ')[1], ac => Int32.Parse(ac.Split(' ')[0]));

                    foreach (var (colour, amount) in amountsAndColours)
                    {
                        if (constraints[colour] < amount)
                        {
                            legal = false;
                            break;
                        }
                    }

                    if (!legal)
                    {
                        break;
                    }
                }

                if (legal)
                {
                    legalGames.Add(i + 1);
                }
            }

            return legalGames.Sum();
        }

        public int Part2(IList<string> games)
        {
            var powers = new List<int>();
            for (int i = 0; i < games.Count; i++)
            {
                var relevantPart = games[i].Split(": ")[1];
                var rounds = relevantPart.Split("; ");

                var minimumAmountsPerColour = new Dictionary<string, int>() { { "red", 0 }, { "green", 0 }, { "blue", 0 } };
                foreach (var round in rounds)
                {
                    var amountsAndColours = round.Split(", ").ToDictionary(ac => ac.Split(' ')[1], ac => Int32.Parse(ac.Split(' ')[0]));

                    foreach (var (colour, amount) in amountsAndColours)
                    {
                        if (minimumAmountsPerColour[colour] < amount)
                        {
                            minimumAmountsPerColour[colour] = amount;
                        }
                    }
                }
                powers.Add(minimumAmountsPerColour.Values.Aggregate(1, (a,b) => a*b));
            }

            return powers.Sum();
        }
    }
}
