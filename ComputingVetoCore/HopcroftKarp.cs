using System.Collections.Generic;
using System.Linq;

namespace ComputingVetoCore
{
    class HopcroftKarp
    {

        private const int UNMATCHED = -1;

        public static int FindLargestMatching(
            HashSet<int> leftNodes,
            IEnumerable<int> rightNodes,
            Dictionary<int, HashSet<int>> adjacencyList)
        {

            var distances = new Dictionary<int, int>();
            var queue = new Queue<int>();
            Dictionary<int, int> leftMatching = leftNodes.ToDictionary(
                leftNode => leftNode,
                matchedNode => UNMATCHED);
            Dictionary<int, int> rightMatching = rightNodes.ToDictionary(
                rightNode => rightNode,
                matchedNode => UNMATCHED);

            while (HasAugmentingPath(
                leftNodes,
                adjacencyList,
                leftMatching,
                rightMatching,
                distances,
                queue))
            {
                foreach (int node in leftNodes.Where(left => IsUnmatched(left, leftMatching)))
                {
                    MatchNode(node, adjacencyList, leftMatching, rightMatching, distances);
                }
            }

            foreach (int unmatched in leftMatching.Keys.Where(x => IsUnmatched(x, leftMatching)).ToList())
            {
                leftMatching.Remove(unmatched);
            }
            return leftMatching.Keys.Count;
        }

        private static bool IsUnmatched(int node, Dictionary<int, int> matching)
        {
            return matching[node] == UNMATCHED;
        }

        private static bool HasAugmentingPath(
            IEnumerable<int> leftNodes,
            Dictionary<int, HashSet<int>> adjacencyList,
            Dictionary<int, int> leftMatching,
            Dictionary<int, int> rightMatching,
            Dictionary<int, int> distances,
            Queue<int> queue)
        {
            foreach (int node in leftNodes)
            {
                if (IsUnmatched(node, leftMatching))
                {
                    distances[node] = 0;
                    queue.Enqueue(node);
                }
                else
                {
                    distances[node] = int.MaxValue;
                }
            }

            distances[UNMATCHED] = int.MaxValue;

            while (queue.Count > 0)
            {
                int leftNode = queue.Dequeue();
                if (distances[leftNode] < distances[UNMATCHED])
                {
                    foreach (int right in adjacencyList[leftNode])
                    {
                        int nextLeft = rightMatching[right];
                        if (distances[nextLeft] == int.MaxValue)
                        {
                            distances[nextLeft] = distances[leftNode] + 1;
                            queue.Enqueue(nextLeft);
                        }
                    }
                }
            }
            return distances[UNMATCHED] != int.MaxValue;
        }

        private static bool MatchNode(
            int leftNode,
            Dictionary<int, HashSet<int>> adjacencyList,
            Dictionary<int, int> leftMatching,
            Dictionary<int, int> rightMatching,
            Dictionary<int, int> distances)
        {
            if (leftNode == UNMATCHED)
            {
                return true;
            }

            foreach (int rightNode in adjacencyList[leftNode])
            {
                int nextLeft = rightMatching[rightNode];
                if (distances[nextLeft] == distances[leftNode] + 1)
                {
                    if (MatchNode(nextLeft, adjacencyList, leftMatching, rightMatching, distances))
                    {
                        rightMatching[rightNode] = leftNode;
                        leftMatching[leftNode] = rightNode;
                        return true;
                    }
                }
            }
            distances[leftNode] = int.MaxValue;
            return false;
        }
    }
}
