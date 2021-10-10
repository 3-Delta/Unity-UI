using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace Unity.DebugWatch.Editor
{
    internal class WatchListAdapter : IList<TreeViewItem>
    {
        private readonly TreeViewItem currentItem = new TreeViewItem();
        WatchRegistry WatchRegistry;
        public int Count
        {
            get
            {
                return WatchRegistry.Watches.Count;
            }
        }

        private Enumerator indexIterator;


        class Enumerator : IEnumerator<TreeViewItem>
        {
            private int currentIndex;

            private readonly WatchListAdapter adapter;

            public Enumerator(WatchListAdapter adapter)
            {
                this.adapter = adapter;
                Reset();
            }
            public void Reset()
            {
                currentIndex = 0;
            }
            internal void MoveToIndex(int newLinearIndex)
            {
                currentIndex = newLinearIndex;
            }

            public bool MoveNext()
            {
                ++currentIndex;
                return currentIndex < adapter.WatchRegistry.Watches.Count;
            }
            public TreeViewItem Current
            {
                get
                {
                    adapter.currentItem.id = currentIndex;
                    adapter.currentItem.displayName = adapter.WatchRegistry.Watches[currentIndex].Watch.GetName();
                    return adapter.currentItem;
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose() { }
        }
        public WatchListAdapter(WatchRegistry watchRegistry)
        {
            WatchRegistry = watchRegistry;
            indexIterator = new Enumerator(this);
        }
        public TreeViewItem this[int linearIndex]
        {
            get
            {
                indexIterator.MoveToIndex(linearIndex);
                return indexIterator.Current;
            }
            set { throw new System.NotImplementedException(); }
        }

        public bool IsReadOnly => false;

        public bool Contains(TreeViewItem item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TreeViewItem> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TreeViewItem item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(TreeViewItem[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(TreeViewItem item)
        {
            throw new System.NotImplementedException();
        }

        public int IndexOf(TreeViewItem item)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(int index, TreeViewItem item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}