using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Models.Objects;
using Services.Services;

namespace W3WParser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InputController : ControllerBase
    {
        private ILogger<InputController> _logger;

        public InputController(ILogger<InputController> logger)
        {
            _logger = logger;
        }
        [HttpPost]
        [Route("/api/input")]
        public IActionResult PostInput([FromBody] InputFromBody body)
        {
            _logger.LogInformation("Trying to parse data...");

            //return Ok();

            try
            {
                CSVReader reader = new();
                var output = reader.ReadCsvFile(body);
                return Ok(new[]
                {
                    output
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                var output = "No values provided";
                return BadRequest(new[] {
                    output
                });
            }

            
        }
    }

    

    public class InputTestResponse
    {
        public string Text { get; set; }
    }
}