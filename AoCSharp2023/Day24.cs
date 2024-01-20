using System.Net.WebSockets;
using System.Numerics;

namespace AoCSharp2023
{
    internal class Day24
    {
        public Day24()
        {
            string[] lines = File.ReadAllLines("./inputs/24.txt");
            var hailStones = ParseHailStones(lines);
            Console.WriteLine(Part1(hailStones));
            Console.WriteLine(Part2(hailStones));
        }

        int Part1(List<HailStone> hailStones)
        {
            var intersectCount = 0;
            for (int i = 0; i < hailStones.Count - 1; i++)
            {
                var hailStone1 = hailStones[i];
                for (int j = i + 1; j < hailStones.Count; j++)
                {
                    var hailStone2 = hailStones[j];
                    //intersectCount = hailStone1.XYIntersection(hailStone2, 7, 27, true) ? intersectCount + 1 : intersectCount;
                    intersectCount = hailStone1.XYTestAreaIntersection(hailStone2, 200000000000000, 400000000000000, false) ? intersectCount + 1 : intersectCount;
                }
            }

            return intersectCount;
        }


        BigInteger Part2(List<HailStone> hailStones)
        {
            // I stole the following insights from reddit as I just ended up with some equations that I can't solve without a 3rd party solver..
            // Stolen idea and some code from https://www.reddit.com/r/adventofcode/comments/18pnycy/comment/kes57qm/?utm_source=share&utm_medium=web2x&context=3
            // Did slightly improve it/write some parts more clearly I think.

            // Insight: for any line to intersect in 3D space is unlikely, if we find one that intersects with 3 hailstones we probably have our solution
            // Insight 2: a rock with a velocity (a,b) hitting a hailstone with velocity (c,d) is the same as a stationary rock velocity (0,0) hitting the hailstone with velocity (c-a, d-b)
            // So we can rewrite hailstones to have their velocity be modified by the velocity of the rock
            // These modified hailstones must both intersect with the stationary rock, so their intersection MUST be the location of the stationary rock.
            // Also we know there must be an intersection for the modified hailstones, as the assignment indicates that there must be a rock that can intersect all hailstones
            var maxVel = 500;
            for (int x = -maxVel; x < maxVel; x++)
            {
                for (int y = -maxVel; y < maxVel; y++)
                {
                    var intersection1 = Intersect(hailStones[0], hailStones[1], (-x, -y));
                    var intersection2 = Intersect(hailStones[0], hailStones[2], (-x,-y));
                    if (intersection1.intersects && intersection1.pos == intersection2.pos)
                    {
                        for (int z = -maxVel; z < maxVel; z++)
                        {
                            var z0 = hailStones[0].Position.Z + (intersection1.t1 * (hailStones[0].Velocity.Z - z));
                            var z1 = hailStones[1].Position.Z + (intersection1.t2 * (hailStones[1].Velocity.Z - z));
                            var z2 = hailStones[2].Position.Z + (intersection2.t2 * (hailStones[2].Velocity.Z - z));
                            if (z0 == z1 && z0 == z2)
                            {
                                // Found a rock that intersects with 3 hailstones, assume it's the final answer
                                var rockPosition = (X: (long)intersection1.pos.X, Y: (long)intersection1.pos.Y, Z: z0);
                                return (BigInteger)rockPosition.X + rockPosition.Y + rockPosition.Z;
                            }
                        }
                    }
                }
            }
            throw new Exception("Did not find a solution within the specified velocity range");
        }

        List<HailStone> ParseHailStones(string[] lines)
        {
            var hailStones = new List<HailStone>();
            foreach (var line in lines)
            {
                var s1 = line.Split(" @ ");
                var position = s1[0].Split(", ").Select(n => long.Parse(n)).ToArray();
                var velocity = s1[1].Split(", ").Select(n => int.Parse(n)).ToArray();
                var hailStone = new HailStone((position[0], position[1], position[2]), (velocity[0], velocity[1], velocity[2]));
                hailStones.Add(hailStone);
            }
            return hailStones;
        }


        // https://math.stackexchange.com/a/3176648
        public (bool intersects, (BigInteger X, BigInteger Y) pos, BigInteger t1, BigInteger t2) Intersect(HailStone one, HailStone two, (int x, int y) offset)
        {
            var s1 = (Pos: (one.Position.X, one.Position.Y), Vel: (X: one.Velocity.X + offset.x, Y: one.Velocity.Y + offset.y));
            var s2 = (Pos: (two.Position.X, two.Position.Y), Vel: (X: two.Velocity.X + offset.x, Y: two.Velocity.Y + offset.y));

            //Determinant
            BigInteger D = (s1.Vel.X * -s2.Vel.Y) - (s1.Vel.Y * -s2.Vel.X);

            if (D == 0) return (false, (-1, -1), -1, -1);

            var pX = s2.Pos.X - s1.Pos.X;
            var pY = s2.Pos.Y - s1.Pos.Y;

            var Qx = (-s2.Vel.Y * pX) - (-s2.Vel.X * pY);
            var Qy = (s1.Vel.X * pY) - (s1.Vel.Y * pX);

            var t1 = Qx / D;
            var t2 = Qy / D; // Figure out why I need a minus here

            var x = (s1.Pos.X + t1 * s1.Vel.X);
            var y = (s1.Pos.Y + t1 * s1.Vel.Y);

            return (true, (x, y), t1, t2);
        }
    }

    class HailStone
    {
        public (long X, long Y, long Z) Position { get; set; }
        public (int X, int Y, int Z) Velocity { get; set; }

        public HailStone((long x, long y, long z) position, (int x, int y, int z) velocity)
        {
            Position = position;
            Velocity = velocity;
        }

        public (bool HasIntersection, (decimal X, decimal Y) Intersection) XYIntersection(HailStone other, (int X, int Y)? velocityOffset = null)
        {
            var offset = velocityOffset ?? (0, 0);
            // Add offset to velocities
            (int X, int Y) thisVel = (Velocity.X + offset.X, Velocity.Y + offset.Y);
            (int X, int Y) othVel = (other.Velocity.X + offset.X, other.Velocity.Y + offset.Y);

            if (AreParallel(thisVel, othVel))
            {
                return (false, (0, 0));
            }

            // Method does not work for vertical lines
            if (othVel.X == 0 || thisVel.X == 0)
            {
                if (othVel.Y == 0 || thisVel.Y == 0)
                {
                    return (false, (0, 0));
                }
                // Create two hailstones with X/Y flipped
                var thisFlip = new HailStone((Position.Y, Position.X, Position.Z), (Velocity.Y, Velocity.X, Velocity.Z));
                var othFlip = new HailStone((other.Position.Y, other.Position.X, other.Position.Z), (other.Velocity.Y, other.Velocity.X, other.Velocity.Z));
                return thisFlip.XYIntersection(othFlip, (offset.Y, offset.X));
            }

            // Move until X is equal
            var nanoSeconds = (Position.X - other.Position.X) / othVel.X;
            var tempY = other.Position.Y + (nanoSeconds * othVel.Y);

            // ax + c = bx + d
            var c = (decimal)Position.Y;
            var d = (decimal)tempY;
            var a = (decimal)thisVel.Y / thisVel.X;
            var b = (decimal)othVel.Y / othVel.X;

            var x = (d - c) / (a - b);

            var xAtIntersection = (decimal)Position.X + x;
            var yAtIntersection = (decimal)Position.Y + (thisVel.Y * (x / thisVel.X));
            return (true, (xAtIntersection, yAtIntersection));
        }

        public bool XYTestAreaIntersection(HailStone other, decimal minXY, decimal maxXY, bool verbose = false)
        {
            var intersection = XYIntersection(other);
            if (!intersection.HasIntersection)
            {
                return false;
            }

            var intersectionInsideTestArea = intersection.Intersection.X >= minXY && intersection.Intersection.X <= maxXY && intersection.Intersection.Y >= minXY && intersection.Intersection.Y <= maxXY;
            var intersectionInFuture =
                (intersection.Intersection.X - (decimal)Position.X) / Velocity.X > 0 &&
                (intersection.Intersection.Y - (decimal)Position.Y) / Velocity.Y > 0 &&
                (intersection.Intersection.X - (decimal)other.Position.X) / other.Velocity.X > 0 &&
                (intersection.Intersection.Y - (decimal)other.Position.Y) / other.Velocity.Y > 0;

            if (verbose)
            {
                Console.Write($"Intersection at (x={intersection.Intersection.X:0.###}, y={intersection.Intersection.Y:0.###})");
                if (!intersectionInsideTestArea)
                {
                    Console.Write(" REJECTED is outside test-area");
                }
                if (!intersectionInFuture)
                {
                    Console.Write("  REJECTED is in the past");
                }
                Console.WriteLine();
            }

            return intersectionInsideTestArea && intersectionInFuture;
        }

        bool AreParallel((int X, int Y) thisVel, (int X, int Y) otherVel)
        {
            return (decimal)thisVel.X * otherVel.Y == (decimal)otherVel.X * thisVel.Y;
        }
    }
}
