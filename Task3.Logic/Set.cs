﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Task3.Logic
{
    /// <summary>
    /// Provides functionality of set of unique elements
    /// </summary>
    public class Set<T> : ISet<T>, ICloneable where T : class, IEquatable<T>
    {
        /// <summary>
        /// inner storage for elements of set
        /// </summary>
        private T[] array;
        private readonly IEqualityComparer<T> equalityComparer;

        /// <summary>
        /// version of set
        /// </summary>
        private int version;

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
            array = new T[capacity];
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
            version++;
            array[Count++] = item;
            return true;
        }
        
        /// <summary>
        /// Static method to create new <see cref="Set{T}"/> which is
        /// the union of <paramref name="first"/> and <paramref name="second"/>
        /// </summary>
        /// <param name="equalityComparer">Equality comparer for result set.
        /// If not specified, comparer of <paramref name="first"/> will be used</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="first"/>
        /// or <paramref name="second"/> is null</exception>
        public static Set<T> Union
            (Set<T> first, Set<T> second, IEqualityComparer<T> equalityComparer = null)
        {
            return MakeStaticOperation
                (first, second, 
                    (set, enumerable) => set.UnionWith(enumerable), equalityComparer);
        }

        /// <summary>
        /// Static method to create new <see cref="Set{T}"/> which is
        /// the intersection of <paramref name="first"/> and <paramref name="second"/>
        /// </summary>
        /// <param name="equalityComparer">Equality comparer for result set.
        /// If not specified, comparer of <paramref name="first"/> will be used</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="first"/>
        /// or <paramref name="second"/> is null</exception>
        public static Set<T> Intersect
            (Set<T> first, Set<T> second, IEqualityComparer<T> equalityComparer = null)
        {
            return MakeStaticOperation
                (first, second, 
                (set, enumerable) => set.IntersectWith(enumerable), equalityComparer);
        }

        /// <summary>
        /// Static method to create new <see cref="Set{T}"/> which is
        /// the exception of <paramref name="second"/> from <paramref name="first"/>
        /// </summary>
        /// /// <param name="equalityComparer">Equality comparer for result set.
        /// If not specified, comparer of <paramref name="first"/> will be used</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="first"/>
        /// or <paramref name="second"/> is null</exception>
        public static Set<T> Except
            (Set<T> first, Set<T> second, IEqualityComparer<T> equalityComparer = null)
        {
            return MakeStaticOperation
                (first, second, 
                    (set, enumerable) => set.ExceptWith(enumerable), equalityComparer);
        }

        /// <summary>
        /// Static method to create new <see cref="Set{T}"/> which is
        /// the symmetric exception of <paramref name="first"/> and 
        /// <paramref name="second"/>
        /// </summary>
        /// <param name="equalityComparer">Equality comparer for result set.
        /// If not specified, comparer of <paramref name="first"/> will be used</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="first"/>
        /// or <paramref name="second"/> is null</exception>
        public static Set<T> SymmetricExcept
            (Set<T> first, Set<T> second, IEqualityComparer<T> equalityComparer = null)
        {
            return MakeStaticOperation
                (first, second, 
                    (set, enumerable) => set.SymmetricExceptWith(enumerable), equalityComparer);
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
            Set<T> set = other as Set<T>;
            set = set ?? new Set<T>(other, equalityComparer);
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
            version++;
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

        /// <summary>
        /// Public method to clone <see cref="Set{T}"/>. Hides 
        /// implementation of <see cref="ICloneable"/>, which returns <see cref="object"/>
        /// </summary>
        /// <returns></returns>
        public Set<T> Clone()
        {
            return new Set<T>(this, equalityComparer, Count);
        }

        /// <summary>
        /// Returns enumerator for <see cref="Set{T}"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws if
        /// <see cref="Set{T}"/> has been changed while enumerating</exception>
        public IEnumerator<T> GetEnumerator()
        {
            int startVersion = version;
            for (int i = 0; i < Count; i++)
            {
                if (startVersion != version)
                    throw new InvalidOperationException("Set has been changed");
                yield return array[i];
            }
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
                if (ReferenceEquals(item, array[i]))
                    return i;
                if (ReferenceEquals(array[i], null))
                    continue;
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

        /// <summary>
        /// Private static method to make static operation with stack
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="equalityComparer"></param>
        /// <param name="operation">must be not null</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="first"/>
        /// or <paramref name="second"/> is null</exception>
        private static Set<T> MakeStaticOperation
            (Set<T> first, IEnumerable<T> second, Action<ISet<T>, IEnumerable<T>> operation,
            IEqualityComparer<T> equalityComparer)
        {
            if (first == null)
                throw new ArgumentNullException($"{nameof(first)} is null");
            if (second == null)
                throw new ArgumentNullException($"{nameof(second)} is null");
            var ret = equalityComparer == null 
                        ? first.Clone() 
                        : new Set<T>(first, equalityComparer);
            operation(ret, second);
            return ret;
        }
        /// <summary>
        /// Implementation of <see cref="ICloneable"/>. Returns new <see cref="Set{T}"/>
        /// based on current;
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}

