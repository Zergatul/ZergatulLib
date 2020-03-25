﻿using System.Collections.Generic;

namespace Zergatul.FileFormat.Pdf
{
    public class XRefTable
    {
        public int Count => _data.Count;
        public IReadOnlyCollection<long> Keys => _data.Keys;

        private Dictionary<long, XRefEntry> _data = new Dictionary<long, XRefEntry>();

        public XRefEntry this[long id] => _data[id];

        public void Add(long id, XRefEntry entry)
        {
            if (_data.ContainsKey(id))
                _data[id] = entry;
            else
                _data.Add(id, entry);
        }

        public void Add(long id, long offset, int generation, bool free)
        {
            Add(id, new XRefEntry(id, offset, generation, free));
        }

        public void MergeWith(XRefTable other)
        {
            foreach (var kv in other._data)
                Add(kv.Key, kv.Value);  
        }

        public void Remove(long id)
        {
            _data.Remove(id);
        }
    }
}