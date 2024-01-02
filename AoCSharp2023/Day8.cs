namespace AoCSharp2023
{
    internal class Day8
    {
        public Day8()
        {
            string[] lines = File.ReadAllLines("./inputs/8.txt");
            //Console.WriteLine(Part1(lines));
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

        public long Part2(string[] lines)
        {
            // TODO: too slow without some form of cycle-detection
            // Bit too ill at the moment to work something out
            // Idea is for each node keep track of when they hit an end-node & what instruction-index they're on, if both ever match we've found a cycle, once we've found the cycles for all nodes we can calculate how long before their cycles align.
            var (lrInstructions, nodes) = Parse(lines);
            var currentNodes = nodes.Values.Where(n => n.Name.Last() == 'A').ToList();
            int steps = 0;
            while (currentNodes.Any(n => n.Name.Last() != 'Z'))
            {
                var instruction = lrInstructions[steps % lrInstructions.Length];
                var newCurrentNodes = currentNodes.Select(n => instruction == 'L' ? n.Left : n.Right);
                steps++;
                currentNodes = newCurrentNodes.ToList();
            }
            return steps;
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
