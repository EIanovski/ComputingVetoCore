using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using Google.OrTools.LinearSolver;

namespace ComputingVetoCore
{
    internal class Utilities
    {
        internal static (int, int) GetMoulinCoefficients(int n, int m)
        {
            int[] BezoutCoefficients = ExtendedEuclidean(n, m);
            int gcd = BezoutCoefficients[0];
            int r = BezoutCoefficients[1] * -1;
            int t = BezoutCoefficients[2];

            while (t <= gcd * n || r <= 0 || t <= 0)
            {
                r += m;
                t += n;
            }
            return (r, t);
        }

        internal static int[] ExtendedEuclidean(int a, int b)
        {
            int[] retvals = { 0, 0, 0 };
            int[] aa = { 1, 0 };
            int[] bb = { 0, 1 };
            int q = 0;

            while (true)
            {
                q = a / b;
                a = a % b;
                aa[0] = aa[0] - q * aa[1];
                bb[0] = bb[0] - q * bb[1];
                if (a == 0)
                {
                    retvals[0] = b; retvals[1] = aa[1]; retvals[2] = bb[1];
                    return retvals;
                };
                q = b / a;
                b = b % a;
                aa[1] = aa[1] - q * aa[0];
                bb[1] = bb[1] - q * bb[0];
                if (b == 0)
                {
                    retvals[0] = a; retvals[1] = aa[0]; retvals[2] = bb[0];
                    return retvals;
                };
            }
        }

        internal static int LargestBicliqueViaMaxFlow(
            Profile profile,
            int candidate,
            int r,
            int t,
            MaxFlowAlgo algo)
        {
            MPM.Graph flowGraph =
                GetComplementFlowGraph(profile, candidate, r, t);
            int largestMatching;
            switch (algo)
            {
                case MaxFlowAlgo.MPM:
                    largestMatching = MPM.MaxFlow(flowGraph);
                    break;
                case MaxFlowAlgo.LP:
                    largestMatching = MaxFlowViaLP(flowGraph);
                    break;
                default:
                    throw new Exception("No implementation for max flow algorithm");
            }
            return (profile.NumberOfVoters * r + (profile.NumberOfCandidates - 1) * t) - largestMatching;
        }

        private static MPM.Graph GetComplementFlowGraph(Profile profile, int c, int r, int t)
        {
            MPM.Graph G = new MPM.Graph();

            foreach (int agent in profile.Voters)
            {
                int agentNode = agent;
                G.AddEdge(MPM.SOURCE, agent, r);
                int worstCandidateIndex = profile.NumberOfCandidates - 1;
                for (int candidateIndex = worstCandidateIndex; candidateIndex >= 0; candidateIndex--)
                {
                    int candidate = profile.AgentsIthChoice(agent, candidateIndex);
                    if (candidate == c)
                    {
                        break;
                    }
                    int candidateNode = profile.NumberOfVoters + candidate;
                    G.AddEdge(agent, candidateNode, r);
                    G.AddEdge(candidateNode, MPM.SINK, t);
                }
            }
            return G;
        }

        internal enum MaxFlowAlgo
        {
            MPM,
            LP
        }

        private static int MaxFlowViaLP(MPM.Graph G)
        {
            if (G.InDegree(MPM.SINK) == 0)
            {
                return 0;
            }
            Solver solver = new Solver("MaxFloweriser", Solver.OptimizationProblemType.GLOP_LINEAR_PROGRAMMING);
            var flowVariables = new Dictionary<int, Dictionary<int, Variable>>();
            Dictionary<int, Dictionary<int, int>> emptyFlow = G.GetEmptyFlow();
            MakeCapacityConstraints(emptyFlow, G, solver, flowVariables);
            MakeBalancedFlowConstraints(emptyFlow.Keys, G, solver, flowVariables);
            Objective objective = solver.Objective();
            foreach (int node in G.InNeighbours(MPM.SINK))
            {
                objective.SetCoefficient(flowVariables[node][MPM.SINK], 1);
            }
            objective.SetMaximization();

            solver.Solve();

            return (int)solver.Objective().Value();
        }

        private static void MakeCapacityConstraints(
            Dictionary<int, Dictionary<int, int>> flow,
            MPM.Graph G,
            Solver solver,
            Dictionary<int, Dictionary<int, Variable>> flowVariables
            )
        {
            foreach (int node in flow.Keys)
            {
                flowVariables[node] = new Dictionary<int, Variable>();
                foreach (int neighbour in flow[node].Keys)
                {
                    flowVariables[node][neighbour] = (solver.MakeNumVar(0, G.GetEdgeCapacity(node, neighbour), node + ", " + neighbour));
                    Constraint ct = solver.MakeConstraint(0, G.GetEdgeCapacity(node, neighbour), "Capacity constraint");
                    ct.SetCoefficient(flowVariables[node][neighbour], 1);
                }
            }
        }

        private static void MakeBalancedFlowConstraints(
            IEnumerable<int> nodes,
            MPM.Graph G,
            Solver solver,
            Dictionary<int, Dictionary<int, Variable>> flowVariables
            )
        {
            foreach (int node in nodes)
            {
                if (MPM.IsInnerNode(node))
                {
                    IEnumerable<int> inNeighbours = G.InNeighbours(node);
                    IEnumerable<int> outNeighbours = G.OutNeighbours(node);
                    Constraint ct = solver.MakeConstraint(0, 0, "Balanced flow constraint");
                    foreach (int neighbour in inNeighbours)
                    {
                        ct.SetCoefficient(flowVariables[neighbour][node], 1);
                    }
                    foreach (int neighbour in outNeighbours)
                    {
                        ct.SetCoefficient(flowVariables[node][neighbour], -1);
                    }
                }
            }
        }

        public static List<List<int>> GetBipartiteComplementOfBlockingGraph(
            Profile profile,
            int blockedCandidate,
            int agentClones,
            int candidateClones)
        {
            var nonAdjacencyList = new List<List<int>>();

            for (int i = 0; i < profile.NumberOfVoters * agentClones; i++)
            {
                nonAdjacencyList.Add(new List<int>());
            }

            foreach (int agent in profile.Voters)
            {
                int worstCandidateIndex = profile.NumberOfCandidates - 1;
                for (int candidateIndex = worstCandidateIndex; candidateIndex >= 0; candidateIndex--)
                {
                    int candidate = profile.AgentsIthChoice(agent, candidateIndex);
                    if (candidate == blockedCandidate)
                    {
                        break;
                    }

                    for (int agentCloneIndex = 0; agentCloneIndex < agentClones; agentCloneIndex++)
                    {
                        int agentIndex = agent * agentClones + agentCloneIndex;
                        for (int candidateCloneIndex = 0; candidateCloneIndex < candidateClones; candidateCloneIndex++)
                        {
                            int candidateGraphLabel =
                                GetGraphLabel(
                                    candidate,
                                    candidateCloneIndex,
                                    candidateClones,
                                    blockedCandidate);
                            nonAdjacencyList[agentIndex].Add(candidateGraphLabel);
                        }
                    }

                }
            }
            return nonAdjacencyList;
        }

        private static int GetGraphLabel(
            int candidate,
            int cloneIndex,
            int numberOfClones,
            int blockedCandidate)
        {
            if (candidate < blockedCandidate)
            {
                return candidate * numberOfClones + cloneIndex;
            }
            else
            {
                return (candidate - 1) * numberOfClones + cloneIndex;
            }
        }

        internal static int LargestBicliqueViaLinearProgramming(List<List<int>> adjacencyListOfComplement, int numberOnLeft, int numberOnRight)
        {
            /*
			 *  Expected input: A list of lists where the first level keys correspond to
			 *  vertices on the left, and the IEnumerable at that key is the list of vertices
			 *  on the right that are NOT adjacent to it. All vertices on a given side are
			 *  to be labelled 0 through to the maximum value.
			 */
            Solver solver = new Solver("Bicliqueriser", Solver.OptimizationProblemType.GLOP_LINEAR_PROGRAMMING);

            var leftVariables = new List<Variable>();
            for (int i = 0; i < numberOnLeft; i++)
            {
                leftVariables.Add(solver.MakeNumVar(0.0, 1.0, "L" + i));
            }
            var rightVariables = new List<Variable>();
            for (int i = 0; i < numberOnRight; i++)
            {
                rightVariables.Add(solver.MakeNumVar(0.0, 1.0, "R" + i));
            }

            for (int i = 0; i < numberOnLeft; i++)
            {
                List<int> nonAdjacentVertices = adjacencyListOfComplement[i];
                for (int j = 0; j < nonAdjacentVertices.Count; j++)
                {
                    Constraint ct = solver.MakeConstraint(0.0, 1.0, "Non-adjacent vertices");
                    ct.SetCoefficient(leftVariables[i], 1);
                    ct.SetCoefficient(rightVariables[nonAdjacentVertices[j]], 1);
                }
            }

            Objective objective = solver.Objective();
            foreach (Variable x in leftVariables)
            {
                objective.SetCoefficient(x, 1);
            }
            foreach (Variable x in rightVariables)
            {
                objective.SetCoefficient(x, 1);
            }
            objective.SetMaximization();

            solver.Solve();

            return (int)solver.Objective().Value();
        }

        internal static int LargestBicliqueViaKonig(
            List<List<int>> adjacencyList,
            int nodesOnLeft,
            int nodesOnRight)
        {
            var leftAdjacencyList = new Dictionary<int, HashSet<int>>();
            for (int leftNode = 0; leftNode < nodesOnLeft; leftNode++)
            {
                leftAdjacencyList[leftNode] = new HashSet<int>(adjacencyList[leftNode]);
            }
            int largestMatching = HopcroftKarp.FindLargestMatching(
                new HashSet<int>(Enumerable.Range(0, nodesOnLeft)),
                Enumerable.Range(0, nodesOnRight),
                leftAdjacencyList);
            return (nodesOnLeft + nodesOnRight) - largestMatching;
        }


        internal static double ParallelAdd(ref double location1, double value)
        {
            double newCurrentValue = location1;
            while (true)
            {
                double currentValue = newCurrentValue;
                double newValue = currentValue + value;
                newCurrentValue = Interlocked.CompareExchange(ref location1, newValue, currentValue);
                if (newCurrentValue == currentValue)
                    return newValue;
            }
        }

        internal static void ShuffleRow(int[,] array, int row)
        {
            int n = array.GetLength(1);
            while (n > 1)
            {
                n--;
                int i = ThreadLocalRandom.Next(n + 1);
                int temp = array[row, i];
                array[row, i] = array[row, n];
                array[row, n] = temp;
            }
        }


        internal static string FormatLatexTable<T>(
            string[] Xlabels,
            string[] Ylabels,
            T[,] data,
            Func<T, string> format = null)
        {
            if (format == null)
            {
                format = x => x.ToString();
            }
            var output = new StringBuilder();
            output.Append(@"\begin{tabular}{");
            output.Append("c | ");
            for (int i = 0; i < Xlabels.Length; i++)
            {
                output.Append("c ");
            }
            output.Append(@"}");
            output.Append(Environment.NewLine);

            output.Append("& ");
            for (int i = 0; i < Xlabels.Length; i++)
            {
                output.Append(Xlabels[i]);
                if (i < Xlabels.Length - 1)
                {
                    output.Append(@" & ");
                }
            }
            output.Append(@"\\");
            output.Append(Environment.NewLine);
            output.Append(@"\hline");
            output.Append(Environment.NewLine);
            for (int i = 0; i < Ylabels.Length; i++)
            {
                output.Append(Ylabels[i]);
                output.Append(@" & ");
                for (int j = 0; j < Xlabels.Length; j++)
                {
                    output.Append(format(data[j, i]));
                    if (j < Xlabels.Length - 1)
                    {
                        output.Append(@" & ");
                    }
                }
                output.Append(@"\\");
                output.Append(Environment.NewLine);
            }
            output.Append(@"\end{tabular}");
            return output.ToString();
        }


        internal static T[,] ProduceNbyMTable<T>(
            IEnumerable<int> agentNumbers,
            IEnumerable<int> candidateNumbers,
            Func<int, int, T> resultFunction)
        {
            T[,] results = new T[candidateNumbers.Count(), agentNumbers.Count()];
            for (int i = 0; i < agentNumbers.Count(); i++)
            {
                for (int j = 0; j < candidateNumbers.Count(); j++)
                {
                    T result = resultFunction(agentNumbers.ElementAt(i), candidateNumbers.ElementAt(j));
                    results[i, j] = result;
                }
            }
            return results;
        }
    }
}