using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AVR_Simulator
{
	public class MappedArray<T> : IList<T>
		where T : struct
	{
		public MappedArray(IList<T> array, int start, int end)
		{
			this.array = array;
			this.Start = start;
			this.End = end;

			this.Count = (this.End - this.Start) + 1;
		}

		private IList<T> array;

		public int Start { get; private set; }
		public int End { get; private set; }

		public T this[int index]
		{
			get
			{
				if (index < 0 || (this.Start + index) > this.End)
					throw new IndexOutOfRangeException();

				return this.array[this.Start + index];
			}
			set
			{
				if (index < 0 || (this.Start + index) > this.End)
					throw new IndexOutOfRangeException();

				this.array[this.Start + index] = value;
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			this.array.CopyTo(array, arrayIndex);
		}

		public void Clear()
		{
			for (int i = this.Start; i <= this.End; i++)
				this.array[i] = default(T);
		}

		public int Count { get; private set; }

		public IEnumerator<T> GetEnumerator()
		{
			return new MappedArrayEnumerator<T>(this);
		}

		private class MappedArrayEnumerator<T> : IEnumerator<T>
			where T : struct
		{
			public MappedArrayEnumerator(MappedArray<T> array)
			{
				this.array = array;
				this.index = -1;
			}

			private MappedArray<T> array;
			private int index;

			public T Current
			{
				get
				{
					return this.array[this.index];
				}
			}

			public void Dispose()
			{

			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				return ++this.index < this.array.Count;
			}

			public void Reset()
			{
				this.index = -1;
			}
		}

		// IList<T>
		int IList<T>.IndexOf(T item)
		{
			throw new NotImplementedException();
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		void ICollection<T>.Add(T item)
		{
			throw new NotImplementedException();
		}

		bool ICollection<T>.Contains(T item)
		{
			throw new NotImplementedException();
		}

		int ICollection<T>.Count
		{
			get { throw new NotImplementedException(); }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}