using System;
using System.Collections.Generic;

namespace Zergatul.FileFormat.Pdf
{
    public class XRefTable
    {
        public int Count => _data.Count;

        private Dictionary<long, XRefEntry> _data = new Dictionary<long, XRefEntry>();

        public void Add(long id, long offset, int generation, bool free)
        {
            var entry = new XRefEntry(offset, generation, free);
            if (_data.ContainsKey(id))
                _data[id] = entry;
            else
                _data.Add(id, entry);
        }
    }
}