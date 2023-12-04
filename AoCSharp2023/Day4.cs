using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoCSharp2023
{
    internal class Day4
    {

        public Day4()
        {
            string[] lines = File.ReadAllLines("./inputs/4.txt");

            Console.WriteLine(EvaluateAllCards(lines));
            Console.WriteLine(EvaluateAllCards2(lines));
        }

        (HashSet<int> winning, List<int> numbersYouHave) GetScratchCard(string line)
        {
            var nums = line.Split(": ")[1];
            var splitByLine = nums.Split(" | ");
            var winning = splitByLine[0].Split(' ').Where(x => x != "").Select(x => int.Parse(x)).ToHashSet();
            var numbersYouHave = splitByLine[1].Split(' ').Where(x => x != "").Select(x => int.Parse(x)).ToList();
            return (winning, numbersYouHave);
        }

        int GetPoints(HashSet<int> winning, IList<int> numbersYouHave)
        {
            var amountOfWinners = numbersYouHave.Where(x => winning.Contains(x)).ToList();
            if (amountOfWinners.Any())
            {
                return 1 << (amountOfWinners.Count - 1);
            }
            return 0;
        }

        int EvaluateAllCards(IList<string> cards)
        {
            List<int> points = new List<int>();
            foreach (var card in cards)
            {
                var (win, have) = GetScratchCard(card);
                points.Add(GetPoints(win, have));
            }
            return points.Sum();
        }

        int EvaluateAllCards2(IList<string> cards)
        {
            List<int> copies = cards.Select(_ => 1).ToList();
            for(int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                var (win, have) = GetScratchCard(card);
                var amountOfWinners = have.Where(x => win.Contains(x)).ToList();
                for (int j = 0; j < amountOfWinners.Count && i+j+1 < copies.Count; j++)
                {
                    copies[i + j+1] += copies[i];
                }
            }
            return copies.Sum();
        }
    }
}
