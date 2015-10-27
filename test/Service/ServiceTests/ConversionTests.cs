using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Service.ServiceTests
{
    public class ConversionTests
    {
        [Fact]
        public async Task SimpleConversionCompletesSuccessfully()
        {
            var xmlToConvert = await SampleDataProvider.GetResourceAsync("Sample-xUnitResult.xml");
            
            // placing the request
            var data = new {XUnitResult = xmlToConvert};
            var conversionRequestResult = await RestRequestUtils.PostAsync("ConversionRequest", data);
            conversionRequestResult.EnsureSuccessStatusCode();

            var location = conversionRequestResult.Headers.Location.PathAndQuery;
            var timeout = DateTime.UtcNow.AddSeconds(10);
            while (DateTime.UtcNow < timeout)
            {
                var statusResponse = await RestRequestUtils.GetAsync(location);

                // wait until the request has been processed and retry
                if (!statusResponse.IsSuccessStatusCode)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.1));
                    continue;
                }

                // success in getting the result; investigating valid data
                var nUnitData = await statusResponse.Content.ReadAsStringAsync();
                var nUnitDataObj = new { NUnitResult = string.Empty};
                nUnitDataObj = JsonConvert.DeserializeAnonymousType(nUnitData, nUnitDataObj);
                var expectedResult = await SampleDataProvider.GetResourceAsync("Sample-nUnitResult.xml");
                Assert.Equal(expectedResult, nUnitDataObj.NUnitResult);
                Console.WriteLine("test");
                return;
            }
            Assert.True(false, "Timeout reached, failing test.");
        }
    }
}
