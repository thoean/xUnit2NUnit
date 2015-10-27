using Microsoft.AspNet.Mvc;
using Service.Contracts;
using Service.Flow;

namespace Service.Controllers
{
    [Route("[controller]")]
    public class ConversionRequestController : Controller
    {
        private readonly IConversionScheduler converter;

        public ConversionRequestController(IConversionScheduler converter)
        {
            this.converter = converter;
        }

        [HttpPost]
        public IActionResult Post([FromBody]XUnitData xUnitData)
        {
            var id = converter.ScheduleConversion(xUnitData);
            return CreatedAtRoute("ConversionResult", new {guid = id}, null);
        }
    }
}
