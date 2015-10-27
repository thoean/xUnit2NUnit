using System;
using Microsoft.AspNet.Mvc;
using Service.Data;

namespace Service.Controllers
{
    [Route("[controller]")]
    public class ConversionResultController : Controller
    {
        private readonly IConversionRepository repository;

        public ConversionResultController(IConversionRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{guid}", Name = "ConversionResult")]
        public IActionResult GetResult(Guid guid)
        {
            var result = repository.GetResult(guid);
            return result == null ? (IActionResult) HttpNotFound() : new ObjectResult(result);
        }
    }
}