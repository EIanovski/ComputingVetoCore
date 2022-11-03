using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputingVetoCore
{
    internal class Simulations
    {
        internal static void AverageNumberOfWinnersIC(IEnumerable<int> agentNumbers,
            IEnumerable<int> candidateNumbers,
            int repetitions,
            Func<Profile, IEnumerable<int>> votingRule)
        {
            AverageNumberOfWinners(agentNumbers,
                candidateNumbers,
                repetitions,
                votingRule,
                (agents, candidates) => Profile.GenerateICProfile(agents, candidates));
        }

        internal static void AverageNumberOfWinners(
            IEnumerable<int> agentNumbers,
            IEnumerable<int> candidateNumbers,
            int repetitions,
            Func<Profile, IEnumerable<int>> votingRule,
            Func<int, int, Profile> generateProfile)
        {
            double[,] numberOfWinners = Utilities.ProduceNbyMTable(
                agentNumbers,
                candidateNumbers,
                (agents, candidates) =>
                {
                    double aggregateNumberOfWinners = 0;
                    Parallel.For(0, repetitions, (i, state) =>
                    {
                        Profile profile = generateProfile(agents, candidates);
                        double numWinners = votingRule(profile).Count();
                        Utilities.ParallelAdd(ref aggregateNumberOfWinners, numWinners);
                    });
                    return aggregateNumberOfWinners / repetitions;
                });
            Console.WriteLine("Number of winners:");

            Console.WriteLine(
                Utilities.FormatLatexTable(
                    candidateNumbers.Select(x => x.ToString()).ToArray(),
                    agentNumbers.Select(x => x.ToString()).ToArray(),
                    numberOfWinners));

            double[,] proportionOfWinners = NumberToProportion(numberOfWinners);

            Console.WriteLine("Proportion of winners:");

            Console.WriteLine(
                Utilities.FormatLatexTable(
                    candidateNumbers.Select(x => x.ToString()).ToArray(),
                    agentNumbers.Select(x => x.ToString()).ToArray(),
                    proportionOfWinners,
                    x => String.Format("{0:0.00}", x)));
        }

        private static double[,] NumberToProportion(double[,] numberOfWinners)
        {
            int numAgents = numberOfWinners.GetLength(0);
            int numCandidates = numberOfWinners.GetLength(1);
            double[,] proportionWinners = new double[numAgents, numCandidates];
            for (int i = 0; i < numAgents; i++)
            {
                for (int j = 0; j < numCandidates; j++)
                {
                    proportionWinners[i, j] = numberOfWinners[i, j] / j;
                }
            }
            return proportionWinners;
        }

    }
}