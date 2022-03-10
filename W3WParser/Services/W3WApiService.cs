using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Core.Models.Objects;
using Core.Models.Responses;
using Microsoft.Extensions.Configuration;

namespace W3WParser.Services
{
    public class W3WApiService
    {
        public HttpClient Client { get; }
        public string Lat { get; }
        public string Lng { get; }
        private string Key { get; set; }

        public W3WApiService(HttpClient client, string lat, string lng)
        {
            Client = client;
            Lat = lat;
            Lng = lng;
            Key = GetApiKey();
        }

        private static string GetApiKey()
        {
            return Startup.Configuration.GetValue<string>("APIKeys:w3w");
        }

        public async Task<W3WAddress>Get()
        {
            var response = await Client.GetFromJsonAsync<W3WResponse.Root>(
                    $"v3/convert-to-3wa?coordinates={Lat}%2C{Lng}&key={Key}"
                );
            return Convert(response);
        }

        private static W3WAddress Convert(W3WResponse.Root response)
        {
            W3WAddress address = new();
            address.Address = response.words;
            return address;
        }


    }
}
