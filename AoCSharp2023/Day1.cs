namespace AoCSharp2023
{
    internal class Day1
    {
        public Day1()
        {
            string[] lines = File.ReadAllLines("./inputs/1.txt");

            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        int Part1(string[] lines)
        {
            return lines.Aggregate(0, (a, b) => OuterDigitFinder(b) + a);
        }

        int Part2(string[] lines)
        {
            return lines.Aggregate(0, (a, b) => OuterDigitOrWordFinder(b) + a);
        }

        int OuterDigitFinder(string word)
        {
            var numericValues = word.Select(c => char.GetNumericValue(c));
            var firstDigit = (int) numericValues.First(c => c > -1);
            var lastDigit = (int) numericValues.Last(c => c > -1);
            return firstDigit * 10 + lastDigit;
        }

        int OuterDigitOrWordFinder(string word)
        {
            int? firstDigit = null;
            int? lastDigit = null;
            for (int i = 0; i < word.Length; i++)
            {
                int subSize = 0;
                while (firstDigit == null && subSize <= 5 && i + subSize < word.Length)
                {
                    firstDigit = TextToInt(word.Substring(i, subSize));
                    subSize++;
                }

                int bSubSize = 0;
                while (lastDigit == null && bSubSize <= 5 && word.Length - (i + bSubSize) > 0)
                {
                    lastDigit = TextToInt(word.Substring(word.Length - (i + bSubSize), bSubSize));
                    bSubSize++;
                }

                if (firstDigit.HasValue && lastDigit.HasValue)
                {
                    break;
                }
            }
            if (firstDigit == null)
            {
                firstDigit = lastDigit;
            }

            if (lastDigit == null)
            {
                lastDigit = firstDigit;
            }

            var result = firstDigit.Value * 10 + lastDigit.Value;
            return result;
        }

        int? TextToInt(string word)
        {
            return word switch
            {
                "1" => 1,
                "2" => 2,
                "3" => 3,
                "4" => 4,
                "5" => 5,
                "6" => 6,
                "7" => 7,
                "8" => 8,
                "9" => 9,
                "one" => 1,
                "two" => 2,
                "three" => 3,
                "four" => 4,
                "five" => 5,
                "six" => 6,
                "seven" => 7,
                "eight" => 8,
                "nine" => 9,
                _ => null
            };
        }
    }
}
