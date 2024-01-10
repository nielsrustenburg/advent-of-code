using Shared.Datastructures;

namespace Shared.AStar
{
    public static class Search<T, N> where N : SearchNode
                                    where T : IPriorityQueue<N>,
                                    new()
    {
        public static SearchNode Execute(N rootNode)
        {
            var expanded = new Dictionary<SearchNode, int>
            {
                { rootNode, rootNode.LowerBoundCost }
            };

            var prioQueue = new T();
            prioQueue.Enqueue(rootNode);

            while (!prioQueue.IsEmpty())
            {
                var currentNode = prioQueue.Dequeue();
                if (currentNode.IsAtTarget()) return currentNode;

                var haveNotFoundCheaper = expanded[currentNode] >= currentNode.LowerBoundCost;
                if (haveNotFoundCheaper)
                {
                    var neighbours = currentNode.GetNeighbours();
                    foreach (var neighbour in neighbours)
                    {
                        int bestKnownCost;
                        var alreadyFound = expanded.TryGetValue(neighbour, out bestKnownCost);
                        if (!alreadyFound || bestKnownCost > neighbour.LowerBoundCost)
                        {
                            prioQueue.Enqueue(neighbour as N);
                            expanded[neighbour] = neighbour.LowerBoundCost;
                        }
                    }
                }
            }
            return null;
        }
    }
}
