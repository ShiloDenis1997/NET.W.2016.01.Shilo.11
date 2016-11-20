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
        public class ItemProductTest : IEquatable<ItemProductTest>
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

        public static IEnumerable<TestCaseData> UnionTestData
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
                object dataUnion = new[]
                {
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("book", 10m), 
                };
                object dataExpected = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                    new ItemProductTest("book", 10m),
                };
                yield return new TestCaseData(dataIn, dataUnion, dataExpected);

            }
        }

        [TestCaseSource(nameof(UnionTestData))]
        [Test]
        public void UnionWith_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] dataUnion,
            ItemProductTest[] dataExpected)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(1);
            foreach (ItemProductTest t in dataIn)
                set.Add(t);
            //act
            set.UnionWith(dataUnion);
            //assert
            CollectionAssert.AreEquivalent(dataExpected, set);
            CollectionAssert.AllItemsAreUnique(set);
        }

        [TestCaseSource(nameof(UnionTestData))]
        [Test]
        public void UnionStatic_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] dataUnion,
            ItemProductTest[] dataExpected)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(1);
            foreach (ItemProductTest t in dataIn)
                set.Add(t);
            //act
            Set<ItemProductTest> actual = Set<ItemProductTest>.Union(set, dataUnion);
            //assert
            CollectionAssert.AreEquivalent(dataExpected, actual);
            CollectionAssert.AllItemsAreUnique(actual);
        }

        public static IEnumerable<TestCaseData> IntersectTestData
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
                object dataIntersect = new[]
                {
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("book", 10m),
                };
                object dataExpected = new[]
                {
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                };
                object dataUnexpected = new[]
                {
                    new ItemProductTest("book", 10m),
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("pen", 150),
                };
                yield return new TestCaseData
                    (dataIn, dataIntersect, dataExpected, dataUnexpected);

            }
        }

        [TestCaseSource(nameof(IntersectTestData))]
        [Test]
        public void IntersectWith_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] dataIntersect,
            ItemProductTest[] dataExpected, ItemProductTest[] dataUnexpected)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(1);
            foreach (ItemProductTest t in dataIn)
                set.Add(t);
            //act
            set.IntersectWith(dataIntersect);
            //assert
            CollectionAssert.IsNotSubsetOf(dataUnexpected, set);
            CollectionAssert.AreEquivalent(dataExpected, set);
            CollectionAssert.AllItemsAreUnique(set);
        }

        [TestCaseSource(nameof(IntersectTestData))]
        [Test]
        public void IntersectStatic_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] dataIntersect,
            ItemProductTest[] dataExpected, ItemProductTest[] dataUnexpected)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(1);
            foreach (ItemProductTest t in dataIn)
                set.Add(t);
            //act
            Set<ItemProductTest> actual = Set<ItemProductTest>.Intersect(set, dataIntersect);
            //assert
            CollectionAssert.IsNotSubsetOf(dataUnexpected, actual);
            CollectionAssert.AreEquivalent(dataExpected, actual);
            CollectionAssert.AllItemsAreUnique(actual);
        }

        public static IEnumerable<TestCaseData> ExceptWithTestData
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
                object dataExcept = new[]
                {
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("book", 10m),
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
                    new ItemProductTest("book", 10m),
                };
                yield return new TestCaseData
                    (dataIn, dataExcept, dataExpected, dataUnexpected);

            }
        }

        [TestCaseSource(nameof(ExceptWithTestData))]
        [Test]
        public void ExceptWith_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] dataExcept,
            ItemProductTest[] dataExpected, ItemProductTest[] dataUnexpected)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(1);
            foreach (ItemProductTest t in dataIn)
                set.Add(t);
            //act
            set.ExceptWith(dataExcept);
            //assert
            CollectionAssert.IsNotSubsetOf(dataUnexpected, set);
            CollectionAssert.AreEquivalent(dataExpected, set);
            CollectionAssert.AllItemsAreUnique(set);
        }

        [TestCaseSource(nameof(ExceptWithTestData))]
        [Test]
        public void ExceptStatic_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] dataExcept,
            ItemProductTest[] dataExpected, ItemProductTest[] dataUnexpected)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(1);
            foreach (ItemProductTest t in dataIn)
                set.Add(t);
            //act
            Set<ItemProductTest> actual = Set<ItemProductTest>.Except(set, dataExcept);
            //assert
            CollectionAssert.IsNotSubsetOf(dataUnexpected, actual);
            CollectionAssert.AreEquivalent(dataExpected, actual);
            CollectionAssert.AllItemsAreUnique(actual);
        }

        public static IEnumerable<TestCaseData> SymmetricExceptWithTestData
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
                object dataExcept = new[]
                {
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("book", 10m),
                };
                object dataExpected = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("book", 10m),
                    new ItemProductTest("pen", 150),
                };
                object dataUnexpected = new[]
                {
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                };
                yield return new TestCaseData
                    (dataIn, dataExcept, dataExpected, dataUnexpected);

            }
        }

        [TestCaseSource(nameof(SymmetricExceptWithTestData))]
        [Test]
        public void SymmetricExceptWith_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] dataSymmetricExcept,
            ItemProductTest[] dataExpected, ItemProductTest[] dataUnexpected)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(1);
            //act
            foreach (ItemProductTest t in dataIn)
                set.Add(t);
            set.SymmetricExceptWith(dataSymmetricExcept);
            //assert
            CollectionAssert.IsNotSubsetOf(dataUnexpected, set);
            CollectionAssert.AreEquivalent(dataExpected, set);
            CollectionAssert.AllItemsAreUnique(set);
        }

        [TestCaseSource(nameof(SymmetricExceptWithTestData))]
        [Test]
        public void SymmetricExceptionStatic_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] dataSymmetricExcept,
            ItemProductTest[] dataExpected, ItemProductTest[] dataUnexpected)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(1);
            foreach (ItemProductTest t in dataIn)
                set.Add(t);
            //act
            Set<ItemProductTest> actual = Set<ItemProductTest>.SymmetricExcept
                (set, dataSymmetricExcept);
            //assert
            CollectionAssert.IsNotSubsetOf(dataUnexpected, actual);
            CollectionAssert.AreEquivalent(dataExpected, actual);
            CollectionAssert.AllItemsAreUnique(actual);
        }

        public static IEnumerable<TestCaseData> IsSubsetOfTestData
        {
            get
            {
                object dataIn = new []
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                object superset = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                yield return new TestCaseData
                    (dataIn, superset, true);
                dataIn = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                superset = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                    new ItemProductTest("piano", 350m), 
                };
                yield return new TestCaseData
                    (dataIn, superset, true);
                dataIn = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                superset = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                };
                yield return new TestCaseData
                    (dataIn, superset, false);
            }
        }

        [TestCaseSource(nameof(IsSubsetOfTestData))]
        [Test]
        public void IsSubsetOf_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] superset, bool expectedAns)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(dataIn);
            //act
            bool res = set.IsSubsetOf(superset);
            //assert
            Assert.AreEqual(expectedAns, res);
        }

        [TestCaseSource(nameof(IsSubsetOfTestData))]
        [Test]
        public void IsSupersetOf_Elements_SetWithElementsExpected
            (ItemProductTest[] subset, ItemProductTest[] dataIn, bool expectedAns)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(dataIn);
            //act
            bool res = set.IsSupersetOf(subset);
            //assert
            Assert.AreEqual(expectedAns, res);
        }

        public static IEnumerable<TestCaseData> IsProperSubsetOfTestData
        {
            get
            {
                object dataIn = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                object superset = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                yield return new TestCaseData
                    (dataIn, superset, false);
                dataIn = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                superset = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                    new ItemProductTest("piano", 350m),
                };
                yield return new TestCaseData
                    (dataIn, superset, true);
                dataIn = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                superset = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                };
                yield return new TestCaseData
                    (dataIn, superset, false);
            }
        }

        [TestCaseSource(nameof(IsProperSubsetOfTestData))]
        [Test]
        public void IsProperSubsetOf_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] superset, bool expectedAns)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(dataIn);
            //act
            bool res = set.IsProperSubsetOf(superset);
            //assert
            Assert.AreEqual(expectedAns, res);
        }

        [TestCaseSource(nameof(IsProperSubsetOfTestData))]
        [Test]
        public void IsProperSupersetOf_Elements_SetWithElementsExpected
            (ItemProductTest[] properSubset, ItemProductTest[] dataIn, bool expectedAns)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(dataIn);
            //act
            bool res = set.IsProperSupersetOf(properSubset);
            //assert
            Assert.AreEqual(expectedAns, res);
        }

        public static IEnumerable<TestCaseData> OverlapsTestData
        {
            get
            {
                object dataIn = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                object otherData = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                yield return new TestCaseData
                    (dataIn, otherData, true);
                dataIn = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                otherData = new[]
                {
                    new ItemProductTest("table", 100m),
                    new ItemProductTest("chaisasdr", 200m),
                    new ItemProductTest("baladl", 40m),
                    new ItemProductTest("peasdn", 150),
                    new ItemProductTest("piasdano", 350m),
                };
                yield return new TestCaseData
                    (dataIn, otherData, true);
                dataIn = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                otherData = new[]
                {
                    new ItemProductTest("tablasdae", 100m),
                    new ItemProductTest("chsdfair", 200m),
                    new ItemProductTest("basdfll", 40m),
                };
                yield return new TestCaseData
                    (dataIn, otherData, false);
            }
        }

        [TestCaseSource(nameof(OverlapsTestData))]
        [Test]
        public void Overlaps_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] otherData, bool expectedAns)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(dataIn);
            //act
            bool res = set.Overlaps(otherData);
            //assert
            Assert.AreEqual(expectedAns, res);
        }

        public static IEnumerable<TestCaseData> SetEqualsTestData
        {
            get
            {
                object dataIn = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                object superset = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                yield return new TestCaseData
                    (dataIn, superset, true);
                dataIn = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                superset = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                    new ItemProductTest("piano", 350m),
                };
                yield return new TestCaseData
                    (dataIn, superset, false);
                dataIn = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                superset = new[]
                {
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                    new ItemProductTest("table", 100m),
                    null,
                    new ItemProductTest("chair", 200m),
                    new ItemProductTest("ball", 40m),
                    new ItemProductTest("pen", 150),
                };
                yield return new TestCaseData
                    (dataIn, superset, true);
            }
        }

        [TestCaseSource(nameof(SetEqualsTestData))]
        [Test]
        public void SetEquals_Elements_SetWithElementsExpected
            (ItemProductTest[] dataIn, ItemProductTest[] otherData, bool expectedAns)
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>(dataIn);
            //act
            bool res = set.SetEquals(otherData);
            //assert
            Assert.AreEqual(expectedAns, res);
        }

        [Test]
        public void SetEnumerating_ExceptionExpected()
        {
            //arrange
            Set<ItemProductTest> set = new Set<ItemProductTest>()
            {
                new ItemProductTest("ball", 12m),
                new ItemProductTest("chair", 14m),
            };
            //assert
            Assert.Throws(typeof(InvalidOperationException),
                () =>
                {
                    foreach (var v in set)
                    {
                        set.Add(new ItemProductTest("new", 1000m));
                    }
                });
        }
    }
}