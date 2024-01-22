namespace AoCSharp2023
{
    internal class Day25
    {
        public Day25()
        {
            var lines = File.ReadAllLines("./inputs/25.txt");
            Console.WriteLine(Part1(lines));
        }

        int Part1(string[] lines)
        {
            (Dictionary<string, Component> nodes, List<(Component, Component)> edges) = CreateNodeGraph(lines);

            int result = KargersAlgorithm(nodes);
            return result;
        }

        // Tried some things myself first but everything I came up with either did not work or was not efficient enough to cough up a solution in a reasonable time
        // Remembered this problem from some graph-theory courses, after some googling for minimum cut algorithms I found Karger's algorithm
        // 1. pick a random edge in the graph (pick a random node + random edge on that node)
        // 2. contract the edge by combining the two nodes: a new node is created containing all edges of the two nodes, while the old nodes are removed
        // 3. repeat until only 2 nodes remain
        // Result is a partition of the nodes in the graph
        // From this we can check how many edges are between the two partitions to find the amount of cuts needed to create the partitioning
        // Running this algorithm multiple times is likely to eventually give the minimum cut,
        // Because we are way more likely to contract an edge that isn't part of the minimum cut
        // In this case, we even know what the minimum cut is: 3, so we can simply run the algorithm until we find that solution
        private int KargersAlgorithm(Dictionary<string, Component> nodes)
        {
            var bestFound = int.MaxValue;
            var rng = new Random();
            var clusterA = new HashSet<Component>();
            var clusterB = new HashSet<Component>();
            while (bestFound > 3)
            {
                var adjacencyList = nodes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Connected.Select(n => n.Name).ToHashSet());

                while (adjacencyList.Count > 2)
                {
                    var randomNodeIndex = rng.Next(adjacencyList.Count);
                    var node = adjacencyList.Keys.ElementAt(randomNodeIndex);
                    var edges = adjacencyList[node];
                    var randomEdgeIndex = rng.Next(edges.Count);
                    var otherNode = edges.AsEnumerable().ElementAt(randomEdgeIndex);
                    var contractedName = node + "," + otherNode;
                    adjacencyList.Add(contractedName, edges.Union(adjacencyList[otherNode]).ToHashSet());
                    adjacencyList[contractedName].Remove(otherNode);
                    adjacencyList[contractedName].Remove(node);
                    adjacencyList.Remove(node);
                    adjacencyList.Remove(otherNode);

                    foreach (var kvp in adjacencyList)
                    {
                        if (kvp.Value.Contains(node) || kvp.Value.Contains(otherNode))
                        {
                            kvp.Value.Remove(node);
                            kvp.Value.Remove(otherNode);
                            kvp.Value.Add(contractedName);
                        }
                    }
                }
                var blobs = adjacencyList.Keys.ToList();
                var clusterANames = blobs[0].Split(',');
                var clusterBNames = blobs[1].Split(',');

                clusterA = clusterANames.Select(name => nodes[name]).ToHashSet();
                clusterB = clusterBNames.Select(name => nodes[name]).ToHashSet();
                int edgesToOtherCluster = 0;
                foreach (var node in clusterA)
                {
                    foreach (var connected in node.Connected)
                    {
                        edgesToOtherCluster = clusterB.Contains(connected) ? edgesToOtherCluster + 1 : edgesToOtherCluster;
                    }
                    if(edgesToOtherCluster > bestFound)
                    {
                        break;
                    }
                }

                bestFound = edgesToOtherCluster < bestFound ? edgesToOtherCluster : bestFound;
            }

            return clusterA.Count * clusterB.Count;
        }

        private static (Dictionary<string, Component>, List<(Component, Component)>) CreateNodeGraph(string[] lines)
        {
            var nodes = new Dictionary<string, Component>();
            var edges = new List<(Component, Component)>();
            Component GetOrCreateNode(string name)
            {
                if (!nodes.TryGetValue(name, out var node))
                {
                    node = new Component(name);
                    nodes[name] = node;
                }
                return node;
            }

            foreach (var line in lines)
            {
                var leftRight = line.Split(": ");
                var rightNames = leftRight[1].Split(' ');

                var leftNode = GetOrCreateNode(leftRight[0]);
                foreach (var right in rightNames)
                {
                    var rightNode = GetOrCreateNode(right);
                    leftNode.AddConnection(rightNode);
                    if (leftNode.CompareTo(rightNode) < 0)
                    {
                        edges.Add((leftNode, rightNode));
                    }
                    else
                    {
                        edges.Add((rightNode, leftNode));
                    }
                }
            }

            return (nodes, edges);
        }
    }

    class Component : IEquatable<Component>, IComparable<Component>
    {
        public string Name { get; set; }
        public HashSet<Component> Connected { get; set; }
        public Component(string name)
        {
            Name = name;
            Connected = new HashSet<Component>();
        }

        public List<Component[]> GetTriangles()
        {
            var triangles = new List<Component[]>();
            if (Connected.Count < 2)
            {
                return triangles;
            }

            var connectList = Connected.ToList();
            for (int b = 0; b < Connected.Count - 1; b++)
            {
                for (int c = b + 1; c < Connected.Count; c++)
                {
                    if (connectList[b].Connected.Contains(connectList[c]))
                    {
                        var alphabetical = new List<Component>() { connectList[b], connectList[c] }.OrderBy(n => n.Name).ToArray();
                        triangles.Add(alphabetical);
                    }
                }
            }
            return triangles;
        }

        public void AddConnection(Component other)
        {
            Connected.Add(other);
            other.Connected.Add(this);
        }

        public bool Equals(Component other)
        {
            if (other == null) return false;

            return other.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name.ToString();
        }

        public int CompareTo(Component other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
