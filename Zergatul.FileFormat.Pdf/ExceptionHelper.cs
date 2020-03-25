using System;
using System.IO;

namespace Zergatul.FileFormat.Pdf
{
    internal static class ExceptionHelper
    {
        public static Exception ArgumentOutOfRangeExceptionByCode(string paramName, ErrorCode code)
        {
            var exception = new ArgumentOutOfRangeException(paramName, code.Message);
            exception.Data.Add(nameof(code.Code), code.Code);
            return exception;
        }

        public static Exception InvalidDataExceptionByCode(ErrorCode code)
        {
            var exception = new InvalidDataException(code.Message);
            exception.Data.Add(nameof(code.Code), code.Code);
            return exception;
        }
    }
}