using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Core.Models.Responses;
using System.Net.Http.Json;
using Services.Services;
using Core.Models.Objects;

namespace W3WParser.Services
{
    public class GoogleApiWebService
    {
        public string Postcode { get; set; }
        public string Key { get; set; }
        public HttpClient Client { get; set; }

        public GoogleApiWebService(HttpClient client, string postcode)
        {
            Postcode = postcode;
            Key = GetGoogleApiKey();
            Client = client;
        }

        private static string GetGoogleApiKey()
        {
            return Startup.Configuration.GetValue<string>("APIKeys:googleApi");
        }

        public async Task<Coords> Get()
        {
            var response = await Client.GetFromJsonAsync<GoogleResponse.Root>(
                    $"json?input={Postcode}&inputtype=textquery&fields=geometry&key={Key}");
            return Convert(response);

        }

        private static Coords Convert(GoogleResponse.Root response)
        {
            GoogleCoordConverter converter = new(
                        response.candidates[0].geometry.location.lat.ToString(),
                        response.candidates[0].geometry.location.lng.ToString()
                    );

            return converter.Convert();
        }
    }

    

}
