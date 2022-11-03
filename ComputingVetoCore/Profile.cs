using System.Collections.Generic;
using System.Linq;

namespace ComputingVetoCore
{
    internal class Profile
    {
        internal int NumberOfVoters;
        internal int NumberOfCandidates;
        private int[,] _profile;

        internal IEnumerable<int> Candidates
        {
            get
            {
                return Enumerable.Range(0, NumberOfCandidates);
            }
        }

        internal IEnumerable<int> Voters
        {
            get
            {
                return Enumerable.Range(0, NumberOfVoters);
            }
        }

        internal int AgentsIthChoice(int agent, int i)
        {
            return _profile[agent, i];
        }

        internal Profile(int[,] preferenceMatrix)
        {
            NumberOfVoters = preferenceMatrix.GetLength(0);
            NumberOfCandidates = preferenceMatrix.GetLength(1);
            _profile = preferenceMatrix;
        }

        internal static Profile GenerateICProfile(int numberOfAgents, int numberOfCandidates)
        {
            var profile = new int[numberOfAgents, numberOfCandidates];

            for (int i = 0; i < numberOfAgents; i++)
            {
                for (int j = 0; j < numberOfCandidates; j++)
                {
                    profile[i, j] = j;
                }
                Utilities.ShuffleRow(profile, i);
            }
            return new Profile(profile);
        }

    }
}