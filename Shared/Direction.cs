using System.Numerics;

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
        public static (int X, int Y) StepInDirection(this Direction direction, int x, int y, int stepSize = 1)
        {
            return (direction) switch
            {
                Direction.Up => (x, y - stepSize),
                Direction.Right => (x + stepSize, y),
                Direction.Down => (x, y + stepSize),
                Direction.Left => (x - stepSize, y),
                _ => throw new NotImplementedException("Should never occur"),
            };
        }

        public static (BigInteger X, BigInteger Y) StepInDirection(this Direction direction, BigInteger x, BigInteger y, BigInteger stepSize)
        {
            return (direction) switch
            {
                Direction.Up => (x, y - stepSize),
                Direction.Right => (x + stepSize, y),
                Direction.Down => (x, y + stepSize),
                Direction.Left => (x - stepSize, y),
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
