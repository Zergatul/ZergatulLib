using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf
{
    public class TrailerDictionary
    {
        public long Size { get; }
        public long? Prev { get; }
        public IndirectObject Root { get; }
        public object Encrypt { get; }
        public IndirectObject Info { get; }
        public byte[] Id { get; }

        internal TrailerDictionary(DictionaryToken dictionary)
        {
            if (!dictionary.ContainsKey(nameof(Size)))
                throw new InvalidDataException("Trailer dictionary should contain [Size] key.");
            if (!dictionary.Is<IntegerToken>(nameof(Size)))
                throw new InvalidDataException("Trailer dictionary error. [Size] key should be integer.");

            if (dictionary.ContainsKey(nameof(Prev)))
            {
                if (!dictionary.Is<IntegerToken>(nameof(Prev)))
                    throw new InvalidDataException("Trailer dictionary error. [Prev] key should be integer.");
            }

            if (!dictionary.ContainsKey(nameof(Root)))
                throw new InvalidDataException("Trailer dictionary should contain [Root] key.");
            if (!dictionary.Is<IndirectReferenceToken>(nameof(Root)))
                throw new InvalidDataException("Trailer dictionary error. [Root] key should be indirect reference.");

            if (dictionary.ContainsKey(nameof(Info)))
            {
                if (!dictionary.Is<IndirectReferenceToken>(nameof(Info)))
                    throw new InvalidDataException("Trailer dictionary error. [Info] key should be indirect reference.");
            }

            Size = dictionary.GetInteger(nameof(Size));
            Prev = dictionary.GetIntegerNullable(nameof(Prev));
            Root = dictionary.GetIndirectObject(nameof(Root));
            // Encrypt // TODO
            Info = dictionary.GetIndirectObjectNullable(nameof(Info));
            // ID // TODO
        }
    }
}