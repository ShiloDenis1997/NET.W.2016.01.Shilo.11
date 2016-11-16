using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Task1.LogicFibonacci
{
    public static class FibonacciGenerator
    {
        /// <summary>
        /// Creates sequence of Fibonacci nubmers
        /// </summary>
        public static IEnumerable<long> GetFibonacciSequence()
        {
            yield return 1;
            yield return 1;
            long pred = 1;
            long cur = 1;
            while (true)
            {
                long temp;
                try
                {
                    temp = checked(pred + cur);
                }
                catch (OverflowException)
                {
                    yield break;
                }
                yield return temp;
                pred = cur;
                cur = temp;
            }
        }
    }
}
