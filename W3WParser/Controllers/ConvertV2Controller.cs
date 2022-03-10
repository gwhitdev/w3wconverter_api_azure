using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Models.Objects;
using Services.Services;
using W3WParser.Services;
using System.Net.Http;
using Core.Models.Responses;
using System.Net.Http.Json;

namespace W3WParser.Controllers
{
    [ApiVersion("2")]
    [ApiController]
    [Route("/api/v{version:apiVersion}/convert")]
    [Produces("application/json")]
    public class ConvertV2Controller : ControllerBase
    {
        private ILogger<ConvertV2Controller> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public FinalOutput finalOutput = new();

        public ConvertV2Controller(ILogger<ConvertV2Controller> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private static Conversion CreateConversion(string id, W3WAddress w3wAddress, string postcode, Coords latLong)
        {
            Conversion conversion = new();
            conversion.Id = id;
            conversion.Addresses = new();

            Addresses addresses = new();
            addresses.Postcode = postcode;
            addresses.W3WAddress = w3wAddress;
            addresses.LatLong = $"{latLong.Lat},{latLong.Lng}";

            conversion.Addresses.Add(addresses);
            return conversion;
        }

        /// <summary>
        /// Receives form body data as JSON that includes comma-separated submitted postcodes and their Ids.
        /// </summary>
        /// <param name="body"></param>
        /// <returns>A JSON object with the converted What3Words address, Google API converted Lat/Long coordinates and the original postcode and Id.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /convert
        ///     {
        ///         "uids": "01,02,03",
        ///         "postcodes": "ng322ls,ll296dg,ng312ls"
        ///     }
        /// </remarks>
        /// <response code="200">Returns a OK response with the computed data</response>
        /// <response code="400">Returns a bad request response if the amount of postcodes provided equals 0.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FinalOutput>> PostInputV2([FromBody] InputFromBody body)
        {
            _logger.LogInformation("Trying to parse data... in API v2.0");

            try
            {
                CSVReader receivedBody = new(body);
                var csv = receivedBody.ReadCsv();

                try
                {
                    finalOutput.Conversion = new();

                    for (var i = 0; i < csv.Postcodes.Count; i++)
                    {
                        var coords = await GetLatLongFromGoogle(csv.Postcodes[i]);
                        var w3w = await GetW3W(coords.Lat, coords.Lng);
                        finalOutput.Conversion.Add(CreateConversion(csv.Uids[i], w3w, csv.Postcodes[i], coords));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message);
                }

                return Ok(new[]
                {
                    finalOutput
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new[] {
                    "No values provided"
                });
            }


        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<Coords> GetLatLongFromGoogle(string postcode)
        {
            var httpClient = _httpClientFactory.CreateClient("googleApiClient");
            GoogleApiWebService googleService = new(httpClient, postcode);
            return await googleService.Get();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<W3WAddress> GetW3W(string lat, string lng)
        {
            var httpClient = _httpClientFactory.CreateClient("w3wApiClient");
            W3WApiService w3wService = new(httpClient, lat, lng);
            return await w3wService.Get();
        }

    }



}