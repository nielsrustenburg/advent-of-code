namespace Shared
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    public static class DirectionExtensions
    {
        public static (int X, int Y) StepInDirection(this Direction direction, int x, int y)
        {
            return (direction) switch
            {
                Direction.Up => (x, y - 1),
                Direction.Right => (x + 1, y),
                Direction.Down => (x, y + 1),
                Direction.Left => (x - 1, y),
                _ => throw new NotImplementedException("Should never occur"),
            };
        }

        public static Direction GetOpposite(this Direction direction)
        {
            return direction switch { 
                Direction.Up => Direction.Down, 
                Direction.Right => Direction.Left, 
                Direction.Down => Direction.Up, 
                Direction.Left => Direction.Right,
                _ => throw new NotImplementedException("Should never occur")
            };
        }
    }
}
