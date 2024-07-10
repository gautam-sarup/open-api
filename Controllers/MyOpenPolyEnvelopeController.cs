using System;

using Microsoft.AspNetCore.Mvc;

namespace OpenAPI2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyOpenPolyEnvelopeController : ControllerBase
    {
        private readonly ILogger<MyOpenPolyEnvelopeController> _logger;

        public MyOpenPolyEnvelopeController(ILogger<MyOpenPolyEnvelopeController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetMyOpenPolyData")]
        public IEnumerable<MyOpenPolyEnvelopeData> Get()
        {
            return Enumerable.Range(1, 2).Select(index => new MyOpenPolyEnvelopeData
            {
                Id = index,
                Name = "OpenPolytechnic",
                Description = "...is awesome"
            })
            .ToArray();
        }
    }
}
