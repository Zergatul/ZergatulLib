namespace Zergatul
{
    public class ErrorCode
    {
        public int Code { get; }
        public string Message { get; }

        public ErrorCode(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}