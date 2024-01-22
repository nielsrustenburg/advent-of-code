using Shared;
using Shared.AStar;
using Shared.Datastructures;

namespace AoCSharp2023
{
    internal class Day17
    {
        public Day17()
        {
            string[] lines = File.ReadAllLines("./inputs/17.txt");
            var grid = GridHelper.CreateNumericalGrid(lines);
            Console.WriteLine(Part1(grid));
            Console.WriteLine(Part2(grid));
        }

        public int Part1(int[,] grid)
        {
            var initialNode = new Route(0, 0, grid);
            var result = Search<HeapPriorityQueue<Route>, Route>.Execute(initialNode);
            return result.Cost;
        }

        public int Part2(int[,] grid)
        {
            var initialNode = new Route2(0, 0, grid);
            var result = Search<HeapPriorityQueue<Route2>, Route2>.Execute(initialNode);
            return result.Cost;
        }
    }

    internal class Route : SearchNode, IComparable<Route>
    {
        public (int X, int Y) Current { get; set; }
        public Direction? Orientation { get; set; }
        public int ConsecutiveStraightSteps { get; set; }
        public override int Cost { get; init; }
        public int[,] Grid { get; set; }

        public Route(int xOrigin, int yOrigin, int[,] grid)
        {
            Current = (xOrigin, yOrigin);
            Orientation = null;
            ConsecutiveStraightSteps = 0;
            Cost = 0;
            Grid = grid;
        }

        public Route(Route previous, Direction direction)
        {
            // Assume the move is checked for validity prior to calling this constructor
            Grid = previous.Grid;
            Current = direction.StepInDirection(previous.Current.X, previous.Current.Y);
            Cost = previous.Cost + Grid[Current.X, Current.Y];
            Orientation = direction;
            if (previous.Orientation == direction)
            {
                ConsecutiveStraightSteps = previous.ConsecutiveStraightSteps + 1;
            }
            else
            {
                ConsecutiveStraightSteps = 1;
            }
        }

        public virtual bool ValidMove(Direction direction)
        {
            var newCoords = direction.StepInDirection(Current.X, Current.Y);
            if (!GridHelper.IsWithinGrid(Grid, newCoords.X, newCoords.Y))
            {
                return false;
            }
            if (Orientation is Direction d && d.GetOpposite() == direction)
            {
                return false;
            }

            if (direction == Orientation)
            {
                return ConsecutiveStraightSteps < 3;
            }
            return true;
        }

        public override bool IsAtTarget()
        {
            return Current.X == Grid.GetLength(0) - 1 && Current.Y == Grid.GetLength(1) - 1;
        }

        public override int GetHeuristicCost()
        {
            return Grid.GetLength(0) - 1 - Current.X + Grid.GetLength(1) - 1 - Current.Y; // At least the amount of tiles distance from the target
        }

        public override SearchNode[] GetNeighbours()
        {
            return new List<Direction> { Direction.Up, Direction.Right, Direction.Down, Direction.Left }
                .Where(d => ValidMove(d))
                .Select(d => new Route(this, d))
                .ToArray();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj == null) return false;
            var other = obj as Route;
            if (other.Current.X != Current.X || other.Current.Y != Current.Y)
            {
                return false;
            }
            if (Orientation != other.Orientation)
            {
                return false;
            }
            if (ConsecutiveStraightSteps != other.ConsecutiveStraightSteps)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 179;
                hash = hash * 167 + Current.X;
                hash = hash * 167 + Current.Y;
                hash = hash * 167 + Orientation != null ? (int)Orientation : 5;
                hash = hash * 167 + ConsecutiveStraightSteps;
                return hash;
            }
        }

        public override string ToString()
        {
            return $"({Current.X},{Current.Y}) {Orientation}:{ConsecutiveStraightSteps}";
        }

        public int CompareTo(Route other)
        {
            if (other == null) return -1;
            if (LowerBoundCost != other.LowerBoundCost)
            {
                return LowerBoundCost.CompareTo(other.LowerBoundCost);
            }
            return GetHeuristicCost().CompareTo(other.GetHeuristicCost());
        }
    }

    internal class Route2 : Route
    {
        public Route2(int xOrigin, int yOrigin, int[,] grid) : base(xOrigin, yOrigin, grid)
        {
        }
        public Route2(Route previous, Direction direction) : base(previous, direction)
        {
        }

        public override bool IsAtTarget()
        {
            return Current.X == Grid.GetLength(0) - 1 && Current.Y == Grid.GetLength(1) - 1 && ConsecutiveStraightSteps > 3;
        }

        public override SearchNode[] GetNeighbours()
        {
            return new List<Direction> { Direction.Up, Direction.Right, Direction.Down, Direction.Left }
                .Where(d => ValidMove(d))
                .Select(d => new Route2(this, d))
                .ToArray();
        }

        public override bool ValidMove(Direction direction)
        {
            var newCoords = direction.StepInDirection(Current.X, Current.Y);
            if (!GridHelper.IsWithinGrid(Grid, newCoords.X, newCoords.Y))
            {
                return false;
            }
            
            if(Orientation == null)
            {
                return true;
            }
            
            if (Orientation is Direction d && d.GetOpposite() == direction)
            {
                return false;
            }

            if (direction == Orientation)
            {
                return ConsecutiveStraightSteps < 10;
            }

            if (direction != Orientation)
            {
                return ConsecutiveStraightSteps > 3;
            }

            return true;
        }
    }
}
