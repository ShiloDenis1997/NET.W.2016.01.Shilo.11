using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2.Logic
{
    /// <summary>
    /// Provides queue functionality to work with generic elements
    /// </summary>
    public class Queue <T>: IEnumerable<T>, ICollection
    {
        /// <summary>
        /// inner storage for elements
        /// </summary>
        private T[] array;
        /// <summary>
        /// position of the first element
        /// </summary>
        private int front;
        /// <summary>
        /// position for next element
        /// </summary>
        private int back;
        /// <summary>
        /// Current version of this instance of <see cref="Queue{T}"/>
        /// </summary>
        private int versionOfQueue = 0;

        /// <summary>
        /// Initializes new instance of <see cref="Queue{T}"/> with specified
        ///  <paramref name="capacity" />.
        /// </summary>
        /// <exception cref="ArgumentException">Throws if 
        /// <paramref name="capacity"/> is less or equal to zero</exception>
        public Queue(int capacity = 50)
        {
            if (capacity <= 0)
                throw new ArgumentException
                    ($"{nameof(capacity)} is less or equal to zero");
            array = new T[capacity];
        }

        /// <summary>
        /// Initializes new instance of <see cref="Queue{T}"/> based on
        /// <paramref name="elements"/> with specified <paramref name="capacity"/>
        /// </summary>
        /// <exception cref="ArgumentException">Throws if 
        /// <paramref name="capacity"/> is less or equal to zero</exception>
        /// <exception cref="ArgumentNullException">Throws if 
        /// <paramref name="elements"/> is null</exception>
        public Queue(IEnumerable<T> elements, int capacity = 50)
        {
            if (capacity <= 0)
                throw new ArgumentException
                    ($"{nameof(capacity)} is less or equal to zero");
            if (elements == null)
                throw new ArgumentNullException
                    ($"{nameof(elements)} parameter is null");
            array = new T[capacity];
            foreach (T el in elements)
            {
                Push(el);
            }
        }

        /// <summary>
        /// Count of elements in the queue
        /// </summary>
        public int Count { get; private set; }
        public object SyncRoot { get; } = new object();
        /// <summary>
        /// Collection is not synchronized
        /// </summary>
        public bool IsSynchronized => false;
        /// <summary>
        /// Indicates if queue is empty
        /// </summary>
        public bool Empty => Count == 0;
        
        

        /// <summary>
        /// Adds new element in the collection. It's usually takes O(1) operations,
        /// but it takes O(n) when it's need to resize queue
        /// </summary>
        /// <param name="element">Element to add into the queue</param>
        public void Push(T element)
        {
            versionOfQueue++;
            if (Count == array.Length)
                ResizeArray(array.Length * 2);
            array[back++] = element;
            Count++;
            if (back == array.Length)
                back = 0;
        }

        /// <summary>
        /// Returns first element in the queue
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws if queue 
        /// is empty</exception>
        public T Front()
        {
            if (Empty)
                throw new InvalidOperationException("Queue is empty");
            return array[front];
        }

        /// <summary>
        /// Removes first element from the queue
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws if queue 
        /// is empty</exception>
        public void Pop()
        {
            if (Empty)
                throw new InvalidOperationException("Queue is empty");
            versionOfQueue++;
            array[front++] = default(T);
            if (front == array.Length)
                front = 0;
            Count--;
        }

        /// <summary>
        /// Copies elements of the queue to array, beginning from 
        /// <paramref name="index"/> position
        /// </summary>
        /// <param name="ar">Array to copy in</param>
        /// <param name="index">Destination start index</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="ar"/> 
        /// is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throws if 
        /// <paramref name="index"/> is out of <paramref name="ar"/> boundaries
        /// </exception>
        /// <exception cref="ArgumentException">Throws if <paramref name="ar"/>
        /// has not enought space to store elements of queue</exception>
        public void CopyTo(Array ar, int index)
        {
            if (ar == null)
                throw new ArgumentNullException($"{nameof(ar)} is null");
            if (index < 0 || index >= ar.Length)
                throw new ArgumentOutOfRangeException($"{nameof(index)} is out of range");
            if (ar.Length - index < Count)
                throw new ArgumentException($"{nameof(ar)} hasn't enought length");
            if (front < back)
            {
                Array.Copy(array, front, ar, index, Count);
            }
            else
            {
                Array.Copy(array, front, ar, index, array.Length - front);
                Array.Copy(array, 0, ar, index + array.Length - front, back);
            }
        }

        /// <summary>
        /// Returns enumerator to enumerate all elements in the queue
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return new QueueEnumerator(this);
        }

        /// <summary>
        /// Returns enumerator to enumerate all elements in the queue
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Private method to resize queue. <paramref name="capacity"/> must
        /// be greater or equal to <see cref="Count"/>
        /// </summary>
        /// <param name="capacity"></param>
        private void ResizeArray(int capacity)
        {
            T[] temp = new T[capacity];
            if (front < back)
            {
                Array.Copy(array, front, temp, 0, Count);
            }
            else
            {
                Array.Copy(array, front, temp, 0, array.Length - front);
                Array.Copy(array, 0, temp, array.Length - front, back);
            }
            front = 0;
            back = Count == capacity ? 0 : Count;
            array = temp;
        }

        /// <summary>
        /// Inner class, which enumerates elements of the queue
        /// </summary>
        private class QueueEnumerator : IEnumerator<T>
        {
            private int pos;
            /// <summary>
            /// Number of already enumerated elements
            /// </summary>
            private int number;
            private Queue<T> queue;
            private int initVersionOfQueue;
            private T currentElement;

            public QueueEnumerator(Queue<T> queue)
            {
                initVersionOfQueue = queue.versionOfQueue;
                this.queue = queue;
                pos = queue.front - 1;
                number = -1;
                currentElement = default(T);
            }

            public void Dispose() {}

            /// <summary>
            /// Moves to the next element of queue
            /// </summary>
            /// <exception cref="InvalidOperationException">Throws if
            /// queue has been changed while enumerating</exception>
            public bool MoveNext()
            {
                if (queue.versionOfQueue != initVersionOfQueue)
                    throw new InvalidOperationException
                        ("Collection has been changed");
                pos++;
                number++;
                if (pos == queue.array.Length)
                    pos = 0;
                if (pos == queue.back && number == queue.Count)
                {
                    currentElement = default(T);
                    return false;
                }
                currentElement = queue.array[pos];
                return true;
            }

            /// <summary>
            /// Begins enumeration from start
            /// </summary>
            /// <exception cref="InvalidOperationException">Throws if 
            /// queue has been changed while enumerating</exception>
            public void Reset()
            {
                if (queue.versionOfQueue != initVersionOfQueue)
                    throw new InvalidOperationException
                        ("Collection has been changed");
                pos = queue.front - 1;
                number = -1;
            }

            public T Current => currentElement;

            object IEnumerator.Current => Current;
        }
    }
}



