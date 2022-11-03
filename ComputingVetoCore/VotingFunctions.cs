using Microsoft.SolverFoundation.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputingVetoCore
{
    internal class VotingFunctions
    {
        internal static TiedWinners FindVetoByConsumptionWinners(Profile profile, int stopCandidate = -1)
        {
            return new TiedWinners(FindVetoByConsumptionLottery(profile, stopCandidate).Keys(), "HHA winners");
        }

        public static Lottery<Rational> FindVetoByConsumptionLottery(Profile profile, int stopCandidate = -1)
        {
            var remainingCapacity = new Dictionary<int, Rational>();
            var tracker = new WorstCandidateTracker(profile);
            var eatenBy = new Dictionary<int, HashSet<int>>();
            var eatenNext = new HashSet<int>();
            foreach (int tastyCandidate in profile.Candidates)
            {
                remainingCapacity[tastyCandidate] = 1;
            }

            while (tracker.RemainingCandidates.Count > 0)
            {
                foreach (int tastyCandidate in eatenNext)
                {
                    eatenBy.Remove(tastyCandidate);
                }
                foreach (int hungryAgent in profile.Voters)
                {
                    int tastyCandidate = tracker.IdOfWorstCandidate(hungryAgent);
                    if (!eatenBy.ContainsKey(tastyCandidate))
                    {
                        eatenBy[tastyCandidate] = new HashSet<int>();
                    }
                    eatenBy[tastyCandidate].Add(hungryAgent);
                }

                if (eatenBy.Keys.Contains(stopCandidate))
                {
                    var pseudoLottery = new Dictionary<int, Rational>();
                    foreach (int tastyCandidate in eatenBy.Keys)
                    {
                        pseudoLottery[tastyCandidate] = 1;
                    }
                    return new Lottery<Rational>(pseudoLottery, "Alex lottery");
                }

                Rational minTime = double.PositiveInfinity;
                eatenNext = new HashSet<int>();

                foreach (int tastyCandidate in eatenBy.Keys)
                {
                    int eatingSpeed = eatenBy[tastyCandidate].Count;
                    Rational newTime = remainingCapacity[tastyCandidate] / eatingSpeed;
                    if (newTime < minTime)
                    {
                        minTime = newTime;
                        eatenNext = new HashSet<int>();
                    }
                    if (newTime == minTime)
                    {
                        eatenNext.Add(tastyCandidate);
                    }
                }
                foreach (int tastyCandidate in eatenBy.Keys)
                {
                    int eatingSpeed = eatenBy[tastyCandidate].Count;
                    remainingCapacity[tastyCandidate] -= minTime * eatingSpeed;
                }
                tracker.RemainingCandidates.ExceptWith(eatenNext);
            }

            var lottery = new Dictionary<int, Rational>();
            foreach (int tastyCandidate in eatenNext)
            {
                int eatingSpeed = eatenBy[tastyCandidate].Count;
                lottery[tastyCandidate] = Rational.One * eatingSpeed / profile.NumberOfVoters;
            }
            return new Lottery<Rational>(lottery, "HHA lottery");
        }


        internal static TiedWinners FindVetoCore(
            Profile profile,
            CoreAlgorithm algo = CoreAlgorithm.MaxFlowLP
            )
        {
            (int, int) coefficients =
                Utilities.GetMoulinCoefficients(profile.NumberOfVoters, profile.NumberOfCandidates);
            int blockingBicliqueSize = profile.NumberOfCandidates * coefficients.Item2;

            var core = new HashSet<int>();

            foreach (int c in profile.Candidates)
            {

                int maxBicliqueSize;
                switch (algo)
                {
                    case CoreAlgorithm.MaxFlowLP:
                        {
                            maxBicliqueSize = Utilities.LargestBicliqueViaMaxFlow(
                                profile,
                                c,
                                coefficients.Item1,
                                coefficients.Item2,
                                Utilities.MaxFlowAlgo.LP);
                            break;
                        }
                    case CoreAlgorithm.LinearProgramming:
                        {
                            List<List<int>> blockingGraphComplementAdjacencyList =
                                Utilities.GetBipartiteComplementOfBlockingGraph(
                                    profile,
                                    c,
                                    coefficients.Item1,
                                    coefficients.Item2);
                            maxBicliqueSize = Utilities.LargestBicliqueViaLinearProgramming(
                                blockingGraphComplementAdjacencyList,
                                coefficients.Item1 * profile.NumberOfVoters,
                                coefficients.Item2 * (profile.NumberOfCandidates - 1));
                            break;
                        }
                    case CoreAlgorithm.Konig:
                        {
                            List<List<int>> blockingGraphComplementAdjacencyList =
                                Utilities.GetBipartiteComplementOfBlockingGraph(
                                    profile,
                                    c,
                                    coefficients.Item1,
                                    coefficients.Item2);
                            maxBicliqueSize = Utilities.LargestBicliqueViaKonig(
                                blockingGraphComplementAdjacencyList,
                                coefficients.Item1 * profile.NumberOfVoters,
                                coefficients.Item2 * (profile.NumberOfCandidates - 1));
                            break;
                        }
                    case CoreAlgorithm.MaxFlow:
                        {
                            maxBicliqueSize = Utilities.LargestBicliqueViaMaxFlow(
                                profile,
                                c,
                                coefficients.Item1,
                                coefficients.Item2,
                                Utilities.MaxFlowAlgo.MPM);
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException("No implementation for core algorithm.");
                        }
                };
                if (maxBicliqueSize < blockingBicliqueSize)
                {
                    core.Add(c);
                }
            }

            return new TiedWinners(core, "Veto core");
        }

        internal enum CoreAlgorithm
        {
            MaxFlowLP,
            LinearProgramming,
            Konig,
            MaxFlow
        }
    }
}