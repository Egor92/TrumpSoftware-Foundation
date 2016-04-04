using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace TrumpSoftware.Common.Tests
{
    public static class AsyncAssert
    {
        public static async Task<T> ThrowsExceptionAsync<T>(Func<Task> task) 
            where T : Exception
        {
            try
            {
                await task();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(T));
                return (T)ex;
            }
 
            if (typeof(T) == typeof(Exception))
                Assert.Fail("Expected exception but no exception was thrown.");
            else
                Assert.Fail("Expected exception of type {0} but no exception was thrown.", typeof(T));
 
            return null;
        }
    }
}
