using System;
using System.Collections;
using System.Collections.Generic;

namespace Task3.Logic
{
    public class Set<T> : ISet<T> where T : class, IEquatable<T>
    {
        private T[] array;
        private readonly IEqualityComparer<T> equalityComparer;

        public Set(int capacity = 50, IEqualityComparer<T> equalityComparer = null)
        {
            if (capacity <= 0)
                throw new ArgumentException(
                    $"{nameof(capacity)} is less or equal to zero");
            array = new T[capacity];
            this.equalityComparer = equalityComparer;
        }

        public Set(IEnumerable<T> elements, IEqualityComparer<T> equalityComparer = null,
            int capacity = 50)
        {
            this.equalityComparer = equalityComparer;
            if (elements == null)
                throw new ArgumentNullException
                    ($"{nameof(elements)} parameter is null");
            if (capacity <= 0)
                throw new ArgumentException(
                    $"{nameof(capacity)} is less or equal to zero");
            UnionWith(elements);
        }

        public int Count { get; private set; }
        public bool IsReadOnly => false;

        public bool Add(T item)
        {
            if (Contains(item))
                return false;
            if (Count == array.Length)
                ResizeArray(array.Length*2);
            array[Count++] = item;
            return true;
        }

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

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            foreach (T el in other)
                if (!Contains(el))
                    return false;
            return true;
        }

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

        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException($"{nameof(other)} is null");
            foreach (T el in other)
                if (Contains(el))
                    return true;
            return false;
        }

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

        public void Clear()
        {
            Count = 0;
            array = new T[array.Length];
        }

        public bool Contains(T item)
        {
            if (!FindItemPos(item).HasValue)
                return false;
            return true;
        }

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

        private void ResizeArray(int capacity)
        {
            T[] temp = new T[capacity];
            Array.Copy(array, temp, Count);
            array = temp;
        }

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

