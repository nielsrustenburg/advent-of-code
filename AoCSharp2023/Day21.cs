using Shared;
using System.Numerics;

namespace AoCSharp2023
{
    internal class Day21
    {
        public Day21()
        {
            string[] lines = File.ReadAllLines("./inputs/21.txt");
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        BigInteger Part1(string[] lines)
        {
            var input = ParseInput(lines);
            var result = AmountOfPlotsReachedInNSteps(input.Grid, input.StartingPlot, 64);
            return result;
        }
        BigInteger Part2(string[] lines)
        {
            var input = ParseInput(lines);
            var width = input.Grid.GetLength(0);
            var totalSteps = 26501365;
            var initialStepsToEdge = (width - 1) / 2;

            // We are relying on the assumption that totalSteps is always initialStepsToEdge + n * width
            BigInteger blocksReached = ((totalSteps - initialStepsToEdge) / width); 
            var sameN = ((blocksReached+1) / 2); // Amount of rings of blocks with same parity as initial block
            var othN = (blocksReached / 2); // Amount of rings of blocks with opposite parity from initial block
            BigInteger otherThanStartBlocks = (othN / 2) * (2 * 4 + (othN - 1) * 8);
            BigInteger sameAsStartBlocks = (sameN / 2) * (0 + (sameN - 1) * 8);

            // Calling a block odd if the centre of the block is reachable only on odd steps
            BigInteger oddFullBlocks = totalSteps % 2 == 0 ? otherThanStartBlocks : sameAsStartBlocks + 1;
            BigInteger evenFullBlocks = totalSteps % 2 == 0 ? sameAsStartBlocks+1 : otherThanStartBlocks;

            BigInteger smallPartialBlocksPerEdge = blocksReached;
            BigInteger largePartialBlocksPerEdge = blocksReached - 1;

            BigInteger oddReachableTilesInFullBlock = AmountOfPlotsReachedInNSteps(input.Grid, input.StartingPlot, width);
            var evenGrid = ParseInput(lines);
            BigInteger evenReachableTilesInFullBlock = AmountOfPlotsReachedInNSteps(evenGrid.Grid, evenGrid.StartingPlot, width + 1);
            var freshGrids = Enumerable.Range(0, 12).Select(_ => ParseInput(lines).Grid).ToArray();
            BigInteger topPartialBlock = AmountOfPlotsReachedInNSteps(freshGrids[0], freshGrids[0][width / 2, width - 1], width-1);
            BigInteger rightPartialBlock = AmountOfPlotsReachedInNSteps(freshGrids[1], freshGrids[1][0, width / 2], (width - 1));
            BigInteger bottomPartialBlock = AmountOfPlotsReachedInNSteps(freshGrids[2], freshGrids[2][width / 2, 0], (width - 1));
            BigInteger leftPartialBlock = AmountOfPlotsReachedInNSteps(freshGrids[3], freshGrids[3][width - 1, width / 2], (width - 1));

            var stepsInSmallBlocks = ((width - 1) / 2) - 1;
            var stepsInLarge = width + stepsInSmallBlocks;
            BigInteger topRightLarge = AmountOfPlotsReachedInNSteps(freshGrids[4], freshGrids[4][0, width - 1], stepsInLarge);
            BigInteger topRightSmall = AmountOfPlotsReachedInNSteps(freshGrids[5], freshGrids[5][0, width - 1], stepsInSmallBlocks);
            BigInteger bottomRightLarge = AmountOfPlotsReachedInNSteps(freshGrids[6], freshGrids[6][0, 0], stepsInLarge);
            BigInteger bottomRightSmall = AmountOfPlotsReachedInNSteps(freshGrids[7], freshGrids[7][0, 0], stepsInSmallBlocks);
            BigInteger bottomLeftLarge = AmountOfPlotsReachedInNSteps(freshGrids[8], freshGrids[8][width - 1, 0], stepsInLarge);
            BigInteger bottomLeftSmall = AmountOfPlotsReachedInNSteps(freshGrids[9], freshGrids[9][width - 1, 0], stepsInSmallBlocks);
            BigInteger topLeftLarge = AmountOfPlotsReachedInNSteps(freshGrids[10], freshGrids[10][width - 1, width - 1], stepsInLarge);
            BigInteger topLeftSmall = AmountOfPlotsReachedInNSteps(freshGrids[11], freshGrids[11][width - 1, width - 1], stepsInSmallBlocks);

            BigInteger total =
              (oddReachableTilesInFullBlock * oddFullBlocks) + (evenReachableTilesInFullBlock * evenFullBlocks) +
              (topPartialBlock + rightPartialBlock + bottomPartialBlock + leftPartialBlock) +
              ((topRightLarge + bottomRightLarge + bottomLeftLarge + topLeftLarge) * largePartialBlocksPerEdge) +
              ((topRightSmall + bottomRightSmall + bottomLeftSmall + topLeftSmall) * smallPartialBlocksPerEdge);

            return total;
        }

        BigInteger AmountOfPlotsReachedInNSteps(IPlot[,] grid, IPlot startingPlot, BigInteger n)
        {
            BigInteger stepCount = 0;
            List<IPlot> plotsToStepOn = new List<IPlot> { startingPlot };
            while (stepCount <= n && plotsToStepOn.Any())
            {
                List<IPlot> plotsForNextStep = new List<IPlot>();
                foreach (var plot in plotsToStepOn)
                {
                    var isUpdate = plot.StepOn(stepCount);
                    if (isUpdate)
                    {
                        var neighbours = GridHelper.FindNeighbours(grid, plot.X, plot.Y);
                        foreach (var neighbour in neighbours)
                        {
                            plotsForNextStep.Add(grid[neighbour.x, neighbour.y]);
                        }
                    }
                }
                plotsToStepOn = plotsForNextStep;
                stepCount++;
            }

            var nIsEven = n % 2 == 0;
            BigInteger amountReached = 0;
            foreach (var plot in grid)
            {
                if (nIsEven && plot.OnEven)
                {
                    amountReached++;
                }
                if (!nIsEven && plot.OnOdd)
                {
                    amountReached++;
                }
            }
            return amountReached;
        }

        (IPlot[,] Grid, IPlot StartingPlot) ParseInput(string[] lines)
        {
            IPlot startingPlot = null;
            IPlot[,] grid = new IPlot[lines.Length, lines[0].Length];
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    IPlot plot = lines[y][x] switch
                    {
                        '.' => new GardenPlot(x, y),
                        '#' => new Rock(x, y),
                        'S' => new GardenPlot(x, y),
                        _ => throw new NotImplementedException("Only expecting '.', '#' or 'S' "),
                    };
                    grid[x, y] = plot;
                    if (lines[y][x] == 'S')
                    {
                        startingPlot = plot;
                    }
                }
            }
            return (grid, startingPlot);
        }
    }

    internal interface IPlot
    {
        public int X { get; }
        public int Y { get; }
        /// <summary>
        /// Will be active on even steps after being reached
        /// </summary>
        public bool OnEven { get; }
        /// <summary>
        /// Will be active on even steps after being reached
        /// </summary>
        public bool OnOdd { get; }
        public BigInteger? InitialStepsToReach { get; }

        /// <summary>
        /// Returns true if the step changed the state of the plot, e.g. first time to be stepped on on an even/odd stepcount
        /// </summary>
        public bool StepOn(BigInteger step);

    }

    internal class Rock : IPlot
    {
        public int X { get; set; }
        public int Y { get; set; }
        /// <summary>
        /// Will be active on even steps after being reached
        /// </summary>
        public bool OnEven { get; init; }
        /// <summary>
        /// Will be active on even steps after being reached
        /// </summary>
        public bool OnOdd { get; init; }
        public BigInteger? InitialStepsToReach { get; init; }
        public Rock(int x, int y)
        {
            X = x; Y = y;
            OnEven = false; OnOdd = false; InitialStepsToReach = null;
        }
        /// <summary>
        /// Returns true if the step changed the state of the plot, e.g. first time to be stepped on on an even/odd stepcount
        /// </summary>
        public bool StepOn(BigInteger step)
        {
            return false;
        }
    }

    internal class GardenPlot : IPlot
    {
        public int X { get; set; }
        public int Y { get; set; }
        /// <summary>
        /// Will be active on even steps after being reached
        /// </summary>
        public bool OnEven { get; set; }
        /// <summary>
        /// Will be active on even steps after being reached
        /// </summary>
        public bool OnOdd { get; set; }
        public BigInteger? InitialStepsToReach { get; set; }
        public GardenPlot(int x, int y)
        {
            X = x; Y = y;
        }

        /// <summary>
        /// Returns true if the step changed the state of the plot, e.g. first time to be stepped on on an even/odd stepcount
        /// </summary>
        public bool StepOn(BigInteger step)
        {
            var stepIsEven = step % 2 == 0;
            bool stateChanged = false;
            if (stepIsEven && !OnEven)
            {
                stateChanged = true;
                OnEven = true;
            }
            if (!stepIsEven && !OnOdd)
            {
                stateChanged = true;
                OnOdd = true;
            }
            if (stateChanged)
            {
                InitialStepsToReach = step;
            }

            return stateChanged;
        }
    }
}
