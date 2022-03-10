using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Models.Objects;
using Services.Services;
using W3WParser.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using Core.Models.Responses;
using System.Net.Http.Json;

namespace W3WParser.Controllers
{
    [ApiVersion("1",Deprecated = true)]
    [ApiController]
    [Route("/api/v{version:apiVersion}/[controller]")]
    public class ConvertController : ControllerBase
    {
        private ILogger<ConvertController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public FinalOutput finalOutput = new();

        public ConvertController(ILogger<ConvertController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [ApiExplorerSettings(IgnoreApi =true)]
        public Conversion CreateConversion(string id, W3WAddress w3wAddress, string postcode, string latLong)
        {
            Conversion conversion = new();
            conversion.Id = id;
            conversion.Addresses = new();

            Addresses addresses = new();
            addresses.Postcode = postcode;
            addresses.W3WAddress = w3wAddress;
            addresses.LatLong = latLong;

            conversion.Addresses.Add(addresses);
            return conversion;
        }

        [HttpPost]
        //[Route("/api/v{version:apiVersion}/conversion")]
        public async Task<IActionResult> PostInput([FromBody] InputFromBody body)
        {
            _logger.LogInformation("Trying to parse data...");

            try
            {
                CSVReader reader = new();
                var received = reader.ReadCsvFile(body);
                Output output = new();

                try
                {
                    output.Postcodes = new();
                    output.Uids = new();
                    output.Coords = new();
                    output.W3WAddress = new();

                    finalOutput.Conversion = new();

                    for (var i = 0; i < received.Postcodes.Count; i++)
                    {    
                        var coord = await GetLatLongFromGoogle(received.Postcodes[i]);
                        var w3w = await GetW3W(coord.Lat, coord.Lng);
                        string latLong = $"{coord.Lat},{coord.Lng}";
                        var tempFinalOutput = CreateConversion(received.Uids[i],w3w, received.Postcodes[i], latLong);
                        finalOutput.Conversion.Add(tempFinalOutput);
                        
                        /*
                        var coord = await GetLatLongFromGoogle(received.Postcodes[i]);
                        output.Postcodes.Add(received.Postcodes[i]);
                        output.Uids.Add(received.Uids[i]);
                        output.Coords.Add(coord);
                        var w3w = await GetW3W(output.Coords[i].Lat, output.Coords[i].Lng);
                        output.W3WAddress.Add(w3w);
                        */
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
                var output = "No values provided";
                return BadRequest(new[] {
                    output
                });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<Output.Coord> GetLatLongFromGoogle(string pc)
        {
            string GOOGLE_API_KEY = new GoogleApiWebService().GetGoogleApiKey();
            string POSTCODE = pc;


            Output.Coord coords = new();
            var httpClient = _httpClientFactory.CreateClient("googleApiClient");
            var response = await httpClient.GetFromJsonAsync<GoogleResponse.Root>(
                    $"json?input={POSTCODE}&inputtype=textquery&fields=geometry&key={GOOGLE_API_KEY}"
                );
            coords.Lat = response.candidates[0].geometry.location.lat.ToString();
            coords.Lng = response.candidates[0].geometry.location.lng.ToString();

            return coords;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<W3WAddress> GetW3W(string lat, string lng)
        {
            string W3W_API_KEY = new W3WApiService().GetApiKey();

            var httpClient = _httpClientFactory.CreateClient("w3wApiClient");
            var response = await httpClient.GetFromJsonAsync<W3WResponse.Root>(
                    $"v3/convert-to-3wa?coordinates={lat}%2C{lng}&key={W3W_API_KEY}"
                );


            W3WAddress address = new();
            address.Address = response.words;


            return address;
        }
        
}
    


}