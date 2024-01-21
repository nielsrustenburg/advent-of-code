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

        // after some googling for minimum cut algorithms I found 
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

        private int Attempt4(Dictionary<string, Component> nodes, List<(Component A, Component B)> edges)
        {
            // Give each edge a score based on how long that Path from A to B becomes after removing A-B
            // The idea is that the longer the path becomes, the more likely we have our cut-edge
            var scoredEdges = edges.Select(e => (e, Score: FindPath(e.A, e.B, new List<(Component, Component)> { e }).Count())).OrderByDescending(es => es.Score).ToList();

            for (int i = 0; i < scoredEdges.Count - 2; i++)
            {
                for (int j = i + 1; j < scoredEdges.Count - 1; j++)
                {
                    for (int k = j + 1; k < scoredEdges.Count; k++)
                    {
                        var path = FindPath(scoredEdges[k].e.A, scoredEdges[k].e.B, new List<(Component, Component)> { scoredEdges[i].e, scoredEdges[j].e, scoredEdges[k].e });
                        if (!path.Any())
                        {
                            System.Diagnostics.Debugger.Break();
                        }
                    }
                }
            }

            return 0;
        }

        // Pick 2 random nodes
        // Find path from A to B
        // Foreach edge in the path
        // --> If removing it, can I still get from A to B
        //   Foreach edge in the new path
        //   --> if removing it, can I still get from A to B
        //     Foreach edge in the newer path
        //     --> if removing it, can I still get from A to B
        //       YES: this cut isn't the right one
        //       NO: this cut is -the- partitioning cut

        // Works but on real input still no answer after 50 mins
        private int Attempt3(Dictionary<string, Component> nodesByName)
        {
            var rng = new Random();
            var nodes = nodesByName.Values.ToList();
            var edgesToCut = new List<(Component, Component)>();
            while (!edgesToCut.Any())
            {
                // Pick 2 random nodes
                var indexA = rng.Next(nodes.Count);
                var indexB = rng.Next(nodes.Count - 1);
                var nodeA = nodes[indexA];
                var nodeB = nodes.Take(indexA).Concat(nodes.Skip(indexA + 1)).ElementAt(indexB);
                // Try to path from A to B
                var p0 = FindPath(nodeA, nodeB, new List<(Component, Component)>());

                for (int i = 0; i < p0.Count - 1; i++)
                {
                    // Make edge i forbidden, see if a path can still be found
                    var forbidden1 = new List<(Component, Component)> { (p0[i], p0[i + 1]) };
                    var p1 = FindPath(nodeA, nodeB, forbidden1);
                    for (int j = 0; j < p1.Count - 1; j++)
                    {
                        var forbidden2 = forbidden1.Append((p1[j], p1[j + 1])).ToList();
                        var p2 = FindPath(nodeA, nodeB, forbidden2);
                        for (int k = 0; k < p2.Count - 1; k++)
                        {
                            var forbidden3 = forbidden2.Append((p2[k], p2[k + 1])).ToList();
                            var p3 = FindPath(nodeA, nodeB, forbidden3);

                            if (p3.Count == 0)
                            {
                                edgesToCut = forbidden3;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        private List<Component> FindPath(Component a, Component z, List<(Component, Component)> forbiddenEdges)
        {
            var candidatePaths = new List<List<Component>> { new List<Component> { a } };
            var target = z;

            while (candidatePaths.Any())
            {
                var currentPath = candidatePaths.First();
                candidatePaths.RemoveAt(0);
                var currentNode = currentPath.Last();

                var options = currentNode.Connected.Except(currentPath);
                foreach (var option in options)
                {
                    if (forbiddenEdges.Contains((currentNode, option)) || forbiddenEdges.Contains((option, currentNode)))
                    {
                        continue;
                    }
                    if (option == target)
                    {
                        return currentPath.Append(target).ToList();
                    }
                    else
                    {
                        candidatePaths.Add(currentPath.Append(option).ToList());
                    }
                }
            }

            return new List<Component>();

            //var path = new List<Component> { a };
            //var visited = new HashSet<Component> { a };
            //var options = current.Connected.Except(visited);
            //if(options.Contains(target))
            //{
            //    return (true, path.Append(target).ToList());
            //}
            //if (options.Any())
            //{

            //}
            //else
            //{
            //    return (false, new List<Component>());
            //}
        }


        private int Attempt2(Dictionary<string, Component> nodes)
        {
            // Triangle: A-B && A-C && B-C 
            // Wrong assumption --> double triangle means they must be together
            // it's extremely likely, but not guaranteed, also actual input barely contains double triangles
            var subClusters = new List<HashSet<Component>>();
            foreach (var (name, node) in nodes)
            {
                var triangles = node.GetTriangles();
                var triangleCount = node.Connected.ToDictionary(n => n, _ => 0);
                foreach (var triangle in triangles)
                {
                    foreach (var tn in triangle)
                    {
                        triangleCount[tn]++;
                    }
                }

                var doubleTriangles = triangleCount.Where(kvp => kvp.Value > 1).Select(kvp => kvp.Key).ToList();
                foreach (var dtriangle in doubleTriangles)
                {
                    var relevantSubClusters = subClusters.Where(c => c.Contains(node) || c.Contains(dtriangle)).ToList();
                    if (!relevantSubClusters.Any())
                    {
                        var newCluster = new HashSet<Component>();
                        relevantSubClusters.Add(newCluster);
                        subClusters.Add(newCluster);
                    }

                    foreach (var cluster in relevantSubClusters)
                    {
                        cluster.Add(node);
                        cluster.Add(dtriangle);
                    }
                }
            }

            var distinctSubclusters = new List<HashSet<Component>>();
            while (subClusters.Any())
            {
                var current = subClusters[0];
                var removeList = new List<int>();
                for (int i = 1; i < subClusters.Count; i++)
                {
                    if (current.Intersect(subClusters[i]).Any())
                    {
                        current.UnionWith(subClusters[i]);
                        removeList.Add(i);
                    }
                }

                if (removeList.Any())
                {
                    foreach (var remove in removeList.OrderByDescending(c => c))
                    {
                        subClusters.RemoveAt(remove);
                    }
                }
                else
                {
                    distinctSubclusters.Add(current);
                    subClusters.RemoveAt(0);
                }
            }

            return 0;
        }

        private int Attempt1(Dictionary<string, Component> nodes)
        {
            var clusterA = new HashSet<Component>();
            var clusterB = new HashSet<Component>();

            foreach (var (_, node) in nodes)
            {
                var rng = new Random();

                if (rng.NextDouble() >= 0.5)
                {
                    clusterB.Add(node);
                }
                else
                {
                    clusterA.Add(node);
                }
            }

            int connectionsToOtherCluster = 0;
            foreach (var aNode in clusterA)
            {
                connectionsToOtherCluster += aNode.Connected.Count(n => clusterB.Contains(n));
            }

            while (connectionsToOtherCluster > 3)
            {
                var candidateA = clusterA.Select(n => (n, Count: CountConnectionsToCluster(n, clusterA, clusterB))).Select(nc => (Node: nc.n, Delta: nc.Count.B - nc.Count.A)).MaxBy(nd => nd.Delta);
                var candidateB = clusterB.Select(n => (n, Count: CountConnectionsToCluster(n, clusterA, clusterB))).Select(nc => (Node: nc.n, Delta: nc.Count.A - nc.Count.B)).MaxBy(nd => nd.Delta);

                var aPositive = candidateA.Delta > 0;
                var bPositive = candidateB.Delta > 0;

                bool aIsLessThanHalfB = clusterA.Count * 2 < clusterB.Count;
                bool bIsLessThanHalfA = clusterB.Count * 2 < clusterA.Count;

                bool moveA = !aIsLessThanHalfB && (bIsLessThanHalfA || (clusterA.Count >= clusterB.Count && aPositive) || (aPositive && !bPositive));

                if (connectionsToOtherCluster < 200)
                {
                    System.Diagnostics.Debugger.Break();
                }

                if (moveA)
                {
                    clusterA.Remove(candidateA.Node);
                    clusterB.Add(candidateA.Node);
                    connectionsToOtherCluster -= candidateA.Delta;
                }
                else
                {
                    clusterB.Remove(candidateB.Node);
                    clusterA.Add(candidateB.Node);
                    connectionsToOtherCluster -= candidateB.Delta;
                }
            }

            var result = clusterA.Count * clusterB.Count;
            return result;
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

        (int A, int B) CountConnectionsToCluster(Component node, HashSet<Component> a, HashSet<Component> b)
        {
            var aCount = node.Connected.Count(node => a.Contains(node));
            var bCount = node.Connected.Count(node => b.Contains(node));
            return (aCount, bCount);
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
