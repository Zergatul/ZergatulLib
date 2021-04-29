using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zergatul.FileFormat.Csv
{
    public static class CsvSerializer
    {
        public static IEnumerable<T> ReadFile<T>(string filename)
        {
            if (Mapper<T>.Func == null)
                GenerateMapper<T>();

            using var fs = File.OpenRead(filename);
            using var enumerator = ParseRows(fs).GetEnumerator();

            if (!enumerator.MoveNext())
                yield break;

            string[] headers = enumerator.Current.Select(m => Encoding.ASCII.GetString(m.Span)).ToArray();

            while (enumerator.MoveNext())
                yield return Mapper<T>.Func(headers, enumerator.Current);
        }

        private static IEnumerable<IEnumerable<Memory<byte>>> ParseRows(Stream stream)
        {
            var reader = new Utf8CsvReader();
            byte[] buffer = new byte[64 * 1024];

            while (!reader.IsFinished)
                yield return ParseRow(stream, reader, buffer);
        }

        private static IEnumerable<Memory<byte>> ParseRow(Stream stream, Utf8CsvReader reader, byte[] buffer)
        {
            while (true)
            {
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case CsvTokenType.Data:
                            yield return reader.Value;
                            break;

                        case CsvTokenType.EndOfLine:
                            yield break;

                        default:
                            throw new InvalidOperationException("Unexpected CsvTokenType.");
                    }
                }

                int read = stream.Read(buffer, 0, buffer.Length);
                if (read == 0)
                {
                    if (reader.End())
                    {
                        if (reader.TokenType == CsvTokenType.Data)
                            yield return reader.Value;
                    }

                    yield break;
                }

                reader.AddData(buffer.AsMemory(0, read));
            }
        }

        private static void GenerateMapper<T>()
        {
            var ctor = typeof(T).GetConstructor(new Type[0]);
            if (ctor == null)
                throw new InvalidOperationException("Cannot find parameterless constructor.");

            var fields = typeof(T).GetFields();
            var properties = typeof(T).GetProperties();

            Mapper<T>.Func = (headers, cells) =>
            {
                using (var headersEnumerator = headers.GetEnumerator())
                using (var cellsEnumerator = cells.GetEnumerator())
                {
                    var item = ctor.Invoke(null);

                    while (true)
                    {
                        if (!headersEnumerator.MoveNext())
                            break;
                        if (!cellsEnumerator.MoveNext())
                            break;

                        var field = fields.SingleOrDefault(f => f.Name == headersEnumerator.Current);
                        if (field != null)
                        {
                            field.SetValue(item, GetValue(cellsEnumerator.Current, field.FieldType));
                            continue;
                        }

                        var property = properties.SingleOrDefault(f => f.Name == headersEnumerator.Current);
                        if (property != null)
                        {
                            property.SetValue(item, GetValue(cellsEnumerator.Current, field.FieldType));
                            continue;
                        }
                    }

                    return (T)item;
                }
            };
        }

        private static object GetValue(Memory<byte> raw, Type type)
        {
            if (type == typeof(int))
                return CellParser.GetInt(raw);
            if (type == typeof(double?))
                return CellParser.GetDoubleNullable(raw);

            throw new NotImplementedException();
        }

        #region Nested classes

        private static class Mapper<T>
        {
            public static Func<IReadOnlyList<string>, IEnumerable<Memory<byte>>, T> Func;
        }

        #endregion
    }
}