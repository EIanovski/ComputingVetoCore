using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputingVetoCore
{
    internal class TiedWinners : ElectionResult, IEnumerable<int>
    {
        IEnumerable<int> _winners;
        string _name;

        public int GetNumberOfWinners()
        {
            return _winners.Count();
        }

        public HashSet<int> GetWinners()
        {
            return new HashSet<int>(_winners);
        }

        public TiedWinners(IEnumerable<int> winners, string name)
        {
            _winners = winners;
            _name = name;
        }

        public void Print()
        {
            StringBuilder output = new StringBuilder();
            foreach (int winner in _winners)
            {
                output.Append(winner + ", ");
            }
            output.Remove(output.Length - 2, 2);
            Console.WriteLine(output.ToString());
        }

        public string GetName()
        {
            return _name;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return _winners.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}