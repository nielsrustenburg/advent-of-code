using Shared;
using System.Numerics;

namespace AoCSharp2023
{
    internal class Day8
    {
        public Day8()
        {
            string[] lines = File.ReadAllLines("./inputs/8.txt");
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        public long Part1(string[] lines)
        {
            var (lrInstructions, nodes) = Parse(lines);
            var currentNode = nodes["AAA"];
            int steps = 0;
            while(currentNode.Name != "ZZZ")
            {
                var instruction = lrInstructions[steps % lrInstructions.Length];
                currentNode = instruction == 'L' ? currentNode.Left : currentNode.Right;
                steps++;
            }
            return steps;
        }

        public BigInteger Part2(string[] lines)
        {
            // Some classic AoC hidden properties:
            // Each will reach exactly 1 endstate
            // They will all only reach it on the first step of the cycle
            var (lrInstructions, nodes) = Parse(lines);
            var currentNodes = nodes.Values.Where(n => n.Name.Last() == 'A').ToList();
            var endStateTracker = currentNodes.Select(_ => new List<(int atStep, int atInstruction, string atEndNode)>()).ToList();
            int steps = 0;

            // Can keep it simple thanks to hidden properties..
            while (endStateTracker.Any(list => !list.Any()))
            {
                var instructionIndex = steps % lrInstructions.Length;
                var instruction = lrInstructions[instructionIndex];
                var newCurrentNodes = currentNodes.Select(n => instruction == 'L' ? n.Left : n.Right);
                steps++;
                currentNodes = newCurrentNodes.ToList();
                for (int i = 0; i < currentNodes.Count; i++)
                {
                    var node = currentNodes[i];
                    if (node.Name.Last() == 'Z')
                    {
                        endStateTracker[i].Add((steps, steps % lrInstructions.Length, node.Name));
                    }
                }
            }

            var timeForCyclesToAlign = AoCMath.LCM(endStateTracker.Select(l => (BigInteger) l[0].atStep));
            return timeForCyclesToAlign;
        }

        (string instructions, Dictionary<string, Node> nodes) Parse(string[] lines)
        {
            var leftRightInstructions = lines[0];
            var nodeLines = lines.Skip(2);
            var nodeLinesSplit = nodeLines.Select(l => l.Split(" = "));
            var nodes = nodeLinesSplit.Select(n => new Node(n[0])).ToDictionary(n => n.Name, n => n);
            foreach(var nSplit in nodeLinesSplit)
            {
                var node = nodes[nSplit[0]];
                var left = nSplit[1].Substring(1, 3);
                var right = nSplit[1].Substring(6, 3);
                node.Left = nodes[left];
                node.Right = nodes[right];
            }
            return (leftRightInstructions, nodes);
        }
    }

    internal class Node
    {
        public Node(string name)
        {
            Name = name;
        }
        public string Name { get; set;}
        public Node Left { get; set;}
        public Node Right { get; set;}
    }
}
