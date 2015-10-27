using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Service.ServiceTests
{
    public class SampleDataProvider
    {
        public static async Task<string> GetResourceAsync(string filename)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"ServiceTests.Resources.{filename}"))
            {
                Assert.NotNull(stream);
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}