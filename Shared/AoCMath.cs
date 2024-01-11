using System.Numerics;

namespace Shared
{
    public static class AoCMath
    {
        //Avoid use if a is never negative, significant performance hit if used often compared to using %
        public static int Mod(int a, int modulus)
        {
            int remainder = a % modulus;
            return remainder + (remainder < 0 ? modulus : 0);
        }

        public static BigInteger Mod(BigInteger a, BigInteger modulus)
        {
            BigInteger remainder = a % modulus;
            return remainder + (remainder < 0 ? modulus : 0);
        }

        public static int ManhattanDistance((int x, int y) pointA, (int x, int y) pointB)
        {
            return Math.Abs(pointA.x - pointB.x) + Math.Abs(pointA.y - pointB.y);
        }

        public static BigInteger GCD(BigInteger a, BigInteger b)
        {
            if (a < 0) return GCD(a * -1, b);
            if (b < 0) return GCD(a, b * -1);
            if (a == 0) return b;
            if (b == 0) return a;

            BigInteger greater, smaller;
            if (a >= b)
            {
                greater = a;
                smaller = b;
            }
            else
            {
                greater = b;
                smaller = a;
            }
            return GCD(smaller, Mod(greater, smaller));
        }

        public static BigInteger LCM(BigInteger a, BigInteger b)
        {
            return a * b / GCD(a, b);
        }

        public static BigInteger LCM(IEnumerable<BigInteger> nums)
        {
            BigInteger result = nums.First();
            foreach (var num in nums.Skip(1))
            {
                result = LCM(result, num);
            }
            return result;
        }

        public static BigInteger ModInv(BigInteger a, BigInteger b)
        {
            (BigInteger aInv, BigInteger _) = ModInvHelper(a, b, (1, 0), (0, 1));
            if (a >= b)
            {
                ModInvHelper(a, b, (1, 0), (0, 1));
            }
            else
            {
                ModInvHelper(b, a, (0, 1), (1, 0));
            }
            return Mod(aInv, b);
        }

        public static BigInteger Shoelace(IList<(BigInteger X, BigInteger Y)> sortedCoordinates)
        {
            BigInteger total = 0;
            for (int i = 0; i < sortedCoordinates.Count; i++)
            {
                var nextIndex = (i + 1) % sortedCoordinates.Count;
                var plus = sortedCoordinates[i].X * sortedCoordinates[nextIndex].Y;
                var minus = sortedCoordinates[nextIndex].X * sortedCoordinates[i].Y;
                total = total + plus - minus;
            }
            return total > 0 ? total / 2 : -total / 2;
        }

        public static BigInteger PicksFindI(BigInteger A,  BigInteger B)
        {
            return (A + 1) - (B / 2);
        }

        private static (BigInteger, BigInteger) ModInvHelper(BigInteger greater, BigInteger smaller, (BigInteger a, BigInteger b) g, (BigInteger a, BigInteger b) s)
        {
            //Find the modular inverse A^-1 for A mod B
            if (smaller == 0) throw new Exception("No Modular Inverse Possible");

            BigInteger x = greater / smaller;
            BigInteger remainder = Mod(greater, smaller);
            if (smaller == 1)
            {
                return s;
            }
            else
            {
                return ModInvHelper(smaller, remainder, s, (g.a - x * s.a, g.b - x * s.b));
            }
        }
    }
}
