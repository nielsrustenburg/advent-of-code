using Shared.Datastructures;

namespace AoCSharp2023
{
    internal class Day22
    {
        public Day22()
        {
            string[] lines = File.ReadAllLines("./inputs/22.txt");
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        public int Part1(string[] lines)
        {
            var bricks = GetBricks(lines);
            LetBricksFall(bricks);
            (Dictionary<int, List<int>> supportsBricks, Dictionary<int, List<int>> isSupportedByBricks) = CreateBrickSupportGraphs(bricks);

            List<int> disintegrateCandidates = new List<int>();
            for (int i = 0; i < bricks.Count; i++)
            {
                if (supportsBricks[i].All(brick => isSupportedByBricks[brick].Count > 1))
                {
                    disintegrateCandidates.Add(i);
                }
            }
            return disintegrateCandidates.Count;
        }

        public int Part2(string[] lines)
        {
            var bricks = GetBricks(lines);
            LetBricksFall(bricks);
            (Dictionary<int, List<int>> supportsBricks, Dictionary<int, List<int>> isSupportedByBricks) = CreateBrickSupportGraphs(bricks);

            // These candidates are the complement of the candidates from Part 1
            List<int> chainDisintegrationCandidates = new List<int>();
            for (int i = 0; i < bricks.Count; i++)
            {
                if (!supportsBricks[i].All(brick => isSupportedByBricks[brick].Count > 1))
                {
                    chainDisintegrationCandidates.Add(i);
                }
            }

            var results = chainDisintegrationCandidates.Select(candidate => ChainDisintegration(candidate, supportsBricks, isSupportedByBricks, bricks));

            return results.Sum();
        }

        List<Brick> GetBricks(string[] lines)
        {
            List<Brick> bricks = new List<Brick>();
            for (int i = 0; i < lines.Length; i++)
            {
                var ab = lines[i].Split('~');
                var a = ab[0].Split(',').Select(coord => int.Parse(coord)).ToArray();
                var b = ab[1].Split(',').Select(coord => int.Parse(coord)).ToArray();


                var brick = new Brick((a[0], a[1], a[2]), (b[0], b[1], b[2]), i);
                bricks.Add(brick);
            }

            return bricks;
        }

        public void LetBricksFall(List<Brick> bricks)
        {
            var bricksOrderedByZLevel = bricks.OrderBy(b => b.Z.A).ToList();
            var bricksBelow = new List<Brick>();

            foreach (var brick in bricksOrderedByZLevel)
            {
                bool stable = brick.Z.A == 0 || bricksBelow.Any(b => brick.SupportedByOther(b));
                while (!stable)
                {
                    brick.Z = (brick.Z.A - 1, brick.Z.B - 1);
                    stable = brick.Z.A == 0 || bricksBelow.Any(b => brick.SupportedByOther(b));
                }
                bricksBelow.Add(brick);
            }
        }

        public (Dictionary<int, List<int>> SupportRelations, Dictionary<int, List<int>> IsSupportedByRelations) CreateBrickSupportGraphs(List<Brick> bricks)
        {
            var supportsBricks = new Dictionary<int, List<int>>();
            var isSupportedByBricks = new Dictionary<int, List<int>>();

            // Initialize empty lists so we don't need to use TryGetValue
            for (int i = 0; i < bricks.Count; i++)
            {
                supportsBricks.Add(i, new List<int>());
                isSupportedByBricks.Add(i, new List<int>());
            }

            for (int i = 0; i < bricks.Count - 1; i++)
            {
                for (int j = i + 1; j < bricks.Count; j++)
                {
                    int? supporter = null;
                    int? supported = null;

                    if (bricks[j].SupportedByOther(bricks[i]))
                    {
                        supporter = i;
                        supported = j;
                    }
                    else if (bricks[i].SupportedByOther(bricks[j]))
                    {
                        supporter = j;
                        supported = i;
                    }

                    if (supporter is int sups && supported is int supped)
                    {
                        supportsBricks[sups].Add(supped);
                        isSupportedByBricks[supped].Add(sups);
                    }
                }
            }

            return (supportsBricks, isSupportedByBricks);
        }

        public int ChainDisintegration(int brickId, Dictionary<int, List<int>> supportRelations, Dictionary<int, List<int>> supportedByRelations, List<Brick> bricks)
        {
            // Because all bricks don't have weird tetris-shapes, if a candidate isn't falling, it won't fall down the line either because all future candidates will have their lower Z-level above that of the candidate
            // So as long as we handle the bricks in order of their Z-levels we will know whether a brick will fall or not by the time we consider it as a candidate
            var candidates = new NaivePriorityQueue<Brick>();
            var disintegratedBricks = new HashSet<int> { brickId };
            foreach (var candidateId in supportRelations[brickId])
            {
                candidates.Enqueue(bricks[candidateId]);
            }

            while (!candidates.IsEmpty())
            {
                var candidate = candidates.Dequeue();
                var candidateSupport = supportedByRelations[candidate.Id];
                var noLongerSupported = candidateSupport.All(supportingBrick => disintegratedBricks.Contains(supportingBrick));
                if (noLongerSupported)
                {
                    disintegratedBricks.Add(candidate.Id);
                    foreach (var newCandidateId in supportRelations[candidate.Id])
                    {
                        candidates.Enqueue(bricks[newCandidateId]);
                    }
                }
            }

            return disintegratedBricks.Count - 1;
        }
    }

    internal record Brick : IComparable<Brick>
    {
        public int Id { get; set; }
        public (int A, int B) X { get; set; }
        public (int A, int B) Y { get; set; }
        public (int A, int B) Z { get; set; }


        public Brick((int X, int Y, int Z) a, (int X, int Y, int Z) b, int id)
        {
            Id = id;
            X = (a.X, b.X);
            Y = (a.Y, b.Y);
            Z = (a.Z, b.Z);
        }

        public bool SupportedByOther(Brick other)
        {
            return Z.A - other.Z.B == 1 && OverlapOnXY(other);
        }

        public bool OverlapOnXY(Brick other)
        {
            return Overlap(X, other.X) && Overlap(Y, other.Y);
        }

        // TODO: Extract
        public bool PointIsOnLine(int point, (int Start, int End) line)
        {
            return point >= line.Start && point <= line.End;
        }

        public bool Overlap((int Start, int End) lineA, (int Start, int End) lineB)
        {
            return PointIsOnLine(lineA.Start, lineB) ||
                   PointIsOnLine(lineA.End, lineB) ||
                   PointIsOnLine(lineB.Start, lineA) ||
                   PointIsOnLine(lineB.End, lineA);
        }

        public int CompareTo(Brick other)
        {
            if(Z.A != other.Z.A)
            {
                return Z.A.CompareTo(other.Z.A);
            } else
            {
                return Z.B.CompareTo(other.Z.B);
            }
        }
    }
}
