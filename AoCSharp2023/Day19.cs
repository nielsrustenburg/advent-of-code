using System.Data;

namespace AoCSharp2023
{
    internal class Day19
    {
        public Day19()
        {
            string[] lines = File.ReadAllLines("./inputs/19.txt");
            (Dictionary<string, Workflow> workflows, List<XMASPart> parts) = ReadInput(lines);
            Console.WriteLine(Part1(workflows, parts));
        }

        (Dictionary<string,Workflow> Workflows, List<XMASPart> Parts) ReadInput(string[] lines)
        {
            var workflows = new Dictionary<string,Workflow>();
            int index = 0;
            while (lines[index] != string.Empty)
            {
                var workflow = new Workflow(lines[index]);
                workflows.Add(workflow.Name, workflow);
                index++;
            }

            var parts = new List<XMASPart>();
            for(int i = index+1; i < lines.Length; i++)
            {
                parts.Add(new XMASPart(lines[i]));
            }

            return (workflows, parts);
        } 

        int Part1(Dictionary<string, Workflow> workflows, List<XMASPart> parts)
        {
            var accepted = new List<XMASPart>();
            foreach(var part in parts)
            {
                var workflowName = "in";
                var outcome = Outcome.GoToWorkflow;
                while(outcome == Outcome.GoToWorkflow)
                {
                    (outcome, workflowName) = workflows[workflowName].Apply(part);
                }
                if(outcome == Outcome.Accept)
                {
                    accepted.Add(part);
                }
            }

            return accepted.Sum(part => part.X + part.M + part.A + part.S);
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
            for(int i = 0; i < Rules.Count; i++)
            {
                var result = xmas.Apply(Rules[i]);
                if(result.Outcome != Outcome.ApplyNext)
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
