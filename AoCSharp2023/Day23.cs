using Shared;

namespace AoCSharp2023.Day23
{
    internal class Day23
    {
        public Day23()
        {
            string[] lines = File.ReadAllLines("./inputs/23.txt");
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        public int Part1(string[] lines)
        {
            var grid = CreateGrid(lines);
            var nodeCoordinates = FindIntersections(grid);
            nodeCoordinates.Insert(0, (1, 0)); // Add starting coordinates
            nodeCoordinates.Add((lines[0].Length - 2, lines.Length - 1)); // Add finish coordinates

            var nodes = nodeCoordinates.Select((n, i) => new Node(i, n.X, n.Y)).ToList();
            var nodesByCoordinates = nodes.ToDictionary(node => (node.X, node.Y), node => node);
            foreach (var node in nodes)
            {
                AddEdges(node, grid, nodesByCoordinates);
            }

            return FindLongestAcyclicPath(nodes[0], nodes.Last(), nodes);
        }

        public int Part2(string[] lines)
        {
            var grid = CreateGrid(lines);
            var nodeCoordinates = FindIntersections(grid);
            nodeCoordinates.Insert(0, (1, 0)); // Add starting coordinates
            nodeCoordinates.Add((lines[0].Length - 2, lines.Length - 1)); // Add finish coordinates

            var nodes = nodeCoordinates.Select((n, i) => new Node(i, n.X, n.Y)).ToList();
            var nodesByCoordinates = nodes.ToDictionary(node => (node.X, node.Y), node => node);
            foreach (var node in nodes)
            {
                AddEdges(node, grid, nodesByCoordinates, false);
            }

            return FindLongestAcyclicPath(nodes[0], nodes.Last(), nodes);
        }


        public void AddEdges(Node node, char[,] grid, Dictionary<(int X, int Y), Node> nodesByCoordinates, bool slopesAreDifficult = true)
        {
            var locations = new List<(int X, int Y, Direction? CameFrom)>
            {
                (node.X, node.Y, null)
            };
            int stepCount = 0;

            while (locations.Any())
            {
                var newLocations = new List<(int X, int Y, Direction? CameFrom)>();
                foreach (var location in locations)
                {
                    if (nodesByCoordinates.TryGetValue((location.X, location.Y), out var foundNode) && foundNode.Id != node.Id)
                    {
                        node.AddNeighbour(foundNode.Id, stepCount);
                    }
                    else
                    {
                        var nbs = GridHelper.FindNeighboursWithDirections(grid, location.X, location.Y);
                        foreach (var (dir, nbcoords) in nbs)
                        {
                            var neighbourChar = grid[nbcoords.x, nbcoords.y];
                            if (dir == location.CameFrom || neighbourChar == '#')
                            {
                                continue;
                            }

                            if (slopesAreDifficult && (
                               (neighbourChar == '>' && dir != Direction.Right) ||
                               (neighbourChar == '<' && dir != Direction.Left) ||
                               (neighbourChar == '^' && dir != Direction.Up) ||
                               (neighbourChar == 'v' && dir != Direction.Down)))
                            {
                                continue;
                            }

                            newLocations.Add((nbcoords.x, nbcoords.y, dir.GetOpposite()));
                        }
                    }
                }
                stepCount++;
                locations = newLocations;
            }
        }

        public int FindLongestAcyclicPath(Node start, Node end, List<Node> nodes)
        {
            var candidatePaths = new List<(List<int> Path, int Length)>
            {
                (new List<int> { start.Id }, 0)
            };

            var bestSoFar = int.MinValue;
            while (candidatePaths.Any())
            {
                var candidate = candidatePaths.Last();
                candidatePaths.RemoveAt(candidatePaths.Count - 1);
                var currentNode = candidate.Path.Last();
                if (currentNode == end.Id)
                {
                    bestSoFar = bestSoFar < candidate.Length ? candidate.Length : bestSoFar;
                }
                else
                {
                    foreach (var (nbId, dist) in nodes[currentNode].NeighbouringNodeIds)
                    {
                        if (!candidate.Path.Contains(nbId))
                        {
                            candidatePaths.Add((candidate.Path.Append(nbId).ToList(), candidate.Length + dist));
                        }
                    }
                }
            }

            return bestSoFar;
        }
        List<(int X, int Y)> FindIntersections(char[,] grid)
        {
            var intersections = new List<(int X, int Y)>();
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y] == '.')
                    {
                        var neighboursByDirection = GridHelper.FindNeighboursWithDirections(grid, x, y);
                        var notAWallNeighbours = neighboursByDirection.Select(kvp => kvp.Value).Select(coords => grid[coords.x, coords.y]).Where(c => c != '#');
                        if (notAWallNeighbours.Count() > 2)
                        {
                            intersections.Add((x, y));
                        }
                    }
                }
            }

            return intersections;
        }

        char[,] CreateGrid(string[] lines)
        {
            var grid = new char[lines[0].Length, lines.Length];
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    grid[x, y] = lines[y][x];
                }
            }
            return grid;
        }
    }

    class Node
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Dictionary<int, int> NeighbouringNodeIds { get; set; }
        public Node(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
            NeighbouringNodeIds = new Dictionary<int, int>();
        }

        public void AddNeighbour(int nbId, int distance)
        {
            NeighbouringNodeIds.Add(nbId, distance);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
