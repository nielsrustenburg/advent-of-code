using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoCSharp2023
{
    internal class Day15
    {
        public Day15()
        {
            string[] lines = File.ReadAllLines("./inputs/15.txt");
            Console.WriteLine(Part1(lines[0]));
            Console.WriteLine(Part2(lines[0]));
        }

        long Part1(string line)
        {
            var allWords = line.Split(',');
            return allWords.Select(x => new HASHString(x).GetHashCode()).Sum();
        }

        long Part2(string line)
        {
            var allWords = line.Split(',');
            var boxes = new Dictionary<int, List<(string label, int lensStrength)>>();
            foreach(var word in allWords)
            {
                if(word.Last() == '-')
                {
                    var label = word.Substring(0, word.Length - 1);
                    var hashCode = new HASHString(label).GetHashCode();
                    if (boxes.TryGetValue(hashCode, out var boxContents))
                    {
                        var index = boxContents.FindIndex(a => a.label == label);
                        if(index >= 0)
                        {
                            boxContents.RemoveAt(index);
                        }
                    }
                } else
                {
                    var wordSplit = word.Split('=');
                    var label = wordSplit[0];
                    var focalLens = int.Parse(wordSplit[1]);
                    var hashCode = new HASHString(label).GetHashCode();
                    if (!boxes.TryGetValue(hashCode, out var boxContents))
                    {
                        boxContents = new List<(string label, int lensStrength)>();
                        boxes[hashCode] = boxContents;
                    }
                    var index = boxContents.FindIndex(a => a.label == label);
                    if (index >= 0)
                    {
                        boxContents[index] = (label, focalLens);
                    } else
                    {
                        boxContents.Add((label, focalLens));
                    }
                }
            }

            long total = 0;
            foreach(var (boxId, box) in boxes)
            {
                for(int i = 0; i < box.Count; i++)
                {
                    total += (boxId + 1) * (i + 1) * box[i].lensStrength;
                }
            }

            return total;
        }
    }

    internal class HASHString
    {
        public string Value { get; init; }
        public HASHString(string s)
        {
            Value = s;
        }
        public new bool Equals(object x, object y)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            int currentValue = 0;
            foreach (var c in Value)
            {
                currentValue += (int)c;
                currentValue *= 17;
                currentValue = currentValue % 256;
            }
            return currentValue;
        }
    }
}
