using System.Data;
using System.Numerics;

namespace AoCSharp2023
{
    internal class Day19
    {
        public Day19()
        {
            string[] lines = File.ReadAllLines("./inputs/19.txt");
            (Dictionary<string, Workflow> workflows, List<XMASPart> parts) = ReadInput(lines);
            Console.WriteLine(Part1(workflows, parts));
            Console.WriteLine(Part2(workflows));
        }

        (Dictionary<string, Workflow> Workflows, List<XMASPart> Parts) ReadInput(string[] lines)
        {
            var workflows = new Dictionary<string, Workflow>();
            int index = 0;
            while (lines[index] != string.Empty)
            {
                var workflow = new Workflow(lines[index]);
                workflows.Add(workflow.Name, workflow);
                index++;
            }

            var parts = new List<XMASPart>();
            for (int i = index + 1; i < lines.Length; i++)
            {
                parts.Add(new XMASPart(lines[i]));
            }

            return (workflows, parts);
        }

        int Part1(Dictionary<string, Workflow> workflows, List<XMASPart> parts)
        {
            var accepted = new List<XMASPart>();
            foreach (var part in parts)
            {
                var workflowName = "in";
                var outcome = Outcome.GoToWorkflow;
                while (outcome == Outcome.GoToWorkflow)
                {
                    (outcome, workflowName) = workflows[workflowName].Apply(part);
                }
                if (outcome == Outcome.Accept)
                {
                    accepted.Add(part);
                }
            }

            return accepted.Sum(part => part.X + part.M + part.A + part.S);
        }

        BigInteger Part2(Dictionary<string, Workflow> workflows)
        {
            var queue = new List<(string WfName, List<string> Constraints)>
            {
                ("in", new List<string>()),
            };

            var accepted = new List<List<string>>();
            var rejected = new List<List<string>>();

            while (queue.Any())
            {
                var next = queue.Last();
                queue.RemoveAt(queue.Count - 1);

                if (next.WfName == "A")
                {
                    accepted.Add(next.Constraints);
                }
                else if (next.WfName == "R")
                {
                    rejected.Add(next.Constraints);
                }
                else
                {
                    var workflow = workflows[next.WfName];
                    var constraints = next.Constraints.ToList();
                    for (int i = 0; i < workflow.Rules.Count; i++)
                    {
                        var rule = workflow.Rules[i];
                        if (rule.Contains(':'))
                        {
                            var split = rule.Split(':');
                            var comparison = split[0];
                            var destination = split[1];
                            queue.Add((destination, constraints.Append(comparison).ToList()));
                            var oppositeConstraint = comparison.Contains('>') ? comparison.Replace('>', 'l') : comparison.Replace('<', 'g'); // l: <= g: >=
                            constraints.Add(oppositeConstraint);
                        }
                        else
                        {
                            queue.Add((rule, constraints));
                        }
                    }
                }
            }

            BigInteger totalAccepted = CountNumbersInConstraintSet(accepted);
            //BigInteger totalRejected = CountNumbersInConstraintSet(rejected);
            return totalAccepted;
        }


        public BigInteger CountNumbersInConstraintSet(List<List<string>> constraintSet)
        {
            BigInteger total = 0;
            foreach (var constraints in constraintSet)
            {
                var minima = new int[] { 1, 1, 1, 1 };
                var maxima = new int[] { 4000, 4000, 4000, 4000 };

                for (int i = 0; i < constraints.Count; i++)
                {
                    var indexToUpdate = constraints[i][0] switch
                    {
                        'x' => 0,
                        'm' => 1,
                        'a' => 2,
                        's' => 3,
                        _ => throw new NotImplementedException()
                    };

                    var amount = int.Parse(constraints[i].Substring(2));
                    var op = constraints[i][1];
                    if (op == '<' && amount < maxima[indexToUpdate])
                    {
                        maxima[indexToUpdate] = amount - 1;
                    }
                    if (op == '>' && amount > minima[indexToUpdate])
                    {
                        minima[indexToUpdate] = amount + 1;
                    }
                    if (op == 'l' && amount <= maxima[indexToUpdate])
                    {
                        maxima[indexToUpdate] = amount;
                    }
                    if (op == 'g' && amount >= minima[indexToUpdate])
                    {
                        minima[indexToUpdate] = amount;
                    }
                }

                BigInteger distinctCombinations = 1;
                for (int i = 0; i < 4; i++)
                {
                    if (maxima[i] > minima[i])
                    {
                        distinctCombinations = distinctCombinations * (maxima[i] + 1 - minima[i]);
                    }
                    else
                    {
                        distinctCombinations = 0;
                    }
                }

                total += distinctCombinations;
            }
            return total;
        }
    }

    internal class Workflow
    {
        public string Name { get; set; }
        public List<string> Rules { get; set; }
        public Workflow(string input)
        {
            var split1 = input.Split('{');
            Name = split1[0];
            Rules = split1[1].TrimEnd('}').Split(',').ToList();
        }

        public (Outcome Outcome, string? WorkflowName) Apply(XMASPart xmas)
        {
            for (int i = 0; i < Rules.Count; i++)
            {
                var result = xmas.Apply(Rules[i]);
                if (result.Outcome != Outcome.ApplyNext)
                {
                    return result;
                }
            }
            throw new Exception("There should always be a rule that results in an outcome other than ApplyNext");
        }
    }

    internal enum Outcome
    {
        ApplyNext,
        GoToWorkflow,
        Reject,
        Accept
    }

    internal class XMASPart
    {
        public int X { get; set; }
        public int M { get; set; }
        public int A { get; set; }
        public int S { get; set; }

        public XMASPart(string input)
        {
            var xmas = input.Split(',').Select(i => i.Split('=')[1]).Select(i => int.Parse(i.TrimEnd('}'))).ToList();
            X = xmas[0];
            M = xmas[1];
            A = xmas[2];
            S = xmas[3];
        }

        public (Outcome Outcome, string? WorkflowName) Apply(string rule)
        {
            var splitColon = rule.Split(':');
            bool succeeds = splitColon.Length == 1; // Else first apply the comparison
            if (splitColon.Length > 1)
            {
                var comparison = splitColon[0];
                var compareProperty = comparison[0];
                var compareOperator = comparison[1];
                var compareValue = int.Parse(comparison.Substring(2));
                var xmasValue = compareProperty switch
                {
                    'x' => X,
                    'm' => M,
                    'a' => A,
                    's' => S,
                    _ => throw new NotImplementedException("should not occur")
                };

                succeeds = compareOperator == '>' ? xmasValue > compareValue : xmasValue < compareValue;
            }

            if (succeeds)
            {
                var outcome = splitColon.Last();
                if (outcome == "A")
                {
                    return (Outcome.Accept, null);
                }
                if (outcome == "R")
                {
                    return (Outcome.Reject, null);
                }
                return (Outcome.GoToWorkflow, outcome);
            }
            return (Outcome.ApplyNext, null);
        }
    }
}
