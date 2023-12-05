// See https://aka.ms/new-console-template for more information
using AoCSharp2023;

Console.WriteLine("Hello, World!");

//new Day1();
//var d2 = new Day2();
//var d4 = new Day4();
var d5 = new Day5();

var testLines = new List<string>()
{
"seeds: 79 14 55 13",
"",
"seed-to-soil map:",
"50 98 2",
"52 50 48",
"",
"soil-to-fertilizer map:",
"0 15 37",
"37 52 2",
"39 0 15",
"",
"fertilizer-to-water map:",
"49 53 8",
"0 11 42",
"42 0 7",
"57 7 4",
"",
"water-to-light map:",
"88 18 7",
"18 25 70",
"",
"light-to-temperature map:",
"45 77 23",
"81 45 19",
"68 64 13",
"",
"temperature-to-humidity map:",
"0 69 1",
"1 0 69",
"",
"humidity-to-location map:",
"60 56 37",
"56 93 4",
};

//var seeds = ParseSeeds(testLines[0]);
//var mappings = ParseMappings(testLines.Skip(2).ToList());

//var mappedSeedLocations = seeds.Select(x => MapSeedToLocation(x, mappings));
//foreach(var mappedSeedLocation in mappedSeedLocations)
//{
//    Console.WriteLine(mappedSeedLocation);
//}

//int MapSeedToLocation(int seed, Dictionary<string, List<AlmanacMapping>> mappings)
//{
//    int currentValue = seed;
//    string currentType = "seed";

//    while(currentType != "location")
//    {
//        // Find map that can map this value
//        var validMapping = mappings[currentType].FirstOrDefault(mapping => mapping.IsWithinRange(currentValue));
//        if(validMapping is AlmanacMapping)
//        {
//            currentValue = validMapping.Map(currentValue);
//            currentType = validMapping.DestinationType;
//        } else
//        {
//            currentType = mappings[currentType][0].DestinationType;
//        }
//    }

//    return currentValue;
//}

//List<int> ParseSeeds(string seedLine)
//{
//    return seedLine.Substring(7).Split(' ').Select(seed => int.Parse(seed)).ToList();
//}


//Dictionary<string, List<AlmanacMapping>> ParseMappings(IList<string> lines)
//{
//    var allMappings = new Dictionary<string, List<AlmanacMapping>>();
//    List<AlmanacMapping> currentMappingList = new List<AlmanacMapping>();
//    string currentSourceType = "";
//    string currentDestinationType = "";
//    for(int i = 0; i < lines.Count; i++) // Assume seedline and empty line have been trimmed off already else i should've been 2
//    {
//        // New mapping-type
//        if (lines[i].Contains(':'))
//        {
//            var types = lines[i].Split(' ')[0].Split("-to-");
//            currentSourceType = types[0];
//            currentDestinationType = types[1];
//            currentMappingList = new List<AlmanacMapping>();
//            allMappings.Add(currentSourceType, currentMappingList);
//        }
//        else if(lines[i].Length > 0)
//        {
//            var mapping = lines[i].Split(' ').Select(x => int.Parse(x)).ToList();
//            currentMappingList.Add(new AlmanacMapping
//            {
//                SourceType = currentSourceType,
//                DestinationType = currentDestinationType,
//                Destination = mapping[0],
//                Source = mapping[1],
//                Range = mapping[2]
//            });
//        }
//    }

//    return allMappings;
//}
