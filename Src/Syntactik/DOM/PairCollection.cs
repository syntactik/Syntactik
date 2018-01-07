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
    public class PairCollection<T> : ICollection<T> where T : Pair
    {
        private readonly List<T> _list;
        private Pair _parent;

        public PairCollection()
        {
            _list = new List<T>();
        }

        public PairCollection(Pair parent)
        {
            _parent = parent;
            _list = new List<T>();
        }

        public virtual void Add(T item)
        {
            Initialize(item);
            _list.Add(item);
        }

        public PairCollection<T> AddRange(IEnumerable<T> items)
        {
            if (items == null) return this;
            foreach (T local in items)
            {
                Add(local);
            }
            return this;
        }

        internal PairCollection<T> AddRangeOverrideParent(IEnumerable<T> items)
        {
            if (items == null) return this;
            foreach (T local in items)
            {
                local.InitializeOverrideParent(_parent);
                _list.Add(local);
            }
            return this;
        }

        public virtual void Clear()
        {
            _list.Clear();
        }

        public virtual bool Contains(T node)
        {
            return (_list.LastIndexOf(node) > -1);
        }

        public void CopyTo(Array array, int index)
        {
            _list.CopyTo((T[])array, index);
        }

        public virtual void CopyTo(T[] array, int index)
        {
            _list.CopyTo(array, index);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void Initialize(T item)
        {
            item.InitializeParent(_parent);
        }

        internal void InitializeParent(Pair parent)
        {
            _parent = parent;
            foreach (var local in _list)
            {
                local.InitializeParent(parent);
            }
        }

        public void Insert(int index, T item)
        {
            Initialize(item);
            _list.Insert(index, item);
        }


        public virtual bool Remove(T item)
        {
            return _list.Remove(item);
        }

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

        public T[] ToArray()
        {
            return _list.ToArray();
        }

        // Properties
        public virtual int Count => _list.Count;

        public virtual bool IsReadOnly => false;

        public bool IsSynchronized => false;

        public T this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                if (_list != null && !ReferenceEquals(_list[index], value))
                {
                    _list[index] = value;
                }
            }
        }

        public Pair Parent => _parent;

    }


}
