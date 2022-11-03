using System.Collections.Generic;

namespace ComputingVetoCore
{
  internal interface ElectionResult
        {
            void Print();

            int GetNumberOfWinners();

            HashSet<int> GetWinners();

            string GetName();
        }
}

