using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Zergatul.Test.Common
{
    public static class Assert2
    {
        public static void ThrowsException<T>(Action action, Func<T, bool> condition)
            where T : Exception
        {
            try
            {
                action();
            }
            catch (T exception) when (condition(exception))
            {
                return;
            }

            Assert.Fail();
        }
    }
}