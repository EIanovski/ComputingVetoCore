using System;
using System.Collections;
using System.Collections.Generic;

namespace ComputingVetoCore
{
    public class Lottery<T> : ElectionResult, IEnumerable<KeyValuePair<int, T>>
    {
        Dictionary<int, T> _lottery;
        string _name;

        public T this[int index]
        {
            get
            {
                return _lottery[index];
            }

            set
            {
                _lottery[index] = value;
            }
        }

        public Lottery(Dictionary<int, T> lottery, string name)
        {
            _lottery = lottery;
            _name = name;
        }

        public IEnumerable<int> Keys()
        {
            return _lottery.Keys;
        }

        public bool ContainsKey(int key)
        {
            return _lottery.ContainsKey(key);
        }

        public void Print()
        {
            foreach (int key in _lottery.Keys)
            {
                Console.WriteLine(key + ": " + _lottery[key]);
            }
        }

        public string GetName()
        {
            return _name;
        }

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            return _lottery.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int GetNumberOfWinners()
        {
            return _lottery.Count;
        }

        public HashSet<int> GetWinners()
        {
            return new HashSet<int>(_lottery.Keys);
        }
    }
}