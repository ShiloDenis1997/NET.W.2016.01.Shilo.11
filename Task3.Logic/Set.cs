using System;
using System.Collections;
using System.Collections.Generic;

namespace Task3.Logic
{
    /// <summary>
    /// Provides functionality of set of unique elements
    /// </summary>
    public class Set<T> : ISet<T> where T : class, IEquatable<T>
    {
        /// <summary>
        /// inner storage for elements of set
        /// </summary>
        private T[] array;
        private readonly IEqualityComparer<T> equalityComparer;

        /// <summary>
        /// Initializes an instance of <see cref="Set{T}"/> with specified
        /// <paramref name="capacity"/> and <paramref name="equalityComparer"/>
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="equalityComparer">Comparer to compare 
        /// elements of the set</param>
        /// <exception cref="ArgumentException">Throws if capacity
        /// is less or equal to zero</exception>
        public Set(int capacity = 50, IEqualityComparer<T> equalityComparer = null)
        {
            if (capacity <= 0)
                throw new ArgumentException(
                    $"{nameof(capacity)} is less or equal to zero");
            array = new T[capacity];
            this.equalityComparer = equalityComparer;
        }

        /// <summary>
        /// Initializes an instance of <see cref="Set{T}"/> with specified
        /// <paramref name="capacity"/> and <paramref name="equalityComparer"/>, 
        /// that based on enumeration of <paramref name="elements"/>
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="equalityComparer">Comparer to compare 
        /// elements of the set</param>
        /// <param name="elements"></param>
        /// <exception cref="ArgumentException">Throws if capacity
        /// is less or equal to zero</exception>
        /// <exception cref="ArgumentNullException">Throws if
        /// <paramref name="elements"/> is null</exception>
        public Set(IEnumerable<T> elements, IEqualityComparer<T> equalityComparer = null,
            int capacity = 50)
        {
            if (elements == null)
                throw new ArgumentNullException
                    ($"{nameof(elements)} parameter is null");
            if (capacity <= 0)
                throw new ArgumentException(
                    $"{nameof(capacity)} is less or equal to zero");
            this.equalityComparer = equalityComparer;
            UnionWith(elements);
        }

        /// <summary>
        /// Number of elements in the set
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// Wether set is readonly
        /// </summary>
        public bool IsReadOnly => false;
        
        /// <summary>
        /// Adds item in the set
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true if item was added to the set, false if item is
        /// already in the set</returns>
        public bool Add(T item)
        {
            if (Contains(item))
                return false;
            if (Count == array.Length)
                ResizeArray(array.Length*2);
            array[Count++] = item;
            return true;
        }

        /// <summary>
        /// Unions elements of <paramref name="other"/> with this set
        /// </summary>
        /// <param name="other"></param>
        /// <exception cref="ArgumentNullException">Throws if other
        /// is null</exception>
        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            if (ReferenceEquals(other, this))
                return;

            foreach (T el in other)
            {
                Add(el);
            }
        }

        /// <summary>
        /// Remain elements, that both in this set and in <paramref name="other"/>
        /// </summary>
        /// <param name="other"></param>
        /// <exception cref="ArgumentNullException">Throws if other 
        /// is null</exception>
        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            if (ReferenceEquals(other, this)) 
                return;
            if (Count == 0)
                return;
            int count = 0;
            T[] temp = new T[array.Length];
            foreach (T el in other)
            {
                if (Remove(el))
                {
                    temp[count++] = el;
                }
            }
            array = temp;
            Count = count;
        }

        /// <summary>
        /// Remains elements of this set, that are not in <paramref name="other"/>
        /// </summary>
        /// <param name="other"></param>
        /// <exception cref="ArgumentNullException">Throws if
        /// <paramref name="other"/> is null</exception>
        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            if (Count == 0)
                return;
            foreach (T el in other)
            {
                Remove(el);
            }
        }

        /// <summary>
        /// Remains elements that are in this set, or in <paramref name="other"/>,
        /// but not in both
        /// </summary>
        /// <param name="other"></param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="other"/> 
        /// is null</exception>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            if (Count == 0)
                UnionWith(other);
            if (ReferenceEquals(other, this))
            {
                Clear();
            }
            Set<T> set = new Set<T>(other, equalityComparer);
            SymmetricExceptWithSet(set);
        }

        /// <summary>
        /// Checks if this set is subset of <paramref name="other"/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="other"/> 
        /// is null</exception>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            int count = 0;
            Set<T> set = new Set<T>(other, equalityComparer);
            foreach(T el in set)
                if (Contains(el))
                    count++;
            if (count == Count)
                return true;
            return false;
        }

        /// <summary>
        /// Checks if this set is superset of <paramref name="other"/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="other"/> 
        /// is null</exception>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            foreach (T el in other)
                if (!Contains(el))
                    return false;
            return true;
        }

        /// <summary>
        /// Checks if this set is proper superset of <paramref name="other"/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="other"/> 
        /// is null</exception>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            int count = 0;
            Set<T> set = new Set<T>(other, equalityComparer);
            foreach (T el in set)
                if (!Contains(el))
                    return false;
                else
                {
                    count++;
                }
            if (count == Count)
                return false;
            return true;
        }

        /// <summary>
        /// Checks if this set is proper subset of <paramref name="other"/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="other"/> 
        /// is null</exception>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            int count = 0;
            bool canBeSubset = false;
            Set<T> set = new Set<T>(other, equalityComparer);
            foreach (T el in set)
                if (Contains(el))
                    count++;
                else
                {
                    canBeSubset = true;
                }
            if (count == Count && canBeSubset)
                return true;
            return false;
        }

        /// <summary>
        /// Checks if this set and <paramref name="other"/> overlaps each other
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="other"/> 
        /// is null</exception>
        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            foreach (T el in other)
                if (Contains(el))
                    return true;
            return false;
        }

        /// <summary>
        /// Cheks if this set and <paramref name="other"/> contains the same 
        /// set of elements. Duplications ignored.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="other"/> 
        /// is null</exception>
        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            int count = 0;
            Set<T> set = new Set<T>(other, equalityComparer);
            foreach (T el in set)
                if (Contains(el))
                    count++;
                else
                    return false;
            if (count == Count)
                return true;
            return false;
        }

        /// <summary>
        /// Clears set
        /// </summary>
        public void Clear()
        {
            Count = 0;
            array = new T[array.Length];
        }

        /// <summary>
        /// Checks if this set contains an <paramref name="item"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            if (!FindItemPos(item).HasValue)
                return false;
            return true;
        }

        /// <summary>
        /// Copies items of this set to <paramref name="ar"/>
        /// </summary>
        /// <param name="ar">Destination array</param>
        /// <param name="arrayIndex">Destination start position</param>
        /// <exception cref="ArgumentNullException">Throws if
        /// <paramref name="ar"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throws if
        /// <paramref name="arrayIndex"/> is out of <paramref name="ar"/>
        /// boundaries</exception>
        /// <exception cref="ArgumentException">Throws if <paramref name="ar"/>
        /// has not enought space for items of this set</exception>
        public void CopyTo(T[] ar, int arrayIndex)
        {
            if (ar == null)
                throw new ArgumentNullException($"{nameof(ar)} is null");
            if (arrayIndex < 0 || arrayIndex >= ar.Length)
                throw new ArgumentOutOfRangeException
                    ($"{nameof(arrayIndex)} is out of range");
            if (ar.Length - arrayIndex < Count)
                throw new ArgumentException
                    ($"{nameof(ar)} has not enought length");
            Array.Copy(array, 0, ar, arrayIndex, Count);
        }

        /// <summary>
        /// Removes <paramref name="item"/> from this set. Takes O(n) operations
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            int? pos = FindItemPos(item);
            if (!pos.HasValue)
                return false;
            Count--;
            for (int i = pos.Value; i < Count; i++)
                array[i] = array[i + 1];
            return true;
        }

        /// <summary>
        /// Adds item to set
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return array[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Resizes inner array. <paramref name="capacity"/> should be
        /// greateer or equal to <see cref="Count"/>.
        /// </summary>
        /// <param name="capacity"></param>
        private void ResizeArray(int capacity)
        {
            T[] temp = new T[capacity];
            Array.Copy(array, temp, Count);
            array = temp;
        }

        /// <summary>
        /// finds position of <paramref name="item"/> in the array
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int? FindItemPos(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                bool compareResult;
                if (equalityComparer != null)
                    compareResult = equalityComparer.Equals(item, array[i]);
                else
                    compareResult = array[i].Equals(item);
                if (compareResult)
                    return i;
            }
            return null;
        }

        /// <summary>
        /// Makes symmetricExcept with <paramref name="other"/> set
        /// </summary>
        /// <param name="other"></param>
        private void SymmetricExceptWithSet(Set<T> other)
        {
            foreach (T el in other)
            {
                if (!Remove(el))
                    Add(el);
            }
        }
    }
}

