#region license
// Copyright © 2017 Maxim O. Trushin (trushin@gmail.com)
//
// This file is part of Syntactik.
// Syntactik is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Syntactik is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Syntactik.  If not, see <http://www.gnu.org/licenses/>.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;

namespace Syntactik.DOM
{
    /// <summary>
    /// Represents collection of <see cref="Pair"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PairCollection<T> : ICollection<T> where T : Pair
    {
        private readonly List<T> _list;
        private Pair _parent;

        /// <summary>
        /// Creates an instance of <see cref="PairCollection{T}"/>.
        /// </summary>
        public PairCollection()
        {
            _list = new List<T>();
        }

        /// <summary>
        /// Creates an instance of <see cref="PairCollection{T}"/>.
        /// </summary>
        /// <param name="parent">Parent assigned to each <see cref="Pair"/> in the collection.</param>
        public PairCollection(Pair parent)
        {
            _parent = parent;
            _list = new List<T>();
        }

        /// <summary>
        /// Adds <see cref="Pair"/> to collection.
        /// </summary>
        /// <param name="pair">Pair to be added.</param>
        public virtual void Add(T pair)
        {
            Initialize(pair);
            _list.Add(pair);
        }

        /// <summary>
        /// Adds range of pairs to collection.
        /// </summary>
        /// <param name="pairs">Range of pairs.</param>
        /// <returns>Resulting instance of collection.</returns>
        public PairCollection<T> AddRange(IEnumerable<T> pairs)
        {
            if (pairs == null) return this;
            foreach (T local in pairs)
            {
                Add(local);
            }
            return this;
        }

        internal PairCollection<T> AddRangeOverrideParent(IEnumerable<T> pairs)
        {
            if (pairs == null) return this;
            foreach (T local in pairs)
            {
                local.InitializeOverrideParent(_parent);
                _list.Add(local);
            }
            return this;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public virtual void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Returns true if collection contains the pair.
        /// </summary>
        /// <param name="pair">Target pair.</param>
        /// <returns>True if collection contains the pair.</returns>
        public virtual bool Contains(T pair)
        {
            return _list.LastIndexOf(pair) > -1;
        }

        /// <summary>
        /// Copies the collection to array.
        /// </summary>
        /// <param name="array">Target array.</param>
        /// <param name="index">Start index.</param>
        public void CopyTo(Array array, int index)
        {
            _list.CopyTo((T[])array, index);
        }

        /// <summary>
        /// Copies the collection to generic array.
        /// </summary>
        /// <param name="array">Target array.</param>
        /// <param name="index">Start index.</param>
        public virtual void CopyTo(T[] array, int index)
        {
            _list.CopyTo(array, index);
        }

        /// <inheritdoc />
        public virtual IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        private void Initialize(T pair)
        {
            if (_parent != null)
                pair.InitializeParent(_parent);
        }

        internal void InitializeParent(Pair parent)
        {
            _parent = parent;
            foreach (var local in _list)
            {
                local.InitializeParent(parent);
            }
        }

        /// <summary>
        /// Inserts <see cref="Pair"/> in collection.
        /// </summary>
        /// <param name="index">Index in the collection.</param>
        /// <param name="pair">Pair to be added.</param>
        public void Insert(int index, T pair)
        {
            Initialize(pair);
            _list.Insert(index, pair);
        }


        /// <summary>
        /// Removes pair from collection.
        /// </summary>
        /// <param name="pair">Pair to be removed.</param>
        /// <returns>True if pair were removed.</returns>
        public virtual bool Remove(T pair)
        {
            return _list.Remove(pair);
        }

        /// <summary>
        /// Replaces existing pair with the new one.
        /// </summary>
        /// <param name="existing">Existing pair.</param>
        /// <param name="newItem">New pair. Can be null.</param>
        /// <returns>True of pair has been replaced.</returns>
        public bool Replace(T existing, T newItem)
        {
            int index = _list.IndexOf(existing);
            if (newItem == null)
            {
                _list.RemoveAt(index);
            }
            else
            {
                Initialize(newItem);
                _list[index] = newItem;
            }
            return (index != -1);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Creates array from the collection.
        /// </summary>
        /// <returns>Generic array of <see cref="Pair"/></returns>
        public T[] ToArray()
        {
            return _list.ToArray();
        }


        /// <inheritdoc />
        public virtual int Count => _list.Count;

        /// <inheritdoc />
        public virtual bool IsReadOnly => false;

        /// <summary>
        /// Gets pair by index.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <returns>Resulting pair.</returns>
        public T this[int index]
        {
            get => _list[index];
            set
            {
                if (_list != null && !ReferenceEquals(_list[index], value))
                {
                    _list[index] = value;
                }
            }
        }

        /// <summary>
        /// Parent of the collection.
        /// </summary>
        public Pair Parent => _parent;

    }


}
