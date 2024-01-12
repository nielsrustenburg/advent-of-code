using Shared;
using System.Numerics;

namespace AoCSharp2023
{
    internal class Day20
    {
        public Day20()
        {
            string[] lines = File.ReadAllLines("./inputs/20.txt");
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        long Part1(string[] lines)
        {
            var modules = CreateModuleGraph(lines);
            long lowPulses = 0;
            long highPulses = 0;
            for (int i = 0; i < 1000; i++)
            {
                var pulses = PressButton(modules);
                lowPulses += pulses.LowPulses;
                highPulses += pulses.HighPulses;
            }

            return lowPulses * highPulses;
        }

        BigInteger Part2(string[] lines)
        {
            var modules = CreateModuleGraph(lines);
            GraphRepresentation(modules);
            var conjunctionPulseRhythms = new Dictionary<string, List<string>>();
            if (!modules.ContainsKey("rx"))
            {
                return -1;
            }
            bool finished = false;
            int presses = 0;
            while (!finished)
            {
                presses++;
                var (_, _, conjunctionPulses) = PressButton(modules);
                if (conjunctionPulses.Any())
                {
                    foreach(var conPulse in conjunctionPulses.DistinctBy(cp => cp.M.Name))
                    {
                        var con = conPulse.M;
                        var pulseRhythm = conjunctionPulses.Aggregate("", (a, b) => b.M == con ? a + (b.Pulse ? 'h' : 'l') : a);
                        if (!conjunctionPulseRhythms.TryGetValue(con.Name, out var occurrences))
                        {
                            occurrences = new List<string>();
                            conjunctionPulseRhythms[con.Name] = occurrences;
                        }
                        conjunctionPulseRhythms[con.Name].Add(pulseRhythm);
                    }
                }

                if (conjunctionPulseRhythms["gk"].Where(s => s.Contains('l')).Count() == 2 && conjunctionPulseRhythms["tf"].Where(s => s.Contains('l')).Count() == 2 && conjunctionPulseRhythms["gx"].Where(s => s.Contains('l')).Count() == 2 && conjunctionPulseRhythms["xr"].Where(s => s.Contains('l')).Count() == 2)
                {
                    var gk = conjunctionPulseRhythms["gk"].FindIndex(s => s.Contains('l'));
                    var tf = conjunctionPulseRhythms["tf"].FindIndex(s => s.Contains('l'));
                    var gx = conjunctionPulseRhythms["gx"].FindIndex(s => s.Contains('l'));
                    var xr = conjunctionPulseRhythms["xr"].FindIndex(s => s.Contains('l'));
                    var lcm = AoCMath.LCM(new List<BigInteger> { gk+1, tf + 1, gx + 1, xr + 1 });
                    return lcm;
                }
            }
            throw new Exception("should return from inside loop");
        }

        void GraphRepresentation(Dictionary<string, Module> modules)
        {

            List<string> graphConnections = new List<string> { "digraph G {" };
            foreach (var module in modules.Values)
            {
                foreach (var c in module.Consumers)
                {
                    graphConnections.Add($"    {module.Name} -> {c.Name};");
                }
            }

            graphConnections.Add("    ");

            foreach (var module in modules.Values)
            {
                if (module is Broadcaster)
                {
                    graphConnections.Add($"    {module.Name} [shape=Mdiamond]");
                }
                else if (module is FlipFlop)
                {
                    graphConnections.Add($"    {module.Name} [shape=triangle]");
                } else if(module is Conjunction)
                {
                    graphConnections.Add($"    {module.Name} [shape=hexagon]");
                } else if(module is Output)
                {
                    graphConnections.Add($"    {module.Name} [shape=Msquare fillcolor=red]");
                }
            }
            graphConnections.Add("}");
            File.WriteAllLines("graph.txt", graphConnections.ToArray());
        }

        (long LowPulses, long HighPulses, List<(Module M, bool Pulse)> ConjunctionSentPulse) PressButton(Dictionary<string, Module> modules)
        {
            var queue = new Queue<(bool HighPulse, Module Consumer, Module Producer)>();
            var conjuctionSentLowPulse = new List<(Module M, bool Pulse)>();
            queue.Enqueue((false, modules["broadcaster"], null));
            long lowPulses = 0;
            long highPulses = 0;
            //bool rxReceivedLow = false;
            while (queue.Any())
            {
                var (pulseIsHigh, consumer, producer) = queue.Dequeue();
                //if (!pulseIsHigh && consumer.Name == "rx")
                //{
                //    rxReceivedLow = true;
                //}
                if (pulseIsHigh)
                {
                    highPulses++;
                }
                else
                {
                    lowPulses++;
                }
                var downstreamMessages = consumer.Pulse(producer, pulseIsHigh);
                if(consumer is Conjunction && downstreamMessages.Any())
                {
                    conjuctionSentLowPulse.Add((consumer, downstreamMessages.First().HighPulse));
                }
                foreach (var message in downstreamMessages)
                {
                    queue.Enqueue(message);
                }
            }
            return (lowPulses, highPulses, conjuctionSentLowPulse);
        }

        Dictionary<string, Module> CreateModuleGraph(string[] lines)
        {
            Dictionary<string, Module> modules = new Dictionary<string, Module>();
            Dictionary<string, string[]> consumersToAdd = new Dictionary<string, string[]>();
            foreach (string line in lines)
            {
                var producerConsumers = line.Split(" -> ");
                var producerCode = producerConsumers[0];
                var consumers = producerConsumers[1].Split(", ");
                Module producer;
                if (producerCode[0] == '%')
                {
                    producer = new FlipFlop(producerCode.Substring(1));
                }
                else if (producerCode[0] == '&')
                {
                    producer = new Conjunction(producerCode.Substring(1));
                }
                else
                {
                    producer = new Broadcaster(producerCode);
                }
                modules.Add(producer.Name, producer);
                consumersToAdd.Add(producer.Name, consumers);
            }

            foreach (var (producerName, consumers) in consumersToAdd)
            {
                var producer = modules[producerName];
                foreach (var consumerName in consumers)
                {
                    if (!modules.TryGetValue(consumerName, out var consumer))
                    {
                        consumer = new Output(consumerName);
                        modules[consumerName] = consumer;
                    }
                    producer.AddConnection(consumer, true);
                    consumer.AddConnection(producer, false);
                }
            }

            return modules;
        }
    }

    internal abstract class Module
    {
        public string Name { get; set; }
        public List<Module> Consumers { get; set; }

        public abstract List<(bool HighPulse, Module Consumer, Module Producer)> Pulse(Module origin, bool highPulse);

        public abstract void AddConnection(Module other, bool asProducer);
        public override string ToString()
        {
            return Name;
        }
    }

    internal class Broadcaster : Module
    {
        public List<Module> Producers { get; set; }
        public Broadcaster(string name)
        {
            Name = name;
            Consumers = new List<Module>();
            Producers = new List<Module>();
        }

        public override void AddConnection(Module other, bool asProducer)
        {
            if (asProducer)
            {
                Consumers.Add(other);
            }
            else
            {
                Producers.Add(other);
            }
        }

        public override List<(bool HighPulse, Module Consumer, Module Producer)> Pulse(Module origin, bool highPulse)
        {
            var downStreamPulses = new List<(bool HighPulse, Module Consumer, Module Producer)>();
            foreach (Module consumer in Consumers)
            {
                downStreamPulses.Add((highPulse, consumer, this));
            }
            return downStreamPulses;
        }
    }

    internal class Conjunction : Module
    {
        public Dictionary<string, bool> PulsesReceived { get; set; }
        public List<Module> Producers { get; set; }

        public Conjunction(string name)
        {
            Name = name;
            PulsesReceived = new Dictionary<string, bool>();
            Consumers = new List<Module>();
            Producers = new List<Module>();
        }

        public override void AddConnection(Module other, bool asProducer)
        {
            if (asProducer)
            {
                Consumers.Add(other);
            }
            else
            {
                PulsesReceived.TryAdd(other.Name, false);
                Producers.Add(other);
            }
        }

        public override List<(bool HighPulse, Module Consumer, Module Producer)> Pulse(Module origin, bool highPulse)
        {
            var downStreamPulses = new List<(bool HighPulse, Module Consumer, Module Producer)>();
            PulsesReceived[origin.Name] = highPulse;
            var pulseToSend = !PulsesReceived.Values.All(high => high);

            foreach (var consumer in Consumers)
            {
                downStreamPulses.Add((pulseToSend, consumer, this));
            }

            return downStreamPulses;
        }
    }

    internal class FlipFlop : Module
    {
        public bool On { get; set; }
        public List<Module> Producers { get; set; }

        public FlipFlop(string name)
        {
            Name = name;
            On = false;
            Consumers = new List<Module>();
            Producers = new List<Module>();
        }

        public override void AddConnection(Module other, bool asProducer)
        {
            if (asProducer)
            {
                Consumers.Add(other);
            }
            else
            {
                Producers.Add(other);
            }
        }

        public override List<(bool HighPulse, Module Consumer, Module Producer)> Pulse(Module origin, bool highPulse)
        {
            var downStreamPulses = new List<(bool HighPulse, Module Consumer, Module Producer)>();
            if (!highPulse)
            {
                On = !On;
                foreach (Module consumer in Consumers)
                {
                    downStreamPulses.Add((On, consumer, this));
                }
            }
            return downStreamPulses;
        }
    }

    internal class Output : Module
    {
        List<Module> Producers { get; set; }
        public Output(string name)
        {
            Name = name;
            Consumers = new List<Module>();
            Producers = new List<Module>();
        }
        public override void AddConnection(Module other, bool asProducer)
        {
            if (!asProducer)
            {
                Producers.Add(other);
            }
        }

        public override List<(bool HighPulse, Module Consumer, Module Producer)> Pulse(Module origin, bool highPulse)
        {
            return new List<(bool HighPulse, Module Consumer, Module Producer)>();
        }
    }
}
