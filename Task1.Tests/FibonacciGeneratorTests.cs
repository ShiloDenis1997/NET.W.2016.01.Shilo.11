using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Task1.LogicFibonacci;

namespace Task1.Tests
{
    [TestFixture]
    public class FibonacciGeneratorTests
    {
        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(4, 3)]
        [TestCase(5, 5)]
        [Test]
        public void GetFibonacciSequence_Pos_NumberExpected(int pos, long expectedFibonacciNumber)
        {
            IEnumerator<long> fibSeq = FibonacciGenerator.GetFibonacciSequence().GetEnumerator();
            bool b = false;
            for (int i = 0; i < pos; i++)
                b = fibSeq.MoveNext();
            if (!b)
                Assert.Fail($"{pos} fibonacci number cannot be method");
            Assert.AreEqual(expectedFibonacciNumber, fibSeq.Current, $"{nameof(pos)} = {pos}");
        }
    }
}
