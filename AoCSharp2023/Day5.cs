namespace AoCSharp2023
{
    public class Day5
    {
        public Day5()
        {
            string[] lines = File.ReadAllLines("./inputs/5.txt");
            var seeds = ParseSeeds(lines[0]);
            var mappings = ParseMappings(lines.Skip(2).ToList());

            var mappedSeedLocations = seeds.Select(x => MapSeedToLocation(x, mappings));
            Console.WriteLine(mappedSeedLocations.Min());

            var seedRanges = ParseSeedRanges(lines[0]);
            var locationRanges = ConvertSeedRangesToLocationRanges(seedRanges, mappings);
            var lowestLocation = locationRanges.Min(r => r.Start);
            Console.WriteLine(lowestLocation);
        }

        List<Range> ConvertSeedRangesToLocationRanges(List<Range> seedRanges, Dictionary<string, List<AlmanacMapping>> mappings)
        {
            var currentType = seedRanges.First().Type;
            var currentRangesToConvert = seedRanges;
            while(currentType != "location")
            {
                currentRangesToConvert = ConvertRange(currentRangesToConvert, mappings);
                currentType = currentRangesToConvert.First().Type;
            }
            return currentRangesToConvert;
        }

        List<Range> ConvertRange(IEnumerable<Range> rangesToConvert, Dictionary<string, List<AlmanacMapping>> mappings)
        {
            // TODO: can consider trying to combine/connect ranges after being done..
            // TODO: Bonus, is IEnumerable dangerous here..? Today is too messy / time consuming to care..
            List<Range> convertedRanges = new List<Range>();
            while (rangesToConvert.Any())
            {
                var currentRange = rangesToConvert.First();
                // TODO: check if range has any validity e.g. not empty or negative? Or should be prevented earlier if my code is clean enough..
                var mappingWithOverlap = mappings[currentRange.Type].FirstOrDefault(m => m.HasSomeOverlap(currentRange));
                if(mappingWithOverlap != null)
                {
                    var (converted, remaining) = mappingWithOverlap.ConvertRange(currentRange);
                    convertedRanges.Add(converted);
                    rangesToConvert = rangesToConvert.Skip(1).Concat(remaining);
                } else
                {
                    var converted = new Range()
                    {
                        Type = mappings[currentRange.Type].First().DestinationType,
                        Start = currentRange.Start,
                        Length = currentRange.Length,
                    };
                    convertedRanges.Add(converted);
                    rangesToConvert = rangesToConvert.Skip(1);
                }
            }
            return convertedRanges;
        }

        long MapSeedToLocation(long seed, Dictionary<string, List<AlmanacMapping>> mappings)
        {
            long currentValue = seed;
            string currentType = "seed";

            while (currentType != "location")
            {
                // Find map that can map this value
                var validMapping = mappings[currentType].FirstOrDefault(mapping => mapping.IsWithinRange(currentValue));
                if (validMapping is AlmanacMapping)
                {
                    currentValue = validMapping.Map(currentValue);
                    currentType = validMapping.DestinationType;
                }
                else
                {
                    currentType = mappings[currentType][0].DestinationType;
                }
            }

            return currentValue;
        }

        List<long> ParseSeeds(string seedLine)
        {
            return seedLine.Substring(7).Split(' ').Select(seed => long.Parse(seed)).ToList();
        }

        List<Range> ParseSeedRanges(string seedLine)
        {
            List<Range> ranges = new List<Range>();
            var seedVals = seedLine.Substring(7).Split(' ').Select(seed => long.Parse(seed)).ToList();
            for(int i = 0; i < seedVals.Count; i += 2)
            {
                ranges.Add(new Range
                {
                    Type = "seed",
                    Start = seedVals[i],
                    Length = seedVals[i + 1],
                });
            }
            return ranges;
        }


        Dictionary<string, List<AlmanacMapping>> ParseMappings(IList<string> lines)
        {
            var allMappings = new Dictionary<string, List<AlmanacMapping>>();
            List<AlmanacMapping> currentMappingList = new List<AlmanacMapping>();
            string currentSourceType = "";
            string currentDestinationType = "";
            for (int i = 0; i < lines.Count; i++) // Assume seedline and empty line have been trimmed off already else i should've been 2
            {
                // New mapping-type
                if (lines[i].Contains(':'))
                {
                    var types = lines[i].Split(' ')[0].Split("-to-");
                    currentSourceType = types[0];
                    currentDestinationType = types[1];
                    currentMappingList = new List<AlmanacMapping>();
                    allMappings.Add(currentSourceType, currentMappingList);
                }
                else if (lines[i].Length > 0)
                {
                    var mapping = lines[i].Split(' ').Select(x => long.Parse(x)).ToList();
                    currentMappingList.Add(new AlmanacMapping
                    {
                        SourceType = currentSourceType,
                        DestinationType = currentDestinationType,
                        Destination = mapping[0],
                        Source = mapping[1],
                        Length = mapping[2]
                    });
                }
            }

            return allMappings;
        }

    }

    public class AlmanacMapping
    {
        public string SourceType { get; init; }
        public long Source { get; init; }
        public long SourceEnd => Source + Length - 1;
        public long Length { get; init; }

        public string DestinationType { get; init; }
        public long Destination { get; init; }

        public bool IsWithinRange(long number)
        {
            return number >= Source && number < Source + Length;
        }

        public bool HasSomeOverlap(Range range)
        {
            return (Source >= range.Start && Source <= range.End) ||  // Source is inside of Range
                   (SourceEnd >= range.Start && SourceEnd <= range.End) || // end of Mapping is inside of Range
                   (range.Start >= Source && range.Start <= SourceEnd) || // range.Start is inside of Mapping
                   (range.End >= Source && range.End <= SourceEnd); // end of Range is inside mapping
        }

        public (Range converted, List<Range> remaining) ConvertRange(Range range)
        {
            if(range.Type != SourceType)
            {
                throw new ArgumentOutOfRangeException(nameof(range));
            }

            var rangeStartInside = (range.Start >= Source && range.Start <= SourceEnd);
            var rangeEndInside = (range.End >= Source && range.End <= SourceEnd);

            // Would not be surprised if all of this can be combined into something much more compact.. but lazy / time crunched
            if (rangeStartInside && rangeEndInside)
            {
                var nRange = new Range
                {
                    Type = DestinationType,
                    Start = Map(range.Start),
                    Length = range.Length,
                };
                return (nRange, new List<Range>());
            }

            var sourceStartInside = (Source >= range.Start && Source <= range.End);
            var sourceEndInside = (SourceEnd >= range.Start && SourceEnd <= range.End);

            if(sourceStartInside && sourceEndInside)
            {
                var nRange = new Range
                {
                    Type = DestinationType,
                    Start = Destination, // Or Map(Source)
                    Length = Length,
                };

                var before = new Range
                {
                    Type = range.Type,
                    Start = range.Start,
                    Length = Source - range.Start,
                };

                var after = new Range
                {
                    Type = range.Type,
                    Start = Source + Length,
                    Length = range.Start + range.Length - (Source + Length)
                };

                return (nRange, new List<Range>() { before, after }.Where(x => x.Length > 0).ToList());
            }

            if(rangeStartInside && sourceEndInside)
            {
                // Start of the range fits, end of range doesn't 
                var nRange = new Range
                {
                    Type = DestinationType,
                    Start = Map(range.Start),
                    Length = Source + Length - range.Start,
                };

                var after = new Range
                {
                    Type = range.Type,
                    Start = Source + Length,
                    Length = range.Length - nRange.Length,
                };
                return (nRange, new List<Range> { after }.Where(x => x.Length > 0).ToList());
            }

            if(rangeEndInside && sourceStartInside)
            {
                // End of range fits, start of range doesn't
                var nRange = new Range
                {
                    Type = DestinationType,
                    Start = Destination,
                    Length = range.Start + range.Length - Source,
                };

                var before = new Range
                {
                    Type = range.Type,
                    Start = range.Start,
                    Length = range.Length - nRange.Length,
                };

                return (nRange, new List<Range> { before }.Where(x => x.Length > 0).ToList());
            }

            throw new Exception("Must've made an error while thinking out how overlap works..");
        }

        public long Map(long number)
        {
            if (IsWithinRange(number))
            {
                return number - Source + Destination;
            }
            throw new ArgumentOutOfRangeException(nameof(number));
        }
    }

    public class Range
    {
        public long Start { get; init; }
        public long Length { get; init; }
        public long End => Start + Length - 1;
        public string Type { get; init; }
    }
}
