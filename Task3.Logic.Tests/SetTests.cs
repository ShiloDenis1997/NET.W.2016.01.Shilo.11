using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Task3.Logic.Tests
{
    [TestFixture]
    public class SetTests
    {
        public class ItemProductTest : IEquatable<SetTests.ItemProductTest>
        {
            public string Name { get; private set; }
            public decimal Price { get; private set; }

            public ItemProductTest(string name, decimal price)
            {
                Name = name;
                Price = price;
            }

            public bool Equals(ItemProductTest other)
            {
                if (ReferenceEquals(other, this))
                    return true;
                if (ReferenceEquals(other, null))
                    return false;
                if (other.GetType() != GetType())
                    return false;
                return string.Compare
                    (Name, other.Name, StringComparison.InvariantCulture) == 0;
            }
        }

        public static IEnumerable<TestCaseData> AddTestData
        {
            get
            {
                object o = new ItemProductTest[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                };
                yield return new TestCaseData(o);
                o = new ItemProductTest[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("table", 200m),
                    new ItemProductTest("ball", 40m),
                };
                yield return new TestCaseData(o);
            }
        }

        [TestCaseSource(nameof(AddTestData))]
        [Test]
        public void Add_Elements_SetWithElementsExpected
            (ItemProductTest[] data)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(1);
            //act
            foreach (ItemProductTest t in data)
                set.Add(t);
            //assert
            foreach (ItemProductTest t in data)
                Assert.AreEqual(true, set.Contains(t));
        }


        public static IEnumerable<TestCaseData> RemoveTestData
        {
            get
            {
                object dataIn = new ItemProductTest[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150), 
                };
                object dataRemove = new[]
                {
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                };
                object dataExpected = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("pen", 150),
                };
                object dataUnexpected = new[]
                {
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                };
                yield return new TestCaseData(dataIn,dataRemove, dataExpected, dataUnexpected);
                
            }
        }

        [TestCaseSource(nameof(RemoveTestData))]
        [Test]
        public void Remove_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] dataRemove,
            ItemProductTest[] dataExpected, ItemProductTest[] dataUnexpected)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(1);
            //act
            foreach (ItemProductTest t in dataIn)
                set.Add(t);
            foreach (ItemProductTest t in dataRemove)
                set.Remove(t);
            //assert
            foreach (ItemProductTest t in dataExpected)
                Assert.AreEqual(true, set.Contains(t));
            foreach (ItemProductTest t in dataUnexpected)
                Assert.AreEqual(false, set.Contains(t));
        }
    }
}
