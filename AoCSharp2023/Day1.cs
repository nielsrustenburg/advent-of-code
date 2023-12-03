using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoCSharp2023
{
    // Part 2 only as I did part 1 in F# with a FParsec
    internal class Day1
    {
        public Day1()
        {
            string[] lines = File.ReadAllLines("./inputs/1.txt");

            var result = lines.Aggregate(0, (a, b) => OuterDigitFinder(b) + a);
            Console.WriteLine(result);
        }

        int OuterDigitFinder(string word)
        {
            int maxDigitSize = 5;
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
            //Console.WriteLine(result);
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
