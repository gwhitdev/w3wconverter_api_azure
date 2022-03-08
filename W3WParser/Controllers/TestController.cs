using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace W3WParser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("/api/test")]
        public IEnumerable<TestResponse> GetTest()
        {
            _logger.LogInformation("Received test GET request");

            return new[]
            {
                    new TestResponse { Text = "Test response"}
                };
        }
    }

    public class TestResponse
    {
        public string Text { get; set; }
    }
}