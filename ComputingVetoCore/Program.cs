using System;
using System.Collections.Generic;

namespace ComputingVetoCore
{


    public class Program
    {
        static void Main(string[] args)
        {

            IEnumerable<int> agentNumbers = new int[]
            {
                2,3,4,5,6,7,8,9,10
            };
            IEnumerable<int> candidateNumbers = new int[]
            {
                2,3,4,5,6,7,8,9,10
            };
            int iterations = 1;
            Func<Profile, TiedWinners> votingFunction = p => VotingFunctions.FindVetoCore(p);
            // or:
            //Func<Profile, TiedWinners> votingFunction = p => VotingFunctions.FindVetoByConsumptionWinners(p);
            Simulations.AverageNumberOfWinnersIC(
                agentNumbers,
                candidateNumbers,
                iterations,
                votingFunction
                );
            Console.ReadLine();
        }
    }

}
