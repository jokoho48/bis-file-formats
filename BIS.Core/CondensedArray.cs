#region

using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace BIS.Core
{
    public class CondensedArray<T> : IEnumerable<T>
    {
        private readonly T[] array;

        public CondensedArray(int nElements, T value)
        {
            ElementCount = nElements;
            IsCondensed = true;
            DefaultValue = value;
            array = null;
        }

        public CondensedArray(T[] array)
        {
            ElementCount = array.Length;
            IsCondensed = false;
            DefaultValue = default(T);
            this.array = array;
        }

        public bool IsCondensed { get; }

        public T DefaultValue { get; }

        public int ElementCount { get; }

        public T this[int i]
        {
            get
            {
                if (IsCondensed) return DefaultValue;
                return array[i];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (IsCondensed)
            {
                for (int i = 0; i < ElementCount; i++)
                    yield return DefaultValue;
            }
            else
            {
                for (int i = 0; i < ElementCount; i++)
                    yield return array[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T[] AsArray()
        {
            return IsCondensed ? Enumerable.Range(0, ElementCount).Select(_ => DefaultValue).ToArray() : array;
        }
    }
}