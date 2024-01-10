namespace Shared.AStar
{
    public abstract class SearchNode : IComparable<SearchNode>
    {
        public abstract int Cost { get; init; }
        public SearchNode parent;

        public abstract bool IsAtTarget();
        public abstract int GetHeuristicCost();
        public abstract SearchNode[] GetNeighbours();
        public override abstract bool Equals(object obj);
        public override abstract int GetHashCode();

        //public SearchNode(int cost)
        //{
        //    this.cost = cost;
        //}

        //public SearchNode(int cost, SearchNode parent) : this(cost)
        //{
        //    this.parent = parent;
        //}

        public int LowerBoundCost
        {
            get
            {
                return Cost + GetHeuristicCost();
            }
        }

        //public List<SearchNode> GetPath()
        //{
        //    if (parent == null) return new List<SearchNode>();
        //    var path = parent.GetPath();
        //    path.Add(this);
        //    return path;
        //}

        int IComparable<SearchNode>.CompareTo(SearchNode? other)
        {
            if (other == null) return -1;
            if (LowerBoundCost != other.LowerBoundCost) return LowerBoundCost.CompareTo(other.LowerBoundCost);
            return GetHeuristicCost().CompareTo(other.GetHeuristicCost());
        }
    }
}
