using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zergatul.Network;

namespace Zergatul.Ftp
{
    public class DefaultFtpListParser : IFtpListParser
    {
        static readonly Regex _skipTotalInfo = new Regex(@"^total\s+\d+$");
        static readonly Regex _iisFileLine = new Regex(@"^(?<month>\d{1,2})-(?<day>\d{1,2})-(?<year>\d{2,4})\s+(?<hour>\d{1,2}):(?<minute>\d{1,2})(?<ampm>\wM)\s+(?<length>(\d+|\<DIR\>))\s+(?<name>.+)$");
        static readonly Regex _unixFileLine = new Regex(@"^(?<dir>(d|-))[rwxst\-]{9}\s+\d+\s+\w+\s+\w+\s+(?<length>\d+)\s+(?<month>\w{3})\s+(?<day>\d{1,2})\s+((?<year>\d{4})\s+)?((?<hour>\d{1,2}):(?<minute>\d{1,2})\s+)?(?<name>.+)$");
        static readonly Regex[] _fileLines = new[] { _iisFileLine, _unixFileLine };

        public FtpFileInfo[] Parse(string response)
        {
            var lines = response.Split(new[] { Constants.TelnetEndOfLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
                return new FtpFileInfo[0];

            bool skipFirstLine = _skipTotalInfo.IsMatch(lines[0]);
            if (skipFirstLine && lines.Length == 1)
                return new FtpFileInfo[0];

            var firstLine = lines[skipFirstLine ? 1 : 0];
            var regexpes = _fileLines.Where(r => r.IsMatch(firstLine)).ToArray();
            if (regexpes.Length > 1)
                throw new Exception("unexpected");
            if (regexpes.Length == 0)
                throw new Exception("Cannot parse file info.");

            var regex = regexpes[0];
            if (regex == _iisFileLine)
                return ParseIIS(lines, skipFirstLine ? 1 : 0);
            if (regex == _unixFileLine)
                return ParseUnix(lines, skipFirstLine ? 1 : 0);

            throw new NotImplementedException();
        }

        private FtpFileInfo[] ParseIIS(string[] lines, int startFrom)
        {
            FtpFileInfo[] result = new FtpFileInfo[lines.Length - startFrom];
            for (int i = startFrom; i < lines.Length; i++)
            {
                var match = _iisFileLine.Match(lines[i]);
                if (!match.Success)
                    throw new Exception("Cannot parse file info. First line was ok.");

                int year = int.Parse(match.Groups["year"].Value);
                if (year < 100)
                    if (year > 50)
                        year = 1900 + year;
                    else
                        year = 2000 + year;
                int month = int.Parse(match.Groups["month"].Value);
                int day = int.Parse(match.Groups["day"].Value);
                int hour = int.Parse(match.Groups["hour"].Value);
                int minute = int.Parse(match.Groups["minute"].Value);
                bool am = match.Groups["ampm"].Value == "AM";
                string lengthStr = match.Groups["length"].Value;
                bool directory = lengthStr == "<DIR>";
                long length = 0;
                if (!directory)
                    length = long.Parse(lengthStr);
                string name = match.Groups["name"].Value;
                result[i - startFrom] = new FtpFileInfo
                {
                    ModifiedDate = new DateTime(year, month, day, hour, minute, 0),
                    Length = length,
                    Name = name,
                    IsDirectory = directory
                };
            }
            return result;
        }

        private FtpFileInfo[] ParseUnix(string[] lines, int startFrom)
        {
            FtpFileInfo[] result = new FtpFileInfo[lines.Length - startFrom];
            for (int i = startFrom; i < lines.Length; i++)
            {
                var match = _unixFileLine.Match(lines[i]);
                // skip links for now
                if (lines[i].Length > 0 && lines[i][0] == 'l')
                    continue;
                if (!match.Success)
                    throw new Exception("Cannot parse file info. First line was ok.");

                string yearStr = match.Groups["year"].Value;
                int year = yearStr == "" ? DateTime.Now.Year : int.Parse(yearStr);
                int month = Parse3CharMonth(match.Groups["month"].Value);
                int day = int.Parse(match.Groups["day"].Value);
                string hourStr = match.Groups["hour"].Value;
                int hour = hourStr == "" ? 0 : int.Parse(hourStr);
                string minuteStr = match.Groups["minute"].Value;
                int minute = minuteStr == "" ? 0 : int.Parse(minuteStr);
                long length = long.Parse(match.Groups["length"].Value);
                string name = match.Groups["name"].Value;
                bool directory = match.Groups["dir"].Value == "d";
                if (directory)
                    length = 0;
                if (directory && (name == "." || name == ".."))
                    continue;
                result[i - startFrom] = new FtpFileInfo
                {
                    ModifiedDate = new DateTime(year, month, day, hour, minute, 0),
                    Length = length,
                    Name = name,
                    IsDirectory = directory
                };
            }
            return result.Where(r => r != null).ToArray();
        }

        private int Parse3CharMonth(string month)
        {
            switch (month.ToLower())
            {
                case "jan": return 1;
                case "feb": return 2;
                case "mar": return 3;
                case "apr": return 4;
                case "may": return 5;
                case "jun": return 6;
                case "jul": return 7;
                case "aug": return 8;
                case "sep": return 9;
                case "oct": return 10;
                case "nov": return 11;
                case "dec": return 12;
                default:
                    throw new Exception("invalid month");
            }
        }
    }
}