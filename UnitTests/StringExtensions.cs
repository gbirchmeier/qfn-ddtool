using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    internal static class StringExtensions
    {
        internal static Task<string> GetResourceStringAsync<TType>(this string resourceName)
        {
            var assembly = typeof(TType).Assembly;
            var resourceStream = assembly.GetManifestResourceStream(resourceName);
            using (var streamReader = new StreamReader(resourceStream, Encoding.UTF8))
            return streamReader.ReadToEndAsync();
        }
    }
}
