using System.Threading;

namespace Zergatul.Tests
{
    public static class TestHelper
    {
        public static bool WaitAll(params WaitHandle[] handles)
        {
            bool? result = null;
            var thread = new Thread(() =>
            {
                result = WaitHandle.WaitAll(handles);
            });
            thread.Start();
            thread.Join();
            return result.Value;
        }
    }
}