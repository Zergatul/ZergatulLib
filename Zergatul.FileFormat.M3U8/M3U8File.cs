using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Zergatul.FileFormat.M3U8
{
    public class M3U8File
    {
        public List<M3U8Item> Items;

        public static async Task<M3U8File> Parse(Stream stream)
        {
            ParseState state = ParseState.Start;
            List<M3U8Item> items = new List<M3U8Item>();

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream && state != ParseState.End)
                {
                    var line = await reader.ReadLineAsync();

                    switch (state)
                    {
                        case ParseState.Start:
                            if (line != "#EXTM3U")
                            {
                                throw new InvalidDataException("Invalid header.");
                            }
                            state = ParseState.Headers;
                            break;

                        case ParseState.Headers:
                            if (line.StartsWith("#EXTINF"))
                            {
                                state = ParseState.StartItem;
                                goto case ParseState.StartItem;
                            }
                            break;

                        case ParseState.StartItem:
                            if (!line.StartsWith("#EXTINF"))
                            {
                                throw new InvalidDataException("Invalid header.");
                            }
                            if (line == "#EXT-X-ENDLIST")
                            {
                                state = ParseState.End;
                                break;
                            }
                            state = ParseState.Item;
                            break;

                        case ParseState.Item:
                            if (!line.StartsWith('#'))
                            {
                                items.Add(new M3U8Item { Url = line });
                            }
                            break;
                    }
                }
            }

            return new M3U8File { Items = items };
        }

        private enum ParseState
        {
            Start,
            Headers,
            StartItem,
            Item,
            End
        }
    }
}