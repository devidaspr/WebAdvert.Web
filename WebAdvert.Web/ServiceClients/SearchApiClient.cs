using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebAdvert.Web.Models;

namespace WebAdvert.Web.ServiceClients
{
    public class SearchApiClient : ISearchApiClient
    {
        private readonly HttpClient _client;
        private readonly string BaseAddress = string.Empty;

        public SearchApiClient(HttpClient client, IConfiguration configuration)
        {
            this._client = client;

            BaseAddress = configuration.GetSection("SearchApi").GetValue<string>("url");
        }

        public async Task<List<AdvertType>> Search(string keyword)
        {
            var result = new List<AdvertType>();
            var callUrl = $"{BaseAddress}/search/v1/{keyword}";
            var httpResponse = await _client.GetAsync(new Uri(callUrl));

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var allAdverts = JsonConvert.DeserializeObject<List<AdvertType>>(responseContent);
                
                result.AddRange(allAdverts);
            }

            return result;
        }
    }
}
